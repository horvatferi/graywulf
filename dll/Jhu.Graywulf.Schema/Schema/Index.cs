﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Reflects a database table or view index
    /// </summary>
    [Serializable]
    public class Index : DatabaseObject, ICloneable
    {
        private TableOrView tableOrView;
        private int indexId;
        private bool isPrimaryKey;
        private bool isClustered;
        private bool isUnique;

        private Lazy<ConcurrentDictionary<string, IndexColumn>> columns;

        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        public string IndexName
        {
            get { return ObjectName; }
            set { ObjectName = value; }
        }

        /// <summary>
        /// Gets or sets the unique ID of the index
        /// </summary>
        public int IndexId
        {
            get { return indexId; }
            set { indexId = value; }
        }

        /// <summary>
        /// Gets or sets the value indication whether the index is the primary key of a table
        /// </summary>
        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
            set { isPrimaryKey = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the index is clustered.
        /// </summary>
        public bool IsClustered
        {
            get { return isClustered; }
            set { isClustered = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether key of the index are unique.
        /// </summary>
        public bool IsUnique
        {
            get { return isUnique; }
            set { isUnique = value; }
        }

        /// <summary>
        /// Gets the collection of columns.
        /// </summary>
        public ConcurrentDictionary<string, IndexColumn> Columns
        {
            get { return columns.Value; }
        }

        public string ColumnListDisplayString
        {
            get
            {
                int q = 0;
                var cols = "";
                foreach (var c in Columns.Values.OrderBy(i => i.KeyOrdinal))
                {
                    if (q > 0)
                    {
                        cols += ", ";
                    }

                    cols += c.Name;
                    if (c.Ordering == IndexColumnOrdering.Descending)
                    {
                        cols += " DESC";
                    }

                    q++;
                }

                return cols;
            }
        }

        /// <summary>
        /// Creates a new index object
        /// </summary>
        public Index()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a new index object and initializes its parent object.
        /// </summary>
        /// <param name="tableOrView"></param>
        public Index(TableOrView tableOrView)
            : base(tableOrView.Dataset)
        {
            InitializeMembers();

            this.tableOrView = tableOrView;
        }

        public Index(Index old)
            :base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their default values
        /// </summary>
        private void InitializeMembers()
        {
            this.ObjectType = DatabaseObjectType.Index;

            this.tableOrView = null;
            this.indexId = -1;
            this.isPrimaryKey = false;
            this.isClustered = false;
            this.isUnique = false;

            this.columns = new Lazy<ConcurrentDictionary<string, IndexColumn>>(this.LoadIndexColumns, true);
        }

        private void CopyMembers(Index old)
        {
            this.ObjectType = old.ObjectType;

            this.tableOrView = old.tableOrView;
            this.indexId = old.indexId;
            this.isPrimaryKey = old.isPrimaryKey;
            this.isClustered = old.isClustered;
            this.isUnique = old.isUnique;

            this.columns = new Lazy<ConcurrentDictionary<string, IndexColumn>>(this.LoadIndexColumns, true);
        }

        /// <summary>
        /// Loads all columns of the index
        /// </summary>
        /// <returns></returns>
        protected ConcurrentDictionary<string, IndexColumn> LoadIndexColumns()
        {
            return Dataset.LoadIndexColumns(this);
        }

        #region ICloneable Members

        /// <summary>
        /// Returns a copy of this index
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Index(this);
        }

        #endregion
    }
}
