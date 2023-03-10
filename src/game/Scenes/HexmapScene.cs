using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using TiledCS;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Friends.Game.Scenes;

public abstract class HexmapScene : GameplayScene
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
    
    private const int ScaleFactor = 1;
    private const string TileDrawingHeightKey = "TileDrawingHeight";
    
    private TiledMap _tiledMap;
    private readonly double Sqrt3 = Math.Sqrt(3);
    private int TileSize;
    private Matrix _transformMatrix;
    private Dictionary<int, TiledTileset> _tilesets;
    private Dictionary<int, Texture2D> _tilesetTextures = new();
    private SpriteBatch _spriteBatch;
    private Dictionary<int, (int TilesetId, int Offset)> _tilesetTilesId;
    private bool _wasTerrainRendered;
    private RenderTarget2D _renderTarget;
    private OrthographicCamera _camera;
    
    public abstract string TilemapFilename { get; }
    
    public HexmapScene(MyGame game) : base(game)
    {
        var viewportAdapter = new BoxingViewportAdapter(game.Window, game.GraphicsDevice, 1920, 1080);
        _camera = new OrthographicCamera(viewportAdapter);
    }

    public override void Initialize()
    {
        _transformMatrix = Matrix.CreateScale(ScaleFactor, ScaleFactor, 1f);

        var rawProject = File.ReadAllText(Path.Combine("Content", "friends.tiled-project"));
        var project = Newtonsoft.Json.JsonConvert.DeserializeObject<Helpers.Tiled.Project>(rawProject);

        base.Initialize();
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        var movementDirection = Vector2.Zero;
        var state = _game.CurKeyboardState;

        if (state.IsKeyDown(Keys.Down))
        {
            movementDirection += Vector2.UnitY;
        }
        if (state.IsKeyDown(Keys.Up))
        {
            movementDirection -= Vector2.UnitY;
        }
        if (state.IsKeyDown(Keys.Left))
        {
            movementDirection -= Vector2.UnitX;
        }
        if (state.IsKeyDown(Keys.Right))
        {
            movementDirection += Vector2.UnitX;
        }

        if (state.IsKeyDown(Keys.OemPlus))
        {
            _camera.Zoom = Math.Clamp(_camera.Zoom + 0.3f * gameTime.GetElapsedSeconds(), _camera.MinimumZoom, _camera.MaximumZoom);
        }

        if (state.IsKeyDown(Keys.OemMinus))
        {
            _camera.Zoom = Math.Clamp(_camera.Zoom - 0.3f * gameTime.GetElapsedSeconds(), _camera.MinimumZoom, _camera.MaximumZoom);
        }

        const float cameraSpeed = 400;
        _camera.Move(movementDirection * cameraSpeed * gameTime.GetElapsedSeconds());
    }
    
    public override void LoadContent()
    {
        base.LoadContent();
        
        _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        _tiledMap = new TiledMap(Path.Combine(_game.Content.RootDirectory, "tilemaps", TilemapFilename));
        TileSize = _tiledMap.TileWidth / 2;
        _tilesets = _tiledMap.GetTiledTilesets(_game.Content.RootDirectory + "/Content/");
        var tilesetIds = _tiledMap.Tilesets.Select(t => t.firstgid).Order().ToList();

        var allTilesetTileIds = new HashSet<int>(_tilesets.Sum(t => t.Value.TileCount));
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

        var pixelWidth = (int)Math.Ceiling(3.0 / 4.0 * _tiledMap.Width * _tiledMap.TileWidth);
        var pixelHeight = (int)Math.Ceiling((_tiledMap.Height + 0.5) * _tiledMap.TileHeight + 5);
        
        _renderTarget = new RenderTarget2D(_game.GraphicsDevice, pixelWidth, pixelHeight);
    }

    protected (int TileX, int TileY) PixelCoordinatesToTileIds(int x, int y)
        => (TileX: TileSize * 3 / 2 * y,
            TileY: (int)Math.Round(TileSize * Sqrt3 * (y + 0.5 * (x&1)), 0));
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (_wasTerrainRendered == false)
        {
            _wasTerrainRendered = true;
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
                        // var tileX = x * _tiledMap.TileWidth;
                        // var tileY = y * _tiledMap.TileHeight;

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
                                (int)(x * _tiledMap.TileWidth * 0.75),
                                (x % 2 == 0 ? y * 84 : y * 84 + 42) - 11,
                                _tiledMap.TileWidth,
                                96
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
                            case Trans.Flip_H:
                                effects = SpriteEffects.FlipHorizontally;
                                break;
                            case Trans.Flip_V:
                                effects = SpriteEffects.FlipVertically;
                                break;

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
                        _spriteBatch.Draw(_tilesetTextures[tilesetId], destination, source, Color.White,
                            (float)rotation, Vector2.Zero, effects, 0);
                    }
                }
            }

            /*
            if (debugRect != null)
            {
                Texture2D _texture = new Texture2D(_game.GraphicsDevice, 1, 1);
                _texture.SetData(new Color[] { Color.Green });
    
                _spriteBatch.Draw(_texture, (Rectangle)debugRect, Color.White);
            }
            */

            _spriteBatch.End();
        }

        _game.GraphicsDevice.SetRenderTarget(null);
        _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_renderTarget, new Vector2(0, 0), _renderTarget.Bounds, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
        _spriteBatch.End();
    }
}