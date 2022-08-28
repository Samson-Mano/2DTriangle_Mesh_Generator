using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class curve_discretization
    {
        Color pt_clr;

        public Tuple<lines_list_store, points_list_store> get_all_points_at_t(int discretized_count, curve_store.curve_types_enum ctype,points_list_store end_pts, points_list_store cntrl_pts, Color t_pt_clr)
        {
            this.pt_clr = t_pt_clr;
            if (ctype == curve_store.curve_types_enum.line)
            {
                // Line parameterized
                point_store start_pt = end_pts.all_pts.ElementAt(0);
                point_store end_pt = end_pts.all_pts.ElementAt(1);

                return get_lines_discretized(discretized_count, start_pt, end_pt);
            }
            else if (ctype == curve_store.curve_types_enum.arc)
            {
                // Arc parameterized
                point_store start_pt = end_pts.all_pts.ElementAt(0);
                point_store end_pt = end_pts.all_pts.ElementAt(1);
                point_store center_pt = cntrl_pts.all_pts.ElementAt(0); // Center point is pt 0
                point_store crown_pt = cntrl_pts.all_pts.ElementAt(1); // Crown point is pt 1

                return get_arcs_discretized(discretized_count, start_pt, end_pt, crown_pt, center_pt);
            }
            else if (ctype == curve_store.curve_types_enum.bezier)
            {
                // 3,4,5 point bezier
                List<point_store> bz_cntrl_pts = new List<point_store>();
                
                // Start point
                bz_cntrl_pts.Add(end_pts.all_pts.ElementAt(0));

                // Control points
                foreach(point_store c_pts in cntrl_pts.all_pts)
                {
                    bz_cntrl_pts.Add(c_pts);
                }

                // End point
                bz_cntrl_pts.Add(end_pts.all_pts.ElementAt(1));

   
                return get_beziers_discretized(discretized_count, bz_cntrl_pts);
            }

            // Below line must never trigger
            return null;
        }


        private Tuple<lines_list_store, points_list_store> get_lines_discretized(int discretized_count, point_store start_pt, point_store end_pt)
        {
            points_list_store rslt_pt_list = new points_list_store();
            lines_list_store rslt_ln_list = new lines_list_store();

            // Find the Line Discretized points
            double spt_x, spt_y;
            double ept_x, ept_y;

            double param_t = ((double)0 / (double)(discretized_count - 1));
            Tuple<double, double> pt_at_param_t = get_line_pt_at_t(param_t, start_pt, end_pt);
            spt_x = pt_at_param_t.Item1;
            spt_y = pt_at_param_t.Item2;

            // Add first point
            rslt_pt_list.add_point(0,spt_x, spt_y, this.pt_clr);

            for (int i = 1; i < discretized_count; i++)
            {
                param_t = ((double)i / (double)(discretized_count - 1));
                pt_at_param_t = get_line_pt_at_t(param_t, start_pt, end_pt);
                ept_x = pt_at_param_t.Item1;
                ept_y = pt_at_param_t.Item2;

                // Add to point list
                rslt_pt_list.add_point(i, ept_x, ept_y, this.pt_clr);

                // Add line list
                rslt_ln_list.add_line(i - 1, spt_x, spt_y, this.pt_clr, ept_x, ept_y, this.pt_clr);

                spt_x = ept_x;
                spt_y = ept_y;
            }

            return new Tuple<lines_list_store, points_list_store>(rslt_ln_list, rslt_pt_list);
        }

        private Tuple<lines_list_store, points_list_store> get_arcs_discretized(int discretized_count, point_store start_pt, point_store end_pt, point_store crown_pt, point_store center_pt)
        {
            points_list_store rslt_pt_list = new points_list_store();
            lines_list_store rslt_ln_list = new lines_list_store();

            // Get the arc angles
            Tuple<double, double> g_angles = global_variables.gvariables_static.get_arc_angles(new PointF((float)start_pt.d_x, (float)start_pt.d_y),
                new PointF((float)end_pt.d_x, (float)end_pt.d_y),
                new PointF((float)crown_pt.d_x, (float)crown_pt.d_y),
                new PointF((float)center_pt.d_x, (float)center_pt.d_y));

            // Arc radii, arc start, sweep angle and arc orientation
            double arc_radii = Math.Sqrt(Math.Pow(center_pt.d_x - crown_pt.d_x, 2) + Math.Pow(center_pt.d_y - crown_pt.d_y, 2));
            double start_angle = g_angles.Item1;
            double sweep_angle = g_angles.Item2;
            int arc_orientation = global_variables.gvariables_static.ordered_orientation(new PointF((float)start_pt.d_x, (float)start_pt.d_y),
                new PointF((float)end_pt.d_x, (float)end_pt.d_y),
                new PointF((float)crown_pt.d_x, (float)crown_pt.d_y));

            // Find the Arc Discretized points
            double spt_x, spt_y;
            double ept_x, ept_y;

            double param_t = ((double)0 / (double)(discretized_count - 1));
            Tuple<double, double> pt_at_param_t = get_arc_pt_at_t(param_t, start_angle, sweep_angle, arc_radii, center_pt.d_x, center_pt.d_y, arc_orientation);
            spt_x = pt_at_param_t.Item1;
            spt_y = pt_at_param_t.Item2;

            // Add first point
            rslt_pt_list.add_point(0,spt_x, spt_y, this.pt_clr);

            for (int i = 1; i < discretized_count; i++)
            {
                param_t = ((double)i / (double)(discretized_count - 1));
                pt_at_param_t = get_arc_pt_at_t(param_t, start_angle, sweep_angle, arc_radii, center_pt.d_x, center_pt.d_y, arc_orientation);
                ept_x = pt_at_param_t.Item1;
                ept_y = pt_at_param_t.Item2;

                // Add to point list
                rslt_pt_list.add_point(i,ept_x, ept_y, this.pt_clr);

                // Add line list
                rslt_ln_list.add_line(i - 1, spt_x, spt_y, this.pt_clr, ept_x, ept_y, this.pt_clr);

                spt_x = ept_x;
                spt_y = ept_y;
            }

            return new Tuple<lines_list_store, points_list_store>(rslt_ln_list, rslt_pt_list);
        }

        private Tuple<lines_list_store, points_list_store> get_beziers_discretized(int discretized_count, List<point_store> bz_cntrl_pts)
        {
            points_list_store rslt_pt_list = new points_list_store();
            lines_list_store rslt_ln_list = new lines_list_store();


            // Find the Beziers Discretized points
            double spt_x, spt_y;
            double ept_x, ept_y;

            double param_t = ((double)0 / (double)(discretized_count - 1));
            Tuple<double, double> pt_at_param_t = getCasteljauPoint(bz_cntrl_pts, bz_cntrl_pts.Count - 1, 0, param_t);
            spt_x = pt_at_param_t.Item1;
            spt_y = pt_at_param_t.Item2;

            // Add first point
            rslt_pt_list.add_point(0, spt_x, spt_y, this.pt_clr);

            for (int i = 1; i < discretized_count; i++)
            {
                param_t = ((double)i / (double)(discretized_count - 1));
                pt_at_param_t = getCasteljauPoint(bz_cntrl_pts, bz_cntrl_pts.Count - 1, 0, param_t);
                ept_x = pt_at_param_t.Item1;
                ept_y = pt_at_param_t.Item2;

                // Add to point list
                rslt_pt_list.add_point(i, ept_x, ept_y, this.pt_clr);

                // Add line list
                rslt_ln_list.add_line(i - 1, spt_x, spt_y, this.pt_clr, ept_x, ept_y, this.pt_clr);

                spt_x = ept_x;
                spt_y = ept_y;
            }

            return new Tuple<lines_list_store, points_list_store>(rslt_ln_list, rslt_pt_list);
        }

        public Tuple<double, double> get_line_pt_at_t(double param_t, point_store start_pt, point_store end_pt)
        {
            // Get line points
            double t_dx = start_pt.d_x * (1 - param_t) + (end_pt.d_x * param_t);
            double t_dy = start_pt.d_y * (1 - param_t) + (end_pt.d_y * param_t);

            return new Tuple<double, double>(t_dx, t_dy);
        }

        public Tuple<double, double> get_arc_pt_at_t(double param_t, double start_ang, double sweep_ang, double arc_radius, double arc_center_x, double arc_center_y, int arc_orientation)
        {
            // Get arc points
            if (arc_orientation < 0)
            {
                param_t = 1 - param_t;
            }

            double t_theta = (start_ang * (1 - param_t)) + ((start_ang + sweep_ang) * param_t);
            double x_p = arc_center_x + arc_radius * Math.Cos(t_theta * (Math.PI / 180));
            double y_p = arc_center_y + arc_radius * Math.Sin(t_theta * (Math.PI / 180));

            return new Tuple<double, double>(x_p, y_p);
        }

        public Tuple<double, double> getCasteljauPoint(List<point_store> cntrl_pts, int r, int i, double param_t)
        {
            if (r == 0) return new Tuple<double, double>(cntrl_pts[i].d_x, cntrl_pts[i].d_y);

            Tuple<double, double> p1 = getCasteljauPoint(cntrl_pts, r - 1, i, param_t);
            Tuple<double, double> p2 = getCasteljauPoint(cntrl_pts, r - 1, i + 1, param_t);

            return new Tuple<double, double>(((1 - param_t) * p1.Item1 + param_t * p2.Item1),
                                             ((1 - param_t) * p1.Item2 + param_t * p2.Item2));
        }

    }
}
