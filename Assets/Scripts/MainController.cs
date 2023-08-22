using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public OSC osc;
    public MidiRecorder midiRecorder1;
    public MidiRecorder midiRecorder2;
    public SynthModule[] synthChain;
    public SynthModule[] synthChain2;

    [Header("Audio Balance")]
    public AnimationCurve pitchCurve;
    public float minPitchVariation = 0;
    public float maxPitchVariation = 1;
    public Vector3 attitudeVariation = 0.25f * Vector3.one;
    public float percussionPitchVariation = 0.35f;

    [Header("Audio Sources")]
    public OSCEventReceiver oscEventPercusionPlay;
    public OSCEventReceiver oscEventPercusionTempo;
    public AudioSource[] percusionSources;
    public AudioLowPassFilter lpf;
    public float minLpf;
    public float maxLpf;
    public AudioChorusFilter chorusFilter;
    public OSCEventReceiver oscEventSynth1;
    public OSCEventReceiver oscEventSynth2;
    private void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;     
    }

    public Action<Vector3> OnReceivedAttitude;
    public Action<float> OnReceivedLight;

    private void OnDestroy() {
        if(oscEventPercusionPlay != null)
            oscEventPercusionPlay.OnEventReceived -= OnEventReceivedPercusionPlay;
        if (oscEventPercusionTempo != null)
            oscEventPercusionTempo.OnEventReceived -= OnEventReceivedPercusionTempo;
        if (oscEventSynth1 != null)
            oscEventSynth1.OnEventReceived -= OnEventReceivedSynth1;
        if (oscEventSynth2 != null)
            oscEventSynth2.OnEventReceived -= OnEventReceivedSynth2;
    }

    private void Update() {
        var audioGenerator = synthChain[0] as AudioGenerator;
        var pitchFromLight = pitchCurve.Evaluate(Mathf.Clamp(light_, 0, maxLightValue_ + 1) / (maxLightValue_ + 1));
        audioGenerator.soundOuput.pitch = Mathf.Lerp(minPitchVariation, maxPitchVariation, pitchFromLight);
        var attFactor = CalculateAttitudeFactor();        
        audioGenerator.pan = Mathf.Lerp(-1, 1, Mathf.Clamp01((attFactor.x - 1 + attitudeVariation.x)/(2 * attitudeVariation.x)));
        UpdatePercussionSources(attFactor);

        OnReceivedLight?.Invoke(Mathf.Clamp(light_, 0, maxLightValue_ + 1) / (maxLightValue_ + 1));
        OnReceivedAttitude?.Invoke(attFactor);
    }

    Func<float, float> funcCorrection = deg => deg > 360 ? deg - 360 : (deg < 0 ? deg + 360 : deg);
    private Vector3 CalculateAttitudeFactor() {
        var compensation = 180f * Vector3.one - refAttitudeValue_;
        var val = attitudeValue_ + compensation;
        val = new Vector3(funcCorrection(val.x), funcCorrection(val.y), funcCorrection(val.z));        
        return val / 180f;
    }

    void UpdatePercussionSources(Vector3 attFactor) {
        if (!modifyPercussionTempo_) return;
        //for (int i = 0; i < percusionSources.Length; i++) {
        //    percusionSources[i].pitch = 1 + Mathf.Lerp(-percussionPitchVariation, percussionPitchVariation, Mathf.Clamp01((attFactor.z - 1 + attitudeVariation.z) / (2 * attitudeVariation.z)));
        //}
        lpf.cutoffFrequency = Mathf.Lerp(minLpf, maxLpf, Mathf.Clamp01((attFactor.z - 1 + attitudeVariation.z) / (2 * attitudeVariation.z)));
    }

    private void Start() {
        osc.SetAddressHandler("/noteon", OnReceiveNoteOn);
        osc.SetAddressHandler("/noteoff", OnReceiveNoteOff);

        osc.SetAddressHandler("/light", OnReceiveLight);
        osc.SetAddressHandler("/maxlight", OnReceiveMaxLight);

        osc.SetAddressHandler("/attitude", OnReceiveAttitude);
        osc.SetAddressHandler("/refattitude", OnReceiveRefAttitude);

        osc.SetAddressHandler("/record", OnReceiveRecord);

        osc.SetAddressHandler("/globalspeed", OnReceiveGlobalSpeed);

        oscEventPercusionPlay.OnEventReceived += OnEventReceivedPercusionPlay;
        oscEventPercusionTempo.OnEventReceived += OnEventReceivedPercusionTempo;

        oscEventSynth1.OnEventReceived += OnEventReceivedSynth1;
        oscEventSynth2.OnEventReceived += OnEventReceivedSynth2;

        for (int i = 0; i < synthChain.Length - 1; i++) {
            synthChain[i].SetSourceModule(synthChain[i + 1]);
        }

        for (int i = 0; i < synthChain2.Length - 1; i++) {
            synthChain2[i].SetSourceModule(synthChain2[i + 1]);
        }
    }

    bool synth1_ = true, synth2_;
    private void OnEventReceivedSynth1(bool obj) {
        synth1_ = obj;
        midiRecorder1.midiListener.isLockedForManager = !obj;
    }

    private void OnEventReceivedSynth2(bool obj) {
        synth2_ = obj;
        if (obj)
            chorusFilter.depth = 0.51f;
        midiRecorder2.midiListener.isLockedForManager = !obj;
    }

    bool modifyPercussionTempo_;
    private void OnEventReceivedPercusionTempo(bool obj) {
        modifyPercussionTempo_ = obj;
    }

    private void OnEventReceivedPercusionPlay(bool obj) {
        for (int i = 0; i < percusionSources.Length; i++) {
            if(obj)
                percusionSources[i].Play();
            else
                percusionSources[i].Stop();
        }
    }

    private void OnReceiveRecord(OscMessage oscM) {
        if(synth1_)
            midiRecorder1.StartRecording(oscM.GetInt(0) == 1);
        if (synth2_)
            midiRecorder2.StartRecording(oscM.GetInt(0) == 1);
    }

    private void OnReceiveGlobalSpeed(OscMessage oscM) {
        Time.timeScale = oscM.GetFloat(0);
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
        light_ = oscM.GetFloat(0);
    }

    private void OnReceiveNoteOff(OscMessage message) {
        var note = message.GetInt(0);
        var velocity = message.GetInt(1);

        MIDI.MidiManager.Instance.NoteOff(note, velocity);
    }

    #region OSC Msgs

    void OnReceiveNoteOn(OscMessage message) {
        var note = message.GetInt(0);
        var velocity = message.GetInt(1);

        MIDI.MidiManager.Instance.NoteOn(note, velocity);
    }

    #endregion

    float light_;
    float maxLightValue_;
    Vector3 refAttitudeValue_;
    Vector3 attitudeValue_;
}
