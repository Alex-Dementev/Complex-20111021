using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public AudioEvent AudioEvent;
    public Transform playerTransform;
    public Transform ObjectTransform;
    private NavMeshAgent agent;
    public Animator animBody;
    public Animator animHead;
    private Rigidbody rb;

    private string Biome = "Biome02";

    public AudioSource AudioSource;

    [Header("Jump Settings")]
    public float jumpForwardForce;
    public float jumpUpForce;
    public float checkDistance;
    public LayerMask groundMask;

    private float WaitingJump;
    private float Waiting;
    private int IsAttached;
    private bool isWaitingWalk = false;
    private Rigidbody PlayerRb;

    private float WalkSpeed;
    private float RunSpeed;

    private float DelayScream;
    private float DelayRoar;

    private int stateAnimBody;
    private int oldStateAnimBody;

    public int ID;

    private bool Saved = true;
    private bool Load;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        PlayerRb = playerTransform.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        DelayScream = Random.Range(16, 55);
        DelayRoar = Random.Range(4, 10);

        Systems.VisibilityLevel += VisibilityLevels;

        VisibilityLevels(SystemsController.CurrentVisibilityLevel);
    }

    private void VisibilityLevels(int level)
    {
        switch(level)
        {
            case 0:
                WalkSpeed = 2.5f;
                RunSpeed = 3.2f;
                break;
            case 1:
                WalkSpeed = 3f;
                RunSpeed = 3.8f;
                break;
            case 2:
                WalkSpeed = 4f;
                RunSpeed = 4.5f;
                break;
            case 3:
                WalkSpeed = 4.5f;
                RunSpeed = 5f;
                break;
        }
    }

    private void OnDestroy()
    {
        Systems.VisibilityLevel -= VisibilityLevels;
    }

    void Update()
    {
        DelayScream -= Time.deltaTime;
        WaitingJump -= Time.deltaTime;
        Waiting -= Time.deltaTime;


        if(CenterSpawnedObjects.Load)
        {
            if(!Load)
            {
                Load = true;

                if(CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedBuilds] == new Vector3(0, 0, 0)) return;

                transform.position = CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedBuilds];
                transform.eulerAngles = CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedBuilds];
                agent.SetDestination(CenterSpawnedObjects.Instance.EnemyPoint[ID]);
            }

            if(PauseController.InvisibleOperations && !Saved)
            {
                Saved = true;
                CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedBuilds] = transform.position;
                CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedBuilds] = transform.eulerAngles;
            }
            else if(!PauseController.InvisibleOperations)
                Saved = false;
        }
        else return;


        if(stateAnimBody != oldStateAnimBody)
        {
            oldStateAnimBody = stateAnimBody;

            switch(stateAnimBody)
            {
                case 0:
                    animBody.CrossFade("Idle", 0.1f);
                    break;
                case 1:
                    animBody.CrossFade("Jump", 0.1f);
                    break;
                case 2:
                    animBody.CrossFade("Walk", 0.1f);
                    break;
                case 3:
                    animBody.CrossFade("Run", 0.1f);
                    break;
            }
        }


        if (CheckForUpcomingCliff() && !isWaitingWalk)
        {
            JumpPush();
        }

        AlignToGround();
        

        if(agent.enabled && (playerTransform.position - transform.position).sqrMagnitude >= 25)
        {
            if(DetectPlayer() && CanReachPlayer())
            {
                agent.SetDestination(playerTransform.position);
                stateAnimBody = 3;
                Systems.Visibility?.Invoke(0.06f * Time.deltaTime);
            }
            else if(!DetectPlayer())
            {
                agent.speed = WalkSpeed;

                if(DelayScream <= 0)
                {
                    DelayScream = Random.Range(16, 55);
                    AudioEvent.PlaySound(3);
                    animHead.CrossFade("Scream", 0.1f);
                }
                
                if(agent.enabled && !isWaitingWalk && HasReachedDestination())
                    StartCoroutine(WaitAndMove());
            }
            else
            {
                DelayRoar -= Time.deltaTime;
                stateAnimBody = 0;

                if(DelayRoar <= 0)
                {
                    DelayRoar = Random.Range(4, 10);
                    AudioEvent.PlaySound(2);
                    animHead.CrossFade("Scream", 0.1f);
                    Systems.Visibility?.Invoke(3f);
                }
            }
        }

        if((playerTransform.position - transform.position).sqrMagnitude <= 20 && Vector3.Angle(transform.forward, playerTransform.position - transform.position) < 70 && IsAttached == 0)
        {
            PlayerRb.AddForce(Vector3.up * 6, ForceMode.Impulse);

            Waiting = 1f;
            agent.enabled = false;
            IsAttached = 1;
            Systems.Visibility?.Invoke(0.3f);
            stateAnimBody = 0;
            animHead.CrossFade("Attack", 0.1f);
        }
        if(Waiting <= 0.8f && IsAttached == 1)
        {
            IsAttached = 2;
            Systems.Heals?.Invoke(15);
            PlayerRb.AddForce(transform.forward.normalized * 18, ForceMode.Impulse);
        }
        if(Waiting <= 0f && IsAttached == 2)
        {
            IsAttached = 0;
            agent.enabled = true;
        }
    }

    private bool CanReachPlayer()
    {
        NavMeshPath path = new NavMeshPath();

        // 1. пробуем построить путь
        if (!agent.CalculatePath(playerTransform.position, path))
            return false;

        if (path.status != NavMeshPathStatus.PathComplete)
            return false;

        // 2. путь должен иметь точки
        if (path.corners == null || path.corners.Length < 2)
            return false;

        // 3. проверяем, где реально заканчивается путь
        Vector3 lastPoint = path.corners[path.corners.Length - 1];

        float distToplayerTransform = Vector3.Distance(lastPoint, playerTransform.position);

        // 🔥 ключевой момент
        // если путь не доводит почти до цели — значит тупик/ловушка
        if (distToplayerTransform > 3f)
            return false;

        return true;
    }

    private void AlignToGround()
    {
        Physics.Raycast(ObjectTransform.position + Vector3.up, Vector3.down, out var hit, 7f, groundMask);
        
        Vector3 moveDir = hit.normal;

        var targetRotation = Quaternion.FromToRotation(ObjectTransform.up, moveDir) * ObjectTransform.rotation;

        ObjectTransform.rotation = Quaternion.Slerp(ObjectTransform.rotation, targetRotation, Time.deltaTime * 4f);
    }

    IEnumerator WaitAndMove()
    {
        stateAnimBody = 0;
        isWaitingWalk = true;

        yield return new WaitForSeconds(2f);

        MoveToNewPoint();

        isWaitingWalk = false;
    }


    private bool CheckForUpcomingCliff()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        
        Vector3 rayDirection = (transform.forward * 1.0f) + (Vector3.down * 2.0f);
        rayDirection.Normalize();

        bool hitGround = Physics.Raycast(rayStart, rayDirection, checkDistance, groundMask);
        Debug.DrawRay(rayStart, rayDirection * checkDistance, hitGround ? Color.green : Color.red);

        return !hitGround;
    }

    void MoveToNewPoint()
    {
        Vector3 point = GetRandomPointInBiome();
        agent.SetDestination(point);
        stateAnimBody = 2;
        CenterSpawnedObjects.Instance.EnemyPoint[ID] = point;
    }


    bool HasReachedDestination()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude <= 1f)
                return true;
        }

        return false;
    }

    Vector3 GetRandomPointInBiome()
    {
        for(int i = 0; i < 10; i++)
        {
            Vector3 random = Random.insideUnitSphere * 60;
            random += transform.position;

            if (NavMesh.SamplePosition(random, out NavMeshHit hit, 60, NavMesh.AllAreas))
            {
                Collider[] cols = Physics.OverlapSphere(hit.position, 0.2f);

                foreach (var col in cols)
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer(Biome))
                    {
                        stateAnimBody = 2;
                        return hit.position;
                    }
                }
            }
        }

        return transform.position;
    }

    private bool DetectPlayer()
    {
        bool b = false;

        for(int i = 0; i < CurrentBiome.CurrentsBioms.Length; i++)
        {
            if(CurrentBiome.CurrentsBioms[i] == Biome)
                b = true;
        }

        if(!b)
            return false;


        Vector3 toPlayer = playerTransform.position - transform.position;

        // Слышит рядом
        if (toPlayer.sqrMagnitude <= 13f * 13f)
            return true;

        // Видит впереди
        float viewDistance = 18f;

        if (toPlayer.sqrMagnitude > viewDistance * viewDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, toPlayer);

        if (angle > 70f) // половина от 120°
            return false;

        Vector3 start = transform.position + Vector3.up;
        Vector3 end = playerTransform.position + Vector3.up;

        if (Physics.Raycast(start, (end - start).normalized, out RaycastHit hit, viewDistance))
        {
            return hit.transform == playerTransform;
        }

        return false;
    }


    private void JumpPush()
    {
        if(agent.enabled && WaitingJump <= 0)
        {
            agent.enabled = false;
            rb.isKinematic = false;

            Vector3 pushDirection = transform.forward * jumpForwardForce + Vector3.up * jumpUpForce;
            rb.AddForce(pushDirection, ForceMode.Impulse);

            stateAnimBody = 1;

            WaitingJump = 0.15f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(agent.enabled || WaitingJump >= 0)
            return;


        if (!agent.enabled && ((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            WaitingJump = 0.05f;
            agent.enabled = true;
            stateAnimBody = 2;
        }
    }
}