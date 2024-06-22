using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    [Serializable]
    public class LocalizedPlot
    {
        public List<List<string>> InitialCGPlot = new();
        public List<string> InitialMiddlePlot = new();
        public List<string> InitialNarration = new();
    }
}