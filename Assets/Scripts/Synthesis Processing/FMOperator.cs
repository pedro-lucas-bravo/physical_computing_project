using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class FMOperator : SynthModule, IParametersModule {

    public Color ampColor;
    public Color multColor;

    SynthModule[] modules;
    float[] preCalculates;

    public bool isPlaying { get { return modules.Length > 0; } }

    void Awake() {
        modules = new SynthModule[0];
        preCalculates = new float[0];
        MaxInputs = int.MaxValue;

        Amplitude = 1;
        //Module Params
        moduleParams_ = new ModuleParam[2];
        moduleParams_[0] = new ModuleParam("Volumen", Amplitude * 100.0f, 10, 100, 0.01f, ampColor, SetAmplitude);
        moduleParams_[1] = new ModuleParam("Multiplicador\nde Modulador", 1, 1, 100, 1, multColor, SetModulatorMultiplier);
    }

    public override void RemoveSourceModule(SynthModule source) {
        if (source.synthType == Type.ChainOperator)
            return;
        var listHelper = modules.ToList();
        if (!listHelper.Contains(source))
            return;
        source.Multiplier = 1;
        listHelper.Remove(source);
        if (source.synthType == Type.Output)
            outputModule_ = null;
        modules = listHelper.ToArray();
        preCalculates = new float[listHelper.Count];
    }

    public override void SetSourceModule(SynthModule module) {
        if (module.synthType == Type.ChainOperator)
            return;
        if (module != this) {
            var listHelper = modules.ToList();
            if (!listHelper.Contains(module)) {
                listHelper.Add(module);
                if (outputModule_ != null && listHelper.Contains(outputModule_)) {//Reorder
                    listHelper.Remove(outputModule_);
                    listHelper.Add(outputModule_);
                }
                if (module.synthType == Type.Output)
                    outputModule_ = module;
                modules = listHelper.ToArray();
                preCalculates = new float[listHelper.Count];
            }
        }
    }

    public override float Calculate(ulong t) {
        float result = 0;
        if (modules.Length == 0) {
            return result;
        } else {
            if (modules.Length == 1) {
                result = modules[0].Calculate(t);
            } else {
                result = modules[0].Calculate((ulong)((preCalculates[0]*t + modules[1].Calculate(t)) / preCalculates[0]));
            }
        }
        return Amplitude * result;          
    }

    public override float PreCalculate() {
        float prec = 0;
        for (int i = 0; i < modules.Length; i++) {
            prec = modules[i].PreCalculate();
            preCalculates[i] = prec;
        }
        return prec;
    }

    public void SetAmplitude(float amp) {
        Amplitude = amp;
    }

    public void SetModulatorMultiplier(float mult) {
        if (modules.Length == 3) {
            modules[1].Multiplier = mult;
        }
    }

    SynthModule outputModule_;

    #region Param Control Implementation

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
