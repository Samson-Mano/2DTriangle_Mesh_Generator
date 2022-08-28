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
  public  class VertexBuffer
    {
        private int _m_renderer_id;

        public int m_rendered_id { get { return this._m_renderer_id; } }

        // size is byte
        public VertexBuffer(float[] data, int size)
        {
            // Main Constructor
            // Set up vertex buffer
            this._m_renderer_id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._m_renderer_id);
            GL.BufferData(BufferTarget.ArrayBuffer, size, data, BufferUsageHint.StaticDraw);

        }

        public void Bind()
        {
            // Bind buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._m_renderer_id);
        }

        public void UnBind()
        {
            // Unbind with 0
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Delete_VertexBuffer()
        {
            // Delete this buffer (acts like a  destructor)
            GL.DeleteBuffer(this._m_renderer_id);
        }
    }
}
