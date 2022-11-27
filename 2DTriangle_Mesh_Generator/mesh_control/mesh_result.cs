using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control
{
    public class mesh_result
    {
        public HashSet<constrained_delaunay_triangulation.mesh_store.point_store> all_points { get; private set; }
        public HashSet<constrained_delaunay_triangulation.mesh_store.edge_store> all_edges { get; private set; }
        public HashSet<constrained_delaunay_triangulation.mesh_store.triangle_store> all_triangles { get; private set; }

        public mesh_result()
        {
            this.all_points = new HashSet<constrained_delaunay_triangulation.mesh_store.point_store>();
            this.all_edges = new HashSet<constrained_delaunay_triangulation.mesh_store.edge_store>();
            this.all_triangles = new HashSet<constrained_delaunay_triangulation.mesh_store.triangle_store>();
        }

        public void add_mesh(HashSet<constrained_delaunay_triangulation.mesh_store.point_store> all_points,
            HashSet<constrained_delaunay_triangulation.mesh_store.edge_store> all_edges, 
            HashSet<constrained_delaunay_triangulation.mesh_store.triangle_store> all_triangles)
        {
            // Add points
            foreach(constrained_delaunay_triangulation.mesh_store.point_store pt in all_points)
            {
                this.all_points.Add(pt);
            }

            // Add edges
            foreach(constrained_delaunay_triangulation.mesh_store.edge_store ed in all_edges)
            {
                this.all_edges.Add(ed);
            }

            // Add triangles
            foreach(constrained_delaunay_triangulation.mesh_store.triangle_store tr in all_triangles)
            {
                this.all_triangles.Add(tr);
            }

        }


        public void remove_mesh(HashSet<constrained_delaunay_triangulation.mesh_store.point_store> all_points,
            HashSet<constrained_delaunay_triangulation.mesh_store.edge_store> all_edges,
            HashSet<constrained_delaunay_triangulation.mesh_store.triangle_store> all_triangles)
        {
            // Remove points
            foreach (constrained_delaunay_triangulation.mesh_store.point_store pt in all_points)
            {
                this.all_points.Remove(pt);
            }

            // Remove edges
            foreach (constrained_delaunay_triangulation.mesh_store.edge_store ed in all_edges)
            {
                this.all_edges.Remove(ed);
            }

            // Remove triangles
            foreach (constrained_delaunay_triangulation.mesh_store.triangle_store tr in all_triangles)
            {
                this.all_triangles.Remove(tr);
            }

        }

    }
}
