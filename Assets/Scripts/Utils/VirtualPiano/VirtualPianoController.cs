using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualPianoController : MonoBehaviour
{
    public Color keyPressColorWhite;
    public Color keyPressColorBlack;
    public Color keyReleaseColorWhite;
    public Color keyReleaseColorBlack;

    public Action<int, int> OnNoteOn;
    public Action<int, int> OnNoteOff;

    public void PressKeyWhite(Image img ) {
        PressKey(img, true);
    }

    public void PressKeyBlack(Image img) {
        PressKey(img, false);
    }

    public void ReleaseKeyWhite(Image img) {
        ReleaseKey(img, true);
    }

    public void ReleaseKeyBlack(Image img) {
        ReleaseKey(img, false);
    }

    void PressKey(Image img, bool isWhite) {
        img.color = isWhite ?  keyPressColorWhite : keyPressColorBlack;
    }

    void ReleaseKey(Image img, bool isWhite) {
        img.color = isWhite ? keyReleaseColorWhite : keyReleaseColorBlack;
    }

    public void NoteOn(int note) {
        OnNoteOn?.Invoke(note, 127);
    }

    public void NoteOff(int note) {
        OnNoteOff?.Invoke(note, 127);
    }
}
