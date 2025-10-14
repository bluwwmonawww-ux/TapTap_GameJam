using UnityEngine;

public class ElemenatAttribue : MonoBehaviour
{
    [SerializeField] private bool IsIce;
    [SerializeField]private bool IsMagma;
    [SerializeField] private bool Appearing;
    void Start()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        Appearing = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Appear()
    {
        Appearing = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        if (IsIce) {
            
            gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D");
        if (Appearing)
        {
            Debug.Log("Appearing");
            if (IsMagma)
            {
                Debug.Log("IsMagma");
                if (other.CompareTag("Player"))
                {
                    other.GetComponent<PlayerStatus>().Health -= 101f;
                    Debug.Log("health" + other.GetComponent<PlayerStatus>().Health);

                }
            }
        }
    }
    public void Disappear()
    {

        if (IsIce)
        {
            
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            

        }
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Appearing = false;
    }
}
