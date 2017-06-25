﻿using AT.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AT.Bridge
{
    public interface TestsBridge
    {
        //Add Here The UC Functions to test
        //Example:
        int testCase1(int someParam);
        string testCase2(string someParam);
		IList<Card> ShuffleCards(Deck TestDeck);
		bool Login( string UserName, string Password);
		string SignUp(string Name, string UserName, string Password);
		void TearDown();
		bool Logout(string UserName, string Password);
		void EditProfilePassword(User User, string Password);
		void EditProfileEmail(User User, string Email);
		Image EditProfileAvatar(Image TestUserImage);
	}
}
