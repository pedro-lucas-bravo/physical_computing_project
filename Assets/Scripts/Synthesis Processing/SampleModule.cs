using UnityEngine;
using System.Collections;
using System;

public class SampleModule : SynthModule, IParametersModule {

    public AudioSource sampleSource;
    public float amplitude;
    public Color ampColor;
    public Color speedColor;
    public TextMesh volumeLabel;
    public TextMesh speedLabel;

    public bool IsPlaying { get { return playing; } }
    public float Speed { get { return speed_; } }

    public override float Amplitude {
        get {
            return originalAmp_;
        }

        set {
            originalAmp_ = value;
            sampleSource.volume = originalAmp_ * ampFactor_;
        }
    }

    float ampFactor_;
    float originalAmp_;

    public float AmplitudeFactor {
        get {
            return ampFactor_;
        }
        set {
            ampFactor_ = value;
            sampleSource.volume = originalAmp_ * ampFactor_;
        }
    }

    void Awake() {
        MaxInputs = 0;
        sample_rate_ = AudioSettings.outputSampleRate;
        speed_ = 50;
        AmplitudeFactor = 1;
        Amplitude = 1;
        //Module Params
        moduleParams_ = new ModuleParam[2];
        moduleParams_[0] = new ModuleParam("Volumen", Amplitude * 100.0f, 10, 100, 0.01f, ampColor, SetAmplitude);
        moduleParams_[1] = new ModuleParam("Velocidad", speed_, 5, 100, 1, speedColor, SetSpeed);        
    }

  
    private void Update() {
        if (!sampleSource.isPlaying && playing)
            sampleSource.Play();
        if (playing) {
            timer_ += Time.deltaTime;
            var timeSynth = (time_position_current - time_position_start) / sample_rate_;
            if (timer_ > (timeSynth + 0.25f)) {//Wait for 200 ms to check if there is not connection
                playing = false;
                sampleSource.Stop();
            }
        }
    }

    //void OnAudioFilterRead(float[] data, int channels) {
    //    if (samples_ == null) {
    //        samples_ = new float[data.Length];
    //        samples_size_ = 0;
    //        samples_index_ = 0;
    //    }
    //    for (var i = 0; i < data.Length; i = i + channels) {
    //        samples_[samples_index_] = data[i];
    //        if (channels == 2) {
    //            samples_[samples_index_] += data[i + 1];
    //            samples_[samples_index_] *= 0.5f;
    //        }
    //        samples_index_ = (samples_index_ + 1) % data.Length;
    //        samples_size_ = (samples_size_ + 1) % data.Length;
    //    }
    //}


    public override float Calculate(ulong t) {
        //amplitudHelper_ = amplitude;
        //sample_ = samples_[samples_current_index_];
        //samples_current_index_ = (samples_current_index_ + 1) % samples_size_;
        //return sample_ * amplitudHelper_;
        if (!playing) {
            time_position_start = t;
            time_position_current = t;
            timer_ = 0;
        } else {
            time_position_current = t;
        }
        playing = true;
        return 0;
    }

    public override float PreCalculate() {
        return 0;
    }   

    public void SetAmplitude(float amp) {
        Amplitude = amp;
        volumeLabel.text = Mathf.CeilToInt(amp *100) + "";
    }

    public void SetSpeed(float speed) {
        speed_ = speed;
        var minPitch = 0;
        var maxPitch = 2;
        var factor = speed_ / 100.0f;
        sampleSource.pitch = minPitch + factor * (maxPitch - minPitch);
        speedLabel.text = speed + "";
    }

    #region Param Control Interface

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

    #endregion

    //float[] samples_;
    //int samples_size_;
    //int samples_index_;
    //int samples_current_index_;
    //float sample_;
    //float amplitudHelper_;
    bool playing;
    float sample_rate_;
    ulong time_position_start;
    ulong time_position_current;
    float timer_;
    ModuleParam[] moduleParams_;
    int currentParamIndex_;
    float speed_;
}

