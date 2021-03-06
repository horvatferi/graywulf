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
    public partial class UserDatabaseInstance : Entity
    {
        public enum ReferenceType : int
        {
            DatabaseVersion = 1,
            DatabaseInstance = 2
        }

        #region Member Variables


        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public DatabaseVersion DatabaseVersion
        {
            get { return DatabaseVersionReference.Value; }
            set { DatabaseVersionReference.Value = value; }
        }

        [XmlIgnore]
        public DatabaseInstance DatabaseInstance
        {
            get { return DatabaseInstanceReference.Value; }
            set { DatabaseInstanceReference.Value = value; }
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
        public User User
        {
            get
            {
                return (User)ParentReference.Value;
            }
        }

        [XmlIgnore]
        public EntityReference<DatabaseVersion> DatabaseVersionReference
        {
            get { return (EntityReference<DatabaseVersion>)EntityReferences[(int)ReferenceType.DatabaseVersion]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("DatabaseVersion")]
        public string DatabaseVersion_ForXml
        {
            get { return DatabaseVersionReference.Name; }
            set { DatabaseVersionReference.Name = value; }
        }

        [XmlIgnore]
        public EntityReference<DatabaseInstance> DatabaseInstanceReference
        {
            get { return (EntityReference<DatabaseInstance>)EntityReferences[(int)ReferenceType.DatabaseInstance]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("DatabaseInstance")]
        public string DatabaseInstance_ForXml
        {
            get { return DatabaseInstanceReference.Name; }
            set { DatabaseInstanceReference.Name = value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public UserDatabaseInstance()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Slice</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public UserDatabaseInstance(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public UserDatabaseInstance(User parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Slice</b> objects.
        /// </summary>
        /// <param name="old">The <b>Slice</b> to copy from.</param>
        public UserDatabaseInstance(UserDatabaseInstance old)
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
            base.EntityType = EntityType.UserDatabaseInstance;
            base.EntityGroup = EntityGroup.Layout;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Slice</b> object to create the deep copy from.</param>
        private void CopyMembers(UserDatabaseInstance old)
        {

        }

        public override object Clone()
        {
            return new UserDatabaseInstance(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<DatabaseVersion>((int)ReferenceType.DatabaseVersion),
                new EntityReference<DatabaseInstance>((int)ReferenceType.DatabaseInstance),
            };
        }

        #endregion
    }
}
