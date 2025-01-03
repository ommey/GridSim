using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using GridSim.Model;


namespace GridSim.ViewModel
{
    public class ElementViewModel : BaseViewModel
    {
        private Element elementModel;

        private ElementTypes elementType => elementModel.ElementType;

        public ElementTypes ElementType
        {
            get { return elementType; }
            set
            {
                elementModel.ElementType = value;
                ElementImage = getElementImage();
                OnPropertyChanged();
            }
        }

        private string getElementImage()
        {
            
            switch (elementType)
            {
                case ElementTypes.Fire:
                    return "/Resources/Fire.gif";
                case ElementTypes.Victim:
                    return "/Resources/Victim.gif";
                case ElementTypes.Hazard:
                    return "/Resources/Hazard.gif";
                case ElementTypes.Smoke:
                    return "/Resources/Smoke.gif";
                case ElementTypes.FireFighter:
                    return "/Resources/FireFighterWalk.gif";
                default:
                    return string.Empty;
                    //throw new ArgumentOutOfRangeException("finns inte walla");
            }
        }

        private string elementImage = string.Empty;
        public string ElementImage
        {
            get { return elementImage; }
            set
            {
                elementImage = value;
                OnPropertyChanged();
            }
        }

        private int y;

        public int Y
        {
            get { return y; }
            set {
                y = value;
                OnPropertyChanged();
            }
        }

        private int x;

        public int X
        {
            get { return x; }
            set {
                x = value; 
                OnPropertyChanged();
            }
        }

        private int elementWidth;
        public int ElementWidth
        {
            get { return elementWidth; }
            set
            {
                elementWidth = value;
                OnPropertyChanged();
            }
        }

        private int elementHeight;
        public int ElementHeight
        {
            get { return elementHeight; }
            set
            {
                elementHeight = value;
                OnPropertyChanged();
            }
        }

        
        public ElementViewModel(ElementTypes elementType , int canvasX, int canvasY)
        {
            this.elementModel = new Element(elementType);
            ElementImage = getElementImage();
            getElementDimensions(elementType);

            this.X = canvasX - (ElementWidth / 2) + 20;
            this.Y = canvasY - ElementHeight / 2;

            
            //ElementWidth = 100;
            //ElementHeight = 100;
        }

        private void getElementDimensions(ElementTypes elementType)
        {
            switch (elementType) 
            {
                case ElementTypes.Fire:
                    ElementWidth = 50;
                    ElementHeight = 50;
                    break;
                case ElementTypes.Victim:
                    ElementWidth = 50;
                    ElementHeight = 50;
                    break;
                case ElementTypes.Hazard:
                    ElementWidth = 50;
                    ElementHeight = 50;
                    break;
                case ElementTypes.Smoke:
                    ElementWidth = 50;
                    ElementHeight = 50;
                    break;
                case ElementTypes.FireFighter:
                    ElementWidth = 50;
                    ElementHeight = 75;
                    break;

            }

        }

        public void changePosition(int newX, int newY)
        {
            X = newX - (ElementWidth / 2) + 20;
            Y = newY - ElementHeight / 2;
        }

    }

    public class FireFighterViewModel : ElementViewModel
    {
        public FireFighterViewModel(String id, int canvasX,int canvasY) : base(ElementTypes.FireFighter, canvasX, canvasY)
        {
            ElementImage = "/Resources/FireFighterWalk.gif";
        }
    }
}
