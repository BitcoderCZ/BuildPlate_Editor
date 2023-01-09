#version 450 core

out vec4 FragColor;

in vec4 fCol;

void main()
{
    if(fCol.a == 0.0)
    {
      discard;
    }

    FragColor = fCol;
}