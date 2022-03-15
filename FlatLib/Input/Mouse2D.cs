using System;
using FlatLib.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FlatLib.Input
{
    public sealed class Mouse2D
    {
        private static readonly Lazy<Mouse2D> instance = new Lazy<Mouse2D>(() => new Mouse2D());

        public static Mouse2D Instance => instance.Value;

        private MouseState prevMouseState;
        private MouseState currMouseState;

        public Point WindowPosition
        {
            get => this.currMouseState.Position;
        }

        public Mouse2D()
        {
            this.currMouseState = Mouse.GetState();
            this.prevMouseState = currMouseState;
        }

        public void Update()
        {
            this.prevMouseState = this.currMouseState;
            this.currMouseState = Mouse.GetState();
        }

        public bool IsLeftButtonDown()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsRightButtonDown()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMiddleButtonDown()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsLeftButtonClicked()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed && this.prevMouseState.LeftButton == ButtonState.Released;
        }

        public bool IsRightButtonClicked()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed && this.prevMouseState.RightButton == ButtonState.Released;
        }

        public bool IsMiddleButtonClicked()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed && this.prevMouseState.MiddleButton == ButtonState.Released;
        }

        public Vector2 GetScreenPosition(Screen screen)
        {
            Rectangle screenDestination = screen.CalculateDestinationRectangle();

            Point windowPosition = this.WindowPosition;

            float sx = windowPosition.X - screenDestination.X;
            float sy = windowPosition.Y - screenDestination.Y;

            sx /= (float)screenDestination.Width;
            sy /= (float)screenDestination.Height;

            sx *= (float)screen.Width;
            sy *= (float)screen.Height;

            sy = (float)screen.Height - sy;

            return new Vector2(sx, sy);
        }
    }
}
