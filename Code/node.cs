using System;
using OpenTK.Graphics.OpenGL;

public class node
{
    public node Parent;
    public Matrix4 offset, position;
    public string name;
    
	public node(node parent, Vector3 Offset, string Name)
	{
        if (parent != null)
            position = parent.position + Offset;
        else
            position = Offset;
        offset = Offset;
        Parent = parent;
        name = Name;
	}
}