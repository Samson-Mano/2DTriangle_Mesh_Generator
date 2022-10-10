using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
// This app class structure
using _2DTriangle_Mesh_Generator.opentk_control.opentk_bgdraw;

namespace _2DTriangle_Mesh_Generator.opentk_control.shader_compiler
{
    public class Shader_Support
    {
        // scale to control the units of drawing area
        private float _boundary_scale = 1.0f;

        // zoom scale
        public float _zm_scale = 1.0f;

        // Translation details
        private Vector3 _current_translation = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 _previous_translation = new Vector3(0.0f, 0.0f, 0.0f);

        public Vector3 previous_translation { get { return this._previous_translation; } }

        // Origin translation
        private float origin_transl_x = 0.0f;
        private float origin_transl_y = 0.0f;

        // Temporary variables to initiate the zoom to fit animation
        private GLControl this_Gcntrl;
        private float param_t = 0.0f;
        private float temp_zm = 1.0f;
        private float temp_tx, temp_ty;

        private Shader this_shader;
        private System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        private drawing_area_control _drawing_area_details = new drawing_area_control( 500, 500,0,0);

        public drawing_area_control drawing_area_details { get { return this._drawing_area_details; } }

        public Shader_Support()
        {
            // Empty constructor
        }

        #region "Zoom and Pan operation of openGL control"
        public void update_primary_scale(Shader s_shader, int width, int height,double bound_x, double bound_y)
        {
            // Assign the shader
            this_shader = s_shader;

            // Update the drawing area
            this._drawing_area_details = new drawing_area_control(width, height,bound_x,bound_y);

            // Primary scale
            this._boundary_scale = this._drawing_area_details.bound_scale;
            global_variables.gvariables_static.boundary_scale = this._boundary_scale;

            scale_Transform(1.0f);
        }

        public void intelli_zoom(Shader s_shader, double e_Delta, int e_X, int e_Y)
        {
            // Assign the shader
            this_shader = s_shader;

            // Get the screen pt before scaling
            PointF screen_pt_b4_scale = this._drawing_area_details.get_normalized_screen_pt(e_X, e_Y, this._zm_scale, this.previous_translation.X, this.previous_translation.Y);

            if (e_Delta > 0)
            {
                if (this._zm_scale < 1000)
                {
                    this._zm_scale = this._zm_scale + 0.1f;
                }
            }
            else if (e_Delta < 0)
            {
                if (this._zm_scale > 0.101)
                {
                    this._zm_scale = this._zm_scale - 0.1f;
                }
            }

            // Get the screen pt after scaling
            PointF screen_pt_a4_scale = this._drawing_area_details.get_normalized_screen_pt(e_X, e_Y, this._zm_scale, this.previous_translation.X, this.previous_translation.Y);

            float tx = (-1.0f) * this._zm_scale * 0.5f * (screen_pt_b4_scale.X - screen_pt_a4_scale.X);
            float ty = (-1.0f) * this._zm_scale * 0.5f * (screen_pt_b4_scale.Y - screen_pt_a4_scale.Y);

            // Scale the view with intellizoom (translate and scale)
            scale_intelli_zoom_Transform(this._zm_scale, tx, ty);
        }

        public void pan_operation(Shader s_shader, float et_X, float et_Y)
        {
            // Assign the shader
            this_shader = s_shader;

            // Pan operation is in progress
            float tx = (float)((float)et_X / (float)(this._drawing_area_details.max_drawing_area_size * 0.5f));
            float ty = (float)((float)et_Y / (float)(this._drawing_area_details.max_drawing_area_size * 0.5f));

            // Translate the drawing area
            translate_Transform(tx, -1.0f * ty);
        }

        public void zoom_to_fit_operation(Shader s_shader, ref GLControl t_this_Gcntrl)
        {
            // Assign the shader
            this_shader = s_shader;
            this_Gcntrl = t_this_Gcntrl;

            // Save the current zoom and translation values to temporary variables (for the animation)
            param_t = 0.0f;
            temp_zm = this._zm_scale;
            temp_tx = previous_translation.X - this.origin_transl_x;
            temp_ty = previous_translation.Y - this.origin_transl_y;

            myTimer = new System.Windows.Forms.Timer();
            myTimer.Enabled = true;
            myTimer.Tick += new EventHandler(TimerEventProcessor);
            myTimer.Interval = 10;
            myTimer.Start();
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            param_t = param_t + 0.05f;

            if (param_t > 1.0f)
            {
                // Set the zoom value to 1.0f
                scale_intelli_zoom_Transform(1.0f, -previous_translation.X+this.origin_transl_x, -previous_translation.Y + this.origin_transl_y);

                // Refresh the painting area
                this_Gcntrl.Invalidate();

                // End the animation
                myTimer.Stop();
                return;
            }
            else
            {
                // Animate the translation & zoom value
                float anim_zm = temp_zm * (1 - param_t) + (1.0f * param_t);
                float anim_tx = ((0.0f) * (1 - param_t) - (temp_tx * param_t));
                float anim_ty = ((0.0f) * (1 - param_t) - (temp_ty * param_t));

                // Scale transformation intermediate
                scale_Transform(anim_zm);

                // Translate transformation intermediate
                translate_Transform(anim_tx, anim_ty);

                // Refresh the painting area
                this_Gcntrl.Invalidate();
            }
        }

        private void scale_intelli_zoom_Transform(float zm, float tx, float ty)
        {
            //update the scale
            scale_Transform(zm);
            // update the translation
            translate_Transform(tx, ty);

            save_translate_transform();
        }

        private void scale_Transform(float zm)
        {
            this._zm_scale = zm;

            //update the scale
            this_shader.SetFloat("gScale", this._zm_scale * this._boundary_scale);
        }

        private void translate_Transform(float trans_x, float trans_y)
        {
            // 2D Translatoin
            _current_translation = new Vector3(trans_x  + this._previous_translation.X,
                                                trans_y + this._previous_translation.Y,
                                                    0.0f + this._previous_translation.Z);

            Matrix4 current_transformation = new Matrix4(1.0f, 0.0f, 0.0f, _current_translation.X,
                0.0f, 1.0f, 0.0f, _current_translation.Y,
                0.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);

            this_shader.SetMatrix4("gTranslation", current_transformation);
        }

        public void save_translate_transform()
        {
            // save the final translation
            _previous_translation = _current_translation;
        }

        public void set_txtshader_color(Color t_clr)
        {
            Vector4 txt_clr = new Vector4(t_clr.R,t_clr.G,t_clr.B,1.0f);
            this_shader.SetVector4("u_Color", txt_clr);
        }
        #endregion
    }
}
