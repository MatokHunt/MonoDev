namespace MonoProject.GameEngine;

public class Engine : Game
{
    private GraphicsDeviceManager graphics;
    private Screen screen;
    private Sprites sprites;
    private Shapes shapes;
    private Camera camera;

    private Vector2 cameraDragOrigin;
    private Vector2 mouseDragOrigin;

    private double frameRate = 1 / 60;
    private double updateRate = 1 / 60;
    private Stopwatch calcTime = new Stopwatch();
    private Stopwatch drawTime = new Stopwatch();

    private Texture2D tileAtlas;
    private Texture2D[] tiles;
    
    private SpriteFont font;
    private World world = new World();

    private Vector2[] vertices;
    private int[] triangleIndices;

    public Engine()
    {
        this.graphics = new GraphicsDeviceManager(this);
        this.graphics.SynchronizeWithVerticalRetrace = true;
        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;
        this.IsFixedTimeStep = true;
    }

    protected override void Initialize()
    {
        //Util.ToggleFullScreen(this.graphics);
        DisplayMode dm = this.GraphicsDevice.DisplayMode;
        this.graphics.PreferredBackBufferWidth = (int)(dm.Width * 0.8f);
        this.graphics.PreferredBackBufferHeight = (int)(dm.Height * 0.8f);
        this.graphics.ApplyChanges();

        this.screen = new Screen(this, this.graphics, 540);
        this.sprites = new Sprites(this);
        this.shapes = new Shapes(this);
        this.camera = new Camera(this.screen);

        Random random = new Random();

        vertices = new Vector2[5];
        vertices[0] = new Vector2(0, 10);
        vertices[1] = new Vector2(10, -10);
        vertices[2] = new Vector2(3, -6);
        vertices[3] = new Vector2(-3, -6);
        vertices[4] = new Vector2(-10, -10);

        int triangleCount = this.vertices.Length - 2;

        this.triangleIndices = new int[triangleCount * 3];
        this.triangleIndices[0] = 0;
        this.triangleIndices[1] = 1;
        this.triangleIndices[2] = 2;
        this.triangleIndices[3] = 0;
        this.triangleIndices[4] = 2;
        this.triangleIndices[5] = 3;
        this.triangleIndices[6] = 0;
        this.triangleIndices[7] = 3;
        this.triangleIndices[8] = 4;

        base.Initialize(); 
    }

