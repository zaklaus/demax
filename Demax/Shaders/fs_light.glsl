#version 330

in vec2 f_texcoord;
in vec3 f_normal;
in vec4 ShadowCoord;

out vec4 outputColor;
in vec3 f_pos_wp;

uniform sampler2D maintexture;

const vec3 ambient = vec3(0.1, 0.1, 0.1);
const vec3 lightVec = vec3(2.0, 1.0, 1.5);
const vec3 lightColor = vec3(0.5, 0.5, 0.5);

void
main()
{
	vec2 flipped_texcoord = vec2(f_texcoord.x, 1.0 - f_texcoord.y);
	float diffuse = clamp(dot(normalize(lightVec), normalize(f_normal)), 0.0, 1.0);

    outputColor = texture(maintexture, flipped_texcoord);
    outputColor = outputColor * vec4(ambient + diffuse + lightColor, 1.0);
}