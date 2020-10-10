using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamController : MonoBehaviour{

    public Transform targetTrans;
    public Transform indicatorCenter;
    public float angularSpeed = 0.1f;
    public TextMesh paramNameLabel;
    public TextMesh paramValueLabel;
    public SpriteRenderer circleRender;
    public SpriteRenderer indicatorRender;
    public Color defaultCircleColor;
    //public DefaultTrackableEventHandler trackableEventHandler;

    public DrawParameterLine.ParameterConnection Connection { get; set; }

    private void Awake() {
        indicatorRender.color = circleRender.color = defaultCircleColor;
    }

    private void Start() {
        //trackableEventHandler.OnTrackingFoundEvent += OnTrackingFoundEvent;
        //trackableEventHandler.OnTrackingLostEvent += OnTrackingLostEvent;
    }

    private void OnTrackingLostEvent() {
        ResetController();
    }

    private void OnTrackingFoundEvent() {
        
    }

    private void OnDestroy() {
        //if (trackableEventHandler != null) {
        //    trackableEventHandler.OnTrackingFoundEvent -= OnTrackingFoundEvent;
        //    trackableEventHandler.OnTrackingLostEvent -= OnTrackingLostEvent;
        //}
    }

    public void SetModule(IParametersModule paramsModule) {
        paramsModule_ = paramsModule;
        var modelParam = paramsModule_.GetNextParameterToControl();
        paramName_ = modelParam.name;
        paramValue_ = modelParam.value;
        paramMin_ = modelParam.minVal;
        paramMax_ = modelParam.maxVal;
        var color = modelParam.color;

        lastRightVect_ = targetTrans.right;

        //Set Labels

        paramNameLabel.text = paramName_;
        paramValueLabel.text = paramValue_ + "";

        //Set Angle
        paramValue_ = Mathf.Clamp(paramValue_, paramMin_, paramMax_);
        var factorParam = (paramValue_ - paramMin_) / (paramMax_ - paramMin_);
        currentAngle_ = minAngle + factorParam * (maxAngle - minAngle);
        indicatorCenter.localEulerAngles = new Vector3(0, 0, -currentAngle_);

        //SetColor
        indicatorRender.color = circleRender.color = color;
    }

    public string GetParamName() {
        return paramName_;
    }

    public void ResetController() {
        DrawParameterLine.Instance.DestroyParameterConnectionLine(transform.parent);
        indicatorRender.color = circleRender.color = defaultCircleColor;
        paramNameLabel.text = "Control";
        paramValueLabel.text = "";
        paramsModule_ = null;
        indicatorCenter.localEulerAngles = Vector3.zero;        
    }

    void SetParameterValue(float factorVal) {
        int paramVal = Mathf.CeilToInt(paramMin_ + factorVal * (paramMax_ - paramMin_));
        if (paramVal != lastParam_) {
            paramValue_ = paramVal;
            paramValueLabel.text = paramValue_ + "";
            lastParam_ = paramVal;
            paramsModule_.SetParameter(paramName_, paramValue_);
            if (Mathf.Abs(lastFactorVal_ - factorVal) >= 0.1f) {
                //LoggingSystem.Instance.Write_Param(Connection.source.name, Connection.destination.name, paramName_, paramValue_);
                lastFactorVal_ = factorVal;
            }
        }
    }

    public void Update() {
        if (paramsModule_ != null) {
            var deltaAngle = Vector3.SignedAngle(lastRightVect_, targetTrans.right, targetTrans.up);//CalculateAngle(targetTrans.right, lastRightVect_);
            lastRightVect_ = targetTrans.right;
            if (Mathf.Abs(deltaAngle) > float.Epsilon)
                currentAngle_ += angularSpeed * Time.deltaTime * deltaAngle;            
            currentAngle_ = Mathf.Clamp(currentAngle_, minAngle, maxAngle);
            indicatorCenter.localEulerAngles = new Vector3(0, 0, -currentAngle_);
            var factor = (currentAngle_ - minAngle) / (maxAngle - minAngle);
            SetParameterValue(factor);
        }
    }

    int lastParam_;
    private readonly float maxAngle = 345;
    private readonly float minAngle = 8;

    //public static float CalculateAngle(Vector3 from, Vector3 to) {
    //    return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    //}

    string paramName_;
    float  paramValue_;
    float paramMin_= 0;
    float paramMax_ = 100;
    IParametersModule paramsModule_;
    Vector3 lastRightVect_;
    float currentAngle_;
    float lastFactorVal_;
}
