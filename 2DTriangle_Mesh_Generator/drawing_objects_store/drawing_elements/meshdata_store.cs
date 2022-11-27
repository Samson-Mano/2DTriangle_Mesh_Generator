using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class meshdata_store
    {
        public points_list_store mesh_vertices { get; private set; }

        public lines_list_store mesh_edges { get; private set; }

        public triangle_list_store mesh_tri { get; private set; }

        public bool is_mesh_exist { get; private set; }


        public meshdata_store(mesh_control.mesh_result i_mesh_result)
        {
            // get vertices
            HashSet<mesh_control.constrained_delaunay_triangulation.mesh_store.point_store> vertices = i_mesh_result.all_points;
            // get edges
            HashSet<mesh_control.constrained_delaunay_triangulation.mesh_store.edge_store> edges = i_mesh_result.all_edges;
            // get triangles 
            HashSet<mesh_control.constrained_delaunay_triangulation.mesh_store.triangle_store> triangles = i_mesh_result.all_triangles;

            // Add to mesh vertices
            this.mesh_vertices = new points_list_store();
            int m_index = 0;
            foreach (var vertex in vertices)
            {
                this.mesh_vertices.add_point(new point_store(m_index, vertex.x, vertex.y, Color.Blue));
                m_index++;
            }

            // Add to mesh edges
            this.mesh_edges = new lines_list_store();
            m_index = 0;
            foreach (var edge in edges)
            {
                this.mesh_edges.add_line(m_index, edge.start_pt.x, edge.start_pt.y, Color.Blue,
                    edge.end_pt.x, edge.end_pt.y, Color.Blue);
                m_index++;
            }

            // Add to mesh triangles
            this.mesh_tri = new triangle_list_store();
            m_index = 0;
            foreach (var triangle in triangles)
            {
                this.mesh_tri.add_triangle(m_index, triangle.pt1.x, triangle.pt1.y, Color.Cyan,
                    triangle.pt2.x, triangle.pt2.y, Color.Cyan,
                    triangle.pt3.x, triangle.pt3.y, Color.Cyan);
                m_index++;
            }

            update_scale(global_variables.gvariables_static.drawing_scale ,
            -global_variables.gvariables_static.drawing_tx,
            -global_variables.gvariables_static.drawing_ty);
        }

        public void update_scale(double d_scale, double tran_tx, double tran_ty)
        {
            // update the scale of the mesh
            this.mesh_vertices.update_scale(d_scale, tran_tx, tran_ty);
            this.mesh_edges.update_scale(d_scale, tran_tx, tran_ty);
            this.mesh_tri.update_scale(d_scale, tran_tx, tran_ty);
        }

        public void set_openTK_objects()
        {
            this.mesh_vertices.set_openTK_objects();
            this.mesh_edges.set_openTK_objects();
            this.mesh_tri.set_openTK_objects();

        }

        public void paint_mesh()
        {
            this.mesh_vertices.paint_all_points();
            this.mesh_edges.paint_all_lines();
           // this.mesh_tri.paint_all_triangles();
        }

    }
}
