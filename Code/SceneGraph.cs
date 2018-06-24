using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using Template_P3;
using OpenTK;
using OpenTK.Input;
using System.Diagnostics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


class SceneGraph
{
    SoundPlayer music;    
    Node root;
    Node teapotN, floorN, carN, empty, smallteapot, carN2;
    Texture wood, cool, track, carTex;// texture to use for rendering
    Shader shader;                          // shader to use for rendering
    Shader postproc;                        // shader to use for post processing
    Mesh teapot, floor, magikarp, car;      // a mesh to draw the teapot using OpenGL 
    RenderTarget target;                    // intermediate render target
    ScreenQuad quad;                        // screen filling quad for post processing
    bool useRenderTarget = true;
    const float PI = 3.1415926535f;         // PI
    float a = 0f, b = 0f;                   // teapot rotation angle
    int lightID, camID;
    public Surface screen;                  // background surface for printing etc.
    Stopwatch timer;                        // timer for measuring frame duration
    Matrix4 ToWorld = Matrix4.Identity, cameraM = Matrix4.Identity;
    KeyboardState KBS;
    Vector3 lightpos3 = new Vector3(0, 15, -3);

    public void Init()
    {
        music = new SoundPlayer("../../assets/DejaVu.wav");
        music.PlayLooping();
        LoadTextures();
        LoadMeshes();
        //set specularity of each mesh
        teapot.specularity = 20;
        floor.specularity = 200;
        car.specularity = 10;        

        shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
        postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
        
        quad = new ScreenQuad();
        timer = new Stopwatch();
        timer.Reset();
        timer.Start();

        // create the render target
        target = new RenderTarget(screen.width, screen.height);

        // pass cameraposition to fragment shader
        camID = GL.GetUniformLocation(shader.programID, "campos");
        GL.UseProgram(shader.programID);
        GL.Uniform3(camID, 0,0,0);

        // pass lightposition to fragment shader
        lightID = GL.GetUniformLocation(shader.programID,"lightPos");   
        GL.UseProgram( shader.programID );
        GL.Uniform3(lightID,lightpos3); //-z is van de camera af

        root = new Node(shader, null, null, false);
        root.localM = Matrix4.Identity;
        CreateChildren();        
    }

    void LoadMeshes()
    {   
        teapot = new Mesh("../../assets/teapot.obj");
        floor = new Mesh("../../assets/floor.obj");
        car = new Mesh("../../assets/car.obj");
    }

    void LoadTextures()
    {
        wood = new Texture("../../assets/wood.jpg");
        cool = new Texture("../../assets/cool.jpg");
        track = new Texture("../../assets/track.jpeg");
        carTex = new Texture("../../assets/car2.jpg");
    }

    //Creates the objects wich will be rendered
    void CreateChildren()
    {
        empty = new Node(shader, null, null, false);
        empty.localM = Matrix4.CreateTranslation( 0, -4, -15 );
        root.children.Add(empty);

        teapotN = new Node(shader, wood, teapot);
        empty.children.Add(teapotN);

        smallteapot = new Node(shader, cool, teapot);
        teapotN.children.Add(smallteapot);

        floorN = new Node(shader, track, floor);
        floorN.localM = Matrix4.CreateScale(5);
        empty.children.Add(floorN);

        carN = new Node(shader, cool, car);
        floorN.children.Add(carN);

        carN2 = new Node(shader, carTex, car);
        floorN.children.Add(carN2);
    }

    public void Render()
    {
        // measure frame duration
        float frameDuration = (float)timer.Elapsed.TotalSeconds;    //timer.ElapsedMilliseconds;
        timer.Reset();
        timer.Start();
        
        CameraControls(frameDuration);

        //prepare matrices for vertex shader
        Vector3 campos3 = -cameraM.Row3.Xyz;
        camID = GL.GetUniformLocation(shader.programID, "campos");
        GL.UseProgram(shader.programID);
        GL.Uniform3(camID, campos3);
        
        Vector3 newlightpos3 = lightpos3;
        lightID = GL.GetUniformLocation(shader.programID,"lightPos");   
        GL.UseProgram( shader.programID );
        GL.Uniform3(lightID,newlightpos3);

        Matrix4 teapotT = Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), -a);       
        teapotN.localM = teapotT;

        Matrix4 smallteapotT = Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0), -b);
        smallteapotT *= Matrix4.CreateScale(0.6f);
        smallteapotT *= Matrix4.CreateTranslation(8f, 7f, 0);
        smallteapot.localM = smallteapotT;

        Matrix4 carT = Matrix4.CreateScale(0.8f);
        carT *= Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0), 0.8f * (float)Math.PI);;
        carT *= Matrix4.CreateTranslation(4f, -2.2f, 0);
        carT *= Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0), b);
        carN.localM = carT;

        Matrix4 carT2 = Matrix4.CreateFromAxisAngle(new Vector3(0, -1, 0), 0.2f * (float)Math.PI); ;
        carT2 *= Matrix4.CreateTranslation(-3.5f, -2.25f, 0);
        carT2 *= Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), b);
        carN2.localM = carT2;


        // update rotation
        a += 1f * frameDuration;
        b += 1.5f * frameDuration;
        if (a > 2 * PI) 
            a -= 2 * PI;
        if (b > 2 * PI)
            b -= 2* PI;

        if (useRenderTarget)
        {
            // enable render target
            target.Bind();

            // render scene to render target
            root.Render(ToWorld, cameraM);

            // render quad
            target.Unbind();
            quad.Render(postproc, target.GetTextureID());
        }
        else
        {
            // render scene directly to the screen
            root.Render(ToWorld, cameraM);
        }
    }

    void CameraControls(float frameDuration)
        {
            KBS = Keyboard.GetState();
            if (KBS.IsKeyDown(Key.E))        
                cameraM *= Matrix4.CreateTranslation(0, -(16f * frameDuration), 0);        
            if (KBS.IsKeyDown(Key.Q))        
                cameraM *= Matrix4.CreateTranslation(0, (16f * frameDuration), 0);        
            if (KBS.IsKeyDown(Key.A))        
                cameraM *= Matrix4.CreateTranslation((16f * frameDuration), 0, 0);        
            if (KBS.IsKeyDown(Key.D))        
                cameraM *= Matrix4.CreateTranslation(-(16f * frameDuration), 0, 0);        
            if (KBS.IsKeyDown(Key.W))        
                cameraM *= Matrix4.CreateTranslation(0, 0, (16f * frameDuration));        
            if (KBS.IsKeyDown(Key.S))        
                cameraM *= Matrix4.CreateTranslation(0, 0, -(16f * frameDuration));        
            if (KBS.IsKeyDown(Key.I))        
                cameraM *= Matrix4.CreateRotationX((-0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.K))        
                cameraM *= Matrix4.CreateRotationX((0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.J))        
                cameraM *= Matrix4.CreateRotationY(-(0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.L))        
                cameraM *= Matrix4.CreateRotationY((0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.U))        
                cameraM *= Matrix4.CreateRotationZ(-(0.8f * frameDuration));        
            if (KBS.IsKeyDown(Key.O))        
                cameraM *= Matrix4.CreateRotationZ((0.8f * frameDuration));        
        }
}
