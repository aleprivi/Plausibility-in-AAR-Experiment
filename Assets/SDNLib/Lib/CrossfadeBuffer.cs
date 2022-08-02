using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using UnityEngine;
using System;
using AForge.Math;
using System.IO;

public class CrossfadeBuffer : MonoBehaviour
{
    float[][] outBuffer;

    int overlap;

    int buffSize;

    /*
     * overlap-> in samples: deve essere un divisore di buffsize


     * WindowType: 
     * 1-hanning
     * 2-hamming
     * 3-blackmann
     */

    public enum WindowType
    {
        hanning, hamming, blackmann, square, triangle, tukey
    };

    float[] window;
    private void createWindow(WindowType type)
    {
        window = new float[buffSize];

        for (int n = 0; n < buffSize; n++)
        {
            switch (type)
            {
                //hanning
                case WindowType.hanning:
                    window[n] = 0.5f * (1f - (float)System.Math.Cos(2f * System.Math.PI * n / (buffSize - 1)));
                    break;
                //hamming
                case WindowType.hamming:
                    window[n] = 0.54f - 0.46f * ((float)System.Math.Cos(2f * System.Math.PI * n / buffSize - 1));
                    break;
                case WindowType.blackmann:
                    window[n] = 0.42f - 0.5f * ((float)System.Math.Cos(2f * System.Math.PI * n / buffSize)) + 0.08f * ((float)System.Math.Cos(4f * System.Math.PI * n / buffSize));
                    break;
                case WindowType.square:
                    window[n] = 1;
                    break;
                case WindowType.triangle:
                    window[n] = 1 - Math.Abs((n - (buffSize / 2.0f)) / (buffSize / 2.0f));
                    break;
                case WindowType.tukey:
                    float alpha = 0.5f;
                    if (n > buffSize / 2)
                    {
                        window[n] = window[buffSize - n];
                    }
                    else
                    {
                        if (n < (alpha * buffSize / 2.0f))
                        {
                            window[n] = (float)(0.5f * (1 - Math.Cos(2 * Math.PI * n / (alpha * buffSize))));
                        }
                        else
                        {
                            window[n] = 1;
                        }
                    }
                    break;
            }
        }

    }

    bool resetInput;

    public CrossfadeBuffer(int bufferSize, int overlapSize, WindowType windowType, bool bypassWaveform)
    {

        if (bypassWaveform) { Debug.Log("Disattivato reset forma d'onda"); }
        resetInput = bypassWaveform;

        buffSize = bufferSize;
        overlap = overlapSize;  //può essere la lunghezza del crossfade

        //Creo i due buffer di uscita
        outBuffer = new float[2][];
        for (int i = 0; i < outBuffer.Length; i++)
        {
            outBuffer[i] = new float[buffSize];
        }

        //Due con nuovo hrtf due con vecchio hrtf
        tempWindow = new Complex[5][];
        for (int i = 0; i < tempWindow.Length; i++)
        {
            tempWindow[i] = new Complex[buffSize * 2];
        }

        //inizializzo old_hrtf
        old_hrtfs = new Complex[2][];
        for (int i = 0; i < outBuffer.Length; i++)
        {
            old_hrtfs[i] = new Complex[buffSize * 2];
        }

        //Creo la finestra
        createWindow(windowType);


        //Inizializzo la stampa del file


    }

    public CrossfadeBuffer(int bufferSize, int overlapSize, WindowType windowType) : this(bufferSize, overlapSize, windowType, false)
    {
    }

    Complex[][] tempWindow;
    Complex[][] old_hrtfs;


    private Complex[][] Util_CopyArrayLinq(Complex[][] source) // jagged array copy
    {
        return source.Select(s => s.ToArray()).ToArray();
    }

    public float[][] getFromBuffer(Complex[] data, Complex[][] hrtfs)
    {

        if (resetInput)
        {
            for (int i = 0; i < buffSize; i++)
            {
                //data[i].Re = (Mathf.Sin(2 * Mathf.PI * 2 * i / (buffSize)) + Mathf.Sin(2 * Mathf.PI * i / (buffSize)))/2;
                data[i].Re = Mathf.Sin(2 * Mathf.PI * 2 * i / (buffSize));
                //data[i].Re = 1;
            }
        }

        for (int i = 0; i < tempWindow.Length; i++)
        {
            tempWindow[i] = new Complex[buffSize * 2];
        }

        //Copio la finestra di dati su tempWindow[0]
        for (int i = 0; i < buffSize; i++)
        {
            tempWindow[0][i] = data[i];
        }

        FourierTransform.FFT(tempWindow[0], FourierTransform.Direction.Forward);


        //Calcolo la FFT e applico le elaborazioni
        for (int i = 0; i < buffSize * 2; i++)
        {
            //NUOVO HRTF
            tempWindow[1][i] = tempWindow[0][i] * hrtfs[0][i] * buffSize * 2;
            tempWindow[2][i] = tempWindow[0][i] * hrtfs[1][i] * buffSize * 2;

            //VECCHIO HRTF
            tempWindow[3][i] = tempWindow[0][i] * old_hrtfs[0][i] * buffSize * 2;
            tempWindow[4][i] = tempWindow[0][i] * old_hrtfs[1][i] * buffSize * 2;
        }
        FourierTransform.FFT(tempWindow[1], FourierTransform.Direction.Backward);
        FourierTransform.FFT(tempWindow[2], FourierTransform.Direction.Backward);

        FourierTransform.FFT(tempWindow[3], FourierTransform.Direction.Backward);
        FourierTransform.FFT(tempWindow[4], FourierTransform.Direction.Backward);

        //Copio i dati in uscita
        float[][] outData = new float[2][];
        for (int i = 0; i < 2; i++) {
            outData[i] = new float[buffSize];
        }


        //Salvo i dati elaborati
        //area Crossfade
        for (int i = 0; i < overlap; i++)
        {
            outData[0][i] = (outBuffer[0][i] + (float)tempWindow[3][i].Re) * window[overlap+i] + (float)tempWindow[1][i].Re * window[i];
            outData[1][i] = (outBuffer[1][i] + (float)tempWindow[4][i].Re) * window[overlap+i] + (float)tempWindow[2][i].Re * window[i];

            /*Canale destro?*/
            outBuffer[0][i] = (float)tempWindow[1][i + buffSize].Re;
            /*Canale sinistro?*/
            outBuffer[1][i] = (float)tempWindow[2][i + buffSize].Re;
        }
        //Resto
        for (int i = overlap; i < buffSize; i++)
        {
            outData[0][i] = (float)tempWindow[1][i].Re;
            outData[1][i] = (float)tempWindow[2][i].Re;

            /*Canale destro?*/
            outBuffer[0][i] = (float)tempWindow[1][i+buffSize].Re;
            /*Canale sinistro?*/
            outBuffer[1][i] = (float)tempWindow[2][i + buffSize].Re;
        }


        //copio la vecchia hrtf
        old_hrtfs = Util_CopyArrayLinq(hrtfs);

        return outData;
    }

    public float[][] getFromBuffer(Complex[] data, Complex[][] hrtfs, bool writeTestFile)
    {
        float[][] outData = getFromBuffer(data, hrtfs);
        if (writeTestFile)
        {
//            Debug.Log("Scrivo File");

        }
        return outData;
    }




    int step = 0;



}
