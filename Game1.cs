using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using System.IO;
using Color = Microsoft.Xna.Framework.Color;
using Tutorial_1.RC_Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SharpDX.Direct2D1;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace Tutorial_1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        SpriteFont fonty;
        Texture2D texBack = null;
        Texture2D texpaddle = null;
        Texture2D texBall = null;
        Texture2D texBoom = null;
        Texture2D texBlock1 = null;
        TextRenderable levelComp = null;
        TextRenderable levelFail = null;
        SpriteList sl = null;
        SpriteList booms = null;
        Sprite3 ball = null;
        Sprite3 paddle = null;
        ImageBackground back1 = null;
        Vector2 ballOffset = new Vector2(35, -10);
        Vector2 speed = new Vector2(2,- 3);
        Rectangle playArea;
        float xx = 350;
        float yy = 500;
        float paddlespeed = 3;
        int lhs = 236;
        int rhs = 564;
        int bot = 543;
        int top = 56;
        int ticks = 0;
        int ticker = 0;
        int score = 0;
        int hits = 0;
        int gameState = 0; 
        int blockOffsetX = 30;
        int blockOffsetY = 30;
        bool showbb = false;
        bool ballStuck = true;
        bool paddleCollisionEnabled = true;
        KeyboardState k;
        KeyboardState prevK;
        HitState hitState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.PreferredBackBufferWidth = 800;
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            LineBatch.init(GraphicsDevice);
            booms = new SpriteList();
            texBack = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\Tutorial 1\assets\Back3.png");

            texBall = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\Tutorial 1\assets\8fF8bjy.png");
            texBoom = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\Tutorial 1\assets\Boom3.png");
            ball = new Sprite3(true, texBall, xx+35, yy-10);
            ball.setBBandHSFractionOfTexCentered(0.7f);

            texpaddle = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\Tutorial 1\assets\Loaf.png");
            paddle = new Sprite3(true, texpaddle, xx, yy);
            paddle.setBBToTexture();

            fonty = Content.Load<SpriteFont>("File");

            back1 = new ImageBackground(texBack, Color.White, GraphicsDevice);

            playArea = new Rectangle(lhs, top, rhs - lhs, bot - top);

            texBlock1 = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\Tutorial 1\assets\white64x32.png");

            levelComp = new TextRenderable("LEVEL COMPLETE!", new Vector2(325, 200), fonty ,Color.MediumPurple);

            levelFail = new TextRenderable("LEVEL FAILED!", new Vector2(325, 200), fonty, Color.MediumPurple);

            populateBlocks();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevK = k;
            k = Keyboard.GetState();

            //gamestate stuff
            switch (gameState)
            {
                case 0: //waiting to start
                    if (k.IsKeyDown(Keys.Space)) gameState = 0;
                    break;
                case 1: //playing 
                    ticks++;
                    break;
                case 2: //menu
                    break;
                case 3: //reset hit
                    gameState = 0;
                    break;
                case 4: //win 
                    if (k.IsKeyDown(Keys.Space))
                    {
                        ResetGame();
                    }
                    break;
                case 5: //fail
                    if (k.IsKeyDown(Keys.Space))
                    {
                        ResetGame();
                    }
                    break;
            }

            //handle show bounding box
            if (k.IsKeyDown(Keys.B) && prevK.IsKeyUp(Keys.B)) showbb = !showbb;

            //handle left and right
            if (k.IsKeyDown(Keys.Right) & paddle.getPosX() < rhs - texpaddle.Width)
            {
                paddle.setPosX(paddle.getPosX() + paddlespeed);

                if (ballStuck)
                {
                    ball.setPosX(paddle.getPosX() + 35);
                }
            }
            if (k.IsKeyDown(Keys.Left) & paddle.getPosX() > lhs)
            {

                paddle.setPosX(paddle.getPosX() - paddlespeed);

                if (ballStuck)
                {
                    ball.setPosX(paddle.getPosX() + 35);
                }
            }

            if (k.IsKeyDown(Keys.C)) gameState = 4;

            //handle game restart
            if (k.IsKeyDown(Keys.R))
            {
                ResetGame();
            }

            //handle ball launch and game start
            if (gameState == 0 && ballStuck)
            {
                ball.setPos(paddle.getPos() + ballOffset);
                if (k.IsKeyDown(Keys.Space) && prevK.IsKeyUp(Keys.Space)) // keyup only for if two balls
                {
                    gameState = 1;
                    ballStuck = false;
                    ball.setDeltaSpeed(speed);
                    ticker = ticks;
                }
            }

            if (gameState == 1)
            {

                // handle paddle hitchecks
                Rectangle rr1 = paddle.getBoundingBoxAA();
                Rectangle rr2 = ball.getBoundingBoxAA();
                hitState = checkHit(rr1, rr2);

                //handle collisions
                
                if (!ballStuck)
                {
                    // handle ball movement and collision
                    ball.savePosition();
                    ball.moveByDeltaXY();
                    Rectangle ballbb = ball.getBoundingBoxAA();


                    if (ballbb.X + ballbb.Width > rhs)
                    {
                        ball.setDeltaSpeed(ball.getDeltaSpeed() * new Vector2(-1, 1));
                    }
                    if (ballbb.X < lhs)
                    {
                        ball.setDeltaSpeed(ball.getDeltaSpeed() * new Vector2(-1, 1));
                    }
                    if (ballbb.Y < top)
                    {
                        ball.setDeltaSpeed(ball.getDeltaSpeed() * new Vector2(1, -1));
                    }
                    if (ballbb.Y > bot)
                    {
                        gameState = 5;
                    }

                    // handle that bloody paddle

                    if (ticks > ticker + 30) paddleCollisionEnabled = true;

                    if (paddleCollisionEnabled & ticks > ticker + 30)
                    {
                        if (hitState.hitTop)
                        {
                            ball.setDeltaSpeed(-ball.getDeltaSpeed() * new Vector2(-1, 1));
                            paddleCollisionEnabled = false;
                            ticker = ticks;
                            hits++;
                        }
                        else if (hitState.hitLeft | hitState.hitRight)
                        {
                            ball.setDeltaSpeed(-ball.getDeltaSpeed() * new Vector2(1, -1));
                            paddleCollisionEnabled = false;
                            ticker = ticks;
                            hits++;
                        }
                        else if (hitState.hit)
                        {
                            ball.setDeltaSpeed(-ball.getDeltaSpeed() * new Vector2(-1, -1));
                            paddleCollisionEnabled = false;
                            ticker = ticks;
                            hits++;
                        }
                    }
                }             

                //handle block collision 

                Rectangle ballbbnow = ball.getBoundingBoxAA();
                int cnt = sl.countActive();
                int rc = sl.collisionWithRect(ballbbnow);
                if (rc != -1)
                {
                    Sprite3 temp = sl.getSprite(rc);
                    ball.setDeltaSpeed(ball.getDeltaSpeed() * new Vector2(1, -1)); // reflect balls
                    temp.hitPoints--;

                    if (temp.hitPoints <= 0)
                    {
                        temp.active = false;
                        temp.visible = false;
                        createExplosion((int)temp.getPosX(), (int)temp.getPosY());
                        speed = speed * 1.015f;
                        paddlespeed = paddlespeed * 1.01f;
                        ball.setDeltaSpeed(speed);
                        score++;
                    }
                    else
                    {
                        if (temp.hitPoints == 2)
                        {
                            temp.setColor(Color.LightPink);
                        }
                        if (temp.hitPoints == 1)
                        {
                            temp.setColor(Color.GhostWhite);
                        }
                    }
                }
                // level clear handling
                if (cnt <= 0)
                {
                    gameState = 4;
                }
            }
            booms.animationTick(gameTime);
            booms.animationTick(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            back1.Draw(_spriteBatch);
            paddle.Draw(_spriteBatch);
            ball.Draw(_spriteBatch);
            booms.Draw(_spriteBatch);

            // handle drawing blocks
            if (gameState == 0 || gameState == 1) sl.Draw(_spriteBatch);

            // handle show bb rendering
            if (showbb)
            {
                paddle.drawBB(_spriteBatch, Color.Black);
                paddle.drawHS(_spriteBatch, Color.Green);
                ball.drawInfo(_spriteBatch, Color.Gray, Color.Green);
                sl.drawInfo(_spriteBatch, Color.Brown, Color.Aqua);
                LineBatch.drawLineRectangle(_spriteBatch, playArea, Color.Blue);

                // handle hitstate debugging
                if (hitState.hit) _spriteBatch.DrawString(fonty, "Hit", new Vector2(630, 60), Color.DarkGoldenrod);
                if (hitState.hitTop) _spriteBatch.DrawString(fonty, "HitTop", new Vector2(630, 80), Color.DarkGoldenrod);
                if (hitState.hitBottom) _spriteBatch.DrawString(fonty, "HitBottom", new Vector2(630, 100), Color.DarkGoldenrod);
                if (hitState.hitLeft) _spriteBatch.DrawString(fonty, "HitLeft", new Vector2(630, 120), Color.DarkGoldenrod);
                if (hitState.hitRight) _spriteBatch.DrawString(fonty, "HitRight", new Vector2(630, 140), Color.DarkGoldenrod);
                if (paddleCollisionEnabled) _spriteBatch.DrawString(fonty, "PADDLE COLLISION ON", new Vector2(630, 160), Color.DarkGoldenrod);
                _spriteBatch.DrawString(fonty, "Ball speed X: " + speed.X.ToString("n2"), new Vector2(10, 300), Color.Purple);
                _spriteBatch.DrawString(fonty, "Ball speed Y: " + speed.Y.ToString("n2"), new Vector2(10, 320), Color.Purple);
                _spriteBatch.DrawString(fonty, "Paddle speed: " + paddlespeed.ToString("n2"), new Vector2(10, 340), Color.Purple);
                _spriteBatch.DrawString(fonty, "ticks: " + ticks + "  ticker: " + ticker, new Vector2(10, 140), Color.Purple);
                _spriteBatch.DrawString(fonty, "gameState: " + gameState, new Vector2(10, 120), Color.Purple);
            }

            if (gameState == 4) levelComp.Draw(_spriteBatch);
            if (gameState == 5) levelFail.Draw(_spriteBatch);

            _spriteBatch.DrawString(fonty, "hits: " + hits, new Vector2(10, 80), Color.Purple);
            _spriteBatch.DrawString(fonty, "score: " + score, new Vector2(10, 100), Color.Purple);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public static Texture2D textFromFile(GraphicsDevice gd, string fName) // rob's background method
        {
            // note needs :using System.IO;
            Stream fs = new FileStream(fName, FileMode.Open);
            Texture2D rc = Texture2D.FromStream(gd, fs);
            fs.Close();
            return rc;
        } 
        public HitState checkHit(Rectangle rect1, Rectangle rect2) //check hitstate
        {
            HitState retv;
            retv.hit = false;
            retv.hitTop = false;
            retv.hitBottom = false;
            retv.hitLeft = false;
            retv.hitRight = false;
            retv.hitInside = false;
            retv.hitExact = false;
        
            Rectangle temp1 = Rectangle.Intersect(rect1, rect2);
        
            if (temp1.Width == 0 && temp1.Height == 0 )
            {
                return retv;
            }

            retv.hit = true;
            if (temp1.Y == rect1.Y) retv.hitTop = true;
            if (temp1.Y + temp1.Height == rect1.Y + rect1.Height) retv.hitBottom = true;
            if (temp1.X == rect1.X) retv.hitLeft = true;
            if (temp1.X + temp1.Width == rect1.X + rect1.Width) retv.hitRight = true;

            return retv;
        } 
        public void ResetGame()
        {
            gameState = 3;
            sl = null;
            score = 0;
            hits = 0;
            paddle.setPos(xx,yy);
            ball.setPos(xx + 35, yy - 10);
            ballStuck = true;
            populateBlocks();
        }
        public void populateBlocks()
        {
            sl = new SpriteList();
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    Sprite3 s = new Sprite3(true, texBlock1, x * 68 + playArea.X + blockOffsetX, y * 36 + playArea.Y + blockOffsetY);
                    switch (y)
                    {
                        case 0:
                            s.hitPoints = 3;
                            s.setColor(Color.LightSkyBlue);
                            break;
                        case 1:
                            s.hitPoints = 3;
                            s.setColor(Color.LightSkyBlue);
                            break;
                        case 2:
                            s.hitPoints = 2;
                            s.setColor(Color.LightPink);
                            break;
                        case 3:
                            s.hitPoints = 2;
                            s.setColor(Color.LightPink);
                            break;
                        case 4:
                            s.hitPoints = 1;
                            s.setColor(Color.GhostWhite);
                            break;
                    }
                    sl.addSpriteReuse(s);
                }
            }
        }
        void createExplosion(int x, int y)
        {
            float scale = 0.6f;
            int xoffset = -2;
            int yoffset = -20;

            Sprite3 s = new Sprite3(true, texBoom, x + xoffset, y + yoffset);
            s.setXframes(7);
            s.setYframes(3);
            s.setWidthHeight(896 / 7 * scale, 384 / 3 * scale);

            Vector2[] anim = new Vector2[21];
            anim[0].X = 0; anim[0].Y = 0;
            anim[1].X = 1; anim[1].Y = 0;
            anim[2].X = 2; anim[2].Y = 0;
            anim[3].X = 3; anim[3].Y = 0;
            anim[4].X = 4; anim[4].Y = 0;
            anim[5].X = 5; anim[5].Y = 0;
            anim[6].X = 6; anim[6].Y = 0;
            anim[7].X = 0; anim[7].Y = 1;
            anim[8].X = 1; anim[8].Y = 1;
            anim[9].X = 2; anim[9].Y = 1;
            anim[10].X = 3; anim[10].Y = 1;
            anim[11].X = 4; anim[11].Y = 1;
            anim[12].X = 5; anim[12].Y = 1;
            anim[13].X = 6; anim[13].Y = 1;
            anim[14].X = 0; anim[14].Y = 2;
            anim[15].X = 1; anim[15].Y = 2;
            anim[16].X = 2; anim[16].Y = 2;
            anim[17].X = 3; anim[17].Y = 2;
            anim[18].X = 4; anim[18].Y = 2;
            anim[19].X = 5; anim[19].Y = 2;
            anim[20].X = 6; anim[20].Y = 2;
            s.setAnimationSequence(anim, 0, 20, 4);
            s.setAnimFinished(2); // make it inactive and invisible
            s.animationStart();

            booms.addSpriteReuse(s); // add the sprite

        }

    }
    public struct HitState
    {
        public bool hit; // true if hit
        public bool hitTop; // true if hit top
        public bool hitBottom; // true if hit bottom 
        public bool hitLeft; // true if hit left
        public bool hitRight; // true if hit right
        public bool hitInside; // true if one inside other
        public bool hitExact; // only true if both bounding boxes exact size
    }
}

// healthbar and text in rc framework
// modify framework to make a purpose built timedText from fadeText