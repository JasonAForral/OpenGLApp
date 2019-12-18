using System;
using System.Collections.Generic;
using System.Text;
using static CSGL.CSGL;   // csgl*

namespace OpenGLApp.src.Graphics.Window
{
    class Window
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Title { get; set; }
        public IntPtr Share { get; set; }
        public IntPtr Monitor { get; set; }

        public Window(int width, int height, string title, IntPtr monitor, IntPtr share)
        {
            this.Width = width;
            this.Height = height;
            this.Title = title;
            this.Monitor = monitor;
            this.Share = share;
        }
    }
}
