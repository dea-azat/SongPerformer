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
        static public float SEMITONE = Mathf.Pow(2f, 1f / 12f);

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

        public static void AdjustPitch(AudioClip clip, SoundPlayer player)
        {
            int key = KeyFinder.KeyFind(clip);
            Logger.log.Error("the key of song is " + key);

            int sampleKey = 21; //GMinor

            int minMajDiff = (sampleKey % 2 == 0) ? -3 : 3;
            minMajDiff = ((key % 2) == (sampleKey % 2)) ? 0 : minMajDiff;

            int diff = ((int)(key - sampleKey) / 2 + minMajDiff + 24) % 12;

            float pitch = 1f;

            if (diff <= 7)
            {
                for (int i = 0; i < diff; i++)
                {
                    pitch *= KeyFinder.SEMITONE;
                }
            }
            else
            {
                for (int i = 0; i < 12 - diff; i++)
                {
                    pitch /= KeyFinder.SEMITONE;
                }
            }

            player.SetPitch(pitch);

            Logger.log.Error("the key diff is " + diff);
        }
    }
}
