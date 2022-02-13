using System.Xml.Linq;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace SE2.Components
{
    public class TileMapComponent: Component
    {
        public class Tile
        {
            public int id;
            public string source;
            public string shaderName;
        }

        protected class Layer
        {
            public List<int> tiles;
        }

        protected string orientation;
        protected string renderorder;
        protected Utils.Vec2 size;
        protected Utils.Vec2 tileSize;
        protected bool infinite;
        protected List<Tile> tiles;
        protected List<Layer> layers;
        protected List<string> textures;
        public bool displayed;

        public TileMapComponent(string tilemap, string shadername = "sprite", bool displayed = true): base()
        {
            this.displayed = displayed;

            tiles = new List<Tile>();
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
                tiles.Add(new Tile() {
                    id = Convert.ToInt32(tiletype.Attribute("id").Value) + 1,
                    source = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(tilemap) + Path.DirectorySeparatorChar + tiletype.Element("image").Attribute("source").Value),
                    shaderName = shadername
                });
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

        public Tile GetTile(int id)
        {
            foreach(Tile tile in tiles)
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

            foreach (Tile tile in tiles)
                Graphics.Renderers.SpriteRenderer.Load(GetWindow(), tile.shaderName, tile.source);
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
                            {
                                Utils.Vec2 offset = new Utils.Vec2(-tileSize.x * size.x * tc.scale.x / 2, tileSize.y * size.y * tc.scale.y / 2) + new Utils.Vec2(tileSize.x * tc.scale.x * Convert.ToInt32(i % Convert.ToInt32(size.x)), -tileSize.y * tc.scale.y * Convert.ToInt32(i / Convert.ToInt32(size.y)));

                                Matrix4 model = Matrix4.Identity
                                    * Matrix4.CreateScale(tileSize.x / 2, tileSize.y / 2, 1)
                                    * Matrix4.CreateScale(tc.scale.x, tc.scale.y, 1)
                                    * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                                    * Matrix4.CreateTranslation(new Vector3(tc.position.x + offset.x, tc.position.y + offset.y, tc.position.z));

                                Graphics.Renderers.SpriteRenderer.Render(GetWindow(), GetTile(layer.tiles[i]).shaderName, GetTile(layer.tiles[i]).source, false, false, model);
                            }
                        }
                    }
                }
            }

        }
    }
}