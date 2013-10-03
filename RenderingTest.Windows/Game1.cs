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

        protected override void Initialize()
        {
            //var device = graphics.GraphicsDevice;
            //device.BlendState = BlendState.AlphaBlend;
            //device.DepthStencilState = DepthStencilState.DepthRead;
            //device.SamplerStates[0] = SamplerState.LinearWrap;
            //device.SamplerStates[1] = SamplerState.LinearWrap;

            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            rasterizerState.FillMode = FillMode.Solid;

            GraphicsDevice.RasterizerState = rasterizerState;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            basement = Content.Load<Model>("Box");
            camera = new FreeCamera();
            camera.SetView(new Vector3(-1.2f, 1.4f, 1.2f),
                                new Vector3(-1.8f, 1.4f, 1.2f), Vector3.Up);

            camera.SetPespective(MathHelper.PiOver4,
                                (float)GraphicsDevice.Viewport.Width,
                                (float)GraphicsDevice.Viewport.Height,
                                0.01f, 10000.0f);

            inputManager = new InputComponentManager();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

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

            if (input.GetThumbStickAmount(Trigger.Left).Y > thumbstickThreshold || input.IsPressKey(Keys.W))
            {
                camera.MoveForward(freeCameraSpeedAmount);
            }

            if (input.GetThumbStickAmount(Trigger.Left).Y < -thumbstickThreshold || input.IsPressKey(Keys.S))
            {
                camera.MoveForward(-freeCameraSpeedAmount);
            }

            if (input.GetThumbStickAmount(Trigger.Left).X < -thumbstickThreshold || input.IsPressKey(Keys.A))
            {
                camera.MoveSide(-freeCameraSpeedAmount);
            }

            if (input.GetThumbStickAmount(Trigger.Left).X > thumbstickThreshold || input.IsPressKey(Keys.D))
            {
                camera.MoveSide(freeCameraSpeedAmount);
            }

            if (input.IsPressControlPad(ControlPad.LeftShoulder) || input.IsPressKey(Keys.Y))
                camera.MoveUp(-freeCameraSpeedAmount);

            if (input.IsPressControlPad(ControlPad.RightShoulder) || input.IsPressKey(Keys.U))
                camera.MoveUp(freeCameraSpeedAmount);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            var yaw = 0; // time * 0.4f;
            var pitch = 0; //time * 0.7f;
            var roll = 0; //time * 1.1f;

            var cameraPosition = new Vector3(0, 0, 5f);

            var aspect = GraphicsDevice.Viewport.AspectRatio;

            var world = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            var view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 0.01f, 10000f);

            var boneTransforms = new Matrix[basement.Bones.Count];
            basement.CopyAbsoluteBoneTransformsTo(boneTransforms);

            //var world = Matrix.CreateTranslation(new Vector3(0, 0, 0));

            foreach (ModelMesh mesh in basement.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    //effect.LightingEnabled = true;
                    //effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0, 0);
                    //effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);
                    //effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0);
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
            //GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }
    }
}
