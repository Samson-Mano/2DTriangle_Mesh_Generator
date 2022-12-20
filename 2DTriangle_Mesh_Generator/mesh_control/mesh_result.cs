using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation;

namespace _2DTriangle_Mesh_Generator.mesh_control
{
    public class mesh_result
    {
        public int mesh_surf_id { get; private set; }

        public point_list_store pts_data { get; private set; }
        public edge_list_store edges_data { get; private set; }
        public triangle_list_store triangles_data { get; private set; }

        public Color tri_color { get; private set; }

        public Color edge_color { get;  private set; }

        public Color point_color { get; private set; }

        public mesh_result(int m_surf_id,point_list_store i_pts_data,
            edge_list_store i_edges_data,
            triangle_list_store i_triangles_data)
        {
            this.mesh_surf_id = m_surf_id;
            this.pts_data = i_pts_data;
            this.edges_data = i_edges_data;
            this.triangles_data = i_triangles_data;

            // Assign random color for mesh
            Random r = new Random();
            int color_code = r.Next(0, 69);
            edge_color = global_variables.gvariables_static.standard_colors[color_code];

            tri_color = Color.FromArgb(50, edge_color);

            point_color = Color.FromArgb(100, edge_color);

        }

        //public void add_mesh()
        //{
        //    // Add points
        //    foreach (constrained_delaunay_triangulation.mesh_store.point_store pt in all_points)
        //    {
        //        this.all_points.Add(pt);
        //    }

        //    // Add edges
        //    foreach (constrained_delaunay_triangulation.mesh_store.edge_store ed in all_edges)
        //    {
        //        this.all_edges.Add(ed);
        //    }

        //    // Add triangles
        //    foreach (constrained_delaunay_triangulation.mesh_store.triangle_store tr in all_triangles)
        //    {
        //        this.all_triangles.Add(tr);
        //    }

        //}


        public void remove_mesh(point_list_store i_pts_data,
            edge_list_store i_edges_data,
            triangle_list_store i_triangles_data)
        {
            // Remove points
            foreach (point_store pt in i_pts_data.get_all_points())
            {
                this.pts_data.remove_point(pt.pt_id);
            }

            // Remove edges
            foreach (edge_store ed in i_edges_data.get_all_edges())
            {
                this.edges_data.remove_edge(ed.edge_id);
            }

            // Remove triangles
            foreach (triangle_store tr in i_triangles_data.get_all_triangles())
            {
                this.triangles_data.remove_triangle(tr.tri_id);
            }

        }

    }
}
