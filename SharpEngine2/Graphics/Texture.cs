using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace SE2.Graphics
{ 
    public enum TextureFilter
    {

        Nearest = 9728,
        Linear = 9729
    }

    public class Texture
    {
        public readonly int Handle;
        public Vector2 size;

        public Texture(int glHandle, Vector2 s)
        {
            Handle = glHandle;
            size = s;
        }

        public void Unload()
        {
            GL.DeleteTexture(Handle);
        }

        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public static Texture LoadFromPixels(int width, int height, IntPtr data, bool generateMipmaps = false, bool srgb = false)
        {
            Vector2 size = new Vector2(width, height);
            int handle = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            GL.TextureStorage2D(handle, 
                generateMipmaps == false ? 1 : (int)Math.Floor(Math.Log(Math.Max(width, height), 2)),
                srgb ? (SizedInternalFormat)All.Srgb8Alpha8: SizedInternalFormat.Rgba8,
                width, 
                height
                );

            GL.TextureSubImage2D(handle, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, data);

            if (generateMipmaps)
                GL.GenerateTextureMipmap(handle);

            GL.TextureParameter(handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TextureParameter(handle, TextureParameterName.TextureMaxLevel, (generateMipmaps == false ? 1 : (int)Math.Floor(Math.Log(Math.Max(width, height), 2))) - 1);

            return new Texture(handle, size);
        }

        public static Texture LoadFromFile(string path, TextureFilter minFilter = TextureFilter.Linear, TextureFilter magFilter = TextureFilter.Linear)
        {
            int handle = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            Vector2 size;

            using (Bitmap image = new Bitmap(path))
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                size = new Vector2(image.Width, image.Height);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new Texture(handle, size);
        }
    }
}
