namespace SE2.Graphics
{
    class Shaders
    {
        // SHAPE SHADER
        public static string GetShapeShaderVertex()
        {
            return @"
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec4 aColor;

out vec4 color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    color = aColor;
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}";
        }

        public static string GetShapeShaderFragment()
        {
            return @"
#version 330 core

out vec4 outputColor;
in vec4 color;

uniform float alpha;

void main()
{
    outputColor = color;
    outputColor.a *= alpha;
}";
        }

        // SPRITE SHADER
        public static string GetBasicSpriteShaderVertex()
        {
            return @"
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    texCoord = aTexCoord;
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}";
        }

        public static string GetBasicSpriteShaderFragment()
        {
            return @"
#version 330 core

out vec4 outputColor;
in vec2 texCoord;

uniform sampler2D texture0;
uniform float alpha;

void main()
{
    outputColor = texture(texture0, texCoord);
    outputColor.a *= alpha;
    if (outputColor.a == 0) {
        discard;
    }
}";
        }

        // FONT SHADER
        public static string GetBasicFontShaderVertex()
        {
            return @"
#version 330 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
out vec2 TexCoords;

uniform mat4 model;
uniform mat4 projection;
uniform mat4 view;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    TexCoords = aTexCoord;
}";
        }

        public static string GetBasicFontShaderFragment()
        {
            return @"
#version 330 core
in vec2 TexCoords;
out vec4 color;

uniform sampler2D text;
uniform vec3 textColor;
uniform float alpha;

void main()
{    
    vec4 sampled = vec4(1.0, 1.0, 1.0, texture(text, TexCoords).r);
    color = vec4(textColor, 1.0) * sampled;
    color.a *= alpha;
    if (color.a == 0) {
        discard;
    }
}  ";
        }

        // IMGUI SHADER
        public static string GetImGUIShaderVertex()
        {
            return @"
#version 330 core

uniform mat4 projection_matrix;

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoord;
layout(location = 2) in vec4 in_color;

out vec4 color;
out vec2 texCoord;

void main()
{
    gl_Position = projection_matrix * vec4(in_position, 0, 1);
    color = in_color;
    texCoord = in_texCoord;
}";
        }

        public static string GetImGUIShaderFragment()
        {
            return @"
#version 330 core

uniform sampler2D in_fontTexture;

in vec4 color;
in vec2 texCoord;

out vec4 outputColor;

void main()
{
    outputColor = color * texture(in_fontTexture, texCoord);
}";
        }
    }
}
