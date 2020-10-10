using UnityEngine;
using System.Collections;

public class AudioOutputGui : MonoBehaviour {

    public SynthesisManager synthesisManager;
    public AudioGenerator audioOuput;

    public void OnPointer() {
        if (synthesisManager.thereIsSource()) {
            //if (audioOuput.enabled && lastLine_ != null)
            //    lastLine_.GetComponent<LineHandler>().DestroyLine();
            synthesisManager.SetDestination(audioOuput);
            //lastLine_ = synthesisManager.drawLines.getCurrentLine();
        }
    }

    GameObject lastLine_;
}
