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
    Matrix4 newM;
    public Matrix4 localM;
    Shader shader;
    Texture texture;
    Mesh mesh;
    bool root = false;
        
    public Node(Shader s, Texture t, Mesh m, bool Root = false)
    {
        shader = s;
        texture = t;
        root = Root;
        mesh = m;
        if (!root)
            localM = mesh.LocalM;
    }

    public void Render(Matrix4 parentM)
    {
        newM = parentM * localM;
        if (!root)
        {            
            mesh.Render(shader, newM, texture);
        }
        foreach(Node n in children)
        {            
          n.Render(newM);            
        }   
    }
}