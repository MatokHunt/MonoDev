using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatLib.Graphics
{
    public sealed class Screen :IDisposable
    {
        private readonly static int MinDim = 64;
        private readonly static int MaxDim = 4096;

        private bool isDisposed;
        private Game game;
        private RenderTarget2D target;
        private bool isSet;

        public int Width 
        { 
            get { return this.target.Width; } 
        }

        public int Height
        {
            get { return this.target.Height; }  
        }

        public Screen(Game game, int width, int height)
        {
            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.isDisposed = false;
            width = Util.Clamp(width, MinDim, MaxDim);
            height = Util.Clamp(height, MinDim, MaxDim);
            this.target = new RenderTarget2D(this.game.GraphicsDevice, width, height);
            this.isSet = false;
        }

        public Screen(Game game, float scale)
        {
            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.isDisposed = false;
            var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.target = new RenderTarget2D(this.game.GraphicsDevice, (int)(width * scale), (int)(height * scale));
            this.isSet = false;
        }

        public Screen(Game game, GraphicsDeviceManager graphics, int scaleToHeight)
        {
            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.isDisposed = false;

            float width;
            float height;
            if (graphics.IsFullScreen) 
            {
                width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                width = graphics.PreferredBackBufferWidth;
                height= graphics.PreferredBackBufferHeight;
            }
            
            float aspect = scaleToHeight / height;
            Console.WriteLine(aspect);
            scaleToHeight = Util.Clamp(scaleToHeight, MinDim, MaxDim);
            var scaleToWidth = width * aspect;
            this.target = new RenderTarget2D(this.game.GraphicsDevice, (int)scaleToWidth, scaleToHeight);
            this.isSet = false;
        }

        public void Set()
        {
            if (this.isSet)
            {
                throw new Exception("Render target is already set.");
            }
            this.game.GraphicsDevice.SetRenderTarget(this.target);
            this.isSet = true;
        }

        public void UnSet()
        {
            if (!this.isSet)
            {
                throw new Exception("Render target is not set.");
            }
            this.game.GraphicsDevice.SetRenderTarget(null);
            this.isSet = false;
        }

        public void Present(Sprites sprites, bool textureFiltering = true)
        {
            if (sprites == null)
            {
                throw new ArgumentNullException("sprites");
            }
#if DEBUG
            this.game.GraphicsDevice.Clear(Color.HotPink);
#else
            this.game.GraphicsDevice.Clear(Color.Black);
#endif

            sprites.Begin(null, textureFiltering);
            sprites.Draw(this.target, null, this.CalculateDestinationRectangle(), Color.White);
            sprites.End();
        }

        internal Rectangle CalculateDestinationRectangle()
        {
            Rectangle backBufferBounds = this.game.GraphicsDevice.PresentationParameters.Bounds;
            float backBufferAspectRatio = (float)backBufferBounds.Width / backBufferBounds.Height;
            float screenAspectRatio = (float)this.Width / this.Height;

            float rx = 0f;
            float ry = 0f;
            float rw = backBufferBounds.Width;
            float rh = backBufferBounds.Height;

            if (backBufferAspectRatio > screenAspectRatio)
            {
                rw = rh * screenAspectRatio;
                rx = (float)(backBufferBounds.Width - rw) / 2f;
            }
            else if(backBufferAspectRatio < screenAspectRatio)
            {
                rh = rw * screenAspectRatio;
                ry = (float)(backBufferBounds.Height - rh) / 2f;
            }

            Rectangle result = new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
            return result;
        }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.target?.Dispose();
            this.isDisposed = true;
        }
    }
}
