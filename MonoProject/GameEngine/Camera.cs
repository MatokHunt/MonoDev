namespace MonoProject.GameEngine;

internal class Camera1
{
    public float Zoom { get; set; }
    public Vector2 Position { get; set; }
    public Rectangle Bounds { get; protected set; }
    public Rectangle VisibleArea { get; protected set; }
    public Matrix Transform { get; protected set; }

    public Camera1(Viewport viewport)
    {
        Bounds = viewport.Bounds;
        Zoom = 1f;
        Position = Vector2.Zero;
    }

    private void UpdateVisibleArea()
    {
        var inverseViewMatrix = Matrix.Invert(Transform);

        var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
        var tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
        var bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
        var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

        var min = new Vector2(
            MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
            MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
        var max = new Vector2(
            MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
            MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
        VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
    }

    private void UpdateMatrix()
    {
        Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                Matrix.CreateScale(Zoom) *
                Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));
        UpdateVisibleArea();
    }

    public void MoveCamera(Vector2 movePosition)
    {
        Vector2 newPosition = Position + movePosition;
        Position = newPosition;
    }

    public void AdjustZoom(float zoomAmount)
    {
        Zoom += zoomAmount;
        if (Zoom < .50f)
        {
            Zoom = .50f;
        }
        if (Zoom > 2f)
        {
            Zoom = 2f;
        }
    }

    public void UpdateCamera(Viewport bounds, Vector2 panDirection, double delta)
    {
        Bounds = bounds.Bounds;
        UpdateMatrix();
        int moveSpeed;

        if (Zoom == .5f)
        {
            moveSpeed = 640;
        }
        else
        {
            moveSpeed = 480;
        }
        panDirection = panDirection * moveSpeed * (float)delta;

        MoveCamera(panDirection);
    }
}
