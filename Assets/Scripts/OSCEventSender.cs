using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCEventSender : MonoBehaviour
{
    public OSC osc;
    public string address;
    public void Send(bool send) {
        var message = new OscMessage();
        message.address = address;
        message.values.Add(send ? 1: 0);
        osc.Send(message);
    }
}
