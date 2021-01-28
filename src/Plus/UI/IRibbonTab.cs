﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.CadPlus.Plus.UI
{
    public interface IRibbonTab
    {
        string Name { get; }
        string Title { get; }
        List<IRibbonGroup> Groups { get; }
    }

    public class RibbonTab : IRibbonTab
    {
        public string Name { get; }
        public string Title { get; }
        public List<IRibbonGroup> Groups { get; }

        public RibbonTab(string name, string title) 
        {
            Name = name;
            Title = title;
            Groups = new List<IRibbonGroup>();
        }
    }
}