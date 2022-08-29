using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _2DTriangle_Mesh_Generator.opentk_control.opentk_bgdraw
{
   public class drawing_area_control
    {
        private int _drawing_area_width;
        private int _drawing_area_height;
        private int _margin = 40;

        public int drawing_area_width { get { return this._drawing_area_width; } }

        public int drawing_area_height { get { return this._drawing_area_height; } }

        public int max_drawing_area_size { get { return Math.Max(this._drawing_area_width, this._drawing_area_height); } } // Returns the maximum of canvas_width or canvas_height

        public int min_drawing_area_size { get { return Math.Min(this._drawing_area_width, this._drawing_area_height); } } // Returns the minimum of canvas_width or canvas_height

        public int drawing_area_center_x { get { return (int)((this._drawing_area_width - max_drawing_area_size) * 0.5f); } }

        public int drawing_areas_center_y { get { return (int)((this._drawing_area_height - max_drawing_area_size) * 0.5f); } }

        public float norm_drawing_area_min { get { return ((float)(min_drawing_area_size - _margin)) / (float)max_drawing_area_size; } } // Normalized minimum canvas size

        public float drawing_area_x_norm_max { get { return (float)this._drawing_area_width / (float)max_drawing_area_size; } } // Normalized canvas width (as seen in screen)

        public float drawing_area_y_norm_max { get { return (float)this._drawing_area_height / (float)max_drawing_area_size; } } // Normalized canvas height (as seen in screen)

        public drawing_area_control( int width, int height)
        {
            this._drawing_area_width = width;
            this._drawing_area_height = height;
        }

        public PointF get_normalized_screen_pt(int screen_X, int screen_Y, float zm, float transl_x, float transl_y)
        {
            // Used in scaling the translation values for zoom scale operation
            float mid_x = drawing_area_width * 0.5f;
            float mid_y = drawing_area_height * 0.5f;

            float mouse_x = ((float)(screen_X - mid_x) / (float)((min_drawing_area_size-_margin) * 0.5f));
            float mouse_y = -1.0f * ((float)(screen_Y - mid_y) / (float)((min_drawing_area_size -_margin) * 0.5f));

            return (new PointF((float)(mouse_x - (2.0f * transl_x)) / zm, (float)(mouse_y - (2.0f * transl_y)) / zm));
        }


    }
}
