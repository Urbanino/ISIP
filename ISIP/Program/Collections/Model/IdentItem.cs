using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ISIP.Program.Collections.Model
{
    class IdentItem : ComboBoxItem
    {
        int ID;

        public IdentItem(int ID, string Content)
        {
            this.ID = ID;
            this.Content = Content;
        }
    }
}
