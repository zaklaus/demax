using OpenTK;
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demax
{
    public static class Extensions
    {
        public static bool IsEmpty(this string s)
        {
            return (s == "");
        }

        public static Vector3 GetVector3(this string s)
        {
            Vector3 v = Vector3.Zero;

            string[] data = s.Split(';');

            TryParseGlobal(data[0], out v.X);
            TryParseGlobal(data[1], out v.Y);
            TryParseGlobal(data[2], out v.Z);

            return v;
        }

        public static bool Compare(Vector3 first, Vector3 second)
        {
            if (
               (first[0] <= second[0])
            && (first[1] <= second[1])
            && (first[2] <= second[2])
            )
                return true;
            return false;
        }

        public static bool IsInside3D(Vector3 start, Vector3 end, Vector3 point)
        {
            if (
                (point.X > start.X && point.X < end.X)
                && (point.Y > start.Y && point.Y < end.Y)
                && (point.Z > start.Z && point.Z < end.Z)
                )
                return true;
            return false;
        }

        public static string GetString(this Vector3 v)
        {
            return string.Format("{0};{1};{2}", v.X, v.Y, v.Z);
        }

        public static bool TryParseGlobal(string s, out float f)
        {
            return float.TryParse(s, NumberStyles.Any, new CultureInfo ("en-US"), out f);
        }

        /// <summary>
        /// Get distance between two 3D points.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static float GetDistance3D(Vector3 first, Vector3 second)
        {
            float deltaX = second.X - first.X;
            float deltaY = second.Y - first.Y;
            float deltaZ = second.Z - first.Z;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }
    }
}
