using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class curve_store
    {
        public int curve_id { get; private set; }

        public double curve_length { get; private set; }

        public int curve_element_density { get; private set; }

        public points_list_store curve_end_pts { get; private set; }

        public point_store curve_mid_pt { get; private set; }

        public points_list_store curve_cntrl_pts { get; private set; }

        public points_list_store curve_t_pts { get; private set; }

        public lines_list_store curve_as_tlines { get; private set; }

        public HashSet<ellipse_store> curve_element_density_nodes { get; private set; }

        public curve_types_enum curve_type { get; private set; }

        public Color curve_color { get; private set; }

        private curve_discretization _curve_discrezitation;

        private Label_list_store elment_density_label;


        public enum curve_types_enum
        {
            line,
            arc,
            bezier
        }

        public curve_store(int t_cur_id, point_store t_curve_start_pt, point_store t_curve_end_pt, List<point_store> t_curve_cntrl_pts, int t_count, curve_types_enum t_ctype, Color t_curve_clr)
        {
            // Main constructor
            this.curve_id = t_cur_id;
            // Add curve end points
            this.curve_end_pts = new points_list_store();
            // Start pt
            this.curve_end_pts.add_point(t_curve_start_pt);

            // Add curve control points
            this.curve_cntrl_pts = new points_list_store();
            foreach (point_store c_pts in t_curve_cntrl_pts)
            {
                this.curve_cntrl_pts.add_point(c_pts);
            }

            // End pt
            this.curve_end_pts.add_point(t_curve_end_pt);

            this.curve_type = t_ctype;
            this.curve_color = t_curve_clr;

            // Discretize the curve into segments
            // Find all the curve points & lines
            this._curve_discrezitation = new curve_discretization(100, t_ctype, this.curve_end_pts, this.curve_cntrl_pts, this.curve_color);
            Tuple<lines_list_store, points_list_store> temp = this._curve_discrezitation.get_all_points_at_t();

            // Add all the lines and points to the list
            this.curve_as_tlines = temp.Item1;
            this.curve_t_pts = temp.Item2;

            // get the mid pt
            this.curve_mid_pt = this.curve_t_pts.all_pts.ElementAt(50);


            // Add curve length
            this.curve_length = get_c_length();
        }

        public double get_c_length()
        {
            double c_length = 0.0;

            if (curve_type == curve_types_enum.line)
            {
                // Line
                point_store s_pt = curve_end_pts.all_pts.ElementAt(0);
                point_store e_pt = curve_end_pts.all_pts.ElementAt(1);

                c_length = (Math.Sqrt(Math.Pow((s_pt.d_x - e_pt.d_x), 2) + Math.Pow((s_pt.d_y - e_pt.d_y), 2)));
            }
            else if (curve_type == curve_types_enum.arc)
            {
                // Arc
                point_store s_pt = curve_end_pts.all_pts.ElementAt(0);
                point_store e_pt = curve_end_pts.all_pts.ElementAt(1);
                point_store arc_center_pt = curve_cntrl_pts.all_pts.ElementAt(0); // Center point is pt 0
                point_store arc_crown_pt = curve_cntrl_pts.all_pts.ElementAt(1); // Crown point is pt 1

                // Arc radius
                double arc_radius = (Math.Sqrt(Math.Pow((arc_center_pt.d_x - arc_crown_pt.d_x), 2) +
                    Math.Pow((arc_center_pt.d_y - arc_crown_pt.d_y), 2)));

                Tuple<double, double> arc_angles = global_variables.gvariables_static.get_arc_angles(new PointF((float)s_pt.d_x, (float)s_pt.d_y),
                    new PointF((float)e_pt.d_x, (float)e_pt.d_y),
                    new PointF((float)arc_crown_pt.d_x, (float)arc_crown_pt.d_y),
                    new PointF((float)arc_center_pt.d_x, (float)arc_center_pt.d_y));

                double arc_start_angle = arc_angles.Item1;
                double arc_sweep_angle = arc_angles.Item2;

                c_length = arc_radius * arc_sweep_angle * (Math.PI / 180.0f);
            }
            else
            {
                // Bezier
                List<point_store> bezier_c_pts = curve_t_pts.all_pts.ToList();
                point_store s_pt = bezier_c_pts.First();


                for (int i = 1; i < bezier_c_pts.Count; i++)
                {
                    point_store e_pt = bezier_c_pts[i];

                    c_length = c_length + (Math.Sqrt(Math.Pow((s_pt.d_x - e_pt.d_x), 2) + Math.Pow((s_pt.d_y - e_pt.d_y), 2)));

                    s_pt = e_pt;
                }

            }


            return c_length;
        }

        public void set_curve_elementdensity(double min_element_length)
        {
            double elem_wd = this.curve_length / min_element_length;
            this.curve_element_density = (int)Math.Ceiling(Math.Round(elem_wd));

            // Set the element density ellipse (to indicate the element density)
            set_curve_elementdensity_ellipse();
        }

        public void set_curve_elementdensity(int t_element_density)
        {
            this.curve_element_density = t_element_density;

            // Set the element density ellipse (to indicate the element density)
            set_curve_elementdensity_ellipse();
        }

        public void set_curve_elementdensity_ellipse()
        {
            // Set the curve element density lines
            curve_element_density_nodes = new HashSet<ellipse_store>();
            int e_id = 0;

            for (int i = 1; i < this.curve_element_density; i++)
            {
                double elem_t = (double)i / this.curve_element_density;

                // Get the point at parameter x
                Tuple<double, double> pt_at_param_t = this._curve_discrezitation.get_point_at_t(elem_t);
                double pt_x, pt_y;

                pt_x = pt_at_param_t.Item1;
                pt_y = pt_at_param_t.Item2;

                curve_element_density_nodes.Add(new ellipse_store(e_id, pt_x, pt_y,
                    global_variables.gvariables_static.drawing_scale,
                    -global_variables.gvariables_static.drawing_tx,
                    -global_variables.gvariables_static.drawing_ty,
                    Color.DarkRed));
                e_id++;
            }

            // Set the element density text
            string label_val = this.curve_element_density.ToString();
            elment_density_label = new Label_list_store();
            elment_density_label.add_label(this.curve_id, label_val, this.curve_mid_pt.d_x, this.curve_mid_pt.d_y,
                global_variables.gvariables_static.drawing_scale, 
                -global_variables.gvariables_static.drawing_tx, 
                -global_variables.gvariables_static.drawing_ty, 
                Color.DarkRed);

        }


        public List<string> get_curve_data()
        {
            // get the curve data
            List<string> curve_data = new List<string>();

            // Curve ID
            curve_data.Add(curve_id.ToString());

            // Start PT
            curve_data.Add(curve_end_pts.all_pts.ElementAt(0).pt_id.ToString());

            // End PT
            curve_data.Add(curve_end_pts.all_pts.ElementAt(1).pt_id.ToString());

            // Element Length
            curve_data.Add(curve_length.ToString("F4"));

            // Element density
            curve_data.Add(curve_element_density.ToString());


            // Is element meshed
            curve_data.Add("false");

            // Element type
            curve_data.Add(curve_type.ToString());

            return curve_data;
        }

        public void update_scale(double d_scale, double tran_tx, double tran_ty)
        {
            // update the scale of curve points
            curve_end_pts.update_scale(d_scale, tran_tx, tran_ty);
            curve_cntrl_pts.update_scale(d_scale, tran_tx, tran_ty);
            curve_t_pts.update_scale(d_scale, tran_tx, tran_ty);
            curve_as_tlines.update_scale(d_scale, tran_tx, tran_ty);
        }


        public void set_openTK_objects()
        {
            // Set the discretized lines openTK 
            this.curve_as_tlines.set_openTK_objects();
        }


        public void set_highlight_openTK_objects(bool is_selected)
        {
            // Set the discretized lines openTK 
            this.curve_as_tlines.set_highlight_openTK_objects();

            // Paint the discretized element list
            foreach (ellipse_store e in this.curve_element_density_nodes)
            {
                e.set_openTK_objects();
            }

            if (is_selected == true)
            {
                // Set the text indicating element density to show the edge is selected
                elment_density_label.set_openTK_objects();
            }
        }

        public void paint_curve()
        {
            // Paint the curves
            // Set openTK becore calling this function
            this.curve_as_tlines.paint_all_lines();
        }

        public void paint_highlight_curve()
        {
            // Paint the highlight curves
            // Set openTK becore calling this function
            this.curve_as_tlines.paint_all_highlight_lines();

            // Paint the discretized element list
            foreach (ellipse_store e in this.curve_element_density_nodes)
            {
                e.paint_ellipse();
            }
        }

        public void paint_highlight_curve_label()
        {
            elment_density_label.paint_labels();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as curve_store);
        }

        public bool Equals(curve_store other_curve)
        {
            // Check 1 (curve ids should not match)
            if (this.Equals(other_curve.curve_id) == true)
            {
                return true;
            }

            // Check 2 (Whether curve type and curve control points match)
            if (this.curve_type.Equals(other_curve.curve_type))
            {
                // Check the end points
                HashSet<point_store> other_curve_endpts_list = other_curve.curve_end_pts.all_pts;
                if (this.curve_end_pts.all_pts.Except(other_curve_endpts_list).Count() == 0 ||
                    this.curve_end_pts.all_pts.Except(other_curve_endpts_list.Reverse()).Count() == 0)
                {
                    // Not tested Yet !!
                    return true;
                }

                // Check the control points
                HashSet<point_store> other_curve_cntrlpts_list = other_curve.curve_cntrl_pts.all_pts;
                if (this.curve_cntrl_pts.all_pts.Except(other_curve_cntrlpts_list).Count() == 0 ||
                    this.curve_cntrl_pts.all_pts.Except(other_curve_cntrlpts_list.Reverse()).Count() == 0)
                {
                    // Not tested Yet !!
                    return true;
                }

            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.curve_id, this.curve_end_pts.GetHashCode(), this.curve_cntrl_pts.GetHashCode());
        }

    }
}
