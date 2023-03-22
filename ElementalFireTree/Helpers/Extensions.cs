﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SRML.Console.Console;
using ElementalFireTree;
using UnityEngine;

namespace ElementalFireTree
{
    public static class Extensions
    {
        public static System.Random rnd = new System.Random();
        public static Console.ConsoleInstance console =  new Console.ConsoleInstance("ElementalFireTree");
        public static void Log(this string message) => console.Log(message);

        public static void PrintContent(this Shader shader, Material material = null)
        {
            "".Log();
            "".Log();

            if (material == null)
                ("Printing Shader: " + shader.name).Log();
            if (material != null)
                ("Printing Shader: " + shader.name + " of Material: " + material).Log();

            for (int i2 = 0; i2 < shader.GetPropertyCount(); i2++)
            {
                if (material != null)
                    (shader.GetPropertyName(i2) + ", " + shader.GetPropertyDescription(i2) + " : " + shader.GetPropertyType(i2) + " = " + material.GetType().GetMethod("Get" + shader.GetPropertyType(i2))).Log();
                if (material == null)
                    (shader.GetPropertyName(i2) + ", " + shader.GetPropertyDescription(i2) + " : " + shader.GetPropertyType(i2)).Log();
                foreach (string str in shader.GetPropertyAttributes(i2))
                {
                    ("   " + str).Log();
                }
            }

            "".Log();
            "".Log();

        }





    }
}