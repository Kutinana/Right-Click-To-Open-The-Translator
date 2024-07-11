using System;

// 通用渲染管线程序集
namespace UnityEngine.Rendering.Universal
{
    // 实例化类     添加到Volume组件菜单中
    [Serializable, VolumeComponentMenu("TranslatorFilter/Blit")]
    // 继承VolumeComponent组件和IPostProcessComponent接口，用以继承Volume框架
    public class Blit : VolumeComponent, IPostProcessComponent
    {
        // 在框架下的属性与Unity常规属性不一样，例如 Int 由 ClampedIntParameter 取代。
        public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0, 1);
        [HideInInspector] public MaterialParameter materialParameter = new MaterialParameter(null);

        private void Awake() {
            
        }
        // 实现接口
        public bool IsActive()
        {
            return active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}