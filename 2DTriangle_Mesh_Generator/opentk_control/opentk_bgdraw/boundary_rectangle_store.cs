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
// This app class structure
using _2DTriangle_Mesh_Generator.opentk_control.opentk_buffer;
using _2DTriangle_Mesh_Generator.opentk_control.shader_compiler;

namespace _2DTriangle_Mesh_Generator.opentk_control.opentk_bgdraw
{
    public class boundary_rectangle_store
    {
        private bool _Isinitialized = false;

        // Buffer objects
        VertexBuffer VertexBufferObject;
        VertexArray VertexArrayObject;
        IndexBuffer ElementBufferObject;
        List<VertexBufferLayout> buffer_layouts;

        private readonly float[] _boundary_rect_vertices =
        {
                 -1.0f, -1.0f, 0.0f, 1.0f , 0.0f, 1.0f,// -X,-Y
                 +1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 1.0f, // +X,-Y
                 +1.0f, +1.0f, 0.0f, 1.0f, 0.0f,1.0f,  // +X,+Y
                 -1.0f, +1.0f, 0.0f, 1.0f, 0.0f, 1.0f, // -X,+Y
                 -1.0f, 0.0f, 1.0f, 0.0f,0.0f ,1.0f, // y = 0 line
                 +1.0f, 0.0f, 1.0f, 0.0f,0.0f ,1.0f, // y = 0 line
                  0.0f, -1.0f, 0.0f, 0.0f,1.0f ,1.0f, // x = 0 line
                  0.0f, +1.0f, 0.0f, 0.0f,1.0f ,1.0f // x = 0 line
            };


        private readonly uint[] _boundary_rect_indices =
        {
                0,1,1,2,2,3,3,0, 4,5, 6,7
            };

        public boundary_rectangle_store()
        {
            // Empty constructor
            this._Isinitialized = false;
        }

        public boundary_rectangle_store(bool initialize, Shader _shader)
        {
            // Constructor to initialize object
            this._Isinitialized = initialize;

            if (this._Isinitialized == true)
            {
                // Setup the buffer and attribute pointers below
                //1.  Set up vertex buffer
                VertexBufferObject = new VertexBuffer(this._boundary_rect_vertices, this._boundary_rect_vertices.Length * sizeof(float));

                //2. Create and add to the buffer layout
                buffer_layouts = new List<VertexBufferLayout>();
                buffer_layouts.Add(new VertexBufferLayout(2, 6));
                buffer_layouts.Add(new VertexBufferLayout(4, 6));

                //3. Setup the vertex Array (Add vertexBuffer binds both the vertexbuffer and vertexarray)
                VertexArrayObject = new VertexArray();
                VertexArrayObject.Add_vertexBuffer(VertexBufferObject, buffer_layouts);

                // 4. Set up element buffer
                ElementBufferObject = new IndexBuffer(_boundary_rect_indices, _boundary_rect_indices.Length);
                ElementBufferObject.Bind();
            }
        }

        public void paint_boundary_rectangle()
        {
            if (this._Isinitialized == true)
            {
                // Rebind the vertexArrayObject
                ElementBufferObject.Bind();
                VertexArrayObject.Add_vertexBuffer(VertexBufferObject, buffer_layouts);

                GL.PointSize(4.0f);

                GL.DrawElements(PrimitiveType.Points, _boundary_rect_indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.DrawElements(PrimitiveType.Lines, _boundary_rect_indices.Length, DrawElementsType.UnsignedInt, 0);
            }

        }
    }
}
