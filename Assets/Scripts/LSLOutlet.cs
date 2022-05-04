using UnityEngine;
using System.Collections;
using LSL;
using System.Diagnostics;
using System.Collections.Generic;
using Looxid.Link;

namespace Assets.LSL4Unity.Scripts
{
    public enum MomentForSampling { Update, FixedUpdate, LateUpdate }


    public class LSLOutlet : MonoBehaviour
    {
        private liblsl.StreamOutlet outlet;
        private liblsl.StreamInfo streamInfo;
        private float[] currentSample;

        public string StreamName = "Unity.ExampleStream";
        public string StreamType = "Unity.Update";
        public int ChannelCount = 1;

        
        Stopwatch watch;

        // Use this for initialization
        void Start()
        {
            watch = new Stopwatch();

            watch.Start();

            currentSample = new float[ChannelCount];

            streamInfo = new liblsl.StreamInfo(StreamName, StreamType, ChannelCount,500);

            outlet = new liblsl.StreamOutlet(streamInfo);
        }

        public void UpdateChannelNum()
        {

        }

        public void SendData(int[][] data, int totalSamples)
        {
            if (data == null || data.Length <= 0) return;
            if (data == null) return;

            int startSample = totalSamples - data.Length;

            
            for (int i = 0; i < data.Length; i++)
            {
                float[] dataLSL =  new float[data.Length];
                for (int j = 0; j < data[i].Length; j++)
                {
                    dataLSL[j] = data[i][j];                      
                }
                outlet.push_sample(dataLSL);
            }

            
        }

        public void SendDataMultiple(int[][] data, int totalSamples, int channelnum)
        {
            if (data == null || data.Length <= 0) return;
            if (data == null) return;

            int startSample = totalSamples - data.Length;


            for (int i = 0; i < data.Length; i++)
            {
                float[] dataLSL = new float[data.Length];
                for (int j = 0; j < data[i].Length; j++)
                {
                    dataLSL[j] = data[i][channelnum];
                }
                outlet.push_sample(dataLSL);
            }


        }



        public void SendDataEEG(EEGRawSignal rawSignalData)
        {
            

            for(int i=0;i<rawSignalData.rawSignal.Count;i++)
            {
                double[] dataInput = new double[rawSignalData.rawSignal.Count];

                for(int j =0;j<rawSignalData.rawSignal[i].ch_data.Length;j++)
                {
                    dataInput[j] = rawSignalData.rawSignal[i].ch_data[j]; 
                }
                outlet.push_sample(dataInput);
            }
           
        }






    }
}