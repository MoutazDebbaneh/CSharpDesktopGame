using System;
using OpenTK;
using OpenTK.Graphics;
using System.Drawing;
using OpenTK.Input;
using System.Threading;
using System.Collections;
using System.Media;

namespace CSharpDesktopGame
{
    class Program : GameWindow
    {
        public static string TITLE = "C# Desktop Game - Mini Project";

        public static int WIDTH = 600;
        public static int HEIGHT = 760;

        public static float BASKET_SPEED = 0.05f;

        public static float BASKET_UPPER_BOUND = -0.17f;
        public static float BASKET_LOWER_BOUND = -1f;

        public static float BASKET_WIDTH = 0.4f;
        public static float BASKET_HEIGHT = 0.2f;

        Vector2 basketPosition = new Vector2(-BASKET_WIDTH/2, -0.75f);

        public static float BALL_HEIGHT = 0.1f;
        public static float BALL_WIDTH = 0.1f;

        public static float BLOCK_HEIGHT = 0.1f;
        public static float BLOCK_WIDTH = 0.5f;

        public static float FALLING_SPEED = 0.015f;

        public static double FALLING_FREQUENCY = 2;
        public static double FREQUENCY_CHANGE = 0.2;

        public static int VERTICAL_SHIFT = 0;
        public static double Y_TRANSLATE = 0.0;

        public static int BackgroundTextureId, BasketTextureId, playBtnTextureId, pauseBtnTextureId;
        public static int ballTextureId, blockTextureId;

        private int score = 0, misses = 0;

        private bool isPaused = false, gameOver = false, lowResolution = false;

        private ArrayList BlocksList = new ArrayList();
        private ArrayList ballsList = new ArrayList();

        public static TextPrinter text = new TextPrinter(TextQuality.High);
        public static Font font = new Font("Times New Roman", 24, FontStyle.Bold);
        Random rnd = new Random();
        private Timer timer;
        SoundPlayer bgSound, gameOverSound;

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
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            BackgroundTextureId = Utilities.LoadTexture(@"Images\bg.png");
            BasketTextureId = Utilities.LoadTexture(@"Images\basket.png");
            playBtnTextureId = Utilities.LoadTexture(@"Images\play.png");
            pauseBtnTextureId = Utilities.LoadTexture(@"Images\pause.png");
            ballTextureId = Utilities.LoadTexture(@"Images\ball.png");
            blockTextureId = Utilities.LoadTexture(@"Images\block.png");

            bgSound = new SoundPlayer(@"bg-music.wav");
            gameOverSound = new SoundPlayer(@"gameover.wav");

            restartGame();
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            bool moveUp = false;

            if (!isPaused)
            {
                if (Keyboard[Key.Right])
                {
                    if (basketPosition.X + BASKET_SPEED + BASKET_WIDTH < 1)
                        basketPosition.X += BASKET_SPEED;
                }

                if (Keyboard[Key.Left])
                {
                    if (basketPosition.X - BASKET_SPEED > -1)
                        basketPosition.X -= BASKET_SPEED;
                }

                if (Keyboard[Key.Up])
                {
                    if (basketPosition.Y + BASKET_SPEED + BASKET_HEIGHT < BASKET_UPPER_BOUND)
                    {
                        basketPosition.Y += BASKET_SPEED;
                        moveUp = true;
                    }
                }

                if (Keyboard[Key.Down])
                {
                    if (basketPosition.Y - BASKET_SPEED  > BASKET_LOWER_BOUND)
                        basketPosition.Y -= BASKET_SPEED;
                }
            }

            if (Keyboard[Key.Space])
            {
                if (gameOver)
                {
                    restartGame();
                    Thread.Sleep(200);
                    return;
                }

                if (!isPaused)
                
                    bgSound.Stop();
            
                else
                    bgSound.PlayLooping();

                isPaused = !isPaused;
                Thread.Sleep(200);
            }

            if (isPaused)
                return;


            for (int i = 0; i < ballsList.Count; i++)
            {
                Vector2 v = (Vector2)ballsList[i];

                if  (
                       (v.X >= basketPosition.X && v.X + BALL_WIDTH <= basketPosition.X + BASKET_WIDTH)
                    )
                {
                    if(
                            ((v.Y >= basketPosition.Y + 0.1)
                            &&
                            (v.Y - FALLING_SPEED <= basketPosition.Y + 0.1))
                        ||
                            ((moveUp && v.Y >= basketPosition.Y - BASKET_SPEED + 0.1) 
                            && 
                            (v.Y <= basketPosition.Y + 0.1))
                        )
                    {
                        score += 1;
                        increaseDifficulty();
                        ballsList.RemoveAt(i);
                        i--;
                        continue;
                    }
                    
                }

                v.Y -= FALLING_SPEED;
                ballsList[i] = v;

                if(v.Y < -1) {
                    misses += 1;
                    ballsList.RemoveAt(i);
                    i--;
                    if(misses >= 3)
                    {
                        endGame();
                        break;
                    }
                    continue;
                }
            }


