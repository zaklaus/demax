#version 330

in vec3 vPosition;
in vec3 vColor;
out vec4 f_color;
void
main()
{
	gl_Position = vec4(vPosition, 1.0);

	f_color = vec4(vColor,1);

}