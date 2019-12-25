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
        private const float sin60_2 = sin60 / 2f;
        static App singleton = null;

        int width, height;
        float fov;
        Stack<Matrix4x4> matrixStack;
        Matrix4x4 view;
        Matrix4x4 world;
        Window window;

        public static int Main(params string[] args)
        {
            Console.WriteLine(File.Exists("resources/models/triforce1.obj"));
            return new App().Start(args);
        }

        public App()
        {
            if (singleton == null)
                singleton = this;
        }


        public int Start(params string[] args)
        {
            if (this != singleton)
                return -1;

            fov = (float)Math.PI / 3;
            string title = "CSGL Window";

            width = 1600;
            height = 900;

            //glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4); // Change this to your targeted major version
            //glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 5); // Change this to your targeted minor version

            if (args.Length > 0)
                title = args[0];
            window = new Window(width, height, title, monitor: NULL, share: NULL);

            glClearColor(0.125f, 0.125f, 0.125f, 1);

            glEnable(GL_DEPTH_TEST);
            glEnable(GL_CULL_FACE);

            Matrix4x4 model = Matrix4x4.Identity;
            view = Matrix4x4.CreateTranslation(0, 0, -2);
            world = Matrix4x4.CreatePerspectiveFieldOfView(fov, width / (float)height, 0.125f, 1024f);
            matrixStack = new Stack<Matrix4x4>();
            matrixStack.Push(Matrix4x4.Identity);

            ChangeFov(fov);
            string vertPath = @"resources/shaders/shader.vert";
            string fragPath = @"resources/shaders/shader.frag";
            if (!File.Exists(vertPath) || !File.Exists(fragPath))
            {
                Console.WriteLine("Cannot read shader files");
                return -1;
            }
            string vert = File.ReadAllText(vertPath);

            string frag = File.ReadAllText(fragPath);

            string texPath = @"resources/textures/colorgrid512.bmp";
            if (!File.Exists(texPath))
            {
                Console.WriteLine("Cannot read texture file");
                return -1;
            }


            ShaderProgram program = new ShaderProgram(vert, frag);

            program.Bind();

            VertexArray vao = new VertexArray();
            vao.Bind();

            uint positionLocation = (uint)glGetAttribLocation(program.Id, "inPosition");
            uint normalLocation = (uint)glGetAttribLocation(program.Id, "inNormal");
            uint texLocation = (uint)glGetAttribLocation(program.Id, "inTexCoord");

            float z = 0.0625f;
            float t = z * 2;
            List<Vertex> vertexList = new List<Vertex>() {
                new Vertex( 0, 1, z,                0, 0,  1,           0.5f,                   1),
                new Vertex(-sin60_2, 0.25f, z,      0, 0,  1,           0.5f - sin60_2 * 0.5f,  0.625f),
                new Vertex( sin60_2, 0.25f, z,      0, 0,  1,           0.5f + sin60_2 * 0.5f,  0.625f),
                new Vertex(-sin60, -0.5f, z,        0, 0,  1,           0.5f - sin60_2,         0.25f),
                new Vertex( 0, -0.5f, z,            0, 0,  1,           0.5f,                   0.25f),
                new Vertex(sin60, -0.5f, z,         0, 0,  1,           0.5f + sin60_2,         0.25f),

                new Vertex( 0, 1, -z,               0, 0, -1,           0.5f,                   1),
                new Vertex( sin60_2, 0.25f, -z,     0,  0, -1,          0.5f - sin60_2 * 0.5f,  0.625f),
                new Vertex(-sin60_2, 0.25f, -z,     0,  0, -1,          0.5f + sin60_2 * 0.5f,  0.625f),
                new Vertex( sin60, -0.5f, -z,       0, 0, -1,           0.5f - sin60_2,         0.25f),
                new Vertex( 0, -0.5f, -z,           0, 0, -1,           0.5f,                   0.25f),
                new Vertex(-sin60, -0.5f, -z,       0, 0, -1,           0.5f + sin60_2,         0.25f),

                new Vertex(-sin60, -0.5f, -z,       -0.5f, sin60, 0,    0, 0),
                new Vertex(-sin60, -0.5f, z,        -0.5f, sin60, 0,    z, 0),
                new Vertex( 0, 1, z,                -0.5f, sin60, 0,    z, sin60),
                new Vertex( 0, 1, -z,               -0.5f, sin60, 0,    0, sin60),

                new Vertex(sin60, -0.5f, z,         0.5f, sin60, 0,     0, 0),
                new Vertex(sin60, -0.5f, -z,        0.5f, sin60, 0,     z, 0),
                new Vertex(0, 1, -z,                0.5f, sin60, 0,     z, sin60),
                new Vertex(0, 1, z,                 0.5f, sin60, 0,     0, sin60),

                new Vertex(-sin60, -0.5f, z,        0, -1,  0,          0, 0),
                new Vertex(-sin60, -0.5f, -z,       0, -1,  0,          z, 0),
                new Vertex( sin60, -0.5f, -z,       0, -1,  0,          z, sin60),
                new Vertex(sin60, -0.5f, z,         0, -1,  0,          0, sin60),

                new Vertex(-sin60_2, 0.25f, z,      0, -1,  0,          0, 0),
                new Vertex(-sin60_2, 0.25f, -z,     0, -1,  0,          z, 0),
                new Vertex( sin60_2, 0.25f, -z,     0, -1,  0,          z, sin60_2),
                new Vertex( sin60_2, 0.25f, z,      0, -1,  0,          0, sin60_2),
                                                                            
                new Vertex( 0, -0.5f, z,            0.5f, sin60, 0,     0, 0),
                new Vertex( 0, -0.5f, -z,           0.5f, sin60, 0,     z, 0),
                new Vertex(-sin60_2, 0.25f, -z,     0.5f, sin60, 0,     z, sin60_2),
                new Vertex(-sin60_2, 0.25f, z,      0.5f, sin60, 0,     0, sin60_2),
                                                                            
                new Vertex(0, -0.5f, -z,            -0.5f, sin60, 0,    0, 0),
                new Vertex(0, -0.5f, z,             -0.5f, sin60, 0,    z, 0),
                new Vertex(sin60_2, 0.25f, z,       -0.5f, sin60, 0,    z, sin60_2),
                new Vertex(sin60_2, 0.25f, -z,      -0.5f, sin60, 0,    0, sin60_2),
            };

            int[] indices = {
                0, 1, 2,
                1, 3, 4,
                2, 4, 5,
                6, 7, 8,
                7, 9, 10,
                8, 10, 11,

                12, 13, 14, 14, 15, 12,
                16, 17, 18, 18, 19, 16,
                20, 21, 22, 22, 23, 20,
                24, 25, 26, 26, 27, 24,
                28, 29, 30, 30, 31, 28,
                32, 33, 34, 34, 35, 32,
            };

            var interleaved = new StaticInterleavedVertexBuffer(vertexList);

            glEnableVertexAttribArray(positionLocation);
            glVertexAttribPointer(positionLocation, 3, GL_FLOAT, GL_FALSE, Vertex.SIZE_BYTES, NULL);

            glEnableVertexAttribArray(normalLocation);
            glVertexAttribPointer(normalLocation, 3, GL_FLOAT, GL_FALSE, Vertex.SIZE_BYTES, new IntPtr(3 * sizeof(float)));

            glEnableVertexAttribArray(texLocation);
            glVertexAttribPointer(texLocation, 2, GL_FLOAT, GL_FALSE, Vertex.SIZE_BYTES, new IntPtr(6 * sizeof(float)));

            StaticElementArrayBuffer indexBuffer = new StaticElementArrayBuffer(indices);

            uint ubo_location = glGetUniformBlockIndex(program.Id, "UniformBufferObject");

            glUniformBlockBinding(program.Id, ubo_location, 0);

            uint ubo = 0;
            glGenBuffers(1, ref ubo);

            glBindBuffer(GL_UNIFORM_BUFFER, ubo);
            glBufferData(GL_UNIFORM_BUFFER, 48 * sizeof(float), NULL, GL_DYNAMIC_DRAW);

            glBindBufferRange(GL_UNIFORM_BUFFER, 0, ubo, 0, 48 * sizeof(float));

            glfwSetWindowSizeCallback(window.Id, (IntPtr _, int w, int h) => singleton.ResizeWindow(w, h));

            unsafe
            {
                Matrix4x4 world = this.world;
                Matrix4x4 view = this.view;
                Matrix4x4 matrixTop = model * matrixStack.Peek();
                glBufferSubData(GL_UNIFORM_BUFFER, 0 * sizeof(float), 16 * sizeof(float), new IntPtr(&matrixTop.M11));
                glBufferSubData(GL_UNIFORM_BUFFER, 16 * sizeof(float), 16 * sizeof(float), new IntPtr(&view.M11));
                glBufferSubData(GL_UNIFORM_BUFFER, 32 * sizeof(float), 16 * sizeof(float), new IntPtr(&world.M11));
            }

            byte[] byteArray = File.ReadAllBytes(texPath);
            Console.WriteLine($"byte array length: {byteArray.Length}");

            uint texture = 0;
            glGenTextures(1, ref texture);
            Console.WriteLine($"Texture ID: {texture}");
            glBindTexture(GL_TEXTURE_2D, texture);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, (int)GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, (int)GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_LINEAR);
            int textureWidth = 512;
            int textureHeight = 512;
            try
            {
                unsafe
                {
                    fixed (byte* bytePtr = &byteArray[0])
                        glTexImage2D(GL_TEXTURE_2D, 0, (int)GL_RGB, textureWidth, textureHeight, 0, GL_RGB, GL_UNSIGNED_BYTE, new IntPtr(bytePtr));
                }
            }
            catch (AccessViolationException)
            {

            }

            //float deltaAngle = 1 / 1f;

            //float angle = 0;

            z /= 2;

            double t0 = glfwGetTime();

            while (glfwWindowShouldClose(window.Id) == 0)
            {
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                double t1 = glfwGetTime();

                float deltaTime = (float)(t1 - t0);
                t0 = t1;

                model *= Matrix4x4.CreateRotationX(deltaTime);
                //model *= Matrix4x4.CreateRotationY(deltaTime);
                //model *= Matrix4x4.CreateRotationZ(deltaTime);
                //matrix4x4 matrixtop = model * matrixstack.peek();

                program.Unbind();
                glLoadIdentity();
                glTranslatef(0, 0, 1 - z);
                glScalef(height / (float)width, 1, 1);
                //glRotatef(angle, 0, 0, 1);
                //angle += deltaTime;

                glColor3b(0, 0x22, 0x11);
                glBegin(GL_TRIANGLES);

                glVertex3f(0, 1, 0);
                glVertex3f(sin60, -0.5f, 0);
                glVertex3f(-sin60, -0.5f, 0);

                glVertex3f(0, 1, 0);
                glVertex3f(-sin60, -0.5f, 0);
                glVertex3f(sin60, -0.5f, 0);

                glEnd();


                program.Bind();
                unsafe
                {
                    glBufferSubData(GL_UNIFORM_BUFFER, 0 * sizeof(float), 16 * sizeof(float), new IntPtr(&model.M11));
                }
                //glLoadIdentity();
                glColor3b(0x66, 0x00, 0);
                //glLoadIdentity();
                //glScalef(0.5f * height / (float)width, 0.5f, 0.5f);
                //glRotatef(angle, 0, 1, 0);
                //angle += deltaAngle;

                glDrawElements(GL_TRIANGLES, indices.Length, GL_UNSIGNED_INT, NULL);
                

                glfwSwapBuffers(window.Id);
                glfwPollEvents();
            }

            glfwTerminate();
            return 0;
        }


        private void ResizeWindow(int w, int h)
        {
            width = w;
            height = h;
            RefreshWorldMatrix();
        }

        private void RefreshWorldMatrix()
        {
            world = Matrix4x4.CreatePerspectiveFieldOfView(fov, width / (float)height, 0.125f, 1024f);
            glViewport(0, 0, width, height);

            unsafe
            {
                fixed (float* worldPtr = &world.M11)
                    glBufferSubData(GL_UNIFORM_BUFFER, 32 * sizeof(float), 16 * sizeof(float), new IntPtr(worldPtr));
            }
        }

        private void ChangeFov(float fov)
        {
            this.fov = fov;
            RefreshWorldMatrix();
        }
    }
}
