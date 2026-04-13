using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe principale che gestisce un controller in prima persona
public class FirstPersonController : MonoBehaviour
{
    // ===================== MOVIMENTO =====================
    [Header("Movement Speeds")]
    
    // Velocità base del personaggio quando cammina
    [SerializeField] private float walkSpeed = 3.0f;
    
    // Moltiplicatore della velocità quando si sprinta (es: 2 = il doppio della velocità)
    [SerializeField] private float sprintMultiplier = 2.0f;


    // ===================== SALTO =====================
    [Header("Jump Parameters")]
    
    // Forza applicata al salto (quanto in alto salta il personaggio)
    [SerializeField] private float jumpForce = 5.0f;
    
    // Moltiplicatore della gravità (serve per rendere il salto più o meno "pesante")
    [SerializeField] private float gravityMultiplier = 1.0f;


    // ===================== VISUALE / CAMERA =====================
    [Header("Look Parameters")]
    
    // Sensibilità del mouse (quanto velocemente ruota la visuale)
    [SerializeField] private float mouseSensitivity = 0.1f;
    
    // Limite massimo di rotazione verticale (evita che la camera si ribalti)
    [SerializeField] private float upDownLookRange = 80f;


    // ===================== RIFERIMENTI =====================
    [Header ("References")]
    
    // Riferimento al CharacterController (gestisce collisioni e movimento)
    [SerializeField] private CharacterController characterController;
    
    // Riferimento alla camera principale (serve per la rotazione verticale)
    [SerializeField] private Camera mainCamera;
    
    // Script che gestisce gli input del giocatore
    [SerializeField] private PlayerInputHandler playerInputHandler;


    // ===================== VARIABILI INTERNE =====================
    
    // Vettore che contiene il movimento attuale (x, y, z)
    private Vector3 currentMovement;
    
    // Rotazione verticale accumulata della camera (su/giù)
    private float verticalRotation;
    
    // Velocità attuale: aumenta se il giocatore sta sprintando
    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);


    // Metodo chiamato una volta all'avvio
    void Start()
    {
        // Blocca il cursore al centro dello schermo (tipico degli FPS)
        Cursor.lockState = CursorLockMode.Locked;
        
        // Nasconde il cursore
        Cursor.visible = false;
    }


    // Metodo chiamato ogni frame
    void Update()
    {
        // Esegue movimento e rotazione solo se il gioco NON è in pausa
        if (!GameManagerBehavior.isPaused)
        {
            HandleMovement();
            HandleRotation();
        }
    }


    // Converte l'input del giocatore (locale) in una direzione nello spazio globale
    private Vector3 CalculateWorldDirection()
    {
        // Input su asse X (sinistra/destra) e Z (avanti/indietro)
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        
        // Converte la direzione in base alla rotazione del player
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        
        // Normalizza per evitare velocità maggiori in diagonale
        return worldDirection.normalized;
    }


    // Gestisce la logica del salto e della gravità
    private void HandleJumping()
    {
        // Se il personaggio è a terra
        if (characterController.isGrounded)
        {
            // Mantiene il personaggio "incollato" al suolo
            currentMovement.y = -0.5f;

            // Se viene premuto il tasto salto
            if (playerInputHandler.JumpTriggered)
            {
                // Applica la forza verso l'alto
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            // Applica la gravità nel tempo (caduta)
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }


    // Gestisce il movimento del personaggio
    private void HandleMovement()
    {
        // Ottiene la direzione di movimento nello spazio
        Vector3 worldDirection = CalculateWorldDirection();
        
        // Applica la velocità sugli assi orizzontali
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        // Gestisce salto e gravità
        HandleJumping();
        
        // Muove il personaggio nel mondo
        characterController.Move(currentMovement * Time.deltaTime);
    }


    // Applica la rotazione orizzontale (destra/sinistra)
    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }


    // Applica la rotazione verticale (su/giù)
    private void ApplyVerticalRotation(float rotationAmount)
    {
        // Limita la rotazione per evitare rotazioni innaturali
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        
        // Applica la rotazione alla camera
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }


    // Gestisce la rotazione basata sull'input del mouse
    private void HandleRotation()
    {
        // Input orizzontale del mouse
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        
        // Input verticale del mouse
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        // Applica le rotazioni
        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }
}