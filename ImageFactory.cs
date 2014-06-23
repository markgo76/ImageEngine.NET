using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Web;

namespace Triplestones.Images
{    
    /// <summary>
    /// Image Factory class for processing and configuring config-driven website image uploads, processing and delivery
    /// Mark Godfrey
    /// Feb 2012
    /// </summary>
	public class ImageFactory
	{

        public enum LegacyImageSizes { Thumbnail, Medium, Feature, Original }
        public enum ImageStorageMode { SV = 1, S3 = 2, AL = 3, DB = 4 } //Server, AmazonS3, All (SV and S3), DB = Blob
        public enum ImageTypes { Main = 1, Feature = 2, Thumb = 3, Logo = 4, Gallery1 = 5, Gallery2 = 6, Gallery3 = 7, Gallery4 = 8, Banner = 9, Gallery5 = 10, Gallery6 = 11, Gallery7 = 12, Gallery8 = 13, Gallery9 = 14, Gallery10 = 15, Gallery11 = 16, Gallery12 = 17 }
        public enum ImageConfigs 
        { 
            NewsMainDetailPage = 2,                 //V6.0 - 476 x 357 //780 x 585 (Detail page)
            NewsMainMedium = 1,                     //V6.0 - 280 x 210 (Main list + Homepages)
            NewsMainCarousalFeature = 4,            //V6.0 - 476 x 357 //500 x 375 (Carousal)
            NewsMainLarge = 5,                      //V6.0 - 340 x 255 (Large listings News Home)
            NewsMainMediumSmall = 6,                //V6.0 - 140 x 105
            NewsMainHUGE = 7,                       //V6.0 - 896 x 672 (VISUAL ARTS SPECIFIC) HUGE
            JobsMainMediumSmall = 2,                //V6.0 - 140 x 105 (HotJobs size Homepages)
            JobsMainMedium = 1,                     //V6.0 - 280 x 210 (Jotw size Homepages)
            MembersResumesMainMediumSmall = 1,      //V6.0 - 140 x 105
            MembersResumesMainMedium = 2,           //V6.0 - 280x210 (Resume detail page)
            WhatsOnMainLarge = 1,                   //V6.0 - 340 x 255 (Large Tab Home)
            WhatsOnMainMedium = 2,                  //V6.0 - 280 x 210 same as News (Detail page)
            WhatsOnMainMediumSmall = 3,             //V6.0 - 140 x 105
            WhatsOnMainSmall = 22,                  //V6.0 - 124 x 93 Used in Bulletins 
            WhatsOnGallery1Thumb = 4,               //V6.0 - 140 x 105 (Gallery 1)
            WhatsOnGallery1Large = 5,               //V6.0 - 340 x 255 (Gallery 1)
            WhatsOnGallery1Feature = 6,             //V6.0 - 780 x 585 (Gallery 1)
            WhatsOnGallery2Thumb = 7,               //V6.0 - 140 x 105 (Gallery 2)
            WhatsOnGallery2Large = 8,               //V6.0 - 340 x 255 (Gallery 2)
            WhatsOnGallery2Feature = 9,             //V6.0 - 780 x 585 (Gallery 2)
            WhatsOnGallery3Thumb = 10,              //V6.0 - 140 x 105 (Gallery 3)
            WhatsOnGallery3Large = 11,              //V6.0 - 340 x 255 (Gallery 3)
            WhatsOnGallery3Feature = 12,            //V6.0 - 780 x 585 (Gallery 3)
            WhatsOnGallery4Thumb = 13,              //V6.0 - 140 x 105 (Gallery 4)
            WhatsOnGallery4Large = 14,              //V6.0 - 340 x 255 (Gallery 4)
            WhatsOnGallery4Feature = 15,            //V6.0 - 780 x 585 (Gallery 4)
            WhatsOnGallery5Thumb = 16,              //V6.0 - 140 x 105 (Gallery 5)
            WhatsOnGallery5Large = 17,              //V6.0 - 340 x 255 (Gallery 5)
            WhatsOnGallery5Feature = 18,            //V6.0 - 780 x 585 (Gallery 5)
            WhatsOnGallery6Thumb = 19,              //V6.0 - 140 x 105 (Gallery 6)
            WhatsOnGallery6Large = 20,              //V6.0 - 340 x 255 (Gallery 6)
            WhatsOnGallery6Feature = 21,            //V6.0 - 780 x 585 (Gallery 6)
            ClassifiedsMainMediumSmall = 3,         //V6.0 - 140 x 105 (Bulletins small)
            ClassifiedsMainMedium = 2,              //V6.0 - 280 x 210 same as News???
            ClassifiedsMainLarge = 1,               //V6.0 - 340 x 255
            GrantsMainMedium = 1,                   //V6.0 - 140 x 105
            GrantsMainLarge = 2,                    //V6.0 - 280 x 210
            DirectoryProfiles_LogoSmall = 1,        //V6.0 - 140 x 105 logo
            DirectoryProfiles_LogoLarge = 2,        //V6.0 - 280 x 210 logo
            DirectoryProfiles_Gallery1_Large = 3,   //V6.0 - 476 x 357 gallery1
            DirectoryProfiles_Gallery1_Small = 4,   //V6.0 - 140 x 105 gallery1
            DirectoryProfiles_Gallery2_Large = 5,   //V6.0 - 476 x 357 gallery2
            DirectoryProfiles_Gallery2_Small = 6,   //V6.0 - 140 x 105 gallery2
            DirectoryProfiles_Gallery3_Large = 7,   //V6.0 - 476 x 357 gallery3
            DirectoryProfiles_Gallery3_Small = 8,   //V6.0 - 140 x 105 gallery3
            DirectoryProfiles_Gallery4_Large = 9,   //V6.0 - 476 x 357 gallery4
            DirectoryProfiles_Gallery4_Small = 10,  //V6.0 - 140 x 105 gallery4
            DirectoryProfiles_Feature = 11,         //V6.0 - 280 x 210 feature
            DirectoryProfiles_Gallery5_Large = 12,   //V6.0 - 476 x 357 gallery1
            DirectoryProfiles_Gallery5_Small = 13,   //V6.0 - 140 x 105 gallery1
            DirectoryProfiles_Gallery6_Large = 14,   //V6.0 - 476 x 357 gallery2
            DirectoryProfiles_Gallery6_Small = 15,   //V6.0 - 140 x 105 gallery2
            DirectoryProfiles_Gallery7_Large = 16,   //V6.0 - 476 x 357 gallery3
            DirectoryProfiles_Gallery7_Small = 17,   //V6.0 - 140 x 105 gallery3
            DirectoryProfiles_Gallery8_Large = 18,   //V6.0 - 476 x 357 gallery4
            DirectoryProfiles_Gallery8_Small = 19,  //V6.0 - 140 x 105 gallery4
            DirectoryProfiles_Gallery9_Large = 20,   //V6.0 - 476 x 357 gallery1
            DirectoryProfiles_Gallery9_Small = 21,   //V6.0 - 140 x 105 gallery1
            DirectoryProfiles_Gallery10_Large = 22,   //V6.0 - 476 x 357 gallery2
            DirectoryProfiles_Gallery10_Small = 23,   //V6.0 - 140 x 105 gallery2
            DirectoryProfiles_Gallery11_Large = 24,   //V6.0 - 476 x 357 gallery3
            DirectoryProfiles_Gallery11_Small = 25,   //V6.0 - 140 x 105 gallery3
            DirectoryProfiles_Gallery12_Large = 26,   //V6.0 - 476 x 357 gallery4
            DirectoryProfiles_Gallery12_Small = 27,  //V6.0 - 140 x 105 gallery4
            MembersAvatar = 1                       //V6.0 - 100 x 100 avatar/logo
        }


