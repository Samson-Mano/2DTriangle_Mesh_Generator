namespace _2DTriangle_Mesh_Generator.mesh_control
{
    partial class mesh_form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mesh_form));
            this.button_mesh = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_keep = new System.Windows.Forms.Button();
            this.dataGridView_surface = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_elemsize = new System.Windows.Forms.TextBox();
            this.dataGridView_edge = new System.Windows.Forms.DataGridView();
            this.button_setelemsize = new System.Windows.Forms.Button();
            this.label_edgedensity = new System.Windows.Forms.Label();
            this.button_P = new System.Windows.Forms.Button();
            this.button_N = new System.Windows.Forms.Button();
            this.Column_edgeID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_startpt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_endID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_elemdensity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_edgemeshed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_edgetype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_surfaceID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_endptid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_boundaryid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_nested = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_meshexist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_surface)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_edge)).BeginInit();
            this.SuspendLayout();
            // 
            // button_mesh
            // 
            this.button_mesh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_mesh.Location = new System.Drawing.Point(429, 320);
            this.button_mesh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_mesh.Name = "button_mesh";
            this.button_mesh.Size = new System.Drawing.Size(133, 38);
            this.button_mesh.TabIndex = 0;
            this.button_mesh.Text = "Create Mesh";
            this.button_mesh.UseVisualStyleBackColor = true;
            this.button_mesh.Click += new System.EventHandler(this.button_mesh_Click);
            // 
            // button_delete
            // 
            this.button_delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_delete.Location = new System.Drawing.Point(599, 320);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(133, 37);
            this.button_delete.TabIndex = 1;
            this.button_delete.Text = "Reject Mesh";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // button_keep
            // 
            this.button_keep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_keep.Location = new System.Drawing.Point(775, 320);
            this.button_keep.Name = "button_keep";
            this.button_keep.Size = new System.Drawing.Size(133, 37);
            this.button_keep.TabIndex = 2;
            this.button_keep.Text = "Keep Mesh";
            this.button_keep.UseVisualStyleBackColor = true;
            this.button_keep.Click += new System.EventHandler(this.button_keep_Click);
            // 
            // dataGridView_surface
            // 
            this.dataGridView_surface.AllowUserToAddRows = false;
            this.dataGridView_surface.AllowUserToDeleteRows = false;
            this.dataGridView_surface.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_surface.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_surfaceID,
            this.Column_endptid,
            this.Column_boundaryid,
            this.Column_nested,
            this.Column_meshexist});
            this.dataGridView_surface.Location = new System.Drawing.Point(12, 12);
            this.dataGridView_surface.Name = "dataGridView_surface";
            this.dataGridView_surface.ReadOnly = true;
            this.dataGridView_surface.RowHeadersWidth = 51;
            this.dataGridView_surface.RowTemplate.Height = 24;
            this.dataGridView_surface.Size = new System.Drawing.Size(439, 221);
            this.dataGridView_surface.TabIndex = 3;
            this.dataGridView_surface.SelectionChanged += new System.EventHandler(this.dataGridView_surface_SelectionChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 262);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Element size :";
            // 
            // textBox_elemsize
            // 
            this.textBox_elemsize.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBox_elemsize.Location = new System.Drawing.Point(156, 259);
            this.textBox_elemsize.Name = "textBox_elemsize";
            this.textBox_elemsize.Size = new System.Drawing.Size(100, 24);
            this.textBox_elemsize.TabIndex = 5;
            // 
            // dataGridView_edge
            // 
            this.dataGridView_edge.AllowUserToAddRows = false;
            this.dataGridView_edge.AllowUserToDeleteRows = false;
            this.dataGridView_edge.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_edge.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_edgeID,
            this.Column_startpt,
            this.Column_endID,
            this.Column_elemdensity,
            this.Column_edgemeshed,
            this.Column_edgetype});
            this.dataGridView_edge.Location = new System.Drawing.Point(469, 13);
            this.dataGridView_edge.Name = "dataGridView_edge";
            this.dataGridView_edge.ReadOnly = true;
            this.dataGridView_edge.RowHeadersWidth = 51;
            this.dataGridView_edge.RowTemplate.Height = 24;
            this.dataGridView_edge.Size = new System.Drawing.Size(447, 220);
            this.dataGridView_edge.TabIndex = 6;
            this.dataGridView_edge.SelectionChanged += new System.EventHandler(this.dataGridView_edge_SelectionChanged);
            // 
            // button_setelemsize
            // 
            this.button_setelemsize.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_setelemsize.Location = new System.Drawing.Point(287, 253);
            this.button_setelemsize.Name = "button_setelemsize";
            this.button_setelemsize.Size = new System.Drawing.Size(137, 37);
            this.button_setelemsize.TabIndex = 7;
            this.button_setelemsize.Text = "Set Element Size";
            this.button_setelemsize.UseVisualStyleBackColor = true;
            this.button_setelemsize.Click += new System.EventHandler(this.button_setelemsize_Click);
            // 
            // label_edgedensity
            // 
            this.label_edgedensity.AutoSize = true;
            this.label_edgedensity.Location = new System.Drawing.Point(596, 262);
            this.label_edgedensity.Name = "label_edgedensity";
            this.label_edgedensity.Size = new System.Drawing.Size(95, 18);
            this.label_edgedensity.TabIndex = 8;
            this.label_edgedensity.Text = "Edge Density";
            // 
            // button_P
            // 
            this.button_P.Location = new System.Drawing.Point(734, 253);
            this.button_P.Name = "button_P";
            this.button_P.Size = new System.Drawing.Size(75, 37);
            this.button_P.TabIndex = 9;
            this.button_P.Text = "+";
            this.button_P.UseVisualStyleBackColor = true;
            this.button_P.Click += new System.EventHandler(this.button_P_Click);
            // 
            // button_N
            // 
            this.button_N.Location = new System.Drawing.Point(499, 253);
            this.button_N.Name = "button_N";
            this.button_N.Size = new System.Drawing.Size(75, 37);
            this.button_N.TabIndex = 10;
            this.button_N.Text = "-";
            this.button_N.UseVisualStyleBackColor = true;
            this.button_N.Click += new System.EventHandler(this.button_N_Click);
            // 
            // Column_edgeID
            // 
            this.Column_edgeID.HeaderText = "Edge ID";
            this.Column_edgeID.MinimumWidth = 6;
            this.Column_edgeID.Name = "Column_edgeID";
            this.Column_edgeID.ReadOnly = true;
            this.Column_edgeID.Width = 75;
            // 
            // Column_startpt
            // 
            this.Column_startpt.HeaderText = "Start ID";
            this.Column_startpt.MinimumWidth = 6;
            this.Column_startpt.Name = "Column_startpt";
            this.Column_startpt.ReadOnly = true;
            this.Column_startpt.Width = 75;
            // 
            // Column_endID
            // 
            this.Column_endID.HeaderText = "End ID";
            this.Column_endID.MinimumWidth = 6;
            this.Column_endID.Name = "Column_endID";
            this.Column_endID.ReadOnly = true;
            this.Column_endID.Width = 75;
            // 
            // Column_elemdensity
            // 
            this.Column_elemdensity.HeaderText = "Element Density";
            this.Column_elemdensity.MinimumWidth = 6;
            this.Column_elemdensity.Name = "Column_elemdensity";
            this.Column_elemdensity.ReadOnly = true;
            this.Column_elemdensity.Width = 75;
            // 
            // Column_edgemeshed
            // 
            this.Column_edgemeshed.HeaderText = "Edge Meshed";
            this.Column_edgemeshed.MinimumWidth = 6;
            this.Column_edgemeshed.Name = "Column_edgemeshed";
            this.Column_edgemeshed.ReadOnly = true;
            this.Column_edgemeshed.Width = 75;
            // 
            // Column_edgetype
            // 
            this.Column_edgetype.HeaderText = "Edge Type";
            this.Column_edgetype.MinimumWidth = 6;
            this.Column_edgetype.Name = "Column_edgetype";
            this.Column_edgetype.ReadOnly = true;
            this.Column_edgetype.Width = 125;
            // 
            // Column_surfaceID
            // 
            this.Column_surfaceID.HeaderText = "Surface ID";
            this.Column_surfaceID.MinimumWidth = 6;
            this.Column_surfaceID.Name = "Column_surfaceID";
            this.Column_surfaceID.ReadOnly = true;
            this.Column_surfaceID.Width = 50;
            // 
            // Column_endptid
            // 
            this.Column_endptid.HeaderText = "Endpt ID";
            this.Column_endptid.MinimumWidth = 6;
            this.Column_endptid.Name = "Column_endptid";
            this.Column_endptid.ReadOnly = true;
            // 
            // Column_boundaryid
            // 
            this.Column_boundaryid.HeaderText = "Boundary ID";
            this.Column_boundaryid.MinimumWidth = 6;
            this.Column_boundaryid.Name = "Column_boundaryid";
            this.Column_boundaryid.ReadOnly = true;
            // 
            // Column_nested
            // 
            this.Column_nested.HeaderText = "Nested Boundary ID";
            this.Column_nested.MinimumWidth = 6;
            this.Column_nested.Name = "Column_nested";
            this.Column_nested.ReadOnly = true;
            // 
            // Column_meshexist
            // 
            this.Column_meshexist.HeaderText = "Mesh Exist";
            this.Column_meshexist.MinimumWidth = 6;
            this.Column_meshexist.Name = "Column_meshexist";
            this.Column_meshexist.ReadOnly = true;
            this.Column_meshexist.Width = 75;
            // 
            // mesh_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 368);
            this.Controls.Add(this.button_N);
            this.Controls.Add(this.button_P);
            this.Controls.Add(this.label_edgedensity);
            this.Controls.Add(this.button_setelemsize);
            this.Controls.Add(this.dataGridView_edge);
            this.Controls.Add(this.textBox_elemsize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView_surface);
            this.Controls.Add(this.button_keep);
            this.Controls.Add(this.button_delete);
            this.Controls.Add(this.button_mesh);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(946, 415);
            this.Name = "mesh_form";
            this.Text = "Mesh control";
            this.Load += new System.EventHandler(this.mesh_form_Load);
            this.SizeChanged += new System.EventHandler(this.mesh_form_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_surface)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_edge)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_mesh;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Button button_keep;
        private System.Windows.Forms.DataGridView dataGridView_surface;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_elemsize;
        private System.Windows.Forms.DataGridView dataGridView_edge;
        private System.Windows.Forms.Button button_setelemsize;
        private System.Windows.Forms.Label label_edgedensity;
        private System.Windows.Forms.Button button_P;
        private System.Windows.Forms.Button button_N;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_edgeID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_startpt;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_endID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_elemdensity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_edgemeshed;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_edgetype;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_surfaceID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_endptid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_boundaryid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_nested;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_meshexist;
    }
}