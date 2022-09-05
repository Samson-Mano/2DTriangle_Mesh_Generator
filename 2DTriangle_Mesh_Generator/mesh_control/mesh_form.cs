using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _2DTriangle_Mesh_Generator.drawing_objects_store;
using _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements;

namespace _2DTriangle_Mesh_Generator.mesh_control
{
    public partial class mesh_form : Form
    {
        private geometry_store geom;

        public mesh_form(geometry_store main_geom)
        {
            InitializeComponent();
            form_size_control();

            this.geom = main_geom;

            // Fill the surface
            foreach (surface_store surf in main_geom.all_surfaces)
            {
                fill_surface_datagridview(surf.get_surface_data());
            }
        }

        private void mesh_form_SizeChanged(object sender, EventArgs e)
        {
            form_size_control();
        }

        private void fill_surface_datagridview(List<string> surf_data)
        {
            // Fill the surface data grid view
            // Surface ID, End pts ID, Boundary ID, Nested boundary ID, Surface meshed
            dataGridView_surface.Rows.Add(surf_data.ToArray());
        }

        private void fill_edge_datagridview(List<string> edge_data)
        {
            // Fill the edge data grid view
            // Edge ID, Start pt id, end pt id, Element density, Edge meshed, Edge type
            dataGridView_edge.Rows.Add(edge_data.ToArray());
        }

        private void form_size_control()
        {
            // Form size 946, 415
            int form_width = (int)(this.Width * 0.5);
            int form_height = this.Height;

            // Surface Data grid view location 12, 12
            dataGridView_surface.Location = new Point(12, 12);
            dataGridView_surface.Size = new Size(form_width - 24, form_height - 200);
            dataGridView_surface.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_surface.MultiSelect = false;

            // Edge data grid view location 469,12
            dataGridView_edge.Location = new Point(form_width, 12);
            dataGridView_edge.Size = new Size(form_width - 24, form_height - 200);
            dataGridView_edge.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_edge.MultiSelect = false;

            // Control 1 
            label1.Location = new Point(dataGridView_surface.Location.X + 24, form_height - 150);
            textBox_elemsize.Location = new Point(dataGridView_surface.Location.X + 124, form_height - 153);
            button_setelemsize.Location = new Point(dataGridView_surface.Location.X + 244, form_height - 159);

            // Control 2
            button_N.Location = new Point(dataGridView_edge.Location.X + 24, form_height - 159);
            label_edgedensity.Location = new Point(dataGridView_edge.Location.X + 124, form_height - 150);
            button_P.Location = new Point(dataGridView_edge.Location.X + 244, form_height - 159);
        }

        private void mesh_form_Load(object sender, EventArgs e)
        {

        }

        private void button_setelemsize_Click(object sender, EventArgs e)
        {

        }

        private void button_N_Click(object sender, EventArgs e)
        {

        }

        private void button_P_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView_surface_SelectionChanged(object sender, EventArgs e)
        {
            // Update selected rows
            int selected_index = dataGridView_surface.CurrentCell.RowIndex;
            if (selected_index > -1)
            {
                int surf_id = Int32.Parse(dataGridView_surface.Rows[selected_index].Cells[0].Value.ToString());

                // Find the surface with surf id
                int surf_index = 0;

               foreach(surface_store surf in geom.all_surfaces)
                {
                    if(surf_id == surf.surf_id)
                    {
                        dataGridView_edge.Rows.Clear();
                        // Fill the edges from the selected surfaces
                        // Outter boundary
                        foreach (curve_store curves in surf.closed_outer_bndry.boundary_curves)
                        {
                            // set the outer boundary curves
                            fill_edge_datagridview(curves.get_curve_data());
                        }

                        
                        foreach(closed_boundary_store innr_bndry in surf.closed_inner_bndries)
                        {
                            // Set the inner boundary curves
                            foreach (curve_store curves in innr_bndry.boundary_curves)
                            {
                                fill_edge_datagridview(curves.get_curve_data());
                            }
                        }
                    }
                    surf_index++;
                }

            }

        }

        private void dataGridView_edge_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void button_mesh_Click(object sender, EventArgs e)
        {

        }

        private void button_delete_Click(object sender, EventArgs e)
        {

        }

        private void button_keep_Click(object sender, EventArgs e)
        {

        }
    }
}
