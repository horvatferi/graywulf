﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    /// <summary>
    /// References a 
    /// </summary>
    public class SearchConditionReference
    {
        public enum ConditionType
        {
            Unknown,
            Where,
            Having,
            JoinOn
        }

        private ConditionType type;
        private List<TableReference> tableReferences;
        private Node node;

        public ConditionType Type
        {
            get { return type; }
            internal set { type = value; }
        }

        public List<TableReference> TableReferences
        {
            get { return tableReferences; }
        }

        public Node Node
        {
            get { return node; }
        }

        /* TODO: delete
        public string Code
        {
            get
            {
                return (node == null) ? String.Empty : node.GetCode(false);
            }
        }*/

        public SearchConditionReference()
        {
            InitializeMembers();
        }

        public SearchConditionReference(Predicate node)
        {
            InitializeMembers();
            UpdateFromNode((Node)node);
        }

        public SearchConditionReference(SearchCondition node)
        {
            InitializeMembers();
            UpdateFromNode((Node)node);
        }

        public SearchConditionReference(SearchConditionBrackets node)
        {
            InitializeMembers();
            UpdateFromNode((Node)node);
        }

        private void InitializeMembers()
        {
            this.type = ConditionType.Unknown;
            this.tableReferences = new List<TableReference>();
            this.node = null;
        }

        private void UpdateFromNode(Node node)
        {
            this.node = node;

            this.tableReferences.Clear();
            this.tableReferences.AddRange(node.EnumerateTableReferences());
        }
    }
}
