using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Linq;



public struct HRTFData
{
    public AForge.Math.Complex[][] HRTFs;
    public float[] Delays;
}
public class SDNEnvConfig : MonoBehaviour
{
    public string CIPIC;
    public bool UsePersonalizedSDN = true;
    public bool UsePersistentDataPath = false;
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
    private int[][] triangles = new int[2652][]; // 2652 x 3-> punto1, punto2, punto3 datio in base alla posizione su points
    private float[][] points = new float[1404][]; // 1404 x 2-> azimuth e elevation?

    private int[][] itds = new int[1404][]; // 1404 righe x 2-> itd left e right
    private float[] headsize = new float[2];

    // interp param
    float g1, g2, g3, det;
    float[] A, B, C, T, invT, X;
    int[][] indices = new int[3][];

    private float[] azimuths = new float[27] { -90, -80, -65, -55, -45, -40, -35, -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 55, 65, 80, 90 };
    private float[] elevations = new float[52] { -90, -45, -39.374f, -33.748f, -28.122f, -22.496f, -16.87f, -11.244f, -5.618f, 0.00800000000000267f, 5.634f, 11.26f, 16.886f, 22.512f, 28.138f, 33.764f, 39.39f, 45.016f, 50.642f, 56.268f, 61.894f, 67.52f, 73.146f, 78.772f, 84.398f, 90.024f, 95.65f, 101.276f, 106.902f, 112.528f, 118.154f, 123.78f, 129.406f, 135.032f, 140.658f, 146.284f, 151.91f, 157.536f, 163.162f, 168.788f, 174.414f, 180.04f, 185.666f, 191.292f, 196.918f, 202.544f, 208.17f, 213.796f, 219.422f, 225.048f, 230.674f, 270 };

    public int BufferSize = 1024;
    public int SystemSampleRate = 44100;

