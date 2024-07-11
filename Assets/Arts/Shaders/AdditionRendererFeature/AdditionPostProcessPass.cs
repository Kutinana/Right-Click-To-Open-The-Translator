namespace UnityEngine.Rendering.Universal
{
    public class AdditionPostProcessPass : ScriptableRenderPass
    {
        //标签名，用于续帧调试器中显示缓冲区名称
        const string CommandBufferTag = "AdditionalPostProcessing Pass";

        // 用于后处理的材质，cmd.Blit()方法需要调用的参数
        public Material m_Material;

        // 属性参数组件，将这个组件里的参数传递给对应的Shader
        Blit m_BlitInfoStruct;

        // 颜色渲染标识符，就是主纹理，或者说源纹理，或者说上一个渲染阶段的最终纹理的标识符，也是一个结构体，标识作用。
        RenderTargetIdentifier m_ColorAttachment;
        // 临时的渲染目标，将源纹理的渲染结果寄存在这里，它包含了标识符结构体。
        RenderTargetHandle m_TemporaryColorTexture01;

        public AdditionPostProcessPass(Material material){
            this.m_Material = material;
        }
        ScriptableRenderer m_scriptableRenderer;
        public void Setup(ScriptableRenderer renderer,Material Material)
        {
            // 初始化主纹理
            this.m_scriptableRenderer = renderer;
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            // 从堆栈中查找对应的属性参数组件
            m_BlitInfoStruct = stack.GetComponent<Blit>();
            this.m_ColorAttachment = m_scriptableRenderer.cameraColorTarget;
            // 从命令缓冲区池中获取一个带标签的命令缓冲区，该标签名可以在后续帧调试器中见到
            var cmd = CommandBufferPool.Get(CommandBufferTag);
            
            // 调用渲染函数
            Render(cmd, ref renderingData);

            // 执行命令缓冲区
            context.ExecuteCommandBuffer(cmd);
            // 释放命令缓存
            CommandBufferPool.Release(cmd);
            // 释放临时RT
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
        }

        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // VolumeComponent是否开启，且非Scene视图摄像机
            if (!renderingData.cameraData.isSceneViewCamera)
            {

                m_Material.SetFloat("_Intensity", m_BlitInfoStruct.Intensity.value);
                // 获取目标相机的描述信息，这个就是创建一个结构体，结构体里面有RenderTexture的各种参数，比如尺寸阿，深度图精度阿，等等
                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                // 设置深度缓冲区，就深度缓冲区的精度为0，不需要！深度缓冲区
                opaqueDesc.depthBufferBits = 0;
                // 通过目标相机的渲染信息创建临时缓冲区
                cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc);

                // 通过材质，将计算结果存入临时缓冲区
                cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier(),m_Material,0);
                // 再从临时缓冲区存入主纹理
                cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment);
            }
        }
    }
}
