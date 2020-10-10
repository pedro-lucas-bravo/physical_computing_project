using UnityEngine;
using System.Collections;

public class SqrOscillator : SynthModule
{

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

    void Awake()
    {
        sample_rate_ = AudioSettings.outputSampleRate;
        MaxInputs = 0;
    }


    public override float Calculate(ulong t)
    {
        return amplitude * multiplier * Mathf.Sign(Mathf.Sin((float)t * fx_));
    }

    public override float PreCalculate()
    {
        fx_ = 2 * Mathf.PI * frequency / sample_rate_;
        return fx_;
    }

    float sample_rate_;
    float fx_;
}
