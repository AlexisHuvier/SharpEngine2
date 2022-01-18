using System.Xml.Linq;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace SE2.Components
{
    public class TileMapComponent: Component
    {
        protected class Layer
        {
            public List<int> tiles;
        }

        protected string orientation;
        protected string renderorder;
        protected Utils.Vec2 size;
        protected Utils.Vec2 tileSize;
        protected bool infinite;
        protected List<Graphics.Tile> tiles;
        protected List<Layer> layers;
        protected List<string> textures;
        public bool displayed;

        public TileMapComponent(string tilemap, string shadername = "sprite", bool displayed = true): base()
        {
            this.displayed = displayed;

            tiles = new List<Graphics.Tile>();
            layers = new List<Layer>();
            textures = new List<string>();

            XElement file = XElement.Load(tilemap);

            orientation = file.Attribute("orientation").Value;
            if(orientation != "orthogonal")
            {
                Trace.WriteLine("[ERROR] SharpEngine2 can only use orthogonal tilemap.");
                throw new Exception("[ERROR] SharpEngine2 can only use orthogonal tilemap.");
            }
            renderorder = file.Attribute("renderorder").Value;
            if(renderorder != "right-down")
            {
                Trace.WriteLine("[ERROR] SharpEngine2 can only use right-down renderorder.");
                throw new Exception("[ERROR] SharpEngine2 can only use right-down renderorder.");
            }
            size = new Utils.Vec2(Convert.ToInt32(file.Attribute("width").Value), Convert.ToInt32(file.Attribute("height").Value));
            tileSize = new Utils.Vec2(Convert.ToInt32(file.Attribute("tilewidth").Value), Convert.ToInt32(file.Attribute("tileheight").Value));
            infinite = Convert.ToBoolean(Convert.ToInt32(file.Attribute("infinite").Value));
            if(infinite)
            {
                Trace.WriteLine("[ERROR] SharpEngine2 can only use non-infinite tilemap.");
                throw new Exception("[ERROR] SharpEngine2 can only use non-infinite tilemap.");
            }

            foreach(XElement tiletype in file.Element("tileset").Elements("tile"))
            {
                textures.Add(Path.GetDirectoryName(tilemap) + Path.DirectorySeparatorChar + tiletype.Element("image").Attribute("source").Value);
                tiles.Add(new Graphics.Tile(
                    Convert.ToInt32(tiletype.Attribute("id").Value) + 1,
                    Path.GetFileNameWithoutExtension(Path.GetDirectoryName(tilemap) + Path.DirectorySeparatorChar + tiletype.Element("image").Attribute("source").Value),
                    shadername
                ));
            }

            foreach(XElement element in file.Elements("layer"))
            {
                List<int> tiles = new List<int>();
                foreach (string tile in element.Element("data").Value.Split(","))
                    tiles.Add(Convert.ToInt32(tile));
                Layer layer = new Layer()
                {
                    tiles = tiles
                };
                layers.Add(layer);
            }
            layers.Reverse();
        }

        public Graphics.Tile GetTile(int id)
        {
            foreach(Graphics.Tile tile in tiles)
            {
                if (tile.id == id)
                    return tile;
            }
            return null;
        }

        public override void Load()
        {
            base.Load();

            foreach(string texture in textures)
                GetWindow().textureManager.AddTexture(Path.GetFileNameWithoutExtension(texture), texture);

            foreach (Graphics.Tile tile in tiles)
                tile.Load(GetWindow());
        }

        public override void Render()
        {
            base.Render();

            if (!displayed)
                return;

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    foreach (Layer layer in layers)
                    {
                        for (int i = 0; i < layer.tiles.Count; i++)
                        {
                            if (layer.tiles[i] != 0)
                                GetTile(layer.tiles[i]).Render(GetWindow(), tc, new Utils.Vec2(-tileSize.x * size.x * tc.scale.x / 2, tileSize.y * size.y * tc.scale.y / 2) + new Utils.Vec2(tileSize.x * tc.scale.x * Convert.ToInt32(i % Convert.ToInt32(size.x)), -tileSize.y * tc.scale.y * Convert.ToInt32(i / Convert.ToInt32(size.y))));
                        }
                    }
                }
            }

        }
    }
}