using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template_P3;
using OpenTK;

class SceneGraph
{
    Node root;
    Matrix4 ToWorld;

    public SceneGraph(Matrix4 toworld)
    {
        ToWorld = toworld;
        root = new Node(toworld, shader, , ,ToWorld);
    }

    public void Render()
    {
        root.Render(ToWorld);
    }
}