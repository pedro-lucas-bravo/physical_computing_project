using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakersController : MonoBehaviour
{
    public OSC osc;
    public int trackNumber;
    public Animator animator;
    // Start is called before the first frame update
    private void Start() {
        osc.SetAddressHandler("/track", OnReceiveLight);
    }
    private void OnReceiveLight(OscMessage oscM) {
        var trackNumberFromOSC = oscM.GetInt(0);
        if (trackNumberFromOSC == trackNumber) {
            var activate = oscM.GetInt(1);
            if (activate == 1)
                animator.Play("MoveSpeaker");
            else
                animator.Play("StaySpeaker");
        }
    }
}
