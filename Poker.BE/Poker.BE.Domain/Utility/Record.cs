﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.BE.Domain.Utility
{
    public class Record
    {
        #region Properties
        private bool isFavourite { get; set; }
        #endregion

        #region Constructors
        public Record()
        {
            isFavourite = false;
        }
        #endregion
    }
}
