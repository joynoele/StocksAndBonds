using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models.Players
{
    interface IIndexAi : IAiPlayer
    {
        Security IndexSecurity { get; }
        void PrintStatus();
    }
}
