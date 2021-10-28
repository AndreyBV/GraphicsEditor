using System;
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

using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Json;



namespace GED
{
    //[Serializable(), XmlInclude(typeof(List<Shape>))]
    [CollectionDataContractAttribute]
    public class Group_shape : IEnumerable, IEnumerator
    {
        [DataMember]
        List<Shape> group_sh = new List<Shape>();
        int pos = -1;
        bool select_ok_c = false;
        bool morf_ok_c = false;
        double pos_slider_c = 0;


        public bool select_ok
        {
            get { return select_ok_c; }
            set { select_ok_c = value; }
        }
        public bool morf_ok
        {
            get { return morf_ok_c; }
            set { morf_ok_c = value; }
        }
        public double pos_slider
        {
            get { return pos_slider_c; }
            set { pos_slider_c = value; }
        }
        public int Count()
        {
            return group_sh.Count();
        }
        public bool Contains(Shape obj)
        {
            return group_sh.Contains(obj);
        }
        public void Add(object obj)
        {
            group_sh.Add(obj as Shape);
        }
        public void Remove(Shape obj)
        {
            group_sh.Remove(obj);
        }
        public void RemoveAt(int ind)
        {
            group_sh.RemoveAt(ind);
        }
        public void Clear()
        {
            group_sh.Clear();
        }
        public Shape this[int i]
        {
            get
            {
                return group_sh[i];
            }
            set
            {
                group_sh[i] = value;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return group_sh.GetEnumerator();
        }
        public bool MoveNext()
        {
            if (pos < group_sh.Count - 1)
            {
                pos++;
                return true;
            }
            else
            {
                return false;
            }
        }

        // Установить указатель (position) перед началом набора.
        public void Reset()
        {
            pos = -1;
        }

        // Получить текущий элемент набора. 
        public object Current
        {
            get { return group_sh[pos]; }
        }
    }
}
