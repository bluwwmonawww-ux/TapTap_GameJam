using UnityEngine;

public class CameraRegionChange : MonoBehaviour
{
    public  GameObject CameraRangeObj;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CameraFollow cameraFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
            cameraFollow.cameraRange = CameraRangeObj.GetComponent<CameraRange>();
            
        }
    }
}
