using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using System.IO;
using Rect = OpenCvSharp.Rect;

public class VehicleDetection : MonoBehaviour
{
    // 输入动态图像
    public RenderTexture renderTexture;
    // 输出图像
    public RawImage rawImage;
    [Header("Haarcascade")]
    public double scaleFactor = 1.1;
    public int minNeighbors = 5;
    public int minSize = 40;

    // 级联分类器
    private CascadeClassifier cascade;
    private Texture2D outputTex;
    private Mat rgbaMat;
    // 灰度图
    private Mat grayMat;

    void Start()
    {
        // 初始化Texture和Mat,添加true启用线性空间
        outputTex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false, true);
        rgbaMat = new Mat(renderTexture.height, renderTexture.width, MatType.CV_8UC4);
        grayMat = new Mat();

        // 加载Haar级联文件
        string xmlPath = Path.Combine(Application.streamingAssetsPath, "cars.xml");
        cascade = new CascadeClassifier(xmlPath);
    }

    void Update()
    {
        // 从RenderTexture读取像素到Texture2D
        Texture2D inputTex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false, true);
        RenderTexture.active = renderTexture;
        inputTex.ReadPixels(new UnityEngine.Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        inputTex.Apply();
        RenderTexture.active = null;

        // 转换Unity Texture到OpenCvSharp Mat
        rgbaMat = UnityOpenCvExtensions.TextureToMat(inputTex); 

        // 转换为灰度图
        Cv2.CvtColor(rgbaMat, grayMat, ColorConversionCodes.RGBA2GRAY);
        // 直方图均衡，改善图像对比图，扩展强度范围
        Cv2.EqualizeHist(grayMat, grayMat);

        // 检测车辆,可调整级联分类器参数适配车辆尺寸
        Rect[] vehicles = cascade.DetectMultiScale(
            grayMat,
            scaleFactor: scaleFactor,
            minNeighbors: minNeighbors,
            flags: HaarDetectionTypes.DoRoughSearch,
            minSize: new Size(minSize, minSize)
        );

        // 绘制红色矩形框（BGR颜色空间）
        foreach (Rect rect in vehicles)
        {
            Cv2.Rectangle(rgbaMat,
                new OpenCvSharp.Point(rect.X, rect.Y),
                new OpenCvSharp.Point(rect.X + rect.Width, rect.Y + rect.Height),
                new Scalar(0, 0, 255, 255), 2); // 红色：BGR(0,0,255)
        }

        
        // Mat图像转换回Unity Texture
        outputTex = UnityOpenCvExtensions.MatToTexture(rgbaMat, outputTex);
        rawImage.texture = outputTex;

        // 释放临时Texture
        Destroy(inputTex);
        
    }
}