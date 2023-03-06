using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Friends.Game.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.ViewportAdapters;
using TiledCS;

namespace Friends.Game.Scenes;

public class OverworldScene : HexmapScene
{
    private World _world;

    public override string TilemapFilename => "main.tmx";

    public OverworldScene(MyGame game) : base(game)
    {
    }
}