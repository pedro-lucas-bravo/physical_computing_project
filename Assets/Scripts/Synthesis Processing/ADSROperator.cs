using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class ADSROperator : SynthModule, IParametersModule {

    public enum State { None, Attack, Decay, Sustain, Release }

    //public LineHandler InputLine { get; set; }
    //public LineHandler OutputLine { get; set; }

    [System.Serializable]
    class SynthInfo {
        
        public float frequency;               
        public State state;

        public SynthInfo(ADSROperator adsrOp) {
            adsrOp_ = adsrOp;
            Reset();
        }

        public void Reset() {                        
            frequency = 440;
            state = State.None;
            timer_ = 0;
            amp_ = 0;
            lastAmp_ = 0;
            midiNumber_ = 0;
            lastState_ = State.None;
            velocityFactor_ = 0;
        }

        public void Play(int midiNumber, int velocity) {
            midiNumber_ = midiNumber;            
            frequency = MIDI.MidiManager.FromMIDItoFREQ(midiNumber);
            lastState_ = State.Attack;            
            setLastAmp_ = true;
            lastVelocityFactor_ = velocity / 127.0f;
        }

        public void Stop() {
            lastState_ = State.Release;            
            setLastAmp_ = true;
        }

        public bool IsPlaying {
            get {
                return state != State.None;
            }
        }

        public float Calculate(ulong t) {
            wasRemoved_ = false;
            adsrOp_.ModuleEnv.Frequency = frequency;
            adsrOp_.ModuleEnv.PreCalculate();
            var lastState = state;
            if (setLastAmp_) {
                lastAmp_ = amp_;
                timer_ = 0;
                setLastAmp_ = false;
                state = lastState_;
                velocityFactor_ = lastVelocityFactor_;
            }
            amp_ = adsrOp_.PerformEnvelope(ref state, ref timer_, velocityFactor_, lastAmp_);
            if (state == State.None && lastState == State.Release) {//Remove from array
                adsrOp_.RemoveFromMidiArray(midiNumber_);
                wasRemoved_ = true;
            }
            return amp_ * adsrOp_.ModuleEnv.Calculate(t);
        }


        public int MidiNumber {
            get { return midiNumber_; }
        }

        public bool WasRemoved {
            get { return wasRemoved_; }
        }

        int timer_;
        float lastAmp_;
        float amp_;
        ADSROperator adsrOp_;
        int midiNumber_;
        bool wasRemoved_;
        bool setLastAmp_;
        State lastState_;
        float velocityFactor_;
        float lastVelocityFactor_;
    }

    public MidiListener midiListener;
    public float attack;
    public float decay;
    public float sustain;
    public float release;
    public Color smoothnessColor;
    public TextMesh smoothnessLabel;

    SynthModule ModuleEnv { get; set; }

    void Awake() {
        MaxInputs = 1;
        sample_rate_ = AudioSettings.outputSampleRate;
        midiListener.OnNoteOn += OnNoteOn;
        midiListener.OnNoteOff += OnNoteOff;
        //amp_ = 0;
        int maxIndex = 200;
        midi_indexes_ = new int[maxIndex];
        synth_infos_midi = new SynthInfo[maxIndex];
        for (int i = 0; i < synth_infos_midi.Length; i++) {
            synth_infos_midi[i] = new SynthInfo(this);
        }
        SetSmoothness(0f);
        //Params
        moduleParams_ = new ModuleParam[1];
        moduleParams_[0] = new ModuleParam("Suavidad", smoothness_ * 100.0f, 0, 100, 0.01f, smoothnessColor, SetSmoothness);
    }

    float smoothness_;
    public float minAttack_ = 0.05f;
    public float maxAttack_ = 2f;
    public float minRelease_ = 0.1f;
    private readonly float maxRelease_ = 5f;

    private void SetSmoothness(float sm) {
        smoothness_ = sm;
        attack = minAttack_ + smoothness_ * (maxAttack_ - minAttack_);
        decay = 0;
        sustain = 1f;
        release = minRelease_ + smoothness_ * (maxRelease_ - minRelease_);
        //smoothnessLabel.text = Mathf.CeilToInt(sm * 100) + "";
    }

    private void OnDestroy() {
        if (midiListener != null) {
            midiListener.OnNoteOn -= OnNoteOn;
            midiListener.OnNoteOff -= OnNoteOff;
        }
    }

    public override void RemoveSourceModule(SynthModule source) {
        for (int i = 0; i < size_infos_; i++) {
            var synthInfo = synth_infos_midi[i];
            synthInfo.Reset();
        }
        size_infos_ = 0;
        ModuleEnv = null;
    }

    public override void SetSourceModule(SynthModule module) {
        if (module != this) {
            ModuleEnv = module;
        }
    }

    public override float Calculate(ulong t) {
        var result = 0f;
        for (int i = 0; i < size_infos_; i++) {
            var synthInfo = synth_infos_midi[i];
            result += synthInfo.Calculate(t);
            if (synthInfo.WasRemoved)
                i--;
        }
        return result;
    }

    public override float PreCalculate() {
        if (ModuleEnv != null)
            return ModuleEnv.PreCalculate();
        else
            return 1;
    }

    void OnNoteOn(int noteNumber, int velocity) {
        if(ModuleEnv != null)
            PlayNote(noteNumber, velocity);
    }

    void OnNoteOff(int noteNumber, int velocity) {
        if (ModuleEnv != null)
            StopNote(noteNumber); 
    }

    //Set parameters

    public void SetAttack(float val) {
        attack = val;
    }

    public void SetDecay(float val) {
        decay = val;
    }

    public void SetSustain(float val) {
        sustain = val;
    }

    public void SetRelease(float val) {
        release = val;
    }

    //Simultaneous MIDI management
    float currentTime_;
    void Update() {
        currentTime_ = Time.time;
    }

    void PlayNote(int midiNumber, int velocity) {
        var synthInfo = synth_infos_midi[midi_indexes_[midiNumber]];
        if (synthInfo.MidiNumber == midiNumber && synthInfo.IsPlaying) {
            synth_infos_midi[midi_indexes_[midiNumber]].Play(midiNumber, velocity);
        } else {
            midi_indexes_[midiNumber] = size_infos_;
            synth_infos_midi[size_infos_].Play(midiNumber, velocity);
            size_infos_++;
        }
        //LoggingSystem.Instance.Write_MIDI(targetName_, 1, midiNumber, velocity, currentTime_);
    }

    void StopNote(int midiNumber) {
        synth_infos_midi[midi_indexes_[midiNumber]].Stop();// Resolve size infos internally after release
        //LoggingSystem.Instance.Write_MIDI(targetName_, 0, midiNumber, 0, currentTime_);
    }

    void RemoveFromMidiArray(int midiNumber) {
        size_infos_--;
        int chagingIndex = midi_indexes_[midiNumber];
        var changingInfo = synth_infos_midi[chagingIndex];
        synth_infos_midi[chagingIndex] = synth_infos_midi[size_infos_];//copy        
        synth_infos_midi[size_infos_] = changingInfo;
        midi_indexes_[synth_infos_midi[chagingIndex].MidiNumber] = chagingIndex;
        synth_infos_midi[size_infos_].Reset();//Reset because it was a copy
    }

    public float PerformEnvelope(ref State st, ref int timer, float refAmp, float lastAmp) {
        var amp = 0f;
        switch (st) {
            case State.None:
                break;
            case State.Attack:
                timer += 1;
                amp = Mathf.Approximately(attack, 0) ? refAmp : lastAmp +  (timer * (refAmp - lastAmp)) / (attack * sample_rate_);
                if (timer / sample_rate_ >= attack) {
                    st = State.Decay;
                    timer = 0;
                    amp = refAmp;
                }
                break;
            case State.Decay:
                timer += 1;
                amp = Mathf.Approximately(decay, 0) ? sustain * refAmp : timer * refAmp * (sustain - 1) / (decay * sample_rate_) + refAmp;
                if (timer / sample_rate_ >= decay) {
                    st = State.Sustain;
                    amp = sustain * refAmp;
                }
                break;
            case State.Sustain:
                amp = sustain * refAmp;
                break;
            case State.Release:
                timer += 1;
                amp = Mathf.Approximately(release, 0) ? 0 : lastAmp * (1 - timer / (release * sample_rate_));
                if (timer / sample_rate_ >= release) {
                    st = State.None;
                    timer = 0;
                    amp = 0;
                }
                break;
            default:
                break;
        }
        return amp;
    }

    #region Param Control Implementation

    public SynthModule GetModule() {
        return this;
    }

    public void SetParameter(string paramName, float value) {
        ModuleParam.SetParameter(moduleParams_, paramName, value);
    }

    public ModuleParam GetNextParameterToControl() {
        return ModuleParam.GetNextParameterToControl(moduleParams_, ref currentParamIndex_);
    }

    public bool Disconnected() {
        return ModuleParam.Disconnect(moduleParams_, ref currentParamIndex_);
    }

    public void Restart() {
        currentParamIndex_ = 0;
    }

    ModuleParam[] moduleParams_;
    int currentParamIndex_;

    #endregion

    float sample_rate_;
    int[] midi_indexes_;
    SynthInfo[] synth_infos_midi;
    int size_infos_;
}