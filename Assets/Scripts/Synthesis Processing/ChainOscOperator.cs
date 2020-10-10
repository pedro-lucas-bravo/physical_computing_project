using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class ChainOscOperator : SynthModule, IParametersModule
{

    public enum Operation { Sum, Multiply}

    public Operation operation;
    public Color ampColor;

    SynthModule[] modules;

    public bool isPlaying { get { return modules.Length > 0; } }

    public override float Frequency {
        get {
            return 0;//Not important yet
        }

        set {
            for (int i = 0; i < modules.Length; i++) {
                modules[i].Frequency = value;
            }
        }
    }


    Func<float,float, float> OperationFn;

    void Awake() {
        modules = new SynthModule[0];
        MaxInputs = int.MaxValue;
        switch (operation) {
            case Operation.Sum:
                OperationFn = Sum;
                initialization_ = 0;
                break;
            case Operation.Multiply:
                OperationFn = Mult;
                initialization_ = 1;
                break;
            default:
                OperationFn = Sum;
                initialization_ = 0;
                break;
        }

        Amplitude = 1;
        //Params
        moduleParams_ = new ModuleParam[1];
        moduleParams_[0] = new ModuleParam("Volumen", Amplitude * 100.0f, 10, 100, 0.01f, ampColor, SetAmplitude);
    }

    private void SetAmplitude(float ampVal) {
        Amplitude = ampVal;
        for (int i = 0; i < modules.Length; i++) {
            if (operation == Operation.Sum && modules[i] is SampleModule)
                (modules[i] as SampleModule).AmplitudeFactor = Amplitude;
        }        
    }

    public override void RemoveSourceModule(SynthModule source) {
        if (source is SampleModule)
            (source as SampleModule).AmplitudeFactor = 1;
        var listHelper = modules.ToList();
        listHelper.Remove(source);
        modules = listHelper.ToArray();
    }

    public override void SetSourceModule(SynthModule module)
    {
        if (module != this && module.synthType != Type.Output
            && !(operation == Operation.Multiply && module.synthType == Type.ChainOperator))
        {
            var listHelper = modules.ToList();
            if (!listHelper.Contains(module))
            {
                listHelper.Add(module);
                modules = listHelper.ToArray();
            }
        }
    }

    public override float Calculate(ulong t)
    {
        float result = initialization_;
        for (int i = 0; i < modules.Length; i++)
        {
            //sum += modules[i].Calculate(t);            
            result = OperationFn(result, modules[i].Calculate(t));
        }
        //if (modules.Length > 0) {//Volume standarization
        //    var minVal = 144f;
        //    amp = 1.0f / modules.Length;
        //    amp = Mathf.Clamp(20*Mathf.Log10(amp), -minVal, 0);
        //    amp = (amp + minVal) / minVal;
        //    result = result / (modules.Length * amp);
        //}
        result_ = result;
        return Amplitude * result;
    }

    float result_;

    //float amp;

    public override float PreCalculate() {
        float prec = 0;
        for (int i = 0; i < modules.Length; i++) {
            prec = modules[i].PreCalculate();
        }
        return prec;
    }

    float initialization_;

    //Operations

    float Sum(float a, float b) {
        return a + b;
    }

    float Mult(float a, float b) {
        return a * b;
    }

    #region Param Control Inplementation

    public SynthModule GetModule() {
        return this;
    }

    public void SetParameter(string paramName, float value) {
        ModuleParam.SetParameter(moduleParams_, paramName, value);
    }

    public ModuleParam GetNextParameterToControl() {
        return ModuleParam.GetNextParameterToControl(moduleParams_, ref currentParamIndex_);
    }

    public bool Disconnected() {
        return ModuleParam.Disconnect(moduleParams_, ref currentParamIndex_);
    }

    public void Restart() {
        currentParamIndex_ = 0;
    }

    ModuleParam[] moduleParams_;
    int currentParamIndex_;    

    #endregion
}
