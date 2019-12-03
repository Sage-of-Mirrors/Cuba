#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNrm;

out vec3 Normal;

struct Light
{
	vec4 position;
	vec4 color;
	float attenuation;
	float ambientcoefficient;
};

layout (std140) uniform Matrices
{
    mat4 projection;
    mat4 view;
	mat4 model;
	vec4 color;
	Light light;
};

void main()
{
    gl_Position = projection * view * model * vec4(aPos, 1.0);
	Normal = aNrm;
}