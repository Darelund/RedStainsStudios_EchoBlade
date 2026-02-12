using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

//Used to rebake the whole map when we enter it
public class AutoNavmeshBaking : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;


    private void Start()
    {
       StartCoroutine(DelayNavmeshBaking());
    }
    private IEnumerator DelayNavmeshBaking()
    {
        yield return new WaitForSeconds(5);
        AutoBakeNavmeshSurface();
    }
    private void AutoBakeNavmeshSurface()
    {
        navMeshSurface = FindAnyObjectByType<NavMeshSurface>();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
}
