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
    public Matrix4 localM;
    Shader shader;
    Texture texture;
    Mesh mesh;
    bool rendernode = true;
        
    public Node(Shader s, Texture t, Mesh m, bool RenderNode = true)
    {
        shader = s;
        texture = t;
        rendernode = RenderNode;
        mesh = m;
        if (rendernode)
            localM = mesh.LocalM;
    }

    public void Render(Matrix4 parentM, Matrix4 Cam)
    {
        Matrix4 TW = localM * parentM;
        Matrix4 TC = Cam;
        if (rendernode)
        {            
            mesh.Render(shader, TW, TC, texture);
        }
        foreach(Node n in children)
        {            
          n.Render(TW, TC);            
        }   
    }
}