using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParametersModule{
    SynthModule GetModule();
    void SetParameter(string paramName, float value);
    ModuleParam GetNextParameterToControl();
    bool Disconnected();
    void Restart();
}

public struct ModuleParam {
    public string name;
    public float value;
    public float minVal;
    public float maxVal;
    public float conversionFactor;
    public Color color;
    public Action<float> paramFunction;

    public ModuleParam(string name_, float value_, float minVal_, float maxVal_, float conversionFact_, Color color_, Action<float> paramFunc_) {
        name = name_;
        value = value_;
        minVal = minVal_;
        maxVal = maxVal_;
        conversionFactor = conversionFact_;
        color = color_;
        paramFunction = paramFunc_;
    }

    public static void SetParameter(ModuleParam[] moduleParams, string paramName, float value) {
        for (int i = 0; i < moduleParams.Length; i++) {
            if (moduleParams[i].name == paramName) {
                moduleParams[i].value = value;
                moduleParams[i].paramFunction(value * moduleParams[i].conversionFactor);
                break;
            }
        }
    }

    public static ModuleParam GetNextParameterToControl(ModuleParam[] moduleParams, ref int currentParamIndex) {
        if (currentParamIndex >= moduleParams.Length)
            currentParamIndex = 0;
        var index = currentParamIndex;
        currentParamIndex++;
        return moduleParams[index];
    }

    public static bool Disconnect(ModuleParam[] moduleParams,ref int currentParamIndex) {
        var disconnect = currentParamIndex >= moduleParams.Length;
        if(disconnect)
            currentParamIndex = 0;
        return disconnect;
    }
}
