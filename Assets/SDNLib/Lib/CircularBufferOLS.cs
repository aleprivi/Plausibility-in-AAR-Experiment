using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using UnityEngine;
using System;
using AForge.Math;
using System.IO;

public class CircularBufferOLS : MonoBehaviour
{
    Complex[] inBuffer;
    int inStartPivot;
    int inLength;
    Complex[][] outBuffer;
    int outStartPivot;
    int outLength;

    int winSize;
    int overlap;

    int buffSize;
    int fftLength;

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
        window = new float[winSize];

        for (int n = 0; n < winSize; n++)
        {
            switch (type) {
                //hanning
                case WindowType.hanning:
                    window[n] = 0.5f * (1f - (float)System.Math.Cos(2f * System.Math.PI * n / (winSize - 1)));
                    break;
                //hamming
                case WindowType.hamming:
                    window[n] = 0.54f - 0.46f * ((float)System.Math.Cos(2f * System.Math.PI * n / winSize - 1));
                    break;
                case WindowType.blackmann:
                    window[n] = 0.42f - 0.5f * ((float)System.Math.Cos(2f * System.Math.PI * n / winSize)) + 0.08f * ((float)System.Math.Cos(4f * System.Math.PI * n / winSize));
                    break;
                case WindowType.square:
                    window[n] = 1;
                    break;
                case WindowType.triangle:
                    window[n] = 1-Math.Abs((n-(winSize/2.0f))/(winSize/2.0f));
                    break;
                case WindowType.tukey:
                    float alpha = 0.5f;
                    if (n > winSize / 2) {
                        window[n] = window[winSize - n];
                    } else {
                        if (n < (alpha * winSize / 2.0f))
                        {
                            window[n] = (float)(0.5f * (1 - Math.Cos(2 * Math.PI * n / (alpha * winSize))));
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

    public CircularBufferOLS(int bufferSize, int overlapSize, WindowType windowType, bool bypassWaveform) {

        if (bypassWaveform) { Debug.Log("Attivato reset forma d'onda"); }
        resetInput = bypassWaveform;

        buffSize = bufferSize;
        fftLength = 2 * buffSize;
        winSize = buffSize;
        overlap = overlapSize;

        //Creo i buffer
        inBuffer = new Complex[buffSize * 2];
        inStartPivot = 0;
        inLength = bufferSize;
        outBuffer = new Complex[2][];
        for (int i = 0; i < outBuffer.Length; i++)
        {
            outBuffer[i] = new Complex[buffSize * 2]; //EVENTUALMENTE DA VARIARE LA BUFFSIZE
        }
        outStartPivot = 0;
        outLength = bufferSize;
        tempWindow = new Complex[3][];
        for (int i = 0; i < tempWindow.Length; i++)
        {
            tempWindow[i] = new Complex[winSize * 2];
        }

        //Creo la finestra
        createWindow(windowType);




        string path = "Assets/Resources/test.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.Close();
    }

    public CircularBufferOLS(int bufferSize, int overlapSize, WindowType windowType) : this(bufferSize, overlapSize, windowType, false) {
    }

    float t = 0;
    private void addToBuffer(Complex[] data)
    {
        Debug.Log("Entrato");
        //SOSTITUISCO CON UN SIMIL-SINE WAVE PER TEST (DA CANCELLARE)
        if (resetInput)
        {
            for (int i = 0; i < buffSize; i++)
            {
                data[i].Re = (Mathf.Sin(t + 0.5f * Mathf.PI * 2 * i / (buffSize)) + Mathf.Sin(t + 0.5f * Mathf.PI * i / (buffSize)))/2;
                //data[i].Re = Mathf.Sin(t + 0.5f * Mathf.PI * 2 * i / (buffSize));
                //data[i].Re = 1;
            }
            t += Mathf.PI;
        }

        for (int i = 0; i < buffSize; i++) {
            inBuffer[i] = inBuffer[buffSize+i];
        }

        //Debug.Log("Aggiunti dati. Parto con: " + inLength + " - Start: " + inStartPivot);
        for (int i = buffSize; i < buffSize*2; i++)
        {
            inBuffer[i] = data[i-buffSize];
        }

    }

    //public bool ThereIsEnoughData()
    //{
    //    if (inLength >= winSize) return true;
    //    else return false;
    //}





    Complex[][] tempWindow;
    public float[][] getFromBuffer(Complex[] data, Complex[][] hrtfs)
    {

        addToBuffer(data);

        for (int i = 0; i < tempWindow.Length; i++)
        {
            tempWindow[i] = new Complex[buffSize*2];
        }

        //COPIO L'intero buffer per le elaborazioni
        for (int i = 0; i < buffSize * 2; i++) {
            tempWindow[0][i] = inBuffer[i];
        }

        //ELABORO...
        FourierTransform.FFT(tempWindow[0], FourierTransform.Direction.Forward);

        for (int i = 0; i < winSize * 2; i++)
        {
            tempWindow[1][i] = tempWindow[0][i] * hrtfs[0][i] * winSize * 2;
            tempWindow[2][i] = tempWindow[0][i] * hrtfs[1][i] * winSize * 2;
            //tempWindow[1][i] = tempWindow[0][i];
            //tempWindow[2][i] = tempWindow[0][i];
        }
        FourierTransform.FFT(tempWindow[1], FourierTransform.Direction.Backward);
        FourierTransform.FFT(tempWindow[2], FourierTransform.Direction.Backward);


        //COPIO i dati in uscita
        float[][] outData = new float[2][];

        for (int i = 0; i < outData.Length; i++)
        {
            outData[i] = new float[buffSize];
        }

        for (int i = 0; i < buffSize; i++)
        {
            outData[0][i] = (float)tempWindow[1][i + buffSize].Re;
            outData[1][i] = (float)tempWindow[2][i + buffSize].Re;
        }

        WriteFile(outData[0]);

        return outData;

        ////int ss = 0;
        //while (ThereIsEnoughData())
        //{
        //    //Copio la finestra di dati
        //    for (int i = 0; i < winSize; i++)
        //    {
        //        tempWindow[0][i] = inBuffer[inStartPivot] * window[i];
        //        inStartPivot = (inStartPivot + 1) % (inBuffer.Length);
        //        inLength--;
        //    }
        //    //rimetto in circolo il pezzo di dato non utilizzato
        //    inLength = inLength + overlap;
        //    inStartPivot = ((inStartPivot - overlap) + inBuffer.Length)  % (inBuffer.Length);

        //    FourierTransform.FFT(tempWindow[0], FourierTransform.Direction.Forward);

        //    for (int i = 0; i < winSize*2; i++)
        //    {
        //        tempWindow[1][i] = tempWindow[0][i] * hrtfs[0][i] * winSize*2;
        //        tempWindow[2][i] = tempWindow[0][i] * hrtfs[1][i] * winSize*2;
        //        //Jffts_hanningA[1][i] = Jffts_hanningA[0][i];
        //        //Jffts_hanningA[2][i] = Jffts_hanningA[0][i];
        //    }
        //    FourierTransform.FFT(tempWindow[1], FourierTransform.Direction.Backward);
        //    FourierTransform.FFT(tempWindow[2], FourierTransform.Direction.Backward);

        //    /*
        //     * ELABORAZIONI --- ToDo
        //     */

        //    //Salvo i dati elaborati
        //    for (int i = 0; i < winSize * 2; i++)
        //    {
        //        int start = (outStartPivot + outLength - overlap) % outBuffer[0].Length;
        //        //outBuffer[0][start].Re += 3;
        //        double a = outBuffer[0][start].Re + tempWindow[1][i].Re;
        //        double b = outBuffer[1][start].Re + tempWindow[2][i].Re;
        //        /*Canale destro?*/
        //        outBuffer[0][start].Re = a;
        //        /*Canale sinistro?*/
        //        outBuffer[1][start].Re = b;
        //        outLength++;
        //    }
        //    outLength -= (winSize + overlap);


        //    //ss++;
        //}

        //Copio i dati in uscita
        //float[][] outData = new float[2][];

        //for (int i = 0; i < outData.Length; i++)
        //{
        //    outData[i] = new float[buffSize];
        //}

        //for (int i = 0; i < buffSize; i++)
        //{
        //    outData[0][i] = (float)outBuffer[0][outStartPivot].Re;
        //    outData[1][i] = (float)outBuffer[1][outStartPivot].Re;
        //    outBuffer[0][outStartPivot] = new Complex();
        //    outBuffer[1][outStartPivot] = new Complex();
        //    outStartPivot = (outStartPivot + 1) % outBuffer[0].Length;
        //    outLength--;
        //}


    }






    int step = 0;
    public void WriteFile()
    {
        //STAMPO I PRIMI SAMPLES (DA CANCELLARE POI)
        if (step < 10)
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
        string path = "Assets/Resources/test.txt";
        StreamWriter writer = new StreamWriter(path, true);
        for (int i = 0; i < data.Length; i++)
        {
            writer.WriteLine(data[i].ToString());
        }
        writer.Close();
    }
}