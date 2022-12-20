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
        private main_form parent_form;
        private geometry_store geom;

        public int selected_surf_id { get; private set; }

        public int selected_edge_id { get; private set; }

        public mesh_form(geometry_store main_geom, main_form t_parent_form)
        {
            InitializeComponent();
            form_size_control();

            this.geom = main_geom;
            this.parent_form = t_parent_form;

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
            // Check wether the input element size is bigger than minimum element size
            double inpt_element_length = 0.0;
            Double.TryParse(textBox_elemsize.Text, out inpt_element_length);

            double min_mesh_size = 0.0f;
            int surf_id;
            int selected_index = dataGridView_surface.CurrentCell.RowIndex;

            if (selected_index > -1)
            {
                surf_id = Int32.Parse(dataGridView_surface.Rows[selected_index].Cells[0].Value.ToString());

                // Find the surface with surf id
                foreach (surface_store surf in geom.all_surfaces)
                {
                    if (surf_id == surf.surf_id)
                    {
                        min_mesh_size = surf.mesh_elem_size;
                        break;
                    }
                }
            }
            else
            {
                return;
            }

            // Check the input element length
            if (inpt_element_length > min_mesh_size)
            {
                textBox_elemsize.Text = min_mesh_size.ToString("F4");
                inpt_element_length = min_mesh_size;
            }


            int surf_index = 0;
            foreach (surface_store surf in geom.all_surfaces)
            {
                if (surf_id == surf.surf_id)
                {
                    // Set the new element length
                    geom.all_surfaces.ElementAt(surf_index).set_curve_element_density(inpt_element_length);
                    break;
                }
                surf_index++;
            }

            // Refill the data gridview
            set_edge_datagridview(surf_index);
            this.parent_form.highlight_selected_surface(surf_id);
        }

        private void button_N_Click(object sender, EventArgs e)
        {
            // Decrease element density
            // Update the value in edge datagrid view
            int selected_index = dataGridView_edge.CurrentCell.RowIndex;
            if (selected_index < 0)
            {
                // Exit if nothing is selected
                return;
            }

            int element_density = Int32.Parse(dataGridView_edge.Rows[selected_index].Cells[4].Value.ToString());


            if (dataGridView_edge.Rows[selected_index].Cells[6].Value.ToString() == "line")
            {
                // Selected edge is line (so the lowest can be 1)
                if (element_density != 1)
                {
                    element_density--;
                }
            }
            else
            {
                // Selected edge is line (so the lowest can be 1)
                if (element_density != 2)
                {
                    element_density--;
                }
            }


            // Find the surface with surf id
            int surf_index = 0;

            foreach (surface_store surf in geom.all_surfaces)
            {
                // Select the surface
                if (this.selected_surf_id == surf.surf_id)
                {
                    if (geom.all_surfaces.ElementAt(surf_index).set_curve_element_density(this.selected_edge_id, element_density) == false)
                    {
                        dataGridView_edge.Rows[selected_index].Cells[4].Value = element_density;
                    }
                    break;
                }
                surf_index++;
            }

            this.parent_form.highlight_selected_edge(this.selected_surf_id, this.selected_edge_id);
        }

        private void button_P_Click(object sender, EventArgs e)
        {
            // Increase element density
            // Update the value in edge datagrid view
            int selected_index = dataGridView_edge.CurrentCell.RowIndex;
            if (selected_index < 0)
            {
                // Exit if nothing is selected
                return;
            }

            int element_density = Int32.Parse(dataGridView_edge.Rows[selected_index].Cells[4].Value.ToString());
            element_density++;



            int surf_index = 0;

            // Find the surface with surf id
            foreach (surface_store surf in geom.all_surfaces)
            {
                // Select the surface
                if (this.selected_surf_id == surf.surf_id)
                {
                    if (geom.all_surfaces.ElementAt(surf_index).set_curve_element_density(this.selected_edge_id, element_density) == false)
                    {
                        dataGridView_edge.Rows[selected_index].Cells[4].Value = element_density;
                    }
                    break;
                }
                surf_index++;
            }

            this.parent_form.highlight_selected_edge(this.selected_surf_id, this.selected_edge_id);

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

                foreach (surface_store surf in geom.all_surfaces)
                {
                    if (surf_id == surf.surf_id)
                    {
                        this.selected_surf_id = surf_id;
                        this.parent_form.highlight_selected_surface(surf_id);

                        set_edge_datagridview(surf_index);

                        // Set the minimum curve length 
                        textBox_elemsize.Text = surf.mesh_elem_size.ToString("F4");

                        return;
                    }
                    surf_index++;
                }

            }

        }

        public void set_edge_datagridview(int surf_index)
        {
            dataGridView_edge.Rows.Clear();
            // Fill the edges from the selected surfaces
            // Outter boundary
            foreach (curve_store curves in geom.all_surfaces.ElementAt(surf_index).closed_outer_bndry.boundary_curves)
            {
                // set the outer boundary curves
                fill_edge_datagridview(curves.get_curve_data());
            }

            foreach (closed_boundary_store innr_bndry in geom.all_surfaces.ElementAt(surf_index).closed_inner_bndries)
            {
                // Set the inner boundary curves
                foreach (curve_store curves in innr_bndry.boundary_curves)
                {
                    fill_edge_datagridview(curves.get_curve_data());
                }
            }
        }

        private void dataGridView_edge_SelectionChanged(object sender, EventArgs e)
        {
            // Update selected rows
            int surface_selected_index = dataGridView_surface.CurrentCell.RowIndex;
            int edge_selected_index = dataGridView_edge.CurrentCell.RowIndex;
            if (surface_selected_index > -1 && edge_selected_index > -1)
            {
                int surf_id = Int32.Parse(dataGridView_surface.Rows[surface_selected_index].Cells[0].Value.ToString());
                int edge_id = Int32.Parse(dataGridView_edge.Rows[edge_selected_index].Cells[0].Value.ToString());

                // Find the edge with edge id
                foreach (surface_store surf in geom.all_surfaces)
                {
                    if (surf_id == surf.surf_id)
                    {
                        // Outter boundaries
                        foreach (curve_store outer_curves in surf.closed_outer_bndry.boundary_curves)
                        {
                            if (edge_id == outer_curves.curve_id)
                            {
                                this.selected_edge_id = edge_id;
                                this.parent_form.highlight_selected_edge(surf_id, edge_id);

                                return;
                            }
                        }

                        foreach (closed_boundary_store innr_bndries in surf.closed_inner_bndries)
                        {
                            // Inner boundaries
                            foreach (curve_store inner_curves in innr_bndries.boundary_curves)
                            {
                                if (edge_id == inner_curves.curve_id)
                                {
                                    this.selected_edge_id = edge_id;
                                    this.parent_form.highlight_selected_edge(surf_id, edge_id);

                                    return;
                                }
                            }
                        }
                    }
                }

            }
        }

        private void button_mesh_Click(object sender, EventArgs e)
        {
            // Create mesh --
            // Create an input for the mesh
            string mesh_inpt = "";
            int inner_id;

            // Get the surface index
            int surf_index = 0;
            foreach (surface_store surf in geom.all_surfaces)
            {
                if (this.selected_surf_id == surf.surf_id)
                {
                    break;
                }
                surf_index++;
            }

            // Outter boundary
            mesh_inpt = mesh_inpt + "*** OUTER BOUNDARY ID =" + geom.all_surfaces.ElementAt(surf_index).closed_outer_bndry.closed_bndry_id + Environment.NewLine;
            foreach (curve_store curves in geom.all_surfaces.ElementAt(surf_index).closed_outer_bndry.boundary_curves)
            {
                // get the outer boundary curves end pt data
                mesh_inpt = mesh_inpt + "** ENDPOINT ID =" + curves.curve_end_pts.all_pts.ElementAt(0).pt_id +
                    ", x=" + curves.curve_end_pts.all_pts.ElementAt(0).d_x +
                    ", y=" + curves.curve_end_pts.all_pts.ElementAt(0).d_y +
                    Environment.NewLine;

                // get the outter boundary curves inner pt data
                inner_id = 0;
                foreach (point_store inr_pt in curves.curve_element_density_pts)
                {
                    mesh_inpt = mesh_inpt + "e" + curves.curve_id + "_" + inner_id +
                        ", x=" + inr_pt.d_x +
                        ", y=" + inr_pt.d_y +
                        Environment.NewLine;

                    inner_id++;
                }
            }
            mesh_inpt = mesh_inpt + "END" + Environment.NewLine;


            foreach (closed_boundary_store innr_bndry in geom.all_surfaces.ElementAt(surf_index).closed_inner_bndries)
            {
                // Set the inner boundary curves
                mesh_inpt = mesh_inpt + "*** INNER BOUNDARY ID =" + innr_bndry.closed_bndry_id + Environment.NewLine;
                foreach (curve_store curves in innr_bndry.boundary_curves)
                {
                    // get the inner boundary curves end pt data
                    mesh_inpt = mesh_inpt + "** ENDPOINT ID =" + curves.curve_end_pts.all_pts.ElementAt(0).pt_id +
                        ", x=" + curves.curve_end_pts.all_pts.ElementAt(0).d_x +
                        ", y=" + curves.curve_end_pts.all_pts.ElementAt(0).d_y +
                        Environment.NewLine;

                    // get the inner boundary curves inner pt data
                    inner_id = 0;
                    foreach (point_store inr_pt in curves.curve_element_density_pts)
                    {
                        mesh_inpt = mesh_inpt + "e" + curves.curve_id + "_" + inner_id +
                            ", x=" + inr_pt.d_x +
                            ", y=" + inr_pt.d_y +
                            Environment.NewLine;

                        inner_id++;
                    }
                }
                mesh_inpt = mesh_inpt + "END" + Environment.NewLine;
            }

            // global_variables.gvariables_static.Show_error_Dialog("Mesh output", mesh_inpt);

            constrained_delaunay_triangle_main CDT = new constrained_delaunay_triangle_main(mesh_inpt);

            // Get the mesh
            if (CDT.is_surface_read == true)
            {
                this.parent_form.add_mesh_data(surf_index, CDT.input_surface.mesh_data);
            }


            // global_variables.gvariables_static.Show_error_Dialog("Input to Mesh algorithm", mesh_inpt);
            // Set the mesh edges  is_meshed = true
            for (int i = 0; i < geom.all_surfaces.ElementAt(surf_index).closed_outer_bndry.boundary_curves.Count; i++)
            {
                geom.all_surfaces.ElementAt(surf_index).closed_outer_bndry.boundary_curves.ElementAt(i).is_curve_meshed = true;
            }

            for (int i = 0; i < geom.all_surfaces.ElementAt(surf_index).closed_inner_bndries.Count; i++)
            {
                // Set the inner boundary curves
                for (int j = 0; j < geom.all_surfaces.ElementAt(surf_index).closed_inner_bndries.ElementAt(i).boundary_curves.Count; j++)
                {
                    geom.all_surfaces.ElementAt(surf_index).closed_inner_bndries.ElementAt(i).boundary_curves.ElementAt(j).is_curve_meshed = true;
                }
            }



            set_edge_datagridview(surf_index);
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            // Delete mesh of the surface (if exist)
            // Get the surface index
            int surf_index = 0;
            foreach (surface_store surf in geom.all_surfaces)
            {
                if (this.selected_surf_id == surf.surf_id)
                {
                    break;
                }
                surf_index++;
            }

            this.parent_form.delete_mesh_data(surf_index);

            // Set the mesh edges  is_meshed = true
            for (int i = 0; i < geom.all_surfaces.ElementAt(surf_index).closed_outer_bndry.boundary_curves.Count; i++)
            {
                geom.all_surfaces.ElementAt(surf_index).closed_outer_bndry.boundary_curves.ElementAt(i).is_curve_meshed = false;
            }

            for (int i = 0; i < geom.all_surfaces.ElementAt(surf_index).closed_inner_bndries.Count; i++)
            {
                // Set the inner boundary curves
                for (int j = 0; j < geom.all_surfaces.ElementAt(surf_index).closed_inner_bndries.ElementAt(i).boundary_curves.Count; j++)
                {
                    geom.all_surfaces.ElementAt(surf_index).closed_inner_bndries.ElementAt(i).boundary_curves.ElementAt(j).is_curve_meshed = false;
                }
            }

            set_edge_datagridview(surf_index);
        }

        private void button_keep_Click(object sender, EventArgs e)
        {
            // Do nothing and exit

        }

        private void mesh_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.parent_form.highlight_selected_surface(-1);
            this.parent_form.is_mesh_form_open = false;
        }
    }
}
