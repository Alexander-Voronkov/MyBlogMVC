using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBlog.Authorization;
using MyBlog.Data.Entities;
using MyBlog.Utils;
using System.Security.Claims;

namespace MyBlog.Data
{
    public static class SeedData
    {
        public static async Task Initialize(
            IServiceProvider serviceProvider,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration
            )
        {
            DbContextOptions<ApplicationContext> options =
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>();

            UserManager<User> userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();


            using (ApplicationContext context = new ApplicationContext(options))
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                if (context.Posts.Any())
                {
                    return;
                }

                // Authorize And Claims **********************************************************************
                string adminEmail = configuration.GetSection("AdminEmail").Value;
                string adminPassword = configuration.GetSection("AdminPassword").Value;

                if (await userManager.FindByNameAsync(adminEmail) == null)
                {
                    // 1
                    User superAdmin = new User
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                        EmailConfirmed = true
                    };

                    IdentityResult result = await userManager.CreateAsync(superAdmin, adminPassword);
                    if (result.Succeeded)
                    {
                        //Add Claims
                        Claim claim1 = new Claim(MyClaims.PostsWriter, MyClaims.PostsWriter);
                        Claim claim2 = new Claim(MyClaims.Admin, MyClaims.Admin);
                        Claim claim3 = new Claim(MyClaims.SuperAdmin, MyClaims.SuperAdmin);

                        await userManager.AddClaimAsync(superAdmin, claim1);
                        await userManager.AddClaimAsync(superAdmin, claim2);
                        await userManager.AddClaimAsync(superAdmin, claim3);
                    }

                    // 2
                    User admin = new User
                    {
                        Email = "vasyan666@gmail.com",
                        UserName = "vasyan666@gmail.com",
                        EmailConfirmed = true
                    };

                     result = await userManager.CreateAsync(admin, adminPassword);
                    if (result.Succeeded)
                    {
                        //Add Claims
                        Claim claim1 = new Claim(MyClaims.PostsWriter, MyClaims.PostsWriter);
                        Claim claim2 = new Claim(MyClaims.Admin, MyClaims.Admin);

                        await userManager.AddClaimAsync(admin, claim1);
                        await userManager.AddClaimAsync(admin, claim2);
                    }


                    // 3
                    User postsWriter = new User
                    {
                        Email = "petya@gmail.com",
                        UserName = "petya@gmail.com",
                        EmailConfirmed = true
                    };

                    result = await userManager.CreateAsync(postsWriter, adminPassword);
                    if (result.Succeeded)
                    {
                        //Add Claims
                        Claim claim1 = new Claim(MyClaims.PostsWriter, MyClaims.PostsWriter);

                        await userManager.AddClaimAsync(postsWriter, claim1);
                    }
                }
                // End of Authorize And Claims **********************************************************************
                else
                {
                    ILogger logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Cannot find the adminEmail");
                    return;
                }


                IList<byte[]> pictures = new List<byte[]>
                {
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine1.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine2.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine3.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine4.png"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine5.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine6.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine7.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine8.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine9.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\ukraine10.png"),

                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music1.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music2.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music3.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music4.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music5.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music6.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music7.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music8.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music9.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\music10.jpg"),

                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy1.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy2.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy3.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy4.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy5.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy6.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy7.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy8.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy9.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy10.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy11.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy12.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy13.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy14.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy15.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy16.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy17.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy18.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy19.jpg"),
                    System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\astronomy20.jpg"),
                };

                byte[] emptyPicture = System.IO.File.ReadAllBytes($"{webHostEnvironment.WebRootPath}\\images\\emptyPicture.jpg");

                IList<List<PostImage>> listOfPostEmptyImages = new List<List<PostImage>>();
                IList<PostImage> postEmptyImages = new List<PostImage>();
                for (int i = 1; i <= pictures.Count; i++)
                {
                    List<PostImage> imageList = new List<PostImage>();
                    for (int j = 1; j <= 10; j++)
                    {
                        PostImage newPostImage = new PostImage
                        {
                            Image = emptyPicture,
                            Description = j % 3 == 0 ? $"Description Example {j}" : null,
                            PostId = i
                        };

                        postEmptyImages.Add(newPostImage);
                        imageList.Add(newPostImage);
                    }
                    listOfPostEmptyImages.Add(imageList);
                }
                ////////////////////////////////////////////////////////////////////////////



                // ///////////////////////////////////////////////////////////////


                IList<Category> categories = new List<Category>
                {
                    new Category{ Name = "Ukraine" },
                    new Category{ Name = "Music" },
                    new Category{ Name = "Astronomy" },
                };
                // ///////////////////////////////////////////////////////////////


