using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using UnityEngine;
using System;
using AForge.Math;
using System.IO;

public class CircularBuffer : MonoBehaviour
{
    Complex[] inBuffer;
    int inStartPivot;
    int inLength;
    Complex[][] outBuffer;
    int outStartPivot;
    int outLength;

    int overlap;

    int buffSize; //= alla windowsize

    /*
     * windowSize-> in sample: deve essere un divisore/multiplo di buffsize
     * overlap-> in samples: deve essere un divisore di buffsize
     */

    /*
     * WindowType: 
     * 1-hanning
     * 2-hamming
     * 3-blackmann
     */

    public enum WindowType {
        hanning, hamming, blackmann, square, triangle, tukey
    };

    float[] window;
    private void createWindow(WindowType type)
    {
        window = new float[buffSize];

        for (int n = 0; n < buffSize; n++)
        {
            switch (type) {
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
                    window[n] = 1-Math.Abs((n-(buffSize / 2.0f))/(buffSize / 2.0f));
                    break;
                case WindowType.tukey:
                    float alpha = 0.5f;
                    if (n > buffSize / 2) {
                        window[n] = window[buffSize - n];
                    } else {
                        if (n < (alpha * buffSize / 2.0f))
                        {
                            window[n] = (float)(0.5f * (1 - Math.Cos(2 * Math.PI * n / (alpha * buffSize))));
                        }
                        else {
                            window[n] = 1;
                        }
                    }

                    
                    break;
            }
        }

    }

    bool resetInput;

    public CircularBuffer(int bufferSize, int overlapSize, WindowType windowType, bool bypassWaveform) {

        if (bypassWaveform) { Debug.Log("Disattivato reset forma d'onda"); }
        resetInput = bypassWaveform;

        buffSize = bufferSize;
        overlap = overlapSize;

        //Creo i buffer
        inBuffer = new Complex[buffSize * 4];
        inStartPivot = 0;
        inLength = bufferSize;

        //Creo i due buffer di uscita
        outBuffer = new Complex[2][];
        for (int i = 0; i < outBuffer.Length; i++)
        {
            outBuffer[i] = new Complex[inBuffer.Length + bufferSize];
        }
        outStartPivot = 0;
        outLength = bufferSize;


        tempWindow = new Complex[3][];
        for (int i = 0; i < tempWindow.Length; i++)
        {
            tempWindow[i] = new Complex[buffSize * 2];
        }

        //Creo la finestra
        createWindow(windowType);


        //Inizializzo la stampa del file

        string path = "Assets/Resources/test.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.Close();
    }

    public CircularBuffer(int bufferSize, int overlapSize, WindowType windowType) : this(bufferSize, overlapSize, windowType, false) {
    }
    

    private void addToBuffer(Complex[] data)
    {

        //SOSTITUISCO CON UN SIMIL-SINE WAVE PER TEST (DA CANCELLARE)
        if (resetInput)
        {
            for (int i = 0; i < buffSize; i++)
            {
                //data[i].Re = (Mathf.Sin(2 * Mathf.PI * 2 * i / (buffSize)) + Mathf.Sin(2 * Mathf.PI * i / (buffSize)))/2;
                data[i].Re = Mathf.Sin(2 * Mathf.PI * 2 * i / (buffSize));
                //data[i].Re = 1;
            }
        }
        //Debug.Log("Aggiunti dati. Parto con: " + inLength + " - Start: " + inStartPivot);
        for (int i = 0; i < buffSize; i++)
        {
            int endData = (inStartPivot + inLength) % inBuffer.Length;
            inBuffer[endData] = data[i];
            inLength++;
        }
        //Debug.Log("Finisco con: " + inLength + " - Start: " + inStartPivot);
        if (inLength >= inBuffer.Length)
        {
            Debug.Log("buffer Pieno!");
        }

    }

    public bool ThereIsEnoughData()
    {
        if (inLength >= buffSize) return true;
        else return false;
    }

