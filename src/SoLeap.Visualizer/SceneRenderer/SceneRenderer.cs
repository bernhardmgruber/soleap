using System.Collections.Generic;
using System.Windows;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WPF;
using SoLeap.World;
using Buffer = SharpDX.Direct3D11.Buffer;
using Vector3 = SharpDX.Vector3;

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

        private readonly VertexShader vertexShader;
        private readonly PixelShader pixelShader;
        private readonly InputLayout inputLayout;

        private readonly ConstantBuffer<FrameConstants> frameConstantsBuffer;
        private readonly ConstantBuffer<ObjectConstants> objectConstantsBuffer;

        private Buffer vertexBuffer;
        private readonly IDictionary<IRenderable, RenderableIdentifier> renderableIdentifiers;

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
            renderableIdentifiers = new Dictionary<IRenderable, RenderableIdentifier>();

            using (var vsBytecode = ShaderBytecode.Compile(VertexShaderCode, "VShader", "vs_4_0", ShaderFlags.EnableStrictness | ShaderFlags.Debug))
            using (var psBytecode = ShaderBytecode.Compile(PixelShaderCode, "PShader", "ps_4_0", ShaderFlags.EnableStrictness | ShaderFlags.Debug))
            using (var inputSignature = ShaderSignature.GetInputSignature(vsBytecode)) {
                vertexShader = new VertexShader(Device, vsBytecode);
                pixelShader = new PixelShader(Device, psBytecode);

                inputLayout = new InputLayout(Device, inputSignature, new[] {
                    new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                    new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, Vector3.SizeInBytes)
                });
            }

            frameConstantsBuffer = new ConstantBuffer<FrameConstants>(Device);
            objectConstantsBuffer = new ConstantBuffer<ObjectConstants>(Device);
        }

        private void SwitchScene(IWorld oldScene, IWorld newScene)
        {
            UnloadScene(oldScene);
            LoadScene(newScene);
        }

        private void LoadScene(IWorld newScene)
        {
            var converter = new CollisionShapeConverter();

            var positions = new List<Vector3>();
            foreach (var rigidBodyRenderable in newScene.Renderables) {
                var vertices = converter.GetVertices(rigidBodyRenderable.CollisionShape);
                var ident = new RenderableIdentifier(offset: positions.Count / 3, vertexCount: vertices.Length / 3);

                positions.AddRange(vertices);
                renderableIdentifiers.Add(rigidBodyRenderable, ident);
            }

            vertexBuffer = Device.CreateBuffer(positions.ToArray());
        }

        private void UnloadScene(IWorld oldScene)
        {
            Set(ref vertexBuffer, null);
            renderableIdentifiers.Clear();
        }

        public override void RenderScene(DrawEventArgs args)
        {
            if (Scene == null)
                return;

            Scene.Update();

            frameConstantsBuffer.Update(new FrameConstants {
                View = Camera.View,
                Projection = Camera.Projection
            });

            var context = Device.ImmediateContext;

            context.ClearRenderTargetView(RenderTargetView, Color.Aquamarine);
            context.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);

            context.InputAssembler.InputLayout = inputLayout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Vector3.SizeInBytes, 0));

            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);

            foreach (var renderable in Scene.Renderables) {
                var ident = renderableIdentifiers[renderable];
                objectConstantsBuffer.Update(new ObjectConstants {
                    Color = renderable.Color.ToInt(),
                    World = renderable.WorldTransform
                });

                context.Draw(ident.VertexCount, ident.Offset);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing) {
                inputLayout.Dispose();
                pixelShader.Dispose();
                vertexShader.Dispose();

                frameConstantsBuffer.Dispose();
                objectConstantsBuffer.Dispose();

                if (Scene != null)
                    UnloadScene(Scene);
            }
        }

        private struct RenderableIdentifier
        {
            public readonly int Offset;
            public readonly int VertexCount;

            public RenderableIdentifier(int offset, int vertexCount)
            {
                Offset = offset;
                VertexCount = vertexCount;
            }
        }
    }
}