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
    public class points_list_store
    {
        public HashSet<point_store> all_pts { get; private set; }

        public point_store get_last_added_pt { get; private set; }

        private float[] _point_vertices = new float[0];

        private uint[] _point_indices = new uint[0];

        // OpenTK variables
        public VertexBuffer point_VertexBufferObject { get; private set; }
        public List<VertexBufferLayout> point_BufferLayout { get; private set; }
        private VertexArray point_VertexArrayObject;
        private IndexBuffer point_ElementBufferObject;

        public points_list_store()
        {
            // Empty constructor
            // Initialize all points
            all_pts = new HashSet<point_store>();
        }

        public void set_openTK_objects()
        {
            // Set the openTK objects for the points
            // Set the vertices
            this._point_vertices = new float[7 * all_pts.Count];
            this._point_indices = new uint[all_pts.Count];

            foreach (point_store pts in all_pts)
            {
                // add the point vertices
                float[] temp_vertices = pts.get_point_vertices();

                int i = pts.pt_id;
                // X, Y, Z Co-ordinate
                this._point_vertices[(i * 7) + 0] = temp_vertices[0];
                this._point_vertices[(i * 7) + 1] = temp_vertices[1];
                this._point_vertices[(i * 7) + 2] = temp_vertices[2];

                // R, G, B, A values
                this._point_vertices[(i * 7) + 3] = temp_vertices[3];
                this._point_vertices[(i * 7) + 4] = temp_vertices[4];
                this._point_vertices[(i * 7) + 5] = temp_vertices[5];
                this._point_vertices[(i * 7) + 6] = temp_vertices[6];

                // Add the point indices
                this._point_indices[i] = (uint)i;
                i++;
            }

            //1.  Set up vertex buffer
            point_VertexBufferObject = new VertexBuffer(this._point_vertices, this._point_vertices.Length * sizeof(float));

            //2. Create and add to the buffer layout
            point_BufferLayout = new List<VertexBufferLayout>();
            point_BufferLayout.Add(new VertexBufferLayout(3, 7)); // Vertex layout
            point_BufferLayout.Add(new VertexBufferLayout(4, 7)); // Color layout  

            //3. Setup the vertex Array (Add vertexBuffer binds both the vertexbuffer and vertexarray)
            point_VertexArrayObject = new VertexArray();
            point_VertexArrayObject.Add_vertexBuffer(point_VertexBufferObject, point_BufferLayout);

            // 4. Set up element buffer
            point_ElementBufferObject = new IndexBuffer(this._point_indices, this._point_indices.Length);
            point_ElementBufferObject.Bind();
        }

        public void paint_all_points()
        {
            // Call set_openTK_objects()
            // Bind before painting
            point_VertexArrayObject.Add_vertexBuffer(point_VertexBufferObject, point_BufferLayout);
            point_ElementBufferObject.Bind();

            // Open GL paint
            GL.PointSize(4.0f);
            GL.DrawElements(PrimitiveType.Points, this._point_indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void add_point(int id,double t_x, double t_y, Color clr)
        {
            // Add point
            point_store temp_pt = new point_store(id, t_x, t_y, clr);

            // Check whether the point already exists
            if (all_pts.Contains(temp_pt) == false)
            {
                // Add new point
                all_pts.Add(temp_pt);
                this.get_last_added_pt = temp_pt;
            }
            else
            {
                // Point already exists
                this.get_last_added_pt = all_pts.Last(obj => obj.Equals(temp_pt));
            }
        }

        public void update_scale(double d_scale, double tran_tx, double tran_ty)
        {
            // update the scale of curves
            for(int i = 0; i < all_pts.Count; i++)
            {
                this.all_pts.ElementAt(i).update_scale(d_scale,tran_tx,tran_ty);
            }

        }

        public void add_point(point_store temp_pt)
        {
            // Check whether the point already exists
            if (all_pts.Contains(temp_pt) == false)
            {
                // Add new point
                all_pts.Add(temp_pt);
                this.get_last_added_pt = temp_pt;
            }
            else
            {
                // Point already exists
                this.get_last_added_pt = all_pts.Last(obj => obj.Equals(temp_pt));
            }
        }
    }
}
