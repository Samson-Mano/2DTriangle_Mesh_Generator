using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
// This app class structure
using _2DTriangle_Mesh_Generator.opentk_control.opentk_buffer;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class closed_boundary_store
    {
        public int closed_bndry_id { get; private set; }
        public HashSet<curve_store> boundary_curves { get; private set; }
        public HashSet<point_store> closed_bndry_pts { get; private set; }

        public double bndry_area { get; private set; }

        public double centroid_x { get; private set; }

        public double centroid_y { get; private set; }

        public double x_min { get; private set; }

        public double y_min { get; private set; }

        public double moi_x { get; private set; }
        public double moi_y { get; private set; }
        public double moi_xy { get; private set; }
        public double moi_P1 { get; private set; }
        public double moi_P2 { get; private set; }
        public double moi_theta { get; private set; }

        public closed_boundary_store(int t_closed_bndry_id, HashSet<curve_store> t_boundary_curves)
        {
            // Main constructor
            this.closed_bndry_id = t_closed_bndry_id;
            this.boundary_curves = new HashSet<curve_store>(t_boundary_curves);

            // Add to closed boundary points (each curve is discretized to 100 pts)
            this.closed_bndry_pts = new HashSet<point_store>();
            foreach (curve_store curves in this.boundary_curves)
            {
                foreach (point_store pt in curves.curve_t_pts.all_pts)
                {
                    int indx = this.closed_bndry_pts.Count - 1;
                    if (indx == -1)
                    {
                        // Add first
                        this.closed_bndry_pts.Add(pt);
                        continue;
                    }

                    // Check the subsequent points before adding the point
                    if (pt.d_x != this.closed_bndry_pts.ElementAt(indx).d_x ||
                        pt.d_y != this.closed_bndry_pts.ElementAt(indx).d_y)
                    {
                        this.closed_bndry_pts.Add(pt);
                    }
                }

                // Remove the final point if the first and last are same
                if (this.closed_bndry_pts.ElementAt(0).d_x == this.closed_bndry_pts.ElementAt(this.closed_bndry_pts.Count - 1).d_x &&
                    this.closed_bndry_pts.ElementAt(0).d_y == this.closed_bndry_pts.ElementAt(this.closed_bndry_pts.Count - 1).d_y)
                {
                    this.closed_bndry_pts.Remove(this.closed_bndry_pts.ElementAt(this.closed_bndry_pts.Count - 1));
                }
            }

        }

        public void set_openTK_objects()
        {
            // Set the curves associated with boundaries openTK 
            foreach (curve_store bndry_curve in this.boundary_curves)
            {
                bndry_curve.set_openTK_objects();
            }
        }

        public void set_geometric_properties()
        {
            // https://leancrew.com/all-this/2018/01/greens-theorem-and-section-properties/
            // http://paulbourke.net/geometry/polygonmesh/
            // Set the cross- section area

            double c_area = 0.0;
            int n = closed_bndry_pts.Count;

            double x_i, x_ip1;
            double y_i, y_ip1;
            this.x_min = Double.MaxValue;
            this.y_min = Double.MaxValue;
            
            for (int i = 0; i < n - 1; i++)
            {
                // Load the values
                x_i = closed_bndry_pts.ElementAt(i).d_x;
                y_i = closed_bndry_pts.ElementAt(i).d_y;
                x_ip1 = closed_bndry_pts.ElementAt(i + 1).d_x;
                y_ip1 = closed_bndry_pts.ElementAt(i + 1).d_y;

                // save the min of x & y
                this.x_min = this.x_min > x_i ? x_i : this.x_min;
                this.y_min = this.y_min > y_i ? y_i : this.y_min;   

                // Calculate area
                c_area = c_area + ((x_i * y_ip1) - (x_ip1 * y_i));
            }

            // Final values
            x_i = closed_bndry_pts.ElementAt(n - 1).d_x;
            y_i = closed_bndry_pts.ElementAt(n - 1).d_y;
            x_ip1 = closed_bndry_pts.ElementAt(0).d_x;
            y_ip1 = closed_bndry_pts.ElementAt(0).d_y;


            // save the min of x & y
            this.x_min = this.x_min > x_i ? x_i : this.x_min;
            this.y_min = this.y_min > y_i ? y_i : this.y_min;

            this.bndry_area = (c_area + ((x_i * y_ip1) - (x_ip1 * y_i))) / 2.0;
            
            if (this.bndry_area < 0.0)
            {
                // Reverse the points if the area is negative
                this.bndry_area = -1 * this.bndry_area;
                this.closed_bndry_pts = closed_bndry_pts.Reverse().ToHashSet();
            }


            // Set the x & y centroid
            double x_center = 0.0, y_center = 0.0;
            // Set the Moments and product of inertia about centroid
            double s_xx = 0.0, s_yy = 0.0, s_xy = 0.0;

            for (int i = 0; i < n - 1; i++)
            {
                // Load the values
                x_i = closed_bndry_pts.ElementAt(i).d_x;
                y_i = closed_bndry_pts.ElementAt(i).d_y;
                x_ip1 = closed_bndry_pts.ElementAt(i + 1).d_x;
                y_ip1 = closed_bndry_pts.ElementAt(i + 1).d_y;


                // Calculate centroid
                x_center = x_center + ((x_i + x_ip1) * ((x_i * y_ip1) - (x_ip1 * y_i)));
                y_center = y_center + ((y_i + y_ip1) * ((x_i * y_ip1) - (x_ip1 * y_i)));

                // Calculate the moment of inertia
                s_xx = s_xx + (Math.Pow(y_i, 2) + (y_i * y_ip1) + Math.Pow(y_ip1, 2)) *
                    ((x_i * y_ip1) - (x_ip1 * y_i));
                s_yy = s_yy + (Math.Pow(x_i, 2) + (x_i * x_ip1) + Math.Pow(x_ip1, 2)) *
                    ((x_i * y_ip1) - (x_ip1 * y_i));
                s_xy = s_xy + ((x_i * y_ip1) + (2 * x_i * y_i) + (2 * x_ip1 * y_ip1) + (x_ip1 * y_i)) *
                    ((x_i * y_ip1) - (x_ip1 * y_i));
            }

            // Final values
            x_i = closed_bndry_pts.ElementAt(n - 1).d_x;
            y_i = closed_bndry_pts.ElementAt(n - 1).d_y;
            x_ip1 = closed_bndry_pts.ElementAt(0).d_x;
            y_ip1 = closed_bndry_pts.ElementAt(0).d_y;

            // Calculate centroid (final pt)
            x_center = x_center + ((x_i + x_ip1) * ((x_i * y_ip1) - (x_ip1 * y_i)));
            y_center = y_center + ((y_i + y_ip1) * ((x_i * y_ip1) - (x_ip1 * y_i)));

            // Calculate the moment of inertia (final pt)
            s_xx = s_xx + (Math.Pow(y_i, 2) + (y_i * y_ip1) + Math.Pow(y_ip1, 2)) *
                ((x_i * y_ip1) - (x_ip1 * y_i));
            s_yy = s_yy + (Math.Pow(x_i, 2) + (x_i * x_ip1) + Math.Pow(x_ip1, 2)) *
                ((x_i * y_ip1) - (x_ip1 * y_i));
            s_xy = s_xy + ((x_i * y_ip1) + (2 * x_i * y_i) + (2 * x_ip1 * y_ip1) + (x_ip1 * y_i)) *
                ((x_i * y_ip1) - (x_ip1 * y_i));


            // Finalize the centroid and moment values
            this.centroid_x = (x_center / (6 * this.bndry_area));
            this.centroid_y = (y_center / (6 * this.bndry_area));

            this.moi_x = (s_xx / 12.0) - (this.bndry_area * Math.Pow(this.centroid_y, 2));
            this.moi_y = (s_yy / 12.0) - (this.bndry_area * Math.Pow(this.centroid_x, 2));
            this.moi_xy = (s_xy / 24.0) - (this.bndry_area * this.centroid_y * this.centroid_x);

            // Principal moment of inertia and direction
            double avg1 = (this.moi_x + this.moi_y) * 0.5;
            double diff1 = (this.moi_x - this.moi_y) * 0.5;

            this.moi_P1 = avg1 + Math.Sqrt(Math.Pow(diff1, 2) + Math.Pow(this.moi_xy, 2));
            this.moi_P2 = avg1 - Math.Sqrt(Math.Pow(diff1, 2) + Math.Pow(this.moi_xy, 2));
            this.moi_theta = Math.Atan2(-1 * this.moi_xy, diff1) * 0.5;
        }

        public void paint_closed_boundary()
        {
            // Paint the curves
            // Set openTK becore calling this function
            foreach (curve_store bndry_curve in this.boundary_curves)
            {
                bndry_curve.paint_curve();
            }
        }
    }
}
