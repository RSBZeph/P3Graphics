#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 worldPos;

uniform float specularity;		// shininess for this current object //
uniform sampler2D pixels;		// texture sampler
uniform vec3 lightPos;			// hardcoded light //
uniform vec3 campos;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
	vec3 L = lightPos - worldPos.xyz;
	float dist = length(L);
	L = normalize( L );
	vec3 lightColor = vec3( 1, 1, 1 );
	lightColor = normalize( lightColor );
	float brightness = 500;
	float ambientfactor = 0.1f;
	vec3 materialColor = texture( pixels, uv ).xyz;

	float attenuation = 1.0f / (dist * dist);
	if (attenuation < 0)
	attenuation = 0;
	if (attenuation > 1)
	attenuation = 1;

	//specular calculation
	vec3 incidenceVector = -L;
	vec3 specularcolor;
	if (dot(incidenceVector, normal.xyz) < 0)
	{
	vec3 reflectionVector = reflect(incidenceVector, normal.xyz);
	reflectionVector = normalize(reflectionVector);
	vec3 surfaceToCamera = normalize(campos - worldPos.xyz);
	float cosAngle = max(0.0, dot(surfaceToCamera, reflectionVector));
	float specularCoefficient = pow(cosAngle, specularity);
	specularcolor = specularCoefficient * lightColor;
	}
	else
	specularcolor = vec3(0,0,0);

	// outputcolor = specular + materialcolor * (ambientfactor * ambient + (1-ambientfactor) * diffuse)
	outputColor = vec4( specularcolor + materialColor * (max( 0.0f, dot( L, normal.xyz ) ) * attenuation * lightColor * brightness * (1 - ambientfactor) + ambientfactor), 1 );
}