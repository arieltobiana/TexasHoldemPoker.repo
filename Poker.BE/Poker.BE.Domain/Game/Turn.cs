﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.BE.Domain.Game
{
    public class Turn
    {
        #region Fields
        private bool CanCheck;
        private bool IsPlayerFold { get; set; }
        public Player CurrentPlayer { get; set; }
        #endregion

        #region Constructors
        public Turn(Player player, bool CanCheck)
        {
            this.CurrentPlayer = player;
            this.CanCheck = CanCheck;
            this.IsPlayerFold = false;
        }
        #endregion

        #region Methods
        public void Check()
        {
            ///  Do noting? 
            ///  <see cref="Round.calculateNextPlayer" />
            ///  and <see cref="Round.PlayMove(Round.Move)" />
            ///  for more information TOMER
        }

        public void Call()
        {
            //TODO
        }

        public void Fold()
        {
            IsPlayerFold = true;
        }

        public void Bet()
        {
            //TODO
        }

        public void Raise()
        {
            //TODO
        }

        public void AllIn()
        {
            //TODO
        }

    }
}
#endregion