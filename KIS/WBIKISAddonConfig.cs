﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using System.Reflection;

/*
Source code copyright 2018, by Michael Billard (Angel-125)
License: GPLV3

Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class WBIKISAddonConfig
    {
        static Type typeKISAddonConfig;
        static MethodInfo miAddPodInventories;

        delegate void AddPodInventoriesDelegate(Part part, int crewCapacity);
        static AddPodInventoriesDelegate addInventories = null;

        public static void InitClass(Assembly kisAssembly)
        {
            typeKISAddonConfig = kisAssembly.GetTypes().Where(t => t.Name.Equals("KISAddonConfig")).FirstOrDefault();
            if (typeKISAddonConfig == null)
            {
                return;
            }
            miAddPodInventories = typeKISAddonConfig.GetMethod("AddPodInventories", BindingFlags.Public | BindingFlags.Static);
            if (miAddPodInventories == null)
            {
                return;
            }

            addInventories = (AddPodInventoriesDelegate)Delegate.CreateDelegate(typeof(AddPodInventoriesDelegate), null, miAddPodInventories);
        }

        public static void AddPodInventories(Part toPart, int crewCapacity)
        {
            if (addInventories != null)
                addInventories(toPart, crewCapacity);
        }
    }
}