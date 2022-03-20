namespace MonoProject.GameEngine;

internal class Cursor
{
    Texture2D texture;
    Point isoPosition;
    Vector2 tileOrigin;
    Vector2 tileScale;

    public Cursor(Texture2D texture)
    {
        this.texture = texture;
        this.isoPosition = Point.Zero;
        this.tileOrigin = new Vector2(16, 16);
        this.tileScale = new Vector2(2f, 2f);
    }

    public void MoveTo(Screen screen, Vector2 target)
    {
        this.isoPosition.X = 0;
        this.isoPosition.Y = 0;
    }

    public void Draw(Sprites sprites, Camera camera)
    {
        sprites.Draw(texture, null, this.tileOrigin, new Vector2(isoPosition.X - camera.Position.X, isoPosition.Y - camera.Position.Y), 0f, this.tileScale, Color.White);
    }
}
