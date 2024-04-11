using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase,
        Attack
    }
    [SerializeField] private Vector3[] patrolPoints;
    private int currentPatrolIndex;
    private NavMeshAgent agent;
    private Transform player;
    public GameObject axe;
    private BoxCollider axeCollider;
    public State currentState;
    private bool isAttacking = false;
    private int eyeHeight = 2;
    public float attackDelay = 1.5f;
    private float viewDistance = 10f;
    private float viewAngle = 60f;
    public Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        currentState = State.Patrol;
        axeCollider = axe.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                animator.SetBool("Patrol",true);
                animator.SetBool("Chase", false);
                animator.SetBool("Attack", false);
                Patrol();
                break;
            case State.Chase:
                animator.SetBool("Patrol", false);
                animator.SetBool("Chase", true);
                animator.SetBool("Attack", false);
                Chase();
                break;
            case State.Attack:
                animator.SetBool("Patrol", false);
                animator.SetBool("Chase", false);
                animator.SetBool("Attack", true);
                if (!isAttacking)
                {
                    StartCoroutine(Attack());
                }
                break;
        }
    }

    private void Patrol()
    {
        agent.speed = 4;
        if (agent.destination != patrolPoints[currentPatrolIndex])
        {
            agent.destination = patrolPoints[currentPatrolIndex];
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[currentPatrolIndex];
        }

        //condición para cambiar al estado Chase
        if (IsPlayerInSight())
        {
            currentState = State.Chase;
        }

        if (CanHearPlayer())
        {
            currentState = State.Chase;
        }
    }

    private bool CanHearPlayer()
    {
        Footsteps playerFootsteps = player.GetComponent<Footsteps>();
        if (playerFootsteps != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            float hearingRange;

            //el enemigo tiene un rango de audición básico para oír al jugador caminando
            if (playerFootsteps.footstepsSound.enabled)
            {
                hearingRange = viewDistance-5;
            }
            //el rango de audición se extiende si el jugador está corriendo
            else if (playerFootsteps.sprintSound.enabled)
            {
                hearingRange = viewDistance;
            }
            else
            {
                //el jugador no hace ruido:
                return false;
            }

            //comprueba si el jugador está dentro del rango de audición
            if (distanceToPlayer <= hearingRange)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPlayerInSight()
    {
        // Ajuste la altura de inicio del rayo a la altura de los ojos del enemigo
        Vector3 rayStart = transform.position + Vector3.up * 2.0f;  // eyeHeight es 2

        // Calcular la dirección exacta desde el enemigo hacia el jugador
        Vector3 directionToPlayer = (player.position - rayStart).normalized;

        // Asegúrate de que el raycast no pase a través de las paredes
        int layerMask = 1 << LayerMask.NameToLayer("Wall");  // Reemplaza "Wall" con la capa de tus paredes
        layerMask = ~layerMask;  // Invierte la máscara para incluir todas las capas excepto las paredes

        // Comprobación para detectar al jugador en medio
        RaycastHit hit;
        if (Physics.Raycast(rayStart, directionToPlayer, out hit, viewDistance, layerMask))
        {
            // Comprueba si el raycast golpeó al jugador
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }





    private void Chase()
    {
        LookAtPlayer();
        agent.speed = 6;
        agent.destination = player.position;

        //condición para cambiar al estado Attack
        if (Vector3.Distance(player.position, transform.position) < 2f)
        {
            agent.isStopped = true;
            currentState = State.Attack;
        }

        //condición para volver a Patrol si el jugador se aleja demasiado y sale del campo de visión
        if (!IsPlayerInSight() && Vector3.Distance(player.position, transform.position) > 10f)
        {
            currentState = State.Patrol;
        }
    }

    private void LookAtPlayer()
    {
        //mira hacia el jugador con una rotación suave
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            //suaviza la rotación hacia el jugador
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private IEnumerator Attack()
    {
        LookAtPlayer();
        isAttacking = true;
        animator.SetTrigger("Attack");

        // Espera un tiempo específico antes de activar el collider, ajusta este tiempo para sincronizar con tu animación
        float activationDelay = attackDelay * 0.4f; // Ajusta este factor para activar el collider antes en la animación
        yield return new WaitForSeconds(activationDelay);
        axeCollider.enabled = true;

        // Espera el tiempo restante de la animación antes de continuar
        yield return new WaitForSeconds(attackDelay - activationDelay);
        axeCollider.enabled = false; // Desactiva el collider después de completar el ataque

        // Restablece el estado
        isAttacking = false;
        if (Vector3.Distance(player.position, transform.position) > 2f)
        {
            agent.isStopped = false;
            currentState = State.Chase;
        }
    }


    public void SetPatrolPoints(Vector3[] newPatrolPoints)
    {
        patrolPoints = newPatrolPoints;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Comprueba si el objeto con el que colisionó es el jugador
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene("GameOver");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

