using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation
{
    public class delaunay_triangulation
    {
        // Local Variables
        public point_list_store points_data { get; private set; }

        public edge_list_store edges_data { get; private set; }

        public triangle_list_store triangles_data { get; private set; }

        public bool is_meshed = false;

        public delaunay_triangulation()
        {
            // Empty constructor
            points_data = new point_list_store();
            edges_data = new edge_list_store();
            triangles_data = new triangle_list_store();
        }

        public void create_super_triangle(List<point_d> corner_pts, List<point_d> edge_pts)
        {
            // Call this first
            // points_data = new point_d(p_inpt_points);

            // sort the points first by x and then by y
            List<point_d> p_inpt_points = new List<point_d>();
            p_inpt_points.AddRange(corner_pts);
            p_inpt_points.AddRange(edge_pts);


            List<point_d> inpt_points = new List<point_d>();
            inpt_points = p_inpt_points.OrderBy(obj => obj.x).ThenBy(obj => obj.y).ToList();


            // intialize the edges and triangles
            edges_data = new edge_list_store();
            triangles_data = new triangle_list_store();

            // Create an imaginary triangle that encloses all the point set
            set_bounding_triangle(inpt_points);


        }

        public void Add_multiple_points(List<point_d> p_inpt_points, int pt_type)
        {
            // create_super_triangle(p_inpt_points);
            int pt_id;

            foreach (point_d i_pt in p_inpt_points)
            {

                // incemental add point
                pt_id = points_data.add_point(i_pt, pt_type);

                // Check whether point lies on edges
                int containing_edge_id = edges_data.get_point_containing_edge(pt_id, points_data);
                if (containing_edge_id != -1)
                {
                    // Point addition to edge
                    incremental_point_addition_edge(pt_id, containing_edge_id);
                }
                else
                {
                    // Check whether point lies inside triangle
                    int containing_triangle_id = triangles_data.get_point_containing_triangle(pt_id, points_data);
                    if (containing_triangle_id != -1)
                    {
                        // Point addition to the triangle
                        incremental_point_addition_inner(pt_id, containing_triangle_id);
                    }
                    else
                    {
                        // Point duplicate ??
                    }
                }
            }
        }

        public void Add_single_point(point_d circumcenter_point, int pt_type)
        {
            // dont call this before calling Add_multiple_points

            // Add the point to local list
            int pt_id = points_data.add_point(circumcenter_point, pt_type);

            // Check whether point lies on edges
            int containing_edge_id = edges_data.get_point_containing_edge(pt_id, points_data);
            if (containing_edge_id != -1)
            {
                // Point addition to edge
                incremental_point_addition_edge(pt_id, containing_edge_id);
            }
            else
            {
                // Check whether point lies inside triangle
                int containing_triangle_id = triangles_data.get_point_containing_triangle(pt_id, points_data);
                if (containing_triangle_id != -1)
                {
                    // Point addition to the triangle
                    incremental_point_addition_inner(pt_id, containing_triangle_id);
                }
                else
                {
                    // Point duplicate ??
                }
            }
        }

        private void incremental_point_addition_inner(int pt_id, int containing_triangle_id)
        {
            // collect the edges of the triangle
            point_store pt = this.points_data.get_point(pt_id);
            (int e1, int e2, int e3) tri_edges = this.triangles_data.get_specific_triangle_edge_ids(containing_triangle_id);
            // collect the points of the triangle
            (int p1, int p2, int p3) tri_pts = this.triangles_data.get_specific_triangle_point_ids(containing_triangle_id);

            List<int> tri_three_edges = new List<int>();
            List<int> tri_three_pts = new List<int>();

            tri_three_edges.Add(tri_edges.e1);
            tri_three_edges.Add(tri_edges.e2);
            tri_three_edges.Add(tri_edges.e3);

            tri_three_pts.Add(tri_pts.p1);
            tri_three_pts.Add(tri_pts.p2);
            tri_three_pts.Add(tri_pts.p3);

            // remove the single triangle
            this.triangles_data.remove_triangle(containing_triangle_id);

            // add the three triangles
            int[] triangle_id = new int[3];
            triangle_id = add_three_triangles(pt, tri_three_pts, tri_three_edges);

            // Flip the bad triangles recursively
            flip_bad_edges(triangle_id[0], pt);
            flip_bad_edges(triangle_id[1], pt);
            flip_bad_edges(triangle_id[2], pt);
        }

        private void incremental_point_addition_edge(int pt_id, int containing_edge_id)
        {
            // Point lies on the edge
            point_store pt = this.points_data.get_point(pt_id);
            edge_store inc_edge = this.edges_data.get_edge(containing_edge_id);

            // Get the left and right triangle id of the edge
            int first_tri_id = inc_edge.left_triangle_id;
            int second_tri_id = inc_edge.right_triangle_id;

            HashSet<int> tri_outside_four_edges_h = new HashSet<int>();
            // Collect the 3 edges of first triangle
            (int e1, int e2, int e3) first_tri_edges = this.triangles_data.get_specific_triangle_edge_ids(first_tri_id);
            // Collect the 3 edges of second tri
            (int e1, int e2, int e3) second_tri_edges = this.triangles_data.get_specific_triangle_edge_ids(second_tri_id);

            // Only 5 will be added (because 1 edge is common)
            tri_outside_four_edges_h.Add(first_tri_edges.e1);
            tri_outside_four_edges_h.Add(first_tri_edges.e2);
            tri_outside_four_edges_h.Add(first_tri_edges.e3);
            tri_outside_four_edges_h.Add(second_tri_edges.e1);
            tri_outside_four_edges_h.Add(second_tri_edges.e2);
            tri_outside_four_edges_h.Add(second_tri_edges.e3);

            // Remove the 1 common edge
            tri_outside_four_edges_h.Remove(inc_edge.edge_id);

            // Remove the common edge
            this.edges_data.remove_edge(containing_edge_id);

            // remove the two triangle
            this.triangles_data.remove_triangle(first_tri_id);
            this.triangles_data.remove_triangle(second_tri_id);

            // Order the IDS of edges and points
            List<int> tri_outside_four_edges_l = tri_outside_four_edges_h.ToList();

            // Variables to strore ordered output of edges and points
            List<int> tri_outside_four_edges = new List<int>();
            List<int> tri_outside_four_pts = new List<int>(); ;
            // Add the first pt
            tri_outside_four_pts.Add(edges_data.get_edge(tri_outside_four_edges_l.First()).start_pt_id);

            order_edges_and_points(ref tri_outside_four_edges_l, ref tri_outside_four_pts, ref tri_outside_four_edges);


            // add the four triangles
            int[] triangle_id = new int[4];
            triangle_id = add_four_triangles(pt,
                tri_outside_four_pts,
                tri_outside_four_edges);

            // Flip the bad triangles recursively
            flip_bad_edges(triangle_id[0], pt);
            flip_bad_edges(triangle_id[1], pt);
            flip_bad_edges(triangle_id[2], pt);
            flip_bad_edges(triangle_id[3], pt);
        }

        private int[] add_four_triangles(point_store new_pt, List<int> tri_corner_four_pts, List<int> tri_corner_four_edges)
        {
            // Add four new edges from the new four corner point
            int[] edge_indices = new int[4];
            // Add Edge 1
            point_store pt1 = points_data.get_point(tri_corner_four_pts[0]);
            double edge_len_1 = get_edge_length(new_pt.pt_coord, pt1.pt_coord);
            point_d mid_pt_1 = get_edge_midpt(new_pt.pt_coord, pt1.pt_coord);
            edge_indices[0] = edges_data.add_edge(new_pt.pt_id, pt1.pt_id, mid_pt_1, edge_len_1, is_boundary_edge(new_pt.pt_id, pt1.pt_id));

            // Add Edge 2
            point_store pt2 = points_data.get_point(tri_corner_four_pts[1]);
            double edge_len_2 = get_edge_length(new_pt.pt_coord, pt2.pt_coord);
            point_d mid_pt_2 = get_edge_midpt(new_pt.pt_coord, pt2.pt_coord);
            edge_indices[1] = edges_data.add_edge(new_pt.pt_id, pt2.pt_id, mid_pt_2, edge_len_2, is_boundary_edge(new_pt.pt_id, pt2.pt_id));

            // Add Edge 3
            point_store pt3 = points_data.get_point(tri_corner_four_pts[2]);
            double edge_len_3 = get_edge_length(new_pt.pt_coord, pt3.pt_coord);
            point_d mid_pt_3 = get_edge_midpt(new_pt.pt_coord, pt3.pt_coord);
            edge_indices[2] = edges_data.add_edge(new_pt.pt_id, pt3.pt_id, mid_pt_3, edge_len_3, is_boundary_edge(new_pt.pt_id, pt3.pt_id));

            // Add Edge 4
            point_store pt4 = points_data.get_point(tri_corner_four_pts[3]);
            double edge_len_4 = get_edge_length(new_pt.pt_coord, pt4.pt_coord);
            point_d mid_pt_4 = get_edge_midpt(new_pt.pt_coord, pt4.pt_coord);
            edge_indices[3] = edges_data.add_edge(new_pt.pt_id, pt4.pt_id, mid_pt_4, edge_len_4, is_boundary_edge(new_pt.pt_id, pt4.pt_id));
            //_________________________________________________________________________________


            // Add four triangles
            int[] output_indices = new int[4];
            point_d s_mid_pt, s_incircle_center;
            double s_circum_radius, c_edge_len, s_shortest_edge;

            // Add First triangle
            s_mid_pt = get_triangle_midpt(new_pt.pt_coord, pt1.pt_coord, pt2.pt_coord);
            s_incircle_center = get_triangle_incircle_center(new_pt.pt_coord, pt1.pt_coord, pt2.pt_coord);
            s_circum_radius = get_edge_length(new_pt.pt_coord, s_incircle_center);
            c_edge_len = edges_data.get_edge(tri_corner_four_edges[0]).edge_length;
            s_shortest_edge = get_minimum_of_three(edge_len_1, c_edge_len, edge_len_2);
            // Create the triangle 1
            output_indices[0] = triangles_data.add_triangle(new_pt.pt_id, pt1.pt_id, pt2.pt_id,
                edge_indices[0], tri_corner_four_edges[0], edge_indices[1],
                s_mid_pt, s_incircle_center,
                s_shortest_edge, s_circum_radius);

            // Add Second triangle
            s_mid_pt = get_triangle_midpt(new_pt.pt_coord, pt2.pt_coord, pt3.pt_coord);
            s_incircle_center = get_triangle_incircle_center(new_pt.pt_coord, pt2.pt_coord, pt3.pt_coord);
            s_circum_radius = get_edge_length(new_pt.pt_coord, s_incircle_center);
            c_edge_len = edges_data.get_edge(tri_corner_four_edges[1]).edge_length;
            s_shortest_edge = get_minimum_of_three(edge_len_2, c_edge_len, edge_len_3);
            // Create the triangle 2
            output_indices[1] = triangles_data.add_triangle(new_pt.pt_id, pt2.pt_id, pt3.pt_id,
                edge_indices[1], tri_corner_four_edges[1], edge_indices[2],
                s_mid_pt, s_incircle_center,
                s_shortest_edge, s_circum_radius);

            // Add Third triangle
            s_mid_pt = get_triangle_midpt(new_pt.pt_coord, pt3.pt_coord, pt4.pt_coord);
            s_incircle_center = get_triangle_incircle_center(new_pt.pt_coord, pt3.pt_coord, pt4.pt_coord);
            s_circum_radius = get_edge_length(new_pt.pt_coord, s_incircle_center);
            c_edge_len = edges_data.get_edge(tri_corner_four_edges[2]).edge_length;
            s_shortest_edge = get_minimum_of_three(edge_len_3, c_edge_len, edge_len_4);
            // Create the triangle 3
            output_indices[2] = triangles_data.add_triangle(new_pt.pt_id, pt3.pt_id, pt4.pt_id,
                edge_indices[2], tri_corner_four_edges[2], edge_indices[3],
                s_mid_pt, s_incircle_center,
                s_shortest_edge, s_circum_radius);


            // Add Fourth triangle
            s_mid_pt = get_triangle_midpt(new_pt.pt_coord, pt4.pt_coord, pt1.pt_coord);
            s_incircle_center = get_triangle_incircle_center(new_pt.pt_coord, pt4.pt_coord, pt1.pt_coord);
            s_circum_radius = get_edge_length(new_pt.pt_coord, s_incircle_center);
            c_edge_len = edges_data.get_edge(tri_corner_four_edges[3]).edge_length;
            s_shortest_edge = get_minimum_of_three(edge_len_4, c_edge_len, edge_len_1);
            // Create the triangle 4
            output_indices[3] = triangles_data.add_triangle(new_pt.pt_id, pt4.pt_id, pt1.pt_id,
                edge_indices[3], tri_corner_four_edges[3], edge_indices[0],
                s_mid_pt, s_incircle_center,
                s_shortest_edge, s_circum_radius);
            //_________________________________________________________________________________


            // Add the triangle details to the edge
            // Edge 1
            edges_data.associate_triangle_to_edge(edge_indices[0], output_indices[3], points_data, triangles_data);
            edges_data.associate_triangle_to_edge(edge_indices[0], output_indices[0], points_data, triangles_data);
            // Edge 2
            edges_data.associate_triangle_to_edge(edge_indices[1], output_indices[0], points_data, triangles_data);
            edges_data.associate_triangle_to_edge(edge_indices[1], output_indices[1], points_data, triangles_data);
            // Edge 3
            edges_data.associate_triangle_to_edge(edge_indices[2], output_indices[1], points_data, triangles_data);
            edges_data.associate_triangle_to_edge(edge_indices[2], output_indices[2], points_data, triangles_data);
            // Edge 4
            edges_data.associate_triangle_to_edge(edge_indices[3], output_indices[2], points_data, triangles_data);
            edges_data.associate_triangle_to_edge(edge_indices[3], output_indices[3], points_data, triangles_data);

            // Boundary Edge a
            edges_data.associate_triangle_to_edge(tri_corner_four_edges[0], output_indices[0], points_data, triangles_data);
            // Boundary Edge b
            edges_data.associate_triangle_to_edge(tri_corner_four_edges[1], output_indices[1], points_data, triangles_data);
            // Boundary Edge c
            edges_data.associate_triangle_to_edge(tri_corner_four_edges[2], output_indices[2], points_data, triangles_data);
            // Boundary Edge d
            edges_data.associate_triangle_to_edge(tri_corner_four_edges[3], output_indices[3], points_data, triangles_data);
            //_________________________________________________________________________________

            return output_indices;
        }

        private int[] add_three_triangles(point_store new_pt, List<int> tri_three_pts, List<int> tri_three_edges)
        {
            // Add three new edges from the new point
            int[] edge_indices = new int[3];

            // Add Edge 1
            point_store pt1 = points_data.get_point(tri_three_pts[0]);
            double edge_len_1 = get_edge_length(new_pt.pt_coord, pt1.pt_coord);
            point_d mid_pt_1 = get_edge_midpt(new_pt.pt_coord, pt1.pt_coord);
            edge_indices[0] = edges_data.add_edge(new_pt.pt_id, pt1.pt_id, mid_pt_1, edge_len_1, is_boundary_edge(new_pt.pt_id, pt1.pt_id));

            // Add Edge 2
            point_store pt2 = points_data.get_point(tri_three_pts[1]);
            double edge_len_2 = get_edge_length(new_pt.pt_coord, pt2.pt_coord);
            point_d mid_pt_2 = get_edge_midpt(new_pt.pt_coord, pt2.pt_coord);
            edge_indices[1] = edges_data.add_edge(new_pt.pt_id, pt2.pt_id, mid_pt_2, edge_len_2, is_boundary_edge(new_pt.pt_id, pt2.pt_id));

            // Add Edge 3
            point_store pt3 = points_data.get_point(tri_three_pts[2]);
            double edge_len_3 = get_edge_length(new_pt.pt_coord, pt3.pt_coord);
            point_d mid_pt_3 = get_edge_midpt(new_pt.pt_coord, pt3.pt_coord);
            edge_indices[2] = edges_data.add_edge(new_pt.pt_id, pt3.pt_id, mid_pt_3, edge_len_3, is_boundary_edge(new_pt.pt_id, pt3.pt_id));
            //_________________________________________________________________________________

            // Add three triangles
            int[] output_indices = new int[3];
            point_d s_mid_pt, s_incircle_center;
            double s_circum_radius, c_edge_len, s_shortest_edge;

            // Add First triangle
            s_mid_pt = get_triangle_midpt(new_pt.pt_coord, pt1.pt_coord, pt2.pt_coord);
            s_incircle_center = get_triangle_incircle_center(new_pt.pt_coord, pt1.pt_coord, pt2.pt_coord);
            s_circum_radius = get_edge_length(new_pt.pt_coord, s_incircle_center);
            c_edge_len = edges_data.get_edge(tri_three_edges[0]).edge_length;
            s_shortest_edge = get_minimum_of_three(edge_len_1, c_edge_len, edge_len_2);
            // Create the triangle 1
            output_indices[0] = triangles_data.add_triangle(new_pt.pt_id, pt1.pt_id, pt2.pt_id,
                edge_indices[0], tri_three_edges[0], edge_indices[1],
                s_mid_pt, s_incircle_center,
                s_shortest_edge, s_circum_radius);

            // Add Second triangle
            s_mid_pt = get_triangle_midpt(new_pt.pt_coord, pt2.pt_coord, pt3.pt_coord);
            s_incircle_center = get_triangle_incircle_center(new_pt.pt_coord, pt2.pt_coord, pt3.pt_coord);
            s_circum_radius = get_edge_length(new_pt.pt_coord, s_incircle_center);
            c_edge_len = edges_data.get_edge(tri_three_edges[1]).edge_length;
            s_shortest_edge = get_minimum_of_three(edge_len_2, c_edge_len, edge_len_3);
            // Create the triangle 2
            output_indices[1] = triangles_data.add_triangle(new_pt.pt_id, pt2.pt_id, pt3.pt_id,
                edge_indices[1], tri_three_edges[1], edge_indices[2],
                s_mid_pt, s_incircle_center,
                s_shortest_edge, s_circum_radius);

            // Add Third triangle
            s_mid_pt = get_triangle_midpt(new_pt.pt_coord, pt3.pt_coord, pt1.pt_coord);
            s_incircle_center = get_triangle_incircle_center(new_pt.pt_coord, pt3.pt_coord, pt1.pt_coord);
            s_circum_radius = get_edge_length(new_pt.pt_coord, s_incircle_center);
            c_edge_len = edges_data.get_edge(tri_three_edges[2]).edge_length;
            s_shortest_edge = get_minimum_of_three(edge_len_3, c_edge_len, edge_len_1);
            // Create the triangle 3
            output_indices[2] = triangles_data.add_triangle(new_pt.pt_id, pt3.pt_id, pt1.pt_id,
                edge_indices[2], tri_three_edges[2], edge_indices[0],
                s_mid_pt, s_incircle_center,
                s_shortest_edge, s_circum_radius);
            //_________________________________________________________________________________

            // Add the triangle details to the edge
            // Edge 1
            edges_data.associate_triangle_to_edge(edge_indices[0], output_indices[2], points_data, triangles_data);
            edges_data.associate_triangle_to_edge(edge_indices[0], output_indices[0], points_data, triangles_data);
            // Edge 2
            edges_data.associate_triangle_to_edge(edge_indices[1], output_indices[0], points_data, triangles_data);
            edges_data.associate_triangle_to_edge(edge_indices[1], output_indices[1], points_data, triangles_data);
            // Edge 3
            edges_data.associate_triangle_to_edge(edge_indices[2], output_indices[1], points_data, triangles_data);
            edges_data.associate_triangle_to_edge(edge_indices[2], output_indices[2], points_data, triangles_data);

            // Edge a
            edges_data.associate_triangle_to_edge(tri_three_edges[0], output_indices[0], points_data, triangles_data);
            // Edge b
            edges_data.associate_triangle_to_edge(tri_three_edges[1], output_indices[1], points_data, triangles_data);
            // Edge c
            edges_data.associate_triangle_to_edge(tri_three_edges[2], output_indices[2], points_data, triangles_data);

            //_________________________________________________________________________________

            return output_indices;
        }

        private int[] add_two_triangles(point_store c_pt, List<int> tri_corner_four_pts, List<int> tri_corner_four_edges)
        {
            // the new pair of triangles
            //
            //             pl
            //            /  \
            //         al/    \bl
            //          /      \
            //         /___a____\
            //    Ot_pt\---b----/c_pt
            //          \      /
            //         ar\    /br
            //            \  /
            //             pr
            //                              

            // add the only new common edge
            int edge_index;
            // Add Edge 
            point_store other_pt = points_data.get_point(tri_corner_four_pts[2]);
            double edge_len_n = get_edge_length(c_pt.pt_coord, other_pt.pt_coord);
            point_d mid_pt_n = get_edge_midpt(c_pt.pt_coord, other_pt.pt_coord);
            edge_index = edges_data.add_edge(c_pt.pt_id, other_pt.pt_id, mid_pt_n, edge_len_n, is_boundary_edge(c_pt.pt_id, other_pt.pt_id));
            //_________________________________________________________________________________

            // Add two triangles
            int[] output_indices = new int[2];
            point_d s_mid_pt, s_incircle_center;
            double s_circum_radius, s_shortest_edge;

            // Add First triangle
            // get point pl_pt 
            point_store pl_pt = points_data.get_point(tri_corner_four_pts[1]);
            // get edges bl_edge and al_edge
            edge_store bl_edge = edges_data.get_edge(tri_corner_four_edges[0]);
            edge_store al_edge = edges_data.get_edge(tri_corner_four_edges[1]);

            s_mid_pt = get_triangle_midpt(c_pt.pt_coord, pl_pt.pt_coord, other_pt.pt_coord);
            s_incircle_center = get_triangle_incircle_center(c_pt.pt_coord, pl_pt.pt_coord, other_pt.pt_coord);
            s_circum_radius = get_edge_length(c_pt.pt_coord, s_incircle_center);
            s_shortest_edge = get_minimum_of_three(bl_edge.edge_length, al_edge.edge_length, edge_len_n);

            // Create the triangle 1
            output_indices[0] = triangles_data.add_triangle(c_pt.pt_id, pl_pt.pt_id, other_pt.pt_id,
            bl_edge.edge_id, al_edge.edge_id, edge_index,
            s_mid_pt, s_incircle_center,
            s_shortest_edge, s_circum_radius);

            // Add Second triangle
            // get point pr_pt 
            point_store pr_pt = points_data.get_point(tri_corner_four_pts[3]);
            // get edges br_edge and ar_edge
            edge_store br_edge = edges_data.get_edge(tri_corner_four_edges[3]);
            edge_store ar_edge = edges_data.get_edge(tri_corner_four_edges[2]);

            s_mid_pt = get_triangle_midpt(c_pt.pt_coord, pr_pt.pt_coord, other_pt.pt_coord);
            s_incircle_center = get_triangle_incircle_center(c_pt.pt_coord, pr_pt.pt_coord, other_pt.pt_coord);
            s_circum_radius = get_edge_length(c_pt.pt_coord, s_incircle_center);
            s_shortest_edge = get_minimum_of_three(br_edge.edge_length, ar_edge.edge_length, edge_len_n);

            // Create the triangle 2
            output_indices[1] = triangles_data.add_triangle(c_pt.pt_id, pr_pt.pt_id, other_pt.pt_id,
            br_edge.edge_id, ar_edge.edge_id, edge_index,
            s_mid_pt, s_incircle_center,
            s_shortest_edge, s_circum_radius);
            //_________________________________________________________________________________

            // Add the triangle details to the edge
            // Common Edge
            edges_data.associate_triangle_to_edge(edge_index, output_indices[0], points_data, triangles_data);
            edges_data.associate_triangle_to_edge(edge_index, output_indices[1], points_data, triangles_data);

            // Boundary Edge bl
            edges_data.associate_triangle_to_edge(bl_edge.edge_id, output_indices[0], points_data, triangles_data);
            // Boundary Edge al
            edges_data.associate_triangle_to_edge(al_edge.edge_id, output_indices[0], points_data, triangles_data);
            // Boundary Edge br
            edges_data.associate_triangle_to_edge(br_edge.edge_id, output_indices[1], points_data, triangles_data);
            // Boundary Edge ar
            edges_data.associate_triangle_to_edge(ar_edge.edge_id, output_indices[1], points_data, triangles_data);
            //_________________________________________________________________________________

            return output_indices;
        }

        private bool is_boundary_edge(int pt1_id, int pt2_id)
        {
            // Returns whether the edge is part of surface boundary
            if(this.points_data.get_point(pt1_id).pt_type == 3 ||
                this.points_data.get_point(pt2_id).pt_type ==3)
            {
                return false;
            }
            return true;
        }

        public void order_edges_and_points(ref List<int> tri_corner_edges, ref List<int> t_c_pts, ref List<int> t_c_edges)
        {
            if (tri_corner_edges.Count == 0)
            {
                // Condition to end recursion
                return;
            }

            // Find the edge containing the point
            int newly_added_pt = t_c_pts[t_c_pts.Count - 1];

            // Cycle through edges and find the edge containing the point
            foreach (int edge_id in tri_corner_edges)
            {
                edge_store ed1 = edges_data.get_edge(edge_id);

                if (ed1.start_pt_id == newly_added_pt)
                {
                    // Add the other point of the edge to the list (which is endpt in this case)
                    t_c_pts.Add(ed1.end_pt_id);
                    // Add the edge to the list
                    t_c_edges.Add(edge_id);
                    break;
                }
                else if (ed1.end_pt_id == newly_added_pt)
                {
                    // Add the other point of the edge to the list (which is startpt in this case)
                    t_c_pts.Add(ed1.start_pt_id);
                    // Add the edge to the list
                    t_c_edges.Add(edge_id);
                    break;
                }
            }

            tri_corner_edges.Remove(t_c_edges[t_c_edges.Count - 1]);

            // recursion
            order_edges_and_points(ref tri_corner_edges, ref t_c_pts, ref t_c_edges);
        }

        private void flip_bad_edges(int tri_id, point_store pt)
        {
            //flip recursively for the new pair of triangles
            //
            //           pl                    pl
            //          /||\                  /  \
            //       al/ || \bl            al/    \bl
            //        /  ||  \              /      \
            //       /  a||b  \    flip    /___a____\
            //     p0\   ||   /pt   =>   p0\---b----/pt
            //        \  ||  /              \      /
            //       ar\ || /br            ar\    /br
            //          \||/                  \  /
            //           p2                    p2
            //
            // Step:1 find the edge of this triangle which does not contain pt
            // ___edge____
            // \         /
            //  \       /
            //   \     /
            //    \   /
            //      pt

            (int e1, int e2, int e3) tri_edges = triangles_data.get_specific_triangle_edge_ids(tri_id);
            int common_edge_id = -1;

            if (edges_data.is_specific_edge_with_endpt(tri_edges.e1, pt.pt_id) == false)
            {
                common_edge_id = tri_edges.e1;
            }
            else if (edges_data.is_specific_edge_with_endpt(tri_edges.e2, pt.pt_id) == false)
            {
                common_edge_id = tri_edges.e2;
            }
            else if (edges_data.is_specific_edge_with_endpt(tri_edges.e3, pt.pt_id) == false)
            {
                common_edge_id = tri_edges.e3;
            }

            // Find the neighbour tri index
            int neighbour_tri_id = edges_data.get_specific_edge_other_triangle_id(common_edge_id, tri_id);

            // legalize only if the triangle has a neighbour
            if (neighbour_tri_id != -1)
            {
                // Collect the 3 edges of neighbour triangle
                (int e1, int e2, int e3) neighbour_tri_edges = this.triangles_data.get_specific_triangle_edge_ids(neighbour_tri_id);

                // check whether the newly added pt is inside the neighbour triangle circum circle
                if (triangles_data.is_point_in_specific_triangle_circumcircle(neighbour_tri_id, pt.pt_coord) == true)
                {
                    HashSet<int> tri_outside_four_edges_h = new HashSet<int>();

                    // Only 5 will be added (because 1 edge is common)
                    tri_outside_four_edges_h.Add(tri_edges.e1);
                    tri_outside_four_edges_h.Add(tri_edges.e2);
                    tri_outside_four_edges_h.Add(tri_edges.e3);
                    tri_outside_four_edges_h.Add(neighbour_tri_edges.e1);
                    tri_outside_four_edges_h.Add(neighbour_tri_edges.e2);
                    tri_outside_four_edges_h.Add(neighbour_tri_edges.e3);

                    // Remove the 1 common edge
                    tri_outside_four_edges_h.Remove(common_edge_id);

                    if (tri_outside_four_edges_h.Count != 4)
                    {
                        System.Windows.Forms.MessageBox.Show("Error!! Some points are closer than tolerance", "Error");
                        return;
                    }
                    // Remove the common edge
                    this.edges_data.remove_edge(common_edge_id);

                    // Remove the two triangles
                    this.triangles_data.remove_triangle(tri_id);
                    this.triangles_data.remove_triangle(neighbour_tri_id);

                    // Order the IDS of edges and points
                    List<int> tri_outside_four_edges_l = tri_outside_four_edges_h.ToList();

                    // Variables to strore ordered output of edges and points
                    List<int> tri_outside_four_edges = new List<int>();
                    List<int> tri_outside_four_pts = new List<int>(); ;
                    // Add the first pt
                    tri_outside_four_pts.Add(pt.pt_id);

                    order_edges_and_points(ref tri_outside_four_edges_l, ref tri_outside_four_pts, ref tri_outside_four_edges);

                    // Add new two triangles
                    int[] triangle_id = new int[2];
                    triangle_id = add_two_triangles(pt, tri_outside_four_pts, tri_outside_four_edges);

                    // recursion below
                    flip_bad_edges(triangle_id[0], pt);
                    flip_bad_edges(triangle_id[1], pt);
                }
            }
        }

        public void flip_non_constrained_edges(int tri_id, point_store pt,int bk_pt)
        {
            //flip recursively for the new pair of triangles
            //
            //           pl                    pl
            //          /||\                  /  \
            //       al/ || \bl            al/    \bl
            //        /  ||  \              /      \
            //       /  a||b  \    flip    /___a____\
            //     p0\   ||   /pt   =>   p0\---b----/pt
            //        \  ||  /              \      /
            //       ar\ || /br            ar\    /br
            //          \||/                  \  /
            //           p2                    p2
            //
            // Step:1 find the edge of this triangle which does not contain pt
            // ___edge____
            // \         /
            //  \       /
            //   \     /
            //    \   /
            //      pt

            (int e1, int e2, int e3) tri_edges = triangles_data.get_specific_triangle_edge_ids(tri_id);
            int common_edge_id = -1;

            if (edges_data.is_specific_edge_with_endpt(tri_edges.e1, pt.pt_id) == false)
            {
                common_edge_id = tri_edges.e1;
            }
            else if (edges_data.is_specific_edge_with_endpt(tri_edges.e2, pt.pt_id) == false)
            {
                common_edge_id = tri_edges.e2;
            }
            else if (edges_data.is_specific_edge_with_endpt(tri_edges.e3, pt.pt_id) == false)
            {
                common_edge_id = tri_edges.e3;
            }

            // Find the neighbour tri index
            int neighbour_tri_id = edges_data.get_specific_edge_other_triangle_id(common_edge_id, tri_id);

            // legalize only if the triangle has a neighbour
            if (neighbour_tri_id != -1)
            {
                // Collect the 3 edges of neighbour triangle
                (int e1, int e2, int e3) neighbour_tri_edges = this.triangles_data.get_specific_triangle_edge_ids(neighbour_tri_id);

                // No condition for constrained triangle
                // check whether the newly added pt is inside the neighbour triangle circum circle
                //if (triangles_data.is_point_in_specific_triangle_circumcircle(neighbour_tri_id, pt.pt_coord) == true)
                //{
                    HashSet<int> tri_outside_four_edges_h = new HashSet<int>();

                    // Only 5 will be added (because 1 edge is common)
                    tri_outside_four_edges_h.Add(tri_edges.e1);
                    tri_outside_four_edges_h.Add(tri_edges.e2);
                    tri_outside_four_edges_h.Add(tri_edges.e3);
                    tri_outside_four_edges_h.Add(neighbour_tri_edges.e1);
                    tri_outside_four_edges_h.Add(neighbour_tri_edges.e2);
                    tri_outside_four_edges_h.Add(neighbour_tri_edges.e3);

                    // Remove the 1 common edge
                    tri_outside_four_edges_h.Remove(common_edge_id);

                    if (tri_outside_four_edges_h.Count != 4)
                    {
                        System.Windows.Forms.MessageBox.Show("Error!! Some points are closer than tolerance", "Error");
                        return;
                    }
                    // Remove the common edge
                    this.edges_data.remove_edge(common_edge_id);

                    // Remove the two triangles
                    this.triangles_data.remove_triangle(tri_id);
                    this.triangles_data.remove_triangle(neighbour_tri_id);

                    // Order the IDS of edges and points
                    List<int> tri_outside_four_edges_l = tri_outside_four_edges_h.ToList();

                    // Variables to strore ordered output of edges and points
                    List<int> tri_outside_four_edges = new List<int>();
                    List<int> tri_outside_four_pts = new List<int>(); ;
                    // Add the first pt
                    tri_outside_four_pts.Add(pt.pt_id);

                    order_edges_and_points(ref tri_outside_four_edges_l, ref tri_outside_four_pts, ref tri_outside_four_edges);

                    // Add new two triangles
                    int[] triangle_id = new int[2];
                    triangle_id = add_two_triangles(pt, tri_outside_four_pts, tri_outside_four_edges);

                if (bk_pt == 13)
                    return;

                // recursion below
                flip_bad_edges(triangle_id[0], pt);
                flip_bad_edges(triangle_id[1], pt);
                //}
            }
        }



        private void set_bounding_triangle(List<point_d> all_input_vertices)
        {
            point_d[] x_sorted = all_input_vertices.OrderBy(obj => obj.x).ToArray();
            point_d[] y_sorted = all_input_vertices.OrderBy(obj => obj.y).ToArray();

            // Define bounding triangle
            double max_x, max_y, k;
            max_x = (x_sorted[x_sorted.Length - 1].x - x_sorted[0].x);
            max_y = (y_sorted[y_sorted.Length - 1].y - y_sorted[0].y);
            k = 100 * Math.Max(max_x, max_y);

            // zeoth _point
            double x_zero, y_zero;
            x_zero = (x_sorted[x_sorted.Length - 1].x + x_sorted[0].x) * 0.5;
            y_zero = (y_sorted[y_sorted.Length - 1].y + y_sorted[0].y) * 0.5;

            // id for the super triangle points
            int pt_count = all_input_vertices.Count;

            // set the vertex
            point_store s_p1 = new point_store(0, 0, Math.Round(k / 2.0f), 3);
            point_store s_p2 = new point_store(1, Math.Round(k / 2.0f), 0.0, 3);
            point_store s_p3 = new point_store(2, -1 * Math.Round(k / 2.0f), -1 * Math.Round(k / 2.0f), 3);

            // Add the supertriangle pts to pts_data
            this.points_data.add_point(s_p1.pt_coord, 3);
            this.points_data.add_point(s_p2.pt_coord, 3);
            this.points_data.add_point(s_p3.pt_coord, 3);

            // Add three new edges for the triangle from the three new point
            int[] stri_edges_id = new int[3];
            // Add Edge 1
            double edge_len_1 = get_edge_length(s_p1.pt_coord, s_p2.pt_coord);
            point_d mid_pt_1 = get_edge_midpt(s_p1.pt_coord, s_p2.pt_coord);
            stri_edges_id[0] = edges_data.add_edge(s_p1.pt_id, s_p2.pt_id, mid_pt_1, edge_len_1,false);

            // Add Edge 2
            double edge_len_2 = get_edge_length(s_p2.pt_coord, s_p3.pt_coord);
            point_d mid_pt_2 = get_edge_midpt(s_p2.pt_coord, s_p3.pt_coord);
            stri_edges_id[1] = edges_data.add_edge(s_p2.pt_id, s_p3.pt_id, mid_pt_2, edge_len_2, false);

            // Add Edge 3
            double edge_len_3 = get_edge_length(s_p3.pt_coord, s_p1.pt_coord);
            point_d mid_pt_3 = get_edge_midpt(s_p3.pt_coord, s_p1.pt_coord);
            stri_edges_id[2] = edges_data.add_edge(s_p3.pt_id, s_p1.pt_id, mid_pt_3, edge_len_3, false);
            //_________________________________________________________________________________


            point_d s_mid_pt = get_triangle_midpt(s_p1.pt_coord, s_p2.pt_coord, s_p3.pt_coord);
            point_d s_incircle_center = get_triangle_incircle_center(s_p1.pt_coord, s_p2.pt_coord, s_p3.pt_coord);
            double s_circum_radius = get_edge_length(s_p1.pt_coord, s_incircle_center);
            double s_shortest_edge = get_minimum_of_three(edge_len_1, edge_len_2, edge_len_3);
            // Create the super triangle
            int sup_tri_index;
            sup_tri_index = triangles_data.add_triangle(s_p1.pt_id, s_p2.pt_id, s_p3.pt_id,
                stri_edges_id[0], stri_edges_id[1], stri_edges_id[2],
                s_mid_pt, s_incircle_center,
                s_shortest_edge, s_circum_radius);

            // Add the triangle details to the outter edges
            // Edge 1
            edges_data.associate_triangle_to_edge(stri_edges_id[0], sup_tri_index, points_data, triangles_data);
            // Edge 2
            edges_data.associate_triangle_to_edge(stri_edges_id[1], sup_tri_index, points_data, triangles_data);
            // Edge 3
            edges_data.associate_triangle_to_edge(stri_edges_id[2], sup_tri_index, points_data, triangles_data);

            // Super triangle creation complete
            //_________________________________________________________________________________
        }

        public static point_d get_triangle_midpt(point_d pt1, point_d pt2, point_d pt3)
        {
            // Mid point of triangle
            return new point_d((pt1.x + pt2.x + pt3.x) / 3.0f, (pt1.y + pt2.y + pt3.y) / 3.0f);
        }

        public static point_d get_edge_midpt(point_d pt1, point_d pt2)
        {
            // Mid point of triangle
            return new point_d((pt1.x + pt2.x) / 2.0f, (pt1.y + pt2.y) / 2.0f);
        }

        public static point_d get_triangle_incircle_center(point_d pt1, point_d pt2, point_d pt3)
        {
            double dA = (pt1.x * pt1.x) + (pt1.y * pt1.y);
            double dB = (pt2.x * pt2.x) + (pt2.y * pt2.y);
            double dC = (pt3.x * pt3.x) + (pt3.y * pt3.y);

            double aux1 = (dA * (pt3.y - pt2.y) + dB * (pt1.y - pt3.y) + dC * (pt2.y - pt1.y));
            double aux2 = -(dA * (pt3.x - pt2.x) + dB * (pt1.x - pt3.x) + dC * (pt2.x - pt1.x));
            double div = (2 * (pt1.x * (pt3.y - pt2.y) + pt2.x * (pt1.y - pt3.y) + pt3.x * (pt2.y - pt1.y)));

            //Circumcircle
            double center_x = aux1 / div;
            double center_y = aux2 / div;

            return new point_d(center_x, center_y);
        }

        public static double get_edge_length(point_d pt1, point_d pt2)
        {
            // get distance between two point
            return Math.Sqrt((pt1.x - pt2.x) * (pt1.x - pt2.x) + (pt1.y - pt2.y) * (pt1.y - pt2.y));
        }

        public static double get_minimum_of_three(double d1, double d2, double d3)
        {
            return d1 < d2 ? (d1 < d3 ? d1 : d3) : (d2 < d3 ? d2 : d3);
        }

    }
}
