﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser
{
    public partial class FunctionTableSource : ITableSource
    {
        public TableValuedFunctionCall FunctionCall
        {
            get { return FindDescendant<TableValuedFunctionCall>(); }
        }

        public TableReference TableReference
        {
            get { return FunctionCall.TableReference; }
            set { FunctionCall.TableReference = value; }
        }

        public override Node Interpret()
        {
            var node = (FunctionTableSource)base.Interpret();

            node.TableReference.InterpretTableSource(this);

            return node;
        }
    }
}
