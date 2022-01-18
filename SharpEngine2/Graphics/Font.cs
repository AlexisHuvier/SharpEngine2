using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using SharpFont;
using System.Diagnostics;

namespace SE2.Graphics
{
    public class Font
    {
        internal struct Character
        {
            public int TextureID;  // ID handle of the glyph texture
            public Utils.Vec2 Size;       // Size of glyph
            public Utils.Vec2 Bearing;    // Offset from baseline to left/top of glyph
            public int Advance;    // Offset to advance to next glyph
        };

        Dictionary<char, Character> Characters;

        internal Face face;

        public Font(string file, uint size)
        {
            Characters = new Dictionary<char, Character>();
            face = new Face(Managers.FontManager.lib, file);
            face.SetPixelSizes(0, size);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            List<uint> glyphsChecks = new List<uint>();
            uint cbis = face.GetFirstChar(out uint back);
            glyphsChecks.Add(back);
            while (face.GetNextChar(cbis, out uint glyphindex) is uint cbos)
            {
                cbis = cbos;
                face.LoadChar(System.Convert.ToChar(cbis), LoadFlags.Render, LoadTarget.Normal);

                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.CompressedRed, face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows, 0, PixelFormat.Red, PixelType.UnsignedByte, face.Glyph.Bitmap.Buffer);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                Character character = new Character() {
                    TextureID = texture,
                    Size = new Utils.Vec2(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows),
                    Bearing = new Utils.Vec2(face.Glyph.BitmapLeft, face.Glyph.BitmapTop),
                    Advance = (int)face.Glyph.Advance.X
                };
                Characters.Add(System.Convert.ToChar(cbis), character);
                if (glyphsChecks.Contains(glyphindex))
                    break;
                glyphsChecks.Add(glyphindex);
            }

        }

        internal List<Character> GetCharacters(string text)
        {
            List<Character> c = new List<Character>();

            foreach (char ch in text)
            {
                if (Characters.ContainsKey(ch))
                    c.Add(Characters[ch]);
                else
                    Trace.WriteLine($"[ERROR] This characters doesn't exists in tff : {ch}");
            }

            return c;
        }

        internal void Unload()
        {
            face.Dispose();
        }
    }
}
