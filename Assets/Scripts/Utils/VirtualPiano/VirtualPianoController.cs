using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class VirtualPianoController : MonoBehaviour
{
    public Color keyPressColorWhite;
    public Color keyPressColorBlack;
    public Color keyReleaseColorWhite;
    public Color keyReleaseColorBlack;

    public GameObject[] keysGameObjects;

    public Action<int, int> OnNoteOn;
    public Action<int, int> OnNoteOff;

    private void Awake() {
        EnhancedTouchSupport.Enable();
    }

    private void OnDestroy() {
        EnhancedTouchSupport.Disable();
    }

    private void Update() {
       var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
        if (touches == 0 && notesEnabled_.ContainsValue(1)) {
            for (int i = 0; i < keysGameObjects.Length; i++) {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(keysGameObjects[i], pointer, ExecuteEvents.pointerExitHandler);
            }
        }
    }

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
        if (!notesEnabled_.TryGetValue(note, out int enable))
            notesEnabled_.Add(note, 1);
        else
            notesEnabled_[note] = 1;
        OnNoteOn?.Invoke(note, 127);
    }

    public void NoteOff(int note) {
        if (!notesEnabled_.TryGetValue(note, out int enable))
            notesEnabled_.Add(note, 0);
        else
            notesEnabled_[note] = 0;
        OnNoteOff?.Invoke(note, 127);
    }

    readonly Dictionary<int, int> notesEnabled_ = new Dictionary<int, int>();
}
