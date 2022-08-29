using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
// This app class structure
using _2DTriangle_Mesh_Generator.drawing_objects_store;
using _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements;
using _2DTriangle_Mesh_Generator.global_variables;

namespace _2DTriangle_Mesh_Generator.txt_input_reader
{
    public class txt_to_surface_conversion
    {
        private txt_rd_reader txt_rd_rslt;

        public HashSet<surface_store> all_surface { get; private set; }

        public HashSet<ellipse_store> all_ellipses { get; private set; }

        public float dr_scale { get; private set; }

        public float dr_tx { get; private set; }

        public float dr_ty { get; private set; }

        public txt_to_surface_conversion(txt_rd_reader t_txt_rd_rslt)
        {
            this.txt_rd_rslt = t_txt_rd_rslt;

            // initialize the surface list
            this.all_surface = new HashSet<surface_store>();

            foreach (txt_rd_reader.string_data_store surf_string in txt_rd_rslt.str_surf_datas)
            {
                int surf_id;
                int.TryParse(surf_string.str_id, out surf_id);

                // Find the outter closed boundary
                closed_boundary_store outer_closed_bndry = find_the_closed_bndry_curves(0, surf_string.str_main_data);

                // Find all the inner closed boundaries
                HashSet<closed_boundary_store> inner_closed_bndries = new HashSet<closed_boundary_store>();

                int bndry_id = 1;
                foreach (string str_nested_bndry in surf_string.str_additional_data)
                {
                    inner_closed_bndries.Add(find_the_closed_bndry_curves(bndry_id, str_nested_bndry));
                    bndry_id++;
                }
                // Add to the surface list
                this.all_surface.Add(new surface_store(surf_id, outer_closed_bndry, inner_closed_bndries));
            }


            if (this.all_surface.Count != 0)
            {
                // Set the boundary size
                double max_x = double.MinValue;
                double min_x = double.MaxValue;
                double max_y = double.MinValue;
                double min_y = double.MaxValue;

                foreach (surface_store surf in this.all_surface)
                {
                    foreach (curve_store curv in surf.closed_outer_bndry.boundary_curves)
                    {
                        foreach (point_store pts in curv.curve_t_pts.all_pts)
                        {
                            // Max, Min x
                            max_x = Math.Max(max_x, pts.d_x);
                            min_x = Math.Min(min_x, pts.d_x);
                            // Max, Min y
                            max_y = Math.Max(max_y, pts.d_y);
                            min_y = Math.Min(min_y, pts.d_y);
                        }
                    }
                }

                // Scale Translation
                double bound_x = Math.Abs(max_x - min_x);
                double bound_y = Math.Abs(max_y - min_y);

                this.dr_scale = (float)(1.8d / Math.Abs(Math.Max(bound_x, bound_y)));

                // Translation values
                this.dr_tx = (-0.5f * (float)(max_x + min_x));
                this.dr_ty = (-0.5f * (float)(max_y + min_y)); 

                // Update the scale of the surface
                for ( int i = 0; i< this.all_surface.Count;i++)
                {
                    this.all_surface.ElementAt(i).update_scale(this.dr_scale, -this.dr_tx, -this.dr_ty);
                }

                // initialize the end points
                this.all_ellipses = new HashSet<ellipse_store>();

                foreach (txt_rd_reader.string_data_store pts_string in txt_rd_rslt.str_endpt_datas)
                {
                    int pts_id;
                    int.TryParse(pts_string.str_id, out pts_id);

                    // Points
                    string[] str_pt = pts_string.str_main_data.Split(',');

                    // Convert to double
                    double x_coord, y_coord;
                    double.TryParse(str_pt[0], out x_coord);
                    double.TryParse(str_pt[1], out y_coord);

                    // Add to the surface list
                    this.all_ellipses.Add(new ellipse_store(pts_id, x_coord, y_coord, this.dr_scale, -this.dr_tx, -this.dr_ty, Color.Brown));
                }

            }
        }

