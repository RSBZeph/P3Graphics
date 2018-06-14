#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 worldPos;
uniform sampler2D pixels;		// texture sampler
uniform vec3 lightPos;			// hardcoded light //
uniform float specularity;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
	vec3 L = lightPos - worldPos.xyz;
	float dist = L.length();
	L = normalize( L );
	vec3 lightColor = vec3( 1, 1, 1 );
	lightColor = normalize( lightColor );
	float brightness = 20;
	float ambientlight = 0.1f;
	vec3 materialColor = texture( pixels, uv ).xyz;
	float attenuation = 1.0f / (dist * dist);

	//specular calculation
	float specularity = 40; // lager = meer wit
	vec3 incidenceVector = -L;
	vec3 reflectionVector = reflect(incidenceVector, normal.xyz);
	vec3 surfaceToCamera = normalize(vec3(0f, -4f, -15f) - worldPos.xyz);
	float cosAngle = min(0.0, dot(surfaceToCamera, reflectionVector));
	float specularCoefficient = pow(cosAngle, specularity);
	vec3 specularcolor = specularCoefficient * lightColor * brightness;

	outputColor = vec4( specularcolor + materialColor * (max( 0.0f, dot( L, normal.xyz ) ) * attenuation * lightColor * brightness * (1 - ambientlight) + ambientlight), 1 );
}