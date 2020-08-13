using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Runtime.InteropServices;

namespace SongPerformer
{
    static class KeyFinder
    {
        [DllImport("libKeyFinder.dll")]
        static extern int KeyFind(double[] data, int dataLength, double ampMax, int sampleRate);

        public static int KeyFind(AudioClip clip)
        {
            int sampleRate = clip.frequency;
            int dataLength = sampleRate * 30;
            float ampMax = 0;

            float[] clipData = new float[dataLength * clip.channels];
            clip.GetData(clipData, 0);


            double[] data = new double[dataLength];
            for (int i = 0; i < dataLength; i++)
            {
                float monoData = clipData[i * 2] / 2f + clipData[i * 2 + 1] / 2f;
                data[i] = (double)monoData;

                if (Mathf.Abs(monoData) > ampMax)
                {
                    ampMax = monoData;
                }
            }

            sampleRate = clip.frequency;

            int key = KeyFind(data, dataLength, ampMax, sampleRate);

            return key;
        }
    }
}
