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

        public double geom_bound_width { get; private set; }

        public double geom_bound_height { get; private set; }

        public geometry_store()
        {
            // Empty constructor
            this.is_geometry_set = false;
        }

        public void add_geometry(HashSet<surface_store> t_all_surfaces, HashSet<ellipse_store> t_all_ellipses, Label_list_store tlabel_list, double d_scale, double d_tx, double d_ty)
        {
            // Add all the surfaces
            this.all_surfaces = new HashSet<surface_store>(t_all_surfaces);
            this.all_endpoints = new HashSet<ellipse_store>(t_all_ellipses);
            all_labels = new Label_list_store();
            all_labels = tlabel_list;

            // Fit the boundary
            fit_the_boundary();
        }

        private void fit_the_boundary()
        {
            double x_min, x_max, y_min, y_max;

            x_min = Double.MaxValue;
            x_max = Double.MinValue;
            y_min = Double.MaxValue;
            y_max = Double.MinValue;

            foreach (surface_store surf in this.all_surfaces)
            {
                x_min = x_min > surf.x_min ? surf.x_min : x_min;
                x_max = x_max < surf.x_max ? surf.x_max : x_max;
                y_min = y_min > surf.y_min ? surf.y_min : y_min;
                y_max = y_max < surf.y_max ? surf.y_max : y_max;
            }

            this.geom_bound_width = x_max - x_min;
            this.geom_bound_height = y_max - y_min;

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


        public void set_surface_highlight_openTK_objects(int surf_id)
        {
            // ReSet the surfaces openTK 
            foreach (surface_store surf in this.all_surfaces)
            {
                surf.set_openTK_objects();
            }

            // Set the highlight surface openTK
            foreach (surface_store surf in this.all_surfaces)
            {
                if (surf.surf_id == surf_id)
                {
                    surf.set_highlight_openTK_objects();
                    return;
                }
            }
        }

        public void paint_highlight_surface(int surf_id)
        {
            // Highlight the selected surface
            foreach (surface_store surf in this.all_surfaces)
            {
                if (surf.surf_id == surf_id)
                {
                    surf.paint_highlight_boundaries();
                    return;
                }
            }
        }


        public void set_edge_highlight_openTK_objects(int surf_id, int edge_id)
        {
            //foreach (surface_store surf in this.all_surfaces)
            //{
            //        // Is the selected edge outer boundary curve
            //        // ReSet the edges openTK 
            //        foreach (curve_store curv in surf.closed_outer_bndry.boundary_curves)
            //        {
            //            curv.set_openTK_objects();
            //        }

            //        // Is the selected edge is part of the inner boundary curves
            //        foreach (closed_boundary_store innr_bndry in surf.closed_inner_bndries)
            //        {
            //            // ReSet the edges openTK 
            //            foreach (curve_store curv in innr_bndry.boundary_curves)
            //            {
            //                curv.set_openTK_objects();
            //            }
            //        }
            //}

            // Find the selected surface
            int surf_index = 0;
            foreach (surface_store surf in this.all_surfaces)
            {
                if (surf.surf_id == surf_id)
                {
                    // Is the selected edge outer boundary curve
                    // Set the highlight edge openTK 
                    foreach (curve_store curv in this.all_surfaces.ElementAt(surf_id).closed_outer_bndry.boundary_curves)
                    {
                        if(curv.curve_id == edge_id)
                        {
                            curv.set_highlight_openTK_objects(true);
                            return;
                        }
                    }

                    // Is the selected edge is part of the inner boundary curves
                    foreach (closed_boundary_store innr_bndry in this.all_surfaces.ElementAt(surf_id).closed_inner_bndries)
                    {
                        // Set the highlight edge openTK 
                        foreach (curve_store curv in innr_bndry.boundary_curves)
                        {
                            if(curv.curve_id == edge_id)
                            {
                                curv.set_highlight_openTK_objects(true);
                                return;
                            }
                        }
                    }
                }
                surf_index++;
            }
        }

        public void paint_highlight_edge(int surf_id,int edge_id)
        {
            // Find the selected surface
            int surf_index = 0;
            foreach (surface_store surf in this.all_surfaces)
            {
                if (surf.surf_id == surf_id)
                {
                    // Is the selected edge outer boundary curve
                    // Highlight the selected edge
                    foreach (curve_store curv in this.all_surfaces.ElementAt(surf_id).closed_outer_bndry.boundary_curves)
                    {
                        if (curv.curve_id == edge_id)
                        {
                            curv.paint_highlight_curve();
                            return;
                        }
                    }

                    // Is the selected edge is part of the inner boundary curves
                    foreach(closed_boundary_store innr_bndry in this.all_surfaces.ElementAt(surf_id).closed_inner_bndries)
                    {
                        // Highlight the selected edge
                        foreach (curve_store curv in innr_bndry.boundary_curves)
                        {
                            if (curv.curve_id == edge_id)
                            {
                                curv.paint_highlight_curve();
                                return;
                            }
                        }
                    }
                }
                surf_index++;
            }
        }

        public void paint_highlight_edge_label(int surf_id, int edge_id)
        {
            // Find the selected surface
            int surf_index = 0;
            foreach (surface_store surf in this.all_surfaces)
            {
                if (surf.surf_id == surf_id)
                {
                    // Is the selected edge outer boundary curve
                    // Highlight the selected edge
                    foreach (curve_store curv in this.all_surfaces.ElementAt(surf_id).closed_outer_bndry.boundary_curves)
                    {
                        if (curv.curve_id == edge_id)
                        {
                            curv.paint_highlight_curve_label();
                            return;
                        }
                    }

                    // Is the selected edge is part of the inner boundary curves
                    foreach (closed_boundary_store innr_bndry in this.all_surfaces.ElementAt(surf_id).closed_inner_bndries)
                    {
                        // Highlight the selected edge
                        foreach (curve_store curv in innr_bndry.boundary_curves)
                        {
                            if (curv.curve_id == edge_id)
                            {
                                curv.paint_highlight_curve_label();
                                return;
                            }
                        }
                    }
                }
                surf_index++;
            }
        }
    }
}
