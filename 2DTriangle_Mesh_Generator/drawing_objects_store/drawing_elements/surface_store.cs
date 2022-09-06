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

        public double x_max { get; private set; }

        public double y_max { get; private set; }

        public double x_centroid { get; private set; }

        public double y_centroid { get; private set; }

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

        public void update_scale(double d_scale, double tran_tx, double tran_ty)
        {
            // called once when the surface is added
            // update the outter boundary scale & trans tx, ty
            this.closed_outer_bndry.update_scale(d_scale, tran_tx, tran_ty);

            // update the inner boundary scale & trans tx, ty
            for (int i = 0; i < this.closed_inner_bndries.Count; i++)
            {
                this.closed_inner_bndries.ElementAt(i).update_scale(d_scale, tran_tx, tran_ty);
            }
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

            this.x_min = Double.MaxValue;
            this.x_max = Double.MinValue;
            this.y_min = Double.MaxValue;
            this.y_max = Double.MinValue;

            // maximum, minimum x & y
            this.x_min = this.x_min > this.closed_outer_bndry.x_min ? this.closed_outer_bndry.x_min : this.x_min;
            this.x_max = this.x_max < this.closed_outer_bndry.x_max ? this.closed_outer_bndry.x_max : this.x_max;
            this.y_min = this.y_min > this.closed_outer_bndry.y_min ? this.closed_outer_bndry.y_min : this.y_min;
            this.y_max = this.y_max < this.closed_outer_bndry.y_max ? this.closed_outer_bndry.y_max : this.y_max;

            double inner_boundary_area = 0.0;

            // Inner boundary
            foreach (closed_boundary_store inner_bndry in this.closed_inner_bndries)
            {
                double ib_area = inner_bndry.bndry_area;

                x_center = x_center - (inner_bndry.centroid_x * ib_area);
                y_center = y_center - (inner_bndry.centroid_y * ib_area);

                inner_boundary_area = inner_boundary_area + ib_area;

                // maximum, minimum x & y (No need exactly because inner boundary doesnot for extremities)
                this.x_min = this.x_min > inner_bndry.x_min ? inner_bndry.x_min : this.x_min;
                this.x_max = this.x_max < inner_bndry.x_max ? inner_bndry.x_max : this.x_max;
                this.y_min = this.y_min > inner_bndry.y_min ? inner_bndry.y_min : this.y_min;
                this.y_max = this.y_max < inner_bndry.y_max ? inner_bndry.y_max : this.y_max;
            }

            // save the geometric parameters
            this.surf_area = (outer_boundary_area - inner_boundary_area);
            this.x_centroid = x_center / this.surf_area;
            this.y_centroid = y_center / this.surf_area;
        }

        public List<string> get_surface_data()
        {
            List<string> surface_data = new List<string>();

            // Surface ID
            surface_data.Add(surf_id.ToString());
            //  // Surface ID, End pts ID, Boundary ID, Nested boundary ID, Surface meshed

            // End PT IDs
            surface_data.Add(this.closed_outer_bndry.str_end_pt_ids);

            // Boundary IDs
            surface_data.Add(this.closed_outer_bndry.str_boundary_curve_ids);

            // Nested boundary ID
            string inner_surf_boundarycurve_ids = "";
            foreach (closed_boundary_store bndry in this.closed_inner_bndries)
            {
                inner_surf_boundarycurve_ids = inner_surf_boundarycurve_ids + "[" + bndry.str_boundary_curve_ids + "] ";
            }
            surface_data.Add(inner_surf_boundarycurve_ids);

            // Surface meshed
            surface_data.Add("false");

            return surface_data;
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

        public void set_highlight_openTK_objects()
        {
            // Set the closed boundaries openTK 
            this.closed_outer_bndry.set_highlight_openTK_objects();

            foreach (closed_boundary_store inner_bndry in this.closed_inner_bndries)
            {
                inner_bndry.set_highlight_openTK_objects();
            }
        }

        public void paint_highlight_boundaries()
        {
            // Paint the boundaries (outer and inner)
            // Set openTK becore calling this function
            this.closed_outer_bndry.paint_highlight_closed_boundary();

            foreach (closed_boundary_store inner_bndry in this.closed_inner_bndries)
            {
                inner_bndry.paint_highlight_closed_boundary();
            }
        }

    }
}
