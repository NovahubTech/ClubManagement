﻿using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.Provider;
using ClubManagement.Ultilities;
using Java.IO;

namespace ClubManagement.Controllers
{
    public static class CloudinaryController
    {
        private const string CloudName = "dw0yzvsvn";

        private const string ApiKey = "759427256828639";

        private const string ApiSecret = "ventEbxZBzh3g5EAfVw7htDIbDA";

        public static async Task<string> UploadImage(Context context, Uri imageUri, string publicId)
        {
            var account =
                new CloudinaryDotNet.Account(CloudName, ApiKey, ApiSecret);

            var cloudinary = new CloudinaryDotNet.Cloudinary(account);

            var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams
            {
                File = new CloudinaryDotNet.Actions.FileDescription(FilePathUtilities.GetAbsoluteFilePath(context, GetImageUri(context, ResizeImage(context, imageUri, 256)))),
                PublicId = publicId
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            return uploadResult.Uri.OriginalString;
        }

        private static Bitmap ResizeImage(Context context, Uri uri, int requireSize)
        {
            var bitmap = MediaStore.Images.Media.GetBitmap(context.ContentResolver, uri);
            var inWidth = bitmap.Width;
            var inHeight = bitmap.Height;

            return inWidth > inHeight
                ? Bitmap.CreateScaledBitmap(bitmap, requireSize, inHeight * requireSize / inWidth, false)
                : Bitmap.CreateScaledBitmap(bitmap, inWidth * requireSize / inHeight, requireSize, false);
        }

        private static Uri GetImageUri(Context inContext, Bitmap inImage)
        {
            inImage.Compress(Bitmap.CompressFormat.Jpeg, 100, new MemoryStream());
            var path = MediaStore.Images.Media.InsertImage(inContext.ContentResolver, inImage, "Title", null);
            return Uri.Parse(path);
        }
    }
}