            for (int i = 0; i < BlocksList.Count; i++)
            {
                Vector2 v = (Vector2)BlocksList[i];

                if (
                    (v.X <= basketPosition.X && v.X - 0.02 + BLOCK_WIDTH >= basketPosition.X)
                    ||
                    (v.X >= basketPosition.X && v.X - 0.02 <= basketPosition.X + BASKET_WIDTH)
                   )
                {

                    if (
                        (v.Y <= basketPosition.Y && v.Y >= basketPosition.Y - BASKET_HEIGHT)
                        ||
                        (v.Y - BLOCK_HEIGHT <= basketPosition.Y && v.Y - BLOCK_HEIGHT >= basketPosition.Y - BASKET_HEIGHT)
                        )
                    {
                        endGame();
                        break;
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
            renderBasket(basketPosition);
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


        private void renderBasket(Vector2 v)
        {
            float x = v.X, y = v.Y;
            GL.BindTexture(TextureTarget.Texture2D, BasketTextureId);

            GL.Begin(BeginMode.Polygon);

            GL.TexCoord2(1, 0);
            GL.Vertex2(x + BASKET_WIDTH, y);

            GL.TexCoord2(1, 1);
            GL.Vertex2(x + BASKET_WIDTH, y + BASKET_HEIGHT);

            GL.TexCoord2(0, 1);
            GL.Vertex2(x, y + BASKET_HEIGHT);

            GL.TexCoord2(0, 0);
            GL.Vertex2(x, y);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.End();
        }

        private void renderPlayPauseBtn()
        {

            int textureId = (isPaused ? playBtnTextureId : pauseBtnTextureId);

            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.Begin(BeginMode.Polygon);

            GL.TexCoord2(1, 0);
            GL.Vertex2(-0.8, 0.98 - Y_TRANSLATE);

            GL.TexCoord2(1, 1);
            GL.Vertex2(-0.8, 0.83 - Y_TRANSLATE);

            GL.TexCoord2(0, 1);
            GL.Vertex2(-0.95, 0.83 - Y_TRANSLATE);

            GL.TexCoord2(0, 0);
            GL.Vertex2(-0.95, 0.98 - Y_TRANSLATE);

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
                text.Print("Score: " + (score * 10), font, Color.Black, new RectangleF(210, 300, 200, 80), OpenTK.Graphics.TextPrinterOptions.Default, OpenTK.Graphics.TextAlignment.Center);
                text.End();
            }

            text.Begin();
            text.Print("Caught Balls: " + score, font, Color.Black, new RectangleF(160, 18 + VERTICAL_SHIFT, 250, 80), OpenTK.Graphics.TextPrinterOptions.Default, OpenTK.Graphics.TextAlignment.Center);
            text.End();

            if (gameOver)
            {
                text.Begin();
                text.Print("Game Over\n Press Space to Play Again", font, Color.Black, new RectangleF(110, 350, 400, 80), OpenTK.Graphics.TextPrinterOptions.Default, OpenTK.Graphics.TextAlignment.Center);
                text.End();
            }


            text.Begin();
            text.Print("Misses: " + misses, font, Color.Black, new RectangleF(200, 18 + VERTICAL_SHIFT, 650, 80), OpenTK.Graphics.TextPrinterOptions.Default, OpenTK.Graphics.TextAlignment.Center);
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
                lowResolution = true;
                VERTICAL_SHIFT = 68;
                Y_TRANSLATE = 0.18;
            }
            else
            {
                return;
            }
            this.Location = startingPoint;
        }

        private void startFallingTimer(double t)
        {

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(t);

            timer = new Timer((ev) =>
            {
                int sign = (rnd.NextDouble() >= 0.5 ? 1 : -1);
                double num = sign * rnd.NextDouble();
                if (num > 0.85)
                    num -= 0.2;
                if (num < 0.1)
                    num += 0.2;
                if (rnd.NextDouble() >= 0.8)
                    BlocksList.Add(new Vector2((float)num, 1f));
                else
                    ballsList.Add(new Vector2((float)num, 1f));

            }, null, startTimeSpan, periodTimeSpan);
        }

        private void stopFallingTimer()
        {
            this.timer.Dispose();
            this.timer = null;
        }

        private void endGame()
        {
            bgSound.Stop();
            gameOverSound.Play();
            isPaused = true;
            gameOver = true;
            stopFallingTimer();
            ballsList.Clear();
            BlocksList.Clear();
        }

        private void restartGame()
        {
            gameOver = false;
            isPaused = false;
            score = 0;
            misses = 0;
            startFallingTimer(FALLING_FREQUENCY);
            bgSound.PlayLooping();
        }
        
        private void increaseDifficulty()
        {
            if (score != 0 && score % 5 == 0 && FALLING_FREQUENCY > 0.5)
            {
                FALLING_FREQUENCY -= FREQUENCY_CHANGE;
                stopFallingTimer();
                startFallingTimer(FALLING_FREQUENCY);
            }
        }

        static void Main(string[] args)
        {
            
            Program program = new Program();
            program.Run();
            
        }

    }
}