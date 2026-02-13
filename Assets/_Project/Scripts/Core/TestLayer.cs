using UnityEngine;

public class TestLayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("What layer I am: " + gameObject.layer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
