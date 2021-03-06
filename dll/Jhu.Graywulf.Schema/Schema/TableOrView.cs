﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Contains information about a database table
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class TableOrView : DatabaseObject, IColumns, IIndexes, ICloneable
    {
        #region Property storage members and private variables

        private LazyProperty<ConcurrentDictionary<string, Column>> columns;
        private LazyProperty<ConcurrentDictionary<string, Index>> indexes;
        private LazyProperty<TableStatistics> statistics;


        #endregion
        #region Properties

        /// <summary>
        /// Gets the collection of columns
        /// </summary>
        [IgnoreDataMember]
        public ConcurrentDictionary<string, Column> Columns
        {
            get { return columns.Value; }
        }

        /// <summary>
        /// Gets the collection of indexes
        /// </summary>
        [IgnoreDataMember]
        public ConcurrentDictionary<string, Index> Indexes
        {
            get { return indexes.Value; }
        }

        [IgnoreDataMember]
        public TableStatistics Statistics
        {
            get { return statistics.Value; }
        }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public TableOrView()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a table or view and initializes its dataset
        /// </summary>
        /// <param name="dataset"></param>
        public TableOrView(DatasetBase dataset)
            : base(dataset)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public TableOrView(TableOrView old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their default values
        /// </summary>
        private void InitializeMembers()
        {
            this.ObjectType = DatabaseObjectType.Unknown;

            this.columns = new LazyProperty<ConcurrentDictionary<string, Column>>(LoadColumns);
            this.indexes = new LazyProperty<ConcurrentDictionary<string, Index>>(LoadIndexes);
            this.statistics = new LazyProperty<TableStatistics>(LoadStatistics);
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(TableOrView old)
        {
            this.ObjectType = old.ObjectType;

            this.columns = new LazyProperty<ConcurrentDictionary<string, Column>>(LoadColumns);
            this.indexes = new LazyProperty<ConcurrentDictionary<string, Index>>(LoadIndexes);
            this.statistics = new LazyProperty<TableStatistics>(LoadStatistics);
        }

        public abstract object Clone();

        private TableStatistics LoadStatistics()
        {
            if (Dataset != null)
            {
                return Dataset.LoadTableStatistics(this);
            }
            else
            {
                return new TableStatistics();
            }
        }
    }
}
