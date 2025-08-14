using UnityEngine;

public class Destroy_Premade : MonoBehaviour
{
    [SerializeField] GameObject premade_Paths;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(premade_Paths);
        }
    }
}
