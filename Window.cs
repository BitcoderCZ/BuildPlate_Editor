using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using OpenTK.Input;
using System.Runtime.InteropServices;

using BuildPlate_Editor.Maths;
using System.Diagnostics;
using System.IO;

using Font = BuildPlate_Editor.Font;
using BuildPlate_Editor.UI;
using System.Threading;

namespace BuildPlate_Editor
{
    public class Window : GameWindow
    {
        public float AspectRatio;

        Shader shader;
        Shader shader2;
        Shader skyboxShader;
        Shader uiShader;

        bool worldLoaded = false;

        // Mouse
        bool mouseLocked;
        Point lastMousePos;
        Point origCursorPosition; // position before lock

        public Font font;
        public KeyboardState keyboardState;
        public Vector2 mousePos;

        public Window()
        {
            Width = 1280;
            Height = 720;
            AspectRatio = (float)Width / (float)Height;
            Title = "BuildPlate_Editor";
        }

        public DebugProc debMessageCallback;

        protected override void OnLoad(EventArgs e)
        {
            MakeCurrent();
            GL.Enable(EnableCap.DebugOutput);
            debMessageCallback = new DebugProc(MessageCallback); // Fixed error: A callback was made on a garbage collected delegate
            GL.DebugMessageCallback(debMessageCallback, IntPtr.Zero);

            // load shaders
            shader = new Shader(); // chunk shader
            shader.Compile("shader");
            shader2 = new Shader(); // not used by anything now
            shader2.Compile("shader2");
            skyboxShader = new Shader();
            skyboxShader.Compile("skybox");
            uiShader = new Shader(); // Used by UI
            uiShader.Compile("ui");

            // all UI stuff copyed from my minecraft repo
            // load font
            font = new Font("Minecraft", 32);
            GUI.Init(font);
            GUI.SetScene(0); // loading

#if DEBUG
            World.Init();
#else
                try {
                    World.Init();
                }
                catch (Exception ex) {
                    Util.Exit(EXITCODE.World_Unknown, ex);
                }
#endif

            Thread worldLoadThread = new Thread(() =>
            {
                World.InitChunks();
                worldLoaded = true;
            });
            worldLoadThread.Start();

            SkyBox.Init("Cold_Sunset", Camera.position, 100f);
            
            base.WindowBorder = WindowBorder.Fixed;
            base.WindowState = WindowState.Normal;
            GL.Viewport(0, 0, Width, Height);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            shader.Bind();
            Camera.SetRotation(new Vector3(0f, 180f, 0f));
            Camera.UpdateView(Width, Height);
            shader.UploadMat4("uProjection", ref Camera.projMatrix);
            shader.UploadMat4("uView", ref Camera.viewMatrix);

            Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);

            LockMouse();
        }

        private void MessageCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            if (id == 131185)
                return;
            byte[] managedArray = new byte[length];
            Marshal.Copy(message, managedArray, 0, length);
            Console.WriteLine($"MessageCallback: Source:{source}, Type:{type}, id:{id}, " +
                $"Severity:{severity}, Message: {Encoding.ASCII.GetString(managedArray)}");
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            float delta = (float)e.Time;

            if (GUI.Scene == 1) {
                // Rotation
                if (keyboardState.IsKeyDown(Key.Left))
                    Camera.Rotation.Y += delta * 160f;
                else if (keyboardState.IsKeyDown(Key.Right))
                    Camera.Rotation.Y -= delta * 160f;
                if (keyboardState.IsKeyDown(Key.Up))
                    Camera.Rotation.X -= delta * 80f;
                else if (keyboardState.IsKeyDown(Key.Down))
                    Camera.Rotation.X += delta * 80f;
            }

            if (mouseLocked) {
                var mouseDelta = System.Windows.Forms.Cursor.Position - new Size(lastMousePos);
                if (mouseDelta != Point.Empty) {
                    Camera.Rotation.X += mouseDelta.Y * 0.25f;
                    Camera.Rotation.Y += -mouseDelta.X * 0.25f;
                    CenterCursor();
                }
            }


