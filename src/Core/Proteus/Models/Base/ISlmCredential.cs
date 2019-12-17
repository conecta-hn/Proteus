using TheXDS.Proteus.Api;
using System.Collections.Generic;
using TheXDS.MCART.Types;

namespace TheXDS.Proteus.Models.Base
{
    public interface IProteusCredential : INameable
    {
        string Id { get; }
        SecurityBehavior? ModuleBehavior { get; }
        SecurityBehavior? ButtonBehavior { get; }
        IEnumerable<ISecurityDescriptor> Descriptors { get; }
        SecurityFlags DefaultGranted { get; }
        SecurityFlags DefaultRevoked { get; }
    }
    public interface IProteusRwCredential : IModelBase<string>, INameable
    {
        new string Name { get; set; }
        SecurityBehavior? ModuleBehavior { get; set; }
        SecurityBehavior? ButtonBehavior { get; set; }
        ICollection<SecurityDescriptor> Descriptors { get; }
        SecurityFlags DefaultGranted { get; set; }
        SecurityFlags DefaultRevoked { get; set; }

    }


    public interface IProteusHierachicalCredential : IProteusCredential
    {
        IProteusHierachicalParentCredential Parent { get; }
        IEnumerable<IProteusRoleCredential> Roles { get; }
    }
    public interface IProteusRwHierachicalCredential : IProteusRwCredential
    {
        UserGroup Parent { get; set; }
        ICollection<UserRole> Roles { get; }
    }


    public interface IProteusHierachicalParentCredential : IProteusHierachicalCredential
    {
        IEnumerable<IProteusHierachicalCredential> Children { get; }
    }
    public interface IProteusRwHierachicalParentCredential : IProteusRwHierachicalCredential
    {
        ICollection<ProteusHierachicalCredential> Children { get; }
    }


    public interface IProteusRoleCredential : IProteusCredential
    {
        IEnumerable<IProteusCredential> Members { get; }
    }
    public interface IProteusRwRoleCredential : IProteusRwCredential
    {
        ICollection<ProteusHierachicalCredential> Members { get; }
    }


    public interface IProteusUserCredential : IProteusHierachicalCredential
    {
        bool Enabled { get; }
        bool ScheduledPasswordChange { get; }
        bool AllowMultiLogin { get; }
        bool Interactive { get; }
    }
    public interface IProteusRwUserCredential : IProteusRwHierachicalCredential
    {
        bool Enabled { get; set; }
        bool ScheduledPasswordChange { get; set; }
        bool AllowMultiLogin { get; set; }
        bool Interactive { get; set; }
    }

}