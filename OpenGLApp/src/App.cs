using System;
using System.Numerics;
using System.Collections.Generic;

using static CSGL.CSGL;   // csgl*
using static CSGL.Glfw3;  // glfw*
using static CSGL.OpenGL; // gl*
using System.IO;
using OpenGLApp.src.Graphics;
using OpenGLApp.src.Graphics.Buffers;
using OpenGLApp.src.Graphics.Shaders;
using OpenGLApp.src.Graphics.Window;

namespace OpenGLApp
{
    public class App
    {
        private const float sin60 = 0.866025388240814208984375f;
        static App singleton = null;

        int width, height;
        float fov;
        Stack<Matrix4x4> matrixStack;
        Matrix4x4 view;
        Matrix4x4 world;
        Window window;

        public static void Main(params string[] args)
        {
            Console.WriteLine(File.Exists("resources/models/triforce1.obj"));
            new App().Start(args);
        }

        public App()
        {
            if (singleton == null)
                singleton = this;
        }


        public void Start(params string[] args)
        {
            if (this != singleton)
                return;

            fov = (float)Math.PI / 2;
            string title = "CSGL Window";

            width = 1600;
            height = 900;
            List<Vertex> vertexList = new List<Vertex>
            {
                new Vertex(1, 0, 0, 0, 0, 1, 0, 0),
                new Vertex(0, 1, 0, 0, 0, 1, 0, 0),
                new Vertex(0, 0, 1, 0, 0, 1, 0, 0),

            };
#if false
            string path = @"resources/models/triforce1.obj";
            if (!File.Exists(path))
                return;

            string[] lines = File.ReadAllLines(path);


            List<float> verts = new List<float>();
            List<float> uv = new List<float>();
            List<float> norms = new List<float>();
            List<float> faces = new List<float>();
            List<int> vertexIndices = new List<int>();
            List<int> uvIndices = new List<int>();
            List<int> normalIndices = new List<int>();

            foreach(string line in lines)
            {
                Console.WriteLine($"Parsing: {line}");
                string[] array = line.Split(' ');
                
                switch (array[0])
                {
                    case "v":
                        verts.Add(float.Parse(array[1]));
                        verts.Add(float.Parse(array[2]));
                        verts.Add(float.Parse(array[3]));
                        break;
                    case "vt":
                        uv.Add(float.Parse(array[1]));
                        uv.Add(float.Parse(array[2]));
                        break;
                    case "vn":
                        norms.Add(float.Parse(array[1]));
                        norms.Add(float.Parse(array[2]));
                        norms.Add(float.Parse(array[3]));
                        break;
                    case "f":
                        for (int i = 1; i < array.Length; ++i)
                        {
                            string[] array2 = array[i].Split('/');
                            vertexIndices.Add(int.Parse(array2[0]));
                            uvIndices.Add(int.Parse(array2[1]));
                            normalIndices.Add(int.Parse(array2[2]));
                        }
                        break;
                    default:
                        break;
                }
            }
            float[] vertices = verts.ToArray();

            int[] indices = vertexIndices.ToArray();
#else
            float[] vertices =
            {
                -1, -1,  1,
                 1, -1,  1,
                 1,  1,  1,
                -1,  1,  1,

                -1, -1, -1,
                 1, -1, -1,
                 1,  1, -1,
                -1,  1, -1,
            };


            int[] indices =
            {
                0, 1, 2, 2, 3, 0,
                4, 7, 6, 6, 5, 4,
                4, 0, 3, 3, 7, 4,
                1, 5, 6, 6, 2, 1,
                3, 2, 6, 6, 7, 3,
                0, 4, 5, 5, 1, 0,
            };
#endif
            //glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4); // Change this to your targeted major version
            //glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 5); // Change this to your targeted minor version

            if (args.Length > 0)
                title = args[0];
            window = new Window(width, height, title, monitor: NULL, share: NULL);

            glClearColor(0.125f, 0.125f, 0.125f, 1);

            glEnable(GL_DEPTH_TEST);
            glEnable(GL_CULL_FACE);

            Matrix4x4 model = Matrix4x4.Identity;
            view = Matrix4x4.CreateTranslation(0, 0, -3);
            world = Matrix4x4.CreatePerspectiveFieldOfView(fov, width / (float)height, 0.125f, 1024f);
            matrixStack = new Stack<Matrix4x4>();

            ChangeFov(fov);

            uint positionLocation = 0;
            //uint normalLocation = 0;

            string vert = @"#version 450

layout(binding = 1) uniform UniformBufferObject {
    mat4 model;
    mat4 view;
    mat4 proj;
} ubo;

layout(location = 0) in vec4 inPosition;
layout(location = 1) in vec3 inNormal;
layout(location = 2) in vec2 inTexCoord;

layout(location = 0) out vec3 fragNormal;
layout(location = 1) out vec2 fragTexCoord;
layout(location = 2) out vec3 fragPosition;

void main() {
    //mat4 ident = mat4(1.0);
    //gl_Position = ubo.proj * ubo.view * ubo.model * vec4(inPosition, 1.0);
    fragTexCoord = inPosition.xy * 0.5 + 0.5;
	//fragNormal = (ubo.view * ubo.model * vec4(inNormal * 0.5, 0.0)).xyz + 0.5;
	//fragNormal = inNormal * 0.5 + 0.5; 
    fragPosition = inPosition.xyz * 0.5 + 0.5;
    gl_Position = ubo.model * inPosition;

}
";

            string frag = @"#version 450

layout(location = 0) in vec3 fragNormal;
layout(location = 1) in vec2 fragTexCoord;
layout(location = 2) in vec3 fragPosition;

layout(location = 0) out vec4 outColor;

void main() {
    //float intensity = dot(fragNormal, vec3(0, 0.7071, 0.7071));
    //outColor = intensity * vec4(0.75, 0.7, 0.1, 1);
	//outColor = vec4(fragNormal, 1.0);
    //outColor = vec4(1.0);
    outColor = vec4(fragPosition, 1.0);
}";

            ShaderProgram program = new ShaderProgram(vert, frag);

            glUseProgram(program.Id);

            VertexArray vao = new VertexArray();
            vao.Bind();

            StaticArrayBuffer vertexBuffer = new StaticArrayBuffer(vertices);
            StaticElementArrayBuffer indexBuffer = new StaticElementArrayBuffer(indices);

            positionLocation = (uint) glGetAttribLocation(program.Id, "inPosition");
            glEnableVertexAttribArray(positionLocation);
            glVertexAttribPointer(positionLocation, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 3, NULL);

            uint ubo_location = glGetUniformBlockIndex(program.Id, "UniformBufferObject");

            glUniformBlockBinding(program.Id, ubo_location, 0);

            uint ubo = 0;
            glGenBuffers(1, ref ubo);

            glBindBuffer(GL_UNIFORM_BUFFER, ubo);
            glBufferData(GL_UNIFORM_BUFFER, 48 * sizeof(float), NULL, GL_DYNAMIC_DRAW);

            glBindBufferRange(GL_UNIFORM_BUFFER, 0, ubo, 0, 48 * sizeof(float));

            Console.WriteLine($"Vertex size: {Vertex.SIZE_BYTES}");
            Console.WriteLine($"float size: {sizeof(float)}");


            glfwSetWindowSizeCallback(window.Id, (IntPtr _, int w, int h) => singleton.ResizeWindow(w, h));


            unsafe
            {
                Matrix4x4 world = this.world;
                Matrix4x4 view = this.view;
                glBufferSubData(GL_UNIFORM_BUFFER, 0 * sizeof(float), 16 * sizeof(float), new IntPtr(&model.M11));
                glBufferSubData(GL_UNIFORM_BUFFER, 16 * sizeof(float), 16 * sizeof(float), new IntPtr(&view.M11));
                glBufferSubData(GL_UNIFORM_BUFFER, 32 * sizeof(float), 16 * sizeof(float), new IntPtr(&world.M11));
            }

            float deltaAngle = 1 / 1024f;
            Matrix4x4 changer = Matrix4x4.CreateRotationY(deltaAngle);

            float x = 2 * sin60;
            //float angle = 0;

            while (glfwWindowShouldClose(window.Id) == 0)
            {
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                //model *= Matrix4x4.CreateRotationX(deltaAngle);
                //model *= Matrix4x4.CreateRotationZ(deltaAngle);
                model *= Matrix4x4.CreateRotationY(deltaAngle);
                Matrix4x4 mvp = model * matrixStack.Peek();

                unsafe
                {
                    glBufferSubData(GL_UNIFORM_BUFFER, 0 * sizeof(float), 16 * sizeof(float), new IntPtr(&mvp.M11));
                }

                glColor3b(0x66, 0x00, 0);
                //glLoadIdentity();
                //glScalef(0.5f * height / (float)width, 0.5f, 0.5f);
                //glRotatef(angle, 0, 1, 0);
                //angle += deltaAngle;

                glDrawElements(GL_TRIANGLES, indices.Length, GL_UNSIGNED_INT, NULL);


                //glScalef(sin60, sin60, sin60);

                glColor3b(0, 0x66, 0);
                glBegin(GL_TRIANGLES);

                glVertex3f(0, 2, -1.125f);
                glVertex3f(x, -1, -1.125f);
                glVertex3f(-x, -1, -1.125f);

                glVertex3f(0, 2, 1.25f);
                glVertex3f(-x, -1, 1.25f);
                glVertex3f(x, -1, 1.25f);

                glVertex3f(0, -2, 1.125f);
                glVertex3f(x, 1, 1.125f);
                glVertex3f(-x, 1, 1.125f);

                glEnd();

                glfwSwapBuffers(window.Id);
                glfwPollEvents();
            }

            glfwTerminate();
        }


        private void ResizeWindow(int w, int h)
        {
            width = w;
            height = h;
            ResetMatrixStack();
        }

        private void ResetMatrixStack()
        {
            world = Matrix4x4.CreatePerspectiveFieldOfView(fov, width / (float)height, 0.125f, 1024f);
            matrixStack.Clear();
            matrixStack.Push(Matrix4x4.Identity);
            matrixStack.Push(world * matrixStack.Peek());
            matrixStack.Push(view * matrixStack.Peek());
            glViewport(0, 0, width, height);
        }

        private void ChangeFov(float fov)
        {
            this.fov = fov;
            ResetMatrixStack();
        }
    }
}
