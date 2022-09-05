using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store
{
    public class geometry_store
    {
        public HashSet<surface_store> all_surfaces { get; private set; }

        public HashSet<ellipse_store> all_endpoints { get; private set; }

        public Label_list_store all_labels { get; private set; }

        public bool is_geometry_set { get; private set; }

        public geometry_store()
        {
            // Empty constructor
            this.is_geometry_set = false;
        }


        public void add_geometry(HashSet<surface_store> t_all_surfaces, HashSet<ellipse_store> t_all_ellipses,Label_list_store tlabel_list, double d_scale, double d_tx, double d_ty)
        {
            // Add all the surfaces
            this.all_surfaces = new HashSet<surface_store>(t_all_surfaces);
            this.all_endpoints = new HashSet<ellipse_store>(t_all_ellipses);
            all_labels = new Label_list_store();
            all_labels = tlabel_list;
        }

        public void set_openTK_objects()
        {
            // Set the surfaces openTK 
            foreach (surface_store surf in this.all_surfaces)
            {
                surf.set_openTK_objects();
            }

            // Set the surface end points openTK
            foreach (ellipse_store ellipse in this.all_endpoints)
            {
                ellipse.set_openTK_objects();
            }

            // Set the label data
            all_labels.set_openTK_objects();

            this.is_geometry_set = true;
        }

        public void paint_geometry()
        {
            if (this.is_geometry_set == true)
            {
                // Paint the surface boundaries
                foreach (surface_store surf in this.all_surfaces)
                {
                    surf.paint_boundaries();
                }
                // Paint the end points boundaries
                foreach (ellipse_store ellipse in this.all_endpoints)
                {
                    ellipse.paint_ellipse();
                }

            }
        }

        public void paint_label()
        {
            if (this.is_geometry_set == true)
            {
                // Paint the labels for the geometry
                all_labels.paint_labels();
            }
        }
    }
}
