using UnityEngine;
using System.Collections;
using System;

public class WaveOscGUI : MonoBehaviour, IParametersModule {

    public enum WaveType { Sin, Square, Triangle, Sawtooth, Noise}

    public SynthesisManager synthesisManager;
    public WaveType type; 
    public float defaultAmp = 1;
    public float defaultFreq = 440;
    public TextMesh volumeLabel;
    public TextMesh frequencyLabel;
    public Color ampColor;
    public Color freqColor;
    public Color multColor;


    // Use this for initialization
    void Awake () {
        switch (type) {
            case WaveType.Sin:
                waveModule_ = gameObject.AddComponent<SinOscillator>();
                break;
            case WaveType.Square:
                waveModule_ = gameObject.AddComponent<SqrOscillator>();
                break;
            case WaveType.Triangle:
                waveModule_ = gameObject.AddComponent<SinOscillator>();
                break;
            case WaveType.Sawtooth:
                waveModule_ = gameObject.AddComponent<SinOscillator>();
                break;
            case WaveType.Noise:
                waveModule_ = gameObject.AddComponent<NoiseGenerator>();
                break;
            default:
                waveModule_ = gameObject.AddComponent<SinOscillator>();
                break;
        }
        waveModule_.synthType = SynthModule.Type.SourceWave;
        waveModule_.Amplitude = defaultAmp;
        waveModule_.Frequency = defaultFreq;
        waveModule_.Multiplier = 1;

        //Module Params
        moduleParams_ = new ModuleParam[3];
        moduleParams_[0] = new ModuleParam("Volumen", waveModule_.Amplitude * 100.0f, 10, 100, 0.01f,ampColor, SetAmplitude);
        moduleParams_[1] = new ModuleParam("Frecuencia (Hz)", waveModule_.Frequency, 1, 18000, 1,freqColor, SetFrequency);
        moduleParams_[2] = new ModuleParam("Multiplicador", waveModule_.Multiplier, 1, 10,1, multColor, SetMultiplier);
    }

    public void OnSelectOsc() {
        synthesisManager.SetSource(waveModule_);        
    }

    public void SetFrequency(float freq) {
        waveModule_.Frequency = freq;
        frequencyLabel.text = Mathf.CeilToInt(freq) + " Hz";
    }

    public void SetAmplitude(float amp) {
        waveModule_.Amplitude = amp;
        volumeLabel.text = Mathf.CeilToInt(amp * 100f) + "";
    }

    public void SetMultiplier(float mult) {
        waveModule_.Multiplier = mult;
    }    

    SynthModule waveModule_;

    #region Interface Implementation

    public SynthModule GetModule() {
        return waveModule_;
    }

    public void SetParameter(string paramName, float value) {
        ModuleParam.SetParameter(moduleParams_, paramName, value);
    }

    public ModuleParam GetNextParameterToControl() {
        var modParam = ModuleParam.GetNextParameterToControl(moduleParams_, ref currentParamIndex_);
        if (!useMultiplier_ && currentParamIndex_ == 2)
            currentParamIndex_ = moduleParams_.Length;
        return modParam;
    }

    public bool Disconnected() {
        return ModuleParam.Disconnect(moduleParams_, ref currentParamIndex_);
    }

    public void Restart() {
        currentParamIndex_ = 0;
    }

    ModuleParam[] moduleParams_;
    bool useMultiplier_ = false;
    int currentParamIndex_;
    #endregion
}
