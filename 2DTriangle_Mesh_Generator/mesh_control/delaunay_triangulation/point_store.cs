using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation
{
    public class point_store
    {
        public int pt_id { get; private set; }

        public point_d pt_coord { get; private set; }

        public int pt_type { get; private set; }

        public point_store(int i_pt_id, double i_x, double i_y, int i_pt_type)
        {
            // Constructor
            // set id
            this.pt_id = i_pt_id;
            // co-ordinate
            this.pt_coord = new point_d(i_x, i_y);

            // point type 1 - End point (or vertex), 2 - Lies on edge, 3 - Lies inside surface
            this.pt_type = i_pt_type;
        }

        public bool Equals(point_d other_ptcoord)
        {
            return this.pt_coord.Equals(other_ptcoord);
        }

        public bool Equals(int other_pt_id)
        {
            return (this.pt_id == other_pt_id);
        }

        public override int GetHashCode()
        {
            return pt_id;
        }
    }
}
