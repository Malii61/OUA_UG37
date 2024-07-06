using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    /// <Patrolling>
    private Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    /// </Patrolling>

    public float lookRadius = 10f;
    public float catchRadius = 15f;
    public Vector3 m_attackOffset;
    public float m_attackRadius;

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;
    private float attackTimer;
    private float attackSpeed;

    private float minDistance = 1f; // Hedefe olan minimum mesafe
    private float maxDistance = 10f; // Hedefe olan maksimum mesafe
    private float checkInterval = 3f; // Kontrol aralýðý
    public float smallMovementThreshold = 1f; // Ufak hareketler için eþik deðeri
    private bool isStuck = false; // Takýldýðý durumu tutan deðiþken
    private Vector3 lastPosition; // Son pozisyonu tutan deðiþken
    private float stuckTimer = 0f; // Kontrol aralýðýný takip eden zamanlayýcý
    public float rotationSpeed = 3f;
    private float raycastDistance = .7f;

    public float damage = 15f;
    public EnemyType enemyType;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        animator.runtimeAnimatorController = EnemyAnimatorOverrider.Instance.GetAnimatorOverride(enemyType);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        CheckPlayerAround();
        CheckWall();
        CheckDoor();
        CheckStuck();

        Vector3 currentVelocity = agent.velocity;
        float currentSpeed = currentVelocity.magnitude;
        animator.SetFloat("Speed", currentSpeed, 0.05f, Time.deltaTime);
    }

    private void CheckWall()
    {
        // Düþmanýn hareket etmesi gereken yönde bir raycast oluþtur
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, wallLayer) && target == null)
        {
            // Düþmanýn önünde bir engel var
            // Yönünü deðiþtirerek engelin etrafýndan dolaþmasýný saðla
            Vector3 direction = hit.point - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            SetRandomPatrolPoint();
        }
    }

    private void CheckDoor()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance + 1f))
        {
            if (hit.transform.TryGetComponent(out Door door))
            {
                if (door.isOpenable())
                    door.Interact(false);
                else
                    SetRandomPatrolPoint();
            }
        }
    }
    private void CheckStuck()
    {
        stuckTimer += Time.deltaTime;
        if (stuckTimer < checkInterval)
        {
            return;
        }
        stuckTimer = 0f;

        Vector3 currentTargetPos = target == null ? walkPoint : target.position;
        // Hedefe olan mesafeyi kontrol et
        float distanceToTarget = Vector3.Distance(transform.position, currentTargetPos);

        // Hedefe ulaþamama durumunu kontrol et
        if (distanceToTarget < minDistance || distanceToTarget > maxDistance)
        {
            isStuck = true;
        }
        else
        {
            // Ufak hareketleri tespit et
            if (Vector3.Distance(transform.position, lastPosition) < smallMovementThreshold)
            {
                isStuck = true;
            }
            else
            {
                isStuck = false;
            }
        }

        // Takýlý kaldýðý durumu kontrol et ve gerekli iþlemleri yap
        if (isStuck)
        {
            // Takýldýðý durumda yapýlacak iþlemler
            FixStuckPosition();
        }

        // Son pozisyonu güncelle
        lastPosition = transform.position;
    }

    private void FixStuckPosition()
    {
        // Düþmanýn takýldýðý durumu tespit ettiðinizde, pozisyonunu düzeltmek için uygun bir yöntem uygulayýn
        // Örneðin, düþmaný biraz geri veya yan tarafa hareket ettirebilirsiniz
        Vector3 newPosition = transform.position + new Vector3(Random.Range(-1, 1), 0f, Random.Range(-1, 1)); // Örnek olarak düþmaný hareket ettiriyoruz
        transform.position = newPosition;
    }
    private void CheckPlayerAround()
    {
        if (target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, lookRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.transform.TryGetComponent(out PlayerController player))
                {
                    stuckTimer = 0;
                    target = player.transform;
                    if (IsTargetHided())
                    {
                        target = null;
                        break;
                    }
                    agent.SetDestination(target.position);
                    break;
                }
            }
        }

        if (target != null)
        {
            float distance = Vector3.Distance(target.position, transform.position);
            if (distance >= catchRadius || IsTargetHided())
            {
                target = null;
                return;
            }

            agent.SetDestination(target.position);
            if (distance <= agent.stoppingDistance && attackTimer >= attackSpeed)
            {
                RotateEnemyToTarget();
                int attackTypeValue = Random.Range(0, 3) + 1;
                animator.SetInteger("AttackTypeValue", attackTypeValue);
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

    private bool IsTargetHided()
    {
        Player player = target.GetComponent<PhotonView>().Owner;
        if (player.CustomProperties.TryGetValue("isHided", out object value))
        {
            return (bool)value;
        }
        return false;
    }

    private void RotateEnemyToTarget()
    {
        // Düþmanýn sizi hedef olarak belirlemesi
        Vector3 targetDirection = target.position - transform.position;
        targetDirection.y = 0f;
        targetDirection.Normalize();

        // Düþmanýn sizi doðru yöne dönmesi
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Y eksenindeki rotasyonu sýfýrlama
        targetRotation *= Quaternion.Euler(0f, 0f, 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Patroling()
    {
        if (!walkPointSet) SetRandomPatrolPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SetRandomPatrolPoint()
    {
        // Ground layer'ýna sahip tüm collider'larý bul
        Collider[] groundColliders = Physics.OverlapSphere(transform.position, 10f, groundLayer);

        // Rastgele bir devriye noktasý seç
        if (groundColliders.Length > 0)
        {
            int randomIndex = Random.Range(0, groundColliders.Length);

            while(groundColliders[randomIndex].CompareTag("House") && enemyType == EnemyType.SchoolGirl ||
                    !groundColliders[randomIndex].CompareTag("House") && enemyType == EnemyType.BunnyGirl)

                randomIndex = Random.Range(0, groundColliders.Length);

            Vector3 randomPosition = groundColliders[randomIndex].transform.position;
            walkPoint = randomPosition;
            walkPointSet = true;
        }
    }
    //Animation Events
    void EventAttack(int remainedHitCount)
    {
        Vector3 center = transform.TransformPoint(m_attackOffset);
        float radius = m_attackRadius;


        Debug.DrawRay(center, transform.forward, Color.red, 0.5f);

        Collider[] cols = Physics.OverlapSphere(center, radius);


        //------------------------
        //Check Enemy Hit Collider
        //------------------------
        foreach (Collider col in cols)
        {
            PlayerController playerController = col.GetComponent<PlayerController>();
            if (playerController == null)
                continue;

            playerController.TakeDamage(center, transform.forward, damage);
        }
        if (remainedHitCount < 1)
            animator.SetInteger("AttackTypeValue", 0);
    }

}
