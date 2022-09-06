using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

using SharpFont;
// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
// This app class structure
using _2DTriangle_Mesh_Generator.opentk_control.opentk_buffer;
using _2DTriangle_Mesh_Generator.Resources;

namespace _2DTriangle_Mesh_Generator.drawing_objects_store.drawing_elements
{
    public struct Character
    {
        public int TextureID { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Bearing { get; set; }
        public int Advance { get; set; }
    }

    public class character_as_texture
    {
        private char char0;
        private double pt_x;
        private double pt_y;

        private float font_size;

        public Character ch { get; private set; }

        private float[] get_char_vertex_coords()
        {
            float[] vertex_coord = new float[24];
            // Add vertex to list
            vertex_coord[0] = (float)pt_x;
            vertex_coord[1] = (float)pt_y;
            // Texture coordinate (bottom left) 
            vertex_coord[2] = 0.0f;
            vertex_coord[3] = 0.0f;

            vertex_coord[4] = (float)pt_x;
            vertex_coord[5] = (float)pt_y - font_size;
            // Texture coordinate (bottom right) 
            vertex_coord[6] = 0.0f;
            vertex_coord[7] = 1.0f;

            vertex_coord[8] = (float)(pt_x + font_size - 0.01);
            vertex_coord[9] = (float)pt_y;
            // Texture coordinate (Top right) 
            vertex_coord[10] = 1.0f;
            vertex_coord[11] = 0.0f;

            ///////////////////////////////////////////
            vertex_coord[12] = (float)pt_x;
            vertex_coord[13] = (float)pt_y - font_size;
            // Texture coordinate (bottom left)  
            vertex_coord[14] = 0.0f;
            vertex_coord[15] = 1.0f;

            vertex_coord[16] = (float)(pt_x + font_size-0.01);
            vertex_coord[17] = (float)pt_y - font_size;
            // Texture coordinate (bottom left)  
            vertex_coord[18] = 1.0f;
            vertex_coord[19] = 1.0f;

            vertex_coord[20] = (float)(pt_x + font_size - 0.01);
            vertex_coord[21] = (float)pt_y;
            // Texture coordinate (bottom left)  
            vertex_coord[22] = 1.0f;
            vertex_coord[23] = 0.0f;

            return vertex_coord;
        }

        public float[] get_char_vertices()
        {
            // Return the point in openGL format
            // return get_label_vertex_coords().Concat(get_label_vertex_color()).ToArray(); ;
            return get_char_vertex_coords();
        }

        public character_as_texture(char t_ch, double t_pt_x, double t_pt_y, float t_font_size)
        {
            this.char0 = t_ch;
            this.pt_x = t_pt_x + 0.005;
            this.pt_y = t_pt_y - 0.005;
            this.font_size = t_font_size;
        }

        public void set_openTK_objects(Dictionary<uint, Character> t_characters)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            if (t_characters.ContainsKey(this.char0) == false)
                return;
            ch = t_characters[this.char0];
        }


        public void paint_character(int i_index)
        {
            // Render glyph texture over quad
            GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Render quad
            GL.DrawArrays(PrimitiveType.Triangles, i_index, 6);
        }

    }



    public class Label_data
    {
        private float font_size = 0.04f;

        public List<character_as_texture> label_char = new List<character_as_texture>();

        public int label_id;
        public double l_x;
        public double l_y;

        public double pt_paint_x;
        public double pt_paint_y;

        public string label_val;

        public Color _label_clr;

        public Label_data(int tlabel_id, string tlabel, double tl_x, double tl_y, double d_scale, double tran_tx, double tran_ty, Color tlabel_clr)
        {
            label_id = tlabel_id;
            l_x = tl_x;
            l_y = tl_y;
            label_val = tlabel;
            pt_paint_x = (l_x - tran_tx) * d_scale;
            pt_paint_y = (l_y - tran_ty) * d_scale;

            // Characters
            int ch_wd = 0;
            label_char = new List<character_as_texture>();
            foreach (Char ch in tlabel)
            {
                label_char.Add(new character_as_texture(ch, pt_paint_x + (ch_wd * (font_size - 0.005)), pt_paint_y, font_size));
                ch_wd++;
            }

            _label_clr = tlabel_clr;
        }
    }



    public class Label_list_store
    {
        Dictionary<uint, Character> _characters = new Dictionary<uint, Character>();

        public List<Label_data> all_labels = new List<Label_data>();

        private float[] _char_vertices = new float[0];

        private uint[] _char_indices = new uint[0];

        public int total_char_count { get; private set; }

        // OpenTK variables
        public VertexBuffer label_VertexBufferObject { get; private set; }
        public List<VertexBufferLayout> label_BufferLayout { get; private set; }
        private VertexArray label_VertexArrayObject;
        private IndexBuffer label_ElementBufferObject;

