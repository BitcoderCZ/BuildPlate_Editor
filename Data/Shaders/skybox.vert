#version 450 core
layout (location = 0) in vec3 aPos;

out vec3 TexCoords;

uniform mat4 uTransform;
uniform mat4 uProjection;
uniform mat4 uView;

void main()
{
	TexCoords = aPos;
    gl_Position = uProjection * uView * (uTransform * vec4(aPos, 1.0));
}