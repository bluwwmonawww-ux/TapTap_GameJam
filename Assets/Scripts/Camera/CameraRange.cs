using UnityEngine;

public class CameraRange : MonoBehaviour
{
    private GameObject CameraRangeObj;
    public float LeftBorder;
    public float RightBorder;
    public float TopBorder;
    public float BottomBorder;
    void Start()
    {
        Vector3 camerarangeposition= transform.position;
        Vector2 camerarangescale= transform.localScale;
        LeftBorder = camerarangeposition.x - camerarangescale.x * 0.5f;
        RightBorder = camerarangeposition.x + camerarangescale.x * 0.5f;
        TopBorder = camerarangeposition.y + camerarangescale.y*0.5f;
        BottomBorder= camerarangeposition.y -camerarangescale.y * 0.5f;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
