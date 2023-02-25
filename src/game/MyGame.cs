using System;
using Friends.Game.Scenes;

namespace Friends.Game;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public class MyGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Scene _activeScene;
    private Scene _nextScene;

    /// <summary>
    ///     Gets the state of keyboard input during the previous frame.
    /// </summary>
    public KeyboardState PrevKeyboardState { get; private set; }

    /// <summary>
    ///     Gets the state of keyboard input during the current frame.
    /// </summary>
    public KeyboardState CurKeyboardState { get; private set; }

    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferHeight = 1400;
        _graphics.PreferredBackBufferWidth = 1920;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _nextScene = new OverworldScene(this);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //    Exit();
        
        PrevKeyboardState = CurKeyboardState;
        CurKeyboardState = Keyboard.GetState();

        //  If there is a next scene waiting to be switched to
        //  transition to that scene.
        if (_nextScene != null)
        {
            TransitionScene();
        }

        //  If there is an active scene, update it.
        if (_activeScene != null)
        {
            _activeScene.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(32, 32, 32));

        //  If there is an active scene, draw it.
        if (_activeScene != null)
        {
            _activeScene.BeforeDraw(_spriteBatch, Color.Black);
            _activeScene.Draw(_spriteBatch);
            _activeScene.AfterDraw(_spriteBatch);
        }

        base.Draw(gameTime);
    }
    
    /// <summary>
    ///     Sets the next scene to switch to.
    /// </summary>
    /// <param name="next">
    ///     The Scene instance to switch to.
    /// </param>
    public void ChangeScene(Scene next)
    {
        //  Only set the _nextScene value if it is not the
        //  same instance as the _activeScene.
        if (_activeScene != next)
        {
            _nextScene = next;
        }
    }    
    
    /// <summary>
    ///     Handles transitioning gracefully from one scene to
    ///     the next.
    /// </summary>
    private void TransitionScene()
    {
        if (_activeScene != null)
        {
            _activeScene.UnloadContent();
        }

        //  Perform a garbage collection to ensure memory is cleared
        GC.Collect();

        //  Set the active scene.
        _activeScene = _nextScene;

        //  Null the next scene value
        _nextScene = null;

        //  If the active scene isn't null, initialize it.
        //  Remember, the Initialize method also calls the LoadContent method
        if (_activeScene != null)
        {
            _activeScene.Initialize();
        }
    }
}
