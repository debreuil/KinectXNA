using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DDW.Kinect.Device;

namespace KinectBasic
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class KinectGame : Microsoft.Xna.Framework.Game
    {
        private static KinectGame inst;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KinectDevice device;

        Texture2D circle;
        SpriteFont font;
        float circleScale = .2f;
        Color curColor = Color.White;

        private KinectGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public static KinectGame GetInstance()
        {
            if (inst == null)
            {
                inst = new KinectGame();
            }
            return inst;
        }

        protected override void Initialize()
        {
            base.Initialize();
            device = new KinectDevice(this.GraphicsDevice.Viewport.Bounds);
            device.Init();
            device.Start();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            circle = this.Content.Load<Texture2D>("circ");
            font = this.Content.Load<SpriteFont>("Segoe");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (device.state == SessionState.Initialized)
            {
                spriteBatch.DrawString(font, "Wave to Begin", new Vector2(30, 30), Color.White);
            }
            else
            {
                int offset = (int)(circleScale * circle.Width / 2);
                Rectangle r = new Rectangle(
                    device.currentX - offset,
                    device.currentY - offset,
                    offset * 2,
                    offset * 2);

                spriteBatch.Draw(circle, r, curColor);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (device != null)
            {
                device.Close();
                device = null;
            }
        }
    }
}
