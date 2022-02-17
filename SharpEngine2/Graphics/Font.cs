using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using FreeTypeSharp;
using static FreeTypeSharp.Native.FT;
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

        readonly Dictionary<char, Character> Characters;

        internal FreeTypeFaceFacade face;
        
        public Font(string file, uint size)
        {
            Characters = new Dictionary<char, Character>();
            if (FT_New_Face(Managers.FontManager.lib.Native, file, 0, out System.IntPtr internalface) != FreeTypeSharp.Native.FT_Error.FT_Err_Ok)
                Trace.WriteLine($"[ERROR] Cannot read font file : {file}");
            face = new FreeTypeFaceFacade(Managers.FontManager.lib, internalface);
            FT_Set_Pixel_Sizes(face.Face, 0, size);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            List<uint> glyphsChecks = new List<uint>();
            uint cbis = FT_Get_First_Char(face.Face, out uint back);
            glyphsChecks.Add(back);
            while (FT_Get_Next_Char(face.Face, cbis, out uint glyphindex) is uint cbos)
            {
                cbis = cbos;
                FT_Load_Char(face.Face, System.Convert.ToChar(cbis), FT_LOAD_RENDER);

                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.CompressedRed, (int)face.GlyphBitmap.width, (int)face.GlyphBitmap.rows, 0, PixelFormat.Red, PixelType.UnsignedByte, face.GlyphBitmap.buffer);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                Character character = new Character() {
                    TextureID = texture,
                    Size = new Utils.Vec2(face.GlyphBitmap.width, face.GlyphBitmap.rows),
                    Bearing = new Utils.Vec2(face.GlyphBitmapLeft, face.GlyphBitmapTop),
                    Advance = face.GlyphMetricHorizontalAdvance
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
    }
}