    private void Awake()
    {
        if(UsePersistentDataPath){
            Debug.Log("Using your Persistend Data Path-->" + Application.persistentDataPath);
        }

        AudioConfiguration AC = AudioSettings.GetConfiguration();
        AC.dspBufferSize = BufferSize;
        AC.sampleRate = SystemSampleRate;
        AudioSettings.Reset(AC);

        //CREO IL BOUNDARY
        GameObject bound = new GameObject("bounds_DO_NOT_REMOVE");
        bound.AddComponent<boundary>();        
        
        buffSize = AC.dspBufferSize;

        fftLength = buffSize;
        int lats = buffSize*1000/AC.sampleRate;
        Debug.Log("SDN Lib correctly started at " + AC.sampleRate + "Hz with sample buffer " + AC.dspBufferSize+". Latency is " + lats +"ms");

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

        TextAsset[] txt_a;

        string folderdelimit = "/";
        #if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
                         folderdelimit = "\\";
        #endif

        if (!UsePersistentDataPath)
        {
            txt_a = Resources.LoadAll<TextAsset>("subject" + cipic_subject + "_txt");
        }
        else {
            Debug.Log("Using your Persistend Data Path-->" + Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt");
            string[] files = Directory.GetFiles(Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt", "*.txt");
            txt_a = new TextAsset[files.Length];
            for (int i = 0; i < files.Length; i++) {
                txt_a[i] = new TextAsset(File.ReadAllText(files[i]));
                txt_a[i].name = files[i].Split(folderdelimit).Last();
            }
        }


        int it_c = 0;

        //LOAD ITDs
        
        //*****NEW!!!!*****
        
        TextAsset ITDs;

        for (int i = 0; i < itds.Length; i++)
        {
            itds[i] = new int[2];
        }

        if (!UsePersistentDataPath)
        {
            ITDs = Resources.Load<TextAsset>("subject" + cipic_subject + "_txt/delays/delays");
            textFile = ITDs.text;
        }
        else
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt" + folderdelimit + "delays", "*.txt");
            textFile = File.ReadAllText(files[0]);
        }


        //Insert ITDs into the array
        it_c = 0;

        foreach (var row in textFile.Split('\n'))
        {

            int col_c = 0;
            //split row into columns
            string[] cols = row.Split(' ');
            if(cols.Length == 2 && it_c < itds.Length){
                itds[it_c][0] = (int)float.Parse(cols[0].Trim(), System.Globalization.NumberStyles.Integer);
                itds[it_c][1] = (int)float.Parse(cols[1].Trim(), System.Globalization.NumberStyles.Integer);
            }else{
                if(it_c < itds.Length){
                    Debug.Log("Error in ITD delays file. it_c = " + it_c);
                }
            }
            
            //increment column counter
            it_c++;

        }

        
        //Load Head Dimensions
        if (!UsePersistentDataPath)
        {
            ITDs = Resources.Load<TextAsset>("subject" + cipic_subject + "_txt/headsize/headsize");
            textFile = ITDs.text;
        }
        else
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt" + folderdelimit + "headsize", "*.txt");
            textFile = File.ReadAllText(files[0]);
        }



        //Insert head size into the array
        string[] size = textFile.Split(' ');
        headsize[0] = (int)float.Parse(size[0].Trim(), System.Globalization.NumberStyles.Float); //X1
        headsize[1] = (int)float.Parse(size[1].Trim(), System.Globalization.NumberStyles.Float); //X3
        Debug.Log("Head size is " + headsize[0] + "x" + headsize[1] + "cm");


        //LOAD HRTF IMPULSES AND DO FFT
        int i_l = 0, i_r = 0, k = 0;      //j_l = 0, j_r = 0, k_l = 0, k_r = 0;
        int j = 0;

        foreach (TextAsset txtfile in txt_a)
        {

            fileName = txtfile.name;
            textFile = txtfile.text;

            j = 0;
            k = 0;

            int itds_id_l = 0;
            int itds_id_r = 0;


            if (fileName[0].Equals('L') == true)
            {
                foreach (var row in textFile.Split('\n'))
                {
                    if (j == elNum)
                    {
                        break;
                    }
                    k = 0;

                    foreach (var col in row.Trim().Split(' '))
                    {
                        matrix_l[i_l][j][k] = new AForge.Math.Complex(float.Parse(col.Trim(), System.Globalization.NumberStyles.Float), 0);
                        k++;
                    }
                    matrix_l[i_l][j] = getMovedHRTF(matrix_l[i_l][j],itds_id_l,0);
                    itds_id_l++;
                        
                    // compute fft
                    AForge.Math.FourierTransform.FFT(matrix_l[i_l][j], AForge.Math.FourierTransform.Direction.Forward);
                    j++;
                }
                i_l++;
            }
            else if (fileName[0].Equals('R') == true)
            {
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
                    matrix_r[i_r][j] = getMovedHRTF(matrix_r[i_r][j], itds_id_r, 1);
                    itds_id_r++;
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
        TextAsset triang;

        
        //LOAD TRIANGLES AND POINTS
        if (!UsePersistentDataPath)
        {
            triang = Resources.Load<TextAsset>("subject" + cipic_subject + "_txt/triangles/triangles");
            textFile = triang.text;
        }
        else
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt" + folderdelimit + "triangles", "*.txt");
            textFile = File.ReadAllText(files[0]);
        }


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
        TextAsset punti;

        if (!UsePersistentDataPath)
        {
            punti = Resources.Load<TextAsset>("subject" + cipic_subject + "_txt/points/points");
            textFile = punti.text;
        }
        else
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt" + folderdelimit + "points", "*.txt");
            textFile = File.ReadAllText(files[0]);
        }



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


        if (!UsePersistentDataPath)
        {
            Debug.Log("HRTF dataset loaded! CIPIC subject ID: " + cipic_subject);
        }
        else {
            Debug.Log("External HRTF dataset correctly loaded!");
        }

    }

