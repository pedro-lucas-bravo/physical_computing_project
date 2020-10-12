using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualsController : MonoBehaviour
{

    public MidiRecorder midiRecorder;
    public MainController mainController;

    //[Header("Objects")]


    [Header("Balance")]
    public float camAngleFactor;
    public float camRotationSmoothTime;

    private void Start() {
        MIDI.MidiManager.OnNoteOn += OnNoteOn;
        MIDI.MidiManager.OnNoteOff += OnNoteOff;

        midiRecorder.OnBeat += OnBeat;

        mainController.OnReceivedAttitude += OnReceivedAttitude;
    }


    Vector3 attitude_;
    private void OnReceivedAttitude(Vector3 att) {
        attitude_ = att;        
    }

    private void OnBeat() {
        
    }

    private void OnDestroy() {
        MIDI.MidiManager.OnNoteOn -= OnNoteOn;
        MIDI.MidiManager.OnNoteOff -= OnNoteOff;

        if(midiRecorder != null)
            midiRecorder.OnBeat -= OnBeat;
    }

    private void OnNoteOn(int note, int velocity) {

    }

    private void OnNoteOff(int note, int velocity) {

    }

    private void Update() {
        var vel = Vector3.zero;
        var currentEuler = Camera.main.transform.rotation.eulerAngles;
        var modified = currentEuler;
        //var x = Mathf.Lerp(-camAngleFactor, camAngleFactor, Mathf.Clamp01((attitude_.x - 1 + mainController.attitudeVariation.x) / (2 * mainController.attitudeVariation.x)));
        //var y = Mathf.Lerp(-camAngleFactor, camAngleFactor, Mathf.Clamp01((attitude_.y - 1 + mainController.attitudeVariation.y) / (2 * mainController.attitudeVariation.y)));
        modified.z = Mathf.Lerp(-camAngleFactor, camAngleFactor, Mathf.Clamp01((attitude_.z - 1 + mainController.attitudeVariation.z) / (2 * mainController.attitudeVariation.z)));
        Camera.main.transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(currentEuler, modified, ref vel, camRotationSmoothTime));
    }
}
