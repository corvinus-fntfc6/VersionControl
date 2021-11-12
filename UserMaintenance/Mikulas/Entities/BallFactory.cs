using Mikulas.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikulas.Entities
{
    public class BallFactory : IToyFactory
    {
        public Color BalColor { get; set; }
        public Toy CreateNew()
        {
            return new Ball();
        }
    }
}
