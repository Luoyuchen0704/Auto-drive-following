using UnityEngine;
using OpenCvSharp;
using System;

public static class UnityOpenCvExtensions
{
    // Texture2D ת Mat��֧�� RGBA32/RGB24 ��ʽ��
    public static Mat TextureToMat(Texture2D texture)
    {
        // ��������ʽ
        if (texture.format != TextureFormat.RGBA32 &&
            texture.format != TextureFormat.RGB24)
        {
            throw new ArgumentException("ֻ֧�� RGBA32 �� RGB24 ��ʽ�� Texture2D");
        }

        // ��ȡԭʼ��������
        byte[] textureData = texture.GetRawTextureData();

        // ���� Mat ����
        Mat mat = new Mat(texture.height, texture.width,
            (texture.format == TextureFormat.RGBA32) ?
                MatType.CV_8UC4 : MatType.CV_8UC3);

        // �����ݸ��Ƶ� Mat
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

        // Unity �� RGBA ת OpenCV �� BGRA����ѡ��
        if (texture.format == TextureFormat.RGBA32)
        {
            Cv2.CvtColor(mat, mat, ColorConversionCodes.RGBA2BGRA);
        }

        return mat;
    }

    // Mat ת Texture2D��֧�� CV_8UC4/CV_8UC3��
    public static Texture2D MatToTexture(Mat mat, Texture2D existingTexture = null)
    {
        // ��� Mat ����
        if (mat.Type() != MatType.CV_8UC4 && mat.Type() != MatType.CV_8UC3)
        {
            throw new ArgumentException("ֻ֧�� CV_8UC4 �� CV_8UC3 ���͵� Mat");
        }

        // �������� Texture2D
        TextureFormat format = (mat.Type() == MatType.CV_8UC4) ?
            TextureFormat.RGBA32 : TextureFormat.RGB24;

        if (existingTexture == null ||
            existingTexture.width != mat.Width ||
            existingTexture.height != mat.Height ||
            existingTexture.format != format)
        {
            existingTexture = new Texture2D(mat.Width, mat.Height, format, false);
        }

        // OpenCV �� BGRA ת Unity �� RGBA����ѡ��
        Mat outputMat = mat.Clone();
        if (format == TextureFormat.RGBA32)
        {
            Cv2.CvtColor(outputMat, outputMat, ColorConversionCodes.BGRA2RGBA);
        }

        // �� Mat ���ݸ��Ƶ� Texture
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