using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation
{
    public class triangle_list_store
    {
        private Dictionary<int, triangle_store> all_triangles;
        private HashSet<int> unique_triangleid_list;

        public int triangles_count { get { return all_triangles.Count; } }

        public triangle_list_store()
        {
            // Empty constructor
            this.all_triangles = new Dictionary<int, triangle_store>();
            this.unique_triangleid_list = new HashSet<int>();
        }

        public int get_point_containing_triangle(int the_pt_id, point_list_store pt_datas)
        {
            // Order the triangle with closest to mid point
            point_store the_pt = pt_datas.get_point(the_pt_id);
            // point_to_triange_mid_distance_comparer mdist_comparer = new point_to_triange_mid_distance_comparer(the_pt.pt_coord);
            // List<triangle_store> sorted_triangle = get_all_triangles().OrderBy(obj => obj, mdist_comparer).ToList();
            List<triangle_store> q_triangle = get_all_triangles().FindAll(obj => obj.is_point_inside_circumcircle(the_pt.pt_coord));

            // returns the triangle which contains this point
            foreach (triangle_store tri in q_triangle)
            {
                if (is_point_inside(the_pt.pt_coord,
                    pt_datas.get_point(tri.pt1_id).pt_coord,
                    pt_datas.get_point(tri.pt2_id).pt_coord,
                    pt_datas.get_point(tri.pt3_id).pt_coord) == true)
                {
                    // return the triange id 
                    return tri.tri_id;
                }
            }

            // No triangle found
            return -1;
        }

        public int add_triangle(int i_pt1_id, int i_pt2_id, int i_pt3_id,
            int i_edge1_id, int i_edge2_id, int i_edge3_id,
            point_d i_mid_pt, point_d i_circum_center,
            double i_shortest_edge_length, double i_circum_radius)
        {
            // Add triangle
            int tri_id = get_unique_triangle_id();
            this.all_triangles.Add(tri_id, new triangle_store(tri_id, i_pt1_id, i_pt2_id, i_pt3_id,
                i_edge1_id, i_edge2_id, i_edge3_id,
                i_mid_pt, i_circum_center,
                i_shortest_edge_length, i_circum_radius)
                );

            return tri_id;
        }

        public double get_triangle_shortest_edge()
        {
            List<triangle_store> tri_l = get_all_triangles();

            double shortest_edge = Double.MaxValue;

            // Find the shortest edge
            foreach (triangle_store tri in tri_l)
            {
                if (tri.shortest_edge_length < shortest_edge)
                {
                    shortest_edge = tri.shortest_edge_length;
                }
            }

            return shortest_edge;
        }

        public triangle_store get_bad_triangle(point_d[] outter_bndry_pts, double Bv, double Hv)
        {
            foreach (triangle_store bad_triangle in get_all_triangles())
            {
                if (is_contain_rect(outter_bndry_pts[0], outter_bndry_pts[1], outter_bndry_pts[2],bad_triangle.mid_pt) == true)
                {

                    if ((bad_triangle.circum_radius / bad_triangle.shortest_edge_length) > Bv)
                    {
                        // condition 1: B parameter => A triangle is well-shaped if all its angles are greater than or equal to 30 degrees
                        return bad_triangle;
                    }
                    else if (bad_triangle.circum_radius > Hv && bad_triangle.shortest_edge_length > Hv)
                    {
                        // condition 2: h parameter => A triangle is well-sized if it satisfies a user - supplied grading function
                        return bad_triangle;
                    }
                }
            }
            return null;
        }

        private bool is_contain_rect(point_d rect_bot_leftpt, point_d rect_top_leftpt, point_d rect_bot_rightpt,point_d test_pt)
        {
            return (rect_bot_leftpt.x < test_pt.x && test_pt.x < rect_bot_rightpt.x &&
                rect_bot_leftpt.y < test_pt.y && test_pt.y < rect_top_leftpt.y);

        }

        public point_d get_triangle_midpt(int tri_id)
        {
            // return the mid point of triangle
            return get_triangle(tri_id).mid_pt;
        }

        public void remove_triangle(int r_tri_id)
        {
            // Remove the triangle
            unique_triangleid_list.Add(r_tri_id);
            this.all_triangles.Remove(r_tri_id);
        }

        public (int pt1_id, int pt2_id, int pt3_id) get_specific_triangle_point_ids(int tri_id)
        {
            // Return the 3 pt_ids of the triangle tri_id 
            return get_triangle(tri_id).get_point_ids();
        }

        public (int e1_id, int e2_id, int e3_id) get_specific_triangle_edge_ids(int tri_id)
        {
            // Return the 3 edge_ids of the triangle tri_id
            return get_triangle(tri_id).get_edge_ids();
        }

        public bool is_point_in_specific_triangle_circumcircle(int tri_id, point_d pt)
        {
            // Return whether the given pt lies inside the in_circle
            return get_triangle(tri_id).is_point_inside_circumcircle(pt);
        }

        // Tests if a 2D point lies inside this 2D triangle.See Real-Time Collision
        // Detection, chap. 5, p. 206.
        //
        // @param point
        // The point to be tested
        // @return Returns true iff the point lies inside this 2D triangle
        private bool is_point_inside(point_d the_pt, point_d pt1, point_d pt2, point_d pt3)
        {
            double pab = the_pt.sub(pt1).cross(pt2.sub(pt1));
            double pbc = the_pt.sub(pt2).cross(pt3.sub(pt2));

            if (has_same_sign(pab, pbc) == false)
            {
                return false;
            }

            double pca = the_pt.sub(pt3).cross(pt1.sub(pt3));

            if (has_same_sign(pab, pca) == false)
            {
                return false;
            }
            return true;
        }

        private bool has_same_sign(double a, double b)
        {
            return Math.Sign(a) == Math.Sign(b);
        }

        private bool test_point_on_line(point_d the_pt, point_d s_pt, point_d e_pt)
        {
            bool rslt = false;
            // Step: 1 Find the cross product
            double dxc, dyc; // Vector 1 Between given point and first point of the line
            dxc = the_pt.x - s_pt.x;
            dyc = the_pt.y - s_pt.y;

            double Threshold = (((s_pt.x - e_pt.x) * (s_pt.x - e_pt.x)) + ((s_pt.y - e_pt.y) * (s_pt.y - e_pt.y))) * 0.0001;

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

        private bool is_point_in_boundary(point_d i_pt, point_d[] bndry_pts)
        {
            // Get the angle between input_pt and the first & last boundary pt
            //int max_pt = bndry_pts.Count - 1;

            //List<point_store> b_pts = this.bndry_pts.ToList();

            double t_angle = point_d.GetAngle(bndry_pts[3], i_pt, bndry_pts[0]);

            // Add the angle  to the inpt_pt and other boundary pts
            for (int i = 0; i < 3; i++)
            {
                t_angle += point_d.GetAngle(bndry_pts[i], i_pt, bndry_pts[i + 1]);
            }

            return (Math.Abs(t_angle) > 1);

        }

        private int get_unique_triangle_id()
        {
            int tri_id;
            // get an unique triangle id
            if (unique_triangleid_list.Count != 0)
            {
                tri_id = unique_triangleid_list.First(); // retrive the edge id from the list which stores the id of deleted edges
                unique_triangleid_list.Remove(tri_id); // remove that id from the unique edge id list
            }
            else
            {
                tri_id = this.all_triangles.Count;
            }
            return tri_id;
        }

        public List<triangle_store> get_all_triangles()
        {
            return this.all_triangles.Values.ToList();
        }

        public triangle_store get_triangle(int tri_id)
        {
            triangle_store tri;
            if (this.all_triangles.TryGetValue(tri_id, out tri) == true)
            {
                return tri;
            }
            return null;
        }
    }
}
