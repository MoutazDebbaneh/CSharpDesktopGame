using System;
using OpenTK;
using OpenTK.Graphics;
using System.Drawing;
using OpenTK.Input;
using System.Threading;
using System.Collections;

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

        public static float BALL_HEIGHT = 0.1f;
        public static float BALL_WIDTH = 0.1f;

        public static float BLOCK_HEIGHT = 0.1f;
        public static float BLOCK_WIDTH = 0.5f;

        public static float FALLING_SPEED = 0.015f;

        public static int BackgroundTextureId, BasketTextureId, playBtnTextureId, pauseBtnTextureId;
        public static int ballTextureId, blockTextureId;

        private float basketDx = 0, basketDy = 0;

        private int score = 0, misses = 0;

        private bool isPaused = false;

        private ArrayList BlocksList = new ArrayList();
        private ArrayList ballsList = new ArrayList();

        public static TextPrinter text = new TextPrinter(TextQuality.High);
        public static Font font = new Font("Times New Roman", 24, FontStyle.Bold);
        Random rnd = new Random();

        public Program() : base(WIDTH, HEIGHT, GraphicsMode.Default, TITLE)
        {
            this.initWindow();
            WindowBorder = WindowBorder.Fixed;
        }


        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            GL.ClearColor(Color.FromArgb(0, 191, 255));

            GL.Enable(EnableCap.Texture2D);
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            BackgroundTextureId = Utilities.LoadTexture(@"Images\bg.png");
            BasketTextureId = Utilities.LoadTexture(@"Images\basket.png");
            playBtnTextureId = Utilities.LoadTexture(@"Images\play.png");
            pauseBtnTextureId = Utilities.LoadTexture(@"Images\pause.png");
            ballTextureId = Utilities.LoadTexture(@"Images\ball.png");
            blockTextureId = Utilities.LoadTexture(@"Images\block.png");


            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(1.5);

            var timer = new Timer((ev) =>
            {
                int sign = (rnd.NextDouble() >= 0.5 ? 1 : -1);
                double num = sign * rnd.NextDouble();
                if (num > 0.85)
                    num -= 0.2;
                if (num < 0.1)
                    num += 0.2;
                if(rnd.NextDouble() >= 0.8)
                    BlocksList.Add(new Vector2((float)num, 1f));
                else
                    ballsList.Add(new Vector2((float)num, 1f));

            }, null, startTimeSpan, periodTimeSpan);


        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!isPaused)
            {
                if (Keyboard[Key.Right])
                {
                    if (basketDx + BASKET_SPEED + BASKET_HALF_WIDTH < 1)
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
                        basketDy += BASKET_SPEED;
                }

                if (Keyboard[Key.Down])
                {
                    if (basketDy - BASKET_SPEED + BASKET_BOTTOM_POS > BASKET_LOWER_BOUND)
                        basketDy -= BASKET_SPEED;
                }
            }

            if (Keyboard[Key.Space])
            {
                isPaused = !isPaused;
                Thread.Sleep(200);
            }


            for (int i = 0; i < ballsList.Count; i++)
            {
                Vector2 v = (Vector2)ballsList[i];

                if(v.Y - BALL_HEIGHT > BASKET_TOP_POS)
                {
                    //Console.WriteLine("ABOVE THE BASKET");
                }

                if(
                    v.Y - BALL_HEIGHT > BASKET_TOP_POS 
                    && (v.Y - BALL_HEIGHT - FALLING_SPEED <= BASKET_TOP_POS) 
                    && ((v.X + BALL_WIDTH >= basketDx - BASKET_HALF_WIDTH) && (v.X + BALL_WIDTH <= basketDx + BASKET_HALF_WIDTH))
                    )
                {
                    score += 1;
                    ballsList.RemoveAt(i);
                    i--;
                    continue;
                }

                v.Y -= FALLING_SPEED;
                ballsList[i] = v;
            }


            for (int i = 0; i < BlocksList.Count; i++)
            {
                Vector2 v = (Vector2)BlocksList[i];

                if (
                    (v.X <= basketDx - BASKET_HALF_WIDTH && v.X + BLOCK_WIDTH >= basketDx - BASKET_HALF_WIDTH)
                    ||
                    (v.X >= basketDx - BASKET_HALF_WIDTH && v.X <= basketDx + BASKET_HALF_WIDTH)
                   )
                {
                    if (
                        (v.Y >= BASKET_TOP_POS && v.Y - BLOCK_HEIGHT <= BASKET_TOP_POS)
                        ||
                        (v.Y <= BASKET_TOP_POS && v.Y - BLOCK_HEIGHT <= BASKET_TOP_POS)
                        )
                    {
                        Console.WriteLine("YOU LOST");
                        BlocksList.RemoveAt(i);
                        i--;
                        continue;
                    }
                }

                v.Y -= FALLING_SPEED;
                BlocksList[i] = v;
            }

        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {

            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            renderBackground();
            renderBasket();
            renderPlayPauseBtn();

            foreach (Vector2 ballVector in ballsList)
            {
                renderBall(ballVector);
            }

            foreach (Vector2 blockVector in BlocksList)
            {
                renderBlock(blockVector);
            }

            renderText();

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

        private void renderPlayPauseBtn()
        {

            int textureId = (isPaused ? playBtnTextureId : pauseBtnTextureId);

            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.Begin(BeginMode.Polygon);

            GL.TexCoord2(1, 0);
            GL.Vertex2(-0.8, 0.98);

            GL.TexCoord2(1, 1);
            GL.Vertex2(-0.8, 0.83);

            GL.TexCoord2(0, 1);
            GL.Vertex2(-0.95, 0.83);

            GL.TexCoord2(0, 0);
            GL.Vertex2(-0.95, 0.98);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.End();
        }

        private void renderBall(Vector2 v)
        {
            float x = v.X, y = v.Y;
            GL.BindTexture(TextureTarget.Texture2D, ballTextureId);

            GL.Begin(BeginMode.Polygon);

            GL.TexCoord2(1, 0);
            GL.Vertex2(x + BALL_WIDTH, y);

            GL.TexCoord2(1, 1);
            GL.Vertex2(x + BALL_WIDTH, y + BALL_HEIGHT);

            GL.TexCoord2(0, 1);
            GL.Vertex2(x, y + BALL_HEIGHT);

            GL.TexCoord2(0, 0);
            GL.Vertex2(x, y);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.End();
        }

        private void renderBlock(Vector2 v)
        {
            float x = v.X, y = v.Y;
            GL.BindTexture(TextureTarget.Texture2D, blockTextureId);

            GL.Begin(BeginMode.Polygon);

            GL.TexCoord2(1, 0);
            GL.Vertex2(x + BLOCK_WIDTH, y);

            GL.TexCoord2(1, 1);
            GL.Vertex2(x + BLOCK_WIDTH, y + BLOCK_HEIGHT);

            GL.TexCoord2(0, 1);
            GL.Vertex2(x, y + BLOCK_HEIGHT);

            GL.TexCoord2(0, 0);
            GL.Vertex2(x, y);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.End();
        }

        private void renderText()
        {

            if (isPaused)
            {
                text.Begin();
                text.Print("Score: " + score, font, Color.Black, new RectangleF(200, 18, 200, 80), OpenTK.Graphics.TextPrinterOptions.Default, OpenTK.Graphics.TextAlignment.Center);
                text.End();
            }


            text.Begin();
            text.Print("Misses: " + misses, font, Color.Black, new RectangleF(200, 18, 650, 80), OpenTK.Graphics.TextPrinterOptions.Default, OpenTK.Graphics.TextAlignment.Center);
            text.End();

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
