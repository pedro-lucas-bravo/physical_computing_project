using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VisualsController : MonoBehaviour
{

    public MidiRecorder midiRecorder;
    public MidiListener[] midiListeners;
    public MainController mainController;
    public Animator camAnimator;
    public PostProcessVolume processVolume;
    public ParticleSystem panParticles;
    public GameObject noteBallPrefab;
    public Renderer centralObjectRender;
    //[Header("Objects")]


    [Header("Balance")]
    public float camAngleFactor;
    public float camRotationSmoothTime;
    public Gradient gradientLightColor;
    public bool useAlfaOnGradient = true;
    public bool activatePanParticales = true;
    public float panLimits = 5;
    public Vector2 noteBallSpawnRange;
    public float timeToDestroyNotaBall;
    public float lensIntensityLimits;
    public float angleSkySpeed;    


    //ColorGrading colorGrading;   
    Bloom bloom_;
    LensDistortion lensDistortion_;

    private void Start() {
        for (int i = 0; i < midiListeners.Length; i++) {
            midiListeners[i].OnNoteOn += OnNoteOn;
            midiListeners[i].OnNoteOff += OnNoteOff;
        }      

        midiRecorder.OnBeat += OnBarBeat;

        mainController.OnReceivedAttitude += OnReceivedAttitude;
        mainController.OnReceivedLight += OnReceivedLight;

        processVolume.profile.TryGetSettings(out bloom_);
        processVolume.profile.TryGetSettings(out lensDistortion_);
    }

    private void OnReceivedLight(float light) {
        //colorGrading.colorFilter.Override(gradientLightColor.Evaluate(light));
        var color = gradientLightColor.Evaluate(light);
        bloom_.color.Override(color);
        panParticles.startColor = color;
        var originalCentralObjectColor = centralObjectRender.material.color;
        if (!useAlfaOnGradient)
            color.a = originalCentralObjectColor.a;
        centralObjectRender.material.color = color;
    }

    Vector3 attitude_;
    private void OnReceivedAttitude(Vector3 att) {
        attitude_ = att;        
    }

    private void OnBarBeat() {
        camAnimator.Play("BeatCam",0,0);
    }

    private void OnDestroy() {
        for (int i = 0; i < midiListeners.Length; i++) {
            if (midiListeners[i] != null) {
                midiListeners[i].OnNoteOn -= OnNoteOn;
                midiListeners[i].OnNoteOff -= OnNoteOff;
            }
        }

        if (midiRecorder != null)
            midiRecorder.OnBarBeat -= OnBarBeat;
    }

    private void OnNoteOn(int note, int velocity) {
        var noteBall = Instantiate(noteBallPrefab);
        noteBall.SetActive(true);
        noteBall.transform.position = new Vector3(
            UnityEngine.Random.Range(-noteBallSpawnRange.x, noteBallSpawnRange.x),
            UnityEngine.Random.Range(-noteBallSpawnRange.x, noteBallSpawnRange.y),
            0);
        noteBall.GetComponent<Renderer>().material.color = gradientLightColor.Evaluate(Mathf.Clamp01((note - 60f) / (84f - 60f)));
        Destroy(noteBall, timeToDestroyNotaBall);
    }

    private void OnNoteOff(int note, int velocity) {

    }

    float lastZ_;
    private void Update() {
        var vel = Vector3.zero;
        var currentEuler = new Vector3(0,0,lastZ_);
        var modified = currentEuler;
        //var x = Mathf.Lerp(-camAngleFactor, camAngleFactor, Mathf.Clamp01((attitude_.x - 1 + mainController.attitudeVariation.x) / (2 * mainController.attitudeVariation.x)));
        //var y = Mathf.Lerp(-camAngleFactor, camAngleFactor, Mathf.Clamp01((attitude_.y - 1 + mainController.attitudeVariation.y) / (2 * mainController.attitudeVariation.y)));
        var attZ = Mathf.Clamp01((attitude_.z - 1 + mainController.attitudeVariation.z) / (2 * mainController.attitudeVariation.z));
        lastZ_ = modified.z = Mathf.Lerp(-camAngleFactor, camAngleFactor, attZ);
        Camera.main.transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(currentEuler, modified, ref vel, camRotationSmoothTime));

        var panPos = panParticles.transform.position;
        panPos.x = Mathf.Lerp(-panLimits, panLimits, Mathf.Clamp01((attitude_.x - 1 + mainController.attitudeVariation.x) / (2 * mainController.attitudeVariation.x)));
        if(activatePanParticales)
            panParticles.transform.position = panPos;

        lensDistortion_.intensity.Override(Mathf.Lerp(-lensIntensityLimits, lensIntensityLimits, attZ));

        UpdateSkyboxMaterial();
    }

    float angleSky_;
    void UpdateSkyboxMaterial() {
        angleSky_ += angleSkySpeed * Time.deltaTime;
        if (angleSky_ >= 360)
            angleSky_ = 0;
        RenderSettings.skybox.SetFloat("_Rotation", angleSky_);
    }
}
