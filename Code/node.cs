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
        
    public Node(Shader s, Texture t, Mesh m)
    {
        shader = s;
        texture = t;
        mesh = m;
        localM = mesh.LocalM;
    }

    public void Render(Matrix4 parentM, Matrix4 TW)
    {
        newM = parentM * localM;    
        mesh.Render(shader, newM, TW, texture);
        foreach(Node n in children)
        {            
          n.Render(newM, TW);            
        }   
    }
}