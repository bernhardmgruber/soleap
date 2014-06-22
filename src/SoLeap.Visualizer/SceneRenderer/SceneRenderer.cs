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
        private const string ShaderFile = @"SceneShader.fx";

        private readonly VertexShader vertexShader;
        private readonly PixelShader pixelShader;
        private readonly InputLayout inputLayout;

        private readonly RasterizerState rasterizerState;

        private readonly ConstantBuffer<FrameConstants> frameConstantsBuffer;
        private readonly ConstantBuffer<ObjectConstants> objectConstantsBuffer;

        private Buffer vertexBuffer;
        private readonly IDictionary<IRenderable, RenderableIdentifier> renderableIdentifiers;

        #region Scene DependencyProperty
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
        #endregion

        public SceneRenderer()
        {
            renderableIdentifiers = new Dictionary<IRenderable, RenderableIdentifier>();

            using (var vsBytecode = ShaderBytecode.CompileFromFile(ShaderFile, "VertexShaderMain", "vs_4_0", ShaderFlags.EnableStrictness | ShaderFlags.Debug))
            using (var psBytecode = ShaderBytecode.CompileFromFile(ShaderFile, "PixelShaderMain", "ps_4_0", ShaderFlags.EnableStrictness | ShaderFlags.Debug))
            using (var inputSignature = ShaderSignature.GetInputSignature(vsBytecode)) {
                vertexShader = new VertexShader(Device, vsBytecode);
                pixelShader = new PixelShader(Device, psBytecode);

                inputLayout = new InputLayout(Device, inputSignature, new[] {
                    new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                    new InputElement("NORMAL", 0, Format.R32G32B32_Float, Vector3.SizeInBytes)
                });
            }

            var desc = RasterizerStateDescription.Default();
            desc.CullMode = CullMode.None;
            rasterizerState = new RasterizerState(Device, desc);

            frameConstantsBuffer = new ConstantBuffer<FrameConstants>(Device);
            objectConstantsBuffer = new ConstantBuffer<ObjectConstants>(Device);

            Camera = new FirstPersonCamera();
        }

        private void SwitchScene(IWorld oldScene, IWorld newScene)
        {
            if (oldScene != null)
                UnloadScene(oldScene);
            if (newScene != null)
                LoadScene(newScene);
        }

        private void LoadScene(IWorld newScene)
        {
            var converter = new CollisionShapeConverter();

            var verticesList = new List<VertexPositionNormal>();
            foreach (var rigidBodyRenderable in newScene.Renderables) {
                var vertices = converter.GetVertices(rigidBodyRenderable.CollisionShape);
                var ident = new RenderableIdentifier(offset: verticesList.Count, vertexCount: vertices.Count);

                verticesList.AddRange(vertices);
                renderableIdentifiers.Add(rigidBodyRenderable, ident);
            }

            vertexBuffer = Device.CreateBuffer(verticesList.ToArray());

            Camera.Position = new Vector3(0.0f, 400, -500.0f);
            Camera.LookAt = new Vector3(0.0f, 200.0f, 0.0f);
            Camera.NearPlane = 1.0f;
            Camera.FarPlane = 1000.0f;
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

            context.ClearRenderTargetView(RenderTargetView, Color.Azure);
            context.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);

            context.Rasterizer.State = rasterizerState;
            context.InputAssembler.InputLayout = inputLayout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, VertexPositionNormal.SizeInBytes, 0));

            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);

            context.VertexShader.SetConstantBuffer(0, frameConstantsBuffer.Buffer);
            context.VertexShader.SetConstantBuffer(1, objectConstantsBuffer.Buffer);
            context.PixelShader.SetConstantBuffer(0, frameConstantsBuffer.Buffer);

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
                rasterizerState.Dispose();

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