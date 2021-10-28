using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Media.SystemSounds;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
//using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System;


namespace GED
{
    [Serializable]
    class Data_for_serialize
    {
        double x1;
        double x2;
        double y1;
        double y2;
        Brush ST;
        double StrokeT;
        object TG;
        Type type;
        bool morf;
        int num_morf;
        int count_morf;
        bool group;
        int number_group;

        public double X1
        {
            get { return x1; }
            set { x1 = value; }
        }
        public double X2
        {
            get { return x2; }
            set { x2 = value; }
        }
        public double Y1
        {
            get { return y1; }
            set { y1 = value; }
        }
        public double Y2
        {
            get { return y2; }
            set { y2 = value; }
        }
        public Brush Stroke
        {
            get { return ST; }
            set { ST = value; }
        }
        public double StrokeThickness
        {
            get { return StrokeT; }
            set { StrokeT = value; }
        }
        public object Tag
        {
            get { return TG; }
            set { TG = value; }
        }
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }
        public bool Morf
        {
            get { return morf; }
            set { morf = value; }
        }
        public int Num_morf
        {
            get { return num_morf; }
            set { num_morf = value; }
        }
        public int Count_morf
        {
            get { return count_morf; }
            set { count_morf = value; }
        }
        public bool Group
        {
            get { return group; }
            set { group = value; }
        }
        public int Number_group
        {
            get { return number_group; }
            set { number_group = value; }
        }
    }
}
