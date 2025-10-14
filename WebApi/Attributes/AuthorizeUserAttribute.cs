﻿using Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace WebApi
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public AuthorizeUserAttribute(params UserRoles[] roles)
        {
            Roles = string.Join(",", roles.Select(src => src.GetDescription()));
        }
    }
}