    private AForge.Math.Complex[] getMovedHRTF(AForge.Math.Complex[] data, int itd_id, int ear){
            
            //L'errore è qui, bisogna pescare l'itd giusto, altrimenti ha problemi
            /*if(ear == 0){
                Debug.Log(itds[itd_id][ear];);
            }*/
            //preparo la fadewindow per sincronizzare i campioni
            AForge.Math.Complex[] moved_data = new AForge.Math.Complex[data.Length];
            for(int i = 0; i < data.Length; i++){
                moved_data[i] = new AForge.Math.Complex(0,0);
                moved_data[i].Re = data[i].Re;
                moved_data[i].Im = data[i].Im;
            }
            

            float[] fade_window = new float[10];
            for(int i = 0; i < 10; i++)
            {
                fade_window[i] = 0.1f*(9-i);
            }

            //fade of last 10 samples
            for(int i = 0; i < 10; i++){
                moved_data[(data.Length - 10) + i].Re *= fade_window[i];
            }
    
            //calcolo il numero di campioni da spostare
            int samples_to_move = itds[itd_id][ear];

            //sposto i campioni
            for(int i = 0; i < data.Length - samples_to_move; i++){
                moved_data[i] = moved_data[i + samples_to_move];
            }
            //riempio con zeri i restanti spazi in fondo
            for(int i = data.Length - samples_to_move; i < data.Length; i++){
                moved_data[i] = new AForge.Math.Complex(0,0);
            }
            
            return moved_data;

    }

    /*float[] fade_window;
    private AForge.Math.Complex[] getMovedHRTF(AForge.Math.Complex[] data, int itd_id, int ear, float threshold = 0.4f){

        //Debug.Log(data.Length);
        //preparo la fadewindow per sincronizzare i campioni
        fade_window = new float[data.Length];
        for(int i = 0; i < data.Length-10; i++)
        {
            fade_window[i] = 1;
        }
        for(int i = 10; i > 0; i--)
        {
            fade_window[data.Length - i] = 10.0f - (i-1)*0.1f;
        }

        //faccio il fade degli ultimi 10 campioni per sicurezza
        for(int i = 0; i < fade_window.Length; i++){
            data[i].Re *= fade_window[i];
        }

        //cerco il max all'interno di data
        float max = 0;
        for(int i = 0; i < data.Length; i++){
            if(data[i].Re > max){
                max = (float)data[i].Re;
            }
        }

        //cerco il picco
        int peak_id=0;
        for(int i = 0; i < data.Length; i++){
            //normalize data
            data[i].Re /= max;
            //smooth data
            float smoothed_sample = 0;
            if(i < data.Length - 1){
                smoothed_sample = 0.5f*(float)data[i].Re + 0.5f*(float)data[i+1].Re;
            }
            //find first peak
            if(smoothed_sample > threshold){
                peak_id = i;
                break;
            }
        }

        for(int i = 0; i < data.Length - peak_id; i++){
            data[i].Re = data[i + peak_id].Re;
        }
        for(int i = data.Length - peak_id; i < data.Length; i++){
            data[i].Re = 0;
        }

        try{
        itds[itd_id][ear] = peak_id;
        }
        catch{
            Debug.Log("itd_id: " + itd_id + " ear: " + ear);
        }
        return data;
    }*/
    public AForge.Math.Complex[] getHRTF_left(int index1, int index2)
    {
        return matrix_l[index1][index2];
    }

    public AForge.Math.Complex[] getHRTF_right(int index1, int index2)
    {
        return matrix_r[index1][index2];
    }


    /*public AForge.Math.Complex[][] getInterpolated_HRTF(float[] aziEle)
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
    }*/

