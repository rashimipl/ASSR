namespace ReactWithASP.Server.InterfaceServices
{
    public interface ILinkedInService
    {
        Task<string> GetAccessToken(string authorizationCode);
        Task<string> UploadImageAsync(string accessToken, byte[] imageBytes, string fileName);
        Task CreatePostAsync(string accessToken, string assetId, string postText);
    //Task<string> GetUserProfile(string accessToken);
    }
}
