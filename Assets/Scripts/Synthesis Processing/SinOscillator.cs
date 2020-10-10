using UnityEngine;
using System.Collections;
using System;

public class SinOscillator : SynthModule {

    public float amplitude;
    public float multiplier = 1;
    public float frequency;
    

    public override float Frequency {
        get {
            return frequency;
        }

        set {
            frequency = value;
        }
    }

    public override float Amplitude {
        get {
            return amplitude;
        }

        set {
            amplitude = value;
        }
    }

    public override float Multiplier {
        get {
            return multiplier;
        }

        set {
            multiplier = value;
        }
    }

    void Awake() {
        sample_rate_ = AudioSettings.outputSampleRate; 
        MaxInputs = 0;
    }


    public override float Calculate(ulong t)
    {
        return amplitude * multiplier * Mathf.Sin((float)t * fx_);
    }

    public override float PreCalculate()
    {
        fx_ = 2 * Mathf.PI * frequency / sample_rate_;
        return fx_;
    }

    float sample_rate_;
    float fx_;
}


//void OnAudioRead(float[] data)
//{
//    for (int i = 0; i < data.Length; i++)
//    {
//        switch (wave)
//        {
//            case waveType.Sine:
//                data[i] = Mathf.Sin(2 * Mathf.PI * frequency * position / sampleRate);
//                break;
//            case waveType.Sawtooth:
//                data[i] = (Mathf.PingPong(frequency * position / sampleRate, 0.5f));
//                break;
//            case waveType.Square:
//                data[i] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / sampleRate)) * 0.5f;
//                break;
//            case waveType.Noise:
//                data[i] = Mathf.PerlinNoise(frequency * position / sampleRate, 0);
//                break;
//        }
//        position++;
//    }
//float Triangle(float minLevel, float maxLevel, float period, float phase, float t) {
//    float pos = Mathf.Repeat(t - phase, period) / period;

//    if (pos < .5f) {
//        return Mathf.Lerp(minLevel, maxLevel, pos * 2f);
//    } else {
//        return Mathf.Lerp(maxLevel, minLevel, (pos - .5f) * 2f);
//    }
//}