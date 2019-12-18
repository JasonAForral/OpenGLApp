using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using static CSGL.CSGL;   // csgl*
using static CSGL.Glfw3;  // glfw*
using static CSGL.OpenGL; // gl*
using System.Text;
using System.Linq;
using OpenGLApp.src.Graphics.Shaders;

namespace ConsoleApp1
{
    public class Program
    {
        private const float sin60 = 0.866025388240814208984375f;
        static bool err = false;
        static Program singleton = null;

        int width, height;
        float fov;
        Stack<Matrix4x4> matrixStack;
        Matrix4x4 view;
        Matrix4x4 world;

        public static void Main(params string[] args)
        {
            new Program().Start(args);
        }

        public Program()
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

            };
#endif
            csglLoadGlfw();
            glfwInit();
            //glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4); // Change this to your targeted major version
            //glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 5); // Change this to your targeted minor version

            if (args.Length > 0)
                title = args[0];

            IntPtr window = glfwCreateWindow(width, height, title, NULL, NULL);
            if (window == null)
                return;
            glfwMakeContextCurrent(window);
            csglLoadGL();

            //glClearColor(0.125f, 0.125f, 0.125f, 0.5f);

            glEnable(GL_DEPTH_TEST);
            glEnable(GL_CULL_FACE);

            Matrix4x4 model = Matrix4x4.Identity;
            view = Matrix4x4.CreateTranslation(0, 0, -3);
            world = Matrix4x4.CreatePerspectiveFieldOfView(fov, width / (float)height, 0.125f, 1024f);
            matrixStack = new Stack<Matrix4x4>();

            matrixStack.Push(Matrix4x4.Identity);
            matrixStack.Push(world * matrixStack.Peek());
            matrixStack.Push(view * matrixStack.Peek());

            uint vao = 0;
            uint vertexBuffer = 0;
            uint indexBuffer = 0;
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

            uint program = glCreateProgram();
            uint vertexShader = new Shader(GL_VERTEX_SHADER, vert).Id;
            uint fragShader = new Shader(GL_FRAGMENT_SHADER, frag).Id;

