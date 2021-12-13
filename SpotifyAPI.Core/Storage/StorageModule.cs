using Autofac;
using EntityFrameworkCore.Triggers;
using Minio;

namespace SpotifyAPI.Core.Storage;

public class StorageModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        MinioClient minio = new MinioClient("minio",
            "root",
            "Password123!"
        ).WithSSL();
        
        builder.RegisterInstance(minio).SingleInstance();

    }
}