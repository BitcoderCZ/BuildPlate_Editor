using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    public class Shader
    {
        public int ProgramId;

        private string vertexShaderSource;
        private string fragmentShaderSource;

        private static List<ShaderVariable> vars = new List<ShaderVariable>();

        public void Compile(string shaderName)
        {
            string path = Program.baseDir + "Data/Shaders/" + shaderName;
            vertexShaderSource = File.ReadAllText(path + ".vert");
            fragmentShaderSource = File.ReadAllText(path + ".frag");

            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexShaderSource);
            GL.CompileShader(vertex);

            GL.GetShader(vertex, ShaderParameter.CompileStatus, out int status);
            
            if (status == 0) {
                string err = GL.GetShaderInfoLog(vertex);
                Console.WriteLine($"ERROR::SHADER::VERTEX::COMPILATION_FAILED\n{ err}");
            }

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentShaderSource);
            GL.CompileShader(fragment);

            GL.GetShader(fragment, ShaderParameter.CompileStatus, out status);

            if (status == 0) {
                string err = GL.GetShaderInfoLog(vertex);
                Console.WriteLine($"ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n{err}");
            }

            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, vertex);
            GL.AttachShader(ProgramId, fragment);
            GL.LinkProgram(ProgramId);

            GL.GetProgram(ProgramId, GetProgramParameterName.LinkStatus, out status);
            if (status == 0) {
                string err = GL.GetProgramInfoLog(vertex);
                Console.WriteLine($"ERROR::SHADER::PROGRAM::LINKING_FAILED\n{err}");
            }

            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);

            GL.GetProgram(ProgramId, GetProgramParameterName.ActiveUniforms, out int numbUniforms);

            GL.GetProgram(ProgramId, GetProgramParameterName.ActiveUniformMaxLength, out int maxNameLength);

            if (numbUniforms > 0) {
                for (int i = 0; i < numbUniforms; i++) {
                    GL.GetActiveUniform(ProgramId, i, maxNameLength, out int length, out int size, out ActiveUniformType type, out string name);
                    int varLocation = GL.GetUniformLocation(ProgramId, name);
                    ShaderVariable var = new ShaderVariable() { name = name, location = varLocation, programId = ProgramId };
                    vars.Add(var);
                }
            }
        }

        public void Bind()
        {
            GL.UseProgram(ProgramId);
        }

        public void UploadInt(string name, int val)
        {
            int loc = GetVarLocation(this, name);
            GL.Uniform1(loc, val);
        }

        public void UploadMat4(string name, ref Matrix4 val)
        {
            GL.UniformMatrix4(GetVarLocation(this, name), false, ref val);
        }

        public static int GetVarLocation(Shader shader, string name)
        {
            ShaderVariable match = new ShaderVariable() { name = name, programId = shader.ProgramId };

            for (int i = 0; i < vars.Count; i++)
                if (vars[i] == match)
                    return vars[i].location;

            return -1;
        }

        struct ShaderVariable
        {
            public string name;
            public int location;
            public int programId;

            public static bool operator ==(ShaderVariable a, ShaderVariable b)
                => a.name == b.name && a.programId == b.programId;
            public static bool operator !=(ShaderVariable a, ShaderVariable b)
                => a.name != b.name || a.programId != b.programId;
        }
    }
}
