using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SynthOperatorGui : MonoBehaviour {

    public SynthesisManager synthesisManager;
    public SynthModule synthOperator;

    //void Awake() {
    //    sumOperator_ = gameObject.AddComponent<SumOperator>();
    //}

    public void OnSelect() {
        synthesisManager.SetSource(synthOperator);
    }

    public void OnPointer()
    {
        if (synthesisManager.thereIsSource())
        {
            synthesisManager.SetDestination(synthOperator);
        }
    }    

    //SumOperator sumOperator_;
}