    public HRTFData getInterpolated_HRTFDatas(float[] aziEle){

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
                break;
            }
            i--;
        }

        HRTFData tmp = new HRTFData();
        tmp.HRTFs = interpHrtf;
        tmp.Delays = getInterpolated_ITDs(aziEle);

        return tmp;
    }

    public float[] getInterpolated_ITDs(float[] aziEle){

        //Computes the ITD for a spherical head
        //theta is the angle between the source and the head axis
        //a is the radius of the head
        //fs is the sampling frequency
        //returns the delay in samples

        //ITD from Duda and Brown
        //in cascade after the head shadow filter
        //here we use vertical-polar coordinate system -> theta [-180; 180]

        //Debug.Log("theta = " + theta);

        //Calculate left ear angle
        float thetaL = aziEle[0] + 90;
        //calculate right ear angle
        float thetaR = aziEle[0] - 90;

        double c = 343;

        //convert theta to radiants
        thetaL = thetaL * (float)Math.PI / 180.0f;
        thetaR = thetaR * (float)Math.PI / 180.0f;

        //optimal head radius
        float a = (0.41f*headsize[0]/2 + 0.22f*headsize[1]/2 + 3.7f) / 100;
        
        double ac = a/c;

        double delayL, delayR;

        if (Mathf.Abs(thetaL) < Mathf.PI/2){
            // adding ac to the result in order to have positive delays and keep the system causal, the ITD stays the same.
            delayL = -ac*Mathf.Cos(thetaL)+ac;
        }else{  //è corretto?
            delayL = ac*(Mathf.Abs(thetaL)-Mathf.PI/2)+ac;
        }

        if (Mathf.Abs(thetaR) < Mathf.PI/2){
            // adding ac to the result in order to have positive delays and keep the system causal, the ITD stays the same.
            delayR = -ac*Mathf.Cos(thetaR)+ac;
        }else{  //è corretto?
            delayR = ac*(Mathf.Abs(thetaR)-Mathf.PI/2)+ac;
        }


        //convert time-delays to samples
        float[] delay_samp = {(float)(delayL*SystemSampleRate), (float)(delayR*SystemSampleRate)};

        return delay_samp;

    }

    

    /*public int[] getInterpolated_ITDs(float[] azEl){
        int[] indexes = getIndices(azEl[0], azEl[1]);
        //Debug.Log("Ids: " + indexes[0] + "--" + indexes[1]);
        //Indexes[0] è il calore sulla X (azimuth ANGLE)
        //Indexes[1] è il valore sulla Y (elevation ANGLE)
        return itds[indexes[0]*52 + indexes[1]];
    }*/

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


    public void SwitchHRTF(string id){
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

        cipic_subject = id; // KEMAR HATS with small pinnae

        string textFile; // 2d text file containing left and right hrtfs for a specific azimuth
        string fileName;

        TextAsset[] txt_a;

        string folderdelimit = "/";

        #if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
                                 folderdelimit = "\\";
        #endif


        if (!UsePersistentDataPath)
        {
            txt_a = Resources.LoadAll<TextAsset>("subject" + cipic_subject + "_txt");
        }
        else
        {
            //Debug.Log("Using your Persistend Data Path-->" + Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt");
            string[] files = Directory.GetFiles(Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt", "*.txt");
            txt_a = new TextAsset[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                txt_a[i] = new TextAsset(File.ReadAllText(files[i]));
                txt_a[i].name = files[i].Split(folderdelimit).Last();
            }
        }


        int i_l = 0, i_r = 0, j = 0, k = 0;

        foreach (TextAsset txtfile in txt_a)
        {

            fileName = txtfile.name;
            textFile = txtfile.text;

            j = 0;
            k = 0;

            if (fileName[0].Equals('L') == true)
            {
                foreach (var row in textFile.Split('\n'))
                {
                    if (j == elNum)
                    {
                        break;
                    }
                    k = 0;

                    foreach (var col in row.Trim().Split(' '))
                    {
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
        TextAsset triang;



        if (!UsePersistentDataPath)
        {
            triang = Resources.Load<TextAsset>("subject" + cipic_subject + "_txt/triangles/triangles");
            textFile = triang.text;
        }
        else
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt" + folderdelimit + "triangles", "*.txt");
            textFile = File.ReadAllText(files[0]);
        }


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
        TextAsset punti;

        if (!UsePersistentDataPath)
        {
            punti = Resources.Load<TextAsset>("subject" + cipic_subject + "_txt/points/points");
            textFile = punti.text;
        }
        else
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath + folderdelimit + "subject" + cipic_subject + "_txt" + folderdelimit + "points", "*.txt");
            textFile = File.ReadAllText(files[0]);
        }

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


        if (!UsePersistentDataPath)
        {
            Debug.Log("HRTF dataset correctly swapped using PersistentDataPath! CIPIC subject ID: " + id);
        }
        else
        {
            Debug.Log("HRTF dataset correctly swapped! CIPIC subject ID: " + id);
        }
    }

}
