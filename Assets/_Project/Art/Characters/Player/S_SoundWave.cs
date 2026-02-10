using UnityEngine;

public class S_SoundWave : MonoBehaviour
{
    public GameObject prefab;
    
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;
    
    public float lifeTime = 5f;
    
    public void LeftStep()
    {
        SpawnAt(leftSpawnPoint);
    }

    public void RightStep()
    {
        SpawnAt(rightSpawnPoint);
    }
    
    private void SpawnAt(Transform spawnPoint)
    {
        if (prefab == null) return;
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
        GameObject spawned = Instantiate(prefab, pos, rot);
        if (lifeTime > 0f) Destroy(spawned, lifeTime);
    }
}
