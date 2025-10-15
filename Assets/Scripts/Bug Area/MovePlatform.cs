using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField]public float MoveDistance;
    [SerializeField] public Vector2 MoveDirection;
    [SerializeField] public float MoveSpeed;
    private Rigidbody2D rigidbody;
    private float DistanceMoved;
    private bool Forward;
    private Vector3 lastfposition;
    void Start()
    {
        DistanceMoved = MoveDistance;
        Forward=true;
        rigidbody= GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.linearVelocity = CalculateMoveSpeed();
        //transform.position += CalculateMoveSpeed() * Time.deltaTime;
        //Debug.Log("distance1" + CalculateMoveSpeed() * Time.deltaTime);
        //Debug.Log("distance2"+(transform.position - lastfposition));
        lastfposition=transform.position;
          DistanceMoved -= CalculateMoveSpeed().magnitude * Time.deltaTime;
        
        

        if (DistanceMoved <= 0)
        {
            Forward = !Forward;
            DistanceMoved=MoveDistance;
        }
        
    }

    public Vector3 CalculateMoveSpeed()
    {
        if (Forward)
        {
            return Vector3.Normalize(MoveDirection) * MoveSpeed ;
        }
        else
        {
            return -Vector3.Normalize(MoveDirection) * MoveSpeed ;
        }
    }

    

}
