#version 330

in vec3 fragNormal;
in vec2 fragTexCoord;

uniform sampler2D uTexture;

out vec4 outColor;

void main() {
    float intensity;
    intensity = dot(fragNormal, vec3(0, 0.7071, 0.7071));
    //intensity = dot(fragNormal, vec3(0, 0, 1));
    //outColor = intensity * vec4(fragTexCoord, 0, 1);
    //outColor = intensity * vec4(0.75, 0.75, 0.125, 1);
    //outColor = vec4(fragNormal, 1);
    //outColor = vec4(fragTexCoord, 0, 1);

    //outColor = intensity * vec4(fragTexCoord, 0, 1);

    outColor = intensity * texture(uTexture, fragTexCoord);
}