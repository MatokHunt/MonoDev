using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatLib
{
    public static class Util
    {
        public static void ToggleFullScreen(GraphicsDeviceManager graphics)
        {
            graphics.HardwareModeSwitch = false;
            graphics.ToggleFullScreen();
        }

        public static int Clamp(int value, int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException("The value of \"min\" is greater than the value of \"max\".");
            }

            if (value < min)
            {
                return min;
            }else if(value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException("The value of \"min\" is greater than the value of \"max\".");
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static void Normalize(ref float x, ref float y)
        {
            float invLen = 1f / (float)Math.Sqrt(x * x + y * y);
            x *= invLen;
            y *= invLen;
        }

        public static Vector2 Transform(Vector2 position, Transform2D transform)
        {
            return new Vector2(
                position.X * transform.CosScaleX - position.Y * transform.SinScaleY + transform.PosX,
                position.X * transform.SinScaleX + position.Y * transform.CosScaleY + transform.PosY);
        }
    }
}
