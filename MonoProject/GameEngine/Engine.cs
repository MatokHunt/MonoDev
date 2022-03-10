namespace MonoProject.GameEngine;

public class Engine : Game
{
    private double frameRate = 1 / 60;
    private double updateRate = 1 / 60;
    private Vector2 scale;
    private float tileSize;
    private bool showStats = true;

    private Texture2D tileAtlas;
    private Texture2D[] tiles;
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont font;
    private Camera camera;
    private Vector2 cameraPanDirection = Vector2.Zero;
    private float currentMouseWheelValue, previousMouseWheelValue;
    private KeyboardState previousKeyboardState;
    private MouseState previousMouseState;
    private Vector2 dragOrigin, mouseOrigin;

    public Engine()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.HardwareModeSwitch = false;
        graphics.IsFullScreen = false;
        graphics.PreferredBackBufferWidth = 800;
        graphics.PreferredBackBufferHeight = 600;
        graphics.ApplyChanges();
        Window.IsBorderless = true;
        Content.RootDirectory = "Content";
        camera = new Camera(graphics.GraphicsDevice.Viewport);
        //graphics.SynchronizeWithVerticalRetrace = false;
        //IsFixedTimeStep = false; // Turn off time step locking
        //TargetElapsedTime = TimeSpan.FromMilliseconds(6.9444); //144 Hz / FPS
        //TargetElapsedTime = TimeSpan.FromMilliseconds(16.6666); //60 Hz / FPS
    }

    protected override void Initialize()
    {
        base.Initialize();
        IsMouseVisible = true;
        scale = new Vector2(4, 4);
        tileSize = 16 * scale.X;
        graphics.IsFullScreen = true;
        graphics.ApplyChanges();
        camera.UpdateCamera(graphics.GraphicsDevice.Viewport, cameraPanDirection, 0);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        tileAtlas = Content.Load<Texture2D>("Textures\\32x32IsometricDebug");
        tiles = new Texture2D[3];
        tiles[0] = new Texture2D(GraphicsDevice, 32, 32);
        tiles[1] = new Texture2D(GraphicsDevice, 32, 32);
        tiles[2] = new Texture2D(GraphicsDevice, 32, 32);
        Color[] data = new Color[32 * 32];
        tileAtlas.GetData(0, new Rectangle(0, 0, 32, 32), data, 0, data.Length);
        tiles[0].SetData(data);
        tileAtlas.GetData(0, new Rectangle(0, 0, 32, 32), data, 0, data.Length);
        tiles[1].SetData(data);
        tileAtlas.GetData(0, new Rectangle(32, 0, 32, 32), data, 0, data.Length);
        tiles[2].SetData(data);

        graphics.IsFullScreen = true;
        graphics.ApplyChanges();

        font = Content.Load<SpriteFont>("Font\\Font");
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();
        updateRate = 1 / gameTime.ElapsedGameTime.TotalSeconds;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            Exit();
        
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

        if(mouseState.RightButton == ButtonState.Pressed)
        {
            if (previousMouseState.RightButton != ButtonState.Pressed)
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
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        frameRate += ((1 / gameTime.ElapsedGameTime.TotalSeconds) - frameRate) * 0.1;

        var fps = string.Format("FPS: {0}", frameRate.ToString("0.00")); 
        var ups = string.Format("UPS: {0}", updateRate.ToString("0.00"));
        var cam = string.Format("CAM: X:{0}, Y:{1}, [X1:{2}, Y1:{3}, X2:{4}, Y2:{5}]", 
            camera.Position.X.ToString("0.00"), camera.Position.Y.ToString("0.00"),
            camera.VisibleArea.Left, camera.VisibleArea.Top, camera.VisibleArea.Right, camera.VisibleArea.Bottom);
        var zoom = string.Format("ZOOM: {0}x", camera.Zoom.ToString("0.00"));

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, camera.Transform);

        Rectangle drawArea = new Rectangle(camera.VisibleArea.X - ((int)tileSize * 2), camera.VisibleArea.Y - ((int)tileSize * 2), camera.VisibleArea.Width + ((int)tileSize * 2), camera.VisibleArea.Height + ((int)tileSize * 2));
        for (int level = 0; level < 3; level++)
        {
            for (int x = 0; x < 60 - (level * 20); x++)
            {
                for (int y = 0; y < 60 - (level * 20); y++)
                {
                    Vector2 drawPos = new Vector2((x * tileSize - y * tileSize), (x * (tileSize / 2) + (y - level * 2) * (tileSize / 2)));
                    if (drawArea.Contains(drawPos))
                    {
                        spriteBatch.Draw(tiles[level], drawPos, new Rectangle(0, 0, 32, 32), Color.Green, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                }
            }
        }
        var drawTime = DateTime.Now;
        spriteBatch.End();
        var draw = string.Format("DRAW: {0}ms", DateTime.Now.Subtract(drawTime).TotalMilliseconds.ToString("0.00"));
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
            spriteBatch.DrawString(font, draw, new Vector2(1, 160), Color.White);
            spriteBatch.DrawString(font, mem, new Vector2(5, 200), Color.White);
            spriteBatch.DrawString(font, mouse, new Vector2(5, 240), Color.White);
            spriteBatch.End();
        }
        base.Draw(gameTime);
    }
}
