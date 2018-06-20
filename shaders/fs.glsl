#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 worldPos;

uniform float specularity;		// shininess for this current object //
uniform sampler2D pixels;		// texture sampler
uniform vec3 lightPos;			// hardcoded light //

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
	vec3 L = lightPos - worldPos.xyz;
	float dist = length(L);
	L = normalize( L );
	vec3 lightColor = vec3( 1, 1, 1 );
	vec3 CamPos = vec3( 0f, -4f, -15f );
	lightColor = normalize( lightColor );
	float brightness = 200;
	float ambientlight = 0.1f;
	vec3 materialColor = texture( pixels, uv ).xyz;
	float attenuation = 1.0f / (dist * dist);
	if (attenuation < 0)
	attenuation = 0;
	if (attenuation > 1)
	attenuation = 1;

	//specular calculation
	vec3 incidenceVector = -L;
	vec3 reflectionVector = reflect(incidenceVector, normal.xyz);
	reflectionVector = normalize(reflectionVector);
	vec3 surfaceToCamera = normalize(CamPos - worldPos.xyz);
	float cosAngle = max(0.0, dot(surfaceToCamera, reflectionVector));
	float specularCoefficient = pow(cosAngle, specularity);
	vec3 specularcolor = specularCoefficient * lightColor;

	outputColor = vec4( specularcolor + materialColor * (max( 0.0f, dot( L, normal.xyz ) ) * attenuation * lightColor * brightness * (1 - ambientlight) + ambientlight), 1 );
	//outputColor = texture( pixels, uv ) + 0.5f * vec4( normal.xyz, 1 );
}