    protected override void LoadContent()
    {
        var fileStream = new FileStream("Content\\Textures\\32x32IsometricDebug.png", FileMode.Open);
        this.tileAtlas = Texture2D.FromStream(GraphicsDevice, fileStream);
        fileStream.Dispose();
        this.tiles = new Texture2D[4];
        this.tiles[1] = new Texture2D(GraphicsDevice, 32, 32);
        this.tiles[2] = new Texture2D(GraphicsDevice, 32, 32);
        this.tiles[3] = new Texture2D(GraphicsDevice, 32, 32);
        Color[] data = new Color[32 * 32];
        this.tileAtlas.GetData(0, new Rectangle(0, 0, 32, 32), data, 0, data.Length);
        this.tiles[1].SetData(data);
        this.tileAtlas.GetData(0, new Rectangle(32, 0, 32, 32), data, 0, data.Length);
        this.tiles[2].SetData(data);

        this.font = Content.Load<SpriteFont>("Font\\Font");

        this.world.LoadChunk(new Point(0, 0));
        /*
        this.world.LoadChunk(new Point(-1, 1));
        this.world.LoadChunk(new Point(1, -1));
        this.world.LoadChunk(new Point(1, 0));
        this.world.LoadChunk(new Point(0, 1));
        this.world.LoadChunk(new Point(1, 1));
        this.world.LoadChunk(new Point(0, 2));
        this.world.LoadChunk(new Point(2, 0));
        this.world.LoadChunk(new Point(2, 1));
        this.world.LoadChunk(new Point(1, 2));
        this.world.LoadChunk(new Point(2, 2));
        */
        Random random = new Random();
        foreach (var chunk in world.LoadedChunks)
        {
            for (byte x = 0; x < chunk.Cell.GetLength(0); x++)
            {
                for (byte y = 0; y < chunk.Cell.GetLength(1); y++)
                {
                    
                    for(byte z = 0; z < chunk.Cell.GetLength(2); z++)
                    {
                        chunk.Cell[x, y, z] = 1;
                    }
                    
                    /*
                    chunk.Cell[x, y, 0] = 1;
                    chunk.Cell[x, y, 1] = (ushort)random.Next(0, 3);
                    if (chunk.Cell[x, y, 1] == 1)
                    {
                        chunk.Cell[x, y, 2] = (ushort)random.Next(0, 3);
                    }
                    */
                }
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        this.calcTime.Restart();
        updateRate = 1 / gameTime.ElapsedGameTime.TotalSeconds;
        Keyboard2D keyboard = Keyboard2D.Instance;
        keyboard.Update();

        Mouse2D mouse = Mouse2D.Instance;
        mouse.Update();

        if (keyboard.IsKeyPressed(Keys.Escape))
        {
            Exit();
        }

        if (keyboard.IsKeyPressed(Keys.OemTilde))
        {
            this.camera.GetExtents(out Vector2 min, out Vector2 max);
            Console.WriteLine("Cam Min: " + min);
            Console.WriteLine("Cam Max: " + max);
        }

        if (keyboard.IsKeyPressed(Keys.F2))
        {
            Util.ToggleFullScreen(this.graphics);
        }

        if (mouse.IsScrollUp())
        {
            this.camera.IncrementZoom();
        }
        if (mouse.IsScrollDown())
        {
            this.camera.DecrementZoom();
        }

        Vector2 amount = new Vector2(0, 0);
        float speed = 3f;

        if (keyboard.IsKeyDown(Keys.A))
        {
            amount.X = -1;
        }
        if (keyboard.IsKeyDown(Keys.D))
        {
            amount.X = 1;
        }
        if (keyboard.IsKeyDown(Keys.W))
        {
            amount.Y = 1;
        }
        if (keyboard.IsKeyDown(Keys.S))
        {
            amount.Y = -1;
        }
        if (amount != Vector2.Zero)
        {
            amount.Normalize();
            this.camera.Move(amount * speed);
        }

        if (!mouse.IsLeftButtonDown())
        {
            cameraDragOrigin = this.camera.Position; 
            mouseDragOrigin = mouse.GetScreenPosition(this.screen);
        }
        if (mouse.IsLeftButtonDown())
        {
            camera.MoveTo(cameraDragOrigin  + ((mouse.GetScreenPosition(this.screen) - mouseDragOrigin)) / this.camera.zoom);
        }
   
        base.Update(gameTime);
        this.calcTime.Stop();
    }

    protected override void Draw(GameTime gameTime)
    {
        float prevDrawTime = (float)this.drawTime.Elapsed.TotalMilliseconds;
        this.drawTime.Restart();
        this.frameRate += (1 / gameTime.ElapsedGameTime.TotalSeconds - this.frameRate) * 0.1;
        this.screen.Set();
        this.GraphicsDevice.Clear(Color.Black);

        Rectangle area = Rectangle.Empty;

        this.sprites.Begin(camera, false);
        //this.sprites.Draw(tiles[1], null, new Vector2(16, 16), new Vector2(camera.Position.X, camera.Position.Y), 0f, new Vector2(2f, 2f), Color.Green);
        foreach (var chunk in world.LoadedChunks)
        {
            area = chunk.Area;
            chunk.Draw(this.sprites, tiles, camera.Position);
        }
        this.sprites.End();

        this.shapes.Begin(camera);
        //Transform2D transform = new Transform2D(new Vector2(0f, 100f), this.angle, 2f);
        //this.shapes.DrawPolygonFill(this.vertices, this.triangleIndices, transform, Color.LightGreen);
        //this.shapes.DrawCircleFill(-32, -32, 64, 64, Color.White);
        this.shapes.DrawLine(0, -5, 0, 5, 1f, Color.White);
        this.shapes.DrawLine(-5, 0, 5, 0, 1f, Color.White);
        area.X += (int)camera.Position.X;
        area.Y += (int)camera.Position.Y;
        this.shapes.DrawRectangle(area, 1f, Color.White);
        this.shapes.End();
        
        this.sprites.Begin(null, true);
#if DEBUG
        this.sprites.DrawString(font, String.Format("UPS: {0}", this.updateRate.ToString("0.00")), new Vector2(0, 0), Color.CornflowerBlue);
        this.sprites.DrawString(font, String.Format("FPS: {0}", this.frameRate.ToString("0.00")), new Vector2(0, 20), Color.CornflowerBlue);
        this.sprites.DrawString(font, String.Format("CAM: {0}", this.camera.Position.ToString()), new Vector2(0, 40), Color.CornflowerBlue);
        Mouse2D mouse = Mouse2D.Instance;
        mouse.Update();
        this.sprites.DrawString(font, String.Format("MOUSE: {0}", mouse.GetScreenPosition(this.screen).ToString()), new Vector2(0, 60), Color.CornflowerBlue);
        this.sprites.DrawString(font, String.Format("CALC: {0}ms", this.calcTime.Elapsed.TotalMilliseconds.ToString("0.00")), new Vector2(0, 80), Color.CornflowerBlue);
        this.sprites.DrawString(font, String.Format("DRAW: {0}ms", prevDrawTime.ToString("0.00")), new Vector2(0, 100), Color.CornflowerBlue);
        this.sprites.DrawString(font, String.Format("AREA: {0}", area.ToString()), new Vector2(0, 120), Color.CornflowerBlue);
#endif
        this.sprites.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);
       
        base.Draw(gameTime);
        this.drawTime.Stop();
    }
}
