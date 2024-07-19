using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;

public class MemoryDiscUIController : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;
    private ParticleSystem m_particleSystem;
    private bool isPlaying = false;
    private static Vector3 startposition;
    List<Texture2D> texture2Ds;
    private void Awake()
    {
        m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_particleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startposition = transform.position;
    }
    //除PlayGif以外功能未测试
    public void ActivateAnimation(Vector3 playerPos,int i = 1)
    {
        texture2Ds = GifToTextureByCS(GetImage(i));
        this.transform.position = new Vector3(playerPos.x, startposition.y, startposition.z);
        StartCoroutine(PlayGif());
        StartCoroutine(GotoIEnumerator(playerPos));
    }
    public void Stop()
    {
        transform.position = startposition;
        isPlaying = false;
    }
    IEnumerator GotoIEnumerator(Vector3 playerPos)
    {
        float bias = 0.1f;
        while (!Mathf.Approximately(transform.position.y, playerPos.y + bias))
        {
            transform.position = new Vector3
            (
                transform.position.x,
                transform.position.y + Mathf.Lerp(transform.position.y, playerPos.y, 0.02f),
                transform.position.z
            );
            yield return null;
        }
    }
    IEnumerator PlayGif()
    {
        isPlaying = true;
        m_particleSystem.Play();
        while (true)
        {
            foreach (var i in texture2Ds)
            {
                Sprite s = Sprite.Create(i, new Rect(0, 0, i.width, i.height), Vector2.zero);
                m_spriteRenderer.sprite = s;
                yield return new WaitForFixedUpdate();
                if (!isPlaying) break;
            }
            if (!isPlaying) break;
        }
        m_particleSystem.Stop();
    }
    public static System.Drawing.Image GetImage(int i = 1)
    {
        Image imageFromFile = Image.FromFile("Assets/Arts/Prop/disk" + i + "animation.gif");
        return imageFromFile;
    }
    List<Texture2D> GifToTextureByCS(Image image)
    {
        List<Texture2D> texture2D = null;
        if (null != image)
        {
            texture2D = new List<Texture2D>();
            //Debug.LogError(image.FrameDimensionsList.Length);
            //image.FrameDimensionsList.Length = 1;
            //根据指定的唯一标识创建一个提供获取图形框架维度信息的实例;
            FrameDimension frameDimension = new FrameDimension(image.FrameDimensionsList[0]);
            //获取指定维度的帧数;
            int framCount = image.GetFrameCount(frameDimension);
            for (int i = 0; i < framCount; i++)
            {
                //选择由维度和索引指定的帧;
                image.SelectActiveFrame(frameDimension, i);
                var framBitmap = new Bitmap(image.Width, image.Height);
                //从指定的Image 创建新的Graphics,并在指定的位置使用原始物理大小绘制指定的 Image;
                //将当前激活帧的图形绘制到framBitmap上;
                System.Drawing.Graphics.FromImage(framBitmap).DrawImage(image, Point.Empty);
                var frameTexture2D = new Texture2D(framBitmap.Width, framBitmap.Height);
                for (int x = 0; x < framBitmap.Width; x++)
                {
                    for (int y = 0; y < framBitmap.Height; y++)
                    {
                        //获取当前帧图片像素的颜色信息;
                        System.Drawing.Color sourceColor = framBitmap.GetPixel(x, y);
                        //设置Texture2D上对应像素的颜色信息;
                        frameTexture2D.SetPixel(x, framBitmap.Height - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                    }
                }
                frameTexture2D.Apply();
                texture2D.Add(frameTexture2D);
            }
        }
        return texture2D;
    }
}
