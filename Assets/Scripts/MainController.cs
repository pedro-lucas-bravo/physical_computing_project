using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public VirtualPianoController virtualPiano;
    public AudioGenerator audioGenerator;
    public SynthModule[] synthChain;
    public Text lightData;

    [Header("Audio Balance")]
    public AnimationCurve pitchCurve;
    public float minPitchVariation = 0;
    public float maxPitchVariation = 1;

    private void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (LightSensor.current != null)
            InputSystem.EnableDevice(LightSensor.current);
        else
            lightData.text = "NO";

        virtualPiano.OnNoteOn += OnNoteOn;
        virtualPiano.OnNoteOff += OnNoteOff;        
    }

    private void OnDestroy() {
        if (virtualPiano != null) {
            virtualPiano.OnNoteOn -= OnNoteOn;
            virtualPiano.OnNoteOff -= OnNoteOff;
        }
        if (LightSensor.current != null)
            InputSystem.DisableDevice(LightSensor.current);
    }

    private void Update() {
        if (LightSensor.current != null) {
            light_ = LightSensor.current.lightLevel.ReadValue();
            lightData.text = light_ + "";
            audioGenerator.soundOuput.pitch = Mathf.Lerp(minPitchVariation, maxPitchVariation, pitchCurve.Evaluate(Mathf.Clamp(light_, 0, maxLightValue_ + 1 ) / (maxLightValue_ + 1)));
        }

    }

    private void Start() {
        CalibrateLightSensor();
        for (int i = synthChain.Length - 1; i >= 1; i--) {
            synthChain[i].SetSourceModule(synthChain[i - 1]);
        }
        audioGenerator.SetSourceModule(synthChain[synthChain.Length - 1]);
    }

    private void OnNoteOn(int note, int velocity) {
        MIDI.MidiManager.OnNoteOn?.Invoke(note, velocity);
    }

    private void OnNoteOff(int note, int velocity) {
        MIDI.MidiManager.OnNoteOff?.Invoke(note, velocity);
    }

    public void CalibrateLightSensor() {
        if (LightSensor.current != null) {
            if (!LightSensor.current.enabled)
                InputSystem.EnableDevice(LightSensor.current);
            maxLightValue_ = LightSensor.current.lightLevel.ReadValue();
        }
    }

    float light_;
    float maxLightValue_;
}
