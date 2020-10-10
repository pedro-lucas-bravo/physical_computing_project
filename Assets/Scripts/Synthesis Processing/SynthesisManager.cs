using UnityEngine;
using System.Collections;

public class SynthesisManager : MonoBehaviour {
	

    public void SetSource(SynthModule sourceModule) {
        currentSource_ = sourceModule;
    }

    public void SetDestination(SynthModule destModule)
    {
        destModule.SetSourceModule(currentSource_);
        currentSource_ = null;
    }

    public void SetConnection(SynthModule sourceModule, SynthModule destModule) {
        sourceModule.SetSourceModule(destModule);
        destModule.SetSourceModule(sourceModule);
    }

    public SynthModule getCurrentSource() {
        return currentSource_;
    }

    SynthModule currentSource_;

    public bool thereIsSource() {
        return currentSource_ != null;
    }

}
