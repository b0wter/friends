namespace Friends.Game.Scenes;

public abstract class GameplayScene : Scene
{
    public GameplayScene(MyGame game) : base(game)
    {
        _game.Content.RootDirectory = "Content";
        _game.IsMouseVisible = true;
    }
}