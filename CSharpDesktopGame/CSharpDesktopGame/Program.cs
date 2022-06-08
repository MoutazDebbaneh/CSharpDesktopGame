using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;

namespace CSharpDesktopGame
{
    class Program : GameWindow
    {
        public static string TITLE = "C# Desktop Game - Mini Project";

        public static int WIDTH = 600;
        public static int HEIGHT = 760;

        public static int textureId;

        public Program() : base(WIDTH, HEIGHT, GraphicsMode.Default, TITLE) { }


        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            GL.ClearColor(Color.FromArgb(0, 191, 255));

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            SwapBuffers();
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

    }
}
