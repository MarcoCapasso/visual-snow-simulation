using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class cambioScena : MonoBehaviour
{ 
    [SerializeField]  private InputActionReference playerInputs;
    private bool playerInRange = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerInRange)
        {
            
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnEnable()
    {
        if (playerInputs != null)
        {
            playerInputs.action.performed += OnPlayerInput;
        }
        
    }

    private void OnDisable()
    {
        if (playerInputs != null)
        {
            playerInputs.action.performed -= OnPlayerInput;
        }
    }

    private void OnPlayerInput(InputAction.CallbackContext context)
    {
        if (playerInRange)
        {
            SceneManager.LoadScene("corridoio"); // Sostituisci "NomeDellaScena" con il nome effettivo della scena che vuoi caricare
        }
    }
}
