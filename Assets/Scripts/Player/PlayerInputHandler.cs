using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Classe che gestisce tutti gli input del giocatore usando il nuovo Input System
public class PlayerInputHandler : MonoBehaviour
{
    // ===================== CONFIGURAZIONE INPUT =====================
    
    [Header("Input Action Asset")]
    
    // Asset che contiene tutte le Input Actions (creato in Unity)
    [SerializeField] private InputActionAsset playerControls;


    [Header("Action Map Name Reference")]
    
    // Nome della Action Map (gruppo di input, es: "Player")
    [SerializeField] private string actionMapName = "Player";


    [Header("Action Name References")]
    
    // Nome delle singole azioni definite nell'Input System
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";


    // ===================== RIFERIMENTI ALLE AZIONI =====================
    
    // Variabili che conterranno le azioni effettive trovate nell'asset
    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;


    // ===================== OUTPUT (LETTE DA ALTRI SCRIPT) =====================
    
    // Input di movimento (WASD / analogico)
    public Vector2 MovementInput { get; private set; }
    
    // Input di rotazione (mouse / stick destro)
    public Vector2 RotationInput { get; private set; }
    
    // True mentre il tasto salto è premuto
    public bool JumpTriggered { get; private set; }
    
    // True mentre il tasto sprint è premuto
    public bool SprintTriggered { get; private set; }


    // Metodo chiamato quando lo script viene inizializzato
    private void Awake()
    {
        // Recupera la Action Map dall'asset usando il nome
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);

        // Recupera le singole azioni dalla Action Map
        movementAction = mapReference.FindAction(movement);
        rotationAction = mapReference.FindAction(rotation);
        jumpAction = mapReference.FindAction(jump);
        sprintAction = mapReference.FindAction(sprint);

        // Collega gli eventi delle azioni alle variabili
        SubscribeActionValuesToInputEvents();
    }


    // Collega gli eventi delle Input Actions alle variabili locali
    private void SubscribeActionValuesToInputEvents()
    {
        // ===================== MOVIMENTO =====================
        
        // Quando viene rilevato input → aggiorna il valore
        movementAction.performed += inputInfo => 
            MovementInput = inputInfo.ReadValue<Vector2>();
        
        // Quando l'input termina → azzera
        movementAction.canceled += inputInfo => 
            MovementInput = Vector2.zero;


        // ===================== ROTAZIONE =====================
        
        rotationAction.performed += inputInfo => 
            RotationInput = inputInfo.ReadValue<Vector2>();
        
        rotationAction.canceled += inputInfo => 
            RotationInput = Vector2.zero;


        // ===================== SALTO =====================
        
        // Quando premi il tasto → true
        jumpAction.performed += inputInfo => 
            JumpTriggered = true;
        
        // Quando rilasci → false
        jumpAction.canceled += inputInfo => 
            JumpTriggered = false;


        // ===================== SPRINT =====================
        
        sprintAction.performed += inputInfo => 
            SprintTriggered = true;
        
        sprintAction.canceled += inputInfo => 
            SprintTriggered = false;
    }


    // Attiva le Input Actions quando l'oggetto viene abilitato
    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }


    // Disattiva le Input Actions quando l'oggetto viene disabilitato
    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }
}