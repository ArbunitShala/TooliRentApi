using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Infrastructure.Auth;

namespace TooliRent.Infrastructure.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Roller
            string[] roles = ["Admin", "Member"];
            foreach (var r in roles)
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole<Guid>(r));

            // Admin
            const string adminEmail = "admin@tooli.com";
            const string adminPwd = "Admin123?";
            var admin = await userMgr.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    IsActive = true
                };
                var res = await userMgr.CreateAsync(admin, adminPwd);
                if (res.Succeeded) await userMgr.AddToRoleAsync(admin, "Admin");
            }

            // Test-medlem
            const string memberEmail = "member@tooli.com";
            const string memberPwd = "User123?";
            var member = await userMgr.FindByEmailAsync(memberEmail);
            if (member is null)
            {
                member = new ApplicationUser
                {
                    UserName = memberEmail,
                    Email = memberEmail,
                    FirstName = "Test",
                    LastName = "Member",
                    EmailConfirmed = true,
                    IsActive = true
                };
                var res = await userMgr.CreateAsync(member, memberPwd);
                if (res.Succeeded) await userMgr.AddToRoleAsync(member, "Member");
            }
        }
    }
}
