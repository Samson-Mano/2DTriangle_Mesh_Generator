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

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class triangle_list_store
    {
        public List<triangle_store> all_tri { get; private set; }
        private points_list_store all_tri_pts;

        private uint[] _tri_indices = new uint[0];

        // OpenTK variables
        private VertexBuffer tripts_VertexBufferObject;
        private List<VertexBufferLayout> tri_BufferLayout;
        private VertexArray tri_VertexArrayObject;
        private IndexBuffer tri_ElementBufferObject;

        public triangle_list_store()
        {
            // Empty constructor
            // Initialize all points
            all_tri = new List<triangle_store>();
            all_tri_pts = new points_list_store();
        }

        public void set_openTK_objects()
        {

            // Set the quadrialateral indices
            int j = 0;
            this._tri_indices = new uint[all_tri.Count * 3];

            foreach (triangle_store tris in all_tri)
            {
                /*
                1
                | \         
                |   \       
                | t   \   
                |       \   
                0__________2
                */
                // 0, 1, 2
                // First index (First point)
                this._tri_indices[j] = (uint)tris.pt0.pt_id;
                j++;

                // Second index (Second point)
                this._tri_indices[j] = (uint)tris.pt1.pt_id;
                j++;

                // Third index (Third point)
                this._tri_indices[j] = (uint)tris.pt2.pt_id;
                j++;
            }

            // Set the openTK objects for the points
            this.all_tri_pts.set_openTK_objects();

            //1.  Get the vertex buffer
            this.tripts_VertexBufferObject = this.all_tri_pts.point_VertexBufferObject;

            //2. Create and add to the buffer layout
            tri_BufferLayout = new List<VertexBufferLayout>();
            tri_BufferLayout.Add(new VertexBufferLayout(3, 7)); // Vertex layout
            tri_BufferLayout.Add(new VertexBufferLayout(4, 7)); // Color layout  

            //3. Setup the vertex Array (Add vertexBuffer binds both the vertexbuffer and vertexarray)
            tri_VertexArrayObject = new VertexArray();
            tri_VertexArrayObject.Add_vertexBuffer(this.tripts_VertexBufferObject, tri_BufferLayout);

            // 4. Set up element buffer
            tri_ElementBufferObject = new IndexBuffer(this._tri_indices, this._tri_indices.Length);
            tri_ElementBufferObject.Bind();
        }

        public void paint_all_triangles()
        {
            // Call set_openTK_objects()
            // Bind before painting
            tri_VertexArrayObject.Add_vertexBuffer(this.tripts_VertexBufferObject, tri_BufferLayout);
            tri_ElementBufferObject.Bind();

            // Open GL paint quadrialateral
            GL.DrawElements(PrimitiveType.Triangles, this._tri_indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void add_triangle(int t_tid, double pt0_x, double pt0_y, Color pt_clr0,
                                     double pt1_x, double pt1_y, Color pt_clr1,
                                     double pt2_x, double pt2_y, Color pt_clr2)
        {
            // Add points
            this.all_tri_pts.add_point((t_tid * 3) + 0, pt0_x, pt0_y, pt_clr0);
            point_store pt0 = this.all_tri_pts.get_last_added_pt;

            this.all_tri_pts.add_point((t_tid * 3) + 1, pt1_x, pt1_y, pt_clr1);
            point_store pt1 = this.all_tri_pts.get_last_added_pt;

            this.all_tri_pts.add_point((t_tid * 3) + 2, pt2_x, pt2_y, pt_clr2);
            point_store pt2 = this.all_tri_pts.get_last_added_pt;

            // Add Quad
            this.all_tri.Add(new triangle_store(t_tid, pt0, pt1, pt2));
        }

    }
}
