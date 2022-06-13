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

        public static float BASKET_SPEED = 0.05f;
        public static float BASKET_HALF_WIDTH = 0.2f;

        public static float BASKET_UPPER_BOUND = -0.17f;
        public static float BASKET_LOWER_BOUND = -0.97f;

        public static float BASKET_TOP_POS = -0.75f;
        public static float BASKET_BOTTOM_POS = -0.95f;

        public static int BackgroundTextureId, BasketTextureId;

        private float basketDx = 0, basketDy = 0;

        public Program() : base(WIDTH, HEIGHT, GraphicsMode.Default, TITLE)
        {
            this.initWindow();
        }


        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            GL.ClearColor(Color.FromArgb(0, 191, 255));

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            BackgroundTextureId = Utilities.LoadTexture(@"Images\bg.png");
            BasketTextureId = Utilities.LoadTexture(@"Images\basket.png");

        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Mouse[MouseButton.Left])
            {
                Console.WriteLine(this.Location.ToString());
            }

            if (Keyboard[Key.Right])
            {
                if(basketDx + BASKET_SPEED + BASKET_HALF_WIDTH < 1)
                    basketDx += BASKET_SPEED;
            }

            if (Keyboard[Key.Left])
            {
                if (basketDx - BASKET_SPEED - BASKET_HALF_WIDTH > -1)
                    basketDx -= BASKET_SPEED;
            }

            if (Keyboard[Key.Up])
            {
                if (basketDy + BASKET_SPEED + BASKET_TOP_POS < BASKET_UPPER_BOUND)
                    basketDy+= BASKET_SPEED;
            }

            if (Keyboard[Key.Down])
            {
                if (basketDy - BASKET_SPEED + BASKET_BOTTOM_POS > BASKET_LOWER_BOUND)
                    basketDy -= BASKET_SPEED;
            }
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);


            renderBackground();
            renderBasket();



            SwapBuffers();
        }

        private void renderBackground()
        {

            GL.BindTexture(TextureTarget.Texture2D, BackgroundTextureId);

            GL.Begin(BeginMode.Polygon);

            GL.TexCoord2(1, 0);
            GL.Vertex2(1, 1);

            GL.TexCoord2(1, 1);
            GL.Vertex2(1, -1);

            GL.TexCoord2(0, 1);
            GL.Vertex2(-1, -1);

            GL.TexCoord2(0, 0);
            GL.Vertex2(-1, 1);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.End();
        }


        private void renderBasket()
        {
            GL.BindTexture(TextureTarget.Texture2D, BasketTextureId);

            GL.Begin(BeginMode.Polygon);

            GL.TexCoord2(1, 0);
            GL.Vertex2(BASKET_HALF_WIDTH + basketDx, BASKET_TOP_POS + basketDy);

            GL.TexCoord2(1, 1);
            GL.Vertex2(BASKET_HALF_WIDTH + basketDx, BASKET_BOTTOM_POS + basketDy);

            GL.TexCoord2(0, 1);
            GL.Vertex2(-BASKET_HALF_WIDTH + basketDx, BASKET_BOTTOM_POS + basketDy);

            GL.TexCoord2(0, 0);
            GL.Vertex2(-BASKET_HALF_WIDTH + basketDx, BASKET_TOP_POS + basketDy);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.End();
        }

        private void initWindow()
        {
            int width = DisplayDevice.Default.Width;
            int height = DisplayDevice.Default.Height;
            Point startingPoint;
            if (width == 1920 && height == 1080)
            {
                startingPoint = new Point(761 - (WIDTH / 2), 10);
            }
            else if(width == 1366 && height == 768)
            {
                startingPoint = new Point(676 - (WIDTH / 2), 10);
                this.Height = 680;
            }
            else
            {
                return;
            }
            this.Location = startingPoint;
        }

        static void Main(string[] args)
        {
            
            Program program = new Program();
            program.Run();
            
        }

    }
}
