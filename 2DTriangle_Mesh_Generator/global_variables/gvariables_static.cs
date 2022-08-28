using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace _2DTriangle_Mesh_Generator.global_variables
{
    public static class gvariables_static
    {
        public static Color glcontrol_background_color = Color.White;

        // Garphics Control variables
        public static bool Is_panflg = false;
        public static bool Is_cntrldown = false;
        public static Color curve_color = Color.BlueViolet;

        // Ellipse size control
        public static double ellipse_size_control = 1.0;

        public static int RoundOff(this int i)
        {
            // Roundoff to nearest 10 (used to display zoom value)
            return ((int)Math.Round(i / 10.0)) * 10;
        }


        public static void Show_error_Dialog(string title, string text)
        {
            var form = new Form()
            {
                Text = title,
                Size = new Size(800, 600)
            };

            form.Controls.Add(new TextBox()
            {
                Font = new Font("Segoe UI", 12),
                Text = text,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill
            });

            form.ShowDialog();
            form.Controls.OfType<TextBox>().First().Dispose();
            form.Dispose();
        }


        public static double get_angle_ABX(PointF A_pt, PointF B_pt, bool is_deg = false)
        {
            // Angle made with X -axis (assuming pt B lies on x_axis)
            double delta_X, delta_Y;
            delta_X = Math.Abs(B_pt.X - A_pt.X);
            delta_Y = Math.Abs(B_pt.Y - A_pt.Y);

            // Angle with x-line
            double angle_with_xaxis;
            angle_with_xaxis = Math.Atan2(delta_Y, delta_X);

            if (B_pt.Y < A_pt.Y) // Second point is lower than first, angle goes down (180-360)
            {
                if (B_pt.X < A_pt.X) // Second point is to the left of first (180-270)
                {
                    angle_with_xaxis = angle_with_xaxis + Math.PI; // 180 degree to 270 degree
                }
                else // Angle range (270 to 360)
                {
                    angle_with_xaxis = (2 * Math.PI) - angle_with_xaxis;  // 270 degree to 360 degree
                }
            }
            else if (B_pt.X < A_pt.X) //Second point is top left of first (90-180)
            {
                angle_with_xaxis = Math.PI - angle_with_xaxis; // 90 degree to 180 degree
            }

            if (is_deg == true)
                angle_with_xaxis = angle_with_xaxis * (180 / Math.PI);

            return angle_with_xaxis;
        }

        public static int ordered_orientation(PointF p, PointF q, PointF r)
        {
            // To find orientation of ordered triplet (p, q, r).
            // The function returns following values
            // 0 --> p, q and r are collinear
            // 1 -->  Clockwise
            // -1 --> Counter clockwise

            double val = (((q.Y - p.Y) * (r.X - q.X)) - ((q.X - p.X) * (r.Y - q.Y)));

            if (Math.Round(val, 3) == 0) return 0; // collinear

            return (val > 0) ? 1 : -1; // clock or counterclock wise
        }

        public static double angle_between_2lines(PointF line1_pt1, PointF line1_pt2, PointF line2_pt1, PointF line2_pt2, bool to_deg = false)
        {
            double dx1, dy1;
            double norm;
            dx1 = line1_pt2.X - line1_pt1.X;
            dy1 = line1_pt2.Y - line1_pt1.Y;
            norm = Math.Sqrt((dx1 * dx1) + (dy1 * dy1));
            // vector 1
            dx1 = dx1 / norm;
            dy1 = dy1 / norm;

            double dx2, dy2;
            dx2 = line2_pt2.X - line2_pt1.X;
            dy2 = line2_pt2.Y - line2_pt1.Y;
            norm = Math.Sqrt((dx2 * dx2) + (dy2 * dy2));
            // vector 2
            dx2 = dx2 / norm;
            dy2 = dy2 / norm;

            // Dot product
            double angle_in_rad = Math.Acos((dx1 * dx2) + (dy1 * dy2));

            if (to_deg == true)
                angle_in_rad = angle_in_rad * (180 / Math.PI);

            return angle_in_rad;
        }

        public static Tuple<double, double> get_arc_angles(PointF chord_start_pt, PointF chord_end_pt, PointF pt_on_arc, PointF arc_center_pt)
        {
            // Start and sweep angle
            double start_angle, sweep_angle;
            if (ordered_orientation(chord_start_pt, chord_end_pt, pt_on_arc) > 0)
            {
                // Counter clockwise in screen co-ordinates
                start_angle = -1.0 * (360 - get_angle_ABX(arc_center_pt, chord_start_pt, true));
                sweep_angle = (angle_between_2lines(arc_center_pt, chord_start_pt, arc_center_pt, chord_end_pt, true));

                if (ordered_orientation(chord_start_pt, chord_end_pt, arc_center_pt) > 0)
                {
                    sweep_angle = 360 - sweep_angle;
                }
            }
            else
            {
                // Clockwise in screen co-ordinates
                start_angle = -1.0 * (360 - get_angle_ABX(arc_center_pt, chord_end_pt, true));
                sweep_angle = (angle_between_2lines(arc_center_pt, chord_end_pt, arc_center_pt, chord_start_pt, true));

                if (ordered_orientation(chord_end_pt, chord_start_pt, arc_center_pt) > 0)
                {
                    sweep_angle = 360 - sweep_angle;
                }
            }

            return new Tuple<double, double>(start_angle, sweep_angle);
        }
    }
}