                string superAdminId = userManager.FindByNameAsync(adminEmail).Result.Id;
                IList<Comment> comments = new List<Comment>
                {
                    new Comment
                    {
                        Message = "This is a first parent comment",
                        Created = DateTime.Now,
                        PostId = 1,
                        UserId = superAdminId,
                        ParentCommentId = null,

                        ChildComments = new List<Comment>
                        {
                            new Comment
                            {
                                Message = "This is a first child comment on the first parent comment above",
                                Created = DateTime.Now.AddMinutes(5),
                                PostId = 1,
                                UserId = superAdminId,
                                //ParentCommentId = 1,
                            },
                            new Comment
                            {
                                Message = "This is a second child comment on the same first parent comment above",
                                Created = DateTime.Now.AddMinutes(10),
                                PostId = 1,
                                UserId = superAdminId,
                                //ParentCommentId = 1,

                                ChildComments = new List<Comment>
                                {
                                    new Comment
                                    {
                                        Message = "This is a first child comment on the 2nd child comment above which is a second child comment on the first parent comment",
                                        Created = DateTime.Now.AddMinutes(5),
                                        PostId = 1,
                                        UserId = superAdminId,
                                        //ParentCommentId = 1,
                                    },
                                    new Comment
                                    {
                                        Message = "This is a second child comment on the 2nd child comment above which is a second child comment on the first parent comment",
                                        Created = DateTime.Now.AddMinutes(10),
                                        PostId = 1,
                                        UserId = superAdminId,
                                        //ParentCommentId = 1,


                                    }
                                }
                            }
                        }
                    },
                    new Comment
                    {
                        Message = "This is a second parent comment",
                        Created = DateTime.Now.AddMinutes(1),
                        PostId = 1,
                        UserId = superAdminId,
                        ParentCommentId = null,

                        ChildComments = new List<Comment>
                        {
                            new Comment
                            {
                                Message = "This is a first child comment on the 2nd parent comment above",
                                Created = DateTime.Now.AddMinutes(5),
                                PostId = 1,
                                UserId = superAdminId,
                                //ParentCommentId = 1,
                            },
                            new Comment
                            {
                                Message = "This is a second child comment on the same 2nd parent comment above",
                                Created = DateTime.Now.AddMinutes(10),
                                PostId = 1,
                                UserId = superAdminId,
                                //ParentCommentId = 1,
                            }
                        }
                    },
                    new Comment
                    {
                        Message = "This is a third parent comment still without child comments",
                        Created = DateTime.Now.AddMinutes(1),
                        PostId = 1,
                        UserId = superAdminId,
                        ParentCommentId = null,
                    }
                };
                // /////////////////////////////////////////////////////////////////


                IList<Post> posts = new List<Post>();
                string loremIpsum = LoremIpsum.LoremIpsumString;

                Post fitstPost = new Post
                {
                    Title = "Example Title for Ukraine 1",
                    Description = "Example Description 1",
                    Body = loremIpsum,
                    Created = DateTime.Now,
                    MainPostImage = pictures[0], //ukrainePicture1,
                    CategoryId = 1,
                    UserId = superAdminId,
                    PostImages = listOfPostEmptyImages[0],
                    Comments = comments
                };

                posts.Add(fitstPost);

                for (int i = 2; i <= 10; i++)
                {
                    posts.Add(new Post
                    {
                        Title = $"Example Title for Ukraine {i}",
                        Description = (i % 3 == 0) ? null : $"Example Description {i}",
                        Body = loremIpsum,
                        Created = DateTime.Now,
                        MainPostImage = pictures[i - 1],
                        CategoryId = 1,
                        UserId = superAdminId,
                        PostImages = listOfPostEmptyImages[i - 1],
                        // Comments = comments
                    });
                }

                for (int i = 11; i <= 20; i++)
                {
                    posts.Add(new Post
                    {
                        Title = $"Example Title for Music {i}",
                        Description = (i % 3 == 0) ? null : $"Example Description {i}",
                        Body = loremIpsum,
                        Created = DateTime.Now,
                        MainPostImage = pictures[i - 1],
                        CategoryId = 2,
                        UserId = superAdminId,
                        PostImages = listOfPostEmptyImages[i - 1],
                        // Comments = comments
                    });
                }

                for (int i = 21; i <= 40; i++)
                {
                    posts.Add(new Post
                    {
                        Title = $"Example Title for Astronomy {i}",
                        Description = (i % 3 == 0) ? null : $"Example Description {i}",
                        Body = loremIpsum,
                        Created = DateTime.Now,
                        MainPostImage = pictures[i - 1],
                        CategoryId = 3,
                        UserId = superAdminId,
                        PostImages = listOfPostEmptyImages[i - 1],
                        // Comments = comments
                    });
                }


                // await context.PostImages.AddRangeAsync(postEmptyImages);
                // await context.Comments.AddRangeAsync(comments);

                await context.Categories.AddRangeAsync(categories);
                await context.Posts.AddRangeAsync(posts);

                await context.SaveChangesAsync();
            }
        }
    }

}
