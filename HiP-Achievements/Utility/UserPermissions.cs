using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using System;
using System.Linq;
using System.Security.Principal;

namespace PaderbornUniversity.SILab.Hip.Achievements.Utility
{
    public class UserPermissions
    {
        public static bool IsAllowedToCreate(IIdentity identity, AchievementStatus status)
        {
            if (status != AchievementStatus.Published && CheckRoles(identity, UserRoles.Student))
                return true;

            return CheckRoles(identity);
        }

        public static bool IsAllowedToEdit(IIdentity identity, AchievementStatus status, string ownerId)
        {
            bool isOwner = ownerId == identity.GetUserIdentity();
            if (status != AchievementStatus.Published && isOwner)
                return true;

            return CheckRoles(identity);
        }

        public static bool IsAllowedToDelete(IIdentity identity, AchievementStatus status, string ownerId)
        {
            bool isOwner = ownerId == identity.GetUserIdentity();
            if (status != AchievementStatus.Published && isOwner)
                return true;

            return CheckRoles(identity);
        }

        public static bool IsAllowedToGet(IIdentity identity, AchievementStatus status, string ownerId)
        {
            bool isOwner = ownerId == identity.GetUserIdentity();
            if (status == AchievementStatus.Published || isOwner)
                return true;

            return CheckRoles(identity);
        }

        public static bool IsAllowedToGetAll(IIdentity identity, AchievementQueryStatus status)
        {
            if (status == AchievementQueryStatus.Published)
                return true;

            return CheckRoles(identity);
        }
      
        public static bool IsAllowedToCreateImage(IIdentity identity, string ownerId)
        {
            bool isOwner = ownerId == identity.GetUserIdentity();
            if (isOwner)
                return true;

            return CheckRoles(identity);
        }

        //Check if the user has the nessesary roles
        static bool CheckRoles(IIdentity identity, UserRoles allowedToProceed = UserRoles.Administrator | UserRoles.Supervisor)
        {
            return identity.GetUserRoles()
                           .Any(x => (Enum.TryParse(x.Value, out UserRoles role) && (allowedToProceed & role) != 0)); // Bitwise AND
        }
    }

    [Flags]
    public enum UserRoles
    {
        None = 1,
        Administrator = 2,
        Supervisor = 4,
        Student = 8,
    }
}
