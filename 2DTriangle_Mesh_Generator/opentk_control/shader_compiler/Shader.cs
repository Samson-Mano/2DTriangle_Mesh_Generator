using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

// Thanks to https://github.com/mariugul/LearnOpenTK/ for very detailed explaination.

namespace _2DTriangle_Mesh_Generator.opentk_control.shader_compiler
{
    public class Shader 
    {
        public readonly int Handle;

        private readonly Dictionary<string, int> _uniformLocations;

        public Shader_Support update_shader = new Shader_Support();

        // This is how you create a simple shader.
        // Shaders are written in GLSL, which is a language very similar to C in its semantics.
        // The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
        // A commented example of GLSL can be found in shader.vert
        public Shader(string vertShader, string fragShader)
        {
            // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
            // The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
            //   The vertex shader won't be too important here, but they'll be more important later.
            // The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
            //   The fragment shader is what we'll be using the most here.

            // Load vertex shader and compile
            // LoadSource is a simple function that just loads all text from the file whose path is given.

            // var shaderSource = LoadSource(vertPath);
            // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            // Now, bind the GLSL source code
            GL.ShaderSource(vertexShader, vertShader);

            // And then compile
            CompileShader(vertexShader);

            // We do the same for the fragment shader
            //  shaderSource = LoadSource(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragShader);
            CompileShader(fragmentShader);

            // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
            // To do this, create a program...
            Handle = GL.CreateProgram();

            // Attach both shaders...
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            // And then link them together.
            LinkProgram(Handle);

            // When the shader program is linked, it no longer needs the individual shaders attacked to it; the compiled code is copied into the shader program.
            // Detach them, and then delete them.
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
            // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
            // later.

            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Next, allocate the dictionary to hold the locations.
            _uniformLocations = new Dictionary<string, int>();

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(Handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(Handle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }
        }

        private static void CompileShader(int shader)
        {
            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            // We link the program
            GL.LinkProgram(program);

            // Check for linking errors
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        // A wrapper function that enables the shader program.
        public void Use()
        {
            GL.UseProgram(Handle);
        }

        // The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
        // you can omit the layout(location=X) lines in the vertex shader, and use this in VertexAttribPointer instead of the hardcoded values.
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }


        // Just loads the entire file into a string.
        private static string LoadSource(string path)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        // Uniform setters
        // Uniforms are variables that can be set by user code, instead of reading them from the VBO.
        // You use VBOs for vertex-related data, and uniforms for almost everything else.

        // Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
        //     1. Bind the program you want to set the uniform on
        //     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
        //     3. Use the appropriate GL.Uniform* function to set the uniform.

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[name], data);
        }

        /*
        ______________________________________________________________________________________________________
        ______________________________________________________________________________________________________
        __________________________ Scale and Translation control _____________________________________________
        ______________________________________________________________________________________________________
        ______________________________________________________________________________________________________
        */

        //// scale to control the units of drawing area
        //private float _primary_scale = 1.0f;
        //// zoom scale
        //private float _zm_scale = 1.0f;
        //// Translation details
        //private Vector3 _current_translation = new Vector3(0.0f, 0.0f, 0.0f);
        //private Vector3 _previous_translation = new Vector3(0.0f, 0.0f, 0.0f);

        //public Vector3 previous_translation { get { return this._previous_translation; } }


        //#region "Zoom and Pan operation of openGL control"
        //public void scale_intelli_zoom_Transform(float zm, float tx, float ty)
        //{
        //    this._zm_scale = zm;

        //    //update the scale
        //    SetFloat("gScale", (this._zm_scale * this._primary_scale));

        //    translate_Transform(tx, ty);
        //    save_translate_transform();
        //}

        //public void scale_Transform(float zm)
        //{
        //    this._zm_scale = zm;

        //    //update the scale
        //    SetFloat("gScale", (this._zm_scale * this._primary_scale));
        //}

        //public void translate_Transform(float trans_x, float trans_y)
        //{
        //    // 2D Translatoin
        //    _current_translation = new Vector3(trans_x + this._previous_translation.X,
        //        trans_y + this._previous_translation.Y,
        //        0.0f + this._previous_translation.Z);

        //    Matrix4 current_transformation = new Matrix4(1.0f, 0.0f, 0.0f, _current_translation.X,
        //        0.0f, 1.0f, 0.0f, _current_translation.Y,
        //        0.0f, 0.0f, 1.0f, 0.0f,
        //        0.0f, 0.0f, 0.0f, 1.0f);

        //    SetMatrix4("gTranslation", current_transformation);
        //    // update_the_drawing_scale();
        //}

        //public void save_translate_transform()
        //{
        //    // save the final translation
        //    _previous_translation = _current_translation;
        //}
        //#endregion

    }
}
