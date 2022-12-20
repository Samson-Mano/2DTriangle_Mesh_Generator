using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation
{
    public class edge_list_store
    {
        //  public HashSet<edge_store> all_edges { get; private set; }

        private Dictionary<int, edge_store> all_edges;
        private HashSet<int> unique_edgeid_list;

        public edge_list_store()
        {
            // Empty Constructor
            this.all_edges = new Dictionary<int, edge_store>();
            this.unique_edgeid_list = new HashSet<int>();
        }

        public int add_edge(int pt1_id, int pt2_id, point_d mid_pt, double edge_length)
        {
            // Add Edge
            int edge_id = get_unique_edge_id();

            this.all_edges.Add(edge_id, new edge_store(edge_id, pt1_id, pt2_id, mid_pt, edge_length));
            return edge_id;
        }

        public void remove_edge(int r_edge_id)
        {
            // Remove edge
            unique_edgeid_list.Add(r_edge_id);

            this.all_edges.Remove(r_edge_id);
        }


        public void associate_triangle_to_edge(int f_edge_id, int tri_id, point_list_store pt_datas, triangle_list_store tri_datas)
        {
            // Get triangle mid point
            point_d tri_midpt = tri_datas.get_triangle_midpt(tri_id);

            // Associate triangle to the edge
            this.all_edges[f_edge_id].associate_triangle(tri_id, tri_midpt, pt_datas);
        }

        public int get_point_containing_edge(int the_pt_id, point_list_store pt_datas)
        {
            point_store the_pt = pt_datas.get_point(the_pt_id);
            List<edge_store> edges_as_list = get_all_edges().Where(obj => obj.is_pt_inside_midcircle(the_pt.pt_coord)).ToList();

            foreach (edge_store edge in edges_as_list)
            {
                if (test_point_on_line(the_pt.pt_coord,
                    pt_datas.get_point(edge.start_pt_id).pt_coord,
                    pt_datas.get_point(edge.end_pt_id).pt_coord, edge.edge_length) == true)
                {
                    // return the edge id
                    return edge.edge_id;
                }
            }

            // No edges found
            return -1;
        }

        public int get_specific_edge_other_triangle_id(int edge_id, int tri_id)
        {
            // Returns the other triangle (other than the given tri id) of an edge_id
            edge_store e1 = get_edge(edge_id);
            return e1.other_triangle_id(tri_id);
        }

        public bool is_specific_edge_endpt(int edge_id, int pt_id)
        {
            // returns whether the given pt_id is either startpt or endpt
            edge_store e1 = get_edge(edge_id);

            return ((e1.start_pt_id == pt_id) || (e1.end_pt_id == pt_id));
        }

        private bool test_point_on_line(point_d the_pt, point_d s_pt, point_d e_pt, double edge_length)
        {
            bool rslt = false;
            // Step: 1 Find the cross product
            double dxc, dyc; // Vector 1 Between given point and first point of the line
            dxc = the_pt.x - s_pt.x;
            dyc = the_pt.y - s_pt.y;

            double Threshold = edge_length * 0.01;

            double dx1, dy1; // Vector 2 Between the second and first point of the line
            dx1 = e_pt.x - s_pt.x;
            dy1 = e_pt.y - s_pt.y;

            double crossprd;
            crossprd = (dxc * dy1) - (dyc * dx1); // Vector cross product

            if (Math.Abs(crossprd) <= Threshold) // Check whether the cross product is within the threshold (other wise Not on the line)
            {
                if (Math.Abs(dx1) >= Math.Abs(dy1)) // The line is more horizontal or <= 45 degrees
                {
                    rslt = (dx1 > 0 ? (s_pt.x < the_pt.x && the_pt.x < e_pt.x ? true : false) :
                                      (e_pt.x < the_pt.x && the_pt.x < s_pt.x ? true : false));
                }
                else // line is more vertical
                {
                    rslt = (dy1 > 0 ? (s_pt.y < the_pt.y && the_pt.y < e_pt.y ? true : false) :
                                        (e_pt.y < the_pt.y && the_pt.y < s_pt.y ? true : false));
                }
            }
            return rslt;
        }

        private int get_unique_edge_id()
        {
            int edge_id;
            // get an unique edge id
            if (unique_edgeid_list.Count != 0)
            {
                edge_id = unique_edgeid_list.First(); // retrive the edge id from the list which stores the id of deleted edges
                unique_edgeid_list.Remove(edge_id); // remove that id from the unique edge id list
            }
            else
            {
                edge_id = this.all_edges.Count;
            }
            return edge_id;
        }

        public List<edge_store> get_all_edges()
        {
            return this.all_edges.Values.ToList();
        }


        public edge_store get_edge(int ed_id)
        {
            edge_store ed;
            if (this.all_edges.TryGetValue(ed_id, out ed) == true)
            {
                return ed;
            }
            return null;
        }
    }
}
