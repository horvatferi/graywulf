﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Subquery
    {
        public SelectStatement SelectStatement
        {
            get { return FindDescendant<SelectStatement>(); }
        }
    }
}
