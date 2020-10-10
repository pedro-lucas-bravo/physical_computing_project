using UnityEngine;
using System.Collections;

public class SqrOscGui : MonoBehaviour {

        public SynthesisManager synthesisManager;


        // Use this for initialization
        void Awake()
        {
            sqrOscillator = gameObject.AddComponent<SqrOscillator>();
            sqrOscillator.amplitude = 1;
            sqrOscillator.frequency = 440;

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnSelectOsc()
        {
            synthesisManager.SetSource(sqrOscillator);
        }

        public void SetFrequency(float freq)
        {
            sqrOscillator.frequency = freq;
        }

        SqrOscillator sqrOscillator;
    }
