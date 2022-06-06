// See https://aka.ms/new-console-template for more information

using Farseer.Net.CacheDemo;
using Farseer.Net.CacheDemo.Repository;
using FS;

FarseerApplication.Run<StartupModule>().Initialize();

new UserService().ToList();
new UserService().Add(new UserPO
{
    Id   = 88,
    Name = "steden",
    Age  = 90
});
new UserService().ToList();

new UserService().Update(88, new UserPO
{
    Id   = 88,
    Name = "steden_new",
    Age  = 99
});

new UserService().Delete(88);