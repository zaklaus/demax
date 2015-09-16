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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Demax
{
    public class CLevel
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public int ID { get; set; }

        public CEntityManager entities {get; set; }
        public XmlDocument xml { get; set; }

        public static List<CLevel> levels = new List<CLevel>();
        public static CLevel CurrentLevel { get; set; }

        public static CLevel LoadLevel(string filename)
        {
            XmlDocument x = new XmlDocument();
            x.Load(filename);
            CLevel lvl = new CLevel { Name = x.DocumentElement.Attributes["name"].InnerText, Author = x.DocumentElement.Attributes["author"].InnerText, Version = x.DocumentElement.Attributes["version"].InnerText, FileName = filename, xml = x, ID = levels.Count - 1, entities = new CEntityManager() };
            levels.Add(lvl);
            CLog.WriteLine(string.Format("Level: {0}, Author: {1}, Version: {2}", lvl.Name, lvl.Author, lvl.Version));

            return lvl;
        }

        public static CLevel StartLevel(string name="", string filename="", int id=-1)
        {
            if(CurrentLevel != null)
            {
                foreach (var e in CurrentLevel.entities.entities)
                    CCore.GetCore().EntityManager.entities.Remove(e);
                CurrentLevel.entities.entities.Clear();
            }

            int levelID = -1;
            if (!name.IsEmpty())
            {
                foreach (var level in levels)
                {
                    if (level.Name == name)
                    {
                        levelID = level.ID; break;
                    }
                }
            }
            else if (!filename.IsEmpty())
            {
                foreach (var level in levels)
                {
                    if (level.FileName == filename)
                    {
                        levelID = level.ID; break;
                    }
                }
            }
            else levelID = id;

            if (levelID != -1)
                CurrentLevel = levels[levelID];

            foreach(XmlNode node in CurrentLevel.xml.DocumentElement.LastChild.ChildNodes)
            {
                CLog.WriteLine(node.Name);
                Vector3 eposition = Vector3.Zero, erotation = Vector3.Zero, escale = Vector3.One;
                string ename = "NewEntity", etag = "";

                if(node.Attributes["position"] != null)
                    eposition = node.Attributes["position"].InnerText.GetVector3();
                if (node.Attributes["rotation"] != null)
                    erotation = node.Attributes["rotation"].InnerText.GetVector3();
                if (node.Attributes["scale"] != null)
                    escale = node.Attributes["scale"].InnerText.GetVector3();
                if (node.Attributes["name"] != null)
                    ename = node.Attributes["name"].InnerText;
                if (node.Attributes["tag"] != null)
                    etag = node.Attributes["tag"].InnerText;

                CEntity root = new CEntity { Name = ename, Tag = etag, Transform = new CTransform(eposition, erotation, escale) };
                CLog.WriteLine(string.Format("Map Entity: {0}", root.Name));
                foreach(XmlNode childNode in node.ChildNodes)
                {
                    if(childNode.Name == "script")
                    {
                        root.AddScript(childNode.Attributes["file"].InnerText);
                    }

                    if (childNode.Name == "volume")
                    {
                        string vtype = "mod";
                        string rigidbody = "false";
                        string shader = "light";
                        string isStatic = "false";
                        Vector3 vposition = Vector3.Zero, vrotation = Vector3.Zero, vscale = Vector3.One;


                        if(childNode.Attributes["position"] != null)
                            vposition = childNode.Attributes["position"].InnerText.GetVector3();
                        if (childNode.Attributes["rotation"] != null)
                            vrotation = childNode.Attributes["rotation"].InnerText.GetVector3();
                        if (childNode.Attributes["scale"] != null)
                            vscale = childNode.Attributes["scale"].InnerText.GetVector3();
                        if (childNode.Attributes["type"] != null)
                            vtype = childNode.Attributes["type"].InnerText;
                        if (childNode.Attributes["rigidbody"] != null)
                            rigidbody = childNode.Attributes["rigidbody"].InnerText;
                        if (childNode.Attributes["static"] != null)
                            isStatic = childNode.Attributes["static"].InnerText;
                        if (childNode.Attributes["shader"] != null)
                            shader = childNode.Attributes["shader"].InnerText;

                        if(vtype=="obj")
                        {
                            ObjVolume o = ObjVolume.LoadOBJ(root, childNode.Attributes["file"].InnerText);
                            o.SetPosition(vposition);
                            o.SetRotation(vrotation);
                            o.SetScale(vscale);
                            foreach (var m in o.meshes)
                            {
                                m.Shader = shader;
                            }
                            if (rigidbody == "true")
                                o.AddRigidbody();
                            if (isStatic == "true")
                                o.SetStatic(true);
                            root.AddModel(o);
                        }
                        else if (vtype == "mod")
                        {
                            ObjVolume o = ObjVolume.ImportModel(root, childNode.Attributes["file"].InnerText);
                            o.SetPosition(vposition);
                            o.SetRotation(vrotation);
                            o.SetScale(vscale);
                            foreach (var m in o.meshes)
                            {
                                m.Shader = shader;
                            }
                            if (rigidbody == "true")
                                o.AddRigidbody();
                            if (isStatic == "true")
                                o.SetStatic(true);
                            root.AddModel(o);
                        }
                    }
                }
                CurrentLevel.entities.entities.Add(root);    
            }
            CCore.GetCore().EntityManager.entities.AddRange(CurrentLevel.entities.entities);

            if(CurrentLevel != null)
                CLog.WriteLine(string.Format("Active Level: {0}", CurrentLevel.Name));
            return CurrentLevel;
        }
    }
}
