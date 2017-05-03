﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT.Domain;

namespace AT.Bridge
{
    class Stub : TestsBridge
    {
		public bool Login(string UserName, string Password)
		{
			return false;
		}

		public IList<Card> ShuffleCards(Deck TestDeck)
		{
			return new List<Card>();
		}

		public User SignUp(string Name, string UserName, string Password)
		{
			return new User(Name, UserName, Password);
		}

		public int testCase1(int someParam)
        {
            return -1;
        }

        public string testCase2(string someParam)
        {
            return "FAKE_HERE";
        }
    }
}