        /// <summary>
        /// Gets a Listing Image based on its configId
        /// </summary>
        public static string GetListingImageURL(int SectionId, int ListingId, Images.ImageFactory.ImageConfigs configId)
        {
            if (Config.SiteUsing_AWS_Proxy_Mode)
            {
                return string.Format((System.Web.HttpContext.Current.Request.IsSecureConnection ? Config.URLDomainRootSecure : Config.URLDomainRoot) + "images.ashx?sectionId={0}&listingId={1}&imageConfigId={2}&BypassCloudfront={3}", SectionId, ListingId, (int)configId, false);
            }
            else
            {
                return ProcessImageHandler(HttpContext.Current, SectionId.ToString(), ListingId.ToString(), "", ((int)(configId)).ToString(), "", "", "", "false", "", "", "");
            }
        }

                /// <summary>
        /// Gets a Listing Image based on its configId optionally bypassing CloudFront
        /// </summary>
        public static string GetListingImageURL(int SectionId, int ListingId, Images.ImageFactory.ImageConfigs configId, bool BypassCloudfront)
        {
            return GetListingImageURL(SectionId, ListingId, configId, BypassCloudfront, false);
        }
        /// <summary>
        /// Gets a Listing Image based on its configId optionally bypassing CloudFront
        /// </summary>
        public static string GetListingImageURL(int SectionId, int ListingId, Images.ImageFactory.ImageConfigs configId, bool BypassCloudfront, bool cropPreview)
        {
            if (Config.SiteUsing_AWS_Proxy_Mode)
            {
                return string.Format((System.Web.HttpContext.Current.Request.IsSecureConnection ? Config.URLDomainRootSecure : Config.URLDomainRoot) + "images.ashx?sectionId={0}&listingId={1}&imageConfigId={2}&BypassCloudfront={3}&getcroppreview={4}", SectionId, ListingId, (int)configId, BypassCloudfront, cropPreview);
            }
            else
            {
                return ProcessImageHandler(HttpContext.Current, SectionId.ToString(), ListingId.ToString(), "", ((int)(configId)).ToString(), "", "", "", BypassCloudfront.ToString(), "", cropPreview.ToString(), "");
            }
        }


        /// <summary>
        /// Gets a default (original) Listing Image based on image type
        /// </summary>
        public static string GetListingImageURL(int SectionId, int ListingId, Images.ImageFactory.ImageTypes imageType)
        {
            if (Config.SiteUsing_AWS_Proxy_Mode)
            {
                return string.Format((System.Web.HttpContext.Current.Request.IsSecureConnection ? Config.URLDomainRootSecure : Config.URLDomainRoot) + "images.ashx?sectionId={0}&listingId={1}&imageType={2}&BypassCloudfront={3}", SectionId, ListingId, (int)imageType, false);
            }
            else
            {
                return ProcessImageHandler(HttpContext.Current, SectionId.ToString(), ListingId.ToString(), ((int)imageType).ToString(), "", "", "", "", "false", "", "", "");
            }
        }
        /// <summary>
        /// Gets a default (original) Listing Image based on image type, and optionally goes straight to S3 for better editing response
        /// </summary>
        public static string GetListingImageURL(int SectionId, int ListingId, Images.ImageFactory.ImageTypes imageType, bool BypassCloudfront)
        {
            if (Config.SiteUsing_AWS_Proxy_Mode)
            {
                return string.Format((System.Web.HttpContext.Current.Request.IsSecureConnection ? Config.URLDomainRootSecure : Config.URLDomainRoot) + "images.ashx?sectionId={0}&listingId={1}&imageType={2}&BypassCloudfront={3}", SectionId, ListingId, (int)imageType, BypassCloudfront);
            }
            else
            {
                return ProcessImageHandler(HttpContext.Current, SectionId.ToString(), ListingId.ToString(), ((int)imageType).ToString(), "", "", "", "", BypassCloudfront.ToString(), "", "", "");
            }
        }
        /// <summary>
		/// For saving as a physical file
		/// </summary>
		public static bool SaveUploadImageAsJpeg(Stream ImageStream, string OutpuFilePath, String ImageName, Int32 ImageWidth, Int32 ImageHeight, Boolean FixedDimensions)
		{
            ImageStream.Position = 0;
            String Extension = ".jpg";
			String MimeType = "image/jpeg";
			long Quality = 85;
			MemoryStream nul;
			return UploadImage(ImageStream, out nul, true, OutpuFilePath, ImageName, ImageWidth, ImageHeight, Extension, MimeType, Quality, FixedDimensions);
		}

