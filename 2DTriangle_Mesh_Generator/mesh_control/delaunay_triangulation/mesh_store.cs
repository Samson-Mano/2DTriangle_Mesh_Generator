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

        public point_list_store result_pts_data { get ; private set; }
        
        public edge_list_store result_edge_data { get ; private set; }

        public triangle_list_store result_tri_data { get; private set; }

        public mesh_store(List<point_d> corner_points, List<point_d> edge_pts, List<point_d> outter_bndry_pts)
        {
            // Empty constructor
            BW_delaunay = new delaunay_triangulation();

            // spacing scale increases the points gap
            double spacing_scale = 1000.0f;

            // scale the points
            List<point_d> scaled_corner_points = new List<point_d>();
            List<point_d> scaled_edge_points = new List<point_d>();
            List<point_d> scaled_outter_bndry_pts = new List<point_d>();

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

            // Scale boundary points and add to list
            foreach (point_d pt in outter_bndry_pts)
            {
                scaled_outter_bndry_pts.Add(new point_d(pt.x * spacing_scale, pt.y * spacing_scale));
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
            bndry_rectangle[1] = new point_d(bndry_pts_x_sort[0].x, bndry_pts_y_sort[bndry_pts_y_sort.Count-1].y);
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

            // Create constrained triangle


            // Finalize mesh(by removing super triangles)
            remove_super_triangle();

            // Remove the mesh out side the boundary


            // Scale back the data and add to output
            this.result_pts_data = new point_list_store();
            this.result_edge_data = new edge_list_store();
            this.result_tri_data = new triangle_list_store();


            foreach(point_store pt in BW_delaunay.points_data.get_all_points())
            {
                point_d t_pt1 = new point_d(pt.pt_coord.x / spacing_scale, pt.pt_coord.y / spacing_scale);

                this.result_pts_data.add_point(t_pt1, pt.pt_type);
            }

            // Add to result edges data
            foreach(edge_store ed in BW_delaunay.edges_data.get_all_edges())
            {

                point_d spt = this.result_pts_data.get_point(ed.start_pt_id).pt_coord;
                point_d ept = this.result_pts_data.get_point(ed.end_pt_id).pt_coord;
                point_d mid_pt = delaunay_triangulation.get_edge_midpt(spt, ept);
                double e_len = delaunay_triangulation.get_edge_length(spt, ept);

                this.result_edge_data.add_edge(ed.start_pt_id, ed.end_pt_id, mid_pt, e_len);
            }

            // Add to result triangle data
            foreach(triangle_store tri in BW_delaunay.triangles_data.get_all_triangles())
            {
                point_d pt1 = this.result_pts_data.get_point(tri.pt1_id).pt_coord;
                point_d pt2 = this.result_pts_data.get_point(tri.pt2_id).pt_coord;
                point_d pt3 = this.result_pts_data.get_point(tri.pt3_id).pt_coord;
                point_d tri_mid = delaunay_triangulation.get_triangle_midpt(pt1, pt2, pt3);
                point_d circum_center = delaunay_triangulation.get_triangle_incircle_center(pt1, pt2, pt3);

                double e_len1 = delaunay_triangulation.get_edge_length(pt1, pt2);
                double e_len2 = delaunay_triangulation.get_edge_length(pt2, pt3);
                double e_len3 = delaunay_triangulation.get_edge_length(pt3, pt1);
                double shortest_len = delaunay_triangulation.get_minimum_of_three(e_len1, e_len2, e_len3);
                double circum_radius = delaunay_triangulation.get_edge_length(pt1, circum_center);

                this.result_tri_data.add_triangle(tri.pt1_id, tri.pt2_id, tri.pt3_id, tri.e1_id, tri.e2_id, tri.e3_id,
                   tri_mid, circum_center, shortest_len, circum_radius);
            }

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

            foreach(int tri_id in delete_triangles)
            {
                // Deleting the triangles here
                BW_delaunay.triangles_data.remove_triangle(tri_id);
            }

        }




    }
}
