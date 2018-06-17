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
    Matrix4 parentM, localM, newM, ToWorld;
    Shader shader;
    Texture texture;
    Mesh mesh;
        
    public Node(Matrix4 p, Shader s, Texture t, Mesh m, Matrix4 TW)
    {
        parentM = p;
        shader = s;
        texture = t;
        mesh = m;
        localM = mesh.LocalM;
        ToWorld = TW;
    }

    public void Render(Matrix4 parent)
    {
        newM = parent * localM;    
        mesh.Render(shader, newM, ToWorld, texture);
        foreach(Node n in children)
        {            
          n.Render(newM);            
        }   
    }
}