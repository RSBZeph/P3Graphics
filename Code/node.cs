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

    //Fills in the variables given when an instance is created
    public Node(Shader s, Texture t, Mesh m, bool RenderNode = true)
    {
        shader = s;
        texture = t;
        rendernode = RenderNode;
        mesh = m;
        if (rendernode)
            localM = mesh.LocalM;
    }

    //When called Renders the Objects and their children, while calculatin the proper matrices for them
    public void Render(Matrix4 parentM, Matrix4 cameraM)
    {
        var TW = localM * parentM;
        var TC = localM * cameraM;
        foreach(Node n in children) //Renders the children
        {            
          n.Render(TW, TC);            
        }   
        TC *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        if (rendernode)               
            mesh.Render(shader, TW, TC, texture);        
    }
}