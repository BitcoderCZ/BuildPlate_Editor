#version 450 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aUv;

layout (location = 0) out vec2 fUv;

uniform mat4 uTransform;

void main()
{
	fUv = aUv;
    gl_Position = uTransform * vec4(aPosition, 1.0);
}