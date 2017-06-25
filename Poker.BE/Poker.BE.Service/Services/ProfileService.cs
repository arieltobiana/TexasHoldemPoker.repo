﻿using Poker.BE.CrossUtility.Exceptions;
using Poker.BE.CrossUtility.Loggers;
using Poker.BE.Domain.Core;
using Poker.BE.Domain.Security;
using Poker.BE.Service.Modules.Caches;
using Poker.BE.Service.Modules.Requests;
using Poker.BE.Service.Modules.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.BE.Service.Services
{
	public class ProfileService : IServices.IProfileService
	{
		#region Fields
		private CommonCache _cache;
		#endregion

		#region properties
		public IDictionary<string, User> Users { get { return _cache.Users; } }
		public ILogger Logger { get { return CrossUtility.Loggers.Logger.Instance; } }
		public UserManager UserManager { get { return _cache.UserManager; } }

		#endregion

		#region Constructors
		public ProfileService()
		{
			_cache = CommonCache.Instance;
		}

		public void Clear()
		{
			Users.Clear();
			UserManager.Clear();
		}
		#endregion

		public EditProfileResult EditProfile(EditProfileRequest request)
		{
			var result = new EditProfileResult();
			result.newUserName = request.UserName;
			try
			{
				UserManager.EditProfile(request.UserName, request.NewUserName ?? request.UserName, request.NewPassword, request.NewAvatar);
				result.newAvatar = request.NewAvatar;
				result.newPassword = request.NewPassword;
				result.newUserName = request.NewUserName;
				result.Success = true;
			}
			catch (UserNotFoundException e)
			{
				result.Success = false;
				result.ErrorMessage = e.Message;
				Logger.Log(e.Message, this);

			}
			catch (IncorrectPasswordException e)
			{
				result.Success = false;
				result.ErrorMessage = e.Message;
				Logger.Log(e.Message, this);
			}
			catch (UserNameTakenException e)
			{
				result.Success = false;
				result.ErrorMessage = e.Message;
				Logger.Log(e.Message, this);
			}
			return result;
		}

		public GetProfileResult GetProfile(GetProfileRequest request)
		{
			var result = new GetProfileResult();
			try
			{
				String password;
				byte[] avatar;
				UserManager.GetProfile(request.UserName, out password, out avatar);
				result.Avatar = avatar;
				result.Password = password;
				result.UserName = request.UserName;
				result.Success = true;
			}
			catch (UserNotFoundException e)
			{
				result.Success = false;
				result.ErrorMessage = e.Message;
				Logger.Log(e.Message, this);
			}
			return result;
		}

        public GetStatisticsResult GetStatistics(GetStatisticsRequest request)
        {
            var result = new GetStatisticsResult();
            try
            {
                result.WinRateStatistics = UserManager.Users[request.UserName].GetWinRateStatistics();
                result.GrossProfitWinRateStatistics = UserManager.Users[request.UserName].GetGrossProfitWinRateStatistics();
            }
            catch (PokerException e)
            {
                result.ErrorMessage = e.Message;
                result.Success = false;
                Logger.Log(e.Message, this);
            }
            return result;
        }
    }
}
