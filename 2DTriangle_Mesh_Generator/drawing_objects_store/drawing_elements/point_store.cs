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

        public int pt_id { get; private set; }

        public double d_x { get; private set; }

        public double d_y { get; private set; }

        private float[] get_vertex_coords()
        {
            float[] vertex_coord = new float[3];
            // Add vertex to list
            vertex_coord[0] = (float)d_x;
            vertex_coord[1] = (float)d_y;
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


    public class PointsComparer : IComparer<point_store>
    {
        private int center_x = 0;
        private int center_y = 0;

        public PointsComparer(double c_x, double c_y)
        {
            this.center_x = (int)(c_x * 100000);
            this.center_y = (int)(c_y * 100000);
        }

        public int Compare(point_store a, point_store b)
        {
            // Sort points in clockwise order
            // https://stackoverflow.com/questions/6989100/sort-points-in-clockwise-order
            int a_x = (int)(a.d_x * 100000);
            int a_y = (int)(a.d_y * 100000);

            int b_x = (int)(b.d_x * 100000);
            int b_y = (int)(b.d_y * 100000);

            // Both points are equal
            if (a_x == b_x && a_y == b_y)
                return 0;

            if (a_x - center_x >= 0 && b_x - center_x < 0)
                return 1;
            if (a_x - center_x < 0 && b_x - center_x >= 0)
                return -1;
            if (a_x - center_x == 0 && b_x - center_x == 0)
            {
                if (a_y - center_y >= 0 || b_y - center_y >= 0)
                    return (a_y > b_y ? 1 : -1);
                return (b_y > a_y ? 1 : -1);
            }

            // compute the cross product of vectors (center -> a) x (center -> b)
            int det = (a_x - center_x) * (b_y - center_y) - (b_x - center_x) * (a_y - center_y);
            if (det < 0)
                return 1;
            if (det > 0)
                return -1;

            // points a and b are on the same line from the center
            // check which point is closer to the center
            int d1 = (a_x - center_x) * (a_x - center_x) + (a_y - center_y) * (a_y - center_y);
            int d2 = (b_x - center_x) * (b_x - center_x) + (b_y - center_y) * (b_y - center_y);
            return (d1 > d2 ? 1 : -1);

        }
    }
}
