using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Uspator.Model
{
    public class Player
    {
        [JsonProperty("Name")]
        public string name { get; set; }
        
        [JsonProperty("x")]
        public int X { get; set; }
        
        [JsonProperty("y")]
        public int Y { get; set; }
        
        [JsonProperty("z")]
        public int Z { get; set; }

        public Player ApplyMove(string direction)
        {
            var clone = (Player)this.MemberwiseClone();
            switch (direction)
            {
                case "-X":
                    clone.X--;
                    break;
                case "+X":
                    clone.X++;
                    break;
                case "-Y":
                    clone.Y--;
                    break;
                case "+Y":
                    clone.Y++;
                    break;
                case "-Z":
                    clone.Z--;
                    break;
                case "+Z":
                    clone.Z++;
                    break;
                default:
                    break;
            }
            
            return clone;
        }

        public string GetPositionString()
        {
            return $"{X}{Y}{Z}";
        }

        public int GetProximity(Player p)
        {
            return Math.Abs(X - p.X) + 
                   Math.Abs(Y - p.Y) +
                   Math.Abs(Z - p.Z);
        }

        public string GetMostFarAxis(Player p)
        {
            var x = Math.Abs(X - p.X);
            var y = Math.Abs(Y - p.Y);
            var z = Math.Abs(Y - p.Y);

            var distances = new List<int>() { x, y, z };
            var max = distances.Max();

            if (x == max)
            {
                return "X";
            }
            else if (y == max)
            {
                return "Y";
            }
            else if (z == max)
            {
                return "Z";
            }
            else
            {
                return "X";
            }
        }
        
        public string GetClosestAxis(Player p)
        {
            var x = Math.Abs(X - p.X);
            var y = Math.Abs(Y - p.Y);
            var z = Math.Abs(Y - p.Y);

            var distances = new List<int>() { x, y, z };
            var min = distances.Min();

            if (x == min)
            {
                return "X";
            }
            else if (y == min)
            {
                return "Y";
            }
            else if (z == min)
            {
                return "Z";
            }
            else
            {
                return "X";
            }
        }

        public string FindPositionDelta(string position)
        {
            var c = position[0].ToString();
            var ic = Convert.ToInt16(c);
            if (ic != X)
            {
                return ic > X ? "+X" : "-X";
            }

            c = position[1].ToString();
            ic = Convert.ToInt16(c);
            if (ic != Y)
            {
                return ic > Y ? "+Y" : "-Y";
            }
            
            c = position[2].ToString();
            ic = Convert.ToInt16(c);
            if (ic != Z)
            {
                return ic > Z ? "+Z" : "-Z";
            }

            return "";
        }
    }
}