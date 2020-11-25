//Made by Hsinpa
//@2019

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

namespace Expect.Vibration
{
    public class VibrationHandler : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private int RComPort;

        [SerializeField]
        private int LComPort;

        [SerializeField]
        private int fps = 30; //Speed, Send message to port per second
        #endregion

        #region Parameter
        private VibrationSerialPort LeftHandPort;
        private VibrationSerialPort RightHandPort;

        private Queue<VibrationDataSet> msgQueue;

        //Key = HandIndex + FingerIndex
        private Dictionary<string, FingerData> fingerDataDict;
        private int msgQueueLength;

        public bool RightHandReady
        {
            get
            {
                return (RightHandPort != null && RightHandPort.isReady);
            }
        }

        public bool LeftHandReady
        {
            get
            {
                return (LeftHandPort != null && LeftHandPort.isReady);
            }
        }

        private float milliSecond;
        private float nextTimeCheckPoint;

        private static VibrationHandler _instance;

        public static VibrationHandler instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<VibrationHandler>();
                }
                return _instance;
            }
        }

        #endregion
        void Awake()
        {
            fingerDataDict = ConstructFindgerDataDict();
            milliSecond = 1f / fps;

            msgQueue = new Queue<VibrationDataSet>();
            msgQueueLength = 0;

            LeftHandPort = new VibrationSerialPort(LComPort, true);
            RightHandPort = new VibrationSerialPort(RComPort, true);
        }

        void Start() {
            FullHandVibration(VibrationIDFlag.HandIndex.Right, VibrationIDFlag.VibrationPower.Normal);
        }

        private void LateUpdate()
        {
            CheckTimerEvent();
        }

        private VibrationSerialPort GetSerialPortByHandIndex(VibrationIDFlag.HandIndex p_handIndex)
        {
            return (p_handIndex == VibrationIDFlag.HandIndex.Right) ? RightHandPort : LeftHandPort;
        }

        public void FullHandVibration(VibrationIDFlag.HandIndex p_handIndex, string power) {
            QueueMsg(new VibrationDataSet(FullHandMsgBinaryFormer("00", power), p_handIndex));
            QueueMsg(new VibrationDataSet(FullHandMsgBinaryFormer("01", power), p_handIndex));
        }

        public void FingerVibration(VibrationIDFlag.HandIndex p_handIndex, string power, VibrationIDFlag.FingerIndex[] fingers ) {
            string[] formedMsgArray = MsgBinaryFormer(p_handIndex, power, fingers);
            if (formedMsgArray != null)
            {
                foreach (string msg in formedMsgArray) {
                    QueueMsg(new VibrationDataSet(msg, p_handIndex));
                }
            }
        }

        private void QueueMsg(VibrationDataSet vDataSet)
        {
            if (msgQueue != null)
            {
                msgQueue.Enqueue(vDataSet);

                msgQueueLength++;
            }
        }

        #region Constructing Message Format
        private string FullHandMsgBinaryFormer(string header, string power)
        {
            string msg = header;
            int segmentLength = 3;

            for (int i = 0; i < segmentLength; i++) {
                msg += power;
            }

            //Debug.Log("Full Msg " + msg);
            return msg;
        }

        private string[] MsgBinaryFormer(VibrationIDFlag.HandIndex handIndex, string power, VibrationIDFlag.FingerIndex[] fingerIndexs) {
            string msgBinaryH = VibrationIDFlag.VibrationHeader.H  + "000000";
            string msgBinaryL = VibrationIDFlag.VibrationHeader.L + "000000";

            for (int i = 0; i < fingerIndexs.Length; i++) {

                if (fingerDataDict.TryGetValue(GetFingerDataDicKey(handIndex, fingerIndexs[i]), out FingerData fingerData))
                {
                    if (fingerData.header == VibrationIDFlag.VibrationHeader.H) {
                        msgBinaryH = EditBinaryMsg(msgBinaryH, fingerData.index, power);
                        //Debug.Log("msgBinaryH : " + msgBinaryH);
                    }
                    else if (fingerData.header == VibrationIDFlag.VibrationHeader.L) {
                        msgBinaryL = EditBinaryMsg(msgBinaryL, fingerData.index, power);
                       // Debug.Log("msgBinaryL : " + msgBinaryL);
                    }
                }
            }

            Debug.Log("msgBinaryH : " + msgBinaryH);
            Debug.Log("msgBinaryL : " + msgBinaryL);

            return new string[] { msgBinaryH, msgBinaryL };
        }

        private Dictionary<string, FingerData> ConstructFindgerDataDict()
        {
            Dictionary<string, FingerData> fingerDataDict = new Dictionary<string, FingerData>();

            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Left, VibrationIDFlag.FingerIndex.Thumb), GetFingerDataByRawCode(VibrationIDFlag.LeftHand.CodeThumb));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Left, VibrationIDFlag.FingerIndex.Center), GetFingerDataByRawCode(VibrationIDFlag.LeftHand.CodeCenter));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Left, VibrationIDFlag.FingerIndex.IndexFinger), GetFingerDataByRawCode(VibrationIDFlag.LeftHand.CodeIndexFinger));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Left, VibrationIDFlag.FingerIndex.BabyFinger), GetFingerDataByRawCode(VibrationIDFlag.LeftHand.CodeBabyFinger));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Left, VibrationIDFlag.FingerIndex.MiddleFinger), GetFingerDataByRawCode(VibrationIDFlag.LeftHand.CodeMiddleFinger));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Left, VibrationIDFlag.FingerIndex.RingFinger), GetFingerDataByRawCode(VibrationIDFlag.LeftHand.CodeRingFinger));

            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Right, VibrationIDFlag.FingerIndex.Thumb), GetFingerDataByRawCode(VibrationIDFlag.RightHand.CodeThumb));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Right, VibrationIDFlag.FingerIndex.Center), GetFingerDataByRawCode(VibrationIDFlag.RightHand.CodeCenter));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Right, VibrationIDFlag.FingerIndex.IndexFinger), GetFingerDataByRawCode(VibrationIDFlag.RightHand.CodeIndexFinger));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Right, VibrationIDFlag.FingerIndex.BabyFinger), GetFingerDataByRawCode(VibrationIDFlag.RightHand.CodeBabyFinger));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Right, VibrationIDFlag.FingerIndex.MiddleFinger), GetFingerDataByRawCode(VibrationIDFlag.RightHand.CodeMiddleFinger));
            fingerDataDict.Add(GetFingerDataDicKey(VibrationIDFlag.HandIndex.Right, VibrationIDFlag.FingerIndex.RingFinger), GetFingerDataByRawCode(VibrationIDFlag.RightHand.CodeRingFinger));

            return fingerDataDict;
        }

        private FingerData GetFingerDataByRawCode(string raw_code) {

            string[] codePair = raw_code.Split(new string[] { ":" }, System.StringSplitOptions.None);

            if (codePair.Length == 2) {
                FingerData fingerData = new FingerData();
                fingerData.header = codePair[0];
                fingerData.index = int.Parse(codePair[1]);
                return fingerData;
            }

            return default(FingerData);
        }

        private string EditBinaryMsg(string raw_binary_msg, int key_index, string power) {
            if (raw_binary_msg.Length == 8) {
                int startIndex = 2;
                int editIndex = startIndex + (key_index * 2);

                System.Text.StringBuilder strBuilder = new System.Text.StringBuilder(raw_binary_msg);
                strBuilder.Remove(editIndex, 2);
                strBuilder.Insert(editIndex, power);

                return strBuilder.ToString();
            }

            return raw_binary_msg;
        }

        private string GetFingerDataDicKey(VibrationIDFlag.HandIndex handIndex, VibrationIDFlag.FingerIndex fingerIndex)
        {
            string handIndexValue = ((int)handIndex).ToString(), fingerIndexValue = ((int)fingerIndex).ToString();
            return handIndexValue + fingerIndexValue;
        }

        #endregion

        #region Handle Messgae Queue

        private void CheckTimerEvent() {
            if (Time.time > nextTimeCheckPoint) {
                SendQueueMsgToPort();
                nextTimeCheckPoint = Time.time+ milliSecond;
            }
        }

        private void SendQueueMsgToPort() {
            if (msgQueue != null && msgQueueLength > 0) {
                //lock (msgQueue) {
                    for (int i = 0; i < msgQueueLength; i++) {
                        VibrationDataSet vDataset = msgQueue.Dequeue();

                        VibrationSerialPort port = GetSerialPortByHandIndex(vDataset.handIndex);
                        if (!string.IsNullOrEmpty(vDataset.content) && port != null && port.isReady) {
                            //Debug.Log("Send Content : " + vDataset.content);
                            port.SendBinary(vDataset.content);
                        }
                    }

                msgQueueLength = 0;
                //}
            }
        }

        private struct VibrationDataSet {
            public string content;

            public VibrationIDFlag.HandIndex handIndex;

            public VibrationDataSet(string content, VibrationIDFlag.HandIndex handIndex)
            {
                this.content = content;
                this.handIndex = handIndex;
            }
        }

        private struct FingerData {
            public string header;
            public int index;
        }
        #endregion

        private void OnApplicationQuit()
        {
            LeftHandPort.Disconnect();
            RightHandPort.Disconnect();
        }
    }
}