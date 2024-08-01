using System.Collections;
using Pathfinding;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;

    public float catchRadius = 15f;


    private Transform target;

    // private Animator animator;
    private float attackTimer;
    public float attackSpeed = 0.5f;

    private float raycastDistance = .7f;

    public float damage = 15f;
    private AIDestinationSetter aiDestinationSetter;
    private AIPath aiPath;
    private bool isPatrolling;
    private bool isFeared;
    private float fearTimer;
    private float fearTimerMax = 4f;
    private PhotonView PV;
    private Coroutine slowCoroutine;
    private float lastRespeedAmount;
    private bool killed = false;

    private void Awake()
    {
        // animator = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        attackSpeed = 1 / attackSpeed;
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(aiDestinationSetter);
            Destroy(aiPath);
        }
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        CheckPlayerAround();
        // animator.SetFloat("Speed", aiPath.maxSpeed, 0.05f, Time.deltaTime);
    }

    private void CheckPlayerAround()
    {
        if (isFeared)
        {
            fearTimer += Time.deltaTime;
            if (fearTimer >= fearTimerMax)
            {
                isPatrolling = false;
                isFeared = false;
                fearTimer = 0f;
            }
            else
            {
                return;
            }
        }

        if (target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, lookRadius, LayerMask.GetMask("Player"));
            foreach (Collider collider in colliders)
            {
                if (collider.transform.TryGetComponent(out PlayerController player) && !IsTargetHided(player.transform))
                {
                    target = player.transform;
                    isPatrolling = false;
                    aiDestinationSetter.fakeTargetPos = Vector3.zero;
                    aiDestinationSetter.target = target;
                    break;
                }
            }
        }

        if (target != null)
        {
            float distance = Vector3.Distance(target.position, transform.position);
            if (distance >= catchRadius || IsTargetHided(target))
            {
                Debug.Log("catch radius dan çıktı");
                target = null;
                return;
            }

            if (distance <= aiPath.endReachedDistance && attackTimer >= attackSpeed)
            {
                Debug.Log("HIT");
                 if (!killed)
                     target.GetComponent<PlayerController>().KillPlayer();
                killed = true;
                int attackTypeValue = Random.Range(0, 3) + 1;
                // animator.SetInteger("AttackTypeValue", attackTypeValue);
                attackTimer = 0;
                target = null;
            }
        }
        else
        {
            Patroling();
        }

        attackTimer += Time.fixedDeltaTime;
    }

    public void SlowEnemy(float percentage, float duration)
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            aiPath.maxSpeed += lastRespeedAmount;
        }

        float amount = aiPath.maxSpeed * percentage / 100f;
        aiPath.maxSpeed -= amount;

        slowCoroutine = StartCoroutine(AfterSlowedEnemy(duration, amount));
    }

    private IEnumerator AfterSlowedEnemy(float duration, float speedAmount)
    {
        lastRespeedAmount = speedAmount;
        yield return new WaitForSeconds(duration);
        aiPath.maxSpeed += speedAmount;
        slowCoroutine = null;
    }

    private bool IsTargetHided(Transform target)
    {
        Player player = target.GetComponent<PhotonView>().Owner;
        if (player.CustomProperties.TryGetValue("isHided", out object value))
        {
            return (bool)value;
        }

        return false;
    }

    public void Fear(Vector3 fearedFrom)
    {
        PV.RPC(nameof(FearRPC), PhotonNetwork.MasterClient, fearedFrom);
    }

    [PunRPC]
    private void FearRPC(Vector3 fearedFrom)
    {
        if (isFeared)
        {
            fearTimer = 0f;
            return;
        }

        Vector3 newPos;
        do
        {
            newPos = FindRandomPos();
        } while (Vector3.Distance(newPos, transform.position) > Vector3.Distance(newPos, fearedFrom));

        Debug.Log("fear: " + newPos);
        target = null;
        isFeared = true;
        isPatrolling = false;
        Patroling(newPos);
    }

    public void Patroling(Vector3 pos = default)
    {
        if (aiPath.reachedEndOfPath && aiDestinationSetter.isFakeTargeted)
        {
            // fake targete erisildi yeni bir patrolling pozisyonu bul
            isPatrolling = false;
        }

        if (isPatrolling) return;
        Vector3 fakePos = pos == default ? FindRandomPos() : pos;
        Debug.Log("Geziyom: " + fakePos);
        aiDestinationSetter.fakeTargetPos = fakePos;
        isPatrolling = true;
    }

    private Vector3 FindRandomPos()
    {
        Vector3 fakePos;
        float minRadius = 5f;
        float maxRadius = 12f;
        do
        {
            fakePos = UtilClass.GetRandomPointInArea(transform.position, minRadius, maxRadius);
            minRadius -= 0.1f;
            maxRadius += 0.1f;
        } while (Physics.Raycast(new Vector3(fakePos.x, 15, fakePos.z), -transform.up, out RaycastHit hit, 40f,
                     LayerMask.GetMask("Environment")) && PathfinderHelper.Instance.IsPositionInTheMap(fakePos));

        return fakePos;
    }
}