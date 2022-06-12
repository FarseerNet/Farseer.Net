// See https://aka.ms/new-console-template for more information

using Farseer.Net.CacheDemo;
using FS;

FarseerApplication.Run<StartupModule>().Initialize();

new UserService().ToEntity();
new UserService().ToEntity();

new UserService().Delete(88);