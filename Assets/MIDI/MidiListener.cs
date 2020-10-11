using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiListener : MonoBehaviour
{
    public bool isLockedForManager = false;
    public Action<int, int> OnNoteOn;
    public Action<int, int> OnNoteOff;
}
