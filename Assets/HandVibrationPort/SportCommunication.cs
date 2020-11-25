using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Text;
using System.Threading;

public class SportCommunication : MonoBehaviour
{

    private SerialPort BLEStreamL;
    private SerialPort BLEStreamR;
    public string comPort;     // 藍芽模組之連接埠
    private Thread readThread; // 宣告執行緒
    //public string readMessage; // 由藍芽讀取到的資料
    //bool isNewMessage;         // 讀取判斷開關


    // Use this for initialization
    void Start()
    {
        if (comPort != "")
        {
            BLEStreamL = new SerialPort("\\\\.\\COM23", 115200); //指定連接埠、鮑率並實例化SerialPort
            BLEStreamR = new SerialPort("\\\\.\\COM22", 115200); //指定連接埠、鮑率並實例化SerialPort
            BLEStreamL.ReadTimeout = 500;
            BLEStreamR.ReadTimeout = 500;
            try
            {
                BLEStreamL.Open();             //開啟SerialPort連線
                BLEStreamR.Open();             //開啟SerialPort連線
                //readThread = new Thread(new ThreadStart(BLERead)); //實例化執行緒與指派呼叫函式
                //readThread.Start();           //開啟執行緒
                Debug.Log("SerialPort開啟連接");
            }
            catch
            {
                Debug.Log("SerialPort連接失敗");
            }
        }
    }
    // Update is called once per frame
    //void Update()
    //{
    //    if (isNewMessage)
    //    {
    //        Debug.Log("BLE readed message");
    //        Debug.Log(readMessage);
    //    }
    //    isNewMessage = false;

    // }

    // private void BLERead()
    //{
    //     while (BLEStream.IsOpen)
    //     {
    //         try
    //         {
    //             readMessage = BLEStream.ReadLine(); //讀取藍芽資料並裝入readMessage
    //             isNewMessage = true;
    //         } catch (System.Exception e) {
    //             Debug.LogWarning(e.Message);
    //         }
    //     }
    // }
    public void BLEWriteL(string binaryL)
    {
        Debug.Log("BLE send message in binary");
        Debug.Log(binaryL);


        try
        {
            // Translate binary to ASCII
            StringBuilder decodedBinary = new StringBuilder();
            for (int i = 0; i < binaryL.Length; i += 8)
            {

                decodedBinary.Append(Convert.ToChar(Convert.ToByte(binaryL.Substring(i, 8), 2)));
            }

            StringBuilder titleB = new StringBuilder();
            titleB.Append(Convert.ToChar(Convert.ToByte("00001010", 2)));
            StringBuilder titleA = new StringBuilder();
            titleA.Append(Convert.ToChar(Convert.ToByte("00001101", 2)));
            string str1 = decodedBinary.ToString();
            string TA = titleA.ToString();
            string TB = titleB.ToString();


            BLEStreamL.Write(str1); //寫入二進制字串轉ASCII的字元到藍芽

            BLEStreamL.Write(TA+TB);
            Debug.Log("BLE send message in ASCII");
            Debug.Log(str1);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void BLEWriteR(string binaryR)
    {
        Debug.Log("BLE send message in binary");
        Debug.Log(binaryR);
        
       
        try
        {
            // Translate binary to ASCII
            StringBuilder decodedBinary = new StringBuilder();
            for (int i = 0; i < binaryR.Length; i += 8)
            {
                
                decodedBinary.Append(Convert.ToChar(Convert.ToByte(binaryR.Substring(i, 8), 2)));
            }
            
            StringBuilder titleB = new StringBuilder();
            titleB.Append(Convert.ToChar(Convert.ToByte("00001010", 2)));
            StringBuilder titleA = new StringBuilder();
            titleA.Append(Convert.ToChar(Convert.ToByte("00001101", 2)));
            string str1 = decodedBinary.ToString();
            string TA = titleA.ToString();
            string TB = titleB.ToString();
          
          
            BLEStreamR.Write(str1); //寫入二進制字串轉ASCII的字元到藍芽
            
            BLEStreamR.Write(TA+TB);
            Debug.Log("BLE send message in ASCII");
            Debug.Log(str1);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    void OnApplicationQuit()  //關閉連接埠
    {
        if (BLEStreamL != null)
        {
            if (BLEStreamL.IsOpen)
            {
                BLEStreamL.Close();
            }
        }
        if (BLEStreamR != null)
        {
            if (BLEStreamR.IsOpen)
            {
                BLEStreamR.Close();
            }
        }
    }
}
       
