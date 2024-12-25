using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridSim.Model
{
    public class Element
    {
        public ElementTypes ElementType;

        public Element(ElementTypes elementType)
        {
            this.ElementType = elementType;
        }
    }
    public enum ElementTypes
    {
        Fire,
        Victim,
        Hazard,
        Smoke,
        FireFighter
    }
}

