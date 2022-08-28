using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class surface_store
    {
        public int surf_id { get; private set; }

        public double surf_area { get; private set; }

        public double x_min { get; private set; }

        public double y_min { get; private set; }

        public double x_centroid { get; private set; }

        public double y_centroid { get; private set; }

        public double x_area_moment { get; private set; }

        public double y_area_moment { get; private set; }

        public double xy_area_moment { get; private set; }

        public double area_moment_p1 { get; private set; }

        public double area_moment_p2 { get; private set; }

        public double area_moment_theta { get; private set; }

        public closed_boundary_store closed_outer_bndry { get; private set; }

        public HashSet<closed_boundary_store> closed_inner_bndries { get; private set; }

        public surface_store(int t_surf_id, closed_boundary_store t_closed_outer_bndry, HashSet<closed_boundary_store> t_closed_inner_bndries)
        {
            // Main constructor
            this.surf_id = t_surf_id;

            // Closed outter boundary
            this.closed_outer_bndry = t_closed_outer_bndry;

            // Closed inner boundaries
            this.closed_inner_bndries = new HashSet<closed_boundary_store>(t_closed_inner_bndries);

            set_surface_geometric_parameter();
        }

        private void set_surface_geometric_parameter()
        {
            // https://leancrew.com/all-this/2018/01/greens-theorem-and-section-properties/

            // Get the surface area
            // Outter boundary
            // Set the outter boundary properties
            this.closed_outer_bndry.set_geometric_properties();

            double outer_boundary_area = this.closed_outer_bndry.bndry_area;
            // Centroid of outter boundary
            double x_center = this.closed_outer_bndry.centroid_x * outer_boundary_area;
            double y_center = this.closed_outer_bndry.centroid_y * outer_boundary_area;
            // Second moment of area
            double x_moi = this.closed_outer_bndry.moi_x;
            double y_moi = this.closed_outer_bndry.moi_y;
            double xy_moi = this.closed_outer_bndry.moi_xy;
            // Principal moment of area
            double moi_p1 = this.closed_outer_bndry.moi_P1;
            double moi_p2 = this.closed_outer_bndry.moi_P2;
            double moi_theta = this.closed_outer_bndry.moi_theta;

            // minimum x & y
            this.x_min = this.x_min> this.closed_outer_bndry.x_min? this.closed_outer_bndry.x_min : this.x_min;
            this.y_min = this.y_min > this.closed_outer_bndry.y_min ? this.closed_outer_bndry.y_min : this.y_min;

            double inner_boundary_area = 0.0;

            // Inner boundary
            foreach (closed_boundary_store inner_bndry in this.closed_inner_bndries)
            {
                double ib_area = inner_bndry.bndry_area;

                x_center = x_center - (inner_bndry.centroid_x * ib_area);
                y_center = y_center - (inner_bndry.centroid_y * ib_area);

                inner_boundary_area = inner_boundary_area + ib_area;

                // moment of inertia
                x_moi = x_moi - inner_bndry.moi_x;
                y_moi = y_moi - inner_bndry.moi_y;
                xy_moi = xy_moi - inner_bndry.moi_xy;

                // principal moment of inertia
                moi_p1 = moi_p1 - inner_bndry.moi_P1;
                moi_p2 = moi_p2 - inner_bndry.moi_P2;
                moi_theta = moi_theta - inner_bndry.moi_theta;

                // minimum x & y
                this.x_min = this.x_min > inner_bndry.x_min ? inner_bndry.x_min : this.x_min;
                this.y_min = this.y_min > inner_bndry.y_min ? inner_bndry.y_min : this.y_min;
            }

            // save the geometric parameters
            this.surf_area = (outer_boundary_area - inner_boundary_area);
            this.x_centroid = x_center / this.surf_area;
            this.y_centroid = y_center / this.surf_area;

            //  Moment of inertia about the centroid
            this.x_area_moment = x_moi;
            this.y_area_moment = y_moi;
            this.xy_area_moment = xy_moi;

            // Principal moment of inertia
            this.area_moment_p1 = moi_p1;
            this.area_moment_p2 = moi_p2;
            this.area_moment_theta = moi_theta;
        }

        public void set_openTK_objects()
        {
            // Set the closed boundaries openTK 
            this.closed_outer_bndry.set_openTK_objects();

            foreach (closed_boundary_store inner_bndry in this.closed_inner_bndries)
            {
                inner_bndry.set_openTK_objects();
            }
        }

        public void paint_boundaries()
        {
            // Paint the boundaries (outer and inner)
            // Set openTK becore calling this function
            this.closed_outer_bndry.paint_closed_boundary();

            foreach (closed_boundary_store inner_bndry in this.closed_inner_bndries)
            {
                inner_bndry.paint_closed_boundary();
            }
        }
    }
}
