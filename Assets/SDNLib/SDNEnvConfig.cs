using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Linq;

public class SDNEnvConfig : MonoBehaviour
{
    public string CIPIC;
    public bool UsePersonalizedSDN = true;
    //public HRTFcontainer HRTFCamera;




    ///<summary>
    ///OLD HRTFcontainer
    /// </summary>
    ///



    private string cipic_subject;

    private static int hrtfLength = 200;
    private static int azNum = 27;
    private static int elNum = 52;

    private bool loaded = false; // flag for hrtf loading finish
    private int buffSize;
    private int fftLength;

    // Triple Jagged Arrays containing all azimuth and elevations hrtfs. Matrix of (25,50,200)
    private AForge.Math.Complex[][][] matrix_l = new AForge.Math.Complex[azNum][][]; // left hrtf 3d matrix, as in Matlab
    private AForge.Math.Complex[][][] matrix_r = new AForge.Math.Complex[azNum][][]; // right hrtf 3d matrix, as in Matlab

    // Delaunay triangles and points (in Matlab)
    private int[][] triangles = new int[2652][]; // 2652 x 3
    private float[][] points = new float[1404][]; // 1404 x 2

    // interp param
    float g1, g2, g3, det;
    float[] A, B, C, T, invT, X;
    int[][] indices = new int[3][];

    private float[] azimuths = new float[27] { -90, -80, -65, -55, -45, -40, -35, -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 55, 65, 80, 90 };
    private float[] elevations = new float[52] { -90, -45, -39.374f, -33.748f, -28.122f, -22.496f, -16.87f, -11.244f, -5.618f, 0.00800000000000267f, 5.634f, 11.26f, 16.886f, 22.512f, 28.138f, 33.764f, 39.39f, 45.016f, 50.642f, 56.268f, 61.894f, 67.52f, 73.146f, 78.772f, 84.398f, 90.024f, 95.65f, 101.276f, 106.902f, 112.528f, 118.154f, 123.78f, 129.406f, 135.032f, 140.658f, 146.284f, 151.91f, 157.536f, 163.162f, 168.788f, 174.414f, 180.04f, 185.666f, 191.292f, 196.918f, 202.544f, 208.17f, 213.796f, 219.422f, 225.048f, 230.674f, 270 };

    private void Awake()
    {

        //CREO IL BOUNDARY
        GameObject bound = new GameObject("bounds_DO_NOT_REMOVE");
        bound.AddComponent<boundary>();
    }

