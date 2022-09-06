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

        public string str_boundary_curve_ids { get; private set; }

        public string str_end_pt_ids { get; private set; }

        public HashSet<point_store> closed_bndry_pts { get; private set; }

        public double bndry_area { get; private set; }

        public double centroid_x { get; private set; }

        public double centroid_y { get; private set; }

        public double x_min { get; private set; }

        public double x_max { get; private set; }

        public double y_min { get; private set; }

        public double y_max { get; private set; }

        public closed_boundary_store(int t_closed_bndry_id, HashSet<curve_store> t_boundary_curves)
        {
            // Main constructor
            this.closed_bndry_id = t_closed_bndry_id;
            this.boundary_curves = new HashSet<curve_store>(t_boundary_curves);

            // Add to closed boundary points (each curve is discretized to 100 pts)
            this.closed_bndry_pts = new HashSet<point_store>();
            string str_curve_id = "";
            string str_pt_id = "";

            // Save the ids of the noundary end pts (for datagridview)
            for (int i = 0; i < t_boundary_curves.Count; i++)
            {
                // set the pt ids to string (start pt id)
                str_pt_id = str_pt_id + t_boundary_curves.ElementAt(i).curve_end_pts.all_pts.ElementAt(0).pt_id + ", ";
            }

            foreach (curve_store curves in this.boundary_curves)
            {
                // Set the id to string
                str_curve_id = str_curve_id + curves.curve_id + ", ";

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

            // remove the last comma from the string and add to the variable
            this.str_boundary_curve_ids = str_curve_id.Substring(0,str_curve_id.Length - 2);
        }

        public void update_scale(double d_scale, double tran_tx, double tran_ty)
        {
            // update the curve scale
            for (int i = 0; i < boundary_curves.Count; i++)
            {
                this.boundary_curves.ElementAt(i).update_scale(d_scale, tran_tx, tran_ty);
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
            this.x_max = Double.MinValue;
            this.y_min = Double.MaxValue;
            this.y_max = Double.MinValue;

            for (int i = 0; i < n - 1; i++)
            {
                // Load the values
                x_i = closed_bndry_pts.ElementAt(i).d_x;
                y_i = closed_bndry_pts.ElementAt(i).d_y;
                x_ip1 = closed_bndry_pts.ElementAt(i + 1).d_x;
                y_ip1 = closed_bndry_pts.ElementAt(i + 1).d_y;

                // save the min & max of x & y
                this.x_min = this.x_min > x_i ? x_i : this.x_min;
                this.x_max = this.x_max < x_i ? x_i : this.x_max;
                this.y_min = this.y_min > y_i ? y_i : this.y_min;
                this.y_max = this.y_max < y_i ? y_i : this.y_max;

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
            this.x_max = this.x_max < x_i ? x_i : this.x_max;
            this.y_min = this.y_min > y_i ? y_i : this.y_min;
            this.y_max = this.y_max < y_i ? y_i : this.y_max;

            this.bndry_area = (c_area + ((x_i * y_ip1) - (x_ip1 * y_i))) / 2.0;

            if (this.bndry_area < 0.0)
            {
                // Reverse the points if the area is negative
                this.bndry_area = -1 * this.bndry_area;
                this.closed_bndry_pts = closed_bndry_pts.Reverse().ToHashSet();
            }

            // Set the x & y centroid
            double x_center = 0.0, y_center = 0.0;

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
            }

            // Final values
            x_i = closed_bndry_pts.ElementAt(n - 1).d_x;
            y_i = closed_bndry_pts.ElementAt(n - 1).d_y;
            x_ip1 = closed_bndry_pts.ElementAt(0).d_x;
            y_ip1 = closed_bndry_pts.ElementAt(0).d_y;

            // Calculate centroid (final pt)
            x_center = x_center + ((x_i + x_ip1) * ((x_i * y_ip1) - (x_ip1 * y_i)));
            y_center = y_center + ((y_i + y_ip1) * ((x_i * y_ip1) - (x_ip1 * y_i)));

            // Finalize the centroid and moment values
            this.centroid_x = (x_center / (6 * this.bndry_area));
            this.centroid_y = (y_center / (6 * this.bndry_area));
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
