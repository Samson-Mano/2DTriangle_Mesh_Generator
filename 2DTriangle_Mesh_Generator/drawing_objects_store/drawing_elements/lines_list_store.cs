using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
// This app class structure
using _2DTriangle_Mesh_Generator.opentk_control.opentk_buffer;
using _2DTriangle_Mesh_Generator.global_variables;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class lines_list_store
    {
        public List<line_store> all_lines { get; private set; }
        private points_list_store all_line_pts;

        private uint[] _line_indices = new uint[0];

        // OpenTK variables
        private VertexBuffer linepts_VertexBufferObject;
        private List<VertexBufferLayout> line_BufferLayout;
        private VertexArray line_VertexArrayObject;
        private IndexBuffer line_ElementBufferObject;

        public lines_list_store()
        {
            // Empty constructor
            // Initialize all points
            all_lines = new List<line_store>();
            all_line_pts = new points_list_store();
        }

        public void set_openTK_objects()
        {

            // Set the line indices
            int j = 0;
            this._line_indices = new uint[all_lines.Count * 2];

            foreach (line_store ln in all_lines)
            {
                // First index (First point)
                this._line_indices[j] = (uint)ln.start_pt.pt_id;
                j++;

                // Second index (Second point)
                this._line_indices[j] = (uint)ln.end_pt.pt_id;
                j++;
            }

            // Set the openTK objects for the points
            this.all_line_pts.set_openTK_objects();

            //1.  Get the vertex buffer
            this.linepts_VertexBufferObject = all_line_pts.point_VertexBufferObject;

            //2. Create and add to the buffer layout
            line_BufferLayout = new List<VertexBufferLayout>();
            line_BufferLayout.Add(new VertexBufferLayout(3, 7)); // Vertex layout
            line_BufferLayout.Add(new VertexBufferLayout(4, 7)); // Color layout  

            //3. Setup the vertex Array (Add vertexBuffer binds both the vertexbuffer and vertexarray)
            line_VertexArrayObject = new VertexArray();
            line_VertexArrayObject.Add_vertexBuffer(this.linepts_VertexBufferObject, line_BufferLayout);

            // 4. Set up element buffer
            line_ElementBufferObject = new IndexBuffer(this._line_indices, this._line_indices.Length);
            line_ElementBufferObject.Bind();
        }

        public void set_highlight_openTK_objects()
        {

            // Set the line indices
            int j = 0;
            this._line_indices = new uint[all_lines.Count * 2];

            foreach (line_store ln in all_lines)
            {
                // First index (First point)
                this._line_indices[j] = (uint)ln.start_pt.pt_id;
                j++;

                // Second index (Second point)
                this._line_indices[j] = (uint)ln.end_pt.pt_id;
                j++;
            }

            // Set the openTK objects for the points
            this.all_line_pts.set_highlight_openTK_objects();

            //1.  Get the vertex buffer
            this.linepts_VertexBufferObject = all_line_pts.point_VertexBufferObject;

            //2. Create and add to the buffer layout
            line_BufferLayout = new List<VertexBufferLayout>();
            line_BufferLayout.Add(new VertexBufferLayout(3, 7)); // Vertex layout
            line_BufferLayout.Add(new VertexBufferLayout(4, 7)); // Color layout  

            //3. Setup the vertex Array (Add vertexBuffer binds both the vertexbuffer and vertexarray)
            line_VertexArrayObject = new VertexArray();
            line_VertexArrayObject.Add_vertexBuffer(this.linepts_VertexBufferObject, line_BufferLayout);

            // 4. Set up element buffer
            line_ElementBufferObject = new IndexBuffer(this._line_indices, this._line_indices.Length);
            line_ElementBufferObject.Bind();
        }
        

        public void paint_all_lines()
        {
            // Call set_openTK_objects()
            // Bind before painting
            line_VertexArrayObject.Add_vertexBuffer(this.linepts_VertexBufferObject, line_BufferLayout);
            line_ElementBufferObject.Bind();

            // Open GL paint Lines
            GL.DrawElements(PrimitiveType.Lines, this._line_indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void paint_all_highlight_lines()
        {
            // Call set_openTK_objects()
            // Bind before painting
            line_VertexArrayObject.Add_vertexBuffer(this.linepts_VertexBufferObject, line_BufferLayout);
            line_ElementBufferObject.Bind();

            // Open GL paint Lines
            GL.DrawElements(PrimitiveType.Lines, this._line_indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void update_scale(double d_scale, double tran_tx, double tran_ty)
        {
            // update scale of all the line pts
            this.all_line_pts.update_scale(d_scale, tran_tx, tran_ty);
        }

        public void add_line(int ln_id, double spt_x, double spt_y, Color spt_clr,
                     double ept_x, double ept_y, Color ept_clr)
        {
            // Add points
            all_line_pts.add_point((ln_id * 2) + 0, spt_x, spt_y, spt_clr);
            point_store spt = all_line_pts.get_last_added_pt;

            all_line_pts.add_point((ln_id * 2) + 1, ept_x, ept_y, ept_clr);
            point_store ept = all_line_pts.get_last_added_pt;

            // Add line

            all_lines.Add(new line_store(ln_id, spt, ept));
        }
    }
}