            if (Camera.Rotation.X < -85)
                Camera.Rotation.X = -85;
            else if (Camera.Rotation.X > 85)
                Camera.Rotation.X = 85;

            // Movement
            if (GUI.Scene == 1) {
                if (keyboardState.IsKeyDown(Key.W))
                    Camera.Move(0f, delta * 8f);
                else if (keyboardState.IsKeyDown(Key.S))
                    Camera.Move(180f, delta * 8f);
                if (keyboardState.IsKeyDown(Key.A))
                    Camera.Move(90f, delta * 8f);
                else if (keyboardState.IsKeyDown(Key.D))
                    Camera.Move(270f, delta * 8f);
                if (keyboardState.IsKeyDown(Key.Space))
                    Camera.position.Y += delta * 6f;
                else if (keyboardState.IsKeyDown(Key.ShiftLeft))
                    Camera.position.Y -= delta * 6f;
            }

            Console.Title = Camera.position.ToString();

            // Other keyboard
            if (keyboardState.IsKeyDown(Key.Escape))
                UnlockMouse();

            GUI.Update(delta);

            float FPS = 1f / delta;
            Title = $"BuildPlate_Editor FPS: {SystemPlus.MathPlus.Round(FPS, 2)}";
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(Color.FromArgb(92, 157, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Bind();
            Camera.UpdateView(Width, Height);
            shader.UploadMat4("uProjection", ref Camera.projMatrix);
            shader.UploadMat4("uView", ref Camera.viewMatrix);
            if (GUI.Scene == 1)
                World.Render(shader);

            skyboxShader.Bind();
            skyboxShader.UploadMat4("uProjection", ref Camera.projMatrix);
            skyboxShader.UploadMat4("uView", ref Camera.viewMatrix);
            GL.Disable(EnableCap.CullFace);
            SkyBox.pos = Camera.position;
            if (GUI.Scene == 1)
                SkyBox.Render(skyboxShader);
            GL.Enable(EnableCap.CullFace);

            GL.DepthMask(false);
            GUI.Render(uiShader);
            GL.DepthMask(true);

            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            keyboardState = e.Keyboard;

            if (e.Key == Key.P) {
                Vector3i pos = (Vector3i)World.cursorPos;
                World.GetBlockIndex(pos, out int sbi, out int bi);
                uint chunkBlock = World.chunks[sbi].GetBlock(pos - World.chunks[sbi].pos * 16);
                string blockName = World.chunks[sbi].palette[chunkBlock].name;

                Console.WriteLine($"Palette Id: {World.GetBlock(sbi, bi)}, Texture Id: {World.GetBlockPalette(sbi, bi).textures[0]}," +
                                    $"Name: {World.GetBlockPalette(sbi, bi).name}");
                Console.WriteLine($"ID: {bi}, SUB: {sbi}, Chunk Pos: {World.chunks[sbi].pos * 16}, Cursor pos: {pos}, Name: {blockName}");
            }

            GUI.OnKeyDown(e.Key, e.Modifiers);
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            keyboardState = e.Keyboard;
            GUI.OnKeyUp(e.Key, e.Modifiers);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            GUI.OnKeyPress(e.KeyChar);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            LockMouse();
            GUI.OnMouseDown(e.Button, e.Position);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            GUI.OnMouseUp(e.Button, e.Position);
        }

        // Mouse
        private void CenterCursor()
        {
            System.Windows.Forms.Cursor.Position = new Point(Width / 2 + Location.X, Height / 2 + Location.Y);
            lastMousePos = System.Windows.Forms.Cursor.Position;
        }
        protected void LockMouse()
        {
            mouseLocked = true;
            origCursorPosition = System.Windows.Forms.Cursor.Position;
            CursorVisible = false;
            CenterCursor();
        }

        protected void UnlockMouse()
        {
            mouseLocked = false;
            CursorVisible = true;
            System.Windows.Forms.Cursor.Position = origCursorPosition;
        }
    }
}
