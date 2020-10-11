using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainControllerMobile : MonoBehaviour
{
    [Header("OSC")]
    public OSC osc;
    public InputField ipAddress;

    [Header("Sensors")]
    public VirtualPianoController virtualPiano;
    public Text lightData;

    private void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (LightSensor.current != null)
            InputSystem.EnableDevice(LightSensor.current);
        else
            lightData.text = "NO";

        virtualPiano.OnNoteOn += OnNoteOn;
        virtualPiano.OnNoteOff += OnNoteOff;

        if (PlayerPrefs.HasKey("ip")) {
            MainControllerMobile.IpAdress = ipAddress.text = PlayerPrefs.GetString("ip");
        } else {
            ipAddress.text = MainControllerMobile.IpAdress;
        }
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
            var message = new OscMessage();
            message.address = "/light";
            message.values.Add(light_);
            osc.Send(message);
        }

    }

    private void Start() {
        CalibrateLightSensor();
    }

    private void OnNoteOn(int note, int velocity) {
        var message = new OscMessage();
        message.address = "/noteon";
        message.values.Add(note);
        message.values.Add(velocity);
        osc.Send(message);
    }

    private void OnNoteOff(int note, int velocity) {
        var message = new OscMessage();
        message.address = "/noteoff";
        message.values.Add(note);
        message.values.Add(velocity);
        osc.Send(message);
    }

    public void CalibrateLightSensor() {
        if (LightSensor.current != null) {
            if (!LightSensor.current.enabled)
                InputSystem.EnableDevice(LightSensor.current);
            var maxLightValue_ = LightSensor.current.lightLevel.ReadValue();
            var message = new OscMessage();
            message.address = "/maxlight";
            message.values.Add(maxLightValue_);
            osc.Send(message);
        }
    }

    public static string IpAdress = "127.0.0.1";
    public void SetIpAdressAndRestart() {
        IpAdress = ipAddress.text.Trim();
        PlayerPrefs.SetString("ip", IpAdress);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    float light_;
}
