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
using _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation;

namespace _2DTriangle_Mesh_Generator
{
    public partial class main_form : Form
    {
        public geometry_store geom_obj { get; private set; }

        // Variables to control openTK GLControl
        // glControl wrapper class
        private opentk_main_control g_control;

        private bool is_model_loaded = false;
        public bool is_mesh_form_open = false;

        mesh_control.mesh_form mesh_form1;

        // Mesh result store
        private List<mesh_control.mesh_result> mesh_result_store = new List<mesh_control.mesh_result>();

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
                    geom_obj.add_geometry(surf_conv.all_surface, surf_conv.all_ellipses, surf_conv.all_labels, surf_conv.dr_scale, surf_conv.dr_tx, surf_conv.dr_ty);
                    geom_obj.set_openTK_objects();

                    is_model_loaded = true;

                    // Update the size of the drawing area
                    g_control.update_drawing_area_size(glControl_main_panel.Width,
                        glControl_main_panel.Height, geom_obj.geom_bound_width, geom_obj.geom_bound_height);
                    // (Ctrl + F) --> Zoom to fit
                    g_control.zoom_to_fit(ref glControl_main_panel);

                    toolStripStatusLabel_zoom_value.Text = "Zoom: " + (gvariables_static.RoundOff((int)(1.0f * 100))).ToString() + "%";

                    // Clear previous mesh
                    this.mesh_result_store = new List<mesh_control.mesh_result>();

