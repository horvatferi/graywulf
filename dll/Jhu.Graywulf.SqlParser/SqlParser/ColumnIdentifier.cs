﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class ColumnIdentifier : ITableReference, IColumnReference
    {
        public static ColumnIdentifier Create(ColumnReference cr)
        {
            var nci = new ColumnIdentifier();
            nci.ColumnReference = cr;

            if (String.IsNullOrEmpty(cr.TableReference.Alias))
            {
                nci.Stack.AddLast(TableName.Create(cr.TableReference.DatabaseObjectName));
            }
            else
            {
                nci.Stack.AddLast(TableName.Create(cr.TableReference.Alias));
            }
            nci.Stack.AddLast(Dot.Create());
            nci.Stack.AddLast(ColumnName.Create(cr.ColumnName));

            return nci;
        }

        private ColumnReference columnReference;

        public ColumnReference ColumnReference
        {
            get { return columnReference; }
            set { columnReference = value; }
        }

        public TableReference TableReference
        {
            get { return columnReference.TableReference; }
            set { columnReference.TableReference = value; }
        }
        
        public ColumnIdentifier()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.columnReference = null;
        }

        public override Node Interpret()
        {
            this.columnReference = ColumnReference.Interpret(this);

            return base.Interpret();
        }

        public override bool AcceptCodeGenerator(CodeGenerator cg)
        {
            return ((SqlCodeGen.SqlCodeGeneratorBase)cg).WriteColumnIdentifier(this);
        }
    }
}
