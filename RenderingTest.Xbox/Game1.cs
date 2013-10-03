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

namespace BullshitTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Model basement;
        FreeCamera camera;
        InputComponentManager inputManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var device = graphics.GraphicsDevice;
            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.DepthRead;
            device.SamplerStates[0] = SamplerState.LinearWrap;
            device.SamplerStates[1] = SamplerState.LinearWrap;

            var rasterizeState = new RasterizerState();
            rasterizeState.CullMode = CullMode.None;
            rasterizeState.DepthBias = 0;
            rasterizeState.FillMode = FillMode.WireFrame;
            rasterizeState.MultiSampleAntiAlias = true;
            rasterizeState.ScissorTestEnable = false;
            rasterizeState.SlopeScaleDepthBias = 0;

            device.RasterizerState = rasterizeState;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            basement = Content.Load<Model>("RectangleRoom");
            camera = new FreeCamera();
            camera.SetView(new Vector3(-1.2f, 1.4f, 1.2f),
                                new Vector3(-1.8f, 1.4f, 1.2f), Vector3.Up);

            camera.SetPespective(MathHelper.PiOver4,
                                (float)GraphicsDevice.Viewport.Width,
                                (float)GraphicsDevice.Viewport.Height,
                                0.01f, 10000.0f);

            inputManager = new InputComponentManager();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            camera.Update(gameTime);
            inputManager.PreUpdate();
            inputManager.Update(gameTime);
            HandleInput();
            inputManager.PostUpdate();

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            var input = inputManager.GetInputComponent(PlayerIndex.One);
            var thumbstickThreshold = 0.2f;

            var freeCameraSpeedAmount = 2.0f;
            var freeCameraTurnAmount = 100f;

            if (input.GetThumbStickAmount(Trigger.Right).Y > thumbstickThreshold || input.IsPressKey(Keys.Up))
            {
                camera.Rotate(new Vector3(0.0f, freeCameraTurnAmount, 0.0f));
            }
            else if (input.GetThumbStickAmount(Trigger.Right).Y < -thumbstickThreshold || input.IsPressKey(Keys.Down))
            {
                camera.Rotate(new Vector3(0.0f, -freeCameraTurnAmount, 0.0f));
            }

            if (input.GetThumbStickAmount(Trigger.Right).X > thumbstickThreshold || input.IsPressKey(Keys.Right))
            {
                camera.Rotate(new Vector3(-freeCameraTurnAmount, 0.0f, 0.0f));
            }
            else if (input.GetThumbStickAmount(Trigger.Right).X < -thumbstickThreshold || input.IsPressKey(Keys.Left))
            {
                camera.Rotate(new Vector3(freeCameraTurnAmount, 0.0f, 0.0f));
            }

            if (input.GetGamePadTriggers(Trigger.Left) > thumbstickThreshold || input.IsPressKey(Keys.W))
            {
                camera.MoveForward(freeCameraSpeedAmount * 2);
            }

            if (input.GetGamePadTriggers(Trigger.Right) > thumbstickThreshold || input.IsPressKey(Keys.S))
            {
                camera.MoveForward(-freeCameraSpeedAmount * 2);
            }

            if (input.GetThumbStickAmount(Trigger.Left).Y > thumbstickThreshold)
            {
                camera.MoveForward(freeCameraSpeedAmount);
            }

            if (input.GetThumbStickAmount(Trigger.Left).Y < -thumbstickThreshold)
            {
                camera.MoveForward(-freeCameraSpeedAmount);
            }

            if (input.GetThumbStickAmount(Trigger.Left).X < -thumbstickThreshold)
            {
                camera.MoveSide(-freeCameraSpeedAmount);
            }

            if (input.GetThumbStickAmount(Trigger.Left).X > thumbstickThreshold)
            {
                camera.MoveSide(freeCameraSpeedAmount);
            }

            if (input.IsPressControlPad(ControlPad.LeftShoulder) || input.IsPressKey(Keys.Y))
                camera.MoveUp(-freeCameraSpeedAmount);

            if (input.IsPressControlPad(ControlPad.RightShoulder) || input.IsPressKey(Keys.U))
                camera.MoveUp(freeCameraSpeedAmount);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var boneTransforms = new Matrix[basement.Bones.Count];
            basement.CopyAbsoluteBoneTransformsTo(boneTransforms);

            var world = Matrix.CreateTranslation(new Vector3(0, 0, 0));

            foreach (ModelMesh mesh in basement.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
