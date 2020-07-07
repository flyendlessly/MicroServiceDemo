using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LoginApi
{
    public class InMemoryConfiguration
    {
        public static IConfiguration Configuration { get; set; }


        /// <summary>
        /// Define which APIs will use this IdentityServer 资源API
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                //clientservice:标识名称，CAS Client Service：显示名称，可以自定义
                new ApiResource("clientservice", "CAS Client Service"),
                new ApiResource("productservice", "CAS Product Service"),
                new ApiResource("agentservice", "CAS Agent Service"),
                //ApiResouce的构造函数有一个重载支持传进一个Claim集合，用于允许该Api资源可以携带那些Claim。
                new ApiResource("secretapi","加密Api",new List<string>(){ "role"}),
                new ApiResource("api1","API Application")
                {
                    UserClaims = { "role", JwtClaimTypes.Role }//角色
                }
            };
        }

        /// <summary>
        /// Define which Apps will use thie IdentityServer 客户端信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "client.api.service",
                    ClientSecrets = new [] { new Secret("clientsecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "clientservice" }
                },
                new Client
                {
                    ClientId = "product.api.service",
                    ClientSecrets = new [] { new Secret("productsecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "clientservice", "productservice" }
                },
                new Client
                {
                    ClientId = "agent.api.service",
                    ClientSecrets = new [] { new Secret("agentsecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "agentservice", "clientservice", "productservice" }
                },
                //用户名密码模式
                //需要添加用户配置，通过用户名密码获取access_token（JWT），包含对应用户信息（权限、关键信息）
                new Client
                {
                    ClientId = "apiClientPassword",
                    ClientSecrets = new [] { new Secret("apiSecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    //允许访问的资源
                     AllowedScopes={
                        "secretapi"
                    }
                },
                //客户端模式
                new Client()
                {
                    //客户端Id
                     ClientId="apiClientCd",
                     //客户端密码
                     ClientSecrets={new Secret("apiSecret".Sha256()) },
                     //客户端授权类型，ClientCredentials:客户端凭证方式
                     AllowedGrantTypes=GrantTypes.ClientCredentials,
                     //允许访问的资源
                     AllowedScopes={
                        //IdentityServerConstants.StandardScopes.OpenId,
                        //IdentityServerConstants.StandardScopes.Profile,
                        "secretapi"
                    },
                    //可以带信息
                    Claims=new List<Claim>(){
                        new Claim(IdentityModel.JwtClaimTypes.Role,"Admin"),
                        new Claim(IdentityModel.JwtClaimTypes.NickName,"Even"),
                        new Claim("eMail","904044929@qq.com")
                    },
                },
                new Client()
                {
                    //客户端Id
                     ClientId="apiClientImpl",
                     ClientName="ApiClient for Implicit",
                     //客户端授权类型，Implicit:隐藏模式
                     AllowedGrantTypes=GrantTypes.Implicit,
                     //允许登录后重定向的地址列表，可以有多个
                     //RedirectUris = {"https://localhost:5002/auth.html" },
                     //允许访问的资源
                     AllowedScopes={
                        "secretapi"
                    },
                     //允许将token通过浏览器传递
                     AllowAccessTokensViaBrowser=true
                },
                new Client
                {
                    ClientId = "postman",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RedirectUris = { "https://localhost:5001/oauth2/callback" },
                    //用于认证的密码
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    //客户端有权访问的范围（Scopes）
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },

                    AllowOfflineAccess = true

                }
            };
        }

        /// <summary>
        /// Define which uses will use this IdentityServer
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "10001",//用户ID
                    Username = "904044929@qq.com",//用户名
                    Password = "asd123",
                    //可以带信息。采用用户密码和密码模式获取Token访问Api，返回的值依然是没有Claim的,要在ApiResource中增加支持
                    Claims=new List<Claim>(){
                        new Claim("role","admin")//Role（角色）这个Claim很有用，可以用来做简单的权限管理。
                    }
                },
                new TestUser
                {
                    SubjectId = "10002",
                    Username = "yunfeizhishang@outlook.com",
                    Password = "asd123",
                    Claims=new List<Claim>(){
                         new Claim(ClaimTypes.Role,"guest")
                     }
                },
                new TestUser
                {
                    SubjectId = "10003",
                    Username = "leo@hotmail.com",
                    Password = "leopassword"
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentity()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()   
            };
        }
    }
}
