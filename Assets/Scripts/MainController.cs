using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public OSC osc;
    public AudioGenerator audioGenerator;
    public SynthModule[] synthChain;

    [Header("Audio Balance")]
    public AnimationCurve pitchCurve;
    public float minPitchVariation = 0;
    public float maxPitchVariation = 1;

    private void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;     
    }

    private void OnDestroy() {
    }

    private void Update() {
        audioGenerator.soundOuput.pitch = Mathf.Lerp(minPitchVariation, maxPitchVariation, pitchCurve.Evaluate(Mathf.Clamp(light_, 0, maxLightValue_ + 1 ) / (maxLightValue_ + 1)));
    }

    private void Start() {
        osc.SetAddressHandler("/noteon", OnReceiveNoteOn);
        osc.SetAddressHandler("/noteoff", OnReceiveNoteOff);
        osc.SetAddressHandler("/light", OnReceiveLight);
        osc.SetAddressHandler("/maxlight", OnReceiveMaxLight);

        for (int i = synthChain.Length - 1; i >= 1; i--) {
            synthChain[i].SetSourceModule(synthChain[i - 1]);
        }
        audioGenerator.SetSourceModule(synthChain[synthChain.Length - 1]);
    }

    private void OnReceiveMaxLight(OscMessage oscM) {
        maxLightValue_ = oscM.GetInt(0);
    }

    private void OnReceiveLight(OscMessage oscM) {
        light_ = oscM.GetInt(0);
    }

    private void OnReceiveNoteOff(OscMessage message) {
        var note = message.GetInt(0);
        var velocity = message.GetInt(1);

        MIDI.MidiManager.OnNoteOff?.Invoke(note, velocity);
    }

    #region OSC Msgs

    void OnReceiveNoteOn(OscMessage message) {
        var note = message.GetInt(0);
        var velocity = message.GetInt(1);

        MIDI.MidiManager.OnNoteOn?.Invoke(note, velocity);
    }

    #endregion

    float light_;
    float maxLightValue_;
}
