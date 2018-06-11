using System;
using OpenTK.Graphics.OpenGL;

public class node
{
    public node Parent;
    public Vector3 offset, position;
    
	public node(node parent, Vector3 Offset)
	{
        if (parent != null)
            position = parent.position + Offset;
        else
            position = Offset;
        offset = Offset;
        Parent = parent;
	}
}