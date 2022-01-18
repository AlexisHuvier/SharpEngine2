using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SE2.Graphics
{
    public class Shader
    {
        public readonly int Handle;

        private readonly Dictionary<string, int> _uniformLocation;

        public Shader(string vertSource, string fragSource)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertSource);
            CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragSource);
            CompileShader(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int nbUniforms);
            _uniformLocation = new Dictionary<string, int>();
            for(int i = 0; i < nbUniforms; i++)
            {
                string key = GL.GetActiveUniform(Handle, i, out _, out _);
                int location = GL.GetUniformLocation(Handle, key);
                _uniformLocation.Add(key, location);
            }
        }

        public static Shader FromFiles(string vertPath, string fragPath) => new Shader(File.ReadAllText(vertPath), File.ReadAllText(fragPath));

        public void Use() => GL.UseProgram(Handle);
        public int GetAttribLocation(string attribName) => GL.GetAttribLocation(Handle, attribName);

        public int GetUniformLocation(string uniform)
        {
            if (_uniformLocation.TryGetValue(uniform, out int location) == false)
            {
                location = GL.GetUniformLocation(Handle, uniform);
                _uniformLocation.Add(uniform, location);

                if (location == -1)
                    Trace.WriteLine($"[ERROR] The uniform '{uniform}' does not exist in the shader '{Handle}'!");
            }

            return location;
        }

        public void Unload() => GL.DeleteProgram(Handle);

        // Uniform setters - BEGIN

        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocation[name], data);
        }

        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocation[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocation[name], true, ref data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocation[name], data);
        }

        // Uniform setters - END

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int code);
            if(code != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Trace.WriteLine($"[ERROR] Error occured whilst compiling Shader({shader}).\n\n {infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int code);
            if (code != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Trace.WriteLine($"[ERROR] Error occured whilst linking Program({program}).\n\n {infoLog}");
            }
        }
    }
}
