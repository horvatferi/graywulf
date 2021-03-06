﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class Domain : Entity
    {
        #region Database IO Functions

        public void LoadFederations(bool forceReload)
        {
            LoadChildren<Federation>(forceReload);
        }

        public void LoadUserGroups(bool forceReload)
        {
            LoadChildren<UserGroup>(forceReload);
        }

        #endregion
    }
}
