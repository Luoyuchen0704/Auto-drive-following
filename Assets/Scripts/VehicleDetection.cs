using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using System.IO;
using Rect = OpenCvSharp.Rect;

public class VehicleDetection : MonoBehaviour
{
    // ���붯̬ͼ��
    public RenderTexture renderTexture;
    // ���ͼ��
    public RawImage rawImage;
    [Header("Haarcascade")]
    public double scaleFactor = 1.1;
    public int minNeighbors = 5;
    public int minSize = 40;

    // ����������
    private CascadeClassifier cascade;
    private Texture2D outputTex;
    private Mat rgbaMat;
    // �Ҷ�ͼ
    private Mat grayMat;

    void Start()
    {
        // ��ʼ��Texture��Mat,���true�������Կռ�
        outputTex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false, true);
        rgbaMat = new Mat(renderTexture.height, renderTexture.width, MatType.CV_8UC4);
        grayMat = new Mat();

        // ����Haar�����ļ�
        string xmlPath = Path.Combine(Application.streamingAssetsPath, "cars.xml");
        cascade = new CascadeClassifier(xmlPath);
    }

    void Update()
    {
        // ��RenderTexture��ȡ���ص�Texture2D
        Texture2D inputTex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false, true);
        RenderTexture.active = renderTexture;
        inputTex.ReadPixels(new UnityEngine.Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        inputTex.Apply();
        RenderTexture.active = null;

        // ת��Unity Texture��OpenCvSharp Mat
        rgbaMat = UnityOpenCvExtensions.TextureToMat(inputTex); 

        // ת��Ϊ�Ҷ�ͼ
        Cv2.CvtColor(rgbaMat, grayMat, ColorConversionCodes.RGBA2GRAY);
        // ֱ��ͼ���⣬����ͼ��Ա�ͼ����չǿ�ȷ�Χ
        Cv2.EqualizeHist(grayMat, grayMat);

        // ��⳵��,�ɵ��������������������䳵���ߴ�
        Rect[] vehicles = cascade.DetectMultiScale(
            grayMat,
            scaleFactor: scaleFactor,
            minNeighbors: minNeighbors,
            flags: HaarDetectionTypes.DoRoughSearch,
            minSize: new Size(minSize, minSize)
        );

        // ���ƺ�ɫ���ο�BGR��ɫ�ռ䣩
        foreach (Rect rect in vehicles)
        {
            Cv2.Rectangle(rgbaMat,
                new OpenCvSharp.Point(rect.X, rect.Y),
                new OpenCvSharp.Point(rect.X + rect.Width, rect.Y + rect.Height),
                new Scalar(0, 0, 255, 255), 2); // ��ɫ��BGR(0,0,255)
        }

        
        // Matͼ��ת����Unity Texture
        outputTex = UnityOpenCvExtensions.MatToTexture(rgbaMat, outputTex);
        rawImage.texture = outputTex;

        // �ͷ���ʱTexture
        Destroy(inputTex);
        
    }
}