using UnityEngine;

public class DestroyAfterXSeconds : MonoBehaviour
{
    [SerializeField] private float seconds;

    private void Awake()
    {
        Destroy(gameObject, seconds);
    }
}
