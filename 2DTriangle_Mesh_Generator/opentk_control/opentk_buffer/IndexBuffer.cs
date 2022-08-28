using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace _2DTriangle_Mesh_Generator.opentk_control.opentk_buffer
{
   public class IndexBuffer
    {
        private int _m_renderer_id;
        private int _m_count;

        public int GetCount { get { return this._m_count; } }

        // count is element count
        public IndexBuffer(uint[] data, int count)
        {
            // Main Constructor
            // Set up Index buffer
            this._m_count = count;
            this._m_renderer_id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._m_renderer_id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, count * sizeof(uint), data, BufferUsageHint.StaticDraw);
        }

        public void Bind()
        {
            // Bind buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._m_renderer_id);
        }

        public void UnBind()
        {
            // Unbind with 0
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Delete_IndexBuffer()
        {
            // Delete this buffer (acts like a  destructor)
            GL.DeleteBuffer(this._m_renderer_id);
        }
    }
}
