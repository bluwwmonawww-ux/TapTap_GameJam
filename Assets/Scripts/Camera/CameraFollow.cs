using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Player;
    public CameraRange cameraRange;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraRange != null)
        {
            Vector3 playerposition = new Vector3(Player.transform.position.x, Player.transform.position.y, -10);
            float Positionx = transform.position.x;
            float Positiony = transform.position.y;
            if (playerposition.x < cameraRange.RightBorder && playerposition.x > cameraRange.LeftBorder)
            {
                Positionx = playerposition.x;
            }
            else if (playerposition.x > cameraRange.RightBorder)
            {
                Positionx = cameraRange.RightBorder;
            }
            else if(playerposition.x < cameraRange.LeftBorder) 
            {
                Positionx= cameraRange.LeftBorder;
            
            }
            if (playerposition.y < cameraRange.TopBorder && playerposition.y > cameraRange.BottomBorder)
            {
                Positiony = playerposition.y;
            }
            else if (playerposition.y > cameraRange.TopBorder)
            {
                Positiony = cameraRange.TopBorder;
            }
            else if (playerposition.y < cameraRange.BottomBorder)
            {
                Positiony = cameraRange.BottomBorder;
            }

            Vector3 targetposition = new Vector3(Positionx, Positiony, -10f);
            if (Vector3.Distance(transform.position , targetposition)>5f)
            {
                transform.position = Vector3.Lerp(transform.position, targetposition, 0.2f);

            }
            else
            {
                transform.position = targetposition;
            }



            //if (transform.position.x > cameraRange.RightBorder)
            //{
            //    Vector3 targetposition = new Vector3(cameraRange.RightBorder, transform.position.y, -10);
            //    transform.position = Vector3.Lerp(transform.position, targetposition, 0.2f);
            //    if (Math.Abs(transform.position.x - cameraRange.RightBorder) < 0.1f)
            //    {
            //        transform.position = targetposition;
            //    }
            //}
            //else if (transform.position.x < cameraRange.LeftBorder)
            //{
            //    Vector3 targetposition = new Vector3(cameraRange.LeftBorder, transform.position.y, -10);
            //    transform.position = Vector3.Lerp(transform.position, targetposition, 0.2f);
            //    if (Math.Abs(transform.position.x - cameraRange.LeftBorder) < 0.1f)
            //    {
            //        transform.position = targetposition;
            //    }
            //}
            //else if (transform.position.y > cameraRange.TopBorder)
            //{
            //    Vector3 targetposition = new Vector3(transform.position.x, cameraRange.TopBorder, -10);
            //    transform.position = Vector3.Lerp(transform.position, targetposition, 0.2f);
            //    if (Math.Abs(transform.position.y - cameraRange.TopBorder) < 0.1f)
            //    {
            //        transform.position = targetposition;
            //    }
            //}
            //else if (transform.position.y < cameraRange.BottomBorder)
            //{
            //    Vector3 targetposition = new Vector3(transform.position.x, cameraRange.BottomBorder, -10);
            //    transform.position = Vector3.Lerp(transform.position, targetposition, 0.2f);
            //    if (Math.Abs(transform.position.y - cameraRange.BottomBorder) < 0.1f)
            //    {
            //        transform.position = targetposition;
            //    }
            //}
            //else
            //{
            //    transform.position = new Vector3(Positionx, Positiony, -10);
            //}
        }
    }
}
