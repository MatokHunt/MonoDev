namespace MonoProject.GameEngine;

internal class Chunk
{
    private const int tileSize = 32;
    private const int halfTileSize = 16;
    private const int chunkSize = 512;
    private const int halfChunkSize = 256;

    private ushort[,,] cell;
    private Point position;
    private Vector2 tileScale;
    private Vector2 tileOrigin;
    
    public ushort[,,] Cell
    {
        get { return cell; }
    }

    public Point Position
    {
        get { return position; }
    }
    
    public Rectangle Area
    { 
        get
        {
            int ix = -chunkSize + this.position.X * chunkSize - this.position.Y * chunkSize;
            int iy = -halfChunkSize - this.position.X * halfChunkSize - this.position.Y * halfChunkSize - halfChunkSize;
            return new Rectangle(
                ix,
                iy,
                chunkSize * (int)tileScale.X,
                chunkSize * (int)tileScale.Y);
        } 
    }
    
    public Chunk(Point position)
    {
        this.cell = new ushort[16, 16, 16];
        this.position = position;
        this.tileScale = new Vector2(2f, 2f);
        this.tileOrigin = new Vector2(16, 16);
    }

    public void Draw(Sprites sprites, Texture2D[] tiles, Vector2 position)
    {
        for (int height = 0; height < this.cell.GetLength(2); height++)
        {
            for (int x = 0; x < this.cell.GetLength(0); x++)
            {
                for (int y = 0; y < this.cell.GetLength(1); y++)
                {
                    float ix = x * tileSize - y * tileSize;
                    ix += this.position.X * chunkSize - this.position.Y * chunkSize;

                    float iy = -x * halfTileSize - y * halfTileSize + height * tileSize;
                    iy -=this.position.X * halfChunkSize + this.position.Y * halfChunkSize;

                    if (this.cell[x, y, height] > 0 && this.IsExposed(x, y, height))
                    {
                        sprites.Draw(tiles[this.cell[x, y, height]], null, this.tileOrigin, new Vector2(ix - position.X, iy - position.Y), 0f, this.tileScale, Color.Green);
                    }
                }
            }
        }
    }

    public bool IsExposed(int x, int y, int height)
    {
        bool isExposed = false;
        if (height + 1 == this.cell.GetLength(2) || this.cell[x, y, height + 1] == 0)
        {
            isExposed = true;
            return isExposed;
        }
        if (x + 1 == this.cell.GetLength(0) || this.cell[x + 1, y, height] == 0 || this.cell[x + 1, y, height] > 1)
        {
            isExposed = true;
            return isExposed;
        }
        if (y + 1 == this.cell.GetLength(1) || this.cell[x, y + 1, height] == 0 || this.cell[x, y + 1, height] > 1)
        {
            isExposed = true;
        }
        return isExposed;
    }
}
