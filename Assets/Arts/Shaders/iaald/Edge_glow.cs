using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TestScripts
{
    public class ToUseGlowShader : MonoBehaviour
    {
        void Start()
        {
            //Get Material.
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            
            //Glow on
            spriteRenderer.material.SetFloat("_IsGlowOn", 1f);

            //Glow off
            //spriteRenderer.material.SetFloat("_IsGlowOn", 0f);

            /**
            其他参数
            float _Offset   :高斯模糊采样偏移量。影响边缘粗细。
            float _ClipThreshold    :对模糊后的A通道做高通的阈值。0为不裁剪，默认0.05以保持相对清晰的像素边缘。
            Vector4 _Color  :高亮颜色
            **/
        }
    }
}
