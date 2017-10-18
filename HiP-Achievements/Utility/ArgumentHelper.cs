using System;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Utility
{
    public static  class ArgumentHelper
    {
        /// <summary>
        /// Tries to deserialize <see cref="AchievementArgs.TypeArgs"/> depenedant on the <see cref="AchievementArgs.Type"/>
        /// If the deserialization fails a <see cref="JsonSerializationException"/> is thrown
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IAchievementTypeArgs GetAchievementTypeArgs(AchievementArgs args)
        {
            var serializer = new JsonSerializer { MissingMemberHandling = MissingMemberHandling.Error };
            switch (args.Type)
            {
                case AchievementType.ExhibitVisited:
                    return args.TypeArgs.ToObject<ExhibitsVisistedArgs>(serializer);
                case AchievementType.RouteFinished:
                    break;
            }

            throw new ArgumentException("The provided type arguments do not contain all necesarry information");
        }
    }
}
