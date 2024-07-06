using UnityEngine;

public class EnemyAnimatorOverrider : MonoBehaviour
{
    public static EnemyAnimatorOverrider Instance { get; private set; }
    [SerializeField] private AnimatorOverrideController[] animatorOverrides;
    private void Awake()
    {
        Instance = this;
    }
    public AnimatorOverrideController GetAnimatorOverride(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.BunnyGirl => animatorOverrides[1],
            _ => animatorOverrides[0],
        };
    }
}
