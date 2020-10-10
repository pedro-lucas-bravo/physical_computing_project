using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamRingController : MonoBehaviour {
    float initDegreeValue;
    public Transform indicatorCenter;

    private void Start() {
        degreeValue_ = initDegreeValue;
        indicatorCenter.eulerAngles = new Vector3(0, 0, degreeValue_);
    }

    public void Update() {
        //degreeValue_ = initDegreeValue + deltaDegree * Time.deltaTime;
        indicatorCenter.eulerAngles = new Vector3(0, 0, degreeValue_);
    }

    float degreeValue_;

}
//    public Transform indicatorCenter;

//    void Start() {
//        initRotation_ = indicatorCenter.rotation;
//        gyro = Input.gyro;
//        if (!gyro.enabled) {
//            gyro.enabled = true;
//        }
//        //referenceRotation = DeviceRotation.Get();
//        CalculateCompensation();
//        //Debug.Log(gyro.attitude.eulerAngles);
        
//    }

//    Quaternion initRotation_;

//    private void Update() {
//        CalculateCompensation();
//        //Debug.Log(gyro.attitude.eulerAngles);
//    }

//    void CalculateCompensation() {

//        var angle = /*Quaternion.Angle(gyro.attitude, Quaternion.identity);*/-gyro.attitude.eulerAngles.z + 90;

//        indicatorCenter.localEulerAngles = new Vector3(0, 0, angle + indicatorCenter.parent.transform.eulerAngles.z);
//        indicatorCenter.rotation = initRotation_;
//        indicatorCenter.localEulerAngles = new Vector3(0, 0, indicatorCenter.localEulerAngles.z);

//        //Vector3 gyroEuler = gyro.attitude.eulerAngles;
//        //indicatorCenter.localEulerAngles = new Vector3(gyroEuler.x,gyroEuler.y, gyroEuler.z);

//        //Vector3 upVec = indicatorCenter.TransformDirection(Vector3.forward);

//        //Debug.LogWarning(upVec);
//        //Debug.Log(gyroEuler);
//        //indicatorCenter.localEulerAngles = new Vector3(0,0, -gyroEuler.z + 90);
//        //Debug.Log(indicatorCenter.localEulerAngles);

//        //indicatorCenter.localRotation = gyro.attitude;
//        //var eulerLocal = indicatorCenter.eulerAngles;
//        //eulerLocal.x = eulerLocal.y = 0;
//        //indicatorCenter.localEulerAngles = eulerLocal;

//        //Vector3 gyroEuler = Input.gyro.attitude.eulerAngles;
//        //indicatorCenter.eulerAngles = new Vector3(-1.0f * gyroEuler.x, -1.0f * gyroEuler.y, gyroEuler.z);

//        //Vector3 upVec = indicatorCenter.InverseTransformDirection(-1f * Vector3.forward);


//        //Quaternion referenceRotation = Quaternion.identity;
//        //Quaternion deviceRotation = DeviceRotation.Get();
//        //Quaternion eliminationOfXY = Quaternion.Inverse(
//        //    Quaternion.FromToRotation(referenceRotation * Vector3.forward,
//        //                              deviceRotation * Vector3.forward)
//        //);
//        //Quaternion rotationZ = eliminationOfXY * deviceRotation;
//        //float roll = rotationZ.eulerAngles.z;
//        //indicatorCenter.localEulerAngles = new Vector3(0, 0, roll);
//    }
//    Quaternion referenceRotation;

//    private void OnDestroy() {
//        gyro.enabled = false;
//    }

//    Gyroscope gyro;
//}

//public static class DeviceRotation {
//    private static bool gyroInitialized = false;

//    public static bool HasGyroscope {
//        get {
//            return SystemInfo.supportsGyroscope;
//        }
//    }

//    public static Quaternion Get() {
//        if (!gyroInitialized) {
//            InitGyro();
//        }

//        return HasGyroscope
//            ? ReadGyroscopeRotation()
//            : Quaternion.identity;
//    }

//    private static void InitGyro() {
//        if (HasGyroscope) {
//            Input.gyro.enabled = true;                // enable the gyroscope
//            Input.gyro.updateInterval = 0.0167f;    // set the update interval to it's highest value (60 Hz)
//        }
//        gyroInitialized = true;
//    }

//    private static Quaternion ReadGyroscopeRotation() {
//        return new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
//    }
//}
