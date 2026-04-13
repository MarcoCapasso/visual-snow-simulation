using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

// Questa classe definisce una "feature" del renderer URP.
// Serve per aggiungere un effetto palinopsia = scia visiva persistente.
public class Palinopsia : ScriptableRendererFeature
{
    // ===================== IMPOSTAZIONI SERIALIZZATE =====================
    
    [System.Serializable]
    public class PalinopsiaSettings
    {
        // Materiale che contiene lo shader dell'effetto
        public Material material;

        // Quando eseguire la pass nella pipeline di rendering
        // Dopo il post-processing evita problemi con HDR (schermo bianco/bruciato)
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    } 

    // Istanza delle impostazioni visibile nell'Inspector
    public PalinopsiaSettings settings = new PalinopsiaSettings();

    // Riferimento alla render pass vera e propria
    private PalinopsiaPass palinopsiaPass;


    // ===================== CREAZIONE DELLA FEATURE =====================
    
    public override void Create()
    {
        // Se non c'è il materiale, non ha senso creare la pass
        if (settings.material == null) return;

        // Creiamo la pass passando il materiale
        palinopsiaPass = new PalinopsiaPass(settings.material);

        // Impostiamo quando deve essere eseguita
        palinopsiaPass.renderPassEvent = settings.renderPassEvent;
    }


    // ===================== AGGIUNTA ALLA PIPELINE =====================
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Controlli di sicurezza
        if (settings.material == null || palinopsiaPass == null) return;

        // Evita esecuzione su camere di preview o riflessione (editor, specchi, ecc.)
        if (renderingData.cameraData.cameraType == CameraType.Preview || 
            renderingData.cameraData.cameraType == CameraType.Reflection) return;

        // Aggiunge la pass alla pipeline
        renderer.EnqueuePass(palinopsiaPass);
    }


    // ===================== PULIZIA =====================
    
    protected override void Dispose(bool disposing)
    {
        palinopsiaPass?.Dispose();
    }


    // ===================== RENDER PASS =====================
    
    // Classe interna che definisce il comportamento dell'effetto
    class PalinopsiaPass : ScriptableRenderPass
    {
        // Materiale usato per il rendering
        private Material m_Material;

        // Texture che memorizza il frame precedente (effetto scia)
        private RTHandle m_HistoryTexture;

        // Flag per capire se siamo al primo frame
        private bool m_ResetHistory = true;

        public PalinopsiaPass (Material material)
        {
            m_Material = material;

            // Richiede una texture intermedia per funzionare correttamente
            requiresIntermediateTexture = true;
        }

        // Rilascia la memoria della texture storica
        public void Dispose()
        {
            m_HistoryTexture?.Release();
        }


        // ===================== DATI DELLA PASS =====================
        
        // Struttura dati passata al RenderGraph
        private class PassData
        {
            public TextureHandle source;       // Frame corrente
            public TextureHandle history;      // Frame precedente
            public TextureHandle destination;  // Output della pass
            public Material material;
            public bool isFirstFrame;          // Serve per gestire il primo frame
        }


        // ===================== RENDER GRAPH =====================
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Recupera dati della pipeline
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            // Texture di input (frame corrente)
            TextureHandle sourceTexture = resourceData.activeColorTexture;
            if (!sourceTexture.IsValid()) return;

            // Descrizione della texture (risoluzione, formato, ecc.)
            RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
            desc.msaaSamples = 1;
            desc.depthBufferBits = 0;
            
            // Alloca o riusa la texture storica
            // Se viene riallocata (es. cambio risoluzione), resettiamo la scia
            bool reallocated = RenderingUtils.ReAllocateHandleIfNeeded(
                ref m_HistoryTexture, 
                desc, 
                FilterMode.Bilinear, 
                TextureWrapMode.Clamp, 
                name: "PalinopsiaHistoryTexture"
            );

            if (reallocated) m_ResetHistory = true;

            // Passiamo la texture storica allo shader
            m_Material.SetTexture("_HistoryTex", m_HistoryTexture);

            // Importiamo la texture nel RenderGraph
            TextureHandle historyHandle = renderGraph.ImportTexture(m_HistoryTexture);

            // Texture temporanea per il risultato
            TextureHandle tempTexture = UniversalRenderer.CreateRenderGraphTexture(
                renderGraph, desc, "PalinopsiaTempTexture", false
            );


            // =====================
            // PASS 1: BLEND (EFFETTO SCIA)
            // =====================
            
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Palinopsia_BlendPass", out var passData))
            {
                passData.source = sourceTexture;
                passData.history = historyHandle;
                passData.destination = tempTexture;
                passData.material = m_Material;
                passData.isFirstFrame = m_ResetHistory;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.UseTexture(passData.history, AccessFlags.Read);
                builder.SetRenderAttachment(passData.destination, 0, AccessFlags.Write);
                builder.AllowGlobalStateModification(true);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    if (data.isFirstFrame)
                    {
                        // PRIMO FRAME:
                        // Copia diretta senza effetto (evita artefatti)
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1,1,0,0), 0.0f, false);
                    }
                    else
                    {
                        // FRAME SUCCESSIVI:
                        // Applica lo shader che combina frame corrente + storico
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1,1,0,0), data.material, 0);
                    }
                });
            }


            // =====================
            // PASS 2: AGGIORNAMENTO STORIA
            // =====================
            
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Palinopsia_UpdateHistory", out var passData))
            {
                passData.source = tempTexture;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderAttachment(historyHandle, 0, AccessFlags.Write);

                builder.AllowGlobalStateModification(true);

                // IMPORTANTE: questa pass NON deve essere saltata
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // Copia il risultato nella texture storica
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1,1,0,0), 0.0f, false);
                });
            }


            // Imposta il risultato finale come output della camera
            resourceData.cameraColor = tempTexture;
            
            // Dopo il primo frame, disattiviamo il reset
            m_ResetHistory = false;
        }
    }
}