#version 450 core

out vec4 FragColor;

in vec2 fUv;

layout (binding = 0) uniform sampler2D tex;

void main()
{
    vec4 col = texture(tex, fUv, 0);
    
    if(col.a == 0.0)
    {
      discard;
    }

    FragColor = col;
}