		public static bool SaveUploadImageAsGif(Stream ImageStream, string OutpuFilePath, String ImageName, Int32 ImageWidth, Int32 ImageHeight, Boolean FixedDimensions)
		{
            ImageStream.Position = 0;
            String Extension = ".gif";
			String MimeType = "image/gif";
			long Quality = 100;
			MemoryStream nul;
			return UploadImage(ImageStream, out nul, true, OutpuFilePath, ImageName, ImageWidth, ImageHeight, Extension, MimeType, Quality, FixedDimensions);
		}

		public static bool SaveUploadImageAsPng(Stream ImageStream, string OutpuFilePath, String ImageName, Int32 ImageWidth, Int32 ImageHeight, Boolean FixedDimensions)
		{
            ImageStream.Position = 0;
            String Extension = ".png";
			String MimeType = "image/png";
			long Quality = 0; // has no effect on png files
			MemoryStream nul;
			return UploadImage(ImageStream, out nul, true, OutpuFilePath, ImageName, ImageWidth, ImageHeight, Extension, MimeType, Quality, FixedDimensions);
		}

		/// <summary>
		/// For saving as a memory stream
		/// </summary>
		public static bool SaveUploadImageAsJpeg(Stream ImageStream, out MemoryStream OutpuStream, Int32 ImageWidth, Int32 ImageHeight, Boolean FixedDimensions)
		{
            ImageStream.Position = 0;
			String Extension = ".jpg";
			String MimeType = "image/jpeg";
			long Quality = 85;
			return UploadImage(ImageStream, out OutpuStream, false, "", "", ImageWidth, ImageHeight, Extension, MimeType, Quality, FixedDimensions);
		}

		public static bool SaveUploadImageAsGif(Stream ImageStream, out MemoryStream OutpuStream, Int32 ImageWidth, Int32 ImageHeight, Boolean FixedDimensions)
		{
            ImageStream.Position = 0;
            String Extension = ".gif";
			String MimeType = "image/gif";
			long Quality = 100;
			return UploadImage(ImageStream, out OutpuStream, false, "", "", ImageWidth, ImageHeight, Extension, MimeType, Quality, FixedDimensions);
		}

		public static bool SaveUploadImageAsPng(Stream ImageStream, out MemoryStream OutpuStream, Int32 ImageWidth, Int32 ImageHeight, Boolean FixedDimensions)
		{
            ImageStream.Position = 0;
            String Extension = ".png";
			String MimeType = "image/png";
			long Quality = 0; // has no effect on png files
			return UploadImage(ImageStream, out OutpuStream, false, "", "", ImageWidth, ImageHeight, Extension, MimeType, Quality, FixedDimensions);
		}

		private static bool UploadImage(Stream ImageStream, out MemoryStream OutputStream, bool isFile, string OutpuFilePath, String ImageName, Int32 MaxWidth, Int32 MaxHeight, String Extension, String MimeType, long Quality, Boolean FixedDimensions)
		{
			try
			{
				System.Drawing.Image UploadedImage = System.Drawing.Image.FromStream(ImageStream);

                OutputStream = null;

                // Resizing Calculations
                double dZoom, OriginalWidth, OriginalHeight;
                Int32 FinalWidth, FinalHeight;

                if (!FixedDimensions)
                {
                    OriginalWidth = UploadedImage.Width;
                    OriginalHeight = UploadedImage.Height;

                    if (MaxHeight == 0)
                    {
                        dZoom = MaxWidth / OriginalWidth;		//aspect ratio is determined by width
                    }
                    else if (MaxWidth == 0)
                    {
                        dZoom = MaxHeight / OriginalHeight;		//aspect ratio is determined by height
                    }
                    else
                    {
                        if ((OriginalWidth / MaxWidth) > (OriginalHeight / MaxHeight))
                            dZoom = MaxWidth / OriginalWidth;		//aspect ratio is determined by width
                        else
                            dZoom = MaxHeight / OriginalHeight;		//aspect ratio is determined by height
                    }

                    FinalWidth = Convert.ToInt32(Math.Ceiling(OriginalWidth * dZoom));
                    FinalHeight = Convert.ToInt32(Math.Ceiling(OriginalHeight * dZoom));

                    // Do not resize images that are smaller
                    if ((OriginalWidth <= FinalWidth) || (OriginalHeight <= FinalHeight))
                    {
                        FinalWidth = Convert.ToInt32(OriginalWidth);
                        FinalHeight = Convert.ToInt32(OriginalHeight);
                    }
                }
                else
                {
                    FinalWidth = MaxWidth;
                    FinalHeight = MaxHeight;
                }

                // Perform Resize
                Bitmap smallerImg = new Bitmap(FinalWidth, FinalHeight);
                Graphics g = Graphics.FromImage(smallerImg);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                foreach (PropertyItem pItem in UploadedImage.PropertyItems)
                {
                    smallerImg.SetPropertyItem(pItem);
                }

                g.DrawImage(UploadedImage, 0, 0, FinalWidth, FinalHeight);

                UploadedImage.Dispose();

                if (!Extension.Equals(".png"))
                {
                    EncoderParameters myParams = new EncoderParameters(1);
                    myParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Quality);

                    if (isFile)
                    {
                        OutputStream = null;
                        smallerImg.Save(OutpuFilePath + ImageName + Extension, GetEncoderInfo(MimeType), myParams);
                    }
                    else
                    {
                        OutputStream = new MemoryStream();
                        smallerImg.Save(OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //byte[] output = stream.ToArray();
                    }
                }
                else
                {
                    if (isFile)
                    {
                        OutputStream = null;
                        smallerImg.Save(OutpuFilePath + ImageName + Extension, ImageFormat.Png);
                    }
                    else
                    {
                        OutputStream = new MemoryStream();
                        smallerImg.Save(OutputStream, ImageFormat.Png);
                    }
                }

                smallerImg.Dispose();

				return true;
			}
			catch (Exception ex)
			{
                BLL.Emailing.Emailing.EmailBug("Problem in ImageFactory.UploadImage", ex.Message + " | " + ex.StackTrace);
				OutputStream = null;
				return false;
			}
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for (j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType == mimeType)
				{
					return encoders[j];
				}
			}
			return null;
		}

