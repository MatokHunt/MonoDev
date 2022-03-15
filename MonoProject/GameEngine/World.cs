namespace MonoProject.GameEngine;

internal class World
{
    public List<Chunk> LoadedChunks { get; private set; }

    public World()
    {
        LoadedChunks = new List<Chunk>();
    }

    public void LoadChunk(Point coords)
    {
        LoadedChunks.Add(new Chunk(coords));
    }
}
