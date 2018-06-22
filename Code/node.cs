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

    public void Render(Matrix4 parentM, Matrix4 cameraM)
    {
        var ToWorld = localM * parentM;
        var TC = cameraM * ToWorld;
        foreach(Node n in children)
        {            
          n.Render(ToWorld, cameraM);            
        }   
        ToWorld = parentM * localM * Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        TC = cameraM * ToWorld;
        if (rendernode)
        {       
            mesh.Render(shader, ToWorld, TC, texture);
        }
    }
}