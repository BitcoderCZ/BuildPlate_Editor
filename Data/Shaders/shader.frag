#version 460 core

out vec4 FragColor;

in vec3 fUv;

layout (location = 5, binding = 1) uniform sampler2DArray textures;

void main()
{
    vec4 col = texture(textures, fUv, 0);
    
    if(col.a == 0.0)
    {
      discard;
    }

    FragColor = col;
}