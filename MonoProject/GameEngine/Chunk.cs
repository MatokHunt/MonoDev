namespace MonoProject.GameEngine;

internal class Chunk
{
    public ushort[,,] Cell { get; set; }
    public Point WorldCoordinates { get; private set; }
    public Rectangle Area
    { 
        get
        {
            var x = (WorldCoordinates.X) * (16 * 4 * Cell.GetLength(0)) - (WorldCoordinates.Y) * (16 * 4 * Cell.GetLength(1)) - ((256 - 16) * 4);
            var y = (WorldCoordinates.Y) * (16 * 2 * Cell.GetLength(1)) - 1024 + ((WorldCoordinates.X * (256)) * 2);
            return new Rectangle(x, y, 512 * 4, 512 * 4);
        } 
    }
    
    public Chunk(Point coords)
    {
        Cell = new ushort[16, 16, 16];
        WorldCoordinates = coords;
    }

    public bool Exposed(ushort x, ushort y, ushort level)
    {
        var isExposed = false;
        if (level + 1 == Cell.GetLength(2) || Cell[x, y, level + 1] == 0)
        {
            isExposed = true;
            return isExposed;
        }
        if (x + 1 == Cell.GetLength(0) || Cell[x + 1, y, level] == 0 || Cell[x + 1, y, level] > 1)
        {
            isExposed = true;
            return isExposed;
        }
        if (y + 1 == Cell.GetLength(1) || Cell[x, y + 1, level] == 0 || Cell[x, y + 1, level] > 1)
        {
            isExposed = true;
        }
        return isExposed;
    }
}
