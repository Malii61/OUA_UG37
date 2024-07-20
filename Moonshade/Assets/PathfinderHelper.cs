using UnityEngine;

public class PathfinderHelper : MonoBehaviour
{
    public static PathfinderHelper Instance;
    private AstarPath astarPath;

    private void Awake()
    {
        Instance = this;
        astarPath = GetComponent<AstarPath>();
    }

    public bool IsPositionInTheMap(Vector3 pos)
    {
        float width = astarPath.data.gridGraph.Width;
        float depth = astarPath.data.gridGraph.Depth;

        if (pos.x > width || pos.x < 0 || pos.z > depth || pos.z < 0)
            return false;

        return true;
    }
}