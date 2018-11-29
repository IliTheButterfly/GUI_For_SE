#region pre-script
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace IngameScript
{
    public class Program : MyGridProgram
    {
#endregion
		//To put your code in a PB copy from this comment...
        public Program()
        {

        }

        public void Save()
        {

        }

        public void Main(string argument)
        {
            
        }

        public int Direction 
        {
            get 
            {
                if (Direction == 4) return 1;
                return (Direction ++);
            }
            set;
        }

        class PulseScan
        {
            private double _distance { get; set; }
            private double _size { get; set; }
            
            private double _angle => Math.ASin(_size/_distance);


            public PulseScan(double distance, double size)
            {
                _distance = distance;
                _size = size;
            }

            public List<string> GetScannedObjects()
            {
                
            }



        }

		//to this comment.
#region post-script
    }
}
#endregion