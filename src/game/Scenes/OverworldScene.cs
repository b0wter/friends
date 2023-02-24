using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Friends.Game.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.ViewportAdapters;
using TiledCS;

namespace Friends.Game.Scenes;

public class OverworldScene : GameplayScene
{
    [Flags]
    enum Trans
    {
        None = 0,
        Flip_H = 1 << 0,
        Flip_V = 1 << 1,
        Flip_D = 1 << 2,

        Rotate_90 = Flip_D | Flip_H,
        Rotate_180 = Flip_H | Flip_V,
        Rotate_270 = Flip_V | Flip_D,

        Rotate_90AndFlip_H = Flip_H | Flip_V | Flip_D,
    }

    private World _world;
    private TiledMap _tiledMap;
    private Matrix _transformMatrix;
    private Dictionary<int, TiledTileset> _tilesets;
    private Dictionary<int, Texture2D> _tilesetTextures = new();
    private SpriteBatch _spriteBatch;
    private Dictionary<int, (int TilesetId, int Offset)> _tilesetTilesId;
    private RenderTarget2D _renderTarget;
    private OrthographicCamera _camera;

    private const int ScaleFactor = 3;

    public OverworldScene(MyGame game) : base(game)
    {
        _game.Content.RootDirectory = "Content";
        _game.IsMouseVisible = true;
        _renderTarget = new RenderTarget2D(game.GraphicsDevice, 5784, 6762);
        var viewportAdapter = new BoxingViewportAdapter(game.Window, game.GraphicsDevice, 1920, 1080);
        _camera = new OrthographicCamera(viewportAdapter);
    }

    public override void Initialize()
    {
        _transformMatrix = Matrix.CreateScale(ScaleFactor, ScaleFactor, 1f);

        base.Initialize();
    }

    public override void LoadContent()
    {
        void AddTilesetTexture(string name)
        {
            var id = _tiledMap.Tilesets.FirstOrDefault(t => t.source.EndsWith($"{name}.tsx")).firstgid;
            _tilesetTextures.Add(id, _game.Content.Load<Texture2D>(Path.Combine("tilesets", "zeshio", name)));
        }
        
        _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        _tiledMap = new TiledMap(Path.Combine(_game.Content.RootDirectory, "tilemaps", "main.tmx"));
        _tilesets = _tiledMap.GetTiledTilesets(_game.Content.RootDirectory + "/Content/");
        var tilesetIds = _tiledMap.Tilesets.Select(t => t.firstgid).Order().ToList();

        var allTilesetTileIds = new HashSet<int>(_tilesets.Count * 64);
        _tiledMap.Layers.SelectMany(t => t.data).ToList().ForEach(x => allTilesetTileIds.Add(x));
        allTilesetTileIds.Remove(0);

        _tilesetTilesId = new Dictionary<int, (int TilesetId, int Offset)>(allTilesetTileIds.Count);
        var highestTilesetId = tilesetIds.Last();
        foreach (var id in allTilesetTileIds)
        {
            var indexOfFirstLargerId =
                id >= highestTilesetId
                    ? tilesetIds.Count
                    : tilesetIds.FindIndex(i => i > id);
            var tilesetId = _tiledMap.Tilesets[indexOfFirstLargerId - 1].firstgid; // tilesetIds[indexOfFirstLargerId - 1];
            _tilesetTilesId.Add(id, (tilesetId, id - tilesetId));
        }
        
        foreach (var tileset in _tiledMap.Tilesets)
        {
            var name = Path.GetFileNameWithoutExtension(tileset.source);
            _tilesetTextures.Add(tileset.firstgid, _game.Content.Load<Texture2D>(Path.Combine("tilesets", "zeshio", name)));
        }
        
        base.LoadContent();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        _game.GraphicsDevice.SetRenderTarget(_renderTarget);
        
        _game.GraphicsDevice.Clear(new Color(24, 24, 24));
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _transformMatrix);

        var tileLayers = _tiledMap.Layers.Where(x => x.type == TiledLayerType.TileLayer);

        foreach (var layer in tileLayers)
        {
            for (var y = 0; y < layer.height; y++)
            {
                for (var x = 0; x < layer.width; x++)
                {
                        var index = (y * layer.width) + x; // Assuming the default render order is used which is from right to bottom
                        var gid = layer.data[index]; // The tileset tile index
                        var tileX = x * _tiledMap.TileWidth;
                        var tileY = y * _tiledMap.TileHeight;

                        // Gid 0 is used to tell there is no tile set
                        if (gid == 0)
                        {
                            continue;
                        }

                        // Helper method to fetch the right TieldMapTileset instance
                        // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                        var mapTileset = _tiledMap.GetTiledMapTileset(gid);

                        // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                        var tileset = _tilesets[mapTileset.firstgid];

                        // Use the connection object as well as the tileset to figure out the source rectangle
                        var rect = _tiledMap.GetSourceRect(mapTileset, tileset, gid);

                        // Create destination and source rectangles
                        var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                        var destination =
                            new Rectangle
                                (
                                    (int)(tileX * 0.75) - 1,
                                    (x % 2 == 0 ? tileY : tileY + 38) - 11, //_tiledMap.TileHeight / 2) - 11,
                                    _tiledMap.TileWidth,
                                    _tiledMap.TileHeight
                                );


                        // You can use the helper methods to get information to handle flips and rotations
                        Trans tileTrans = Trans.None;
                        if (_tiledMap.IsTileFlippedHorizontal(layer, x, y)) tileTrans |= Trans.Flip_H;
                        if (_tiledMap.IsTileFlippedVertical(layer, x, y)) tileTrans |= Trans.Flip_V;
                        if (_tiledMap.IsTileFlippedDiagonal(layer, x, y)) tileTrans |= Trans.Flip_D;

                        SpriteEffects effects = SpriteEffects.None;
                        double rotation = 0f;
                        switch (tileTrans)
                        {
                            case Trans.Flip_H: effects = SpriteEffects.FlipHorizontally; break;
                            case Trans.Flip_V: effects = SpriteEffects.FlipVertically; break;

                            case Trans.Rotate_90:
                                rotation = Math.PI * .5f;
                                destination.X += _tiledMap.TileWidth;
                                break;

                            case Trans.Rotate_180:
                                rotation = Math.PI;
                                destination.X += _tiledMap.TileWidth;
                                destination.Y += _tiledMap.TileHeight;
                                break;

                            case Trans.Rotate_270:
                                rotation = Math.PI * 3 / 2;
                                destination.Y += _tiledMap.TileHeight;
                                break;

                            case Trans.Rotate_90AndFlip_H:
                                effects = SpriteEffects.FlipHorizontally;
                                rotation = Math.PI * .5f;
                                destination.X += _tiledMap.TileWidth;
                                break;

                            default:
                                break;
                        }

                        // Render sprite at position tileX, tileY using the rect
                        var (tilesetId, offset) = _tilesetTilesId[gid];
                        _spriteBatch.Draw(_tilesetTextures[tilesetId], destination, source, Color.White, (float)rotation, Vector2.Zero, effects, 0);                    
                }
            }
        }

        /*
        // If mouse is over a collider, display its bounds
        if (debugRect != null)
        {
            Texture2D _texture = new Texture2D(_game.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.Green });

            _spriteBatch.Draw(_texture, (Rectangle)debugRect, Color.White);
        }
        */

        _spriteBatch.End();
        
        _game.GraphicsDevice.SetRenderTarget(null);
        _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_renderTarget, new Vector2(0, 0), _renderTarget.Bounds, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
        _spriteBatch.End();

        // base.Draw(gameTime);
    }
}