                    glControl_main_panel.Invalidate();
                }
            }
        }

        private void createMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create mesh
            if (is_model_loaded == true)
            {
                mesh_form1 = new mesh_control.mesh_form(geom_obj, this);
                mesh_form1.ShowInTaskbar = false;
                mesh_form1.StartPosition = FormStartPosition.CenterParent;
                mesh_form1.Opacity = 0.9;

                this.is_mesh_form_open = true;
                mesh_form1.ShowDialog();
            }

        }

        private void exportMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Renumber all the Nodes
            int renum_iterator = 0;
            int i, j;
            int prev_id;
            bool is_exist;
            Dictionary<int, int>[] renum_nodes = new Dictionary<int, int>[mesh_result_store.Count];
            Dictionary<int, int>[] renum_edges = new Dictionary<int, int>[mesh_result_store.Count];
            Dictionary<int, int>[] renum_tris = new Dictionary<int, int>[mesh_result_store.Count];

            for (i = 0; i < mesh_result_store.Count; i++)
            {
                renum_nodes[i] = new Dictionary<int, int>();
                // Nodes
                foreach (mesh_control.delaunay_triangulation.point_store pts in mesh_result_store[i].pts_data.get_all_points())
                {
                    // Check whether the point already exists
                    is_exist = false;
                    prev_id = -1;
                    for (j = i - 1; j >= 0; j--)
                    {
                        // Get the point list from previous mesh point data
                        List<point_store> previous_mesh_pts = mesh_result_store[j].pts_data.get_all_points();
                        point_store existing_node = previous_mesh_pts.Find(obj => obj.pt_coord.Equals(pts.pt_coord));
                        if (existing_node != null)
                        {
                            prev_id = renum_nodes[j][existing_node.pt_id];
                            is_exist = true;
                            break;
                        }
                    }

                    if (is_exist == true)
                    {
                        // The point already exists from previous mesh so, use its new id
                        renum_nodes[i].Add(pts.pt_id, prev_id);
                    }
                    else
                    {
                        // Link the pt unique id to the new id which spans through all mesh
                        renum_nodes[i].Add(pts.pt_id, renum_iterator);
                        renum_iterator++;
                    }
                }
            }

            renum_iterator = 0;

            for (i = 0; i < mesh_result_store.Count; i++)
            {
                renum_edges[i] = new Dictionary<int, int>();
                // Edges
                foreach (mesh_control.delaunay_triangulation.edge_store edg in mesh_result_store[i].edges_data.get_all_edges())
                {
                    // Check whether the edge already exists
                    is_exist = false;
                    prev_id = -1;
                    for (j = i - 1; j >= 0; j--)
                    {
                        // Get the point list from previous mesh point data
                        List<edge_store> previous_mesh_edges = mesh_result_store[j].edges_data.get_all_edges();
                        // Current edges (new start & end pt id)
                        int n_start_pt_id = renum_nodes[i][edg.start_pt_id];
                        int n_end_pt_id = renum_nodes[i][edg.end_pt_id];

                        edge_store existing_edge = previous_mesh_edges.Find(obj =>
                        (renum_nodes[j][obj.start_pt_id] == n_start_pt_id && renum_nodes[j][obj.end_pt_id] == n_end_pt_id) ||
                        (renum_nodes[j][obj.start_pt_id] == n_end_pt_id && renum_nodes[j][obj.end_pt_id] == n_start_pt_id));

                        if (existing_edge != null)
                        {
                            prev_id = renum_edges[j][existing_edge.edge_id];
                            is_exist = true;
                            break;
                        }
                    }

                    if (is_exist == true)
                    {
                        // The point already exists from previous mesh so, use its new id
                        renum_edges[i].Add(edg.edge_id, prev_id);
                    }
                    else
                    {
                        // Link the pt unique id to the new id which spans through all mesh
                        renum_edges[i].Add(edg.edge_id, renum_iterator);
                        renum_iterator++;
                    }
                }
            }

            renum_iterator = 0;
            string t_tri_sets = "TRIANGLE SETS " + Environment.NewLine;

            for (i = 0; i < mesh_result_store.Count; i++)
            {
                t_tri_sets = t_tri_sets + "SET " + (i + 1) + Environment.NewLine;
                renum_tris[i] = new Dictionary<int, int>();
                // Triangles
                foreach (mesh_control.delaunay_triangulation.triangle_store tri in mesh_result_store[i].triangles_data.get_all_triangles())
                {
                    // Link the pt unique id to the new id which spans through all mesh
                    renum_tris[i].Add(tri.tri_id, renum_iterator);
                    t_tri_sets = t_tri_sets + renum_iterator + ", ";
                    renum_iterator++;
                }

                // Remove the last two characters
                t_tri_sets = t_tri_sets.Remove(t_tri_sets.Length - 2, 2) + Environment.NewLine;
            }


            i = 0;
            // Export the mesh as *.txt format
            string temp_str = "";
            int temp_id;
            Dictionary<int, string> point_id_str = new Dictionary<int, string>();
            Dictionary<int, string> edge_id_str = new Dictionary<int, string>();
            Dictionary<int, string> tri_id_str = new Dictionary<int, string>();
            // Nodes set
            string n_corner_nodes_str = "CORNER NODES  " + Environment.NewLine;
            string n_edge_nodes_str = "EDGE NODES  " + Environment.NewLine;
            string e_bndry_edge_str = "BOUNDARY EDGES  " + Environment.NewLine;

            foreach (mesh_control.mesh_result mesh_result in mesh_result_store)
            {
                // Points
                foreach (mesh_control.delaunay_triangulation.point_store pts in mesh_result.pts_data.get_all_points())
                {
                    temp_id = renum_nodes[i][pts.pt_id];
                    temp_str = temp_id + ","
                        + pts.pt_coord.x + ","
                        + pts.pt_coord.y;

                    // Add only if the node is not already present
                    if (point_id_str.ContainsKey(temp_id) != true)
                    {
                        point_id_str.Add(temp_id, temp_str);

                        // Add to corner and edge node list
                        if (pts.pt_type == 1)
                        {
                            // corner nodes
                            n_corner_nodes_str = n_corner_nodes_str + temp_id + ", ";
                        }

                        if (pts.pt_type == 2)
                        {
                            // Edge nodes
                            n_edge_nodes_str = n_edge_nodes_str + temp_id + ", ";
                        }

                    }
                }
                // Remove the last two characters
                n_corner_nodes_str = n_corner_nodes_str.Remove(n_corner_nodes_str.Length - 2, 2) + Environment.NewLine;
                n_edge_nodes_str = n_edge_nodes_str.Remove(n_edge_nodes_str.Length - 2, 2) + Environment.NewLine;

                // Edges
                foreach (mesh_control.delaunay_triangulation.edge_store eds in mesh_result.edges_data.get_all_edges())
                {
                    temp_id = renum_edges[i][eds.edge_id];
                    temp_str = temp_id + ","
                        + renum_nodes[i][eds.start_pt_id] + ","
                        + renum_nodes[i][eds.end_pt_id];

                    // Add only if the edge is not already present
                    if (edge_id_str.ContainsKey(temp_id) != true)
                    {
                        edge_id_str.Add(temp_id, temp_str);

                        // Add to the boundary edges store
                        if (eds.is_boundary_edge == true)
                        {
                            e_bndry_edge_str = e_bndry_edge_str + temp_id + ", ";
                        }
                    }
                }
                // Remove the last two characters
                e_bndry_edge_str = e_bndry_edge_str.Remove(e_bndry_edge_str.Length - 2, 2) + Environment.NewLine;

                // Triangles
                foreach (mesh_control.delaunay_triangulation.triangle_store tri in mesh_result.triangles_data.get_all_triangles())
                {
                    temp_id = renum_tris[i][tri.tri_id];
                    temp_str = temp_id + ","
                        + renum_nodes[i][tri.pt1_id] + ","
                        + renum_nodes[i][tri.pt2_id] + ","
                        + renum_nodes[i][tri.pt3_id];

                    // Add only if the triangle is not already present
                    if (tri_id_str.ContainsKey(temp_id) != true)
                    {
                        tri_id_str.Add(temp_id, temp_str);
                    }
                }
                i++;
            }

            // Results string
            string points_str = "NODES " + Environment.NewLine;
            string edges_str = "EDGES " + Environment.NewLine;
            string triangles_str = "TRIANGLES " + Environment.NewLine;

            foreach (var r_str in point_id_str.OrderBy(obj => obj.Key))
            {
                points_str = points_str + r_str.Value + Environment.NewLine;
            }

            foreach (var r_str in edge_id_str.OrderBy(obj => obj.Key))
            {
                edges_str = edges_str + r_str.Value + Environment.NewLine;
            }

            foreach (var r_str in tri_id_str.OrderBy(obj => obj.Key))
            {
                triangles_str = triangles_str + r_str.Value + Environment.NewLine;
            }



            string rslt_str = "____________________________________ Mesh data ________________________________" + Environment.NewLine;
            rslt_str = rslt_str + "--- Generated from 2D Traingle Mesh generator developed by Samson Mano ----" + Environment.NewLine;
            rslt_str = rslt_str + "________________________________________________________________________________" + Environment.NewLine;
            rslt_str = rslt_str + points_str + "END" + Environment.NewLine;
            rslt_str = rslt_str + "____________________________________________________________________________" + Environment.NewLine;
            rslt_str = rslt_str + edges_str + "END" + Environment.NewLine;
            rslt_str = rslt_str + "____________________________________________________________________________" + Environment.NewLine;
            rslt_str = rslt_str + triangles_str + "END" + Environment.NewLine;
            rslt_str = rslt_str + "____________________________________________________________________________" + Environment.NewLine;
            rslt_str = rslt_str + n_corner_nodes_str + "END" + Environment.NewLine;
            rslt_str = rslt_str + "____________________________________________________________________________" + Environment.NewLine;
            rslt_str = rslt_str + n_edge_nodes_str + "END" + Environment.NewLine;
            rslt_str = rslt_str + "____________________________________________________________________________" + Environment.NewLine;
            rslt_str = rslt_str + e_bndry_edge_str + "END" + Environment.NewLine;
            rslt_str = rslt_str + "____________________________________________________________________________" + Environment.NewLine;
            rslt_str = rslt_str + t_tri_sets + "END" + Environment.NewLine;
            rslt_str = rslt_str + "____________________________________________________________________________" + Environment.NewLine;


            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "mesh1.txt";
            save.Filter = "Text File | *.txt";

            if (save.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(save.OpenFile());

                writer.WriteLine(rslt_str);

                writer.Dispose();
                writer.Close();
            }


            //  global_variables.gvariables_static.Show_error_Dialog("Result", rslt_str);

        }

        public void highlight_selected_surface(int surf_id)
        {
            // Surface selection changed from mesh form
            geom_obj.set_surface_highlight_openTK_objects(surf_id);

            // Refresh the controller
            glControl_main_panel.Invalidate();
        }

        public void highlight_selected_edge(int surf_id, int edge_id)
        {
            // Edge selection changed from mesh form
            geom_obj.set_edge_highlight_openTK_objects(surf_id, edge_id);

            // Refresh the controller
            glControl_main_panel.Invalidate();
        }

        public void add_mesh_data(int surf_id, mesh_store i_mesh_data)
        {
            // Store the mesh data from mesh form

            this.mesh_result_store.Add(new mesh_control.mesh_result(surf_id, i_mesh_data.result_pts_data,
i_mesh_data.result_edge_data,
i_mesh_data.result_tri_data));

            // Add mesh data
            geom_obj.implement_mesh(this.mesh_result_store);

            // Set mesh openTK objects
            geom_obj.set_openTK_mesh_objects();

            // Refresh the controller
            glControl_main_panel.Invalidate();
        }

        public void delete_mesh_data(int surf_id)
        {
            // Store the mesh data from mesh form
            int remove_index;
            remove_index = this.mesh_result_store.FindIndex(obj => obj.mesh_surf_id == surf_id);

            this.mesh_result_store.RemoveAt(remove_index);

            // Add mesh data
            geom_obj.implement_mesh(this.mesh_result_store);

            // Set mesh openTK objects
            geom_obj.set_openTK_mesh_objects();

            // Refresh the controller
            glControl_main_panel.Invalidate();
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
                glControl_main_panel.Height, 2.0, 2.0);

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
            g_control.update_shader_uniform_var(1, Color.Green);
            geom_obj.paint_label();

            if (is_mesh_form_open == true)
            {
                g_control.set_opengl_shader(0);

                GL.LineWidth(3.30f);
                // Surface selection changed from mesh form
                geom_obj.paint_highlight_surface(mesh_form1.selected_surf_id);

                // Edge selection changed from mesh form
                geom_obj.paint_highlight_edge(mesh_form1.selected_surf_id, mesh_form1.selected_edge_id);

                g_control.set_opengl_shader(1);
                g_control.update_shader_uniform_var(1, Color.Red);
                geom_obj.paint_highlight_edge_label(mesh_form1.selected_surf_id, mesh_form1.selected_edge_id);
            }


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
