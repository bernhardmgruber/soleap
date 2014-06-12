using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WPF;
using SoLeap.World;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SoLeap.Visualizer
{
    public class SceneRenderer
        : D3D11
    {
        #region Shader Code

        private const string VertexShaderCode = @"
float4 VShader(float4 position : POSITION) : SV_POSITION
{
    return position;
}
";

        private const string PixelShaderCode = @"
float4 PShader(float4 position : SV_POSITION) : SV_TARGET
{
    return float4(1.0f, 0.0f, 0.0f, 1.0f);
}
";

        #endregion Shader Code

        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout inputLayout;

        private Buffer vertexBuffer;


        public IWorld Scene
        {
            get { return (IWorld)GetValue(SceneProperty); }
            set { SetValue(SceneProperty, value); }
        }

        public static readonly DependencyProperty SceneProperty = DependencyProperty.Register(
            "Scene", typeof(IWorld), typeof(SceneRenderer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, ScenePropertyChanged));

        private static void ScenePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var renderer = (SceneRenderer)dependencyObject;
            renderer.SwitchScene((IWorld)args.OldValue, (IWorld)args.NewValue);
        }

        public SceneRenderer()
        {
        }

        private void SwitchScene(IWorld oldScene, IWorld newScene)
        {
            UnloadScene(oldScene);
            LoadScene(newScene);
        }

        private void LoadScene(IWorld newScene)
        {
            using (var vsBytecode = ShaderBytecode.Compile(VertexShaderCode, "VShader", "vs_4_0", ShaderFlags.EnableStrictness | ShaderFlags.Debug))
            using (var psBytecode = ShaderBytecode.Compile(PixelShaderCode, "PShader", "ps_4_0", ShaderFlags.EnableStrictness | ShaderFlags.Debug))
            using (var inputSignature = ShaderSignature.GetInputSignature(vsBytecode)) {
                vertexShader = new VertexShader(Device, vsBytecode);
                pixelShader = new PixelShader(Device, psBytecode);

                inputLayout = new InputLayout(Device, inputSignature, new[] {
                    new InputElement("POSITION", 0, Format.R32G32B32_Float, 0)
                });

                vertexBuffer = Device.CreateBuffer(new[] {
                    new Vector3(0.0f, 0.5f, 0.5f),
                    new Vector3(0.5f, -0.5f, 0.5f),
                    new Vector3(-0.5f, -0.5f, 0.5f)
                });
            }
        }

        private void UnloadScene(IWorld oldScene)
        {
            Set(ref vertexBuffer, null);
            Set(ref inputLayout, null);
            Set(ref pixelShader, null);
            Set(ref vertexShader, null);
        }

        public override void RenderScene(DrawEventArgs args)
        {
            if (Scene == null)
                return;

            Scene.Update();

            var context = Device.ImmediateContext;

            context.ClearRenderTargetView(RenderTargetView, Color.Aquamarine);
            context.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);

            context.InputAssembler.InputLayout = inputLayout;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 12, 0));
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);

            context.Draw(3, 0);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing) {
                if (Scene != null)
                    UnloadScene(Scene);
            }
        }
    }
}