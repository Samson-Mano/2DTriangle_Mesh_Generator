using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation
{
    public class point_list_store
    {
        private Dictionary<int, point_store> all_points;
        private HashSet<int> unique_pointid_list;


        public point_list_store()
        {
            // Empty constructor
            this.all_points = new Dictionary<int, point_store>();
            this.unique_pointid_list = new HashSet<int>();
        }

        public int add_point(point_d i_pt,int i_pt_type)
        {
            // Add point
            int point_id = get_unique_point_id();

            this.all_points.Add(point_id, new point_store(point_id, i_pt.x, i_pt.y, i_pt_type));

            return point_id;
        }

        public void remove_point(int pt_id)
        {
            // Remove point
            unique_pointid_list.Add(pt_id);
            this.all_points.Remove(pt_id);
        }

        private int get_unique_point_id()
        {
            int point_id;
            // get an unique edge id
            if (unique_pointid_list.Count != 0)
            {
                point_id = unique_pointid_list.First(); // retrive the edge id from the list which stores the id of deleted edges
                unique_pointid_list.Remove(point_id); // remove that id from the unique edge id list
            }
            else
            {
                point_id = this.all_points.Count;
            }
            return point_id;
        }

        public int get_boundary_point_id(point_d pt_coord)
        {
            // Returns only the boundary points (pt_type 1 & 2)
            foreach(point_store pt in get_all_points())
            {
                // Skip the inner points
                if (pt.pt_type == 3)
                    continue;
                
                // If point type is 1 or 2 then check the coordinate match
                if(pt.Equals(pt_coord)==true)
                {
                    return pt.pt_id;
                }
            }

            // none found (this should never trigger)
            return -1;
        }

        public List<point_store> get_all_points()
        {
            return this.all_points.Values.ToList();
        }

        public point_store get_point(int pt_id)
        {
            point_store pt;
            if (this.all_points.TryGetValue(pt_id, out pt) == true)
            {
                return pt;
            }
            return null;
        }
    }
}
