using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace _2DTriangle_Mesh_Generator.opentk_control.opentk_buffer
{
  public  class VertexBufferLayout
    {
        private int _count;
        private int _stride_count;
        private int _stride_size;

        public int count { get { return this._count; } }

        public int stride_size { get { return this._stride_size; } }

        public VertexBufferLayout(int t_count, int t_index_stride)
        {
            this._count = t_count;
            // Only support Float type as of now
            // Normalized is always false !!! (Send the data normalized)
            this._stride_count = t_index_stride;
            this._stride_size = sizeof(float) * t_index_stride;
        }
    }
}
