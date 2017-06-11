﻿using Poker.BE.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Poker.BE.Data.Repositories
{
    public class UserRepository : IRepository<UserEntity>
    {
        public UserRepository(MainContext context) : base(context)
        {
        }

        public MainContext MainContext { get { return Context as MainContext; } }
    }
}
