using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template_P3;
using OpenTK;
using System.Diagnostics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

class SceneGraph
{
    Node root;
    Texture wood, earth;                    // texture to use for rendering
    Shader shader;                          // shader to use for rendering
    Shader postproc;                        // shader to use for post processing
    Mesh teapot, floor;                     // a mesh to draw the teapot using OpenGL
    RenderTarget target;                    // intermediate render target
    ScreenQuad quad;                        // screen filling quad for post processing
    bool useRenderTarget = true;
    const float PI = 3.1415926535f;         // PI
    float a = 0f;                           // teapot rotation angle
    public Surface screen;                  // background surface for printing etc.
    Stopwatch timer;                        // timer for measuring frame duration

    public void Init()
    {
        LoadTextures();
        LoadMeshes();
        teapot.specularity = 20;
        floor.specularity = 70;

        shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
        postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
        
        quad = new ScreenQuad();
        timer = new Stopwatch();
        timer.Reset();
        timer.Start();

        // create the render target
        target = new RenderTarget(screen.width, screen.height);

        int lightID = GL.GetUniformLocation(shader.programID,"lightPos");   
        GL.UseProgram( shader.programID );
        GL.Uniform3(lightID,10.0f, 0.0f, 10.0f); //20x20x20 worldspace, telkens van -10 tot 10, voorwerp rond (0, 0, 0), -z is van de camera af

        root = new Node(shader,wood , teapot);
        root.localM = Matrix4.Identity;
        CreateChildren();
    }

    void LoadMeshes()
    {   
        teapot = new Mesh("../../assets/teapot.obj");
        floor = new Mesh("../../assets/floor.obj");
    }

    void LoadTextures()
    {
        earth = new Texture("../../assets/Earth.png");
        wood = new Texture("../../assets/wood.jpg");
    }

    void CreateChildren()
    {
        Node teapotN = new Node(shader, earth, teapot); 
        teapotN.localM = new Matrix4(new Vector4(1,0,0,0),new Vector4(0,1,0,-6),new Vector4(0,0,0,-15),new Vector4(0,0,0,1));
        root.children.Add(teapotN);
        Node floorN = new Node(shader, wood, floor);
        floorN.localM = new Matrix4(new Vector4(1,0,0,0),new Vector4(0,0,0,2),new Vector4(0,0,1,0),new Vector4(0,0,0,1));
        teapotN.children.Add(floorN);
    }

    public void Render()
    {
            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            // prepare matrix for vertex shader
            Matrix4 transform = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);
            Matrix4 ftransform = transform;
            Matrix4 ToWorld = transform;
            //transform *= Matrix4.CreateTranslation(0, -4, -15);
            //ftransform *= Matrix4.CreateTranslation(0, -6, -15);
            //transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            //ftransform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            

            // update rotation
            a += 0.001f * frameDuration;
            if (a > 2 * PI) a -= 2 * PI;

            if (useRenderTarget)
            {
                // enable render target
                target.Bind();

                // render scene to render target
                root.Render(ToWorld, ToWorld);

                // render quad
                target.Unbind();
                quad.Render(postproc, target.GetTextureID());
            }
            else
            {
                // render scene directly to the screen
                root.Render(ToWorld, ToWorld);
            }
    }
}