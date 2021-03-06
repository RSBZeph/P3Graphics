﻿using System.Diagnostics;
using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3
{
    class Game
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        public SceneGraph scenegraph;

        // initialize
        public void Init()
        {
            scenegraph = new SceneGraph();
            scenegraph.screen = screen;
            scenegraph.Init();
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffff00);
        }

        // Calls the render method from the SceneGraph
        public void RenderGL()
        {
            scenegraph.Render();
        }
    }
} // namespace Template_P3