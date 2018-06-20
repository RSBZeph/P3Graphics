using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template_P3;
using OpenTK;
using OpenTK.Input;
using System.Diagnostics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

class SceneGraph
{
    Node root;
    Node teapotN, floorN;
    Texture wood, earth;                    // texture to use for rendering
    Shader shader;                          // shader to use for rendering
    Shader postproc;                        // shader to use for post processing
    Mesh teapot, floor;                     // a mesh to draw the teapot using OpenGL
    RenderTarget target;                    // intermediate render target
    ScreenQuad quad;                        // screen filling quad for post processing
    bool useRenderTarget = true;
    const float PI = 3.1415926535f;         // PI
    float a = 0f;                           // teapot rotation angle
    int lightID;
    public Surface screen;                  // background surface for printing etc.
    Stopwatch timer;                        // timer for measuring frame duration
    Matrix4 transform, ftransform, ToWorld = Matrix4.Identity, cameraM = Matrix4.Identity;
    KeyboardState KBS;
    Vector3 lightpos3 = new Vector3(7, 2, 0);

    public void Init()
    {
        LoadTextures();
        LoadMeshes();
        teapot.specularity = 20;
        floor.specularity = 80;

        shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
        postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
        
        quad = new ScreenQuad();
        timer = new Stopwatch();
        timer.Reset();
        timer.Start();

        // create the render target
        target = new RenderTarget(screen.width, screen.height);

        lightID = GL.GetUniformLocation(shader.programID,"lightPos");   
        GL.UseProgram( shader.programID );
        GL.Uniform3(lightID,lightpos3); //-z is van de camera af

        root = new Node(shader, null, null, true);
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
        teapotN = new Node(shader, wood, teapot);
        root.children.Add(teapotN);
        floorN = new Node(shader, wood, floor);
        teapotN.children.Add(floorN);
    }

    public void Render()
    {
        // measure frame duration
        float frameDuration = (float)timer.Elapsed.TotalSeconds;//timer.ElapsedMilliseconds;
        timer.Reset();
        timer.Start();
        
        CameraControls(frameDuration);

        //prepare matrix for vertex shader
        Matrix4 transform = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
        transform *= Matrix4.CreateTranslation(0, -4, -15);
        transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        teapotN.localM = transform;

        ToWorld = cameraM;

        Matrix4 lightposM = Matrix4.CreateTranslation(lightpos3);
        lightposM *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        lightposM = (ToWorld * lightposM);
        Vector3 newlightpos3 = lightposM.Row3.Xyz;
        lightID = GL.GetUniformLocation(shader.programID,"lightPos");   
        GL.UseProgram( shader.programID );
        GL.Uniform3(lightID,newlightpos3);

        //floorN.localM = ftransform;

        // update rotation
        //a += 1f * frameDuration;
        a = 0;
        if (a > 2 * PI) 
            a -= 2 * PI;

        if (useRenderTarget)
        {
            // enable render target
            target.Bind();

            // render scene to render target
            root.Render(ToWorld);

            // render quad
            target.Unbind();
            quad.Render(postproc, target.GetTextureID());
        }
        else
        {
            // render scene directly to the screen
            root.Render(ToWorld);
        }
    }

    void CameraControls(float frameDuration)
        {
            KBS = Keyboard.GetState();
            if (KBS.IsKeyDown(Key.E))        
                cameraM *= Matrix4.CreateTranslation(0, (8f * frameDuration), 0);        
            if (KBS.IsKeyDown(Key.Q))        
                cameraM *= Matrix4.CreateTranslation(0, -(8f * frameDuration), 0);        
            if (KBS.IsKeyDown(Key.A))        
                cameraM *= Matrix4.CreateTranslation((8f * frameDuration), 0, 0);        
            if (KBS.IsKeyDown(Key.D))        
                cameraM *= Matrix4.CreateTranslation(-(8f * frameDuration), 0, 0);        
            if (KBS.IsKeyDown(Key.W))        
                cameraM *= Matrix4.CreateTranslation(0, 0, (8f * frameDuration));        
            if (KBS.IsKeyDown(Key.S))        
                cameraM *= Matrix4.CreateTranslation(0, 0, -(8f * frameDuration));        
            if (KBS.IsKeyDown(Key.K))        
                cameraM *= Matrix4.CreateRotationX((0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.I))        
                cameraM *= Matrix4.CreateRotationX(-(0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.J))        
                cameraM *= Matrix4.CreateRotationY((0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.L))        
                cameraM *= Matrix4.CreateRotationY(-(0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.U))        
                cameraM *= Matrix4.CreateRotationZ((0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.O))        
                cameraM *= Matrix4.CreateRotationZ(-(0.8f * frameDuration));        
        }
}
