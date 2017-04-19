using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ISIP.Program
{
    public class CollectionItem : ListBoxItem
    {
        public int Modified;

        public CollectionItem(string Content, int Modified)
        {
            this.Content = Content;
            this.Modified = Modified;
        }

        public CollectionItem(string Content)
        {
            this.Content = Content;
        }

        public CollectionItem() { }
    }
}
