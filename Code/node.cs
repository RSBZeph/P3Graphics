using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template_P3;


namespace template_P3.Code
{
    class Node
    {
        public List<Node> nodes = new List<Node>();

        public Node(Mesh mesh, Matrix4 transformParent, Node child)
        {

        }

        public void CreateChild(Mesh mesh, Matrix4 transformParent, Node child)
        {
            Node Child = new Node(mesh, transformParent, child);
            nodes.Add(Child);
        }

    }
}
