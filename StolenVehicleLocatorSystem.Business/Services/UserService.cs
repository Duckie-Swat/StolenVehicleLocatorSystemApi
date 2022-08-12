using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StolenVehicleLocatorSystem.Business.Extensions;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.Contracts.Filters;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using System.Security.Claims;


namespace StolenVehicleLocatorSystem.Business.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;


        public UserService(UserManager<User> userManager,
            RoleManager<Role> roleManager, IMapper mapper,
            ILogger<UserService> logger
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;

        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserDetailDto> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResponseModel<UserDetailDto>> PagedQueryAsync(UserFilter filter)
        {
            var query = _userManager.Users;
            query = query.Where(user => string.IsNullOrEmpty(filter.Keyword)
                        || user.Id.ToString().Contains(filter.Keyword) || user.Email.Contains(filter.Keyword));

            if (!string.IsNullOrEmpty(filter.OrderProperty) && filter.Desc != null)
            {
                query = query.OrderByPropertyName(filter.OrderProperty, (bool)filter.Desc);
            }


            var users = await query.PaginateAsync(filter.Page, filter.Limit);
            return new PagedResponseModel<UserDetailDto>
            {
                CurrentPage = filter.Page,
                TotalItems = users.TotalItems,
                Items = _mapper.Map<IEnumerable<UserDetailDto>>(users.Items),
                TotalPages = users.TotalPages
            };
        }

        public async Task<UserDetailDto> Register(RegisterUserDto newUser, string role)
        {
            var userCheck = await _userManager.FindByEmailAsync(newUser.Email);
            var roleCheck = await _roleManager.FindByNameAsync(role);
            if (userCheck != null)
            {
                _logger.LogError("Account exist");
                return null;
            }
            else if (roleCheck == null)
            {
                _logger.LogError("Role doesn't exist");
                return null;
            }
            else
            {
                var user = _mapper.Map<User>(newUser);
                user.PhoneNumberConfirmed = user.EmailConfirmed = true;
                user.UserName = user.Email;

                var result = await _userManager.CreateAsync(user, newUser.Password);

                result = _userManager.AddToRoleAsync(user, role).Result;

                result = await
                _userManager.AddClaimsAsync(
                    user,
                    new Claim[]
                    {
                            new Claim(JwtClaimTypes.Email, user.Email),
                            new Claim(JwtClaimTypes.Role, role)
                    }
                );
                return _mapper.Map<UserDetailDto>(user);
            }
        }
    }

}