    Complex[][] tempWindow;
    public float[][] getFromBuffer(Complex[] data, Complex[][] hrtfs)
    {

        addToBuffer(data);

        for (int i = 0; i < tempWindow.Length; i++)
        {
            tempWindow[i] = new Complex[buffSize * 2];
        }

        //int ss = 0;
        while (ThereIsEnoughData())
        {
            //Copio la finestra di dati
            for (int i = 0; i < buffSize; i++)
            {
                tempWindow[0][i] = inBuffer[inStartPivot] * window[i];
                inStartPivot = (inStartPivot + 1) % (inBuffer.Length);
                inLength--;
            }
            //rimetto in circolo il pezzo di dato non utilizzato
            inLength = inLength + overlap;
            inStartPivot = ((inStartPivot - overlap) + inBuffer.Length)  % (inBuffer.Length);

            FourierTransform.FFT(tempWindow[0], FourierTransform.Direction.Forward);


            //Calcolo la FFT

            for (int i = 0; i < buffSize * 2; i++)
            {
                tempWindow[1][i] = tempWindow[0][i] * hrtfs[0][i] * buffSize * 2;
                tempWindow[2][i] = tempWindow[0][i] * hrtfs[1][i] * buffSize * 2;
                //tempWindow[1][i] = tempWindow[0][i];
                //tempWindow[2][i] = tempWindow[0][i];
            }
            FourierTransform.FFT(tempWindow[1], FourierTransform.Direction.Backward);
            FourierTransform.FFT(tempWindow[2], FourierTransform.Direction.Backward);


            //Salvo i dati elaborati
            for (int i = 0; i < buffSize * 2; i++)
            {
                int start = (outStartPivot + outLength - overlap) % outBuffer[0].Length;
                //outBuffer[0][start].Re += 3;
                double a = outBuffer[0][start].Re + tempWindow[1][i].Re;
                double b = outBuffer[1][start].Re + tempWindow[2][i].Re;
                /*Canale destro?*/
                outBuffer[0][start].Re = a;
                /*Canale sinistro?*/
                outBuffer[1][start].Re = b;
                outLength++;
            }
            outLength -= (buffSize + overlap);


            //ss++;
        }

        //Copio i dati in uscita
        float[][] outData = new float[2][];

        for (int i = 0; i < outData.Length; i++)
        {
            outData[i] = new float[buffSize];
        }

        for (int i = 0; i < buffSize; i++)
        {
            outData[0][i] = (float)outBuffer[0][outStartPivot].Re;
            outData[1][i] = (float)outBuffer[1][outStartPivot].Re;
            outBuffer[0][outStartPivot] = new Complex();
            outBuffer[1][outStartPivot] = new Complex();
            outStartPivot = (outStartPivot + 1) % outBuffer[0].Length;
            outLength--;
        }

        return outData;
    }

    public float[][] getFromBuffer(Complex[] data, Complex[][] hrtfs, bool writeTestFile) {
        float[][] outData = getFromBuffer(data, hrtfs);
        if (writeTestFile) {
            Debug.Log("Scrivo File");
            WriteFile(outData[0]);
        }
        return outData;
    }




    int step = 0;
    public void WriteFile()
    {
        //STAMPO I PRIMI SAMPLES (DA CANCELLARE POI)
        if (step < 2)
        //if(step == 1)
        {
            string path = "Assets/Resources/test.txt";
            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, true);
            //for (int i = 0; i < circularBufferOLA.Length; i++){
            //    writer.WriteLine(circularBufferOLA[i].Re.ToString());
            //}
            //for (int i = 0; i < result[0].Length; i++)
            //{
            //    writer.WriteLine(result[0][i].ToString());
            //}
            //writer.WriteLine(circularBuffer.Length.ToString());
            //for (int i = 0; i < inBuffer.Length; i++)
            //{
            //    writer.WriteLine(inBuffer[i].Re.ToString());
            //}
            for (int i = 0; i < outBuffer[0].Length; i++)
            {
                writer.WriteLine(outBuffer[0][i].Re.ToString());
            }
            writer.Close();
        }

        step++;
    }

    public void WriteFile(float[] data)
    {
        if (step < 20)
        {
            string path = "Assets/Resources/test.txt";
            StreamWriter writer = new StreamWriter(path, true);
            for (int i = 0; i < data.Length; i++)
            {
                writer.WriteLine(data[i].ToString());
            }
            writer.Close();
        }
        step++;
    }
}