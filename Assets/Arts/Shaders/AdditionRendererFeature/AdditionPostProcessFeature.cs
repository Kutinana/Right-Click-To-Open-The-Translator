namespace UnityEngine.Rendering.Universal
{
    public class AdditionPostProcessFeature : ScriptableRendererFeature
    {
        // 后处理Pass
        AdditionPostProcessPass postPass;
        // 根据Shader生成的材质
        public Material material;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // 设置调用后处理Pass，初始化参数
            postPass.Setup(renderer, material);
            // 添加该Pass到渲染管线中
            renderer.EnqueuePass(postPass);
        }

        // 对象初始化时会调用该函数
        public override void Create()
        {
            postPass = new AdditionPostProcessPass(material);
            // 渲染时机 = 透明物体渲染后
            postPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }
    }
}