using System;
using System.Collections.Generic;
using DXSample11.Model;

namespace DXSample11.ViewModel
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            Items = new List<Dummy>();
            var r = new Random();
            for (var i = 1; i <= 10; i++)
            {
                var x = r.Next(3);
                Items.Add(new Dummy() { Id = i, LinkedId = x == 0 ? (int?) null : x });
            }          
        }

        public ICollection<Dummy> Items { get; }
    }
}