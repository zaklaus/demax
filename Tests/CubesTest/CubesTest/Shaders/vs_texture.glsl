#version 330

in vec3 vPosition;
in vec2 texcoord;
out vec2 f_texcoord;
uniform mat4 M;
uniform mat4 V;
uniform mat4 P;
uniform mat4 MVP;

void
main()
{
	gl_Position = MVP * vec4(vPosition, 1.0);

	f_texcoord = texcoord;

}