    // Use this for initialization
    void Start()
    {
        //Application.targetFrameRate = 10;

        AudioConfiguration AC = AudioSettings.GetConfiguration();
        Debug.Log(AC.sampleRate);
        AC.dspBufferSize = 512;
        AudioSettings.Reset(AC);

        buffSize = AC.dspBufferSize;
        fftLength = 2 * buffSize;
        Debug.Log(AC.dspBufferSize);
        // full hrtf set
        for (int i = 0; i < matrix_l.Length; i++)
        {
            matrix_l[i] = new AForge.Math.Complex[elNum][];
            matrix_r[i] = new AForge.Math.Complex[elNum][];
            for (int jj = 0; jj < matrix_l[i].Length; jj++)
            {
                matrix_l[i][jj] = new AForge.Math.Complex[fftLength];
                matrix_r[i][jj] = new AForge.Math.Complex[fftLength];
            }
        }

        // HRTF selection -> generic or personalised

        if (UsePersonalizedSDN)
        {
            cipic_subject = CIPIC;
            if (cipic_subject == "165")
                Debug.Log("NEED A PERSONALISED HRTF DATASET ! CHANGE IN SUBJECT_INFO.");
        }
        else
            cipic_subject = "165"; // KEMAR HATS with small pinnae


        string textFile; // 2d text file containing left and right hrtfs for a specific azimuth
        string fileName;
        //string filePath = subj_i.DataPath + "subject" + cipic_subject + "_txt";

        // list all hrtf text files in the directory
        //var targetDirectory = Directory.GetFiles(filePath, "*.txt", SearchOption.TopDirectoryOnly); //  SearchOption.TopDirectoryOnly
        //            Debug.Log("number of files:" + targetDirectory.GetLength(0))

        TextAsset[] txt_a = Resources.LoadAll<TextAsset>("subject" + cipic_subject + "_txt");

        int i_l = 0, i_r = 0, j = 0, k = 0;      //j_l = 0, j_r = 0, k_l = 0, k_r = 0;

        foreach (TextAsset txtfile in txt_a)
        //foreach (string currentFile in targetDirectory)
        {
            //fileName = Path.GetFileName(currentFile);
            //textFile = File.ReadAllText(currentFile);

            fileName = txtfile.name;
            textFile = txtfile.text;

            j = 0;
            k = 0;

            //Debug.Log(currentFile);

            if (fileName[0].Equals('L') == true)
            {
                //Debug.Log(fileName);
                //Debug.Log(textFile.Split('\n').Length);
                // store one by one    [i=azimuth, j=elevation, k=sample]
                foreach (var row in textFile.Split('\n'))
                {
                    if (j == elNum)
                    {
                        break;
                    }
                    k = 0;

                    foreach (var col in row.Trim().Split(' '))
                    {
                        //Debug.Log("i_l "+ i_l + "- j "+ j + "- k " + k);
                        matrix_l[i_l][j][k] = new AForge.Math.Complex(float.Parse(col.Trim(), System.Globalization.NumberStyles.Float), 0);
                        k++;
                    }
                    // compute fft
                    AForge.Math.FourierTransform.FFT(matrix_l[i_l][j], AForge.Math.FourierTransform.Direction.Forward);
                    j++;
                }
                i_l++;
            }
            else if (fileName[0].Equals('R') == true)
            {
                //Debug.Log(fileName);
                // store one by one    [i=azimuth, j=elevation, k=sample]
                foreach (var row in textFile.Split('\n'))
                {
                    if (j == elNum)
                    {
                        break;
                    }
                    k = 0;
                    foreach (var col in row.Trim().Split(' '))
                    {
                        matrix_r[i_r][j][k] = new AForge.Math.Complex(float.Parse(col.Trim(), System.Globalization.NumberStyles.Float), 0);
                        k++;
                    }
                    // compute fft
                    AForge.Math.FourierTransform.FFT(matrix_r[i_r][j], AForge.Math.FourierTransform.Direction.Forward);
                    j++;
                }
                i_r++;
            }
            else
            {
                //Debug.Log("WARNING! This file is not in the correct format!");
                //break;
            }

        }

        // LOAD triangles and points
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = new int[3];
        }
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new float[2];
        }
        // triangles

