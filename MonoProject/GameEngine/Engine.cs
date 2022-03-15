namespace MonoProject.GameEngine;

public class Engine : Game
{
    private double frameRate = 1 / 60;
    private double updateRate = 1 / 60;
    private Vector2 scale;
    private float tileSize;
    private bool showStats = true;
    private Stopwatch calcTime = new Stopwatch();
    private Stopwatch drawTime = new Stopwatch();

    private Texture2D tileAtlas;
    private Texture2D[] tiles;
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont font;
    //private Camera camera;
    private Vector2 cameraPanDirection = Vector2.Zero;
    private float currentMouseWheelValue, previousMouseWheelValue;
    private KeyboardState previousKeyboardState;
    private MouseState previousMouseState;
    private Vector2 dragOrigin, mouseOrigin;
    private World world = new World();

    private Sprites sprites;
    private Screen screen;
    private Shapes shapes;
    private Camera camera;

    private Vector2[] vertices;
    private float angle = 0f;

    public Engine()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        //camera = new Camera(graphics.GraphicsDevice.Viewport);
        //graphics.SynchronizeWithVerticalRetrace = false;
        //IsFixedTimeStep = false; // Turn off time step locking
        //TargetElapsedTime = TimeSpan.FromMilliseconds(6.9444); //144 Hz / FPS
        //TargetElapsedTime = TimeSpan.FromMilliseconds(16.6666); //60 Hz / FPS
    }

    protected override void Initialize()
    {
        IsMouseVisible = true;
        //scale = new Vector2(4, 4);
        //tileSize = 16 * scale.X;
        //graphics.IsFullScreen = true;
        //Window.IsBorderless = true;
        //graphics.ApplyChanges();
        //camera.UpdateCamera(graphics.GraphicsDevice.Viewport, cameraPanDirection, 0);
        Util.ToggleFullScreen(this.graphics);
        this.sprites = new Sprites(this);
        this.screen = new Screen(this, this.graphics, 540);
        this.shapes = new Shapes(this);
        this.camera = new Camera(this.screen);

        Random random = new Random();

        vertices = new Vector2[5];
        vertices[0] = new Vector2(0, 10);
        vertices[1] = new Vector2(10, -10);
        vertices[2] = new Vector2(3, -6);
        vertices[3] = new Vector2(-3, -6);
        vertices[4] = new Vector2(-10, -10);

        base.Initialize(); 
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        //tileAtlas = Content.Load<Texture2D>("Textures\\32x32IsometricDebug");
        var fileStream = new FileStream("Content\\Textures\\32x32IsometricDebug.png", FileMode.Open);
        tileAtlas = Texture2D.FromStream(GraphicsDevice, fileStream);
        fileStream.Dispose();
        tiles = new Texture2D[4];
        //tiles[0] = new Texture2D(GraphicsDevice, 32, 32);
        tiles[1] = new Texture2D(GraphicsDevice, 32, 32);
        tiles[2] = new Texture2D(GraphicsDevice, 32, 32);
        tiles[3] = new Texture2D(GraphicsDevice, 32, 32);
        Color[] data = new Color[32 * 32];
        tileAtlas.GetData(0, new Rectangle(0, 0, 32, 32), data, 0, data.Length);
        tiles[1].SetData(data);
        tileAtlas.GetData(0, new Rectangle(32, 0, 32, 32), data, 0, data.Length);
        tiles[2].SetData(data);

        font = Content.Load<SpriteFont>("Font\\Font");

        world.LoadChunk(new Point(0, 0));
        world.LoadChunk(new Point(-1, 1));
        world.LoadChunk(new Point(1, -1));
        world.LoadChunk(new Point(1, 0));
        world.LoadChunk(new Point(0, 1));
        world.LoadChunk(new Point(1, 1));
        world.LoadChunk(new Point(0, 2));
        world.LoadChunk(new Point(2, 0));
        world.LoadChunk(new Point(2, 1));
        world.LoadChunk(new Point(1, 2));
        world.LoadChunk(new Point(2, 2));

        var rand = new Random();
        foreach (var chunk in world.LoadedChunks)
        {
            for (byte x = 0; x < chunk.Cell.GetLength(0); x++)
            {
                for (byte y = 0; y < chunk.Cell.GetLength(1); y++)
                {
                    /*
                    for(byte z = 0; z < chunk.Cell.GetLength(2); z++)
                    {
                        chunk.Cell[x, y, z] = 1;
                    }
                    */
                    
                    chunk.Cell[x, y, 0] = 1;
                    chunk.Cell[x, y, 1] = (ushort)rand.Next(0, 3);
                    if (chunk.Cell[x, y, 1] == 1)
                    {
                        chunk.Cell[x, y, 2] = (ushort)rand.Next(0, 3);
                    }
                }
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        //var keyboardState = Keyboard.GetState();
        //var mouseState = Mouse.GetState();
        //updateRate = 1 / gameTime.ElapsedGameTime.TotalSeconds;
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

        if (keyboard.IsKeyPressed(Keys.E))
        {
            this.camera.IncrementZoom();
        }
        if (keyboard.IsKeyPressed(Keys.Q))
        {
            this.camera.DecrementZoom();
        }
        if (keyboard.IsKeyDown(Keys.A))
        {
            this.camera.Move(-Vector2.UnitX);
        }
        if (keyboard.IsKeyDown(Keys.D))
        {
            this.camera.Move(Vector2.UnitX);
        }
        if (keyboard.IsKeyDown(Keys.W))
        {
            this.camera.Move(Vector2.UnitY);
        }
        if (keyboard.IsKeyDown(Keys.S))
        {
            this.camera.Move(-Vector2.UnitY);
        }

        this.angle += MathHelper.PiOver2 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        /*
        cameraPanDirection = Vector2.Zero;
        if (keyboardState.IsKeyDown(Keys.F1) && !previousKeyboardState.IsKeyDown(Keys.F1))
        {
            showStats = !showStats;
        }
        if (keyboardState.IsKeyDown(Keys.W))
        {
            cameraPanDirection.Y = -1;
        }
        if (keyboardState.IsKeyDown(Keys.S))
        {
            cameraPanDirection.Y = 1;
        }
        if (keyboardState.IsKeyDown(Keys.A))
        {
            cameraPanDirection.X = -1;
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            cameraPanDirection.X = 1;
        }
        
        previousMouseWheelValue = currentMouseWheelValue;
        currentMouseWheelValue = mouseState.ScrollWheelValue;
        var zoomDir = 0f;
        if (currentMouseWheelValue > previousMouseWheelValue)
        {
            zoomDir = .5f;
        }
        if (currentMouseWheelValue < previousMouseWheelValue)
        {
            zoomDir = -.5f;
        }

        if (zoomDir != 0)
        {
            camera.AdjustZoom(zoomDir);
        }
        if (cameraPanDirection != Vector2.Zero)
        {
            cameraPanDirection.Normalize();
        }

        if(mouseState.LeftButton == ButtonState.Pressed)
        {
            if (previousMouseState.LeftButton != ButtonState.Pressed)
            {
                dragOrigin = camera.Position;
                mouseOrigin = mouseState.Position.ToVector2();
            }
            else
            {
                camera.Position = (dragOrigin - ((mouseState.Position.ToVector2() - mouseOrigin) / camera.Zoom));
            }
        }

        camera.UpdateCamera(graphics.GraphicsDevice.Viewport, cameraPanDirection, gameTime.ElapsedGameTime.TotalSeconds);
        previousKeyboardState = keyboardState;
        previousMouseState = mouseState;
        base.Update(gameTime);
        */
    }

    protected override void Draw(GameTime gameTime)
    {
        this.screen.Set();
        this.GraphicsDevice.Clear(Color.Black);

        Viewport viewport = this.GraphicsDevice.Viewport;

        this.sprites.Begin(camera, false);
        this.sprites.Draw(tiles[1], null, new Vector2(16, 16), new Vector2(camera.Position.X, camera.Position.Y), 0f, new Vector2(2f, 2f), Color.Green);
        this.sprites.End();

        Color lineColor = Color.DarkBlue;

        this.shapes.Begin(camera);
        //this.shapes.DrawRectangle(32 + camera.Position.X, 0 + camera.Position.Y,23, 47, 1f, Color.DarkOliveGreen);
        //this.shapes.DrawLine(new Vector2(-24 + camera.Position.X, 0 + camera.Position.Y), new Vector2(15 + camera.Position.X, 33 + camera.Position.Y), 3, Color.DarkGreen);
        //this.shapes.DrawCircle(0, 32, 32, 48, 1, Color.White);
        //this.shapes.DrawPoloygon(this.verticies, 1f, Color.White);
        //Scale, then Rotate, then Translate
        //Matrix transform = Matrix.CreateScale(1f) * Matrix.CreateRotationZ(MathHelper.TwoPi / 10f) * Matrix.CreateTranslation(0f, 100f, 0f);

        Transform2D transform = new Transform2D(new Vector2(0f, 100f), this.angle, 2f);
        this.shapes.DrawPoloygon(this.vertices, transform, 1f, Color.White);

        this.shapes.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);
        /*
        GraphicsDevice.Clear(Color.Black);
        frameRate += ((1 / gameTime.ElapsedGameTime.TotalSeconds) - frameRate) * 0.1;

        var fps = string.Format("FPS: {0}", frameRate.ToString("0.00")); 
        var ups = string.Format("UPS: {0}", updateRate.ToString("0.00"));
        var cam = string.Format("CAM: X:{0}, Y:{1}, [X1:{2}, Y1:{3}, X2:{4}, Y2:{5}]", 
            camera.Position.X.ToString("0.00"), camera.Position.Y.ToString("0.00"),
            camera.VisibleArea.Left, camera.VisibleArea.Top, camera.VisibleArea.Right, camera.VisibleArea.Bottom);
        var zoom = string.Format("ZOOM: {0}x", camera.Zoom.ToString("0.00"));

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, camera.Transform);

        Rectangle drawArea = new Rectangle(camera.VisibleArea.X - ((int)tileSize * 2), camera.VisibleArea.Y - ((int)tileSize * 2), 
                                        camera.VisibleArea.Width + ((int)tileSize * 2), camera.VisibleArea.Height + ((int)tileSize * 2));

        calcTime.Restart();
        var chunks = "CHUNKS: ";
        foreach (var chunk in world.LoadedChunks)
        {
            var chunkStat = "X CHUNK: {0}";
            Primatives.DrawRectangle(chunk.Area, graphics.GraphicsDevice, camera);
            if (drawArea.Intersects(chunk.Area))
            {
                for (byte level = 0; level < chunk.Cell.GetLength(2); level++)
                {
                    for (byte x = 0; x < chunk.Cell.GetLength(0); x++)
                    {
                        for (byte y = 0; y < chunk.Cell.GetLength(1); y++)
                        {
                            Vector2 drawPos = new Vector2((chunk.WorldCoordinates.X * tileSize * chunk.Cell.GetLength(0)) + (x * tileSize - y * tileSize) - (chunk.WorldCoordinates.Y * tileSize * chunk.Cell.GetLength(1)),
                                                        (chunk.WorldCoordinates.X * (tileSize / 2) * chunk.Cell.GetLength(0)) + (x * (tileSize / 2) + (y - level * 2) * (tileSize / 2) + (chunk.WorldCoordinates.Y * (tileSize / 2) * chunk.Cell.GetLength(1))));
                            if (drawArea.Contains(drawPos) && chunk.Cell[x, y, level] > 0 && chunk.Exposed(x, y, level))
                            {
                                spriteBatch.Draw(tiles[chunk.Cell[x, y, level]], drawPos, new Rectangle(0, 0, 32, 32), Color.Green, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                            }
                        }
                    }
                }
                chunks += String.Format("[{0}, {1}]", chunk.WorldCoordinates.X, chunk.WorldCoordinates.Y);
                chunkStat += String.Format("[{0}, {1}, {2}]", chunk.WorldCoordinates.X, chunk.WorldCoordinates.Y, chunk.Area.ToString());
                spriteBatch.DrawString(font, chunkStat, new Vector2(chunk.Area.X, chunk.Area.Y), Color.White);
                spriteBatch.DrawString(font, "X", new Vector2(chunk.Area.X + chunk.Area.Width, chunk.Area.Y + chunk.Area.Height), Color.White);
            }
        }
        calcTime.Stop();
        var calc = string.Format("CALC: {0}ms", calcTime.Elapsed.TotalMilliseconds);
        drawTime.Restart();
        spriteBatch.End();
        drawTime.Stop();
        var draw = string.Format("DRAW: {0}ms", drawTime.Elapsed.TotalMilliseconds);
        var mem = string.Format("MEM: {0}kb", GC.GetTotalMemory(false) / 1000);
        var mouse = string.Format("MOUSE: X:{0}, Y:{1}, R:{2}, M:{3}, L:{4}", previousMouseState.X, previousMouseState.Y, previousMouseState.RightButton == ButtonState.Pressed, previousMouseState.MiddleButton == ButtonState.Pressed, previousMouseState.LeftButton == ButtonState.Pressed);

        if (showStats)
        {
            // No transform, is "GUI" space, doesn't follow camera movements
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);
            spriteBatch.DrawString(font, fps, new Vector2(1, 1), Color.White);
            spriteBatch.DrawString(font, ups, new Vector2(1, 40), Color.White);
            spriteBatch.DrawString(font, cam, new Vector2(1, 80), Color.White);
            spriteBatch.DrawString(font, zoom, new Vector2(1, 120), Color.White);
            spriteBatch.DrawString(font, mouse, new Vector2(1, 160), Color.White);
            spriteBatch.DrawString(font, calc, new Vector2(1, 200), Color.White);
            spriteBatch.DrawString(font, draw, new Vector2(1, 240), Color.White);
            spriteBatch.DrawString(font, chunks, new Vector2(1, 280), Color.White);
            spriteBatch.DrawString(font, mem, new Vector2(1, 320), Color.White);
            spriteBatch.End();
        }
        */
        base.Draw(gameTime);
    }
}
