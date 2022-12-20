using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation;

namespace _2DTriangle_Mesh_Generator.mesh_control
{
    public class constrained_delaunay_triangle_main
    {


        public static double eps = 0.000001; // 10^-6

        public bool is_surface_read = false;

        public surface_store input_surface { get; private set; }

        public constrained_delaunay_triangle_main(string inpt_string)
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
            HashSet<point_store> bndry_pts = new HashSet<point_store>();
            int bndryid;

            using (StringReader reader = new StringReader(bndry_str))
            {
                string line;

                // Read the first line
                line = reader.ReadLine();

                string bndryid_str = line.Split('=').Last();
                bndryid = int.Parse(bndryid_str);

                int pt_id = 0;

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

                    double x_coord;
                    double y_coord;
                    int pt_type;

                    if (id_str.StartsWith("e") == true)
                    {
                        pt_type = 2; // Points on the edges
                    }
                    else
                    {
                        pt_type = 1; // Corner points
                    }

                    // Parse the x & y coordinates
                    x_coord = Double.Parse(x_str);
                    y_coord = Double.Parse(y_str);

                    // Add to the boundary point list
                    point_store ind_bndry_pt = new point_store(pt_id, x_coord, y_coord, pt_type);
                    bndry_pts.Add(ind_bndry_pt);
                    pt_id++;
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
                public HashSet<point_store> bndry_pts { get; private set; }
                public bool is_outter_bndry { get; private set; }


                public boundary_store(int t_bndry_id, HashSet<point_store> t_bndry_pts, bool t_is_outter_bndry)
                {
                    this.boundary_id = t_bndry_id;
                    this.bndry_pts = t_bndry_pts;
                    this.is_outter_bndry = t_is_outter_bndry;
                }

                public bool is_point_in_boundary(point_d i_pt)
                {
                    // Get the angle between input_pt and the first & last boundary pt
                    int max_pt = this.bndry_pts.Count - 1;

                    List<point_store> b_pts = this.bndry_pts.ToList();

                    double t_angle = point_d.GetAngle(b_pts[b_pts.Count - 1].pt_coord, i_pt, b_pts[0].pt_coord);

                    // Add the angle  to the inpt_pt and other boundary pts
                    for (int i = 0; i < b_pts.Count - 1; i++)
                    {
                        t_angle += point_d.GetAngle(b_pts[i].pt_coord, i_pt, b_pts[i + 1].pt_coord);
                    }

                    return (Math.Abs(t_angle) > 1);

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
                List<point_d> corner_points = new List<point_d>();
                List<point_d> edge_points = new List<point_d>();
                List<point_d> outter_bndry_pts = new List<point_d>();

                // Get the outter boundary points
                foreach (point_store pts in outter_bndry.bndry_pts)
                {
                    if(pts.pt_type == 1)
                    {
                        // Add to corner point list
                        corner_points.Add(pts.pt_coord);
                    }
                    else if(pts.pt_type == 2)
                    {
                        // Add to edge point list
                        edge_points.Add(pts.pt_coord);
                    }

                    // Add the outter boudary points
                    outter_bndry_pts.Add(pts.pt_coord);
                }

                // Get the inner boundary points
                foreach (boundary_store innr_bndry in inner_bndries)
                {
                    foreach (point_store pts in innr_bndry.bndry_pts)
                    {
                        if (pts.pt_type == 1)
                        {
                            // Add to corner point list
                            corner_points.Add(pts.pt_coord);
                        }
                        else if (pts.pt_type == 2)
                        {
                            // Add to edge point list
                            edge_points.Add(pts.pt_coord);
                        }
                    }
                }

                // Delaunay triangulation happens here
                mesh_data = new mesh_store(corner_points,edge_points, outter_bndry_pts);
              

            }

            private bool is_point_in_surface(point_d i_pt)
            {
                if (outter_bndry.is_point_in_boundary(i_pt) == true)
                {
                    foreach (boundary_store innr_bndry in inner_bndries)
                    {
                        if (innr_bndry.is_point_in_boundary(i_pt) == true)
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
        }

        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

    }
}
