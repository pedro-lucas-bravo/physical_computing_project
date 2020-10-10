using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public VirtualPianoController virtualPiano;
    public AudioGenerator audioGenerator;
    public SynthModule[] synthChain;

    //[Header("Audio Balance")]
    //public float maxAmplitude = 1f;

    private void Awake() {
        virtualPiano.OnNoteOn += OnNoteOn;
        virtualPiano.OnNoteOff += OnNoteOff;        
    }

    private void OnDestroy() {
        if (virtualPiano != null) {
            virtualPiano.OnNoteOn -= OnNoteOn;
            virtualPiano.OnNoteOff -= OnNoteOff;
        }
    }

    private void Start() {
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
}
