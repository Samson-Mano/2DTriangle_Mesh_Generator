using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
// This app class structure
using _2DTriangle_Mesh_Generator.opentk_control;
using _2DTriangle_Mesh_Generator.global_variables;
using _2DTriangle_Mesh_Generator.txt_input_reader;
using _2DTriangle_Mesh_Generator.drawing_objects_store;


namespace _2DTriangle_Mesh_Generator
{
    public partial class main_form : Form
    {
        public geometry_store geom_obj { get; private set; }

        // Variables to control openTK GLControl
        // glControl wrapper class
        private opentk_main_control g_control;

        private bool is_model_loaded = false;

        mesh_control.mesh_form mesh_form1;

        // Cursor point on the GLControl
        private PointF click_pt;

        public main_form()
        {
            InitializeComponent();
        }

        private void main_form_Load(object sender, EventArgs e)
        {
            // Initialize the geometry object
            geom_obj = new geometry_store();

            // Load the wrapper class to control the openTK glcontrol
            g_control = new opentk_main_control();

            // Fill the gcontol panel
            glControl_main_panel.Dock = DockStyle.Fill;
        }

        #region "Menu events"
        private void importGeometryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Import Geometry
            
            OpenFileDialog ow = new OpenFileDialog();
            ow.DefaultExt = "*.txt";
            ow.Filter = "Samson Mano's Varai2D raw data - txt Files (*.txt)|*.txt";
            ow.ShowDialog();

            if (File.Exists(ow.FileName) == true)
            {
                txt_rd_reader txt_rd = new txt_rd_reader(ow.FileName);
                txt_to_surface_conversion surf_conv = new txt_to_surface_conversion(txt_rd);


                if (surf_conv.all_surface.Count != 0)
                {
                    // Re-initialize the geometry
                    geom_obj = new geometry_store();
                    geom_obj.add_geometry(surf_conv.all_surface, surf_conv.all_ellipses,surf_conv.all_labels,surf_conv.dr_scale,surf_conv.dr_tx,surf_conv.dr_ty);
                    geom_obj.set_openTK_objects();

                    is_model_loaded = true;

                    // Update the size of the drawing area
                    g_control.update_drawing_area_size(glControl_main_panel.Width,
                        glControl_main_panel.Height, geom_obj.geom_bound_width, geom_obj.geom_bound_height);
                    // (Ctrl + F) --> Zoom to fit
                    g_control.zoom_to_fit(ref glControl_main_panel);

                    toolStripStatusLabel_zoom_value.Text = "Zoom: " + (gvariables_static.RoundOff((int)(1.0f * 100))).ToString() + "%";

                    glControl_main_panel.Invalidate();
                }
            }
        }


        private void createMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create mesh
            if (is_model_loaded == true)
            {
                mesh_form1 = new mesh_control.mesh_form(geom_obj);
                mesh_form1.ShowInTaskbar = false;
                mesh_form1.StartPosition = FormStartPosition.CenterParent;
                mesh_form1.Opacity = 0.9;

                mesh_form1.ShowDialog();
            }

        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Exit application
            this.Close();
        }
        #endregion

        #region "glControl Main Panel Events"
        private void glControl_main_panel_Load(object sender, EventArgs e)
        {
            // Load the wrapper class to control the openTK glcontrol
            g_control = new opentk_main_control();

            // Update the size of the drawing area
            g_control.update_drawing_area_size(glControl_main_panel.Width,
                glControl_main_panel.Height,2.0,2.0);

            // Refresh the controller (doesnt do much.. nothing to draw)
            glControl_main_panel.Invalidate();
        }

        private void glControl_main_panel_Paint(object sender, PaintEventArgs e)
        {
            // Paint the drawing area (glControl_main)
            // Tell OpenGL to use MyGLControl
            glControl_main_panel.MakeCurrent();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Paint the background color
            g_control.set_opengl_shader(0);
            g_control.paint_opengl_control_background();

            // Display the model using OpenGL
            GL.LineWidth(2.437f);
            geom_obj.paint_geometry();

            // Display the label
            g_control.set_opengl_shader(1);
            geom_obj.paint_label();


            // OpenTK windows are what's known as "double-buffered". In essence, the window manages two buffers.
            // One is rendered to while the other is currently displayed by the window.
            // This avoids screen tearing, a visual artifact that can happen if the buffer is modified while being displayed.
            // After drawing, call this function to swap the buffers. If you don't, it won't display what you've rendered.
            glControl_main_panel.SwapBuffers();
        }

        private void glControl_main_panel_SizeChanged(object sender, EventArgs e)
        {
            if (g_control == null)
                return;
            // glControl size changed
            // Update the size of the drawing area
            g_control.update_drawing_area_size(glControl_main_panel.Width,
                glControl_main_panel.Height, geom_obj.geom_bound_width, geom_obj.geom_bound_height);

            toolStripStatusLabel_zoom_value.Text = "Zoom: " + (gvariables_static.RoundOff((int)(1.0f * 100))).ToString() + "%";

            // Refresh the painting area
            glControl_main_panel.Invalidate();
        }

        private void glControl_main_panel_MouseEnter(object sender, EventArgs e)
        {
            // set the focus to enable zoom/ pan & zoom to fit
            glControl_main_panel.Focus();
        }

        private void glControl_main_panel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Pan operation starts if Ctrl + Mouse Right Button is pressed
            if (gvariables_static.Is_cntrldown == true && e.Button == MouseButtons.Right)
            {
                // save the current cursor point
                click_pt = new PointF(e.X, e.Y);
                // Set the variable to indicate pan operation begins
                gvariables_static.Is_panflg = true;
            }
        }

        private void glControl_main_panel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // User press hold Cntrl key and mouse wheel
            if (gvariables_static.Is_cntrldown == true)
            {
                // Zoom operation commences
                glControl_main_panel.Focus();

                g_control.intelli_zoom_operation(e.Delta, e.X, e.Y);

                // Update the zoom value in tool strip status bar
                toolStripStatusLabel_zoom_value.Text = "Zoom: " + (gvariables_static.RoundOff((int)(100f * g_control._zoom_val))).ToString() + "%";
                // Refresh the painting area
                glControl_main_panel.Refresh();
            }
        }

        private void glControl_main_panel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (gvariables_static.Is_panflg == true)
            {
                g_control.pan_operation(e.X - click_pt.X, e.Y - click_pt.Y);

                // Refresh the painting area
                glControl_main_panel.Refresh();
            }
        }

        private void glControl_main_panel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Pan operation ends once the Mouse Right Button is released
            if (gvariables_static.Is_panflg == true)
            {
                gvariables_static.Is_panflg = false;

                // Pan operation ends (save the translate transformation)
                g_control.pan_operation_complete();

                // Refresh the painting area
                glControl_main_panel.Invalidate();
            }
        }

        private void glControl_main_panel_KeyDown(object sender, KeyEventArgs e)
        {
            // Keydown event
            if (e.Control == true)
            {
                // User press and hold Cntrl key
                gvariables_static.Is_cntrldown = true;

                if (e.KeyCode == Keys.F)
                {
                    // (Ctrl + F) --> Zoom to fit
                    g_control.zoom_to_fit(ref glControl_main_panel);

                    toolStripStatusLabel_zoom_value.Text = "Zoom: " + (gvariables_static.RoundOff((int)(1.0f * 100))).ToString() + "%";
                    toolStripStatusLabel_zoom_value.Invalidate();
                }
            }
        }

        private void glControl_main_panel_KeyUp(object sender, KeyEventArgs e)
        {
            // Keyup event
            gvariables_static.Is_cntrldown = false;
        }
        #endregion

    }
}
