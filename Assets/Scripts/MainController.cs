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
    public float attitudeVariation = 0.25f;

    [Header("Audio Sources")]

    public AudioSource[] sources;

    private void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;     
    }

    private void OnDestroy() {
    }

    private void Update() {
        audioGenerator.soundOuput.pitch = Mathf.Lerp(minPitchVariation, maxPitchVariation, pitchCurve.Evaluate(Mathf.Clamp(light_, 0, maxLightValue_ + 1 ) / (maxLightValue_ + 1)));
        var attFactor = CalculateAttitudeFactor();
        sources[0].panStereo = Mathf.Lerp(-1, 1, Mathf.Clamp01((attFactor.x - 1 + attitudeVariation)/(2 * attitudeVariation)));
    }

    Func<float, float> funcCorrection = deg => deg > 360 ? deg - 360 : (deg < 0 ? deg + 360 : deg);
    private Vector3 CalculateAttitudeFactor() {
        var compensation = 180f * Vector3.one - refAttitudeValue_;
        var val = attitudeValue_ + compensation;
        val = new Vector3(funcCorrection(val.x), funcCorrection(val.y), funcCorrection(val.z));        
        return val / 180f;
    }

    private void Start() {
        osc.SetAddressHandler("/noteon", OnReceiveNoteOn);
        osc.SetAddressHandler("/noteoff", OnReceiveNoteOff);

        osc.SetAddressHandler("/light", OnReceiveLight);
        osc.SetAddressHandler("/maxlight", OnReceiveMaxLight);

        osc.SetAddressHandler("/attitude", OnReceiveAttitude);
        osc.SetAddressHandler("/refattitude", OnReceiveRefAttitude);

        for (int i = synthChain.Length - 1; i >= 1; i--) {
            synthChain[i].SetSourceModule(synthChain[i - 1]);
        }
        audioGenerator.SetSourceModule(synthChain[synthChain.Length - 1]);
    }

    private void OnReceiveRefAttitude(OscMessage oscM) {
        refAttitudeValue_ = new Vector3(oscM.GetInt(0), oscM.GetInt(1), oscM.GetInt(2));
    }

    private void OnReceiveAttitude(OscMessage oscM) {
        attitudeValue_ = new Vector3(oscM.GetInt(0), oscM.GetInt(1), oscM.GetInt(2));
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
    Vector3 refAttitudeValue_;
    Vector3 attitudeValue_;
}
