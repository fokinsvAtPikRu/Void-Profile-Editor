using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Void_Profile_Editor.Model
{
    /// <summary>
    /// Модель данных для узла с усилиями N, Mx, My
    /// </summary>
    public partial class NodeForceData : ObservableObject
    {
        [ObservableProperty]
        private string _nodeName;

        [ObservableProperty]
        private double _n;

        [ObservableProperty]
        private double _mx;

        [ObservableProperty]
        private double _my;

        /// <summary>
        /// Проверка наличия валидных значений
        /// </summary>
        public bool HasValidValues
        {
            get
            {
                return !double.IsNaN(N) &&
                       !double.IsInfinity(N) &&
                       !double.IsNaN(Mx) &&
                       !double.IsInfinity(Mx) &&
                       !double.IsNaN(My) &&
                       !double.IsInfinity(My);
            }
        }

        public NodeForceData()
        {
            NodeName = "Узел";
            N = 0;
            Mx = 0;
            My = 0;
        }

        public NodeForceData(string nodeName, double n, double mx, double my)
        {
            NodeName = nodeName;
            N = n;
            Mx = mx;
            My = my;
        }

        public void Reset()
        {
            N = 0;
            Mx = 0;
            My = 0;
        }

        public override string ToString()
        {
            return $"{NodeName}: N={N:F2} кН, Mx={Mx:F2} кН·м, My={My:F2} кН·м";
        }

        public NodeForceData Clone()
        {
            return new NodeForceData
            {
                NodeName = this.NodeName,
                N = this.N,
                Mx = this.Mx,
                My = this.My
            };
        }
    }
}