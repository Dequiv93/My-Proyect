using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CharacterCrontroller : MonoBehaviour
{
    
    //Variables Globales
    [SerializeField] private float velocidadCaminar = 6f;
    [SerializeField] private float velocidadTrotar = 12f;
    [SerializeField] private float velocidadCorrer = 20f;
    [SerializeField] private float velocidadDash = 30f;
    [SerializeField] private float distanciaDash = 10f;
    [SerializeField] private float cooldownDash = 2f;
    public float moveSpeed = 10f; // Velocidad del jugador
    public float jumpForce = 5f; // Fuerza aplicada al saltar
    public float gravityScale = 2f; // Scale of gravity applied to the character
    public Transform groundCheck; // Transform to check if the player is on the ground
    public LayerMask groundLayer; // Layer that represents the ground
    public float groundCheckRadius = 0.1f; // Radius for ground check

    //Dash

    [SerializeField] private float duracionDash = 0.2f;
    [SerializeField] private float tiempoDash = 15f;

    
    private float siguienteDash = 0f;

    private float velocidadActual;
    public float attackRange = 5.0f;
    [SerializeField] private Transform attackPosition;
    public float detectionRange = 10f;

    public int maxHealth = 100;

    int currentHealth;

    Vector3 movimiento;
    Vector3 dashTarget;

    private bool isDashing = false;
    private Rigidbody rb;
    [SerializeField] private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    public Transform enemy;
    public bool isDead = false;
    public int heavyAttackDamage = 30; // Damage dealt by heavy attack



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocidadActual = velocidadTrotar;
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        // Handle movement
        float InputHorizontal = Input.GetAxis("Horizontal");
        float InputVertical = Input.GetAxis("Vertical");

        movimiento = Quaternion.Euler(0, 45 , 0) * new Vector3(InputHorizontal, 0f, InputVertical).normalized;

        // Handle dashing
        if (Input.GetKeyDown(KeyCode.D) && !isDashing && Time.time > siguienteDash)
        {
            isDashing = true;
            animator.SetTrigger("Dash"); // Reiniciar la animación
            dashTarget = transform.position + movimiento * distanciaDash;
            siguienteDash = Time.time + cooldownDash;
        }


        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidadActual = velocidadCorrer;
            animator.SetBool("isRunning", true); // set running animation
            animator.SetBool("isWalking", false); // ensure walking is flase 
        }
        else if (Input.GetKey(KeyCode.LeftControl)) 
        {
            velocidadActual = velocidadCaminar;
            animator.SetBool("isWalking", true); // Set walking animation
            animator.SetBool("isRunning", false); // Ensure running is false
        }
        else if (movimiento != Vector3.zero)
        {
            velocidadActual = velocidadTrotar;
            animator.SetBool("isWalking", true); // Set walking animation
            animator.SetBool("isRunning", false); // Ensure running is false
        }
        else
        {
            // Stop walking and running animations
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
        
        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, velocidadDash * Time.deltaTime);
            if(Vector3.Distance(transform.position, dashTarget) < 0.1f)
            {
                isDashing = false;
            }

        }

        else if (movimiento != Vector3.zero)
        {
            rb.AddForce(movimiento * velocidadActual, ForceMode.VelocityChange);
            // Rotate the character to face the direction of movement
            Quaternion targetRotation = Quaternion.LookRotation(movimiento);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Adjust the speed of rotation as needed
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Attack();
            Debug.Log("Atacando");
        }
        
        float distance = Vector3.Distance(transform.position, enemy.position);
        if (distance < attackRange)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null && !enemyController.isDead)
            {
                Attack();
            }
        }
        // Handle jumping
        if (isGrounded && Input.GetButtonDown("Jump")) // Default jump key is Jump
        {
            Jump();
            Debug.Log("Brincando");
        }

        // Update animator parameters
        animator.SetBool("isJumping", !isGrounded);
        // Reset isJumping when grounded
        if (isGrounded && isJumping)
        {
            isJumping = false; // Reset jump state when landing
        }

    }
    void Jump()
    {
        // Apply jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;
        animator.SetTrigger("Jump"); // Trigger jump animation
    }
    void FixedUpdate()
    {
        // Apply gravity
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
        }
    }

    void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPosition.position, attackRange);
        foreach (var enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null && !enemyController.isDead) // Solo atacar si no está muerto.
            {
                enemyController.TakeDamage(25);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // No hacer nada si ya está muerto.
        currentHealth -= damage;
        Debug.Log(" Player took " + damage + " damage.");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true; // Marcar al enemigo como muerto.
            Died();
        }
    }

    void Died()
    {
        Debug.Log("Player died.");
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // Trigger walikng animation
            animator.SetBool("isWalking", true);
            Debug.Log("Caminando");
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // Stop walikng animation
            animator.SetBool("isWalking", false);
            Debug.Log("Deteniendo");
        }
    }
}

