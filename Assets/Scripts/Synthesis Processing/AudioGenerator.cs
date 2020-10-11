using UnityEngine;
using System.Collections;
using System;

public class AudioGenerator : SynthModule {

    public AudioSource soundOuput;
    public float frequency = 440;
    public float gain = 0.05f;
    public float pan = 0;

    private ulong position;

    private void Awake() {
        sample_rate_ = AudioSettings.outputSampleRate;
        enabled = false;
        MaxInputs = 1;
    }

    SynthModule module;

    public bool isPlaying {
        get { return module != null; }
    }

    public override void RemoveSourceModule(SynthModule source) {
        module = null;
        enabled = false;
    }

    public override void SetSourceModule(SynthModule module)
    {
        SetCurrentModule(module);
    }


    public void SetCurrentModule(SynthModule newModule) {
        if (newModule != null)
        {
            module = newModule;
            enabled = true;
            soundOuput.Play();
        }
        else {
            enabled = false;
        }
    }

    float sample;
    void OnAudioFilterRead(float[] data, int channels)
    {
        module.PreCalculate();
        for (var i = 0; i < data.Length; i = i + channels)
        {
            sample = gain * module.Calculate(position);
            data[i] = sample * Mathf.Lerp(1, 0, Mathf.Clamp(pan, 0, 1));  
            if (channels == 2) data[i + 1] = sample * Mathf.InverseLerp(-1, 0, Mathf.Clamp(pan, -1, 0));
            position++;
        }
    }

    public override float PreCalculate()
    {
        return 0;
    }

    public override float Calculate(ulong t)
    {
        return 0;
    }

    int sample_rate_;
}
