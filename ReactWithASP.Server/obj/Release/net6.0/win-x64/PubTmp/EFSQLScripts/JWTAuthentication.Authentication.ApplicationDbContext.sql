IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240516122730_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240516122730_Initial', N'6.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [CreatedOn] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [PhotoUrl] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [AccountConnection] (
        [id] int NOT NULL IDENTITY,
        [UserGuid] nvarchar(max) NOT NULL,
        [AccountName] nvarchar(max) NOT NULL,
        [AccountIcon] nvarchar(max) NOT NULL,
        [AccountId] int NOT NULL,
        [Status] bit NOT NULL,
        CONSTRAINT [PK_AccountConnection] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [Applicationsetting] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(max) NOT NULL,
        [ApplicationURL] nvarchar(max) NOT NULL,
        [APIURL] nvarchar(max) NOT NULL,
        [FacebookURL] nvarchar(max) NOT NULL,
        [YoutubeURL] nvarchar(max) NOT NULL,
        [InstagramURL] nvarchar(max) NOT NULL,
        [TwitterURL] nvarchar(max) NOT NULL,
        [AdminURL] nvarchar(max) NOT NULL,
        [SupervisorURL] nvarchar(max) NOT NULL,
        [CompanyURL] nvarchar(max) NOT NULL,
        [UserImagesURL] nvarchar(max) NOT NULL,
        [WorkerReportImagesURL] nvarchar(max) NOT NULL,
        [WorkerDocumentImagesURL] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Applicationsetting] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [CompanySetting] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [PhoneNo] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Reg_Year] int NOT NULL,
        CONSTRAINT [PK_CompanySetting] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [DeveloperSetting] (
        [Id] int NOT NULL IDENTITY,
        [WebMasterEmail] nvarchar(max) NOT NULL,
        [DeveloperEmail] nvarchar(max) NOT NULL,
        [TestMode] bit NOT NULL,
        [CopytoWebmaster] bit NOT NULL,
        [Copytodeveloper] bit NOT NULL,
        CONSTRAINT [PK_DeveloperSetting] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [Drafts] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [PostIcon] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [UserGuid] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [AccountOrGroupName] nvarchar(max) NULL,
        [AccountOrGroupId] nvarchar(max) NULL,
        CONSTRAINT [PK_Drafts] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [FacebookUsers] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NOT NULL,
        [AccessToken] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_FacebookUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [group] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [GroupIcon] nvarchar(max) NULL,
        [UserGuid] nvarchar(max) NOT NULL,
        [Status] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        CONSTRAINT [PK_group] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [Groups] (
        [Id] int NOT NULL,
        [UserGuid] nvarchar(max) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [groupIcon] nvarchar(max) NOT NULL,
        [SocialMediaId] int NOT NULL
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [GroupSocialMedia] (
        [Id] int NOT NULL IDENTITY,
        [GroupId] int NOT NULL,
        [SocialMediaId] int NOT NULL,
        CONSTRAINT [PK_GroupSocialMedia] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [Hashtag] (
        [Id] int NOT NULL IDENTITY,
        [HashtagName] nvarchar(max) NOT NULL,
        [HashtagGroupId] int NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        CONSTRAINT [PK_Hashtag] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [HashtagGroup] (
        [Id] int NOT NULL IDENTITY,
        [UserGuid] nvarchar(max) NOT NULL,
        [HashtagGroupName] nvarchar(max) NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        CONSTRAINT [PK_HashtagGroup] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [NotificationSetting] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [UserGUID] nvarchar(max) NOT NULL,
        [DeviceToken] nvarchar(max) NULL,
        [Title] nvarchar(max) NULL,
        [Descriptions] nvarchar(max) NULL,
        [CreatedOn] datetime2 NULL,
        [ImageIcon] nvarchar(max) NULL,
        CONSTRAINT [PK_NotificationSetting] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [PrivacyPolicy] (
        [Id] int NOT NULL IDENTITY,
        [CreatedOn] datetime2 NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [Meta_Description] nvarchar(max) NOT NULL,
        [Username] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_PrivacyPolicy] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [ScheduledPost] (
        [Id] int NOT NULL IDENTITY,
        [UserGuid] nvarchar(max) NOT NULL,
        [ScheduledType] nvarchar(max) NOT NULL,
        [Days] nvarchar(max) NULL,
        [Months] nvarchar(max) NULL,
        [ScheduledTime] nvarchar(max) NOT NULL,
        [ScheduledDate] nvarchar(max) NULL,
        [FromDate] nvarchar(max) NULL,
        [ToDate] nvarchar(max) NULL,
        [IsPublished] bit NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [MediaUrl] nvarchar(max) NOT NULL,
        [Tags] nvarchar(max) NOT NULL,
        [AccountOrGroupName] nvarchar(max) NOT NULL,
        [AccountOrGroupId] nvarchar(max) NOT NULL,
        [createdOn] datetime2 NOT NULL,
        CONSTRAINT [PK_ScheduledPost] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [SMTPsetting] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        [Host] nvarchar(max) NOT NULL,
        [Port] nvarchar(max) NOT NULL,
        [Enable_SSl] bit NOT NULL,
        [UserDefaultCredentials] bit NOT NULL,
        [ToEmail] nvarchar(max) NOT NULL,
        [MailToCC] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_SMTPsetting] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [SocialMedia] (
        [Id] int NOT NULL IDENTITY,
        [SocialMediaName] nvarchar(max) NOT NULL,
        [src] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_SocialMedia] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [SocialMediaAccountSettings] (
        [Id] int NOT NULL IDENTITY,
        [SocialMediaId] int NOT NULL,
        [TimeLimit] int NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [IsImageAllow] bit NOT NULL,
        [IsVedioAllow] bit NOT NULL,
        [IsTextAllow] bit NOT NULL,
        [Text] nvarchar(300) NOT NULL,
        [ImagePath] nvarchar(max) NOT NULL,
        [VideoPath] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_SocialMediaAccountSettings] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [SocialMediaPosts] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [PostIcon] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [UserGuid] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [AccountOrGroupName] nvarchar(max) NULL,
        [AccountOrGroupId] nvarchar(max) NULL,
        CONSTRAINT [PK_SocialMediaPosts] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [SubscriptionPlans] (
        [Id] int NOT NULL,
        [PlanName] nvarchar(max) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [ConnectedChannels] int NOT NULL,
        [SmartContentSuggestionsMonthly] int NOT NULL,
        [ImageSuggestionsMonthly] nvarchar(max) NOT NULL,
        [DailyPostInspirations] nvarchar(max) NOT NULL,
        [DraftedPosts] nvarchar(max) NOT NULL,
        [PostsDaily] nvarchar(max) NOT NULL,
        [ScheduledPostsQueue] nvarchar(max) NOT NULL,
        [MultiImageVideoPosts] bit NOT NULL,
        [RecurringPosts] bit NOT NULL,
        [PremiumSupport] bit NOT NULL
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [UserGroupPosts] (
        [Id] int NOT NULL IDENTITY,
        [PostId] int NOT NULL,
        [GroupId] int NOT NULL,
        CONSTRAINT [PK_UserGroupPosts] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [UserSubscriptions] (
        [Id] int NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [UserGUID] nvarchar(max) NOT NULL,
        [SubsPlanID] int NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [Posts] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ContentType] nvarchar(max) NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [MediaURL] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [GroupId] int NOT NULL,
        [Likes] int NOT NULL,
        [Views] int NOT NULL,
        [Comments] int NOT NULL,
        [Shares] int NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [StatusCode] int NOT NULL,
        CONSTRAINT [PK_Posts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Posts_group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [group] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [GroupSocialMedias] (
        [Id] int NOT NULL IDENTITY,
        [GroupId] int NOT NULL,
        [SocialMediaId] int NOT NULL,
        CONSTRAINT [PK_GroupSocialMedias] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GroupSocialMedias_SocialMedia_SocialMediaId] FOREIGN KEY ([SocialMediaId]) REFERENCES [SocialMedia] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [UserSocialMediaStatus] (
        [Id] int NOT NULL IDENTITY,
        [UserGuid] nvarchar(max) NOT NULL,
        [SocialMediaId] int NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_UserSocialMediaStatus] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserSocialMediaStatus_SocialMedia_SocialMediaId] FOREIGN KEY ([SocialMediaId]) REFERENCES [SocialMedia] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [PostLikes] (
        [Id] int NOT NULL IDENTITY,
        [PostId] int NOT NULL,
        [UserGuid] nvarchar(max) NOT NULL,
        [PostLikesCount] int NOT NULL,
        CONSTRAINT [PK_PostLikes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PostLikes_SocialMediaPosts_PostId] FOREIGN KEY ([PostId]) REFERENCES [SocialMediaPosts] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [PostShares] (
        [Id] int NOT NULL IDENTITY,
        [PostId] int NOT NULL,
        [UserGuid] nvarchar(max) NOT NULL,
        [PostSharesCount] int NOT NULL,
        CONSTRAINT [PK_PostShares] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PostShares_SocialMediaPosts_PostId] FOREIGN KEY ([PostId]) REFERENCES [SocialMediaPosts] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE TABLE [PostViews] (
        [Id] int NOT NULL IDENTITY,
        [PostId] int NOT NULL,
        [UserGuid] nvarchar(max) NOT NULL,
        [PostViewsCount] int NOT NULL,
        CONSTRAINT [PK_PostViews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PostViews_SocialMediaPosts_PostId] FOREIGN KEY ([PostId]) REFERENCES [SocialMediaPosts] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE INDEX [IX_GroupSocialMedias_SocialMediaId] ON [GroupSocialMedias] ([SocialMediaId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE INDEX [IX_PostLikes_PostId] ON [PostLikes] ([PostId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE INDEX [IX_Posts_GroupId] ON [Posts] ([GroupId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE INDEX [IX_PostShares_PostId] ON [PostShares] ([PostId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE INDEX [IX_PostViews_PostId] ON [PostViews] ([PostId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    CREATE INDEX [IX_UserSocialMediaStatus_SocialMediaId] ON [UserSocialMediaStatus] ([SocialMediaId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240823065103_abc')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240823065103_abc', N'6.0.30');
END;
GO

COMMIT;
GO

