﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.BE.Domain.Core
{
    public abstract class AbstractUser
    {
        // TODO: for Idan

        #region Properties
        protected string UserName { get; set;}
        protected string Password { get; set; }
        #endregion

    }
}
