using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models.Players
{
    public interface IIndexAi : IAiPlayer
    {
        Security IndexSecurity { get; }
    }
}
