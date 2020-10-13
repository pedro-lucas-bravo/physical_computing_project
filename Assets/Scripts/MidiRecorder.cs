using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiRecorder : MonoBehaviour
{
    public MidiListener midiListener;
    public AudioSource basePercussionSource;
    public int baseBeatsNumber = 8;
    //public int BPM = 90;
    public int recordingBeatsSpace = 16;

    public Action OnBeat;

    private void Awake() {
        midiListener.OnNoteOn += OnNoteOn;
        midiListener.OnNoteOff += OnNoteOff;
        referenceBeatSamples_ = basePercussionSource.clip.samples / baseBeatsNumber;
        baseDspTime_ = AudioSettings.dspTime;
    }

    private void OnNoteOff(int note, int velocity) {
        RecordInfo(false, note, velocity);
    }

    private void OnNoteOn(int note, int velocity) {
        RecordInfo(true, note, velocity);
    }

    bool firstRecord_;
    void RecordInfo(bool isOn, int note, int velocity) {
        if (!record_) return;
        lockBaseTime_ = true;
        if (!firstRecord_) {
            var barLength = basePercussionSource.clip.length / (0.25f * baseBeatsNumber);
            var factor = Mathf.Abs((float)(AudioSettings.dspTime - baseDspTime_)) / barLength;
            if (factor < 0.25) {
                Debug.Log("After Beat " + factor);
            } else {
                Debug.Log("Before Beat " + factor);
                baseDspTime_ += barLength;
            }
            firstRecord_ = true;
        }
        notesSequence_.Add(new MidiInfo(isOn, AudioSettings.dspTime - baseDspTime_, note, velocity));
    }

    private void OnDestroy() {
        if (midiListener != null) {
            midiListener.OnNoteOn -= OnNoteOn;
            midiListener.OnNoteOff -= OnNoteOff;
        }
    }

    int referenceBeatSamples_;
    int lastBeat_;
    double baseDspTime_;
    double previousBaseDspTime_;
    long globalBeat_;
    bool lockBaseTime_;
    private void Update() {
        if (basePercussionSource.timeSamples / referenceBeatSamples_ != lastBeat_) {
            if (basePercussionSource.timeSamples < referenceBeatSamples_) {
                lastBeat_ = 0;                
            } else
                lastBeat_++;
            globalBeat_++;
            if (globalBeat_ % 4 == 0){
                if (!lockBaseTime_) {
                    previousBaseDspTime_ = baseDspTime_;
                    baseDspTime_ = AudioSettings.dspTime;
                    Debug.Log(lastBeat_);
                    if (notesSequence_.Count > 0)
                        lockBaseTime_ = true; //lock again to play
                }
                OnBeat?.Invoke();
            }                     
        }
            
        if (!record_ && lockBaseTime_) {
            int readyCounter = 0;
            for (int i = 0; i < notesSequence_.Count; i++) {
                if (AudioSettings.dspTime - baseDspTime_ >= notesSequence_[i].Time && !notesSequence_[i].Ready) {
                    notesSequence_[i].Ready = true;
                    if (notesSequence_[i].IsOn)
                        midiListener.OnNoteOn?.Invoke(notesSequence_[i].Note, notesSequence_[i].Velocity);
                    else
                        midiListener.OnNoteOff?.Invoke(notesSequence_[i].Note, notesSequence_[i].Velocity);
                }
                if (notesSequence_[i].Ready)
                    readyCounter++;
            }
            if (readyCounter == notesSequence_.Count) { //Reset, another loop
                for (int i = 0; i < notesSequence_.Count; i++) {
                   notesSequence_[i].Ready = false;
                }
                lockBaseTime_ = false;
            }
        }
    }

    public void StartRecording(bool start) {
        if (start) {
            notesSequence_.Clear();
            lockBaseTime_ = false;
            midiListener.isLockedForManager = false;
            firstRecord_ = false;
        }
        record_ = start;
        if (!start) {
            if (lockBaseTime_) // was locked
                midiListener.isLockedForManager = true;
            lockBaseTime_ = false;
        }
    }

    [System.Serializable]
    class MidiInfo {
        public bool IsOn;
        public double Time;
        public int Note;
        public int Velocity;
        public bool Ready;

        public MidiInfo(bool isOn, double time, int note, int velocity, bool ready = false) {
            IsOn = isOn;
            Time = time;
            Note = note;
            Velocity = velocity;
            Ready = ready;
        }
    }

    List<MidiInfo> notesSequence_ =  new List<MidiInfo>();
    bool record_;
}
