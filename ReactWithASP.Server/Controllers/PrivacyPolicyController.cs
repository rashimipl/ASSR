//using Ganss.Xss;
using Autofac.Core;
using Autofac.Features.Metadata;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PayPal.Api;
using PayPal;
using PayPalCheckoutSdk.Orders;
using Quartz;
using ReactWithASP.Server.Models;
using SendGrid.Helpers.Mail;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Tweetinvi.Core.Models.Properties;
using Tweetinvi.Models;
using Xabe.FFmpeg;
using static YourNamespace.Controllers.AccountController;

namespace ReactWithASP.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class PrivacyPolicyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public PrivacyPolicyController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

     [HttpPost("SavePolicyContents")]
      public async Task<IActionResult> SavePolicyContents([FromBody] ContentRequest request)
      {
      string type = request.Index == 0 ? "Terms" : "Privacy";

      // Optional: Get latest existing of that type
      var existing = await _context.PolicyContents
          .Where(c => c.Type == type)
          .OrderByDescending(c => c.CreatedOn)
          .FirstOrDefaultAsync();

      if (existing != null)
      {
        // Update existing
        existing.Content = request.Content;
        existing.UpdatedOn = DateTime.UtcNow;
      }
      else
      {
        // Add new
        var newContent = new PolicyContents
        {
          Type = type,
          Content = request.Content,
          CreatedOn = DateTime.Now
        };
        _context.PolicyContents.Add(newContent);
      }

      await _context.SaveChangesAsync();
      return Ok(new { Message = "Content saved successfully" });
    }
    [HttpGet("get/{index}")]
    public async Task<IActionResult> GetContent(int index)
    {
      string type = index == 0 ? "Terms" : "Privacy";

      var content = await _context.PolicyContents
          .Where(c => c.Type == type)
          .OrderByDescending(c => c.CreatedOn)
          .FirstOrDefaultAsync();

      if (content == null)
        return NotFound("Content not found");

      return Ok(content);
    }


    [HttpGet("GetPrivacyPolicyInfo")]
    public IActionResult GetPrivacyPolicyInfo()
    {
      var htmlContent = @"
                  <!DOCTYPE html>
                  <html lang='en'>
                  <head>
                      <meta charset='UTF-8'>
                      <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                      <title>Privacy Policy</title>                 
                        </head> 
                        <body>
                         <div class='message'>
     <h2>About</h2>
     <h3>Welcome to ""Asar App"" AsarApp Platform for Managing your social Media channels. We understand the importance of your privacy and the security of your data, and we are committed to protecting the information you share with us. This Privacy Policy explains how we collect, use, and safeguard your personal data.1. Information We Collect.</h3>
    <h2>1-AsserApp Services</h2>
    <h3>AsserApp social media management platform allows you to bring together all of your social media accounts for easy access and management. also from Asser web app or mobile App , you can manage your social media, marketing, and advertising campaigns engage with your audience , schedule and publish messages , manage customer care communications, monitor your brand and other brands, and analyze the results of these activities. We also offer training services through and Optimizing of your business by using intelligent Tools on this platform and ai tool.</h3>    
    <h2>2-Information we collect and how we use it</h2>
    <h3>We collect information about you when you use our Services, visit our websites, and interact with us in a business context. Information may be collected when you provide it to us directly, or automatically when you use the Services and websites. We may also collect data from other sources, including those that are publicly available or from third-party data providers.</h3>
    <h2>3-Using our Services</h2>
    <h3>AsserApp collects information to provide you with our Services and to improve our service offerings. We collect contact, account, and preference details for account setup, personalization, and communication. Billing, Services plan, and business data help us manage our Services. We process Content, social profile, and messaging data to facilitate your use of our Services. Lastly, logs, usage data, and support details allow us to maintain security and improve our Services.</h3>
    <h2>-Account Information</h2>
    <h3>Contact and profile information E.g., Name and email address, organization name, address, language preferences, time zone, image (if you choose to provide this), preferred types of communications Creating an account and user authentication of the Services. Communicating with you about the Services, and events.</h3>
    <h2>- Social Network profile information</h2>
    <h3>E.g., Facebook username, Creating an account or user authentication of the Services if you choose to use a social login.</h3>
    <h2>- Billing, plan, and business information</h2>
    <h3>E.g., Credit card number, organization name Provisioning and payment of the Services.</h3>
    <h2>-Content</h2>
    <h3>Content uploaded to, downloaded from, and visible on the Services. this includes materials curated and uploaded by users to the Services, including, messages, posts, comments, images, videos advertising information; messages, posts, reals, Stutes, comments, prompts and outputs from AI-Powered Services; and mentions (such as, likes) from Social Network users. E.g., Instagram posts from an organization’s account.</h3>
    <h2>4-Children’s Privacy</h2>
    <h3>We do not knowingly collect personal information from children. Our Services are not intended for use by children and should only be accessed by individuals who are at least 18 years old and are using the Services for business purposes. If you believe we may have information from a child, please.</h3>
    <h2>5- How We Use Information</h2>
    <h3>Where you are using our Services and making decisions about the personal data that is being processed in the Services (including selecting the Social Network accounts you wish to connect to the Services, uploading and using Content, or defining your own search queries, filters and time frames for the listening services), you are acting as a data controller and AsserApp is acting as a data processor. We rely on you to comply with applicable privacy laws when collecting, using, or disclosing information about individuals through the Services, including obtaining any necessary consents and providing any necessary notices.</h3>
    <h2>6- Sharing Information</h2>
    <h3>We do not sell or share your personal data with third parties except in the following cases:
    ·        When we have your consent.
    ·        When required by law or legal regulations.
    ·        When working with trusted partners to provide application services (ensuring their compliance with privacy standards).</h3>
    <h2>7-Data Protection</h2>
    <h3>We use advanced security technologies to protect your data from unauthorized access, modification, or disclosure.</h3>
    <h2>8-User Rights</h2>
    <h3>You can access, modify, or delete your personal data at any time. You may also request to stop the use of your data for specific purposes.</h3>
    <h2>9-Changes to this Privacy Policy</h2>
    <h3>We may make changes to this Privacy Policy at any time to reflect updates to our Services, applicable laws, and other factors. We will include a prominent notice on this website and/or our Services if we make any material changes, but we encourage you to stay informed by reviewing this page periodically.</h3>
    <h2> 10-How to contact us</h2>
    <h3>If you have any questions, concerns or feedback, please email our Privacy team and Privacy Officer/Data Protection Officer at info@asserapp.com.</h3>
    </div>
                      <script>
                     if (window.opener) {
                // Send a message to the parent window (make sure the status is 'cancel' or      other      states as needed)
                window.opener.postMessage({
                  status: 'cancel'
                }, 'https://assrweb.digitalnoticeboard.biz/'); // Send it only to the   expected  domain
               }
                  </script>
                          </body>
                        </html>";

      return new ContentResult
      {
        Content = htmlContent,
        ContentType = "text/html",
        StatusCode = 200
      };
    }





  }
}