        /// <summary>
        /// Gets an Image data source from a http request
        /// </summary>
        public static byte[] GetImage(string filename)
        {
            return GetImage(filename, false);
        }
        /// <summary>
        /// Gets an Image data source from a http request
        /// </summary>
        public static byte[] GetImage(string filename, bool forceProxyCall)
        {
            byte[] imageData = null;

            if (Config.SiteUsing_AWS_Proxy_Mode || forceProxyCall)
            {

                try
                {

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(filename);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = response.GetResponseStream();
                        using (BinaryReader br = new BinaryReader(responseStream))
                        {
                            imageData = br.ReadBytes((int)response.ContentLength);
                        }
                    }

                    return imageData;
                }
                catch (Exception ex)
                {
                    // 404s from S3 will come into here
                    return null;
                }
            }
            else
            {
                return imageData;
            }

        }

        /// <summary>
        /// Returns true if a record exists in tblListingImages for this ListingId/SectionId/ImageType
        /// </summary>
        public static bool ImageExists(int ListingId, Globals.SectionIds SectionId, int ImageType)
        {
            return GetImageId(ListingId, SectionId, ImageType) > 0;
        }

        private static object cacheLocker = new object();
        protected static int GetImageId(int ListingId, Globals.SectionIds SectionId, int ImageType)
        {
            string cacheKey = "Other_" + ListingId + "_ImageId_" + ImageType.ToString(); ;
            string cacheKeyJobs = "Jobs_Job_" + ListingId + "_ImageId_" + ImageType.ToString();
            string cacheKeyNews = "Content_ContentMain_" + ListingId + "_ImageId_" + ImageType.ToString();
            string cacheKeyClassies = "Classifieds_Classified_" + ListingId + "_ImageId_" + ImageType.ToString();
            string cacheKeyWhatsOn = "WhatsOn_WhatsOn_" + ListingId + "_ImageId_" + ImageType.ToString();
            string cacheKeyProfiles = "DirectoryProfiles_DirectoryProfile_" + ListingId + "_ImageId_" + ImageType.ToString();
            string cacheKeyResumes = "MembersResumes_MemberResume_" + ListingId + "_ImageId_" + ImageType.ToString();
            string cacheKeyMembers = "Members_Members_GetMemberProfile_" + ListingId + "_ImageId_" + ImageType.ToString();
            string cacheKeyGrants = "Grants_Grant_GetGrant_" + ListingId + "_ImageId_" + ImageType.ToString();

            switch (SectionId)
            {
                case Globals.SectionIds.Jobs:
                    cacheKey = cacheKeyJobs;
                    break;
                case Globals.SectionIds.News:
                    cacheKey = cacheKeyNews;
                    break;
                case Globals.SectionIds.WhatsOn:
                    cacheKey = cacheKeyWhatsOn;
                    break;
                case Globals.SectionIds.Classifieds:
                    cacheKey = cacheKeyClassies;
                    break;
                case Globals.SectionIds.DirectoryProfiles:
                    cacheKey = cacheKeyProfiles;
                    break;
                case Globals.SectionIds.MembersResumes:
                    cacheKey = cacheKeyResumes;
                    break;
                case Globals.SectionIds.Members:
                    cacheKey = cacheKeyMembers;
                    break;
                case Globals.SectionIds.Grants:
                    cacheKey = cacheKeyGrants;
                    break;
            }

            int ImageId = 0;
            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                // Lock it, get it, cache it
                lock (cacheLocker)
                {
                    HttpContext.Current.Cache[cacheKey] = ImageId = DAL.DBProvider.Listings.ListingImage_GetImageId(ListingId, (int)SectionId, ImageType, true);
                }
            }
            else
            {
                // Serve from cache
                ImageId = (int)HttpContext.Current.Cache[cacheKey];
            }

            return ImageId;
        }

        public static string ProcessImageHandler(HttpContext context, string qsectionId, string qListingId, string qimageType, string qimageConfigId, string qimageId, string qfilename, string qorgId, string qbypassCloudfront, string qgetUncropped, string qgetCropPreview, string qfileExt)
        {

            // Get variables
            Globals.SectionIds SectionId = (Globals.SectionIds)int.Parse(qsectionId);
            IS3Configurable thisSection = GetS3ImageConfigSection(SectionId);
            ImageConfigItem ImageConfig = null;

            int ListingId;
            if (!int.TryParse(qListingId, out ListingId)) ListingId = 0;
            int ImageType = 0;

            int ListingIdReal = ListingId;

            if (SectionId != Globals.SectionIds.Advertising)
            {
                ImageConfig = thisSection.ImageConfigs[qimageConfigId];
                if (!int.TryParse(qimageType, out ImageType))
                {
                    // Get Image type from config
                    ImageType = (int)Enum.Parse(typeof(ImageFactory.ImageTypes), ImageConfig.ImageType);
                }

            }


            bool BypassCloudFront;
            if (!bool.TryParse(qbypassCloudfront, out BypassCloudFront)) BypassCloudFront = false;
            bool GetUncropped;
            if (!bool.TryParse(qgetUncropped, out GetUncropped)) GetUncropped = false;
            bool GetCropPreview;
            if (!bool.TryParse(qgetCropPreview, out GetCropPreview)) GetCropPreview = false;
            string originalFilename = "";
            string filename = "";
            string defaultFilename = "";
            int S3MaxCache = 0;
            string S3CloudFrontDistributionId = "";
            string BucketName = "";
            string CloudFrontURL = "";
            string DynamicResizeFilename = "";
            string noImage = "noimage.jpg";
            string unproxyOutput = "/test.jpg";

            string originalContentType = context.Response.ContentType;
            context.Response.ContentType = "image/jpeg";

            // **********************************************************************************************************************************************
            // **********************************************************************************************************************************************
            // MG AUG 2013 - Do listingId / imageId rewrite right here - then no templates need to be changed
            if (!GetUncropped && !GetCropPreview) // We are passing the imageId straight in for a get uncropped request - so dont try and look it up in tblListingImages
            {
                ListingId = GetImageId(ListingId, SectionId, ImageType);
            }

            // **********************************************************************************************************************************************
            // **********************************************************************************************************************************************

            // Stop image caching at client
            //context.Response.Cache.SetExpires(DateTime.Now.AddMinutes(-1));
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (SectionId == Globals.SectionIds.Advertising)
            {
                try
                {

                // Do AdServer Proxying here, separate to main image handling
                filename = qfilename;

                // Adapt encoding to file type
                string fileExtension = filename.Split('.')[1];
                switch (fileExtension)
                {
                    case "gif":
                        context.Response.ContentType = "image/gif";
                        break;
                    case "png":
                        context.Response.ContentType = "image/png";
                        break;
                    case "tiff":
                        context.Response.ContentType = "image/tiff";
                        break;
                    case "tif":
                        context.Response.ContentType = "image/tiff";
                        break;
                    case "bmp":
                        context.Response.ContentType = "image/bmp";
                        break;
                    default:
                        context.Response.ContentType = "image/jpeg";
                        break;
                }


                    string BannerMediaExternalURL = string.Format(Globals.Settings.AdServer.CloudFrontURL, filename);
                    byte[] imageData = Images.ImageFactory.GetImage(BannerMediaExternalURL);

                    if (Config.SiteUsing_AWS_Proxy_Mode)
                    {
                        context.Response.BinaryWrite(imageData);
                    }
                    else
                    {
                        // As we're in a handler we can't just return a string with the URL of the image
                        // So we'll wrap it up in a context var, and reset the content type back to whatever it was
                        if (qsectionId == "3" || qsectionId == "9")
                        {
                            unproxyOutput = BannerMediaExternalURL;
                        }
                        else
                        {
                            unproxyOutput = BannerMediaExternalURL.Replace("/arthur", "");
                        }
                        context.Response.ContentType = originalContentType;
                        return unproxyOutput;
                    }
                }
                catch (Exception ex)
                {
                    BLL.Emailing.Emailing.EmailBug("Error serving banner media from ImageFactory", "Filename = " + filename + ", " + ex.Message + " : " + ex.StackTrace);
                }
            }

            else
            {

                // All other main images
                try
                {
                    ImageConfig = thisSection.ImageConfigs[qimageConfigId];
                    noImage = NoImageName(ImageConfig);
                }
                catch { }

                string fileExtension = "jpg";

                if (qfileExt != null && qfileExt != "")
                {
                    fileExtension = qfileExt;
                }

                switch (fileExtension)
                {
                    case "gif":
                        context.Response.ContentType = "image/gif";
                        break;
                    case "png":
                        context.Response.ContentType = "image/png";
                        break;
                    case "tiff":
                        context.Response.ContentType = "image/tiff";
                        break;
                    case "tif":
                        context.Response.ContentType = "image/tiff";
                        break;
                    case "bmp":
                        context.Response.ContentType = "image/bmp";
                        break;
                    default:
                        context.Response.ContentType = "image/jpeg";
                        break;
                }


                filename = string.Format(BypassCloudFront ? thisSection.S3URL : thisSection.CloudFrontURL, thisSection.ManagedImagesS3Path + (GetUncropped ? "uncropped/" : "") + thisSection.ImageBaseFileName + "." + fileExtension);
                defaultFilename = string.Format(thisSection.CloudFrontURL, thisSection.ManagedImagesS3Path + noImage);

                
                S3MaxCache = thisSection.CacheMaxAgeAWS;
                S3CloudFrontDistributionId = thisSection.CloudFrontDistributionId;
                CloudFrontURL = thisSection.CloudFrontURL;
                BucketName = thisSection.Bucket;
                DynamicResizeFilename = thisSection.ManagedImagesS3Path + thisSection.ImageBaseFileName + ".jpg";


                if (ImageConfig == null)
                {

                    if (ImageType != -1)
                    {
                        //Serve original for type
                        originalFilename = string.Format(filename, (int)SectionId, ListingId, (ImageFactory.ImageTypes)ImageType, 0, 0, 0);

                        byte[] thisImageData = Images.ImageFactory.GetImage(originalFilename);

                        // FYI GetImage byte[] will ALWAYS return NULL when ProxyMode is turned off

                        if (thisImageData != null)
                        {
                            if (Config.SiteUsing_AWS_Proxy_Mode) context.Response.BinaryWrite(thisImageData);
                        }
                        else
                        {
                            thisImageData = Images.ImageFactory.GetImage(defaultFilename);
                            if (thisImageData == null && Config.EnvironmentMode != Config.EnvironmentModes.Live) thisImageData = Images.ImageFactory.GetImage(defaultFilename.Replace("/" + Config.URLDomainMode, ""));

                            if (thisImageData != null)
                            {
                                // Server no-image image
                                if (Config.SiteUsing_AWS_Proxy_Mode) context.Response.BinaryWrite(thisImageData);
                            }
                            else
                            {
                                // Even the no image is 404 - or proxy mode is off
                                if (Config.SiteUsing_AWS_Proxy_Mode)
                                {
                                    // Return empty bytes if proxy on
                                    context.Response.BinaryWrite(new byte[1]);
                                }
                                else
                                {
                                    // Else just attempt to set the original URL
                                    if (Config.EnvironmentMode == Config.EnvironmentModes.Live)
                                    {
                                        unproxyOutput = originalFilename;
                                    }
                                    else
                                    {
                                        unproxyOutput = originalFilename.Replace(Config.URLDomainMode + "/", "");
                                    }
                                    context.Response.ContentType = originalContentType;
                                }
                            }
                        }
                    }
                    else
                    {
                        // No matching config in system - serve default
                        if (Config.SiteUsing_AWS_Proxy_Mode)
                        {
                            context.Response.BinaryWrite(Images.ImageFactory.GetImage(defaultFilename));
                        }
                        else
                        {
                            // As we're in a handler we can't just return a string with the URL of the image
                            // So we'll wrap it up in a context var, and reset the content type back to whatever it was
                            if (Config.EnvironmentMode == Config.EnvironmentModes.Live)
                            {
                                unproxyOutput = defaultFilename;
                            }
                            else
                            {
                                unproxyOutput = defaultFilename.Replace(Config.URLDomainMode + "/", "");
                            }
                            context.Response.ContentType = originalContentType;
                        }
                    }
                }
                else
                {

                    // [sectionId]-[ListingId]-[ImageType]-[Width]x[Height]-[ImageConfigId].jpg

                    // Return specific configuration image
                    originalFilename = string.Format(filename, (int)SectionId, ListingId, ImageConfig.ImageType, 0, 0, 0);
                    filename = string.Format(filename, (int)SectionId, ListingId, ImageConfig.ImageType, ImageConfig.Width, ImageConfig.Height, ImageConfig.ImageConfigId);
                    DynamicResizeFilename = string.Format(DynamicResizeFilename, (int)SectionId, ListingId, ImageConfig.ImageType, ImageConfig.Width, ImageConfig.Height, ImageConfig.ImageConfigId);


                    byte[] imageData = ListingId == 0 ? null : Images.ImageFactory.GetImage(filename); // Shorcut if listingId = 0 - dont try and retrieve data
                    

                    if (imageData == null)
                    {
                        // FYI - It always will be NULL when proxy mode turned off
                        // Missing image - try to get original
                        imageData = Images.ImageFactory.GetImage(originalFilename);


                        if (imageData == null)
                        {
                            // No original, write default image
                            if (Config.SiteUsing_AWS_Proxy_Mode)
                            {
                                context.Response.BinaryWrite(Images.ImageFactory.GetImage(Config.EnvironmentMode == Config.EnvironmentModes.Live ? defaultFilename : defaultFilename.Replace(Config.URLDomainMode + "/", "")));
                            }
                            else
                            {
                                if (ListingId == 0) filename = defaultFilename; // Set to no-image where no record in tblListingImages
                                if (Config.EnvironmentMode == Config.EnvironmentModes.Live)
                                {
                                    unproxyOutput = filename;
                                }
                                else
                                {
                                    unproxyOutput = filename.Replace(Config.URLDomainMode + "/", "");
                                }
                                context.Response.ContentType = originalContentType;
                            }
                        }
                        else
                        {
                            // When Proxy mode off - you never get in here, no resize can happen on-the-fly
                            // Resize original into missing version and save
                            MemoryStream output;
                            Stream imageStream = new MemoryStream(imageData);
                            if (ImageFactory.SaveUploadImageAsJpeg(imageStream, out output, ImageConfig.Width, ImageConfig.Height, false))
                            {
                                // Upload resize to Amazon S3
                                AmazonHelper.PutMedia(BucketName, DynamicResizeFilename, output, S3MaxCache);
                                //AmazonHelper.InvalidateContent(S3CloudFrontDistributionId, DynamicResizeFilename); - No need to invalidate as file was missing - new filename is created
                            }

                            // Serve original as resize does nto appear to be immediately serveable
                            context.Response.BinaryWrite(imageData);
                        }
                    }
                    else
                    {
                        // Write good config image
                        if (Config.SiteUsing_AWS_Proxy_Mode) context.Response.BinaryWrite(imageData);
                    }

                }
            }

            if (Config.SiteUsing_AWS_Proxy_Mode)
            {
                return null;
            }
            else
            {
                context.Response.ContentType = originalContentType;
                return unproxyOutput;
            } 
        }

        /// <summary>
        /// Formulate correct size noimage
        /// </summary>
        public static string NoImageName(ImageConfigItem imageConfig)
        {
            int width = imageConfig.Width;
            int height = imageConfig.Height;
            return "noimage" + width.ToString() + "x" + height.ToString() + ".jpg";
        }

        public static void Go301Redirect40(string URL, HttpContext context)
        {
            context.Response.RedirectPermanent(URL);
        }
        public static void Go301Redirect35(string URL, HttpContext context)
        {
            context.Response.Clear();
            context.Response.Status = "301 Moved Permanently";
            context.Response.AddHeader("Location", URL);
            context.Response.StatusCode = 301;
            context.Response.End();
        }

        /// <summary>
        /// Returns the S3 - Configured Sectioanl Configs for doing Image Processing
        /// </summary>
        public static IS3Configurable GetS3ImageConfigSection(Globals.SectionIds SectionId)
        {
            IS3Configurable S3SectionConfig;
            switch (SectionId)
            {
                case Globals.SectionIds.Jobs:
                    S3SectionConfig = Globals.Settings.Jobs;
                    break;
                case Globals.SectionIds.News:
                default:
                    S3SectionConfig = Globals.Settings.Content;
                    break;
                case Globals.SectionIds.WhatsOn:
                    S3SectionConfig = Globals.Settings.WhatsOn;
                    break;
                case Globals.SectionIds.Classifieds:
                    S3SectionConfig = Globals.Settings.Classifieds;
                    break;
                case Globals.SectionIds.DirectoryProfiles:
                    S3SectionConfig = Globals.Settings.DirectoryProfiles;
                    break;
                case Globals.SectionIds.MembersResumes:
                    S3SectionConfig = Globals.Settings.MembersResumes;
                    break;
                case Globals.SectionIds.Members:
                    S3SectionConfig = Globals.Settings.Members;
                    break;
                case Globals.SectionIds.Grants:
                    S3SectionConfig = Globals.Settings.Grants;
                    break;

            }

            return S3SectionConfig;
        }

        public static byte[] GetCroppedImage(string imageURL, int x, int y, int width, int height)
        {
            // Open file
            WebClient wc = new WebClient();

            using (Image OriginalImage = System.Drawing.Image.FromStream(wc.OpenRead(imageURL)))
            {
                using (Bitmap bmp = new Bitmap(width, height))
                {
                    bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);

                    using (Graphics Graphic = Graphics.FromImage(bmp))
                    {
                        Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                        Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        Graphic.DrawImage(OriginalImage, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);

                        MemoryStream ms = new MemoryStream();
                        bmp.Save(ms, OriginalImage.RawFormat);

                        return ms.GetBuffer();

                    }

                }

            }

        }

        /// <summary>
        /// Processes an image for a listing, resizing etc where config specifies
        /// </summary>
        public static ImageConfigItem ProcessListingImage(ImageStorageMode storageMode, ImageTypes imageType, Telerik.Web.UI.UploadedFile thisFile, string cavPath, string S3BucketName, string S3CloudFrontDistributionId, string CloudFrontBaseURL, string S3Path, int S3MaxCache, int sectionId, int ListingId, string baseFilename, ImageConfigArray configArray)
        {
            return ProcessListingImage(storageMode, imageType, thisFile.InputStream, cavPath, S3BucketName, S3CloudFrontDistributionId, CloudFrontBaseURL, S3Path, S3MaxCache, sectionId, ListingId, baseFilename, configArray);
        }

        /// <summary>
        /// Returns true if a given image is a portrait aspect ratio
        /// </summary>
        public static bool IsPortrait(Image image)
        {
            return image.Height >= image.Width;
        }

        /// <summary>
        /// Adds canvas space around an image if its aspect ratio does not match the imagetype/config ratio
        /// </summary>
        public static Stream AddImageCanvas(Stream fileStream, ImageTypes ImageType, IS3Configurable S3SectionConfig, Color backgroundColor)
        {
            ImageConfigArray theseConfigs = S3SectionConfig.ImageConfigs.GetConfigsByImageType(ImageType.ToString());
            return AddImageCanvas(fileStream, theseConfigs, S3SectionConfig, backgroundColor);
        }

        /// <summary>
        /// Adds canvas space around an image if its aspect ratio does not match the imagetype/config ratio
        /// </summary>
        public static Stream AddImageCanvas(Stream fileStream, ImageConfigArray theseConfigs, IS3Configurable S3SectionConfig, Color backgroundColor)
        {
            using (Image thisImage = Image.FromStream(fileStream))
            {
                ImageConfigItem referenceConfig = theseConfigs[theseConfigs.Count - 1];

                if (AspectRatiosMatch(thisImage, referenceConfig, 2))
                {
                    // Ratio matches, just pass original stream back
                    return fileStream;
                }
                else
                {
                    // Add canvas
                    int newWidth, newHeight, newX, newY;
                    if (IsPortrait(thisImage))
                    {
                        newHeight = thisImage.Height;
                        newWidth = (int)Math.Round((decimal)newHeight * ((decimal)referenceConfig.Width / (decimal)referenceConfig.Height), 0);

                        if (newWidth < thisImage.Width)
                        {
                            // Image is not "portrait enough" - expand height to get all the width in
                            decimal widthDiff = (decimal)thisImage.Width / (decimal)newWidth;
                            newHeight = (int)Math.Round((thisImage.Height * widthDiff), 0);
                            newWidth = (int)Math.Round((decimal)newHeight / ((decimal)referenceConfig.Height / (decimal)referenceConfig.Width), 0);
                        }

                        newX = (newWidth / 2) - (thisImage.Width / 2);
                        newY = (newHeight / 2) - (thisImage.Height / 2);
                    }
                    else
                    {
                        newWidth = thisImage.Width;
                        newHeight = (int)Math.Round((decimal)newWidth / ((decimal)referenceConfig.Width / (decimal)referenceConfig.Height), 0);

                        if (newHeight < thisImage.Height)
                        {
                            // Image is not "landscape enough" - expand width to get all the height in
                            decimal heightDiff = (decimal)thisImage.Height / (decimal)newHeight;
                            newWidth = (int)Math.Round((thisImage.Width * heightDiff) ,0);
                            newHeight = (int)Math.Round((decimal)newWidth / ((decimal)referenceConfig.Width / (decimal)referenceConfig.Height), 0);
                        }

                        newX = (newWidth / 2) - (thisImage.Width / 2);
                        newY = (newHeight / 2) - (thisImage.Height / 2);
                    }

                    using (Bitmap bmp = new Bitmap(newWidth, newHeight))
                    {
                        bmp.SetResolution(thisImage.HorizontalResolution, thisImage.VerticalResolution);

                        using (Graphics Graphic = Graphics.FromImage(bmp))
                        {
                            Graphic.FillRectangle(new SolidBrush(backgroundColor), 0, 0, newWidth, newHeight);
                            Graphic.SmoothingMode = SmoothingMode.None;
                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(thisImage, newX, newY, new Rectangle(0,0,thisImage.Width, thisImage.Height), GraphicsUnit.Pixel);

                            MemoryStream ms = new MemoryStream();
                            bmp.Save(ms, thisImage.RawFormat);

                            return ms;

                        }

                    }

                }

            }
        }

        /// <summary>
        /// Returns whether a source image matches an ImageConfig - int accuracy the no. of decimal places that must match (default 2)
        /// </summary>
        /// <param name="imageToCheck">The Image object to check</param>
        /// <param name="configToCompare">The ImageConfig item to compare</param>
        /// <param name="accuracy">The decimal places to match in the compare (default 2)</param>       
        public static bool AspectRatiosMatch(Image imageToCheck, ImageConfigItem configToCompare, int? accuracy)
        {
            int _accuracy = 2;
            decimal imageRatio = (decimal)imageToCheck.Width / (decimal)imageToCheck.Height;
            decimal configRatio = (decimal)configToCompare.Width / (decimal)configToCompare.Height;


            return (Math.Round(imageRatio, _accuracy) == Math.Round(configRatio, _accuracy));

        }

        /// <summary>
        /// Processes an image for a listing, resizing etc where config specifies
        /// </summary>
        public static ImageConfigItem ProcessListingImage(ImageStorageMode storageMode, ImageTypes imageType, Stream imageStream, string cavPath, string S3BucketName, string S3CloudFrontDistributionId, string CloudFrontBaseURL, string S3Path, int S3MaxCache, int sectionId, int ListingId, string baseFilename, ImageConfigArray configArray) 
        {
            try
            {
                System.Drawing.Image UploadedImage = System.Drawing.Image.FromStream(imageStream);
                int OriginalWidth = UploadedImage.Width;
                int OriginalHeight = UploadedImage.Height;
                string fileName = "";
                ImageConfigItem previewConfig = null;

                if (storageMode == ImageStorageMode.SV || storageMode == ImageStorageMode.AL)
                {
                    // Do Hostworks network path storage                    

                    foreach (Server s in MultiServerConfig.Servers)
                    {
                        if (s.ServerName == "enid" || s.ServerName == "beta" || Config.EnvironmentMode == Config.EnvironmentModes.Live)
                        {

                            // Original Image
                            fileName = string.Format(baseFilename, sectionId.ToString(), ListingId.ToString(), imageType.ToString(), 0, 0, 0);
                            ImageFactory.SaveUploadImageAsJpeg(imageStream, s.UNCPath + cavPath, fileName, OriginalWidth, OriginalHeight, true);

                            // Cycle thru Configs
                            foreach (ImageConfigItem i in configArray)
                            {
                                fileName = string.Format(baseFilename, sectionId.ToString(), ListingId.ToString(), imageType.ToString(), i.Width, i.Height, i.ImageConfigId);
                                ImageFactory.SaveUploadImageAsJpeg(imageStream, s.UNCPath + cavPath, fileName, i.Width, i.Height, false);
                            }

                        }
                    }

                }

                if (storageMode == ImageStorageMode.S3 || storageMode == ImageStorageMode.AL)
                {
                    // Do Amazon S3 bucket storage

                    MemoryStream output;
                    List<string> invalidationList = new List<string>();

                    // Original Image
                    fileName = string.Format(baseFilename, sectionId.ToString(), ListingId.ToString(), imageType.ToString(), 0, 0, 0) + ".jpg";

                    // Check original for this ID exists - if not, we don't need to invalidate anything if it's new
                    bool needsInvalidation = (Images.ImageFactory.GetImage(string.Format(CloudFrontBaseURL, S3Path + fileName), true) != null);

                    if (ImageFactory.SaveUploadImageAsJpeg(imageStream, out output, OriginalWidth, OriginalHeight, true))
                    {
                        
                        AmazonHelper.PutMedia(S3BucketName, S3Path + fileName, output, S3MaxCache);
                        invalidationList.Add(S3Path + fileName); // Now batch-invalidating below
                        //AmazonHelper.InvalidateContent(S3CloudFrontDistributionId, S3Path + fileName);
                    }

                    // Cycle thru Configs
                    foreach (ImageConfigItem i in configArray)
                    {
                        if (i.Active)
                        {
                            fileName = string.Format(baseFilename, sectionId.ToString(), ListingId.ToString(), imageType.ToString(), i.Width, i.Height, i.ImageConfigId) + ".jpg";
                            previewConfig = i;

                            if (ImageFactory.SaveUploadImageAsJpeg(imageStream, out output, i.Width, i.Height, i.FixedDimensions))
                            {
                                // Upload media to Amazon S3
                                AmazonHelper.PutMedia(S3BucketName, S3Path + fileName, output, S3MaxCache);
                                invalidationList.Add(S3Path + fileName); // Now batch-invalidating below
                                //AmazonHelper.InvalidateContent(S3CloudFrontDistributionId, S3Path + fileName);
                            }
                        }
                    }

                    // Do invalidations if needed
                    if (needsInvalidation) AmazonHelper.InvalidateContent(S3CloudFrontDistributionId, invalidationList);
                }

                return previewConfig;

            }
            catch (Exception ex)
            {
                BLL.Emailing.Emailing.EmailBug(ex.Source, ex.Message + "|" + ex.StackTrace);
                return null;
            }
        }



    public interface IS3Configurable
    {
        string Bucket { get; set; }
        string S3URL { get; set; }
        string CloudFrontDistributionId { get; set; }
        string CloudFrontURL { get; set; }
        string ManagedImagesS3Path { get; set; }
        string ManagedImagesSVPath { get; set; }
        int CacheMaxAgeAWS { get; set; }
        int SectionId { get; set; }
        string ImageBaseFileName { get; set; }
        ImageConfigArray ImageConfigs { get; }
        ImageFactory.ImageStorageMode ManagedImagesStorageMode { get; set; }

    }

}