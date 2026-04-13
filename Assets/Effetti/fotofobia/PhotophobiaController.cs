using UnityEngine;

// Questo script simula un effetto di "fotofobia" (fastidio alla luce).
// Quando il giocatore guarda un oggetto con tag "monitor", aumenta uno stress visivo.
// Questo valore viene passato a uno shader che applica effetti grafici (es. distorsione, grana, luminosità).
public class PhotophobiaController : MonoBehaviour
{
    // ===================== MATERIALI =====================
    
    [Header("Riferimenti Materiali")]
    
    // Materiale che contiene lo shader della fotofobia.
    // Deve avere una proprietà float chiamata "_StressVisivo".
    // Va assegnato manualmente dall'Inspector.
    public Material matFotofobia;


    // ===================== PARAMETRI MONITOR =====================
    
    [Header("Parametri Monitor")]

    // Tag utilizzato per identificare gli oggetti che rappresentano un monitor.
    // Permette di rendere lo script generico (non dipende da oggetti specifici).
    public string monitorTag = "Monitor";

    // Distanza massima del raycast (quanto lontano il giocatore può "guardare" un monitor).
    public float maxDistance = 5f;


    // ===================== STATO (DEBUG) =====================
    
    [Header("Stato Corrente (Debug)")]

    // Valore attuale dello stress visivo.
    // Range limita solo la visualizzazione nello slider dell'Inspector.
    [Range(0f, 1f)] public float currentFotofobia = 0f;


    // ===================== VELOCITÀ =====================
    
    [Header("Velocità")]

    // Velocità con cui lo stress aumenta quando si guarda il monitor
    public float speedIn = 0.5f;

    // Velocità con cui lo stress diminuisce quando si smette di guardare
    public float speedOut = 0.25f;


    // Metodo chiamato una volta all'avvio
    void Start() 
    {
        // Reset iniziale dello shader per evitare valori residui tra scene o play
        if (matFotofobia != null) 
        {
            matFotofobia.SetFloat("_StressVisivo", 0f);
            Debug.Log("Fotofobia resettata a 0!");
        }
    }


    // Metodo chiamato ogni frame
    void Update()
    {
        // =====================
        // 1. CONTROLLO SGUARDO (RAYCAST)
        // =====================

        // Indica se il giocatore sta guardando un monitor
        bool guardandoMonitor = false;

        // Creiamo un raggio dalla posizione del player in avanti
        Ray ray = new Ray(transform.position, transform.forward);

        // Variabile che conterrà le info della collisione
        RaycastHit hit;

        // Disegno del raggio (visibile solo nell'Editor)
        Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.green);

        // Lancio del raycast
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Controllo del tag dell'oggetto colpito
            if (hit.collider.CompareTag(monitorTag))
            {
                guardandoMonitor = true;
            }
        }


        // =====================
        // 2. AGGIORNAMENTO STRESS VISIVO
        // =====================

        if (guardandoMonitor)
        {
            // Aumenta gradualmente fino a un massimo (0.75)
            currentFotofobia = Mathf.MoveTowards(
                currentFotofobia,   // valore attuale
                0.75f,              // valore target
                speedIn * Time.deltaTime // velocità nel tempo
            );
        }
        else
        {
            // Diminuisce gradualmente fino a 0
            currentFotofobia = Mathf.MoveTowards(
                currentFotofobia,
                0f,
                speedOut * Time.deltaTime
            );
        }
        

        // =====================
        // 3. INVIO DATI ALLO SHADER
        // =====================

        // Aggiorna il valore nel materiale (se assegnato)
        if (matFotofobia != null)
        {
            matFotofobia.SetFloat("_StressVisivo", currentFotofobia);
        }
    }
}