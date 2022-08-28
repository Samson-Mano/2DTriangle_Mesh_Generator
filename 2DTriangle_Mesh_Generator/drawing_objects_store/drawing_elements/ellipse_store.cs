using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class ellipse_store
    {
        public int ellipse_id { get; private set; }

        public double d_x { get; private set; }

        public double d_y { get; private set; }

        public Color ellipse_clr { get; private set; }

        public double ellipse_raidus { get; private set; }

        private triangle_list_store ellipse_segments = new triangle_list_store();

        private int segment_count = 30;

        public ellipse_store(int t_ellipse_id, double t_x, double t_y, Color pt_clr, double t_radius)
        {
            // Main constructor
            this.ellipse_id = t_ellipse_id;
            this.d_x = t_x;
            this.d_y = t_y;
            this.ellipse_clr = pt_clr;
            this.ellipse_raidus = t_radius;

            // Set the ellipse region as triangles
            set_ellipse_segments();
        }

        private void set_ellipse_segments()
        {
            ellipse_segments = new triangle_list_store();

            double origin_x = this.d_x;
            double origin_y = this.d_y;

            // Pt0
            double pt_0_x = this.d_x + this.ellipse_raidus * Math.Cos(0.0d);
            double pt_0_y = this.d_y + this.ellipse_raidus * Math.Sin(0.0d);

            double pt_1_x, pt_1_y;
            double angle_rad;


            for (int i = 1; i <= segment_count; i++)
            {
                angle_rad = i * (360.0d / segment_count) * (Math.PI / 180.0d);

                // Pt1
                pt_1_x = this.d_x + this.ellipse_raidus * Math.Cos(angle_rad);
                pt_1_y = this.d_y + this.ellipse_raidus * Math.Sin(angle_rad);

                ellipse_segments.add_triangle((i - 1),
                    pt_0_x, pt_0_y, this.ellipse_clr,
                    pt_1_x, pt_1_y, this.ellipse_clr,
                    origin_x, origin_y, this.ellipse_clr);

                pt_0_x = pt_1_x;
                pt_0_y = pt_1_y;
            }
        }

        public void set_openTK_objects()
        {
            // Set openTK
            ellipse_segments.set_openTK_objects();
        }

        public void paint_ellipse()
        {
            // Paint the ellipse
            ellipse_segments.paint_all_triangles();
        }

    }
}
