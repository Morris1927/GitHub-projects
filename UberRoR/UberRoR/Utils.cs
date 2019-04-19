using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UberRoR {
    class Utils {

        public static void PrintFields(Type type, object instance) {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            List<string> list = new List<string>();
            string listString = "";

            list.Add("============================================================================================");
            foreach (var item in fields) {
                list.Add(string.Format("{0}: {1}", item.Name, item.GetValue(instance)));
            }
            list.Add("============================================================================================");
            listString = string.Join("\n", list.ToArray());
            Debug.Log(listString);
        }
    }
}
