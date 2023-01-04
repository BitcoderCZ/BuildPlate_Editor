#version 450 core

out vec4 FragColor;

layout (location = 0) in vec2 fUv;

layout (binding = 0) uniform sampler2D uTexture;

void main()
{
    FragColor = texture(uTexture, fUv);
}