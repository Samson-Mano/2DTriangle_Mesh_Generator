using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control
{
    public class constrained_delaunay_triangulation
    {
        public static double eps = 0.000001; // 10^-6

        public bool is_surface_read = false;

        public surface_store input_surface { get; private set; }

        public constrained_delaunay_triangulation(string inpt_string)
        {
            // Intiailize with input string
            // Example below
            //***OUTER BOUNDARY ID = 0
            //**ENDPOINT ID = 2, x = -1.59386390011642, y = 9.03712415066991
            //e8_0, x = 1.73946949767451, y = 9.03712415066991
            //e8_1, x = 5.07280289546544, y = 9.03712415066991
            //e8_2, x = 8.40613629325637, y = 9.03712415066991
            //e8_3, x = 11.7394696910473, y = 9.03712415066991
            //e8_4, x = 15.0728030888382, y = 9.03712415066991
            //**ENDPOINT ID = 6, x = 18.4061364866292, y = 9.03712415066991
            //e9_0, x = 18.4061364866291, y = 5.70379079262162
            //e9_1, x = 18.4061364866292, y = 2.37045743457333
            //e9_2, x = 18.4061364866292, y = -0.962875923474956
            //e9_3, x = 18.4061364866292, y = -4.29620928152324
            //e9_4, x = 18.4061364866292, y = -7.62954263957153
            //e9_5, x = 18.4061364866292, y = -10.9628759976198
            //e9_6, x = 18.4061364866292, y = -14.2962093556681
            //e9_7, x = 18.4061364866292, y = -17.6295427137164
            //**ENDPOINT ID = 5, x = 18.4061364866292, y = -20.9628760717647
            //e7_0, x = 15.0728030888382, y = -20.9628760717647
            //e7_1, x = 11.7394696910473, y = -20.9628760717647
            //e7_2, x = 8.40613629325637, y = -20.9628760717647
            //e7_3, x = 5.07280289546544, y = -20.9628760717647
            //e7_4, x = 1.73946949767451, y = -20.9628760717647
            //**ENDPOINT ID = 4, x = -1.59386390011642, y = -20.9628760717647
            //e6_0, x = -1.59386390011642, y = -17.6295427137164
            //e6_1, x = -1.59386390011642, y = -14.2962093556681
            //e6_2, x = -1.59386390011642, y = -10.9628759976198
            //e6_3, x = -1.59386390011642, y = -7.62954263957153
            //e6_4, x = -1.59386390011642, y = -4.29620928152324
            //e6_5, x = -1.59386390011642, y = -0.962875923474958
            //e6_6, x = -1.59386390011642, y = 2.37045743457333
            //e6_7, x = -1.59386390011642, y = 5.70379079262162
            //END
            //***INNER BOUNDARY ID = 1
            //**ENDPOINT ID = 8, x = 15.6497120699614, y = -5.33159186943867
            //e11_0, x = 13.9817829243054, y = -9.49065784928343
            //e11_1, x = 12.3478183760682, y = -11.5701908392058
            //e11_2, x = 10.7478184252497, y = -11.5701908392058
            //e11_3, x = 9.18178307184998, y = -9.49065784928343
            //**ENDPOINT ID = 7, x = 7.64971231586899, y = -5.33159186943867
            //e10_0, x = 8.44814918034579, y = -1.87034066512357
            //e10_1, x = 11.649712349923, y = -0.331589412039423
            //e10_2, x = 14.8512754016485, y = -1.87034091032858
            //END
            //***INNER BOUNDARY ID = 2
            //**ENDPOINT ID = 10, x = 13.8836298747204, y = 5.62309097614107
            //e13_0, x = 11.5836294460616, y = 3.52780522688642
            //**ENDPOINT ID = 9, x = 9.28363007334665, y = 5.62309097614107
            //e12_0, x = 11.5836293574176, y = 8.14780555987883
            //END


            // Seperate all the boundaries (First bndry is the outter boundary)
            List<string> bndry = new List<string>();
            int inner_bndry_count = -1; // start with -1 to avoid counting outter boundary

            // Get the outter boundary
            surface_store.boundary_store outter_bndry;

            // Get the inner boundary
            HashSet<surface_store.boundary_store> inner_bndry;

            try
            {
                using (StringReader reader = new StringReader(inpt_string))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string bndry_str = "";
                        while (line.Substring(0, 3) != "END")
                        {
                            bndry_str = bndry_str + line + Environment.NewLine;
                            line = reader.ReadLine();
                        }

                        // Add to the boundary list
                        bndry.Add(bndry_str);
                        inner_bndry_count++;
                    }
                }

                // Get the outter boundary
                outter_bndry = get_bndry(bndry[0], true);

                // Get the inner boundary
                inner_bndry = new HashSet<surface_store.boundary_store>();

                for (int i = 0; i < inner_bndry_count; i++)
                {
                    inner_bndry.Add(get_bndry(bndry[i + 1], false));
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error reading input !!!" + Environment.NewLine + ex, "Samson Mano");
                is_surface_read = false;
                return;
            }


            // Set the surface
            input_surface = new surface_store(outter_bndry, inner_bndry);
            input_surface.mesh_surface();

            is_surface_read = true;
        }

        private surface_store.boundary_store get_bndry(string bndry_str, bool is_outter)
        {
            HashSet<mesh_store.point_store> bndry_pts = new HashSet<mesh_store.point_store>();
            int bndryid;

            using (StringReader reader = new StringReader(bndry_str))
            {
                string line;

                // Read the first line
                line = reader.ReadLine();

                string bndryid_str = line.Split('=').Last();
                bndryid = int.Parse(bndryid_str);


                while ((line = reader.ReadLine()) != null)
                {


                    // split by comma ,
                    string[] pointstr = line.Split(',');
                    // get the id (split by equalto = and get the last)
                    string id_str = RemoveWhitespace(pointstr[0].Split('=').Last());
                    // get the x_coord (split by equalto = and get the last)
                    string x_str = RemoveWhitespace(pointstr[1].Split('=').Last());
                    // get the y_coord (split by equalto = and get the last)
                    string y_str = RemoveWhitespace(pointstr[2].Split('=').Last());

                    // Check wether the id is from edge
                    int pt_id;
                    double x_coord;
                    double y_coord;
                    int pt_type;

                    if (id_str.StartsWith("e") == true)
                    {
                        // remove the first character 'e'
                        id_str = id_str.Remove(0, 1);

                        // split the edge id with _
                        string[] new_id = id_str.Split('_');
                        int id_prefix = int.Parse(new_id[0]);
                        int id_suffix = int.Parse(new_id[1]);

                        pt_id = (id_prefix * 100000) + id_suffix;
                        pt_type = 2;
                    }
                    else
                    {
                        pt_id = int.Parse(id_str);
                        pt_type = 1;
                    }

                    // Parse the x & y coordinates
                    x_coord = Double.Parse(x_str);
                    y_coord = Double.Parse(y_str);

                    // Add to the boundary point list
                    mesh_store.point_store ind_bndry_pt = new mesh_store.point_store(pt_id, x_coord, y_coord, pt_type);
                    bndry_pts.Add(ind_bndry_pt);
                }
            }

            // Return the boundary
            return new surface_store.boundary_store(bndryid, bndry_pts, is_outter);
        }

        public class surface_store
        {
            public class boundary_store
            {
                public int boundary_id { get; private set; }
                public HashSet<mesh_store.point_store> bndry_pts { get; private set; }
                public bool is_outter_bndry { get; private set; }


                public boundary_store(int t_bndry_id, HashSet<mesh_store.point_store> t_bndry_pts, bool t_is_outter_bndry)
                {
                    this.boundary_id = t_bndry_id;
                    this.bndry_pts = t_bndry_pts;
                    this.is_outter_bndry = t_is_outter_bndry;
                }
            }

            public boundary_store outter_bndry { get; private set; }
            public HashSet<boundary_store> inner_bndries { get; private set; }
            public mesh_store mesh_data { get; private set; }

            public surface_store(boundary_store t_outter_bndry, HashSet<boundary_store> t_inner_bndries)
            {
                this.outter_bndry = t_outter_bndry;
                this.inner_bndries = t_inner_bndries;
            }

            public void mesh_surface()
            {
                // Mesh surface
                List<mesh_store.point_store> fixed_point = new List<mesh_store.point_store>();

                // Get the outter boundary points
                foreach (mesh_store.point_store pts in outter_bndry.bndry_pts)
                {
                    fixed_point.Add(pts);
                }

                // Get the inner boundary points
                foreach (boundary_store innr_bndry in inner_bndries)
                {
                    foreach (mesh_store.point_store pts in innr_bndry.bndry_pts)
                    {
                        fixed_point.Add(pts);
                    }
                }

                // Add to the mesh store
                mesh_data = new mesh_store();
                mesh_data.Add_multiple_points(fixed_point);

                // Setup well sized triangle
                // Parameter 1: Chew's first algorithm B=1, Ruppert's B=Sqrt(2), Chew's second algorithm B=Sqrt(5)/2
                double B_var = Math.Sqrt(2);
                // Parameter 2: h is the desired side length of triangle in the triangulation (user input)
                double H_var = mesh_data.all_triangles.OrderBy(obj => obj.shortest_edge).ToArray()[0].shortest_edge * 2.0f;

                // Find and queue bad triangles
                mesh_store.triangle_store bad_triangle = mesh_data.all_triangles.Find(obj => triangle_angle_size_contstraint(obj, B_var, H_var) == true);

                while (bad_triangle != null)
                {
                    mesh_data.Add_single_point(bad_triangle.circum_center);

                    bad_triangle = mesh_data.all_triangles.Find(obj => triangle_angle_size_contstraint(obj, B_var, H_var) == true);
                }

                // Finalize mesh (by removing super triangles)
                List<int> remove_index = new List<int>();

                // Remove all the triangles not inside the surface
                for (int i = mesh_data.all_triangles.Count - 1; i > -1; i--)
                {
                    if (is_point_in_surface(mesh_data.all_triangles[i].shrunk_vertices[0]) == false ||
                        is_point_in_surface(mesh_data.all_triangles[i].shrunk_vertices[1]) == false ||
                        is_point_in_surface(mesh_data.all_triangles[i].shrunk_vertices[2]) == false)
                    {
                        remove_index.Add(i);
                    }

                }

                foreach (int i in remove_index)
                {
                    mesh_data.all_triangles.RemoveAt(i);
                }

                // Remove all the edges not associated with triangle
                remove_index.Clear();
                for (int i = mesh_data.all_edges.Count - 1; i > -1; i--)
                {
                    if (mesh_data.all_triangles.Exists(obj => obj.contains_edge(mesh_data.all_edges[i]) == true) == false)
                    {
                        remove_index.Add(i);
                    }
                }

                foreach (int i in remove_index)
                {
                    mesh_data.all_edges.RemoveAt(i);
                }

                // Remove all the points not associated with edges
                remove_index.Clear();
                for (int i = mesh_data.all_points.Count - 1; i > -1; i--)
                {
                    if (mesh_data.all_edges.Exists(obj => obj.start_pt == mesh_data.all_points[i] || obj.end_pt == mesh_data.all_points[i]) == false)
                    {
                        remove_index.Add(i);
                    }
                }
                foreach (int i in remove_index)
                {
                    mesh_data.all_points.RemoveAt(i);
                }

            }

            private bool triangle_angle_size_contstraint(mesh_store.triangle_store bad_triangle, double Bv, double Hv)
            {
                if (is_point_in_surface(bad_triangle.shrunk_vertices[0]) == true &&
                            is_point_in_surface(bad_triangle.shrunk_vertices[1]) == true &&
                            is_point_in_surface(bad_triangle.shrunk_vertices[2]) == true)
                {
                    if (bad_triangle.circumradius_shortest_edge_ratio > Bv)
                    {
                        // condition 1: B parameter => A triangle is well-shaped if all its angles are greater than or equal to 30 degrees
                        return true;
                    }
                    else if (bad_triangle.circum_radius > Hv && bad_triangle.shortest_edge > Hv)
                    {
                        // condition 2: h parameter => A triangle is well-sized if it satisfies a user - supplied grading function
                        return true;
                    }
                    else
                    {
                        // Conditions are not met
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }


            private bool is_point_in_surface(mesh_store.point_store i_pt)
            {
                if (is_point_in_boundary(outter_bndry, i_pt) == true)
                {
                    foreach (boundary_store innr_bndry in inner_bndries)
                    {
                        if (is_point_in_boundary(innr_bndry, i_pt) == true)
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

            private bool is_point_in_boundary(boundary_store bndry, mesh_store.point_store i_pt)
            {
                // Get the angle between input_pt and the first & last boundary pt
                int max_pt = bndry.bndry_pts.Count - 1;

                double t_angle = GetAngle(bndry.bndry_pts.Last(), i_pt, bndry.bndry_pts.First());

                // Add the angle  to the inpt_pt and other boundary pts
                List<mesh_store.point_store> b_pts = bndry.bndry_pts.ToList();

                for (int i = 0; i < bndry.bndry_pts.Count - 1; i++)
                {
                    t_angle += GetAngle(b_pts[i], i_pt, b_pts[i + 1]);
                }

                return (Math.Abs(t_angle) > 1);

            }

            private double GetAngle(mesh_store.point_store A_pt,
    mesh_store.point_store B_pt, mesh_store.point_store C_pt)
            {
                // Get the dot product.
                double dot_product = DotProduct(A_pt, B_pt, C_pt);

                // Get the cross product.
                double cross_product = CrossProductLength(A_pt, B_pt, C_pt);

                // Calculate the angle.
                return Math.Atan2(cross_product, dot_product);
            }

            private double CrossProductLength(mesh_store.point_store A_pt,
    mesh_store.point_store B_pt, mesh_store.point_store C_pt)
            {
                // Get the vectors' coordinates.
                double BAx = A_pt.x - B_pt.x;
                double BAy = A_pt.y - B_pt.y;
                double BCx = C_pt.x - B_pt.x;
                double BCy = C_pt.y - B_pt.y;

                // Calculate the Z coordinate of the cross product.
                return (BAx * BCy - BAy * BCx);
            }

            // Return the dot product AB · BC.
            // Note that AB · BC = |AB| * |BC| * Cos(theta).
            private double DotProduct(mesh_store.point_store A_pt,
    mesh_store.point_store B_pt, mesh_store.point_store C_pt)
            {
                // Get the vectors' coordinates.
                double BAx = A_pt.x - B_pt.x;
                double BAy = A_pt.y - B_pt.y;
                double BCx = C_pt.x - B_pt.x;
                double BCy = C_pt.y - B_pt.y;

                // Calculate the dot product.
                return (BAx * BCx + BAy * BCy);
            }


        }

        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }




        #region " Delaunay Triangulation - Bowyer Watson Incremental Algorithm"
        public class mesh_store
        {
            // Local Variables
            private List<point_store> _all_points = new List<point_store>();
            private List<edge_store> _all_edges = new List<edge_store>();
            private List<triangle_store> _all_triangles = new List<triangle_store>();
            public bool is_meshed = false;

            private List<int> unique_edgeid_list = new List<int>();
            private List<int> unique_triangleid_list = new List<int>();

            // super triangle points
            private point_store s_p1, s_p2, s_p3;

            public List<point_store> all_points
            {
                get { return this._all_points; }
            }

            public List<triangle_store> all_triangles
            {
                get { return this._all_triangles; }
            }

            public List<edge_store> all_edges
            {
                get { return this._all_edges; }
            }

            public mesh_store()
            {
                // Empty constructor
            }

            public void Add_multiple_points(List<point_store> p_inpt_points)
            {
                // Call this first
                this._all_points = new List<point_store>(p_inpt_points);
                // Transfer the parent variable to local point_store list
                //foreach (pslg_datastructure.point2d pt in parent_inpt_points)
                //{
                //    this._all_points.Add(new point_store(pt.id, pt.x, pt.y, pt));
                //}


                // sort the points first by x and then by y
                this._all_points = this._all_points.OrderBy(obj => obj.x).ThenBy(obj => obj.y).ToList();


                // intialize the edges and triangles
                this._all_edges = new List<edge_store>();
                this._all_triangles = new List<triangle_store>();

                // Create an imaginary triangle that encloses all the point set
                set_bounding_triangle(this.all_points);

                foreach (point_store i_pt in this.all_points)
                {
                    // incemental add point
                    incremental_point_addition(i_pt);
                }
            }

            public void Add_single_point(point_store p_onemore_point)
            {
                // dont call this before calling Add_multiple_points
                point_store temp_pt = new point_store(this.all_points.Count, p_onemore_point.x, p_onemore_point.y, 3);
                // Add the point to local list
                this._all_points.Add(temp_pt);

                // call the incremental add point
                incremental_point_addition(this.all_points[this.all_points.Count - 1]);
            }

            private void incremental_point_addition(point_store pt)
            {
                // Find the index of triangle containing this point
                int containing_triangle_index = this.all_triangles.FindIndex(obj => obj.is_point_inside(pt) == true);

                if (containing_triangle_index != -1)
                {
                    // collect the edges of the triangle
                    edge_store tri_edge_a = this.all_triangles[containing_triangle_index].e1;
                    edge_store tri_edge_b = this.all_triangles[containing_triangle_index].e2;
                    edge_store tri_edge_c = this.all_triangles[containing_triangle_index].e3;

                    // remove the single triangle
                    remove_triangle(containing_triangle_index);

                    // add the three triangles
                    int[] triangle_id = new int[3];
                    triangle_id = add_three_triangles(pt, tri_edge_a, tri_edge_b, tri_edge_c);

                    // Flip the bad triangles recursively
                    flip_bad_edges(triangle_id[0], pt);
                    flip_bad_edges(triangle_id[1], pt);
                    flip_bad_edges(triangle_id[2], pt);
                }
                else
                {
                    // Point lies on the edge
                    // Find the edge which is closest to the pt
                    int the_edge_id = this.all_edges.Find(obj => obj.test_point_on_line(pt)).edge_id;

                    int first_tri_index = this.all_edges[the_edge_id].left_triangle.tri_id;
                    int second_tri_index = this.all_edges[the_edge_id].right_triangle.tri_id;

                    // collect the other two edges of first triangle
                    edge_store[] first_tri_other_two_edge = new edge_store[2];
                    first_tri_other_two_edge = this.all_triangles[first_tri_index].get_other_two_edges(this.all_edges[the_edge_id]);

                    // collect the other two edges of second triangle
                    edge_store[] second_tri_other_two_edge = new edge_store[2];
                    second_tri_other_two_edge = this.all_triangles[second_tri_index].get_other_two_edges(this.all_edges[the_edge_id]);

                    // Remove the common edge
                    unique_edgeid_list.Add(this.all_edges[the_edge_id].edge_id);
                    // this._all_edges.RemoveAt(the_edge_index);
                    this._all_edges[the_edge_id].remove_edge();

                    // remove the two triangle
                    remove_triangle(first_tri_index);
                    remove_triangle(second_tri_index);

                    // add the three triangles
                    int[] triangle_id = new int[4];
                    triangle_id = add_four_triangles(pt, first_tri_other_two_edge[0], first_tri_other_two_edge[1], second_tri_other_two_edge[0], second_tri_other_two_edge[1]);

                    // Flip the bad triangles recursively
                    flip_bad_edges(triangle_id[0], pt);
                    flip_bad_edges(triangle_id[1], pt);
                    flip_bad_edges(triangle_id[2], pt);
                    flip_bad_edges(triangle_id[3], pt);
                }
            }

            //public void Finalize_mesh(pslg_datastructure.surface_store the_surface)
            //{
            //    // Call this after calling Add_multiple_points & Add_single_point (if required)
            //    // Finalize mesh is the final step to transfer the mesh from local data to global
            //    local_output_edges = new List<pslg_datastructure.edge2d>();
            //    local_output_triangle = new List<pslg_datastructure.triangle2d>();


            //    int i = 0;
            //    foreach (triangle_store the_tri in this.all_triangles)
            //    {
            //        // Check if the triangles lies inside the surface


            //        if (the_surface.pointinsurface(the_tri.shrunk_vertices[0].x, the_tri.shrunk_vertices[0].y) == false ||
            //            the_surface.pointinsurface(the_tri.shrunk_vertices[1].x, the_tri.shrunk_vertices[1].y) == false ||
            //            the_surface.pointinsurface(the_tri.shrunk_vertices[2].x, the_tri.shrunk_vertices[2].y) == false)
            //        {
            //            // continue because the face is not in surface
            //            continue;
            //        }

            //        pslg_datastructure.triangle2d temp_tri = new pslg_datastructure.triangle2d(i, the_tri.pt1.get_parent_data_type, the_tri.pt2.get_parent_data_type, the_tri.pt3.get_parent_data_type);
            //        local_output_triangle.Add(temp_tri);
            //        i++;
            //    }

            //    i = 0;
            //    foreach (edge_store the_edge in this.all_edges)
            //    {
            //        // Check if the edges lies inside the surface
            //        pslg_datastructure.edge2d temp_edge = new pslg_datastructure.edge2d(i, the_edge.start_pt.get_parent_data_type, the_edge.end_pt.get_parent_data_type);

            //        if (local_output_triangle.Exists(obj => obj.edge_exists(temp_edge) == true)) //so check whether this edge is associated with a face
            //        {
            //            local_output_edges.Add(temp_edge);
            //            i++;
            //        }
            //    }

            //    // set the mesh complete
            //    is_meshed = true;
            //}

            private void remove_triangle(int tri_index)
            {
                int edge_index1 = this.all_triangles[tri_index].e1.edge_id;
                int edge_index2 = this.all_triangles[tri_index].e2.edge_id;
                int edge_index3 = this.all_triangles[tri_index].e3.edge_id;

                // Edge 1
                if (edge_index1 != -1)
                {
                    this._all_edges[edge_index1].remove_triangle(this.all_triangles[tri_index]);
                }
                // Edge 2
                if (edge_index2 != -1)
                {
                    this._all_edges[edge_index2].remove_triangle(this.all_triangles[tri_index]);
                }
                // Edge 3
                if (edge_index3 != -1)
                {
                    this._all_edges[edge_index3].remove_triangle(this.all_triangles[tri_index]);
                }

                unique_triangleid_list.Add(this.all_triangles[tri_index].tri_id);
                this._all_triangles[tri_index].remove_triangle();
            }

            public int[] add_three_triangles(point_store new_pt, edge_store edge_a, edge_store edge_b, edge_store edge_c)
            {
                // Add three new edges from the new point
                int[] edge_indices = new int[3];
                // Add Edge 1
                edge_indices[0] = add_edge(new_pt, edge_a.start_pt);

                // Add Edge 2
                edge_indices[1] = add_edge(new_pt, edge_b.start_pt);

                // Add Edge 3
                edge_indices[2] = add_edge(new_pt, edge_c.start_pt);
                //_________________________________________________________________________________

                // Add three triangles
                int[] output_indices = new int[3];
                // Add First triangle
                output_indices[0] = add_triangle(new_pt, edge_a.start_pt, edge_a.end_pt, this.all_edges[edge_indices[0]], edge_a, this.all_edges[edge_indices[1]].sym);

                // Add Second triangle
                output_indices[1] = add_triangle(new_pt, edge_b.start_pt, edge_b.end_pt, this.all_edges[edge_indices[1]], edge_b, this.all_edges[edge_indices[2]].sym);

                // Add Third triangle
                output_indices[2] = add_triangle(new_pt, edge_c.start_pt, edge_c.end_pt, this.all_edges[edge_indices[2]], edge_c, this.all_edges[edge_indices[0]].sym);
                //_________________________________________________________________________________

                // Add the triangle details to the edge
                // Edge 1
                this._all_edges[edge_indices[0]].add_triangle(this.all_triangles[output_indices[0]]);
                this._all_edges[edge_indices[0]].add_triangle(this.all_triangles[output_indices[2]]);
                // Edge 2
                this._all_edges[edge_indices[1]].add_triangle(this.all_triangles[output_indices[0]]);
                this._all_edges[edge_indices[1]].add_triangle(this.all_triangles[output_indices[1]]);
                // Edge 3
                this._all_edges[edge_indices[2]].add_triangle(this.all_triangles[output_indices[1]]);
                this._all_edges[edge_indices[2]].add_triangle(this.all_triangles[output_indices[2]]);

                // Edge a
                this._all_edges[edge_a.edge_id].add_triangle(this.all_triangles[output_indices[0]]);
                // Edge b
                this._all_edges[edge_b.edge_id].add_triangle(this.all_triangles[output_indices[1]]);
                // Edge c
                this._all_edges[edge_c.edge_id].add_triangle(this.all_triangles[output_indices[2]]);

                //_________________________________________________________________________________

                return output_indices;
            }

            public int[] add_four_triangles(point_store new_pt, edge_store edge_a, edge_store edge_b, edge_store edge_c, edge_store edge_d)
            {
                // Add four new edges from the new point
                int[] edge_indices = new int[4];
                // Add Edge 1
                edge_indices[0] = add_edge(new_pt, edge_a.start_pt);

                // Add Edge 2
                edge_indices[1] = add_edge(new_pt, edge_b.start_pt);

                // Add Edge 3
                edge_indices[2] = add_edge(new_pt, edge_c.start_pt);

                // Add Edge 4
                edge_indices[3] = add_edge(new_pt, edge_d.start_pt);
                //_________________________________________________________________________________

                // Add four triangles
                int[] output_indices = new int[4];
                // Add First triangle
                output_indices[0] = add_triangle(new_pt, edge_a.start_pt, edge_a.end_pt, this.all_edges[edge_indices[0]], edge_a, this.all_edges[edge_indices[1]].sym);

                // Add Second triangle
                output_indices[1] = add_triangle(new_pt, edge_b.start_pt, edge_b.end_pt, this.all_edges[edge_indices[1]], edge_b, this.all_edges[edge_indices[2]].sym);

                // Add Third triangle
                output_indices[2] = add_triangle(new_pt, edge_c.start_pt, edge_c.end_pt, this.all_edges[edge_indices[2]], edge_c, this.all_edges[edge_indices[3]].sym);

                // Add Fourth triangle
                output_indices[3] = add_triangle(new_pt, edge_d.start_pt, edge_d.end_pt, this.all_edges[edge_indices[3]], edge_d, this.all_edges[edge_indices[0]].sym);
                //_________________________________________________________________________________

                // Add the triangle details to the edge
                // Edge 1
                this._all_edges[edge_indices[0]].add_triangle(this.all_triangles[output_indices[0]]);
                this._all_edges[edge_indices[0]].add_triangle(this.all_triangles[output_indices[3]]);
                // Edge 2
                this._all_edges[edge_indices[1]].add_triangle(this.all_triangles[output_indices[0]]);
                this._all_edges[edge_indices[1]].add_triangle(this.all_triangles[output_indices[1]]);
                // Edge 3
                this._all_edges[edge_indices[2]].add_triangle(this.all_triangles[output_indices[1]]);
                this._all_edges[edge_indices[2]].add_triangle(this.all_triangles[output_indices[2]]);
                // Edge 4
                this._all_edges[edge_indices[3]].add_triangle(this.all_triangles[output_indices[2]]);
                this._all_edges[edge_indices[3]].add_triangle(this.all_triangles[output_indices[3]]);

                // Edge a
                this._all_edges[edge_a.edge_id].add_triangle(this.all_triangles[output_indices[0]]);
                // Edge b
                this._all_edges[edge_b.edge_id].add_triangle(this.all_triangles[output_indices[1]]);
                // Edge c
                this._all_edges[edge_c.edge_id].add_triangle(this.all_triangles[output_indices[2]]);
                // Edge d
                this._all_edges[edge_d.edge_id].add_triangle(this.all_triangles[output_indices[3]]);
                //_________________________________________________________________________________

                return output_indices;
            }

            public int[] add_two_triangles(point_store new_pt, edge_store tri_a_edge_0, edge_store tri_a_edge_1, edge_store tri_b_edge_0, edge_store tri_b_edge_1)
            {
                // add the only new common edge
                int edge_index;
                // Add Edge 
                edge_index = add_edge(tri_a_edge_1.start_pt, tri_b_edge_1.start_pt);
                //_________________________________________________________________________________

                // Add two triangles
                int[] output_indices = new int[2];
                // Add First triangle
                output_indices[0] = add_triangle(tri_a_edge_1.start_pt, tri_b_edge_0.start_pt, this.all_edges[edge_index].sym.start_pt, tri_a_edge_1, tri_b_edge_0, this.all_edges[edge_index].sym);

                // Add Second triangle
                output_indices[1] = add_triangle(tri_b_edge_1.start_pt, tri_a_edge_0.start_pt, this.all_edges[edge_index].start_pt, tri_b_edge_1, tri_a_edge_0, this.all_edges[edge_index]);
                //_________________________________________________________________________________

                // Add the triangle details to the edge
                // Common Edge
                this._all_edges[edge_index].add_triangle(this.all_triangles[output_indices[0]]);
                this._all_edges[edge_index].add_triangle(this.all_triangles[output_indices[1]]);

                // Edge a
                this._all_edges[tri_a_edge_1.edge_id].add_triangle(this.all_triangles[output_indices[0]]);
                this._all_edges[tri_b_edge_0.edge_id].add_triangle(this.all_triangles[output_indices[0]]);
                // Edge b
                this._all_edges[tri_b_edge_1.edge_id].add_triangle(this.all_triangles[output_indices[1]]);
                this._all_edges[tri_a_edge_0.edge_id].add_triangle(this.all_triangles[output_indices[1]]);
                //_________________________________________________________________________________

                return output_indices;
            }

            private int add_edge(point_store pt1, point_store pt2)
            {
                // Add Edge
                int edge_index = get_unique_edge_id();
                if (edge_index > this.all_edges.Count - 1)
                {
                    // new edge is added
                    edge_store temp_edge = new edge_store();
                    temp_edge.add_edge(edge_index, pt1, pt2);
                    this._all_edges.Add(temp_edge);
                }
                else
                {
                    // existing edge is revised (previously deleted edge is now filled)
                    this._all_edges[edge_index].add_edge(edge_index, pt1, pt2);
                }
                return edge_index;
            }

            private int add_triangle(point_store pt1, point_store pt2, point_store pt3, edge_store e1, edge_store e2, edge_store e3)
            {
                // Add Triangle
                int tri_index = get_unique_triangle_id();
                if (tri_index > this.all_triangles.Count - 1)
                {
                    // new triangle is added
                    triangle_store temp_tri = new triangle_store();
                    temp_tri.add_triangle(tri_index, pt1, pt2, pt3, e1, e2, e3);
                    this._all_triangles.Add(temp_tri);
                }
                else
                {
                    // existing triangle is revised (previously deleted triangle is now filled)
                    this._all_triangles[tri_index].add_triangle(tri_index, pt1, pt2, pt3, e1, e2, e3);
                }
                return tri_index;
            }

            private void flip_bad_edges(int tri_index, point_store pt)
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
                // find the edge of this triangle whihc does not contain pt
                int common_edge_index = this.all_triangles[tri_index].get_other_edge_id(pt);

                // Find the neighbour tri index
                int neighbour_tri_index = this.all_edges[common_edge_index].other_triangle_id(this.all_triangles[tri_index]);

                // legalize only if the triangle has a neighbour
                if (neighbour_tri_index != -1)
                {
                    // check whether the newly added pt is inside the neighbour triangle
                    if (this.all_triangles[neighbour_tri_index].is_point_in_circumcircle(pt) == true)
                    {
                        // collect the other two edges of the triangle
                        edge_store[] tri_other_two_edges = new edge_store[2];
                        tri_other_two_edges = this.all_triangles[tri_index].get_other_two_edges(this.all_edges[common_edge_index]);

                        // collect the other two edges of the neighbour triangle
                        edge_store[] neighbour_tri_other_two_edges = new edge_store[2];
                        neighbour_tri_other_two_edges = this.all_triangles[neighbour_tri_index].get_other_two_edges(this.all_edges[common_edge_index]);

                        // Remove the common edge
                        unique_edgeid_list.Add(this.all_edges[common_edge_index].edge_id);
                        // this._all_edges.RemoveAt(common_edge_index);
                        this._all_edges[common_edge_index].remove_edge();

                        // Remove the two triangles
                        remove_triangle(tri_index);
                        remove_triangle(neighbour_tri_index);

                        // Add new two triangles
                        int[] triangle_id = new int[2];
                        triangle_id = add_two_triangles(pt, tri_other_two_edges[0], tri_other_two_edges[1], neighbour_tri_other_two_edges[0], neighbour_tri_other_two_edges[1]);

                        // recursion below
                        flip_bad_edges(triangle_id[0], pt);
                        flip_bad_edges(triangle_id[1], pt);
                    }
                }
            }

            private int get_unique_edge_id()
            {
                int edge_id;
                // get an unique edge id
                if (unique_edgeid_list.Count != 0)
                {
                    edge_id = unique_edgeid_list[0]; // retrive the edge id from the list which stores the id of deleted edges
                    unique_edgeid_list.RemoveAt(0); // remove that id from the unique edge id list
                }
                else
                {
                    edge_id = this.all_edges.Count;
                }
                return edge_id;

            }

            private int get_unique_triangle_id()
            {
                int tri_id;
                // get an unique triangle id
                if (unique_triangleid_list.Count != 0)
                {
                    tri_id = unique_triangleid_list[0]; // retrive the triangle id from the list which stores the id of deleted edges
                    unique_triangleid_list.RemoveAt(0); // remove that id from the unique triangle id list
                }
                else
                {
                    tri_id = this.all_triangles.Count;
                }
                return tri_id;
            }

            private void set_bounding_triangle(List<point_store> all_input_vertices)
            {
                point_store[] x_sorted = all_input_vertices.OrderBy(obj => obj.x).ToArray();
                point_store[] y_sorted = all_input_vertices.OrderBy(obj => obj.y).ToArray();

                // Define bounding triangle
                double max_x, max_y, k;
                max_x = (x_sorted[x_sorted.Length - 1].x - x_sorted[0].x);
                max_y = (y_sorted[y_sorted.Length - 1].y - y_sorted[0].y);
                k = 1000 * Math.Max(max_x, max_y);

                // zeoth _point
                double x_zero, y_zero;
                x_zero = (x_sorted[x_sorted.Length - 1].x + x_sorted[0].x) * 0.5;
                y_zero = (y_sorted[y_sorted.Length - 1].y + y_sorted[0].y) * 0.5;

                // id for the super triangle points
                int pt_count = all_input_vertices.Count;

                // set the vertex
                this.s_p1 = new point_store(pt_count + 1, 0, Math.Round(k / 2.0f), 3);
                this.s_p2 = new point_store(pt_count + 2, Math.Round(k / 2.0f), 0.0, 3);
                this.s_p3 = new point_store(pt_count + 3, -1 * Math.Round(k / 2.0f), -1 * Math.Round(k / 2.0f), 3);

                // Add three new edges for the triangle from the three new point
                int[] edge_indices = new int[3];
                // Add Edge 1
                edge_indices[0] = add_edge(this.s_p1, this.s_p2);

                // Add Edge 2
                edge_indices[1] = add_edge(this.s_p2, this.s_p3);

                // Add Edge 3
                edge_indices[2] = add_edge(this.s_p3, this.s_p1);
                //_________________________________________________________________________________

                // Create the super triangle
                int sup_tri_index;
                sup_tri_index = add_triangle(this.s_p1, this.s_p2, this.s_p3, this.all_edges[edge_indices[0]], this.all_edges[edge_indices[1]], this.all_edges[edge_indices[2]]);

                // Add the triangle details to the outter edges
                // Edge 1
                this.all_edges[edge_indices[0]].add_triangle(this.all_triangles[sup_tri_index]);
                // Edge 2
                this.all_edges[edge_indices[1]].add_triangle(this.all_triangles[sup_tri_index]);
                // Edge 3
                this.all_edges[edge_indices[2]].add_triangle(this.all_triangles[sup_tri_index]);

                // Super triangle creation complete
                //_________________________________________________________________________________
            }

            public class point_store
            {
                public int pt_id { get; private set; }
                public double x { get; private set; }

                public double y { get; private set; }

                public int pt_type { get; private set; }

                public point_store(int i_pt_id, double i_x, double i_y, int i_pt_type)
                {
                    // Constructor
                    // set id
                    this.pt_id = i_pt_id;
                    // co-ordinate
                    this.x = i_x;
                    this.y = i_y;
                    // point type 1 - End point (or vertex), 2 - Lies on edge, 3 - Lies inside surface
                    this.pt_type = i_pt_type;
                }

                public bool Equals(point_store other)
                {
                    if (Math.Abs(this.x - other.x) <= eps && Math.Abs(this.y - other.y) <= eps)
                    {
                        return true;
                    }
                    return false;
                }

                // Operators
                public point_store sub(point_store other_pt)
                {
                    double ab_x = this.x - other_pt.x;
                    double ab_y = this.y - other_pt.y;

                    return new point_store(-1, ab_x, ab_y, other_pt.pt_type);
                }

                public point_store add(point_store other_pt)
                {
                    double ab_x = this.x + other_pt.x;
                    double ab_y = this.y + other_pt.y;

                    return new point_store(-1, ab_x, ab_y, other_pt.pt_type);
                }

                public double dot(point_store other_pt)
                {
                    return ((this.x * other_pt.x) + (this.y * other_pt.y));
                }

                public double cross(point_store other_pt)
                {
                    return ((this.y * other_pt.x) - (this.x * other_pt.y));
                }

                public point_store mult(double v)
                {
                    return (new point_store(this.pt_id, this.x * v, this.y * v, 3));
                }

            }

            public class edge_store
            {
                public int edge_id { get; private set; }

                public point_store start_pt { get; private set; }

                public point_store end_pt { get; private set; }

                public triangle_store left_triangle { get; private set; }

                public triangle_store right_triangle { get; private set; }

                public edge_store sym { get; private set; }

                public bool is_fixed_edge { get; private set; }

                public double edge_length
                {
                    get { return Math.Sqrt(Math.Pow(this.start_pt.x - this.end_pt.x, 2) + Math.Pow(this.start_pt.y - this.end_pt.y, 2)); }
                }

                public edge_store()
                {
                    // Empty Constructor
                }

                public void add_edge(int i_edge_id, point_store i_start_pt, point_store i_end_pt)
                {
                    // set id
                    this.edge_id = i_edge_id;
                    // set start and end pt
                    this.start_pt = i_start_pt;
                    this.end_pt = i_end_pt;

                    // set triangles to null
                    this.left_triangle = null;
                    this.right_triangle = null;
                    //______________________________________________________________

                    // Set the symmetrical edge
                    // set id
                    this.sym = new edge_store();
                    this.sym.edge_id = i_edge_id;
                    // set end and start pt
                    this.sym.start_pt = i_end_pt;
                    this.sym.end_pt = i_start_pt;

                    // set triangles to null
                    this.sym.left_triangle = null;
                    this.sym.right_triangle = null;
                    this.sym.sym = this;

                    // Check whether the edge is fixed edge (constrained delaunay triangle)
                    this.is_fixed_edge = false;
                    if ((this.start_pt.pt_type == 0 || this.start_pt.pt_type == 1) &&
                        (this.end_pt.pt_type == 0 || this.end_pt.pt_type == 1))
                    {
                        this.is_fixed_edge = true;
                    }

                }

                public void remove_edge()
                {
                    // set id
                    this.edge_id = -1;
                    // remove start and end pt
                    this.start_pt = null;
                    this.end_pt = null;

                    // remove triangles to null
                    this.left_triangle = null;
                    this.right_triangle = null;
                    //______________________________________________________________

                    // Remove the symmetrical edge
                    // set id
                    this.sym.edge_id = -1;
                    // remove end and start pt
                    this.sym.start_pt = null;
                    this.sym.end_pt = null;

                    // remove triangles to null
                    this.sym.left_triangle = null;
                    this.sym.right_triangle = null;
                }

                public int other_triangle_id(triangle_store the_triangle)
                {
                    if (the_triangle.tri_id != -1)
                    {
                        // Function to return the other triangle associated with this edge
                        if (this.left_triangle != null)
                        {
                            if (the_triangle.Equals(this.left_triangle) == true)
                            {
                                if (this.right_triangle == null)
                                {
                                    return -1;
                                }
                                return this.right_triangle.tri_id;
                            }
                        }

                        if (this.right_triangle != null)
                        {
                            if (the_triangle.Equals(this.right_triangle) == true)
                            {
                                if (this.left_triangle == null)
                                {
                                    return -1;
                                }
                                return this.left_triangle.tri_id;
                            }
                        }
                    }
                    return -1;
                }

                public void add_triangle(triangle_store the_triangle)
                {
                    // check whether the input triangle has this edge
                    //if (the_triangle.contains_edge(this) == true)
                    //{
                    if (rightof(the_triangle.mid_pt, this) == true)
                    {
                        // Add the right triangle
                        this.right_triangle = the_triangle;
                        this.sym.left_triangle = the_triangle;
                    }
                    else
                    {
                        // Add the left triangle
                        this.left_triangle = the_triangle;
                        this.sym.right_triangle = the_triangle;
                    }
                    //}
                }

                public void remove_triangle(triangle_store the_triangle)
                {
                    // check whether the input triangle has this edge
                    //if (the_triangle.contains_edge(this) == true)
                    //{
                    if (rightof(the_triangle.mid_pt, this) == true)
                    {
                        // Remove the right triangle
                        this.right_triangle = null;
                        this.sym.left_triangle = null;
                    }
                    else
                    {
                        // Remove the left triangle
                        this.left_triangle = null;
                        this.sym.right_triangle = null;
                    }
                    //}
                }

                private bool ccw(point_store a, point_store b, point_store c)
                {
                    // Computes | a.x a.y  1 |
                    //          | b.x b.y  1 | > 0
                    //          | c.x c.y  1 |
                    return (((b.x - a.x) * (c.y - a.y)) - ((b.y - a.y) * (c.x - a.x))) > 0;
                }

                private bool rightof(point_store x, edge_store e)
                {
                    return ccw(x, e.end_pt, e.start_pt);
                }

                public bool Equals(edge_store other)
                {
                    if (other.start_pt.Equals(this.start_pt) == true && other.end_pt.Equals(this.end_pt) == true)
                    {
                        return true;
                    }
                    return false;
                }

                public bool Equals_without_orientation(edge_store other)
                {
                    if (this.edge_id != -1)
                    {
                        if ((other.Equals(this) == true) || (other.Equals(this.sym) == true))
                        {
                            return true;
                        }
                    }
                    return false;
                }


                public bool test_point_on_line(point_store pt)
                {
                    bool rslt = false;
                    // Step: 1 Find the cross product
                    double dxc, dyc; // Vector 1 Between given point and first point of the line
                    dxc = pt.x - start_pt.x;
                    dyc = pt.y - start_pt.y;

                    double Threshold = edge_length * 0.01;

                    double dx1, dy1; // Vector 2 Between the second and first point of the line
                    dx1 = end_pt.x - start_pt.x;
                    dy1 = end_pt.y - start_pt.y;

                    double crossprd;
                    crossprd = (dxc * dy1) - (dyc * dx1); // Vector cross product

                    if (Math.Abs(crossprd) <= Threshold) // Check whether the cross product is within the threshold (other wise Not on the line)
                    {
                        if (Math.Abs(dx1) >= Math.Abs(dy1)) // The line is more horizontal or <= 45 degrees
                        {
                            rslt = (dx1 > 0 ? (start_pt.x < pt.x && pt.x < end_pt.x ? true : false) :
                                              (end_pt.x < pt.x && pt.x < start_pt.x ? true : false));
                        }
                        else // line is more vertical
                        {
                            rslt = (dy1 > 0 ? (start_pt.y < pt.y && pt.y < end_pt.y ? true : false) :
                                                (end_pt.y < pt.y && pt.y < start_pt.y ? true : false));
                        }
                    }
                    return rslt;
                }

                public point_store find_common_pt(edge_store other_edge)
                {
                    // Returns the common point of this edge and the other edges
                    if (this.start_pt.Equals(other_edge.start_pt))
                    {
                        return this.start_pt;
                    }
                    else if (this.start_pt.Equals(other_edge.end_pt))
                    {
                        return this.start_pt;
                    }
                    else if (this.end_pt.Equals(other_edge.start_pt))
                    {
                        return this.end_pt;
                    }
                    else if (this.end_pt.Equals(other_edge.end_pt))
                    {
                        return this.end_pt;
                    }
                    return null;
                }

                public point_store find_other_pt(point_store pt)
                {
                    if (this.start_pt.Equals(pt) == true)
                    {
                        return this.end_pt;
                    }
                    else if (this.end_pt.Equals(pt) == true)
                    {
                        return this.start_pt;
                    }

                    return null;
                }


            }

            public class triangle_store
            {
                public int tri_id { get; private set; }
                public point_store pt1 { get; private set; }
                public point_store pt2 { get; private set; }
                public point_store pt3 { get; private set; }
                public point_store mid_pt { get; private set; }

                public edge_store e1 { get; private set; } // from pt1 to pt2

                public edge_store e2 { get; private set; } // from pt2 to pt3

                public edge_store e3 { get; private set; } // from pt3 to pt1

                public point_store circum_center { get; private set; }

                double _circum_circle_radius;
                double _shortest_edge;


                public point_store[] shrunk_vertices { get; } = new point_store[3];

                public double circumradius_shortest_edge_ratio
                {
                    get { return (this._circum_circle_radius / this._shortest_edge); }
                }

                public double circum_radius
                {
                    get { return this._circum_circle_radius; }
                }

                public double shortest_edge
                {
                    get { return this._shortest_edge; }
                }

                public triangle_store()
                {
                    // Empty Constructor
                }

                public void add_triangle(int i_tri_id, point_store i_p1, point_store i_p2, point_store i_p3, edge_store i_e1, edge_store i_e2, edge_store i_e3)
                {
                    // set id
                    this.tri_id = i_tri_id;
                    // set points
                    this.pt1 = i_p1;
                    this.pt2 = i_p2;
                    this.pt3 = i_p3;
                    // set edges
                    this.e1 = i_e1;
                    this.e2 = i_e2;
                    this.e3 = i_e3;
                    // set the mid point
                    this.mid_pt = new point_store(-1, (this.pt1.x + this.pt2.x + this.pt3.x) / 3.0f, (this.pt1.y + this.pt2.y + this.pt3.y) / 3.0f, 3);

                    set_incircle();
                    set_shortest_edge();
                    set_shrunk_vertices();
                }

                public void remove_triangle()
                {
                    // set id
                    this.tri_id = -1;
                    // set points
                    this.pt1 = null;
                    this.pt2 = null;
                    this.pt3 = null;
                    // set edges
                    this.e1 = null;
                    this.e2 = null;
                    this.e3 = null;
                    // set the mid point
                    this.mid_pt = null;

                    // in center
                    this.circum_center = null;
                    this._circum_circle_radius = 0.0f;

                    // shortest edge
                    this._shortest_edge = 0.0f;

                    //remove shrunk vertices
                    this.shrunk_vertices[0] = null;
                    this.shrunk_vertices[1] = null;
                    this.shrunk_vertices[2] = null;
                }

                private void set_shrunk_vertices()
                {
                    double shrink_factor = 0.98;
                    point_store pt1 = new point_store(-1, this.mid_pt.x * (1 - shrink_factor) + (this.pt1.x * shrink_factor), this.mid_pt.y * (1 - shrink_factor) + (this.pt1.y * shrink_factor), 3);
                    point_store pt2 = new point_store(-1, this.mid_pt.x * (1 - shrink_factor) + (this.pt2.x * shrink_factor), this.mid_pt.y * (1 - shrink_factor) + (this.pt2.y * shrink_factor), 3);
                    point_store pt3 = new point_store(-1, this.mid_pt.x * (1 - shrink_factor) + (this.pt3.x * shrink_factor), this.mid_pt.y * (1 - shrink_factor) + (this.pt3.y * shrink_factor), 3);

                    this.shrunk_vertices[0] = pt1;
                    this.shrunk_vertices[1] = pt2;
                    this.shrunk_vertices[2] = pt3;
                }

                private void set_incircle()
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

                    this.circum_center = new point_store(-1, center_x, center_y, 3);
                    this._circum_circle_radius = Math.Sqrt((center_x - pt1.x) * (center_x - pt1.x) + (center_y - pt1.y) * (center_y - pt1.y));
                    //this._ellipse_edge = new point2d(-1, center_x - this._circle_radius, center_y - this._circle_radius);
                    // this._ellipse_edge = new point2d(-1, center_x - 2, center_y - 2);
                }

                private void set_shortest_edge()
                {
                    double edge_len_1 = e1.edge_length;
                    double edge_len_2 = e2.edge_length;
                    double edge_len_3 = e3.edge_length;

                    this._shortest_edge = edge_len_1 < edge_len_2 ? (edge_len_1 < edge_len_3 ? edge_len_1 : edge_len_3) : (edge_len_2 < edge_len_3 ? edge_len_2 : edge_len_3);
                    //x < y ? (x < z ? x : z) : (y < z ? y : z)
                }

                public bool contains_point(point_store the_point)
                {
                    // find whether the point belongs to the triangle
                    if (the_point.Equals(this.pt1) == true ||
                        the_point.Equals(this.pt2) == true ||
                        the_point.Equals(this.pt3) == true)
                    {
                        return true;
                    }
                    return false;
                }

                public point_store get_other_pt(edge_store the_edge)
                {
                    // Returns the third vertex of this triangle which is not part of the given edge
                    if (the_edge.Equals_without_orientation(this.e1) == true)
                    {
                        return (this.e2.find_common_pt(this.e3));
                    }
                    else if (the_edge.Equals_without_orientation(this.e2) == true)
                    {
                        return (this.e3.find_common_pt(this.e1));
                    }
                    else if (the_edge.Equals_without_orientation(this.e3) == true)
                    {
                        return (this.e1.find_common_pt(this.e2));
                    }
                    return null;
                }

                public edge_store[] get_other_two_edges(edge_store the_edge)
                {
                    edge_store[] other_two_edge = new edge_store[2];

                    if (this.e1.Equals_without_orientation(the_edge) == true)
                    {
                        other_two_edge[0] = this.e2;
                        other_two_edge[1] = this.e3;
                        return other_two_edge;
                    }
                    else if (this.e2.Equals_without_orientation(the_edge) == true)
                    {
                        other_two_edge[0] = this.e3;
                        other_two_edge[1] = this.e1;
                        return other_two_edge;
                    }
                    else if (this.e3.Equals_without_orientation(the_edge) == true)
                    {
                        other_two_edge[0] = this.e1;
                        other_two_edge[1] = this.e2;
                        return other_two_edge;
                    }
                    return null;
                }

                public int get_other_edge_id(point_store pt)
                {
                    if (this.e1.start_pt.Equals(pt) == false && this.e1.end_pt.Equals(pt) == false)
                    {
                        return this.e1.edge_id;
                    }
                    else if (this.e2.start_pt.Equals(pt) == false && this.e2.end_pt.Equals(pt) == false)
                    {
                        return this.e2.edge_id;
                    }
                    else if (this.e3.start_pt.Equals(pt) == false && this.e3.end_pt.Equals(pt) == false)
                    {
                        return this.e3.edge_id;
                    }

                    return -1;
                }

                public bool contains_edge(edge_store the_edge)
                {
                    // find whether the edge belongs to the triangle
                    if (the_edge.Equals_without_orientation(this.e1) == true ||
                        the_edge.Equals_without_orientation(this.e2) == true ||
                        the_edge.Equals_without_orientation(this.e3) == true)
                    {
                        return true;
                    }
                    return false;
                }

                // Tests if a 2D point lies inside this 2D triangle.See Real-Time Collision
                // Detection, chap. 5, p. 206.
                //
                // @param point
                // The point to be tested
                // @return Returns true iff the point lies inside this 2D triangle
                public bool is_point_inside(point_store the_pt)
                {
                    double pab = the_pt.sub(this.pt1).cross(this.pt2.sub(this.pt1));
                    double pbc = the_pt.sub(this.pt2).cross(this.pt3.sub(this.pt2));

                    if (has_same_sign(pab, pbc) == false)
                    {
                        return false;
                    }

                    double pca = the_pt.sub(this.pt3).cross(this.pt1.sub(this.pt3));

                    if (has_same_sign(pab, pca) == false)
                    {
                        return false;
                    }


                    // Point test
                    if (e1.test_point_on_line(the_pt) == true)
                    {
                        return false;
                    }
                    else if (e2.test_point_on_line(the_pt) == true)
                    {
                        return false;
                    }
                    else if (e3.test_point_on_line(the_pt) == true)
                    {
                        return false;
                    }

                    return true;
                }

                private bool has_same_sign(double a, double b)
                {
                    return Math.Sign(a) == Math.Sign(b);
                }


                //// Tests if a given point lies in the circumcircle of this triangle.Let the
                //// triangle ABC appear in counterclockwise (CCW) order. Then when det &gt;
                //// 0, the point lies inside the circumcircle through the three points a, b
                //// and c.If instead det &lt; 0, the point lies outside the circumcircle.
                //// When det = 0, the four points are cocircular.If the triangle is oriented
                //// clockwise (CW) the result is reversed.See Real-Time Collision Detection,
                //// chap. 3, p. 34.
                public bool is_point_in_circumcircle(point_store the_pt)
                {
                    double a11 = pt1.x - the_pt.x;
                    double a21 = pt2.x - the_pt.x;
                    double a31 = pt3.x - the_pt.x;

                    double a12 = pt1.y - the_pt.y;
                    double a22 = pt2.y - the_pt.y;
                    double a32 = pt3.y - the_pt.y;

                    double a13 = (pt1.x - the_pt.x) * (pt1.x - the_pt.x) + (pt1.y - the_pt.y) * (pt1.y - the_pt.y);
                    double a23 = (pt2.x - the_pt.x) * (pt2.x - the_pt.x) + (pt2.y - the_pt.y) * (pt2.y - the_pt.y);
                    double a33 = (pt3.x - the_pt.x) * (pt3.x - the_pt.x) + (pt3.y - the_pt.y) * (pt3.y - the_pt.y);

                    double det = (a11 * a22 * a33 + a12 * a23 * a31 + a13 * a21 * a32 - a13 * a22 * a31 - a12 * a21 * a33 - a11 * a23 * a32);

                    return ((is_oriented_ccw() == true) ? det > 0.0 : det < 0.0);
                }

                public bool is_point_inside_circumcircle(point_store pt)
                {
                    double d_squared = (pt.x - circum_center.x) * (pt.x - circum_center.x) +
                        (pt.y - circum_center.y) * (pt.y - circum_center.y);
                    return d_squared < (this._circum_circle_radius * this._circum_circle_radius);
                }

                private bool is_oriented_ccw()
                {
                    double a11 = pt1.x - pt3.x;
                    double a21 = pt2.x - pt3.x;

                    double a12 = pt1.y - pt3.y;
                    double a22 = pt2.y - pt3.y;

                    double det = a11 * a22 - a12 * a21;

                    return det > 0.0;
                }

                public bool Equals(triangle_store the_triangle)
                {
                    // find the triangle equals
                    if (this.contains_edge(the_triangle.e1) == true &&
                       this.contains_edge(the_triangle.e2) == true &&
                       this.contains_edge(the_triangle.e3) == true)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }
        #endregion

    }
}
