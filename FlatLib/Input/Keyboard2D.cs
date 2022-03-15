using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FlatLib.Input
{
    public sealed class Keyboard2D
    {
        private static readonly Lazy<Keyboard2D> instance = new Lazy<Keyboard2D>(() =>new Keyboard2D());

        public static Keyboard2D Instance => instance.Value;

        private KeyboardState prevKeyboardState;
        private KeyboardState currKeyboardState;

        public Keyboard2D()
        {
            this.currKeyboardState = Keyboard.GetState();
            this.prevKeyboardState = currKeyboardState;
        }

        public void Update()
        {
            this.prevKeyboardState = this.currKeyboardState;
            this.currKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return this.currKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return this.currKeyboardState.IsKeyDown(key) && !this.prevKeyboardState.IsKeyDown(key);
        }
    }
}
