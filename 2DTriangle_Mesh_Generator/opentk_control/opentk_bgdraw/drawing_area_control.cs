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
        private double _geom_bound_width = 1.0;
        private double _geom_bound_height = 1.0;
        private double _bound_scale;

        private int _margin = 40;

        public int drawing_area_width { get { return this._drawing_area_width; } }

        public int drawing_area_height { get { return this._drawing_area_height; } }

        public int max_drawing_area_size { get { return Math.Max(this._drawing_area_width, this._drawing_area_height); } } // Returns the maximum of canvas_width or canvas_height

        public int min_drawing_area_size { get { return Math.Min(this._drawing_area_width, this._drawing_area_height); } } // Returns the minimum of canvas_width or canvas_height

        public int drawing_area_center_x { get { return (int)((this._drawing_area_width - max_drawing_area_size) * 0.5f); } }

        public int drawing_areas_center_y { get { return (int)((this._drawing_area_height - max_drawing_area_size) * 0.5f); } }

        public float bound_scale { get { return (float)this._bound_scale; } } // Normalized minimum canvas size

        public drawing_area_control(int width, int height, double geom_bound_x, double geom_bound_y)
        {
            this._drawing_area_width = width;
            this._drawing_area_height = height;


            // Scale geom bound to 1.8d
            double geom_scale = 1.8d / Math.Abs(Math.Min(geom_bound_x, geom_bound_y));

            this._geom_bound_width = Math.Abs(geom_bound_x * geom_scale);
            this._geom_bound_height = Math.Abs(geom_bound_y * geom_scale);

            // Scale picture bound to 1.8d
            double main_pic_scale = 1.8d / Math.Abs(Math.Max(width, height));

            double scale_drawing_width = width * main_pic_scale;
            double scale_drawing_height = height * main_pic_scale;

            this._bound_scale = Math.Min((scale_drawing_height / this._geom_bound_height),
                        (scale_drawing_width / this._geom_bound_width));
        }

        public PointF get_normalized_screen_pt(int screen_X, int screen_Y, float zm, float transl_x, float transl_y)
        {
            // Used in scaling the translation values for zoom scale operation
            float mid_x = drawing_area_width * 0.5f;
            float mid_y = drawing_area_height * 0.5f;

            float mouse_x = ((float)(screen_X - mid_x) / (float)((min_drawing_area_size - _margin) * 0.5f));
            float mouse_y = -1.0f * ((float)(screen_Y - mid_y) / (float)((min_drawing_area_size - _margin) * 0.5f));

            return (new PointF((float)(mouse_x - (2.0f * transl_x)) / zm, (float)(mouse_y - (2.0f * transl_y)) / zm));
        }


    }
}
