using UnityEngine;
using System.Collections;

public abstract class SynthModule: MonoBehaviour {

    public enum Type { SourceWave, ChainOperator, SoloOperator, Envelope, Output}

    public int MaxInputs { get; set; }

    public Type synthType;

    public virtual float Frequency { get; set; }
    public virtual float Amplitude { get; set; }
    public virtual float Multiplier { get; set; }

    public virtual void SetSourceModule(SynthModule module) { }
    public virtual void RemoveSourceModule(SynthModule source) { }
    
    public abstract float PreCalculate();
    public abstract float Calculate(ulong t);

    public int usedInputs { get; set; }
}