        private closed_boundary_store find_the_closed_bndry_curves(int bndry_id, string str_bndry_curve_ids)
        {
            // Outter boundary
            int previous_end_pt_id = -1, previous_start_pt_id = -1;
            string[] bndry_curves_id = str_bndry_curve_ids.Split(',');

            HashSet<curve_store> bndry_curves = new HashSet<curve_store>();
            // Find the boundary lines
            for (int i = 0; i < bndry_curves_id.Length; i++)
            {
                int curve_id;
                int.TryParse(bndry_curves_id[i], out curve_id);
                string[] end_ptid = new string[2];

                // Find the type of boundary
                int cur_index = -1;

                // Check the lines
                cur_index = line_index_found(curve_id);

                if (cur_index != -1)
                {
                    end_ptid = this.txt_rd_rslt.str_line_datas[cur_index].str_main_data.Split(',');
                    int start_pt_id, end_pt_id;

                    // Find the start and end pt id
                    int.TryParse(end_ptid[0], out start_pt_id);
                    int.TryParse(end_ptid[1], out end_pt_id);

                    // Check the ids before proceeding
                    if (previous_end_pt_id != start_pt_id && previous_end_pt_id != end_pt_id)
                    {
                        if (previous_end_pt_id == -1)
                        {
                            // First instance so ignore
                        }
                        else if (previous_start_pt_id == start_pt_id || previous_start_pt_id == end_pt_id)
                        {
                            int temp_id = previous_start_pt_id;
                            previous_start_pt_id = previous_end_pt_id;
                            previous_end_pt_id = temp_id;
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Error");
                        }
                    }

                    // Flip ids based on the previous id
                    if ((previous_end_pt_id != start_pt_id) && previous_end_pt_id != -1)
                    {
                        int temp_id = start_pt_id;
                        start_pt_id = end_pt_id;
                        end_pt_id = temp_id;
                    }

                    previous_start_pt_id = start_pt_id;
                    previous_end_pt_id = end_pt_id;

                    point_store start_pt = str_to_points(start_pt_id, this.txt_rd_rslt.str_endpt_datas[endpt_index_found(start_pt_id)].str_main_data);
                    point_store end_pt = str_to_points(end_pt_id, this.txt_rd_rslt.str_endpt_datas[endpt_index_found(end_pt_id)].str_main_data);
                    // Additional inputs (Control points) Empty for line
                    List<point_store> cntrl_pts = new List<point_store>();

                    // Create line
                    curve_store line = new curve_store(curve_id, start_pt, end_pt, cntrl_pts, 100, curve_store.curve_types_enum.line, gvariables_static.curve_color);
                    // Add to the boundary curve list
                    bndry_curves.Add(line);
                    continue;
                }

                // Check the arcs
                cur_index = arc_index_found(curve_id);

                if (cur_index != -1)
                {
                    end_ptid = this.txt_rd_rslt.str_arc_datas[cur_index].str_main_data.Split(',');
                    int start_pt_id, end_pt_id;

                    // Find the start and end pt id
                    int.TryParse(end_ptid[0], out start_pt_id);
                    int.TryParse(end_ptid[1], out end_pt_id);


                    // Check the ids before proceeding
                    if (previous_end_pt_id != start_pt_id && previous_end_pt_id != end_pt_id)
                    {
                        if (previous_end_pt_id == -1)
                        {
                            // First instance so ignore
                        }
                        else if (previous_start_pt_id == start_pt_id || previous_start_pt_id == end_pt_id)
                        {
                            int temp_id = previous_start_pt_id;
                            previous_start_pt_id = previous_end_pt_id;
                            previous_end_pt_id = temp_id;
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Error");
                        }
                    }

                    // Flip ids based on the previous id
                    if ((previous_end_pt_id != start_pt_id) && previous_end_pt_id != -1)
                    {
                        int temp_id = start_pt_id;
                        start_pt_id = end_pt_id;
                        end_pt_id = temp_id;
                    }

                    previous_start_pt_id = start_pt_id;
                    previous_end_pt_id = end_pt_id;

                    point_store start_pt = str_to_points(start_pt_id, this.txt_rd_rslt.str_endpt_datas[endpt_index_found(start_pt_id)].str_main_data);
                    point_store end_pt = str_to_points(end_pt_id, this.txt_rd_rslt.str_endpt_datas[endpt_index_found(end_pt_id)].str_main_data);

                    // Additional inputs (Control points)
                    List<point_store> cntrl_pts = new List<point_store>();
                    int c_id = -100;
                    foreach (string str_cntrl_pt in this.txt_rd_rslt.str_arc_datas[cur_index].str_additional_data)
                    {
                        cntrl_pts.Add(str_to_points(c_id, str_cntrl_pt));
                        c_id = c_id - 1;
                    }

                    // Create Arc
                    curve_store arc = new curve_store(curve_id, start_pt, end_pt, cntrl_pts, 100, curve_store.curve_types_enum.arc, gvariables_static.curve_color);
                    // Add to the boundary curve list
                    bndry_curves.Add(arc);
                    continue;
                }

                // Check the beziers
                cur_index = bezier_index_found(curve_id);

                if (cur_index != -1)
                {
                    end_ptid = this.txt_rd_rslt.str_bezier_datas[cur_index].str_main_data.Split(',');
                    int start_pt_id, end_pt_id;

                    // Find the start and end pt id
                    int.TryParse(end_ptid[0], out start_pt_id);
                    int.TryParse(end_ptid[1], out end_pt_id);


                    // Check the ids before proceeding
                    if (previous_end_pt_id != start_pt_id && previous_end_pt_id != end_pt_id)
                    {
                        if (previous_end_pt_id == -1)
                        {
                            // First instance so ignore
                        }
                        else if (previous_start_pt_id == start_pt_id || previous_start_pt_id == end_pt_id)
                        {
                            int temp_id = previous_start_pt_id;
                            previous_start_pt_id = previous_end_pt_id;
                            previous_end_pt_id = temp_id;
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Error");
                        }
                    }

                    // Flip ids based on the previous id
                    bool cntrl_pt_reverse = false;
                    if ((previous_end_pt_id != start_pt_id) && previous_end_pt_id != -1)
                    {
                        int temp_id = start_pt_id;
                        start_pt_id = end_pt_id;
                        end_pt_id = temp_id;
                        cntrl_pt_reverse = true;
                    }

                    previous_start_pt_id = start_pt_id;
                    previous_end_pt_id = end_pt_id;

                    point_store start_pt = str_to_points(start_pt_id, this.txt_rd_rslt.str_endpt_datas[endpt_index_found(start_pt_id)].str_main_data);
                    point_store end_pt = str_to_points(end_pt_id, this.txt_rd_rslt.str_endpt_datas[endpt_index_found(end_pt_id)].str_main_data);

                    // Additional inputs (Control points)
                    List<point_store> cntrl_pts = new List<point_store>();
                    int c_id = -100;
                    foreach (string str_cntrl_pt in this.txt_rd_rslt.str_bezier_datas[cur_index].str_additional_data)
                    {
                        cntrl_pts.Add(str_to_points(c_id, str_cntrl_pt));
                        c_id = c_id - 1;
                    }

                    if (cntrl_pt_reverse == true)
                    {
                        // reverse the control points if the start and end points are reversed
                        cntrl_pts.Reverse();
                    }

                    // Create Arc
                    curve_store bezier = new curve_store(curve_id, start_pt, end_pt, cntrl_pts, 100, curve_store.curve_types_enum.bezier, gvariables_static.curve_color);
                    // Add to the boundary curve list
                    bndry_curves.Add(bezier);
                    continue;
                }
            }

            return new closed_boundary_store(bndry_id, bndry_curves);
        }

