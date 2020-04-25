using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models.Players
{
    public interface IAiPlayer : IPlayer
    {
        // methods that make all the buy/sell choices for this parciular AI
        void TakeTurn(IList<Asset> securities, int year);
        // action taken to ingest information but no buy/sell happens
        void Observe(IList<Asset> securities, int year); 
    }
}
