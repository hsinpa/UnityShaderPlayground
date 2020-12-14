using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class MyBlitRenderPass : ScriptableRenderPass
{
    string profilerTag;

    Material materialToBlit;
    RenderTargetIdentifier cameraColorTargetIdent;
    RenderTargetHandle tempTexture;
    Camera _camera;
    Mesh quad_;

    public MyBlitRenderPass(string profilerTag,
      RenderPassEvent renderPassEvent, Material materialToBlit)
    {
        this.profilerTag = profilerTag;
        this.renderPassEvent = renderPassEvent;
        this.materialToBlit = materialToBlit;
    }


    Mesh CreateQuad()
    {
        var mesh = new Mesh();
        mesh.vertices = new Vector3[4] {
            new Vector3( 1f,  1f,  0f),
            new Vector3(-1f,  1f,  0f),
            new Vector3(-1f, -1f,  0f),
            new Vector3( 1f, -1f,  0f),
        };
        mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }


    // This isn't part of the ScriptableRenderPass class and is our own addition.
    // For this custom pass we need the camera's color target, so that gets passed in.
    public void Setup(RenderTargetIdentifier cameraColorTargetIdent, Camera camera)
    {
        this.cameraColorTargetIdent = cameraColorTargetIdent;
        this._camera = camera;
    }

    // called each frame before Execute, use it to set up things the pass will need
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        // create a temporary render texture that matches the camera
        cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
    }

    // Execute is called for every eligible camera every frame. It's not called at the moment that
    // rendering is actually taking place, so don't directly execute rendering commands here.
    // Instead use the methods on ScriptableRenderContext to set up instructions.
    // RenderingData provides a bunch of (not very well documented) information about the scene
    // and what's being rendered.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!quad_) quad_ = CreateQuad();

        // fetch a command buffer to use
        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
        cmd.Clear();

        // the actual content of our custom render pass!
        // we apply our material while blitting to a temporary texture
        cmd.Blit(cameraColorTargetIdent, tempTexture.Identifier(), materialToBlit, 0);

        // ...then blit it back again 
        cmd.Blit(tempTexture.Identifier(), cameraColorTargetIdent);

        cmd.DrawMesh(quad_, Matrix4x4.identity, materialToBlit, 0, 0);

        // don't forget to tell ScriptableRenderContext to actually execute the commands
        context.ExecuteCommandBuffer(cmd);

        // tidy up after ourselves
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    // called after Execute, use it to clean up anything allocated in Configure
    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(tempTexture.id);
    }

}
