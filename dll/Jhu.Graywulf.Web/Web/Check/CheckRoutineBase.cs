﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Web;

namespace Jhu.Graywulf.Web.Check
{
    public abstract class CheckRoutineBase
    {
        public abstract void Execute(PageBase page);
    }
}
