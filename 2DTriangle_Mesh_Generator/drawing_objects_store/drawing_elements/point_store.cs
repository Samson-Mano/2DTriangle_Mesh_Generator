using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class point_store
    {
        // Size of the int -2,147,483,648 to 2,147,483,647
        private int _x;
        private int _y;
        private Color _pt_clr = Color.Red;

        private double pt_paint_x;
        private double pt_paint_y;

        public int pt_id { get; private set; }

        public double d_x { get; private set; }

        public double d_y { get; private set; }

        private float[] get_vertex_coords()
        {
            float[] vertex_coord = new float[3];
            // Add vertex to list
            vertex_coord[0] = (float)pt_paint_x;
            vertex_coord[1] = (float)pt_paint_y;
            vertex_coord[2] = 0.0f;

            return vertex_coord;
        }

        private float[] get_vertex_color()
        {
            float[] vertex_clr = new float[4];

            // Add vertex color to the list
            vertex_clr[0] = ((float)this._pt_clr.R / 255.0f);
            vertex_clr[1] = ((float)this._pt_clr.G / 255.0f);
            vertex_clr[2] = ((float)this._pt_clr.B / 255.0f);
            vertex_clr[3] = ((float)this._pt_clr.A / 255.0f);

            return vertex_clr;
        }

        public void update_scale(double d_scale, double tran_tx, double tran_ty)
        {
            this.pt_paint_x = (d_x - tran_tx) * d_scale;
            this.pt_paint_y = (d_y - tran_ty) * d_scale;
        }

        public float[] get_point_vertices()
        {
            // Return the point in openGL format
            return get_vertex_coords().Concat(get_vertex_color()).ToArray(); ;
        }

        public point_store(int t_pt_id, double t_x, double t_y, Color clr)
        {
            // No need to check below (bcoz or points are sent scaled to 1.0f
            //if ((t_x < -2000 || t_x > 2000) || (t_y < -2000 || t_y > 2000))
            //{
            //    throw new ArgumentException("Point must be in range [-2000, 2000]");
            //}

            // input t_x & t_y should be less than 2000.0000000 (to avoid exceeding int size)
            // Main constructor
            this.pt_id = t_pt_id;
            this.d_x = t_x;
            this.d_y = t_y;

            // Add the input to integer to avoid floating point impressision issues
            // Easier to compare the inputs
            this._x = (int)(t_x * 100000);
            this._y = (int)(t_y * 100000);

            this._pt_clr = clr;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as point_store);
        }

        public bool Equals(point_store other_pt)
        {
            // Check 1 (Point ids should not match)
            if (this.Equals(other_pt.pt_id) == true)
            {
                return true;
            }

            // Check 2 (Whether point co-ordinates match)
            if (this.Equals(other_pt._x, other_pt._y) == true)
            {
                return true;
            }
            return false;
        }

        private bool Equals(int other_pt_id)
        {
            // Check whether the ids are matching
            return (this.pt_id == other_pt_id);
        }

        public bool Equals(int other_pt_x, int other_pt_y)
        {
            // Check whether the point-coordinates are the same
            if (this._x == other_pt_x && this._y == other_pt_y)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.pt_id, this._x, this._y);
        }
    }
}
