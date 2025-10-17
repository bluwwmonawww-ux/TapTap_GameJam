using UnityEngine;

public class SaveButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnMouseDown();
        }
    }
    public void OnMouseDown()
    {
        Debug.Log("Game Saved");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerActionRecorder>().Save();
        Debug.Log("Game Saved");
    }
}
