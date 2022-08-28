using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class triangle_store
    {
        public int tri_id { get; private set; }

        public point_store pt0 { get; private set; }

        public point_store pt1 { get; private set; }

        public point_store pt2 { get; private set; }

        public triangle_store(int t_tri_id, point_store t_pt0, point_store t_pt1, point_store t_pt2)
        {
            // Main constructor
            this.tri_id = t_tri_id;
            this.pt0 = t_pt0;
            this.pt1 = t_pt1;
            this.pt2 = t_pt2;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as triangle_store);
        }

        public bool Equals(triangle_store other_tri)
        {
            // Check 1 (Line ids should not match)
            if (this.tri_id ==other_tri.tri_id)
            {
                return true;
            }

            // Check 2 (Whether line end points match)
            if (is_point_attached(other_tri.pt0) == true &&
                is_point_attached(other_tri.pt1) == true &&
                is_point_attached(other_tri.pt2) == true)
            {
                return true;
            }
            return false;
        }

        private bool is_point_attached(point_store pt)
        {
            if (this.pt0.Equals(pt) ||
             this.pt1.Equals(pt) ||
             this.pt2.Equals(pt))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.tri_id, this.pt0.pt_id, this.pt1.pt_id, this.pt2.pt_id);
        }


    }
}
