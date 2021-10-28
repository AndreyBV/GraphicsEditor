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

namespace GED
{
    [Serializable]
    class Save_shape : IEnumerable, IEnumerator
    {
        List<Data_for_serialize> save = new List<Data_for_serialize>();
        int pos = -1;

        public void Add(object obj)
        {
            save.Add(obj as Data_for_serialize);
        }
        public void Clear()
        {
            save.Clear();
        }
        public int Count()
        {
            return save.Count();
        }
        public Data_for_serialize this[int i]
        {
            get
            {
                return save[i];
            }
            set
            {
                save[i] = value;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return save.GetEnumerator();
        }
        public bool MoveNext()
        {
            if (pos < save.Count - 1)
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
            get { return save[pos]; }
        }

    }
}
