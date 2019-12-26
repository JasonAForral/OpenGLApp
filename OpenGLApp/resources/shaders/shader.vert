#version 330

uniform UniformBufferObject {
    mat4 model;
    mat4 view;
    mat4 proj;
} ubo;

in vec3 inPosition;
in vec3 inNormal;
in vec2 inTexCoord;

out vec3 fragNormal;
out vec2 fragTexCoord;

void main() {
    gl_Position = ubo.proj * ubo.view * ubo.model * vec4(inPosition, 1);
    fragTexCoord = inTexCoord;
    fragNormal = (ubo.view * ubo.model * vec4(inNormal * 0.5, 0)).xyz + 0.5;
}