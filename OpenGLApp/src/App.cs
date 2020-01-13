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
using System.Text;
using OpenGLApp.src.Graphics.Textures;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OpenGLApp
{
    public class App
    {
        #region Fields
        private const float s = 0.8660254f;
        private const float t = s / 2f;
        private static App singleton = null;
        private int width, height;
        private float fov;
        private Stack<Matrix4x4> matrixStack;
        private Matrix4x4 view;
        private Matrix4x4 world;
        private Window window;
        #endregion

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

#if UNSAFE
            Console.WriteLine("Usafe are enabled");
#else
            Console.WriteLine("Usafe are disabled");
#endif
            #region Something
            if (this != singleton)
                return -1;

            fov = MathF.PI / 3;

            string title = "CSGL Window";

            width = 1600;
            height = 900;

            //glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4); // Change this to your targeted major version
            //glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 5); // Change this to your targeted minor version

            if (args.Length > 0)
                title = args[0];
            window = new Window(width, height, title, monitor: IntPtr.Zero, share: IntPtr.Zero);

            glClearColor(0.125f, 0.125f, 0.125f, 1);

            glEnable(GL_DEPTH_TEST);
            glEnable(GL_CULL_FACE);
            #endregion

            #region Matrix Init
            Matrix4x4 model = Matrix4x4.Identity;
            view = Matrix4x4.CreateTranslation(0, 0, -2);
            world = Matrix4x4.CreatePerspectiveFieldOfView(fov, width / (float)height, 0.125f, 1024f);
            matrixStack = new Stack<Matrix4x4>();
            matrixStack.Push(Matrix4x4.Identity);
            ChangeFov(fov);
            #endregion

            #region File Checking
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
            #endregion

            #region Shader and polygon init
            var program = new ShaderProgram(vert, frag);
            program.Bind();

            var vao = new VertexArray();
            vao.Bind();

            const float z = 0.0625f;
            int length;
            {
                var vertexList = new List<Vertex>() {
                    new Vertex( 0, 1, z,        0, 0,  1,       0.5f,             1),
                    new Vertex(-t, 0.25f, z,    0, 0,  1,       0.5f - t * 0.5f,  0.625f),
                    new Vertex( t, 0.25f, z,    0, 0,  1,       0.5f + t * 0.5f,  0.625f),
                    new Vertex(-s, -0.5f, z,    0, 0,  1,       0.5f - t,         0.25f),
                    new Vertex( 0, -0.5f, z,    0, 0,  1,       0.5f,             0.25f),
                    new Vertex(s, -0.5f, z,     0, 0,  1,       0.5f + t,         0.25f),

                    new Vertex( 0, 1, -z,       0, 0, -1,       0.5f,             1),
                    new Vertex( t, 0.25f, -z,   0, 0, -1,       0.5f - t * 0.5f,  0.625f),
                    new Vertex(-t, 0.25f, -z,   0, 0, -1,       0.5f + t * 0.5f,  0.625f),
                    new Vertex( s, -0.5f, -z,   0, 0, -1,       0.5f - t,         0.25f),
                    new Vertex( 0, -0.5f, -z,   0, 0, -1,       0.5f,             0.25f),
                    new Vertex(-s, -0.5f, -z,   0, 0, -1,       0.5f + t,         0.25f),

                    new Vertex(-s, -0.5f, -z,   -0.5f, s, 0,    0, 0),
                    new Vertex(-s, -0.5f, z,    -0.5f, s, 0,    z, 0),
                    new Vertex( 0, 1, z,        -0.5f, s, 0,    z, s),
                    new Vertex( 0, 1, -z,       -0.5f, s, 0,    0, s),

                    new Vertex(s, -0.5f, z,     0.5f, s, 0,     0, 0),
                    new Vertex(s, -0.5f, -z,    0.5f, s, 0,     z, 0),
                    new Vertex(0, 1, -z,        0.5f, s, 0,     z, s),
                    new Vertex(0, 1, z,         0.5f, s, 0,     0, s),

                    new Vertex(-s, -0.5f, z,    0, -1,  0,      0, 0),
                    new Vertex(-s, -0.5f, -z,   0, -1,  0,      z, 0),
                    new Vertex( s, -0.5f, -z,   0, -1,  0,      z, s),
                    new Vertex(s, -0.5f, z,     0, -1,  0,      0, s),

                    new Vertex(-t, 0.25f, z,    0, -1,  0,      0, 0),
                    new Vertex(-t, 0.25f, -z,   0, -1,  0,      z, 0),
                    new Vertex( t, 0.25f, -z,   0, -1,  0,      z, t),
                    new Vertex( t, 0.25f, z,    0, -1,  0,      0, t),

                    new Vertex( 0, -0.5f, z,    0.5f, s, 0,     0, 0),
                    new Vertex( 0, -0.5f, -z,   0.5f, s, 0,     z, 0),
                    new Vertex(-t, 0.25f, -z,   0.5f, s, 0,     z, t),
                    new Vertex(-t, 0.25f, z,    0.5f, s, 0,     0, t),

                    new Vertex(0, -0.5f, -z,    -0.5f, s, 0,    0, 0),
                    new Vertex(0, -0.5f, z,     -0.5f, s, 0,    z, 0),
                    new Vertex(t, 0.25f, z,     -0.5f, s, 0,    z, t),
                    new Vertex(t, 0.25f, -z,    -0.5f, s, 0,    0, t),
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

                length = indices.Length;

                var interleaved = new StaticInterleavedVertexBuffer(vertexList);

                uint positionLocation = program.GetLocation("inPosition");
                uint normalLocation = program.GetLocation("inNormal");
                uint texLocation = program.GetLocation("inTexCoord");

                glEnableVertexAttribArray(positionLocation);
                glVertexAttribPointer(positionLocation, 3, GL_FLOAT, GL_FALSE, Vertex.SIZE_BYTES, NULL);

                glEnableVertexAttribArray(normalLocation);
                glVertexAttribPointer(normalLocation, 3, GL_FLOAT, GL_FALSE, Vertex.SIZE_BYTES, new IntPtr(3 * sizeof(float)));

                glEnableVertexAttribArray(texLocation);
                glVertexAttribPointer(texLocation, 2, GL_FLOAT, GL_FALSE, Vertex.SIZE_BYTES, new IntPtr(6 * sizeof(float)));

                StaticElementArrayBuffer indexBuffer = new StaticElementArrayBuffer(indices);

                uint ubo_location = program.GetLocation("UniformBufferObject");
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

                var texture = new Texture(texPath);

                if (texture.Data == null)
                    return -1;
            }
            #endregion

            glfwSwapInterval(1);
            glLoadIdentity();
            glTranslatef(0, 0, 0.96875f);
            glScalef(height / (float)width, 1, 1);
            glColor3b(0, 0x22, 0x11);

            var speed = ((1 / 3.0f) / MathF.PI);
            var timer = Stopwatch.StartNew();

            Console.WriteLine($"High Resolution Stopwatch: {Stopwatch.IsHighResolution}");
            while (glfwWindowShouldClose(window.Id) == 0)
            {
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                var deltaTime = (float)timer.Elapsed.TotalSeconds;
                timer.Restart();

                //model *= Matrix4x4.CreateRotationX(deltaTime);
                model *= Matrix4x4.CreateRotationY(speed * deltaTime);
                //model *= Matrix4x4.CreateRotationZ(deltaTime);
                //matrix4x4 matrixtop = model * matrixstack.peek();

                #region Fallback Triangle
                program.Unbind();

                glBegin(GL_TRIANGLES);

                glVertex3f(0, 1, 0);
                glVertex3f(-s, -0.5f, 0);
                glVertex3f(s, -0.5f, 0);

                glEnd();
                #endregion


                program.Bind();

                unsafe
                {
                    glBufferSubData(GL_UNIFORM_BUFFER, 0 * sizeof(float), 16 * sizeof(float), new IntPtr(&model.M11));
                }

                glDrawElements(GL_TRIANGLES, length, GL_UNSIGNED_INT, NULL);


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
