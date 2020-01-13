# CSGL
CSGL was created to provide a simple, low-level OpenGL loader for the OpenGL 1.0 thru 4.5 specification.
CSGL provides a P/Invoke wrapper for GLFW3 as well, as it uses it internally to fetch the process addresses.  

### Setup
 - Add `CSGL.cs` to your project.
 - Skip ahead to `Usage` if you would like to use **OpenGL 4.5**.
 - Open up `CSGL.cs` and start a multiline comment at **the version one above** the OpenGL version you would like to use.
 - End the multiline comment at the end (under the OpenGL 4.5 region).
 - Make sure `glfw3.dll` or `libglfw.so` is located in your working directory. \*
 - Make sure that your native libraries target the same platform as the .NET application.
 
 \* *For a slightly more advanced DLL loader, which also supports OSX, consider taking a look at [LTP.Interop](https://github.com/latet-party/LTP.Interop).*  
 *(does not include OpenGL macros/"abstraction").*
  
### Usage
 - (Optional) Add using directives:
```csharp
using static CSGL.CSGL;   // csgl*
using static CSGL.Glfw3;  // glfw*
using static CSGL.OpenGL; // gl*
```  
`using static` requires **C#6** or higher.
 - Create an OpenGL context:
```csharp
csglLoadGlfw();
glfwInit();
glfwWindowHint( GLFW_CONTEXT_VERSION_MAJOR, 4 ); // Change this to your targeted major version
glfwWindowHint( GLFW_CONTEXT_VERSION_MINOR, 5 ); // Change this to your targeted minor version

IntPtr window = glfwCreateWindow( 640, 480, "CSGL Window", IntPtr.Zero, IntPtr.Zero );

glfwMakeContextCurrent( window );
```  
  - Load OpenGL:  
```csharp
csglLoadGL();
```
You can now use OpenGL!

### Abstraction
CSGL provides minimal-overhead abstraction:
- `CSGLWindow` is an abstraction over GLFW windows.
- `CSGLSprite` is a 2D-sprite "object".
- `CSGLShader` is an abstracted vertex+fragment shader program.

CSGL provides several optimized macros as well.  
  
Some optimized methods **require** an `unsafe` context, compile with `UNSAFE` symbol to enable these methods, otherwise safe (often slower) alternatives will be used.

### Cross-platform
CSGL can be ran under Mono, this can be achieved by:
 - Having the latest GLFW3 binary in your executable directory, named `libglfw.so`

# GLFW
The GLFW GitGub repository can be found [here](https://github.com/glfw/glfw).  
The GLFW website can be found [here](http://www.glfw.org/).
