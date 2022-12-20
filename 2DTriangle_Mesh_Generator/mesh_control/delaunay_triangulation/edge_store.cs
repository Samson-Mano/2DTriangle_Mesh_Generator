using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation
{
    public class edge_store
    {
        public int edge_id { get; private set; }

        public int start_pt_id { get; private set; }

        public int end_pt_id { get; private set; }

        public point_d mid_pt { get; private set; }

        public int left_triangle_id { get; private set; }

        public int right_triangle_id { get; private set; }

        public double edge_length { get; private set; }

        public edge_store(int i_edge_id, int i_start_pt_id, int i_end_pt_id, point_d i_mid_pt, double i_edge_length)
        {
            // Empty Constructor
            // set id
            this.edge_id = i_edge_id;
            // set start and end pt
            this.start_pt_id = i_start_pt_id;
            this.end_pt_id = i_end_pt_id;

            // set triangles to null
            this.left_triangle_id = -1;
            this.right_triangle_id = -1;

            // Set the mid & edges
            this.mid_pt = i_mid_pt;
            this.edge_length = i_edge_length;
            //______________________________________________________________
        }

        public void associate_triangle(int tri_id, point_d tri_midpt, point_list_store pt_datas)
        {
            // get the start point and end point
            point_d spt = pt_datas.get_point(start_pt_id).pt_coord;
            point_d ept = pt_datas.get_point(end_pt_id).pt_coord;

            if (rightof(tri_midpt, spt, ept) == true)
            {
                // Add the right triangle
                this.right_triangle_id = tri_id;
            }
            else
            {
                // Add the left triangle
                this.left_triangle_id = tri_id;
            }
        }

        public int other_triangle_id(int tri_id)
        {
            //  return the other triangle (other than tri_id) associated with this edge
            if (tri_id == this.right_triangle_id)
            {
                return this.left_triangle_id;
            }
            else if (tri_id == this.left_triangle_id)
            {
                return this.right_triangle_id;
            }
            return -1;
        }

        public int other_point_id(int pt_id)
        {
            // Return the point id of other point than the given
            if (pt_id == this.start_pt_id)
            {
                return this.end_pt_id;
            }
            else if (pt_id == this.end_pt_id)
            {
                return this.start_pt_id;
            }
            return -1;
        }

        public bool is_pt_inside_midcircle(point_d pt)
        {
            return ((pt.x - mid_pt.x) * (pt.x - mid_pt.x) + (pt.y - mid_pt.y) * (pt.y - mid_pt.y)) < ((edge_length * edge_length) / 4);
        }

        private bool ccw(point_d a, point_d b, point_d c)
        {
            // Computes | a.x a.y  1 |
            //          | b.x b.y  1 | > 0
            //          | c.x c.y  1 |
            return (((b.x - a.x) * (c.y - a.y)) - ((b.y - a.y) * (c.x - a.x))) > 0;
        }

        private bool rightof(point_d pt, point_d edge_spt, point_d edge_ept)
        {
            return ccw(pt, edge_ept, edge_spt);
        }

        public bool Equals(int other_edge_id)
        {
            return (this.edge_id == other_edge_id);
        }

        public override int GetHashCode()
        {
            return edge_id;
        }
    }
}
