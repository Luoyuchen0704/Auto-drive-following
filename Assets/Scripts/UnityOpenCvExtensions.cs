using UnityEngine;
using OpenCvSharp;
using System;

public static class UnityOpenCvExtensions
{
    // Texture2D 转 Mat（支持 RGBA32/RGB24 格式）
    public static Mat TextureToMat(Texture2D texture)
    {
        // 检查纹理格式
        if (texture.format != TextureFormat.RGBA32 &&
            texture.format != TextureFormat.RGB24)
        {
            throw new ArgumentException("只支持 RGBA32 或 RGB24 格式的 Texture2D");
        }

        // 获取原始像素数据
        byte[] textureData = texture.GetRawTextureData();

        // 创建 Mat 对象
        Mat mat = new Mat(texture.height, texture.width,
            (texture.format == TextureFormat.RGBA32) ?
                MatType.CV_8UC4 : MatType.CV_8UC3);

        // 将数据复制到 Mat
        unsafe
        {
            fixed (byte* pTextureData = textureData)
            {
                IntPtr matDataPtr = mat.Data;
                Buffer.MemoryCopy(
                    pTextureData,
                    (void*)matDataPtr,
                    textureData.Length,
                    textureData.Length
                );
            }
        }

        // Unity 的 RGBA 转 OpenCV 的 BGRA（可选）
        if (texture.format == TextureFormat.RGBA32)
        {
            Cv2.CvtColor(mat, mat, ColorConversionCodes.RGBA2BGRA);
        }

        return mat;
    }

    // Mat 转 Texture2D（支持 CV_8UC4/CV_8UC3）
    public static Texture2D MatToTexture(Mat mat, Texture2D existingTexture = null)
    {
        // 检查 Mat 类型
        if (mat.Type() != MatType.CV_8UC4 && mat.Type() != MatType.CV_8UC3)
        {
            throw new ArgumentException("只支持 CV_8UC4 或 CV_8UC3 类型的 Mat");
        }

        // 创建或复用 Texture2D
        TextureFormat format = (mat.Type() == MatType.CV_8UC4) ?
            TextureFormat.RGBA32 : TextureFormat.RGB24;

        if (existingTexture == null ||
            existingTexture.width != mat.Width ||
            existingTexture.height != mat.Height ||
            existingTexture.format != format)
        {
            existingTexture = new Texture2D(mat.Width, mat.Height, format, false);
        }

        // OpenCV 的 BGRA 转 Unity 的 RGBA（可选）
        Mat outputMat = mat.Clone();
        if (format == TextureFormat.RGBA32)
        {
            Cv2.CvtColor(outputMat, outputMat, ColorConversionCodes.BGRA2RGBA);
        }

        // 将 Mat 数据复制到 Texture
        byte[] textureData = new byte[outputMat.Total() * outputMat.ElemSize()];
        unsafe
        {
            fixed (byte* pTextureData = textureData)
            {
                IntPtr matDataPtr = outputMat.Data;
                Buffer.MemoryCopy(
                    (void*)matDataPtr,
                    pTextureData,
                    textureData.Length,
                    textureData.Length
                );
            }
        }

        existingTexture.LoadRawTextureData(textureData);
        existingTexture.Apply();

        return existingTexture;
    }
}