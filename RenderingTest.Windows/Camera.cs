#region File Description
//-----------------------------------------------------------------------------
// CameraBase.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;
#endregion

namespace BullshitTest
{
    /// <summary>
    /// This is the base class of every camera.
    /// It can configure View matrix and Projection matrix.
    /// </summary>
    public class Camera
    {
        #region Fields

        /// <summary>
        /// Field of View
        /// </summary>
        float fov = MathHelper.PiOver4;

        float screenWidth = 0;
        float screenHeight = 0;
        float aspectRatio = 0;

        /// <summary>
        /// The Camera's near distance
        /// </summary>
        float near = 1.0f;

        /// <summary>
        /// The Camera's far distance
        /// </summary>
        float far = 1000.0f;

        /// <summary>
        /// The Camera position
        /// </summary>
        protected Vector3 position = Vector3.Zero;

        /// <summary>
        /// The Camera old position
        /// </summary>
        Vector3 oldPosition = Vector3.Zero;

        /// <summary>
        /// The Camera up vector
        /// </summary>
        protected Vector3 up = Vector3.Up;

        /// <summary>
        /// The Camera right vector
        /// </summary>
        Vector3 right = Vector3.Right;

        /// <summary>
        /// The Camera direction
        /// </summary>
        Vector3 direction = Vector3.Forward;

        /// <summary>
        /// The camera looks at the target position.
        /// </summary>
        protected Vector3 target = Vector3.Forward;

        /// <summary>
        /// The Camera velocity
        /// </summary>
        Vector3 velocity = Vector3.Zero;

        /// <summary>
        /// The projection matrix
        /// </summary>
        Matrix projection = Matrix.Identity;

        /// <summary>
        /// The view matrix
        /// </summary>
        Matrix view = Matrix.Identity;

        /// <summary>
        /// The bounding frustum representing the current camera view.
        /// </summary>
        BoundingFrustum boundingFrustrum;

        #endregion

        #region Properties

        public Matrix Projection
        {
            get { return projection; }
            protected set { projection = value; }
        }

        public Matrix View
        {
            get { return view; }
            protected set { view = value; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 Direction
        {
            get { return direction; }
        }

        public Vector3 Target
        {
            get { return target; }
        }

        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector3 Up
        {
            get { return up; }
        }

        public Vector3 Right
        {
            get { return right; }
        }

        public float AspectRatio
        {
            get { return aspectRatio; }
        }

        public float FieldOfView
        {
            get { return fov; }
        }

        public float Near
        {
            get { return near; }
        }

        public float Far
        {
            get { return far; }
        }

        public float Width
        {
            get { return screenWidth; }
        }

        public float Height
        {
            get { return screenHeight; }
        }

        public BoundingFrustum BoundingFrustrum
        {
            get { return this.boundingFrustrum; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Camera()
            : base()
        {
            near = 1.0f;
            far = 1000.0f;
            position = oldPosition = Vector3.Zero;
            up = Vector3.Up;
            right = Vector3.Right;
            direction = Vector3.Forward;
            target = Vector3.Forward;
            velocity = Vector3.Zero;
            projection = Matrix.Identity;
            view = Matrix.Identity;
            boundingFrustrum = new BoundingFrustum(view * projection);
        }

        /// <summary>
        /// Update a velocity
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            if (oldPosition == this.Position)
            {
                this.velocity = Vector3.Zero;
            }
            else
            {
                this.velocity = this.position - oldPosition;

                oldPosition = this.position;
            }

            this.boundingFrustrum.Matrix = view * projection;
        }

        /// <summary>
        /// Set the view matrix of the camera
        /// </summary>
        public Matrix SetView(Vector3 position, Vector3 target, Vector3 up)
        {
            //  Make a camera's direction
            this.direction = target - position;
            this.direction.Normalize();

            this.position = position;
            this.target = target;
            this.up = up;

            //  Make a camera's right vector
            this.right = Vector3.Cross(direction, up);
            this.right.Normalize();

            //  Make a camera's view matrix
            View = Matrix.CreateLookAt(this.Position, this.target, this.Up);

            return View;
        }

        /// <summary>
        /// Set the projection matrix of the camera
        /// </summary>
        public Matrix SetPespective(float fov, float width, float height,
                                     float near, float far)
        {
            this.screenWidth = width;
            this.screenHeight = height;
            this.fov = fov;
            this.near = near;
            this.far = far;

            aspectRatio = screenWidth / screenHeight;

            //  Make a camera's projection matrix
            projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far);

            return projection;
        }

        /// <summary>
        /// It changes the width and height of screen.
        /// </summary>
        public void Resize(float width, float height)
        {
            SetPespective(this.fov, width, height, this.Near, this.Far);
        }

        /// <summary>
        /// Reset the camera
        /// </summary>
        public void Reset()
        {
            this.position = Vector3.Zero;
            this.up = Vector3.Up;
            this.right = Vector3.Right;
            this.direction = Vector3.Forward;
            this.target = position + direction;

            this.View = Matrix.Identity;
        }
    }
}
