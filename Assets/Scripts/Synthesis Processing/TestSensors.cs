using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSensors : MonoBehaviour
{
    Gyroscope gyro;

    void Start() {

        gyro = Input.gyro;
        if (!gyro.enabled) {
            gyro.enabled = true;
        }
        initRotation_ = gameObject.transform.rotation;
        initGyro_ = gyro.attitude;
    }

    void Update() {
        gameObject.transform.rotation = Quaternion.Inverse(gyro.attitude * Quaternion.Inverse(initGyro_)) * initRotation_;
    }

    Quaternion initRotation_;
    Quaternion initGyro_;
}

