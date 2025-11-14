using System;
using Microsoft.Extensions.DependencyInjection;
using FileKit.Interface;
using FileKit.Services;
namespace FileKit.Extensions;

public static class FileKitCollectionsExtension
{
     public static IServiceCollection AddFileKit(this IServiceCollection services)
        {
            services.AddSingleton<IFileService,FileService>();
            return services;
        }
}
