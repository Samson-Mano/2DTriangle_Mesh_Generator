using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation
{
    public class mesh_store
    {
        private delaunay_triangulation BW_delaunay;

        public point_list_store result_pts_data { get; private set; }

        public edge_list_store result_edge_data { get; private set; }

        public triangle_list_store result_tri_data { get; private set; }

        public mesh_store(List<point_d> corner_points, List<point_d> edge_pts, List<point_d> outter_bndry_pts, List<List<point_d>> inner_bndry_pts)
        {
            // Empty constructor
            BW_delaunay = new delaunay_triangulation();

            // spacing scale increases the points gap
            double spacing_scale = 1000.0f;

            // scale the points
            List<point_d> scaled_corner_points = new List<point_d>();
            List<point_d> scaled_edge_points = new List<point_d>();
            List<point_d> scaled_outter_bndry_pts = new List<point_d>();
            List<List<point_d>> scaled_inner_bndry_pts = new List<List<point_d>>();

            // Scale corner points and add to list
            foreach (point_d pt in corner_points)
            {
                scaled_corner_points.Add(new point_d(pt.x * spacing_scale, pt.y * spacing_scale));
            }

            // Scale edge points and add to list
            foreach (point_d pt in edge_pts)
            {
                scaled_edge_points.Add(new point_d(pt.x * spacing_scale, pt.y * spacing_scale));
            }

            // Scale outter boundary points and add to list
            foreach (point_d pt in outter_bndry_pts)
            {
                scaled_outter_bndry_pts.Add(new point_d(pt.x * spacing_scale, pt.y * spacing_scale));
            }

            // Scale the inner boundary points and add to list
            foreach (List<point_d> i_bndry_pts in inner_bndry_pts)
            {
                List<point_d> sc_pt = new List<point_d>();
                foreach (point_d pt in i_bndry_pts)
                {
                    sc_pt.Add(new point_d(pt.x * spacing_scale, pt.y * spacing_scale));
                }
                scaled_inner_bndry_pts.Add(sc_pt);
            }

            // Create a boundary rectangle
            List<point_d> bndry_pts_x_sort = new List<point_d>();
            List<point_d> bndry_pts_y_sort = new List<point_d>();

            point_d[] bndry_rectangle = new point_d[3];
            bndry_pts_x_sort = scaled_outter_bndry_pts.OrderBy(obj => obj.x).ToList();
            bndry_pts_y_sort = scaled_outter_bndry_pts.OrderBy(obj => obj.y).ToList();

            // Boundary rectangle
            // 1---
            // |   |
            // |   |
            // 0---2
            bndry_rectangle[0] = new point_d(bndry_pts_x_sort[0].x, bndry_pts_y_sort[0].y);
            bndry_rectangle[1] = new point_d(bndry_pts_x_sort[0].x, bndry_pts_y_sort[bndry_pts_y_sort.Count - 1].y);
            bndry_rectangle[2] = new point_d(bndry_pts_x_sort[bndry_pts_x_sort.Count - 1].x, bndry_pts_y_sort[0].y);

            // Create super triangle
            BW_delaunay.create_super_triangle(scaled_edge_points, scaled_corner_points);

            // Create mesh for all the fixed points (Outter boundary and inner boundary points)
            BW_delaunay.Add_multiple_points(scaled_corner_points, 1);
            BW_delaunay.Add_multiple_points(scaled_edge_points, 2);

            // Setup well sized triangle
            // Parameter 1: Chew's first algorithm B=1, Ruppert's B=Sqrt(2), Chew's second algorithm B=Sqrt(5)/2
            double B_var = Math.Sqrt(2);
            // Parameter 2: h is the desired side length of triangle in the triangulation (user input)
            double H_var = BW_delaunay.triangles_data.get_triangle_shortest_edge() * 2.0f;

            // Find and queue bad triangles
            triangle_store bad_triangle = BW_delaunay.triangles_data.get_bad_triangle(bndry_rectangle, B_var, H_var);
            int inf_loop_check = 1;

            while (bad_triangle != null)
            {
                if (inf_loop_check > 2000000)
                {
                    // Stuck in infinite loop (or too many element count)
                    break;
                }

                BW_delaunay.Add_single_point(bad_triangle.circum_center, 3);

                bad_triangle = BW_delaunay.triangles_data.get_bad_triangle(bndry_rectangle, B_var, H_var);
                inf_loop_check++;
            }

            //goto ineligible;

            // Create constrained triangulation
            // Get all the input constrained edges
            edge_list_store inpt_constrained_edges = get_constrained_edges(BW_delaunay.points_data, scaled_outter_bndry_pts, scaled_inner_bndry_pts);

            // Intersect with triangulated boundary edges
            //  edge_list_store missing_constrained_edges = get_missing_constrained_edges(BW_delaunay.edges_data, inpt_constrained_edges);
            List<edge_store> missing_constrained_edges = get_missing_constrained_edges(BW_delaunay.edges_data, inpt_constrained_edges);

            inf_loop_check = 1;
            while (missing_constrained_edges.Count > 0)
            {
                if (inf_loop_check == 200000)
                {
                    // Stuck in infinite loop (or too many element count)
                    break;
                }

                // Fix the edge one after another starting from [0]
                (int tri_id, point_store pt) intersecting_data = get_constrained_intersection_data(BW_delaunay, missing_constrained_edges[0]);

                // Flip the constrained edge intersecting bad edge
                BW_delaunay.flip_non_constrained_edges(intersecting_data.tri_id, intersecting_data.pt, inf_loop_check);

                // Re-populate missing constrained edges
                missing_constrained_edges = get_missing_constrained_edges(BW_delaunay.edges_data, inpt_constrained_edges);

                inf_loop_check++;
            }

            // Finalize mesh(by removing super triangles)
            remove_super_triangle();

            // Remove the mesh out side the boundary
            (List<int> pts, List<int> edg, List<int> tri) final_mesh_data = remove_triangles_outside_surface(scaled_outter_bndry_pts, scaled_inner_bndry_pts);


            //ineligible:

            // Scale back the data and add to output
            this.result_pts_data = new point_list_store();
            this.result_edge_data = new edge_list_store();
            this.result_tri_data = new triangle_list_store();

            // Renumber the ids of the mesh data
            Dictionary<int, int> pt_id_data = new Dictionary<int, int>();
            Dictionary<int, int> edge_id_data = new Dictionary<int, int>();
            // Dictionary<int, int> tri_id_data = new Dictionary<int, int>();


            // Add the points to the result point list
            foreach (int existing_pt_id in final_mesh_data.pts)
            {
                point_store pt = BW_delaunay.points_data.get_point(existing_pt_id);
                // Scale back to original scale (remove the spacing gap from original)
                point_d sc_t_pt = new point_d(pt.pt_coord.x / spacing_scale, pt.pt_coord.y / spacing_scale);

                int new_pt_id = this.result_pts_data.add_point(sc_t_pt, pt.pt_type);
                pt_id_data.Add(existing_pt_id, new_pt_id);
            }

            // Add the edges to the result edge list
            List<edge_store> inpt_constrained_edges_list = inpt_constrained_edges.get_all_edges();

            foreach (int existing_edge_id in final_mesh_data.edg)
            {
                edge_store ed = BW_delaunay.edges_data.get_edge(existing_edge_id);
                int pt1_id = pt_id_data[ed.start_pt_id];
                int pt2_id = pt_id_data[ed.end_pt_id];

                bool is_bndry_edge = false;
                if(ed.is_boundary_edge == true)
                {
                    // Confirm the boundary edge
                    if (inpt_constrained_edges_list.Exists(obj => (obj.start_pt_id == ed.start_pt_id && obj.end_pt_id == ed.end_pt_id)||
                    (obj.start_pt_id == ed.end_pt_id && obj.end_pt_id == ed.start_pt_id)) == true)
                    {
                        is_bndry_edge = true;
                    }
                }


                int new_ed_id = this.result_edge_data.add_edge(pt1_id, pt2_id, ed.mid_pt, ed.edge_length, is_bndry_edge);
                edge_id_data.Add(existing_edge_id, new_ed_id);
            }

            // Add the triangles to the result triangle list
            foreach (int existing_tri_id in final_mesh_data.tri)
            {
                triangle_store tr = BW_delaunay.triangles_data.get_triangle(existing_tri_id);
                int pt1_id = pt_id_data[tr.pt1_id];
                int pt2_id = pt_id_data[tr.pt2_id];
                int pt3_id = pt_id_data[tr.pt3_id];

                int ed1_id = edge_id_data[tr.e1_id];
                int ed2_id = edge_id_data[tr.e2_id];
                int ed3_id = edge_id_data[tr.e3_id];

                this.result_tri_data.add_triangle(pt1_id, pt2_id, pt3_id, ed1_id, ed2_id, ed3_id, tr.mid_pt, tr.circum_center, tr.shortest_edge_length, tr.circum_radius);
                // edge_id_data.Add(existing_edge_id, new_ed_id);
            }

        }

        private (int tri_id, point_store c_pt) get_constrained_intersection_data(delaunay_triangulation BW_delaunay, edge_store missing_constrained_edge)
        {
            // Returns the triangle id and other point of the edge which is intersecting the constrained edge
            // start with missing constrained edge start point
            int pt_id = missing_constrained_edge.start_pt_id;

            // Find all the edges attached to the start point
            List<edge_store> tri_edges = new List<edge_store>();
            foreach (edge_store ed in BW_delaunay.edges_data.get_all_edges())
            {
                if (ed.start_pt_id == pt_id || ed.end_pt_id == pt_id)
                {
                    tri_edges.Add(ed);
                }
            }

            // Get all the traingle associated with these edges
            HashSet<int> tri_ids = new HashSet<int>();

            foreach (edge_store t_edge in tri_edges)
            {
                // Add to hashset because we only wany unique triangles
                tri_ids.Add(t_edge.left_triangle_id);
                tri_ids.Add(t_edge.right_triangle_id);
            }


            // Check whether the other edges of triangle from startpoint intersects with the constrained edge
            // Create line seqment AB (Apt -> Bpt)
            point_d A_pt = BW_delaunay.points_data.get_point(missing_constrained_edge.start_pt_id).pt_coord;
            point_d B_pt = BW_delaunay.points_data.get_point(missing_constrained_edge.end_pt_id).pt_coord;

            // Cycle through the triangles (surrounding the missing constrained edge start point)
            foreach (int t_id in tri_ids.ToList())
            {
                // Get the other edge from the missing constrained edge start point
                // Step:1 find the edge of this triangle which does not contain pt
                // ___edge____
                // \         /
                //  \       /
                //   \     /
                //    \   /
                //      pt

                (int e1, int e2, int e3) tri_edges_id = BW_delaunay.triangles_data.get_specific_triangle_edge_ids(t_id);
                int common_edge_id = -1;

                if (BW_delaunay.edges_data.is_specific_edge_with_endpt(tri_edges_id.e1, pt_id) == false)
                {
                    common_edge_id = tri_edges_id.e1;
                }
                else if (BW_delaunay.edges_data.is_specific_edge_with_endpt(tri_edges_id.e2, pt_id) == false)
                {
                    common_edge_id = tri_edges_id.e2;
                }
                else if (BW_delaunay.edges_data.is_specific_edge_with_endpt(tri_edges_id.e3, pt_id) == false)
                {
                    common_edge_id = tri_edges_id.e3;
                }

                // Create the line segment QR (Qpt -> Rpt) 
                edge_store potential_intersecting_edge = BW_delaunay.edges_data.get_edge(common_edge_id);

                point_d Q_pt = BW_delaunay.points_data.get_point(potential_intersecting_edge.start_pt_id).pt_coord;
                point_d R_pt = BW_delaunay.points_data.get_point(potential_intersecting_edge.end_pt_id).pt_coord;

                // Check whether AB intersects QR
                if (edge_list_store.is_two_lines_intersect(A_pt, B_pt, Q_pt, R_pt) == true)
                {
                    return (t_id, BW_delaunay.points_data.get_point(missing_constrained_edge.start_pt_id));
                }
            }

            // Nothing found (throws exception)
            return (-1, null);
        }

        private edge_list_store get_constrained_edges(point_list_store pt_d, List<point_d> sc_outter_bndry_pts, List<List<point_d>> sc_inner_bndry_pts)
        {
            // point_list_store pt_d = new point_list_store();
            edge_list_store ed_d = new edge_list_store();

            int i;
            int pt_id1, pt_id2;
            double ed_length;
            point_d m_pt;
            // store the start pt and end pt id to close the edge loop
            int start_pt_id = -1, end_pt_id = -1;

            // Add points and edges for constrained outter boundary edge
            for (i = 0; i < sc_outter_bndry_pts.Count - 1; i++)
            {
                pt_id1 = pt_d.get_boundary_point_id(sc_outter_bndry_pts[i]);
                pt_id2 = pt_d.get_boundary_point_id(sc_outter_bndry_pts[i + 1]);
                ed_length = delaunay_triangulation.get_edge_length(sc_outter_bndry_pts[i], sc_outter_bndry_pts[i + 1]);
                m_pt = delaunay_triangulation.get_edge_midpt(sc_outter_bndry_pts[i], sc_outter_bndry_pts[i + 1]);

                ed_d.add_edge(pt_id1, pt_id2, m_pt, ed_length, true);

                if (i == 0)
                {
                    start_pt_id = pt_id1;
                }

                if (i == sc_outter_bndry_pts.Count - 2)
                {
                    end_pt_id = pt_id2;
                }
            }

            // Add the last edge
            pt_id1 = end_pt_id;
            pt_id2 = start_pt_id;
            ed_length = delaunay_triangulation.get_edge_length(sc_outter_bndry_pts[sc_outter_bndry_pts.Count - 1], sc_outter_bndry_pts[0]);
            m_pt = delaunay_triangulation.get_edge_midpt(sc_outter_bndry_pts[sc_outter_bndry_pts.Count - 1], sc_outter_bndry_pts[0]);

            ed_d.add_edge(pt_id1, pt_id2, m_pt, ed_length, true);

            // Add points and edges for constrained inner boundary edge
            foreach (List<point_d> sc_i_bdnry_pts in sc_inner_bndry_pts)
            {
                for (i = 0; i < sc_i_bdnry_pts.Count - 1; i++)
                {
                    pt_id1 = pt_d.get_boundary_point_id(sc_i_bdnry_pts[i]);
                    pt_id2 = pt_d.get_boundary_point_id(sc_i_bdnry_pts[i + 1]);
                    ed_length = delaunay_triangulation.get_edge_length(sc_i_bdnry_pts[i], sc_i_bdnry_pts[i + 1]);
                    m_pt = delaunay_triangulation.get_edge_midpt(sc_i_bdnry_pts[i], sc_i_bdnry_pts[i + 1]);

                    ed_d.add_edge(pt_id1, pt_id2, m_pt, ed_length, true);

                    if (i == 0)
                    {
                        start_pt_id = pt_id1;
                    }

                    if (i == sc_i_bdnry_pts.Count - 2)
                    {
                        end_pt_id = pt_id2;
                    }
                }

                // Add the last edge
                pt_id1 = end_pt_id;
                pt_id2 = start_pt_id;
                ed_length = delaunay_triangulation.get_edge_length(sc_i_bdnry_pts[sc_i_bdnry_pts.Count - 1], sc_i_bdnry_pts[0]);
                m_pt = delaunay_triangulation.get_edge_midpt(sc_i_bdnry_pts[sc_i_bdnry_pts.Count - 1], sc_i_bdnry_pts[0]);

                ed_d.add_edge(pt_id1, pt_id2, m_pt, ed_length, true);
            }

            // Return the points and edges data
            return ed_d;
        }

        private List<edge_store> get_missing_constrained_edges(edge_list_store ed_d, edge_list_store constrained_edges)
        {
            List<edge_store> missing_edge_list = new List<edge_store>(constrained_edges.get_all_edges());

            // Cycle through delunay edges
            foreach (edge_store d_edge in ed_d.get_all_edges())
            {
                // Check for only boundary edges
                if (d_edge.is_boundary_edge == false)
                    continue;


                int r_edge_id = -1;
                bool is_found = false;
                foreach (edge_store c_edge in constrained_edges.get_all_edges())
                {
                    if ((d_edge.start_pt_id == c_edge.start_pt_id && d_edge.end_pt_id == c_edge.end_pt_id) ||
                        (d_edge.start_pt_id == c_edge.end_pt_id && d_edge.end_pt_id == c_edge.start_pt_id))
                    {
                        r_edge_id = c_edge.edge_id;
                        is_found = true;
                        break;
                    }
                }

                // delete the constrained edge because it already part of the delaunay triangulation
                if (is_found == true)
                {
                    missing_edge_list.Remove(missing_edge_list.Find(obj => obj.Equals(r_edge_id)));
                }
            }

            return missing_edge_list;
        }

        private void remove_super_triangle()
        {
            // remove all the edges attached to point 0,1,2
            HashSet<int> delete_edge = new HashSet<int>();

            foreach (edge_store edge in BW_delaunay.edges_data.get_all_edges())
            {
                if (edge.start_pt_id == 0 || edge.end_pt_id == 0 ||
                   edge.start_pt_id == 1 || edge.end_pt_id == 1 ||
                   edge.start_pt_id == 2 || edge.end_pt_id == 2)
                {
                    delete_edge.Add(edge.edge_id);
                }
            }

            // remove the 3 super triangle pts
            //BW_delaunay.points_data.remove_point(0);
            //BW_delaunay.points_data.remove_point(1);
            //BW_delaunay.points_data.remove_point(2);

            // Remove all the triangles attached to the edges
            HashSet<int> delete_triangles = new HashSet<int>();

            foreach (int edge_id in delete_edge)
            {
                edge_store ed = BW_delaunay.edges_data.get_edge(edge_id);

                if (ed.left_triangle_id != -1)
                {
                    delete_triangles.Add(ed.left_triangle_id);
                }

                if (ed.right_triangle_id != -1)
                {
                    delete_triangles.Add(ed.right_triangle_id);
                }

                // Deleting the edges here
                BW_delaunay.edges_data.remove_edge(edge_id);
            }

            foreach (int tri_id in delete_triangles)
            {
                // Deleting the triangles here
                BW_delaunay.triangles_data.remove_triangle(tri_id);
            }

        }

        private (List<int> pts, List<int> edg, List<int> tri) remove_triangles_outside_surface(List<point_d> sc_outter_bndry_pts, List<List<point_d>> sc_inner_bndry_pts)
        {
            // Find all the triangles which are inside the meshed surface
            HashSet<int> result_tri_ids = new HashSet<int>();

            foreach (triangle_store tri in BW_delaunay.triangles_data.get_all_triangles())
            {
                if (is_point_in_surface(tri.mid_pt, sc_outter_bndry_pts, sc_inner_bndry_pts) == true)
                {
                    // Add the tri id which is inside the surface
                    result_tri_ids.Add(tri.tri_id);
                }
            }

            // Find all the edges which are added to the triangle
            HashSet<int> result_edg_ids = new HashSet<int>();
            HashSet<int> result_pts_ids = new HashSet<int>();

            foreach (int tri_ids in result_tri_ids)
            {
                // Add all three edges from the result triangle
                (int e1, int e2, int e3) tri_edge = BW_delaunay.triangles_data.get_specific_triangle_edge_ids(tri_ids);

                result_edg_ids.Add(tri_edge.e1);
                result_edg_ids.Add(tri_edge.e2);
                result_edg_ids.Add(tri_edge.e3);

                // Add all thress pts from the result triangle
                (int p1, int p2, int p3) tri_pt = BW_delaunay.triangles_data.get_specific_triangle_point_ids(tri_ids);

                result_pts_ids.Add(tri_pt.p1);
                result_pts_ids.Add(tri_pt.p2);
                result_pts_ids.Add(tri_pt.p3);
            }


            return (result_pts_ids.ToList(), result_edg_ids.ToList(), result_tri_ids.ToList());
        }

        private bool is_point_in_surface(point_d i_pt, List<point_d> sc_outter_bndry_pts, List<List<point_d>> sc_inner_bndry_pts)
        {
            // Check whether the point is on surface boundary
            // Outter boundary
            if (is_point_lies_on_boundary(i_pt, sc_outter_bndry_pts) == true)
            {
                // return true because do not remove point which lies on surface boundary
                return true;
            }

            // Inner boundary
            foreach (List<point_d> i_bndry_pts in sc_inner_bndry_pts)
            {
                if (is_point_lies_on_boundary(i_pt, i_bndry_pts) == true)
                {
                    return true;
                }
            }

            // Check whether the point is inside boundary
            if (is_point_inside_boundary(i_pt, sc_outter_bndry_pts) == true)
            {
                foreach (List<point_d> i_bndry_pts in sc_inner_bndry_pts)
                {
                    if (is_point_inside_boundary(i_pt, i_bndry_pts) == true)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool is_point_lies_on_boundary(point_d i_pt, List<point_d> sc_bndry_pts)
        {
            // Returns whether the point lies on the boundary points. ie., the given point is same as boundary points
            foreach (point_d pts in sc_bndry_pts)
            {
                if (pts.Equals(i_pt))
                {
                    return true;
                }

            }
            return false;
        }

        public bool is_point_inside_boundary(point_d i_pt, List<point_d> sc_bndry_pts)
        {
            // Get the angle between input_pt and the first & last boundary pt
            int max_pt = sc_bndry_pts.Count - 1;

            double t_angle = point_d.GetAngle(sc_bndry_pts[sc_bndry_pts.Count - 1], i_pt, sc_bndry_pts[0]);

            // Add the angle  to the inpt_pt and other boundary pts
            for (int i = 0; i < sc_bndry_pts.Count - 1; i++)
            {
                t_angle += point_d.GetAngle(sc_bndry_pts[i], i_pt, sc_bndry_pts[i + 1]);
            }

            return (Math.Abs(t_angle) > 1);

        }


    }
}
