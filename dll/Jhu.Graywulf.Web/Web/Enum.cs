﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jhu.Graywulf.Web
{
    public enum JobType
    {
        Unknown,
        Query,
        ExportTable
    }

    public enum DockingStyle
    {
        None,
        Top,
        Left,
        Bottom,
        Right,
        Fill
    }
}