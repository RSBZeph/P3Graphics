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
        public List<Node> children = new List<Node>();

        public Node(Mesh mesh, Matrix4 transformParent, Node child)
        {

        }

        public void CreateChild(Node Child)
        {
            //Node Child = new Node(mesh, transformParent, child);
            children.Add(Child);
        }

        public void Input()
        {
            foreach (Node child in children)
                child.Input();
        }

        public void Update()
        {
            foreach (Node child in children)
                child.Update();
        }
        public void Render()
        {
            foreach (Node child in children)
                child.Render();
        }




    }
}
