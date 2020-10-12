using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCEventReceiver : MonoBehaviour
{
    public OSC osc;
    public string adress;

    public Action<bool> OnEventReceived;
    private void Start() {
        osc.SetAddressHandler(adress, OnReceive);
    }

    private void OnReceive(OscMessage oscM) {
        OnEventReceived?.Invoke(oscM.GetInt(0) == 1);
    }
}
