using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.API.Model;

namespace VkPlayer
{
    public class NewVKAudio: VKAudio
    {
        public string DurationInMinutes
        {
            get
            {
                return (duration % 60 >9) ? (duration / 60 + ":" + duration % 60) : (duration / 60 + ":0" + duration % 60);
            }
        }
    }
}