            //fixed (char* fragPtr = frag)
            //{
            //    uint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            //}
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragShader);

            if (vertexShader == 0 || fragShader == 0)
                return;

            glLinkProgram(program);

            glValidateProgram(program);

            int isLinked = 0;

            glGetProgramiv(program, GL_LINK_STATUS, ref isLinked);
            if (isLinked == 0)
            {
                int loglen = 0;
                glGetProgramiv(program, GL_INFO_LOG_LENGTH, ref loglen);

                IntPtr log = Marshal.AllocHGlobal(loglen);

                glGetProgramInfoLog(program, loglen, ref loglen, log);

                char[] chrs = new char[loglen];
                string logs = null;
                Marshal.Copy(log, chrs, 0, loglen);
                logs.Concat(chrs.AsEnumerable());
                Marshal.FreeHGlobal(log);
                Console.Error.WriteLine(logs);

                err = true;
                return;
            }

            glUseProgram(program);

            glGenVertexArrays(1, ref vao);
            glBindVertexArray(vao);

            glGenBuffers(1, ref vertexBuffer);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);

            unsafe
            {
                fixed (float* vertexPtr = vertices)
                {
                    glBufferData(GL_ARRAY_BUFFER, vertices.Length * sizeof(float), new IntPtr(vertexPtr), GL_STATIC_DRAW);
                }
            }

            glGenBuffers(1, ref vertexBuffer);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);

            unsafe
            {
                fixed (float* vertexPtr = vertices)
                {
                    glBufferData(GL_ARRAY_BUFFER, vertices.Length * sizeof(float), new IntPtr(vertexPtr), GL_STATIC_DRAW);
                }
            }

            glGenBuffers(1, ref indexBuffer);
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffer);
            unsafe
            {
                fixed (int* indexPtr = indices)
                {
                    glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.Length * sizeof(int), new IntPtr(indexPtr), GL_STATIC_DRAW);
                }
            }

            positionLocation = (uint) glGetAttribLocation(program, "inPosition");
            glEnableVertexAttribArray(positionLocation);
            glVertexAttribPointer(positionLocation, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 3, NULL);

            uint ubo_location = glGetUniformBlockIndex(program, "UniformBufferObject");

            glUniformBlockBinding(program, ubo_location, 0);

            uint ubo = 0;
            glGenBuffers(1, ref ubo);

            glBindBuffer(GL_UNIFORM_BUFFER, ubo);
            glBufferData(GL_UNIFORM_BUFFER, 48 * sizeof(float), NULL, GL_DYNAMIC_DRAW);

            glBindBufferRange(GL_UNIFORM_BUFFER, 0, ubo, 0, 48 * sizeof(float));

            glfwSetWindowSizeCallback(window, WindowSizeCallback);


            unsafe
            {
                Matrix4x4 world = this.world;
                Matrix4x4 view = this.view;
                glBufferSubData(GL_UNIFORM_BUFFER, 0 * sizeof(float), 16 * sizeof(float), new IntPtr(&model.M11));
                glBufferSubData(GL_UNIFORM_BUFFER, 16 * sizeof(float), 16 * sizeof(float), new IntPtr(&view.M11));
                glBufferSubData(GL_UNIFORM_BUFFER, 32 * sizeof(float), 16 * sizeof(float), new IntPtr(&world.M11));
            }

            float inverseAspect = height / (float)width;
            float x = sin60 * inverseAspect;
            float deltaAngle = 1 / 1024f;
            Matrix4x4 changer = Matrix4x4.CreateRotationY(deltaAngle);

            while (glfwWindowShouldClose(window) == 0)
            {
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                model *= Matrix4x4.CreateRotationY(deltaAngle);
                Matrix4x4 mvp = model * matrixStack.Peek();

                unsafe
                {
                    glBufferSubData(GL_UNIFORM_BUFFER, 0 * sizeof(float), 16 * sizeof(float), new IntPtr(&mvp.M11));
                }


                glColor3f(0.1875f, 0.375f, 0.5625f);
                glDrawElements(GL_TRIANGLES, indices.Length, GL_UNSIGNED_INT, NULL);

                //glScalef(sin60, sin60, sin60);

                //glRotatef(angle * 2, 0, 0, 1);
                //angle += deltaAngle;

                glBegin(GL_TRIANGLES);

                glColor3f(1, 0, 0);
                glVertex3f(0, 1, -0.0625f);
                glColor3f(0, 1, 0);
                glVertex3f(-x, -0.5f, -0.0625f);
                glColor3f(0, 0, 1);
                glVertex3f(x, -0.5f, -0.0625f);

                glColor3f(1, 0, 0);
                glVertex3f(0, 1, 0.0625f);
                glColor3f(0, 0, 1);
                glVertex3f(x, -0.5f, 0.0625f);
                glColor3f(0, 1, 0);
                glVertex3f(-x, -0.5f, 0.0625f);

                glEnd();

                glfwSwapBuffers(window);
                glfwPollEvents();
            }

            //Terminal terminal = new Terminal();
            //terminal.Start();
            //// User user = new User();
            //if (args != null)
            //    foreach(string arg in args) {
            //        Console.WriteLine(arg);
            //}
            glfwTerminate();
        }



        private static uint CreateShader(string shaderScript, uint shaderType)
        {
#if true
            uint shaderID = glCreateShader(shaderType);
            IntPtr scriptPtr = Marshal.AllocHGlobal(shaderScript.Length);
            Marshal.Copy(Encoding.ASCII.GetBytes(shaderScript), 0, scriptPtr, shaderScript.Length);

            int length = shaderScript.Length;

            glShaderSource(shaderID, 1, ref scriptPtr, ref length);
            glCompileShader(shaderID);

            Marshal.FreeHGlobal(scriptPtr);

            int isCompiled = 0;

            glGetShaderiv(shaderID, GL_COMPILE_STATUS, ref isCompiled);

            if (isCompiled == 0)
            {
                int loglen = 0;
                glGetShaderiv(shaderID, GL_INFO_LOG_LENGTH, ref loglen);

                IntPtr log = Marshal.AllocHGlobal(loglen);

                glGetShaderInfoLog(shaderID, loglen, ref loglen, log);

                char[] chrs = new char[loglen];
                string logs = "";
                Marshal.Copy(log, chrs, 0, loglen);
                logs.Concat(chrs.AsEnumerable());
                Marshal.FreeHGlobal(log);
                Console.Error.WriteLine(logs);

                err = true;
                return 0;
            }
            return shaderID;
#else
            unsafe
            {
                char[] charArray = shaderScript.ToCharArray();
                uint shaderID = glCreateShader(shaderType);
                fixed (char* vertPtr = charArray)
                {

                    IntPtr ptr = new IntPtr(vertPtr);
                    int length = charArray.Length;
                    glShaderSource(shaderID, 1, ref ptr, ref length);
                    glCompileShader(shaderID);
                    Console.WriteLine($"null terminated: {charArray[^1] == 0}");
                    Console.WriteLine($"length of string: {shaderScript.Length}");
                    Console.WriteLine($"length of length: {length}");

                    int result = 0;
                    glGetShaderiv(shaderID, GL_COMPILE_STATUS, ref result);
                    if (GL_FALSE == result)
                    {
                        //int lengthA;
                        length = 0;
                        glGetShaderiv(shaderID, GL_INFO_LOG_LENGTH, ref length);
                        Console.WriteLine($"length {length}");
                        char[] message = new char[length];
                        fixed (char* msgPtr = message)
                        {
                            ptr = new IntPtr(msgPtr);
                            glGetShaderInfoLog(shaderID, length * sizeof(char), ref length, ptr);
                            Console.WriteLine("Failed to compile shader!");
                            Console.WriteLine(message);
                        }
                        glDeleteShader(shaderID);
                        return 0;
                    }

                }
                return shaderID;
            }
#endif
        }

        private static void WindowSizeCallback(IntPtr _, int w, int h)
        {
            singleton.ResizeWindow(w, h);
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
