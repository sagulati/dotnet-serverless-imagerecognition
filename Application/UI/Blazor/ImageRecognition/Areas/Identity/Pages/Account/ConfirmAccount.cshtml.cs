using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ImageRecognition.BlazorFrontend.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmAccountModel : PageModel
    {
        private readonly CognitoUserManager<CognitoUser> _userManager;

        public ConfirmAccountModel(UserManager<CognitoUser> userManager)
        {
            _userManager = userManager as CognitoUserManager<CognitoUser>;
        }

        [BindProperty] public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return NotFound($"Unable to load user with ID '{userId}'.");

                var result = await _userManager.ConfirmSignUpAsync(user, Input.Code, true);
                if (!result.Succeeded)
                    throw new InvalidOperationException($"Error confirming account for user with ID '{userId}':");
                return RedirectToPage("./Login");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public class InputModel
        {
            [Required] [Display(Name = "Code")] public string Code { get; set; }
        }
    }
}