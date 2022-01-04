using System;
using System.Collections.Generic;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace MyWishlist.Utils
{
    internal class CloudinaryImagesStorage : IImagesStorage
    {
        private readonly Cloudinary _cloudinary;
        private readonly IOptions<Constants> _options;

        public CloudinaryImagesStorage(IOptions<Constants> options)
        {
            var link = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
            var accountUri = new Uri(link);
            var userInfo = accountUri.UserInfo.Split(':');

            var account = new Account
            {
                Cloud = accountUri.Host,
                ApiKey = userInfo[0],
                ApiSecret = userInfo[1]
            };
            _cloudinary = new Cloudinary(account);
            _options = options;
        }

        public string Upload(string path)
        {
            if (path == null) return null;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(path),
                Transformation = new Transformation().Height(_options.Value.ImageHeight)
            };

            var uploadResult = _cloudinary.Upload(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public void Delete(string path)
        {
            if (path == null) return;
            var ids = new List<string>();
            var id = Path.GetFileNameWithoutExtension(path);
            ids.Add(id);

            var delParams = new DelResParams()
            {
                PublicIds = ids,
                Invalidate = true
            };

            _cloudinary.DeleteResources(delParams);
        }
    }
}