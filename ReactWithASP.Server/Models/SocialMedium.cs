/*using System;
using System.Collections.Generic;

namespace ReactWithASP.Server.Models
{
    public partial class SocialMedium
    {
        public SocialMedium()
        {
            GroupSocialMedia = new HashSet<GroupSocialMedium>();
            Groups = new HashSet<Group>();
            UserSocialMediaStatuses = new HashSet<UserSocialMediaStatus>();
        }

        public int Id { get; set; }
        public string SocialMediaName { get; set; } = null!;
        public string Src { get; set; } = null!;

        public virtual ICollection<GroupSocialMedium> GroupSocialMedia { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<UserSocialMediaStatus> UserSocialMediaStatuses { get; set; }
    }
}
*/