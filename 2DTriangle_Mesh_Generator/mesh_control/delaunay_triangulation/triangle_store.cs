using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation
{
    public class triangle_store
    {
        public int tri_id { get; private set; }
        public int pt1_id { get; private set; }
        public int pt2_id { get; private set; }
        public int pt3_id { get; private set; }

        public point_d mid_pt { get; private set; }
        public point_d circum_center { get; private set; }

        public double circum_radius { get; private set; }
        public double shortest_edge_length { get; private set; }

        public int e1_id { get; private set; } // from pt1 to pt2

        public int e2_id { get; private set; } // from pt2 to pt3

        public int e3_id { get; private set; } // from pt3 to pt1

        public (int pt1_id, int pt2_id, int pt3_id) get_point_ids()
        {
            return (this.pt1_id, this.pt2_id, this.pt3_id);
        }

        public (int e1_id, int e2_id, int e3_id) get_edge_ids()
        {
            return (this.e1_id, this.e2_id, this.e3_id);
        }

        public triangle_store(int i_tri_id, int i_pt1_id, int i_pt2_id,
            int i_pt3_id, int i_e1_id, int i_e2_id, int i_e3_id,
            point_d i_mid_pt, point_d i_circum_center,
            double i_shortest_edge_length, double i_circum_radius)
        {
            // Empty Constructor
            this.tri_id = i_tri_id;

            // Point ids
            this.pt1_id = i_pt1_id;
            this.pt2_id = i_pt2_id;
            this.pt3_id = i_pt3_id;

            // Edge ids
            this.e1_id = i_e1_id;
            this.e2_id = i_e2_id;
            this.e3_id = i_e3_id;

            // Set mid pt and cirum center
            this.mid_pt = i_mid_pt;
            this.circum_center = i_circum_center;

            this.shortest_edge_length = i_shortest_edge_length;
            this.circum_radius = i_circum_radius;
        }

        public bool is_point_inside_circumcircle(point_d pt)
        {
            double d_squared = (pt.x - circum_center.x) * (pt.x - circum_center.x) +
                (pt.y - circum_center.y) * (pt.y - circum_center.y);
            return d_squared < (this.circum_radius * this.circum_radius);

        }

        public bool Equals(int other_tri_id)
        {
            return (this.tri_id == other_tri_id);
        }

        public override int GetHashCode()
        {
            return tri_id;
        }
    }

    public class point_to_triange_mid_distance_comparer : IComparer<triangle_store>
    {
        public point_d inpt_pt { get; set; }

        public point_to_triange_mid_distance_comparer(point_d i_inpt_pt)
        {
            this.inpt_pt = i_inpt_pt;
        }

        public int Compare(triangle_store tri1, triangle_store tri2)
        {
            double dist1 = ((tri1.mid_pt.x - inpt_pt.x) * (tri1.mid_pt.x - inpt_pt.x)) + ((tri1.mid_pt.y - inpt_pt.y) * (tri1.mid_pt.y - inpt_pt.y));
            double dist2 = ((tri2.mid_pt.x - inpt_pt.x) * (tri2.mid_pt.x - inpt_pt.x)) + ((tri2.mid_pt.y - inpt_pt.y) * (tri2.mid_pt.y - inpt_pt.y));

            return dist1.CompareTo(dist2);
        }
    }

}
