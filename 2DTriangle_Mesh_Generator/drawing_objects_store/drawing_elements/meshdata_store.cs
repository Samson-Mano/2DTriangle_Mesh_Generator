using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class meshdata_store
    {
        public points_list_store mesh_vertices { get; private set; }

        public lines_list_store mesh_edges { get; private set; }

        public triangle_list_store mesh_tri { get; private set; }

        public triangle_list_store mesh_shrunk_tri { get; private set; }

        public bool is_mesh_exist { get; private set; }


        public meshdata_store(List<mesh_control.mesh_result> i_mesh_result)
        {
            //// get vertices
            //HashSet<mesh_control.constrained_delaunay_triangulation.mesh_store.point_store> vertices = i_mesh_result.all_points;
            //// get edges
            //HashSet<mesh_control.constrained_delaunay_triangulation.mesh_store.edge_store> edges = i_mesh_result.all_edges;
            //// get triangles 
            //HashSet<mesh_control.constrained_delaunay_triangulation.mesh_store.triangle_store> triangles = i_mesh_result.all_triangles;

            // initiate mesh vertices
            this.mesh_vertices = new points_list_store();
            // initiate mesh edges
            this.mesh_edges = new lines_list_store();
            // initiate mesh triangles
            this.mesh_tri = new triangle_list_store();
            // initiate shrunken mesh triangles
            this.mesh_shrunk_tri = new triangle_list_store();


            int pt_index = 0;
            int ed_index = 0;
            int tr_index = 0;

            foreach (mesh_control.mesh_result m_r in i_mesh_result)
            {
                // Add to mesh vertices
                foreach (var vertex in m_r.pts_data.get_all_points())
                {
                    if (vertex.pt_type == 3)
                        continue;

                    this.mesh_vertices.add_point(new point_store(pt_index, vertex.pt_coord.x, vertex.pt_coord.y, m_r.point_color));
                    pt_index++;
                }

                // Add to mesh edges
                foreach (var edge in m_r.edges_data.get_all_edges())
                {
                    point_d start_pt = m_r.pts_data.get_point(edge.start_pt_id).pt_coord;
                    point_d end_pt = m_r.pts_data.get_point(edge.end_pt_id).pt_coord;

                    this.mesh_edges.add_line(ed_index, start_pt.x, start_pt.y, m_r.edge_color,
                        end_pt.x, end_pt.y, m_r.edge_color);
                    ed_index++;
                }

                // Add to mesh triangles & mesh shrunk triangles
                foreach (var triangle in m_r.triangles_data.get_all_triangles())
                {
                    point_d pt1 = m_r.pts_data.get_point(triangle.pt1_id).pt_coord;
                    point_d pt2 = m_r.pts_data.get_point(triangle.pt2_id).pt_coord;
                    point_d pt3 = m_r.pts_data.get_point(triangle.pt3_id).pt_coord;
                    
                    this.mesh_tri.add_triangle(tr_index, pt1.x, pt1.y, m_r.tri_color,
                        pt2.x, pt2.y, m_r.tri_color,
                        pt3.x, pt3.y, m_r.tri_color);

                    double midpt_x = (pt1.x + pt2.x + pt3.x) / 3.0f;
                    double midpt_y = (pt1.y + pt2.y + pt3.y) / 3.0f;
                    double shrink_factor = global_variables.gvariables_static.triangle_shrink_factor;

                    this.mesh_shrunk_tri.add_triangle(tr_index,
                        (midpt_x * (1 - shrink_factor) + (pt1.x * shrink_factor)),
                        (midpt_y * (1 - shrink_factor) + (pt1.y * shrink_factor)), m_r.tri_color,
                         (midpt_x * (1 - shrink_factor) + (pt2.x * shrink_factor)),
                        (midpt_y * (1 - shrink_factor) + (pt2.y * shrink_factor)), m_r.tri_color,
                         (midpt_x * (1 - shrink_factor) + (pt3.x * shrink_factor)),
                        (midpt_y * (1 - shrink_factor) + (pt3.y * shrink_factor)), m_r.tri_color);


                    tr_index++;
                }

                
                

            }
          

            update_scale(global_variables.gvariables_static.drawing_scale,
            -global_variables.gvariables_static.drawing_tx,
            -global_variables.gvariables_static.drawing_ty);
        }

        public void update_scale(double d_scale, double tran_tx, double tran_ty)
        {
            // update the scale of the mesh
            this.mesh_vertices.update_scale(d_scale, tran_tx, tran_ty);
            this.mesh_edges.update_scale(d_scale, tran_tx, tran_ty);
            this.mesh_tri.update_scale(d_scale, tran_tx, tran_ty);
            this.mesh_shrunk_tri.update_scale(d_scale, tran_tx, tran_ty);   
        }

        public void set_openTK_objects()
        {
            this.mesh_vertices.set_openTK_objects();
            this.mesh_edges.set_openTK_objects();
            this.mesh_tri.set_openTK_objects();
            this.mesh_shrunk_tri.set_openTK_objects();
        }

        public void paint_mesh()
        {


            if (global_variables.gvariables_static.is_paint_shrunk_triangle == true)
            {
                this.mesh_shrunk_tri.paint_all_triangles();
            }
            else
            {
                this.mesh_tri.paint_all_triangles();
            }

            this.mesh_edges.paint_all_lines();

            
            this.mesh_vertices.paint_all_points();


          

            // this.mesh_tri.paint_all_triangles();
        }

    }
}
