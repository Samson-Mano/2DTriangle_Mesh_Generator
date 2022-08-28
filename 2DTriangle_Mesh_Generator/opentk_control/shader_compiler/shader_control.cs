using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.opentk_control.shader_compiler
{
  public  class shader_control
    {
        public enum shader_type
        {
            br_shader,
            dr_shader
        }

        #region "Vertex Shaders"
        public string br_vert_shader()
        {
            return "#version 330 core\r\n" +
            "\r\n" +
            "layout(location = 0) in vec3 aPosition;\r\n" +
            "layout(location = 1) in vec4 vertexColor;\r\n" +
            "\r\n" +
            "uniform float gScale = 1.0f;\r\n" +
            "\r\n" +
            "uniform mat4 gTranslation = mat4(1.0, 0.0, 0.0, 0.0,  // 1. column\r\n" +
                                             "0.0, 1.0, 0.0, 0.0,  // 2. column\r\n" +
                                             "0.0, 0.0, 1.0, 0.0,  // 3. column\r\n" +
                                             "0.0, 0.0, 0.0, 1.0); // 4. column\r\n" +
            "\r\n" +
            "\r\n" +
            "out vec4 v_Color;\r\n" +
            "\r\n" +
            "void main()\r\n" +
            "{\r\n" +
                "v_Color = vertexColor;\r\n" +
                 "\r\n" +
                "gl_Position = gTranslation * vec4(gScale * aPosition, 1.0);\r\n" +
            "}";
        }
        #endregion

        #region "Fragment shaders"
        public string br_frag_shader()
        {
            return "#version 330 core\r\n" +
                "\r\n" +
                "in vec4 v_Color;\r\n" +
                "out vec4 f_Color; // fragment's final color (out to the fragment shader)\r\n" +
                "\r\n" +
                "void main()\r\n" +
                "{\r\n" +
                "f_Color = v_Color;\r\n" +
                "}";
        }
        #endregion

        public shader_control()
        {
            // Empty cconstructor
        }

        public string get_vertex_shader(shader_type stype)
        {
            // Returns the vertex shader
            string vert_out = "";
            if (stype == shader_type.br_shader)
            {
                vert_out = br_vert_shader();
            }
            return vert_out;
        }

        public string get_fragment_shader(shader_type stype)
        {
            // Returns the fragment shader
            string frag_out = "";
            if (stype == shader_type.br_shader)
            {
                frag_out = br_frag_shader();
            }
            return frag_out;
        }
    }
}
