using Amazon.S3.Model;
using FireSharp.Extensions;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBTSOCIAL.Src.Features.Chat.Helpers
{
    public class UserHelper
    {
        private readonly UserManager<AUser> _userManager;
        public UserHelper(UserManager<AUser> userManager)
        {
            Console.WriteLine("Init UserHelper");
            _userManager = userManager;
        }
        public async Task<string> GetFullNameById(string userId)
        {
            AUser? user = await _userManager.FindByIdAsync(userId);
            
            if (user == null) return "Member";
            return user.FirstName + " " + user.LastName;
        }
        public async Task<string?> GetAvaterUserById(string userId)
        {
            AUser? user = await _userManager.FindByIdAsync(userId);
            
            if (user == null) return null;
            return user.AvataUrl;
        }
        public async Task<string?> GetUserIdByUserName(string userName)
        {
            AUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null) 
            {
                return null;
            }
            
            return user.Id.ToString();
        }
    }
        
}
