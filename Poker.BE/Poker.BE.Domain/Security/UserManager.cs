﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poker.BE.Domain.Core;
using Poker.BE.Domain.Utility.Exceptions;

namespace Poker.BE.Domain.Security
{
	public class UserManager
	{
		#region Properties
		protected Dictionary<string, User> UsersDictionary;

		#endregion

		#region Methods
		public UserManager()
		{
			UsersDictionary = new Dictionary<string, User>();
		}

		public User AddUser(string userName, string password, double sumToDeposit)
		{
			if (CheckExistingUser(userName))
				throw new UserNameTakenException();
			if (!CheckPasswordValidity(password))
				throw new InvalidPasswordException();
			if (sumToDeposit < 0)
				throw new InvalidDepositException();
			User UserToAdd = new User(userName, password, sumToDeposit);
			UsersDictionary.Add(userName, UserToAdd);
			return UserToAdd;
		}

		protected bool RemoveUser(string userName)
		{
			if (CheckExistingUser(userName))
			{
				UsersDictionary.Remove(userName);
				return true;
			}
			return false;
		}

		protected bool CheckExistingUser(string userName)
		{
			if (userName != null)
				return (UsersDictionary.ContainsKey(userName));
			return false;
		}


		protected bool CheckPasswordValidity(string password)
		{
			if (password.Length >= 6) return true;
			return false;
		}

		public User LogIn(string userName, string password)
		{
			if (!CheckExistingUser(userName)) // We check that the user is existing in our DB
				throw new UserNotFoundException();
			User UserToCheck;
			if (UsersDictionary.TryGetValue(userName, out UserToCheck))
			{ // We take the User Object from our DB
				string GoodPassword = UserToCheck.Password;
				bool arePasswordMatching = GoodPassword.Equals(password, StringComparison.Ordinal); // We check that the password is correct
				if (!arePasswordMatching)
				{
					throw new IncorrectPasswordException();
				}
				UserToCheck.Connect();
				return UserToCheck;
			}
			return null;
		}

		public bool LogOut(User userToLogout)
		{
			if (!CheckExistingUser(userToLogout.UserName)) // We check that the user is existing in our DB
				throw new UserNotFoundException();
			userToLogout.Disconnect();
			return true;
		}


		#endregion

	}
}
