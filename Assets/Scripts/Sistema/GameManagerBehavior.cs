using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Serve per usare effetti come DepthOfField e FilmGrain
using UnityEngine.UI; // Serve per usare elementi UI come lo Slider

// Classe che gestisce lo stato globale del gioco (pausa, menu, impostazioni, ecc.)
public class GameManagerBehavior : MonoBehaviour
{
    // ===================== UI =====================
    
    // Riferimento al menu di pausa
    [SerializeField] GameObject pauseMenu;
    
    // Riferimento al menu delle impostazioni
    [SerializeField] GameObject settingsMenu;
    
    // Slider usato per controllare l'intensità della neve
    [SerializeField] Slider valoreSlider;
    
    // Valore salvato dell'intensità della neve (default iniziale)
    [SerializeField] private float userSnowIntensity = 0.15f;

    
    // ===================== POST-PROCESSING =====================
    
    // Effetto profondità di campo (sfocatura)
    private DepthOfField _dof;
    
    // Effetto grana (qui usato per simulare la neve)
    private FilmGrain _filmGrain;


    // ===================== STATO GLOBALE =====================
    
    // Variabile statica: indica se il gioco è in pausa (accessibile da altri script)
    public static bool isPaused;


    // Metodo chiamato dallo Slider quando il valore cambia
    public void IntensitàNeve(float valoreSlider)
    {
        // Salva il nuovo valore scelto dall'utente
        userSnowIntensity = valoreSlider;

        // Se l'effetto è disponibile
        if(_filmGrain != null)
        {
            // Applica subito il cambiamento SOLO se il gioco non è in pausa
            if(!isPaused) 
                _filmGrain.intensity.Override(userSnowIntensity);
        }
    }


    // Metodo chiamato all'avvio della scena
    void Start()
    {
        // Disattiva i menu all'inizio (sicurezza)
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
    
        // Imposta lo stato iniziale del gioco (non in pausa)
        isPaused = false;
        Time.timeScale = 1f; // Il tempo scorre normalmente

        // Imposta lo slider al valore salvato
        if(valoreSlider != null) 
            valoreSlider.value = userSnowIntensity;
    }


    // Metodo chiamato dal sistema Input quando viene premuto il tasto pausa
    void OnPausa(InputValue value)
    {
        // Controlla se il tasto è stato premuto
        if (value.isPressed)
        {
            // Se NON siamo in pausa → metti in pausa
            if (!isPaused)
            {
                PauseGame();
            }
            // Se siamo già in pausa → riprendi il gioco
            else
            {
                ResumeGame();
            }
        }
    }


    // Update vuoto (attualmente non utilizzato)
    void Update() 
    {
    }


    // ===================== GESTIONE PAUSA =====================
    
    // Attiva la pausa
    private void PauseGame()
    {
        // Mostra il menu pausa
        pauseMenu.SetActive(true);
        
        // Aggiorna stato
        isPaused = true;
        
        // Ferma il tempo (tutto si blocca)
        Time.timeScale = 0f; 
        
        // Sblocca e mostra il cursore
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Attiva effetti visivi di pausa
        if (_dof) _dof.active = true;
        if (_filmGrain) _filmGrain.intensity.Override(0.05f);
    }


    // Disattiva la pausa
    public void ResumeGame()
    {
        // Nasconde i menu
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        
        // Aggiorna stato
        isPaused = false;
        
        // Riattiva il tempo
        Time.timeScale = 1f; 
        
        // Nasconde il cursore (tipico negli FPS)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        // Ripristina gli effetti visivi
        if (_dof) _dof.active = false;
        if (_filmGrain) _filmGrain.intensity.Override(userSnowIntensity);
    }


    // ===================== MENU IMPOSTAZIONI =====================
    
    // Apre il menu impostazioni
    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    // Chiude il menu impostazioni e torna alla pausa
    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }


    // ===================== SCENE E USCITA =====================
    
    // Torna al menu principale
    public void MainMenu()
    {
        // Assicura che il tempo sia normale prima di cambiare scena
        Time.timeScale = 1f;
        
        // Carica la scena "MainMenu"
        SceneManager.LoadScene("MainMenu");
    }


    // Chiude il gioco
    public void QuitGame()
    {
        // Funziona nella build finale del gioco
        Application.Quit();

        // Serve per fermare il gioco dentro l'Editor Unity
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}