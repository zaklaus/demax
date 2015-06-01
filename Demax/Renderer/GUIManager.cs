//
//  Author:
//    Dominik Madarász combatwz.sk@gmail.com
//
//  Copyright (c) 2015, ZaKlaus
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the [ORGANIZATION] nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Jitter.Collision;
using Jitter;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace Demax
{
    public class GUIManager
    {
        CRenderer r;
        public void Draw()
        {
            r = CCore.GetCore().Renderer;
            
            List<Vector3> a = new System.Collections.Generic.List<Vector3>();
            a.Add(new Vector3(-1,-1,0));
            a.Add(new Vector3(1, -1, 0));
            a.Add(new Vector3(1, 1, 0));
            a.Add(new Vector3(-1, 1, 0));

            GL.BindBuffer(BufferTarget.ArrayBuffer, r.shaders[CShaderProgram.LoadShaderPointer("gui_color")].GetBuffer("vPosition"));

            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(a.ToArray().Length * Vector3.SizeInBytes), a.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(r.shaders[CShaderProgram.LoadShaderPointer("gui_color")].GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, r.shaders[CShaderProgram.LoadShaderPointer("gui_color")].GetBuffer("vColor"));

            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(a.ToArray().Length * Vector3.SizeInBytes), a.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(r.shaders[CShaderProgram.LoadShaderPointer("gui_color")].GetAttribute("vColor"), 3, VertexAttribPointerType.Float, false, 0, 0);

            r.shaders[CShaderProgram.LoadShaderPointer("gui_color")].EnableVertexAttribArrays();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 4);
            r.shaders[CShaderProgram.LoadShaderPointer("gui_color")].DisableVertexAttribArrays();
        }
    }
}
