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
   public class VertexArray
    {
        private int _m_renderer_id;


        public VertexArray()
        {
            this._m_renderer_id = GL.GenVertexArray();
            //   GL.BindVertexArray(this._m_renderer_id);
        }

        public void Add_vertexBuffer(VertexBuffer vb, List<VertexBufferLayout> layout_list)
        {
            // Add and Bind a vertex buffer to an appropriate layout
            // Vertex Buffer layout  contains the information about co-ordinates, normals, color etc

            Bind();
            vb.Bind();

            // Set up the layout here
            int offset = 0;

            for (int i = 0; i < layout_list.Count; i++)
            {
                VertexBufferLayout layout = layout_list[i];
                GL.EnableVertexAttribArray(i);
                GL.VertexAttribPointer(i, layout.count, VertexAttribPointerType.Float, false, layout.stride_size, offset);


                // Offset is the previous layout count (most likely the stride will remain the same)
                offset = offset + (layout.count * sizeof(float));
            }
        }

        public void Bind()
        {
            // Bind buffer
            GL.BindVertexArray(this._m_renderer_id);
        }

        public void UnBind()
        {
            // Unbind with 0
            GL.BindVertexArray(0);
        }

        public void Delete_VertexArray()
        {
            // Delete this buffer (acts like a  destructor)
            GL.DeleteVertexArray(this._m_renderer_id);

        }

    }
}
