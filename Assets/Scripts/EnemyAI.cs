using System.Collections;
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
    public State currentState;
    private bool isAttacking = false;
    public float attackDelay = 1.5f;
    private float viewDistance = 10f;
    private float viewAngle = 60f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        currentState = State.Patrol;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                if (!isAttacking)
                {
                    StartCoroutine(Attack());
                }
                break;
        }
    }

    private void Patrol()
    {
        print("Patrol");
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
    }

    private bool IsPlayerInSight()
    {
        //dirección del enemigo al jugador
        Vector3 directionToPlayer = player.position - transform.position;
        //ángulo entre el frente del enemigo y la dirección al jugador
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        //si el jugador está dentro del ángulo de visión y a la distancia de visión, entonces el jugador está a la vista
        if (angleToPlayer < viewAngle / 2 && directionToPlayer.magnitude <= viewDistance)
        {
            //comprobación adicional para asegurarse de que no hay obstáculos en medio
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, viewDistance))
            {
                //comprueba si el raycast golpeó al jugador
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
        print("Chase");
        LookAtPlayer();
        agent.destination = player.position;

        //condición para cambiar al estado Attack
        if (Vector3.Distance(player.position, transform.position) < 2f)
        {
            agent.destination = agent.destination;
            currentState = State.Attack;
        }

        //condición para volver a Patrol si el jugador se aleja demasiado y sale del campo de visión
        if (!IsPlayerInSight() && Vector3.Distance(player.position, transform.position) > 15f)
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
        print("Attack");
        isAttacking = true;

        // Aquí puedes agregar animaciones de ataque o efectos de sonido
        Debug.Log("Attacking player");

        //espera por el tiempo de delay antes del próximo ataque
        yield return new WaitForSeconds(attackDelay);

        //comprueba si el jugador aún está en rango de ataque después de esperar
        if (Vector3.Distance(player.position, transform.position) > 2f)
        {
            //si el jugador se ha alejado, vuelve al estado Chase
            currentState = State.Chase;
        }

        isAttacking = false;

    }

    public void SetPatrolPoints(Vector3[] newPatrolPoints)
    {
        patrolPoints = newPatrolPoints;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Comprueba si el objeto con el que colisionó es el jugador
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("GameOver");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

