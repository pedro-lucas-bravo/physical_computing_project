using UnityEngine;
using System.Collections.Generic;
#if UNITY_STANDALONE || UNITY_EDITOR
//using NAudio.Midi;
#endif
using System;

namespace MIDI {

    //public struct NoteEvent{
    //    public int NoteNumber;
    //    public int Velocity;

    //    public NoteEvent(int noteNumberVal, int velocityVal) {
    //        NoteNumber = noteNumberVal;
    //        Velocity = velocityVal;
    //    }
    //}

    public class MidiManager: MonoBehaviour {

        public static MidiManager Instance;

        public static Action<int, int> OnNoteOn;
        public static Action<int, int> OnNoteOff;

//#if UNITY_STANDALONE || UNITY_EDITOR
//        public class MidiInInfo {            
//            public NAudio.Midi.MidiIn inputDevice;             
//            public int index;           
//        }

//        List<MidiInInfo> midiInputDevices;
//#endif


//        #region Initialization Methods

//        public void Awake() {
//            Instance = this;
            
//#if UNITY_STANDALONE || UNITY_EDITOR
//            Resources.UnloadUnusedAssets();
//            GC.Collect();            

//            midiInputDevices = new List<MidiInInfo>();
//            for (int i = 0; i < MidiIn.NumberOfDevices; ++i) {
//                var info = new MidiInInfo();
//                info.index = i;
//                midiInputDevices.Add(info);
//            }
//            midiControlEvent = new EventHandler<MidiInMessageEventArgs>(midiIn_MessageReceived);
//#endif
//#if UNITY_ANDROID && !UNITY_EDITOR
//            SetupAndroid();
//#endif
//        }


//        #endregion

//#if UNITY_STANDALONE || UNITY_EDITOR

//        #region Finalization Methods

//        public void Finish() {
//            for (int i = 0; i < midiInputDevices.Count; i++) {
//                var midiInputDevice = midiInputDevices[i].inputDevice;
//                if (midiInputDevice != null /*&& midiInputDevices[i].active*/) {
//#pragma warning disable
//                    try {
//                        midiInputDevice.Stop();
//                        midiInputDevice.Close();
//                        midiInputDevice.MessageReceived -= midiControlEvent;
//                    } catch (Exception e) {
//                    }
//#pragma warning restore
//                }
//            }
//            midiInputDevices.Clear();
//            Resources.UnloadUnusedAssets();
//            GC.Collect();
//        }

//#endregion

//#region Real-Time checking Methods

//        private void Update() {
//            CheckDevices();
//            CheckForNullDevices();
//        }

//        public void CheckForNullDevices() {
//            if (midiControlEvent == null)
//                midiControlEvent = new EventHandler<MidiInMessageEventArgs>(midiIn_MessageReceived);
//            for (int i = 0; i < midiInputDevices.Count; ++i) {
//                if (midiInputDevices[i].inputDevice == null) {
//#pragma warning disable
//                    try {//HACK
//                        midiInputDevices[i].inputDevice.Stop();
//                        midiInputDevices[i].inputDevice.Close();
//                        midiInputDevices[i].inputDevice.MessageReceived -= midiControlEvent;
//                    } catch (Exception e) {
//                    }
//#pragma warning restore
//                    midiInputDevices[i].inputDevice = new MidiIn(i);
//                    midiInputDevices[i].inputDevice.Start();
//                    midiInputDevices[i].inputDevice.MessageReceived += midiControlEvent;
//                }
//            }
//        }

//        public void CheckDevices() {
//            if (midiInputDevices.Count != MidiIn.NumberOfDevices) {
//                RefreshDevices();
//                return;
//            }
////            for (int i = 0; i < midiInputDevices.Count; i++) {
                
////                var inputDevice = midiInputDevices[i].inputDevice;
////                if (midiInputDevices[i].inputDevice != null) {
////#pragma warning disable
////                    try {//HACK
////                        inputDevice.Stop();
////                        inputDevice.Close();
////                        inputDevice.MessageReceived -= midiControlEvent;
////                    } catch (Exception e) {
////                    }
////#pragma warning restore
////                } else {
////#pragma warning disable
////                    try {//HACK
////                        inputDevice.Stop();
////                        inputDevice.Close();
////                        inputDevice.MessageReceived -= midiControlEvent;
////                    } catch (Exception e) {
////                    }
////#pragma warning restore
////                    inputDevice = midiInputDevices[i].inputDevice = new MidiIn(i);
////                    inputDevice.Start();
////                    inputDevice.MessageReceived += midiControlEvent;

////                }
////                Resources.UnloadUnusedAssets();
////                GC.Collect();
                

////            }
//        }

//#endregion

//#region Message Methods

//        public void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e) {
//            // Exit if the MidiEvent is null or is the AutoSensing command code  
//            if (e.MidiEvent != null && e.MidiEvent.CommandCode == MidiCommandCode.AutoSensing) {
//                return;
//            }
//            int device = (sender as MidiIn).DeviceNo;
//            if (e.MidiEvent.CommandCode == MidiCommandCode.ControlChange) {
//                ControlChangeEvent cce;
//                cce = (ControlChangeEvent)e.MidiEvent;

//                //msgCmdCode = "Control";
//                //Debug.Log(msgCmdCode + ": " + device + ", " + cce.Controller + ", " + cce.Channel + ", " + MidiIn.DeviceInfo(device).ProductName + ", " + MidiIn.DeviceInfo(device).ProductId + ", " + cce.ControllerValue);
//            }

//            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn || e.MidiEvent.CommandCode == MidiCommandCode.NoteOff) {
//                NAudio.Midi.NoteEvent cce;
//                cce = (NAudio.Midi.NoteEvent)e.MidiEvent;

//                if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn) {
//                    if (OnNoteOn != null)
//                        OnNoteOn(cce.NoteNumber, cce.Velocity);                    
//                }
//                else {
//                    if (OnNoteOff != null)
//                        OnNoteOff(cce.NoteNumber, cce.Velocity);
//                }
//            }

            
//        }

//#endregion

//#region Cleaning Functions

//        void RefreshDevices() {
//            var newMidiInputDevices = new List<MidiInInfo>();
//            for (int i = 0; i < MidiIn.NumberOfDevices; ++i) {
//                var newInfo = new MidiInInfo();
//                newInfo.index = i;
//                newMidiInputDevices.Add(newInfo);
//            }
//            if (midiInputDevices != null)
//                midiInputDevices.Clear();
//            midiInputDevices = newMidiInputDevices;
//            Resources.UnloadUnusedAssets();
//            GC.Collect();
//        }

//#endregion

//#region Public Methods

//#endregion

//#region Private Members

//        EventHandler<MidiInMessageEventArgs> midiControlEvent;

//#endregion
//#endif
        public static float FromMIDItoFREQ(int midiNote) {
            return Mathf.Pow(2, (midiNote - 69) / 12.0f) * 440f;
        }

//#if UNITY_ANDROID && !UNITY_EDITOR
//        //C# code
//        private void SetupAndroid() {
//            AndroidJavaClass unityActivity =
//              new AndroidJavaClass("com.unity3d.player.UnityPlayer");

//            AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context");
            
//            unityActivity =
//              new AndroidJavaClass("com.unity3d.player.UnityPlayer");

//            var context = unityActivity.GetStatic<AndroidJavaObject>
//                                   ("currentActivity");

//            object[] midiManagerParams = new object[1];
//            midiManagerParams[0] = contextClass.GetStatic<string>
//                                   ("MIDI_SERVICE");

//            AndroidJavaObject androidMidiManager = context.Call<AndroidJavaObject>("getSystemService", midiManagerParams);

//            AndroidJavaObject[] infos = androidMidiManager.Call<AndroidJavaObject[]>("getDevices");

//            if (infos.Length >= 1) {
//                androidMidiManager.Call("openDevice", infos[0], new AndroidMidiOnDeviceOpenedListener(context), new AndroidJavaObject("android.os.Handler"));
//            }
//            AndroidJavaClass androidMidiEvents = new AndroidJavaClass("lucas.midiextensions.midievents.MidiEvents");
//            androidMidiEvents.CallStatic("RegisterDeviceCallback", androidMidiManager, new AndroidMidiDeviceCallback(context, androidMidiManager));
//        }     

//        class AndroidMidiDeviceCallback : AndroidJavaProxy {

//            AndroidJavaObject context_;

//            public AndroidMidiDeviceCallback(AndroidJavaObject context, AndroidJavaObject midiManager) : base("lucas.midiextensions.midievents.IMidiDeviceCallback") {
//                context_ = context;
//                midiManager_ = midiManager;
//            }

//            void onDeviceAdded(AndroidJavaObject device) {
//                // Debug.Log("/n ADD: " + device.Call<string>("toString"));
//                ShowToast("Controlador MIDI conectado");
//                midiManager_.Call("openDevice", device, new AndroidMidiOnDeviceOpenedListener(context_), new AndroidJavaObject("android.os.Handler"));
//            }

//            void onDeviceRemoved(AndroidJavaObject device) {
//                Debug.LogWarning("/n REMOVE: " + device.Call<string>("toString"));
//            }

//            void onDeviceStatusChanged(AndroidJavaObject status) {

//            }

//            AndroidJavaObject midiManager_;
//        }

//        class AndroidMidiOnDeviceOpenedListener : AndroidJavaProxy {

//            AndroidJavaObject context_;

//            public AndroidMidiOnDeviceOpenedListener(AndroidJavaObject context) : base("android.media.midi.MidiManager$OnDeviceOpenedListener") {
//                context_ = context;
//            }

//            void onDeviceOpened(AndroidJavaObject device) {                
//                AndroidJavaClass androidMidiEvents = new AndroidJavaClass("lucas.midiextensions.midievents.MidiEvents");
//                androidMidiEvents.CallStatic("ConfigureMidiReceiver", device, new AndroidMidiEventReceiver());
//                Debug.LogWarning("OPEN");
//            }
//        }

//        class AndroidMidiEventReceiver : AndroidJavaProxy {

//            public AndroidMidiEventReceiver() : base("lucas.midiextensions.midievents.IMidiEventReceiver") { }

//            void onSend(int type, int channel, int data1, int data2) {
//                //NoteOff = 8
//                //NoteON = 9
//                //Polyphonic Aftertouch = 10
//                //Control Change = 11
//                //Program Change = 12
//                //Channel Aftertouch = 13
//                //Pitch Wheel = 14
//                if (type == 9) {
//                    if (OnNoteOn != null)
//                        OnNoteOn(data1, data2);
//                }
//                if (type == 8){
//                    if (OnNoteOff != null)
//                        OnNoteOff(data1, data2);
//                }
//            }
//        }

//        public static AndroidJavaObject ClassForName(string className) {
//            using (var clazz = new AndroidJavaClass("java.lang.Class")) {
//                return clazz.CallStatic<AndroidJavaObject>("forName", className);
//            }
//        }

//        // Cast extension method
//        public static AndroidJavaObject Cast(AndroidJavaObject source, string destClass) {
//            using (var destClassAJC = ClassForName(destClass)) {
//                return destClassAJC.Call<AndroidJavaObject>("cast", source);
//            }
//        }

//        private static void ShowToast(string msg) {
//            AndroidJavaClass toastClass =
//                       new AndroidJavaClass("android.widget.Toast");

//            object[] toastParams = new object[3];
//            AndroidJavaClass unityActivity =
//              new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//            toastParams[0] =
//                         unityActivity.GetStatic<AndroidJavaObject>
//                                   ("currentActivity");
//            toastParams[1] = msg;
//            toastParams[2] = toastClass.GetStatic<int>
//                                   ("LENGTH_LONG");

//            AndroidJavaObject toastObject =
//                            toastClass.CallStatic<AndroidJavaObject>
//                                          ("makeText", toastParams);
//            toastObject.Call("show");
//        }

//#endif

    }

}
