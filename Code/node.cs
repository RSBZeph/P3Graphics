using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template_P3;

class Node
{
    public List<Node> children = new List<Node>();
    Matrix4 ToWorld;
    public Matrix4 localM;
    Shader shader;
    Texture texture;
    Mesh mesh;
    bool rendernode;
        
    public Node(Shader s, Texture t, Mesh m, bool Rendernode = true)
    {
        shader = s;
        texture = t;
        rendernode = Rendernode;
        mesh = m;
        if (rendernode)
            localM = mesh.LocalM;
    }

    public void Render(Matrix4 parentM)
    {
        ToWorld = parentM * localM;
        if (rendernode)
        {            
            mesh.Render(shader, ToWorld, texture);
        }
        foreach(Node n in children)
        {            
          n.Render(ToWorld);            
        }   
    }
}