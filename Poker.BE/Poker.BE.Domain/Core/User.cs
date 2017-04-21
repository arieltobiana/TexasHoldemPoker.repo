﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.BE.Domain.Core
{
    public class User : AbstractUser
    {
        #region Methods
        public  User (string userName, string password, double sumToDeposit){
            UserName = userName;
            Password = password;
            UserBank = new Bank (sumToDeposit);
            IsConnected = true;
        }

        public void Connect(){
            IsConnected = true;
        }

        public void Disconnect(){
            IsConnected = false;
        }

        #endregion   
    } 
}
