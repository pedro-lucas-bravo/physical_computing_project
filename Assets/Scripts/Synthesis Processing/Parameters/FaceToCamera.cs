using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamera : MonoBehaviour{

    //public Transform targetReference;

    // Start is called before the first frame update
    void Start()
    {
        camTrans_ = Camera.main.transform;
        trans_ = transform;
    }

    // Update is called once per frame
    void LateUpdate(){
        trans_.LookAt(trans_.position + camTrans_.rotation * Vector3.forward,
                        camTrans_.rotation * Vector3.up);
        //if(targetReference != null)
        //    trans_.forward = -targetReference.up;
    }

    Transform camTrans_;
    Transform trans_;
}
