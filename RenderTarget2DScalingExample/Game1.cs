using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RenderTarget2DScalingExample;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Target to render game to
    private RenderTarget2D _renderTarget;

    // Destination for rendered game
    private Rectangle _renderDestination;

    // A profoundly shitty background to show scaling result
    private Texture2D _awfulBackground;

    private bool _resizing;
    private int _targetWidth = 1280;
    private int _targetHeight = 720;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Drawing to a 1280x720 virtual game area
        _graphics.PreferredBackBufferWidth = _targetWidth;
        _graphics.PreferredBackBufferHeight = _targetHeight;
        _graphics.ApplyChanges();

        Window.ClientSizeChanged += OnClientSizeChanged;
        Window.AllowUserResizing = true;
        Window.IsBorderless = false;

        _renderTarget = new RenderTarget2D(GraphicsDevice, _targetWidth, _targetHeight);
        _renderDestination = new Rectangle();

        this.CalculateRenderDimensions();

        base.Initialize();
    }

    // Recalculate what our scaling logic will be whenever the window size is changed
    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        // The resizing flag is just to stop the window needlessly recalculating as you drag the window (calculate once)
        if (!_resizing)
        {
            _resizing = true;
            CalculateRenderDimensions();
            _resizing = false;
        }
    }

    private void CalculateRenderDimensions()
    {
        Point size = GraphicsDevice.Viewport.Bounds.Size;

        float scaleX = (float)size.X / _targetWidth;
        float scaleY = (float)size.Y / _targetHeight;
        float scale = Math.Min(scaleX, scaleY);

        _renderDestination.Width = (int)(_renderTarget.Width * scale);
        _renderDestination.Height = (int)(_renderTarget.Height * scale);

        _renderDestination.X = (size.X - _renderDestination.Width) / 2;
        _renderDestination.Y = (size.Y - _renderDestination.Height) / 2;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _awfulBackground = Content.Load<Texture2D>("AwfulLandscape");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw the game world at desired internal resolution
        GraphicsDevice.SetRenderTarget(_renderTarget);
        _spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp);
        _spriteBatch.Draw(_awfulBackground, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        _spriteBatch.End();

        // Draw game world at scaled resolution
        GraphicsDevice.SetRenderTarget(null);
        _spriteBatch.Begin();
        _spriteBatch.Draw(_renderTarget, _renderDestination, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
