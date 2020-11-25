//Made by Hsinpa
//@2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Text;
using System.Threading;

namespace Expect.Vibration {

    public class VibrationSerialPort
    {
        private SerialPort _sp;
        private bool _debugMode;
        public bool isReady;

        public VibrationSerialPort(int port_num, bool isDebugMode)
        {
            _sp = null;
            isReady = false;
            _sp = new SerialPort("\\\\.\\COM" + port_num, 115200);
            _sp.ReadTimeout = 300;

            try
            {
                _sp.Open();             //開啟SerialPort連線
                                        //Thread readThread = new Thread(new ThreadStart(ReadMsgFromSensor)); //實例化執行緒與指派呼叫函式
                                        //readThread.Start();           //開啟執行緒
                isReady = true;
                Debug.Log(port_num + " SerialPort開啟連接");
            }
            catch
            {
                Debug.Log(port_num + " SerialPort連接失敗");
            }
        }

        public void SendBinary(string binary)
        {
            if (_sp == null || !_sp.IsOpen) return;

            try
            {
                // Translate binary to ASCII
                StringBuilder decodedBinary = new StringBuilder();
                for (int i = 0; i < binary.Length; i += 8)
                {

                    decodedBinary.Append(Convert.ToChar(Convert.ToByte(binary.Substring(i, 8), 2)));
                }

                StringBuilder titleB = new StringBuilder();
                titleB.Append(Convert.ToChar(Convert.ToByte("00001010", 2)));
                StringBuilder titleA = new StringBuilder();
                titleA.Append(Convert.ToChar(Convert.ToByte("00001101", 2)));
                string str1 = decodedBinary.ToString();
                string TA = titleA.ToString();
                string TB = titleB.ToString();

                _sp.Write(str1); //寫入二進制字串轉ASCII的字元到藍芽

                _sp.Write(TA + TB);
                //Debug.Log("BLE send message in ASCII");
                Debug.Log(binary);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        public void Disconnect()  //關閉連接埠
        {
            if (_sp != null)
            {
                if (_sp.IsOpen)
                {
                    isReady = false;
                    _sp.Close();
                }
            }
        }
    }
}
