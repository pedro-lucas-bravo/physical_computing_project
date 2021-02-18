using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalizedController : MonoBehaviour
{
    public OSC osc;
    public int number;
    public string command;
    public Transform target;
    public float minValue = 0;
    public float maxValue = 1;

    // Start is called before the first frame update
    private void Start() {
        osc.SetAddressHandler(command, OnReceiveEffect);
    }
    private void OnReceiveEffect(OscMessage oscM) {
        var trackNumberFromOSC = oscM.GetInt(0);
        if (trackNumberFromOSC == number) {
            var val = oscM.GetFloat(1);
            var scale = Mathf.Lerp(minValue, maxValue, Mathf.Clamp01(val));
            target.localScale = scale * Vector3.one;
        }
    }
}
