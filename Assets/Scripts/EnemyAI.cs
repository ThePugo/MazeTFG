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

        //condici�n para cambiar al estado Chase
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

            //el enemigo tiene un rango de audici�n b�sico para o�r al jugador caminando
            if (playerFootsteps.footstepsSound.enabled)
            {
                hearingRange = viewDistance-5;
            }
            //el rango de audici�n se extiende si el jugador est� corriendo
            else if (playerFootsteps.sprintSound.enabled)
            {
                hearingRange = viewDistance;
            }
            else
            {
                //el jugador no hace ruido:
                return false;
            }

            //comprueba si el jugador est� dentro del rango de audici�n
            if (distanceToPlayer <= hearingRange)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPlayerInSight()
    {
        //direcci�n del enemigo al jugador
        Vector3 directionToPlayer = player.position - transform.position;
        //�ngulo entre el frente del enemigo y la direcci�n al jugador
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        //si el jugador est� dentro del �ngulo de visi�n y a la distancia de visi�n, entonces el jugador est� a la vista
        if (angleToPlayer < viewAngle / 2 && directionToPlayer.magnitude <= viewDistance)
        {
            //comprobaci�n adicional para asegurarse de que no hay obst�culos en medio
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, viewDistance))
            {
                //comprueba si el raycast golpe� al jugador
                if (hit.transform == player)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void Chase()
    {
        LookAtPlayer();
<<<<<<< HEAD
        agent.speed = 6;
=======
        agent.speed = 5;
>>>>>>> 99f4bf326fc5e7b8e4e196ac5ce6920eeed012a6
        agent.destination = player.position;

        //condici�n para cambiar al estado Attack
        if (Vector3.Distance(player.position, transform.position) < 2f)
        {
            agent.isStopped = true;
            currentState = State.Attack;
        }

        //condici�n para volver a Patrol si el jugador se aleja demasiado y sale del campo de visi�n
        if (!IsPlayerInSight() && Vector3.Distance(player.position, transform.position) > 10f)
        {
            currentState = State.Patrol;
        }
    }

    private void LookAtPlayer()
    {
        //mira hacia el jugador con una rotaci�n suave
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            //suaviza la rotaci�n hacia el jugador
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private IEnumerator Attack()
    {
        LookAtPlayer();
        isAttacking = true;
        animator.SetTrigger("Attack");

        // Espera un tiempo espec�fico antes de activar el collider, ajusta este tiempo para sincronizar con tu animaci�n
        float activationDelay = attackDelay * 0.4f; // Ajusta este factor para activar el collider antes en la animaci�n
        yield return new WaitForSeconds(activationDelay);
        axeCollider.enabled = true;

        // Espera el tiempo restante de la animaci�n antes de continuar
        yield return new WaitForSeconds(attackDelay - activationDelay);
        axeCollider.enabled = false; // Desactiva el collider despu�s de completar el ataque

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
        // Comprueba si el objeto con el que colision� es el jugador
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene("GameOver");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

