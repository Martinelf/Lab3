using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PluginBase
{
    public interface IPlugin
    {
        string Name { get; }
        void Initialize(Control container, PictureBox pictureBox);
    }
}