//        Debug.Log("subject" + cipic_subject + "_txt/triangles/triangles");
        TextAsset triang = Resources.Load<TextAsset>("subject" + cipic_subject + "_txt/triangles/triangles");

        //filePath = subj_i.DataPath + "/subject" + cipic_subject + "_txt/triangles/triangles.txt";
        //textFile = File.ReadAllText(filePath);

        textFile = triang.text;

        j = 0;
        foreach (var row in textFile.Split('\n'))
        {
            if (j == 2652)
            {
                break;
            }
            k = 0;
            foreach (var col in row.Trim().Split(' '))
            {
                triangles[j][k] = int.Parse(col.Trim(), System.Globalization.NumberStyles.Integer);
                k++;
            }
            j++;
        }
        // points
        TextAsset punti = Resources.Load<TextAsset>("subject" + cipic_subject + "_txt/points/points");

        //filePath = subj_i.DataPath + "/subject" + cipic_subject + "_txt/points/points.txt";
        //textFile = File.ReadAllText(filePath);

        textFile = punti.text;
        j = 0;
        foreach (var row in textFile.Split('\n'))
        {
            if (j == 1404)
            {
                break;
            }
            k = 0;
            foreach (var col in row.Trim().Split(' '))
            {
                points[j][k] = float.Parse(col.Trim(), System.Globalization.NumberStyles.Float);
                k++;
            }
            j++;
        }

        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = new int[2];
        }
        T = new float[4];
        invT = new float[4];
        X = new float[2];

        // flag
        loaded = true;
        Debug.Log("HRTF dataset loaded ! CIPIC subject ID: " + cipic_subject);


    }


    public AForge.Math.Complex[] getHRTF_left(int index1, int index2)
    {
        return matrix_l[index1][index2];
    }

    public AForge.Math.Complex[] getHRTF_right(int index1, int index2)
    {
        return matrix_r[index1][index2];
    }

    public AForge.Math.Complex[][] getInterpolated_HRTF(float[] aziEle)
    // Returns the interpolated HRTFs which are a linear combination of HRTFs A, B and C weighted by g1, g2 and g3, respectively.
    // Algorithm from Hannes Gamper, "Head-related transfer function interpolation in azimuth, elevation, and distance" (2013): https://asa.scitation.org/doi/pdf/10.1121/1.4828983
    // The following implementation is based on a JavaScript implementation of Tomasz Woźniak: https://github.com/tmwoz/hrtf-panner-js 
    {
        

        // Variables initialisation
        int i, j;
        AForge.Math.Complex[][] interpHrtf = new AForge.Math.Complex[2][];
        for (i = 0; i < interpHrtf.Length; i++)
        {
            interpHrtf[i] = new AForge.Math.Complex[fftLength];
        }

        //Interpolation
        i = triangles.Length - 1;
        T = new float[4];
        invT = new float[4];
        X = new float[2];
        while (i >= 0)
        {
            A = points[triangles[i][0] - 1]; // -1 because Matlab indexing
            B = points[triangles[i][1] - 1];
            C = points[triangles[i][2] - 1];
            T[0] = A[0] - C[0];
            T[1] = A[1] - C[1];
            T[2] = B[0] - C[0];
            T[3] = B[1] - C[1];
            invT[0] = T[3];
            invT[1] = -T[1];
            invT[2] = -T[2];
            invT[3] = T[0];
            det = 1 / (T[0] * T[3] - T[1] * T[2]);
            for (j = 0; j < invT.Length; j++)
                invT[j] *= det;
            X[0] = aziEle[0] - C[0];
            X[1] = aziEle[1] - C[1];
            g1 = invT[0] * X[0] + invT[2] * X[1];
            g2 = invT[1] * X[0] + invT[3] * X[1];
            g3 = 1 - g1 - g2;
            if (g1 >= 0 && g2 >= 0 && g3 >= 0)
            {
                indices[0] = getIndices(A[0], A[1]);
                indices[1] = getIndices(B[0], B[1]);
                indices[2] = getIndices(C[0], C[1]);
                for (j = 0; j < fftLength; j++)
                {
                    interpHrtf[0][j] = g1 * matrix_l[indices[0][0]][indices[0][1]][j] + g2 * matrix_l[indices[1][0]][indices[1][1]][j] + g3 * matrix_l[indices[2][0]][indices[2][1]][j];
                    interpHrtf[1][j] = g1 * matrix_r[indices[0][0]][indices[0][1]][j] + g2 * matrix_r[indices[1][0]][indices[1][1]][j] + g3 * matrix_r[indices[2][0]][indices[2][1]][j];
                }
                return interpHrtf;
            }
            i--;
        }
        return interpHrtf;
    }

    public int[] getIndices(float azimuth, float elevation) // get the matrix indices relative to azimuth (0) and elevation (1)
    {
        int[] indices = new int[2];

        float[] azimuths2 = new float[azNum];
        float[] elevations2 = new float[elNum];

        for (int i = 0; i < azNum; i++) // Is there a way to do it more efficiently than a for loop? 
        {
            azimuths2[i] = Math.Abs(azimuths[i] - azimuth);
        }
        for (int i = 0; i < elNum; i++)
        {
            elevations2[i] = Math.Abs(elevations[i] - elevation);
        }

        // Find index of minimum
        indices[0] = Enumerable.Range(0, azimuths2.Length).Aggregate((a, b) => (azimuths2[a] < azimuths2[b]) ? a : b);
        indices[1] = Enumerable.Range(0, elevations2.Length).Aggregate((a, b) => (elevations2[a] < elevations2[b]) ? a : b);

        return indices;
    }

    public int getHrtfLength()
    {
        return hrtfLength;
    }

    public bool getLoaded()
    {
        return loaded;
    }
}
