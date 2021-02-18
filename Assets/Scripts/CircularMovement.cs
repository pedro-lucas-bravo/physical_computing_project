using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{

    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        trans_ = transform;
        point_ = trans_.position;
    }

    // Update is called once per frame
    void Update()
    {
        angle_ = speed * Time.deltaTime;
        trans_.Rotate(trans_.parent.up, angle_, Space.World);
    }
    Transform trans_;
    Vector3 point_;
    float angle_;
}