        public void add_label(int label_id, string label_v, double l_x, double l_y, double d_scale, double tran_tx, double tran_ty, Color label_clr)
        {
            // Add the label to list this.dr_scale, -this.dr_tx, -this.dr_ty, Color.Brown
            all_labels.Add(new Label_data(label_id, label_v, l_x, l_y, d_scale, tran_tx, tran_ty, label_clr));
        }


        public void set_openTK_objects()
        {
            // Set the openTK objects for the points
            // Set the freetype texture
            // initialize library
            Library lib = new Library();
            uint pixelheight = 64;

            // Get the FreeSans.ttf from the resource
            byte[] res_ttf = Resource_font.HyperFont;

            MemoryStream ms = new MemoryStream(res_ttf);


            Face face = new Face(lib, ms.ToArray(), 0);

            face.SetPixelSizes(0, pixelheight);

            // set 1 byte pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // set texture unit
            GL.ActiveTexture(TextureUnit.Texture0);

            // Load first 128 characters of ASCII set
            for (uint c = 0; c < 128; c++)
            {
                try
                {
                    // load glyph
                    //face.LoadGlyph(c, LoadFlags.Render, LoadTarget.Normal);
                    face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                    GlyphSlot glyph = face.Glyph;
                    FTBitmap bitmap = glyph.Bitmap;

                    // create glyph texture
                    int texObj = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texObj);
                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                                  PixelInternalFormat.R8, bitmap.Width, bitmap.Rows, 0,
                                  PixelFormat.Red, PixelType.UnsignedByte, bitmap.Buffer);

                    // set texture parameters
                    GL.TextureParameter(texObj, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TextureParameter(texObj, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TextureParameter(texObj, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(texObj, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    // add character
                    Character ch = new Character();
                    ch.TextureID = texObj;
                    ch.Size = new Vector2(bitmap.Width, bitmap.Rows);
                    ch.Bearing = new Vector2(glyph.BitmapLeft, glyph.BitmapTop);
                    ch.Advance = (int)glyph.Advance.X.Value;
                    _characters.Add(c, ch);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " Loading Text Error");
                }
            }

            // bind default texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // set default (4 byte) pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            // Set the openTk for labels -> characters
            int i = 0;
            for (i = 0; i < all_labels.Count; i++)
            {
                for (int j = 0; j < all_labels[i].label_char.Count; j++)
                {
                    this.all_labels[i].label_char[j].set_openTK_objects(_characters);
                }
            }


            this.total_char_count = 0;

            // Count the number of characters in the whole label
            foreach (Label_data labl in all_labels)
            {
                foreach (character_as_texture char0 in labl.label_char)
                {
                    this.total_char_count++;
                }
            }

            // Set the vertices
            this._char_vertices = new float[24 * total_char_count];
            this._char_indices = new uint[6 * total_char_count];
            i = 0;

            foreach (Label_data labl in all_labels)
            {
                foreach (character_as_texture char0 in labl.label_char)
                {
                    // add the point vertices
                    float[] temp_vertices = char0.get_char_vertices();


                    int j = 0;
                    // X, Y, Z Co-ordinate

                    for (j = 0; j < temp_vertices.Length; j++)
                    {
                        this._char_vertices[(i * 24) + j] = temp_vertices[j];
                    }

                    // Add the point indices
                    for (j = 0; j < 6; j++)
                    {
                        this._char_indices[(i * 6) + j] = (uint)((i * 6) + j);
                    }
                    i++;
                }
            }

            //1.  Set up vertex buffer
            label_VertexBufferObject = new VertexBuffer(this._char_vertices, this._char_vertices.Length * sizeof(float));

            //2. Create and add to the buffer layout
            label_BufferLayout = new List<VertexBufferLayout>();
            label_BufferLayout.Add(new VertexBufferLayout(2, 4)); // Vertex layout
            label_BufferLayout.Add(new VertexBufferLayout(2, 4)); // Texture layout  

            //3. Setup the vertex Array (Add vertexBuffer binds both the vertexbuffer and vertexarray)
            label_VertexArrayObject = new VertexArray();
            label_VertexArrayObject.Add_vertexBuffer(label_VertexBufferObject, label_BufferLayout);

            // 4. Set up element buffer
            label_ElementBufferObject = new IndexBuffer(this._char_indices, this._char_indices.Length);
            label_ElementBufferObject.Bind();
        }

        public void paint_labels()
        {
            // Call set_openTK_objects()
            // Bind before painting
            label_VertexArrayObject.Add_vertexBuffer(label_VertexBufferObject, label_BufferLayout);
            label_ElementBufferObject.Bind();

            // GL paint
            GL.Enable(EnableCap.Blend);

            int i_index = 0;
            for (int i = 0; i < all_labels.Count; i++)
            {
                for (int j = 0; j < all_labels[i].label_char.Count; j++)
                {
                    this.all_labels[i].label_char[j].paint_character(i_index*6);
                    i_index++;
                }
            }
        }
    }
}
