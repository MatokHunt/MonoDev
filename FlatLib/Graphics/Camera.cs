using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatLib.Graphics
{
    public sealed class Camera
    {
        public readonly static float MinZ = 1f;
        public readonly static float MaxZ = 2048f;

        public readonly static int MinZoom = 1;
        public readonly static int MaxZoom = 3;

        private Vector2 position;
        private float z;
        private float baseZ;

        private float aspectRatio;
        private float fieldOfView;

        private Matrix view;
        private Matrix projection;

        public int zoom;

        public Vector2 Position
        {
            get { return this.position; }
        }

        public float Z
        {
            get { return this.z; }
        }

        public float BaseZ
        {
            get { return this.baseZ; }
        }

        public Matrix View
        {
            get { return this.view; }
        }

        public Matrix Projection
        {
            get { return this.projection; }
        }

        public Camera(Screen screen)
        {
            if (screen == null)
            {
                throw new ArgumentNullException("screen");
            }
            this.aspectRatio = (float)screen.Width / screen.Height;
            this.fieldOfView = MathHelper.PiOver2;

            this.position = new Vector2(0, 0);
            this.baseZ = this.GetZFromHeight(screen.Height);
            this.z = this.baseZ;

            this.UpdateMatricies();

            this.zoom = 1;
        }

        public void UpdateMatricies()
        {
            this.view = Matrix.CreateLookAt(new Vector3(0, 0, this.z), Vector3.Zero, Vector3.Up);
            this.projection = Matrix.CreatePerspectiveFieldOfView(this.fieldOfView, this.aspectRatio, Camera.MinZ, Camera.MaxZ);
        }

        public float GetZFromHeight(float height)
        {
            return (0.5f * height) / (float)Math.Tan(0.5f * this.fieldOfView);
        }

        public float GetHeightFromZ()
        {
            return this.z * (float)Math.Tan(0.5f * this.fieldOfView) * 2f;
        }

        public void MoveZ(float amount)
        {
            this.z += amount;
            this.z = Util.Clamp(this.z, MinZ, MaxZ);
        }

        public void ResetZ()
        {
            this.z = this.baseZ;
        }

        public void Move(Vector2 amount)
        {
            amount.Floor();
            this.position += amount;
        }

        public void MoveTo(Vector2 position)
        {
            position.Floor();
            this.position = position;
        }

        public void IncrementZoom()
        {
            this.zoom++;
            this.zoom = Util.Clamp(this.zoom, MinZoom, MaxZoom);
            this.z = this.baseZ / this.zoom;
        }

        public void DecrementZoom()
        {
            this.zoom--;
            this.zoom = Util.Clamp(this.zoom, MinZoom, MaxZoom);
            this.z = this.baseZ / this.zoom;
        }

        public void SetZoom(int amount)
        {
            this.zoom = amount;
            this.zoom = Util.Clamp(this.zoom, MinZoom, MaxZoom);
            this.z = this.baseZ / this.zoom;
        }

        public void GetExtents(out float width, out float height)
        {
            height = this.GetHeightFromZ();
            width = height * this.aspectRatio;
        }

        public void GetExtents(out float left, out float right, out float bottom, out float top)
        {
            this.GetExtents(out float width, out float height);
            left = this.position.X - width * 0.5f;
            right = left + width;
            bottom = this.position.Y - height * 0.5f;
            top = bottom + height;
        }

        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            this.GetExtents(out float left, out float right, out float bottom, out float top);
            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }

        public Rectangle GetViewRectangle()
        {
            this.GetExtents(out Vector2 min, out Vector2 max);
            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}
