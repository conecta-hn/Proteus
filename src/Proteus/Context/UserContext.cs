/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Data.Entity;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Context
{
    public class UserContext : ProteusContext
    {
        public DbSet<ButtonSecurityDescriptor> ButtonSecurityDescriptors { get; set; }
        public DbSet<ModuleSecurityDescriptor> ModuleSecurityDescriptors { get; set; }
        public DbSet<ServiceSecurityDescriptor> ServiceSecurityDescriptors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<LoginToken> LoginTokens { get; set; }
        public DbSet<Aviso> Avisos { get; set; }
        public DbSet<ConfigRepository> ConfigRepositories { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}