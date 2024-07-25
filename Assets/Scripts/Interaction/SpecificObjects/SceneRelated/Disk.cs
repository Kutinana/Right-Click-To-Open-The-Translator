using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Disk: MonoBehaviour
{
    public string name;
    private System.Drawing.Image image;
    private List<Texture2D> mTexture2DList;
    private float mTime = 0f;
    private float m_timeLeft;
    private float m_accum = 0f;
    private int m_frames = 0;
    private float fps;
    private SpriteRenderer mImage;
    private void Awake()
    {
        image = System.Drawing.Image.FromFile(Application.dataPath + "/Resources/Sprites/Objects/BeginPlace/" + name + ".gif");
        mTexture2DList = LoadImage();
        mImage = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {

    }

    private List<Texture2D> LoadImage()
    {
        List<Texture2D> tex = new List<Texture2D>();
        if (image != null)
        {
            FrameDimension frame = new FrameDimension(image.FrameDimensionsList[0]);
            int frameCount = image.GetFrameCount(frame);
            for (int i = 0; i < frameCount; i++)
            {
                image.SelectActiveFrame(frame, i);
                Bitmap frameBitmap = new Bitmap(image.Width, image.Height);
                using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(frameBitmap))
                {
                    graphics.DrawImage(image, Point.Empty);
                }
                Texture2D frameTexture = new Texture2D(frameBitmap.Width, frameBitmap.Height, TextureFormat.ARGB32, true);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                frameBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                byte[] bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);
                ms.Dispose();

                frameTexture.LoadImage(bytes);
                tex.Add(frameTexture);
            }
        }
        return tex;
    }
}