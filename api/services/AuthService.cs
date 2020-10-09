﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SS.Api.helpers.extensions;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.authorization;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.services
{
    /// <summary>
    /// This should load up our User context with claims from the database. 
    /// </summary>
    public class AuthService
    {
        private readonly SheriffDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AuthService(SheriffDbContext dbContext, IWebHostEnvironment hostEnvironment)
        {
            _db = dbContext;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<List<Claim>> GenerateClaims(List<Claim> currentClaims)
        {
            var claims = new List<Claim>();

            //Enable all roles for now. 
            if (_hostEnvironment.IsDevelopment())
                claims.Add(new Claim(ClaimTypes.Role, Role.SystemAdministrator));

            var idirName = currentClaims.GetValueByType("preferred_username").Replace("@idir", "");
            var idirId = Guid.Parse(currentClaims.GetValueByType("idir_userid"));

            //Match by IdirID (already logged into SSO before) or Idir with no IdirID (created, but hasn't logged in yet).
            //Hopefully IdirID doesn't change when the base IdirName does (getting married / divorced etc).
            var user = await _db.User.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.IdirId == idirId || !u.IdirId.HasValue && u.IdirName == idirName);

            if (user?.IsEnabled != true) 
                return claims;

            user.IdirId ??= idirId;
            user.LastLogin = DateTime.UtcNow;
            user.KeyCloakId = Guid.Parse(currentClaims.GetValueByType(ClaimTypes.NameIdentifier));
            await _db.SaveChangesAsync();

            claims.AddRange(user.ActiveRoles.SelectToList(r => new Claim(ClaimTypes.Role, r.Name)));
            claims.AddRange(user.Permissions.SelectToList(p => new Claim(CustomClaimTypes.Permission, p.Name)));
            claims.Add(new Claim(CustomClaimTypes.UserId, user.Id.ToString()));
            if (user.HomeLocationId.HasValue)
                claims.Add(new Claim(CustomClaimTypes.HomeLocationId, user.HomeLocationId.ToString()));
            return claims;
        }
    }
}
