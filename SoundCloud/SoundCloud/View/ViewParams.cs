using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoundCloud.Data;
using SoundCloud.Interfaces;

namespace SoundCloud.View
{
    public class ViewParams
    {
        public IDataManager DataManager { get; set; }
        public IApplicationController Controller { get; set; }
    }
}
