﻿/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Slice</b> entity.
    /// </summary>
    public partial class Slice : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private long from;
        private long to;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Lower limit of the interval of the <b>Partitioning Column</b> values belonging to this slice.
        /// </summary>
        [DBColumn]
        public long From
        {
            get { return from; }
            set { from = value; }
        }

        /// <summary>
        /// Upper limit of the interval of the <b>Partitioning Column</b> values belonging to this slice.
        /// </summary>
        [DBColumn]
        public long To
        {
            get { return to; }
            set { to = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Database Definition</b> object to which this <b>Slice</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public DatabaseDefinition DatabaseDefinition
        {
            get
            {
                return (DatabaseDefinition)ParentReference.Value;
            }
        }

        [XmlIgnore]
        public Dictionary<string, Partition> Partitions
        {
            get { return GetChildren<Partition>(); }
            set { SetChildren<Partition>(value); }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public Slice()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Slice</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public Slice(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public Slice(DatabaseDefinition parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Slice</b> objects.
        /// </summary>
        /// <param name="old">The <b>Slice</b> to copy from.</param>
        public Slice(Slice old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        private void InitializeMembers()
        {
            base.EntityType = EntityType.Slice;
            base.EntityGroup = EntityGroup.Federation;

            this.from = 0;
            this.to = 0;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Slice</b> object to create the deep copy from.</param>
        private void CopyMembers(Slice old)
        {
            this.from = old.from;
            this.to = old.to;
        }

        public override object Clone()
        {
            return new Slice(this);
        }

        protected override Type[] CreateChildTypes()
        {
            return new Type[]
            {
                typeof(Partition),
            };
        }

        #endregion

        /// <summary>
        /// Generates the partitions for a slice. Partition names and limits should be supplied.
        /// </summary>
        /// <param name="partitionNames">Array containing the names of the partitions.</param>
        /// <param name="partitionLimits">Two dimensional array containing the limits
        /// of the partitions.</param>
        /// <returns>A list of newly generated partition entities.</returns>
        /// <remarks>
        /// The length of the <paramref name="partitionNames"/> and the length of the first dimension
        /// of the <paramref name="partitionLimits"/> parameter must be the same. The <paramref name="partitionLimits"/>
        /// array must have the second dimension of the size of 2.
        /// </remarks>
        public List<Partition> GeneratePartitions(string[] partitionNames, long[][] partitionLimits)
        {
            List<Partition> partitions = new List<Partition>();

            for (int pi = 0; pi < partitionNames.Length; pi++)
            {
                Partition np = new Partition(this);

                np.Name = partitionNames[pi];
                np.From = partitionLimits[pi][0];
                np.To = partitionLimits[pi][1];
                np.Save();

                partitions.Add(np);
            }

            return partitions;
        }
    }
}
