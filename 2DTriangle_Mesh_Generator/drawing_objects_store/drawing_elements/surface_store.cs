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
            for(int i = 0; i < this.closed_inner_bndries.Count;i++)
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

                // minimum x & y
                this.x_min = this.x_min > inner_bndry.x_min ? inner_bndry.x_min : this.x_min;
                this.y_min = this.y_min > inner_bndry.y_min ? inner_bndry.y_min : this.y_min;
            }

            // save the geometric parameters
            this.surf_area = (outer_boundary_area - inner_boundary_area);
            this.x_centroid = x_center / this.surf_area;
            this.y_centroid = y_center / this.surf_area;
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
