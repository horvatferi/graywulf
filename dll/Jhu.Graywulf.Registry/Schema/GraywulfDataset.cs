﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class GraywulfDataset : Schema.SqlServer.SqlServerDataset, ICloneable
    {
        [NonSerialized]
        private Context context;

        [NonSerialized]
        private EntityProperty<DatabaseDefinition> databaseDefinition;

        [NonSerialized]
        private EntityProperty<DatabaseVersion> databaseVersion;

        [NonSerialized]
        private EntityProperty<DatabaseInstance> databaseInstance;

        private string connectionStringCache;

        public Context Context
        {
            get { return context; }
            set
            {
                context = value;
                UpdateContext();
            }
        }

        [DataMember]
        public EntityProperty<DatabaseDefinition> DatabaseDefinition
        {
            get { return databaseDefinition; }
            set { databaseDefinition = value; }
        }

        [DataMember]
        public EntityProperty<DatabaseVersion> DatabaseVersion
        {
            get { return databaseVersion; }
            set { databaseVersion = value; }
        }

        [DataMember]
        public EntityProperty<DatabaseInstance> DatabaseInstance
        {
            get { return databaseInstance; }
            set { databaseInstance = value; }
        }

        [IgnoreDataMember]
        public override string ConnectionString
        {
            get
            {
                if (connectionStringCache == null)
                {
                    CacheSchemaConnectionString();
                }

                return connectionStringCache;
            }
            set
            {
                connectionStringCache = value;
                //throw new InvalidOperationException("Connection string of graywulf datasets cannot be set directly."); // TODO
            }
        }

        [IgnoreDataMember]
        public bool IsSpecificInstanceRequired
        {
            get { return !databaseInstance.IsEmpty; }
        }

        public GraywulfDataset()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public GraywulfDataset(DatasetBase old)
            : base(old)
        {
            InitializeMembers(new StreamingContext());
        }

        public GraywulfDataset(GraywulfDataset old)
            : base(old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.context = null;

            this.databaseDefinition = new EntityProperty<DatabaseDefinition>();
            this.databaseVersion = new EntityProperty<DatabaseVersion>();
            this.databaseInstance = new EntityProperty<DatabaseInstance>();

            this.connectionStringCache = null;
        }

        private void CopyMembers(GraywulfDataset old)
        {
            this.context = old.context;

            this.databaseDefinition = new EntityProperty<DatabaseDefinition>(old.databaseDefinition);
            this.databaseVersion = new EntityProperty<DatabaseVersion>(old.databaseVersion);
            this.databaseInstance = new EntityProperty<DatabaseInstance>(old.databaseInstance);

            this.connectionStringCache = null;
        }

        private void UpdateContext()
        {
            this.databaseDefinition.Context = context;
            this.databaseVersion.Context = context;
            this.databaseInstance.Context = context;
        }

        public void CacheSchemaConnectionString()
        {
            if (!databaseInstance.IsEmpty)
            {
                connectionStringCache = databaseInstance.Value.GetConnectionString().ConnectionString;
            }
            else if (!databaseVersion.IsEmpty)
            {
                connectionStringCache = databaseVersion.Value.DatabaseDefinition.GetConnectionString().ConnectionString;
            }
            else
            {
                connectionStringCache = databaseDefinition.Value.GetConnectionString().ConnectionString;
            }
        }
    }
}