        private int line_index_found(int curve_id)
        {
            int out_index = 0;
            foreach (txt_rd_reader.string_data_store line_string in this.txt_rd_rslt.str_line_datas)
            {
                int line_id;
                int.TryParse(line_string.str_id, out line_id);

                if (line_id == curve_id)
                {
                    // Return the index of the line id found
                    return out_index;
                }
                out_index++;
            }

            // Return -1 if not found
            return -1;
        }

        private int arc_index_found(int curve_id)
        {
            int out_index = 0;
            foreach (txt_rd_reader.string_data_store arc_string in this.txt_rd_rslt.str_arc_datas)
            {
                int arc_id;
                int.TryParse(arc_string.str_id, out arc_id);

                if (arc_id == curve_id)
                {
                    // Return the index of the arc id found
                    return out_index;
                }
                out_index++;
            }

            // Return -1 if not found
            return -1;
        }

        private int bezier_index_found(int curve_id)
        {
            int out_index = 0;
            foreach (txt_rd_reader.string_data_store bezier_string in this.txt_rd_rslt.str_bezier_datas)
            {
                int bezier_id;
                int.TryParse(bezier_string.str_id, out bezier_id);

                if (bezier_id == curve_id)
                {
                    // Return the index of the bezier id found
                    return out_index;
                }
                out_index++;
            }

            // Return -1 if not found
            return -1;
        }

        private int endpt_index_found(int pt_id)
        {
            int out_index = 0;
            foreach (txt_rd_reader.string_data_store endpt_string in this.txt_rd_rslt.str_endpt_datas)
            {
                int endpt_id;
                int.TryParse(endpt_string.str_id, out endpt_id);

                if (endpt_id == pt_id)
                {
                    // Return the index of the line id found
                    return out_index;
                }
                out_index++;
            }

            // Return -1 if not found
            return -1;
        }

        private point_store str_to_points(int pt_id, string str_ptdata)
        {
            // Points
            string[] str_pt = str_ptdata.Split(',');

            // Convert to double
            double x_coord, y_coord;
            double.TryParse(str_pt[0], out x_coord);
            double.TryParse(str_pt[1], out y_coord);

            // Create point and return
            return new point_store(pt_id, x_coord, y_coord, gvariables_static.curve_color);
        }
    }
}
