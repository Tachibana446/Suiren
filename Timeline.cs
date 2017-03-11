using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suiren
{
    interface Timeline
    {
        /// <summary>
        /// タイムラインのロードくらいはみんな実装するほうが良いよね
        /// </summary>
        /// <returns></returns>
        Task LoadTimeline();
    }

}
