// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using E_Journal.Infrastructure;
using E_Journal.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace E_Journal.WebUI.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IJournalRepository _repository;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IJournalRepository repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _repository = repository;
        }

        [BindProperty]
        public string GroupSelectorOptions { get; set; }

        [BindProperty]
        public string TeacherSelectorOptions { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} является обязательным полем.")]
            [Display(Name = "Имя")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "{0} является обязательным полем.")]
            [Display(Name = "Фамилия")]
            public string SecondName { get; set; }

            [Required(ErrorMessage = "{0} является обязательным полем.")]
            [Display(Name = "Отчество")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "{0} обязателен для ввода.")]
            [EmailAddress(ErrorMessage = "Введён некорректный Email.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "{0} обязателен для ввода.")]
            [StringLength(100, ErrorMessage = "{0} должен быть длинной от {2} и до {1} символов.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [Required(ErrorMessage = "{0} обязателен для ввода.")]
            [DataType(DataType.Password)]
            [Display(Name = "Повторный пароль")]
            [Compare("Password", ErrorMessage = "Указаны разные пароли.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "{0} является обязательным полем.")]
            [Display(Name = "Роль пользователя")]
            public string Role { get; set; }

            [Required(ErrorMessage = "{0} является обязательным полем.")]
            [Display(Name = "Связанное имя")]
            public string AssociatedName { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            SetSelectorOptions();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.Email = Input.Email;
                user.UserName = Input.Email;
                user.FirstName = Input.FirstName;
                user.SecondName = Input.SecondName;
                user.LastName = Input.LastName;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    result = await SetUserRole(user, Input.Role);

                    if (result.Succeeded)
                    {
                        result = await SetUserAssociatedId(user, Input.Role, Input.AssociatedName);

                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User created a new account with password.");

                            await SendEmailAsync(user, returnUrl);

                            if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            {
                                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                            }
                            else
                            {
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                            }
                        }
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            SetSelectorOptions();
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        // выполняется как при GET, так и при POST, так как они лишь используют одно представление
        // и как я понял, даже имеющиеся данные в форму подставляет браузер, а не сервер 
        // получается, данные Input сервер не возвращает при вызове Page()? Фиг его знает.
        private void SetSelectorOptions()
        {
            GroupSelectorOptions = String.Join("|", _repository.Groups
                .Select(g => g.Name)
                .ToArray());
            TeacherSelectorOptions = String.Join("|", _repository.Teachers
                .Select(g => g.Name)
                .OrderBy(n => n)
                .ToArray());
        }
        private IdentityResult IsRolePermitted(string roleName) =>
            roleName switch
            {
                ApplicationRoles.Student or ApplicationRoles.Teacher => IdentityResult.Success,
                _ => IdentityResult.Failed(
                    new IdentityError[]
                    {
                        new IdentityError()
                        {
                            Description = $"The role named '{roleName}' is not supported or does not exist."
                        }
                    })
            };
        private async Task<IdentityResult> SetUserRole(ApplicationUser user, string roleName)
        {
            var result = IsRolePermitted(roleName);

            if (!result.Succeeded)
            {
                return result;
            }

            return await _userManager.AddToRoleAsync(user, roleName);
        }
        private async Task<IdentityResult> SetUserAssociatedId(ApplicationUser user, string roleName, string associatedName)
        {
            var result = IsRolePermitted(roleName);

            if (!result.Succeeded)
            {
                return result;
            }

            if (roleName == ApplicationRoles.Teacher)
            {
                user.AssociatedId = _repository.Teachers
                    .FirstOrDefault(t => t.Name == associatedName)
                    ?.Id ?? 0;
            }
            else if (roleName == ApplicationRoles.Student)
            {
                user.AssociatedId = _repository.Groups
                    .FirstOrDefault(t => t.Name == associatedName)
                    ?.Id ?? 0;
            }

            if (user.AssociatedId != 0)
            {
                return await _userManager.UpdateAsync(user);
            }
            else
            {
                return IdentityResult.Failed(
                    new IdentityError[]
                    {
                        new IdentityError()
                        {
                            Description = $"The item named '{associatedName}' with '{roleName}' role does not exist."
                        }
                    });
            }
        }
        private async Task SendEmailAsync(ApplicationUser user, string returnUrl)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(Input.Email, "Подтвердите свой Email",
                $"Пожалуйста, подтвердите свой аккаунт, <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>перейдя по этой ссылке</a>.");
        }
    }
}


