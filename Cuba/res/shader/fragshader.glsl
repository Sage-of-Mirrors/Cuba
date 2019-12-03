#version 330 core

in vec3 Normal;

out vec4 fragColor;

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
	vec4 ambient_color = light.ambientcoefficient * color * light.color;

	vec3 nrm = normalize(transpose(inverse(mat3(model))) * Normal);

	float diffuseCoefficient = max(0.0, dot(nrm, -light.position.xyz));
    vec4 diffuse = diffuseCoefficient * color * light.color;

	fragColor = ambient_color + diffuse;
}