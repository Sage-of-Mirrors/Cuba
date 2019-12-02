#version 330 core

out vec4 fragColor;

layout (std140) uniform Matrices
{
    mat4 projection;
    mat4 view;
	mat4 model;
	vec4 color;
};

void main()
{
	fragColor = color;
}