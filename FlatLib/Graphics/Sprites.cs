using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatLib.Graphics
{
    public sealed class Sprites : IDisposable
    {
        private bool isDisposed;
        private Game game;
        private SpriteBatch sprites;
        private BasicEffect effect;

        public Sprites(Game game)
        {
            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.isDisposed = false;
            this.sprites = new SpriteBatch(this.game.GraphicsDevice);
            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.TextureEnabled = true;
            this.effect.LightingEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.World = Matrix.Identity;
            this.effect.Projection = Matrix.Identity;
            this.effect.View = Matrix.Identity;
        }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }
            this.effect?.Dispose();
            this.sprites?.Dispose();
            this.isDisposed = true;
        }

        public void Begin(Camera camera, bool isTextureFilteringEnabled)
        {
            SamplerState samplerState = SamplerState.PointClamp;
            if (isTextureFilteringEnabled)
            {
                samplerState = SamplerState.PointWrap;
            }

            if (camera == null)
            {
                Viewport viewport = this.game.GraphicsDevice.Viewport;
                this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, 0, viewport.Height, 0, 1f);
                this.effect.View = Matrix.Identity;
            }
            else
            {
                camera.UpdateMatricies();
                this.effect.View = camera.View;
                this.effect.Projection = camera.Projection;
            }
            
            this.sprites.Begin(blendState: BlendState.AlphaBlend, samplerState: samplerState, rasterizerState: RasterizerState.CullNone, effect: this.effect);
        }

        public void End()
        {
            this.sprites.End();
        }

        public void Draw(Texture2D texture, Vector2 origin, Vector2 position, Color color)
        {
            this.sprites.Draw(texture, position, null, color, 0f, origin, 1f, SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Texture2D texture, Rectangle? source, Vector2 origin, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            this.sprites.Draw(texture, position, source, color, rotation, origin, scale, SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Texture2D texture, Rectangle? source, Rectangle destination, Color color)
        {
            this.sprites.Draw(texture, destination, source, color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
        }
    }
}
