using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReactWithASP.Server.Authentication;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Models.Posts;
using ReactWithASP.Server.Models.Settings;
using System.Reflection.Emit;

namespace JWTAuthentication.Authentication
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<SubscriptionPlans>()
        .HasNoKey();
            builder.Entity<UserSubscriptions>()
        .HasNoKey();
            builder.Entity<Groups>()
        .HasNoKey();

            /*builder.Entity<GroupPlatform>()
           .HasOne(gp => gp.Group)
           .WithMany(g => g.Platforms)
           .HasForeignKey(gp => gp.GroupId);*/
            /*builder.Entity<GroupSocialMedia>()
            .HasKey(gsm => new { gsm.GroupId, gsm.SocialMediaId });*/

            /*builder.Entity<GroupSocialMedia>()
                .HasOne(gsm => gsm.Group)
                .WithMany(g => g.GroupSocialMedias)
                .HasForeignKey(gsm => gsm.GroupId);*/

            /*builder.Entity<GroupSocialMedia>()
                .HasOne(gsm => gsm.SocialMedia)
                .WithMany(sm => sm.GroupSocialMedias)
                .HasForeignKey(gsm => gsm.SocialMediaId);*/
            builder.Entity<PostLikes>()
            .HasOne(pl => pl.SocialMediaPosts)
            .WithMany(smp => smp.PostLikes)
            .HasForeignKey(pl => pl.PostId);

            builder.Entity<PostShares>()
            .HasOne(ps => ps.SocialMediaPosts)
            .WithMany(smp => smp.PostShares)
            .HasForeignKey(ps => ps.PostId);

            builder.Entity<PostViews>()
           .HasOne(pv => pv.SocialMediaPosts)
           .WithMany(smp => smp.PostViews)
           .HasForeignKey(pv => pv.PostId);


        }
        public DbSet<SubscriptionPlans> SubscriptionPlans { get; set; }
        public DbSet<UserSubscriptions> UserSubscriptions { get; set; }
        public DbSet<SocialMedia> SocialMedia { get; set; }
        public DbSet<UserSocialMediaStatus> UserSocialMediaStatus {  get; set; }
        //public DbSet<Groups> Groups { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<GroupSocialMedia> GroupSocialMedia { get; set; }
        public DbSet<group> group { get; set; }

        public DbSet<SocialMediaPosts> SocialMediaPosts { get; set; }
        public DbSet<PostLikes> PostLikes { get; set; }
        public DbSet<PostShares> PostShares { get; set; }
        public DbSet<PostViews> PostViews { get; set; }
        public DbSet<UserGroupPosts> UserGroupPosts { get; set; }
        //public DbSet<SocialMedia> GroupSocialMedias { get; set; }
        public DbSet<NotificationSetting> NotificationSetting { get; set; }

        public DbSet<ScheduledPost> ScheduledPost { get; set; }
        public DbSet<Hashtag> Hashtag { get; set; }
        public DbSet<HashtagGroup> HashtagGroup { get; set; }
        public DbSet<PrivacyPolicy> PrivacyPolicy { get; set; }

        public DbSet<AccountConnection> AccountConnection { get; set; }
        public DbSet<FacebookPageModel> FacebookUsers { get; set; }
        public DbSet<SMTPsetting> SMTPsetting { get; set; }
        public DbSet<CompanySetting> CompanySetting { get; set; }
        public DbSet<DeveloperSetting> DeveloperSetting { get; set; }
        public DbSet<Applicationsetting> Applicationsetting { get; set; }
        public DbSet<SocialMediaAccountSettings> SocialMediaAccountSettings { get; set; }
        public DbSet<Drafts> Drafts { get; set; }
        public DbSet<PayPalTransactions> PayPalTransactions { get; set; }
        public DbSet<Notification> Notification { get; set; }


    }
}