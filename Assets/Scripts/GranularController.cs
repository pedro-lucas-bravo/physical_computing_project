using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranularController : MonoBehaviour
{
    public OSC osc;
    public int number;
    public ParticleSystem particles;
    public float minRate = 0;
    public float maxRate = 20f;
    public float minSize = 0.25f;
    public float maxSize = 3f;
    
    // Start is called before the first frame update
    private void Start() {
        osc.SetAddressHandler("/granular", OnReceiveEffect);
    }
    private void OnReceiveEffect(OscMessage oscM) {
        var trackNumberFromOSC = oscM.GetInt(0);
        if (trackNumberFromOSC == number) {
            var grainFreq = oscM.GetFloat(1);
            var grainDuration = oscM.GetFloat(2);

            particles.startSpeed = Mathf.Lerp(minRate, maxRate, Mathf.Clamp01(grainFreq / 80));
            particles.startSize = Mathf.Lerp(minSize, maxSize, Mathf.Clamp01(grainDuration / 500));
        }
    }
}
