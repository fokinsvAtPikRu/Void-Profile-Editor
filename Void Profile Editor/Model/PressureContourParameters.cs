using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Model
{
    public class PressureContourParameters
    {
        public Dictionary<string, double> DoubleParameters { get; set; }
        public Dictionary<string,int> IntParameters { get; set; }
        public PressureContourParameters() 
        {
            DoubleParameters = new Dictionary<string, double>();
            DoubleParameters.Add("h0", 0.0);
            DoubleParameters.Add("Толщина", 0.0);
            DoubleParameters.Add("Ст1.отступ линии от Ст2", 0.0);
            DoubleParameters.Add("Ст1.отступ линии от Ст4", 0.0);
            DoubleParameters.Add("Ст1.смещение отверстия от Ст4", 0.0);
            DoubleParameters.Add("Ст1.ширина отверстия", 0.0);
            DoubleParameters.Add("Ст2.отступ линии от Ст1", 0.0);
            DoubleParameters.Add("Ст2.отступ линии от Ст3", 0.0);
            DoubleParameters.Add("Ст2.смещение отверстия от Ст1", 0.0);
            DoubleParameters.Add("Ст2.ширина отверстия", 0.0);
            DoubleParameters.Add("Ст3.отступ линии от Ст2", 0.0);
            DoubleParameters.Add("Ст3.отступ линии от Ст4", 0.0);
            DoubleParameters.Add("Ст3.смещение отверстия от Ст4", 0.0);
            DoubleParameters.Add("Ст3.ширина отверстия", 0.0);
            IntParameters = new Dictionary<string, int>();
            IntParameters.Add("Вкл редактирование контура", 1);
            IntParameters.Add("Вкл сторона 1", 1);
            IntParameters.Add("Вкл сторона 2", 1);
            IntParameters.Add("Вкл сторона 3", 1);
        }
    }
}
