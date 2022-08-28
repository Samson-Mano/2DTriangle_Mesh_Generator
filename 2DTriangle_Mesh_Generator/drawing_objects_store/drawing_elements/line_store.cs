using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public class line_store
    {
        public int ln_id { get; private set; }

        public point_store start_pt { get; private set; }

        public point_store end_pt { get; private set; }

        public line_store(int t_ln_id, point_store t_start_pt, point_store t_end_pt)
        {
            // Main constructor
            this.ln_id = t_ln_id;
            this.start_pt = t_start_pt;
            this.end_pt = t_end_pt;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as line_store);
        }

        public bool Equals(line_store other_line)
        {
            // Check 1 (Line ids should not match)
            if (this.Equals(other_line.ln_id) == true)
            {
                return true;
            }

            // Check 2 (Whether line end points match)
            if ((this.start_pt.Equals(other_line.start_pt) && this.end_pt.Equals(other_line.end_pt)) ||
                (this.start_pt.Equals(other_line.end_pt) && this.end_pt.Equals(other_line.start_pt)))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.start_pt.pt_id, this.end_pt.pt_id);
        }
    }
}
