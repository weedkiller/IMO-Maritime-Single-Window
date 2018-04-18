using System.Linq;
using System.Threading.Tasks;
using IMOMaritimeSingleWindow.Data;
using IMOMaritimeSingleWindow.Helpers;
using IMOMaritimeSingleWindow.Identity;
using IMOMaritimeSingleWindow.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
 

namespace IMOMaritimeSingleWindow.Controllers
{
    //[Authorize]
    [Route("api/[controller]")] 
    public class AccountController : Controller
    {
        private readonly UserDbContext userDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            UserDbContext userDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            this.userDbContext = userDbContext;
        }

        [Authorize(Roles = "admin")]
        // POST api/accounts/register
        [HttpPost("registerwopw")]
        public async Task<IActionResult> Register([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<ApplicationUser>(model);

            var result = await _userManager.CreateAsync(userIdentity);

            //TODO: Implement functionality for sending email to user with new account

            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            //Create the Person associated with the ApplicationUser 
            await userDbContext.Person.AddAsync(new Person
            {
                IdentityId = userIdentity.Id,
                FirstName = model.FirstName,
                LastName = model.LastName
            });
            await userDbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }
        
        // POST api/accounts/register
        [HttpPost("registerwithpw")]
        public async Task<IActionResult> RegisterWithPassword([FromBody]RegistrationWithPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<ApplicationUser>(model);

            var result = await _userManager.CreateAsync(userIdentity, model.Password);
            await userDbContext.SaveChangesAsync();

            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            //Create the Person associated with the ApplicationUser 
            await userDbContext.Person.AddAsync(new Person {
                IdentityId = userIdentity.Id,
                FirstName = model.FirstName,
                LastName = model.LastName
            });
            
            await userDbContext.Password.AddAsync(new Password
            {
                IdentityId = userIdentity.Id,
                PasswordHash = userIdentity.PasswordHash
            });

            await userDbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }


        //[Authorize(Roles = "admin")]
        [HttpGet("getrole/all")]
        public IActionResult GetAllRoles()
        {
            var rolesQueryAble = _roleManager.Roles;
            var roles = from role in rolesQueryAble
                        select role.Name;
            
            return Json(roles);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("getrole")]
        public IActionResult GetRole()
        {
            var userClaimsPrincipal = Request.HttpContext.User;
            var role = userClaimsPrincipal.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(f => f.Value);
            return Json(role);
        }

    }
}
