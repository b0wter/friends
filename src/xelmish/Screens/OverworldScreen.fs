namespace Friends.Xelmish.Screens

open System
open System.IO
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open MonoGame.Extended
open TiledCS

module OverworldScreen =
    open Xelmish.Viewables
    open Xelmish.Model
    
    /// Used in the transformation matrix to render the tile SpriteBatch
    let private ScaleFactor = 1.0f // XNA uses single precision floats
    
    let private DefaultTransformationMatrix = Matrix.CreateScale (ScaleFactor, ScaleFactor, 1.0f)
    
    let private Sqrt3 = MathF.Sqrt 3.0f
    
    type private TileSetKey = {
        TileSetId: int
        Offset: int
    }
    
    type Model = {
        TiledMap: TiledMap
        /// <summary>
        /// Size of the tiles in pixels. It's the number of pixel from the center to the outer circle of the hexagon.
        /// This is equivalent to the distance from the center to one of the "spikes"
        /// </summary>
        /// <remarks>
        /// See https://www.redblobgames.com/grids/hexagons/#basics for the definition of `Size`.
        /// </remarks>
        TileSize: int
        TileSets: Map<int, TiledTileset>
        TileSetTextures: Map<int, Texture2D>
        /// <summary>
        /// Maps each id used in the TileMap to tileset id and an offset. The offset is the position of the tile in the
        /// tileset
        /// </summary>
        TileSetTileIds: Map<int, TileSetKey>
        Camera: OrthographicCamera
    }
    
    let init (tiledMapName: string) =
        let tiledMap = Path.Combine ()
        {
            Counter = 0
        }
        
        
    type Message =
        | CameraUp
        | CameraDown
        | CameraLeft
        | CameraRight

    let view model dispatch =
        let text size = text "connection" size Colour.White (-0.5, 0.)
        [
            yield text 100.0 "TETRIS!" (400, 40)
        ]
