using GymManagementSystem.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.DataSeed
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                bool hasUsers = userManager.Users.Any();
                bool hasRoles = roleManager.Roles.Any();

                if (hasUsers && hasRoles)
                    return;

                if (!hasRoles)
                {
                    var roles = new List<IdentityRole>
                    {
                        new IdentityRole() {Name="SuperAdmin"},
                        new IdentityRole() {Name="Admin"}
                    };
                    foreach(var roleName in roles.Select(r => r.Name))
                    {
                        if(!await roleManager.RoleExistsAsync(roleName!))
                        {
                            var roleResult=await roleManager.CreateAsync(new IdentityRole(roleName!));
                            if (!roleResult.Succeeded)
                            {
                                logger.LogError($"Failed to create Role {roleName}:{string.Join(";",roleResult.Errors.Select(E=>E.Description))}");
                            }
                        }
                    }
                }
                if (!hasUsers)
                {
                    var superAdmin = new ApplicationUser()
                    {
                        FirstName = "Ahmed",
                        LastName="Waleed",
                        UserName="AhmedWaleed",
                        Email="ahmedwaliid31@gmail.com",
                        PhoneNumber="01111103462"
                    };
                    var superAdminResult= await userManager.CreateAsync(superAdmin, "P@ssw0rd");

                    if (superAdminResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                    }
                    else
                    {
                        logger.LogError($"Failed to create superadmin user :{string.Join(";", superAdminResult.Errors.Select(E => E.Description))}");
                        return;
                    }

                    var admin = new ApplicationUser()
                    {
                        FirstName="Ahmed",
                        LastName="Aly",
                        UserName="AhmedAly",
                        Email = "ahmedalyy31@gmail.com",
                        PhoneNumber ="01111101234"
                    };

                    var adminResult=await userManager.CreateAsync(admin, "P@ssw0rd");

                    if (adminResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                    else
                    {
                        logger.LogError($"Failed to create admin user :{string.Join(";", adminResult.Errors.Select(E => E.Description))}");
                        return;
                    }
                    logger.LogInformation($"Seeded SuperAdmin [{superAdmin.Email}], Admin [{admin.Email}]");
                }

            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Identity Seeding Failed");
                throw;
            }
        }

    }
}
