using System;
using Microsoft.Xna.Framework;

namespace FlatLib
{
    public struct Transform2D
    {
        public float PosX;
        public float PosY;
        public float CosScaleX;
        public float CosScaleY;
        public float SinScaleX;
        public float SinScaleY;

        public Transform2D(Vector2 position, float angle, Vector2 scale)
        {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            this.PosX = position.X;
            this.PosY = position.Y;
            this.CosScaleX = cos * scale.X;
            this.SinScaleX = sin * scale.X;
            this.CosScaleY = cos * scale.Y;
            this.SinScaleY = sin * scale.Y;
        }

        public Transform2D(Vector2 position, float angle, float scale)
        {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            this.PosX = position.X;
            this.PosY = position.Y;
            this.CosScaleX = cos * scale;
            this.SinScaleX = sin * scale;
            this.CosScaleY = cos * scale;
            this.SinScaleY = sin * scale;
        }

        public Matrix ToMatrix()
        {
            Matrix result = Matrix.Identity;
            result.M11 = this.CosScaleX;
            result.M12 = this.SinScaleY;
            result.M21 = -this.SinScaleX;
            result.M22 = this.CosScaleY;
            result.M41 = this.PosX;
            result.M42 = this.PosY;

            return result;
        }
    }
}
