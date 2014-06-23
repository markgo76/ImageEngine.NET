using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace Triplestones.Images
{
    /// <summary>
    /// Http Handler for Image upload/processing requests from FineUpload / jCrop
    /// Mark Godfrey
    /// Feb 2012
    /// </summary>
    public class ImageUploadHandler : IHttpHandler
    {
        class fineUploadJSONResponse
        {
            public bool success { get; set; }
            public string uploadName { get; set; }
            public string url { get; set; }
        }

        public void ProcessRequest(HttpContext context)
        {

            if (context.Request.Files.Count > 0)
            {
                // We have attachments to upload
                for (int i = 0; i < context.Request.Files.Count; i++)
                {
                    try
                    {
                        HttpPostedFile file = context.Request.Files[i];

                        if (file.ContentLength > 0)
                        {
                            // Get SectionId, ImageTypeId from request
                            int SectionId = context.Request["sectionid"] != null ? int.Parse(context.Request["sectionid"]) : 0;
                            int Imagetype = context.Request["imagetype"] != null ? int.Parse(context.Request["imagetype"]) : 0;

                            // Get imageId from new config table, and rename uncropped version befor initial upload
                            int imageId = DAL.DBProvider.Listings.ListingImage_Insert(null, SectionId, Imagetype, true);

                                
                            //[sectionId]-[ListingId]-[ImageType]-[Width]x[Height]-[ImageConfigId].jpg
                            // Grab Sectionalised configs for S3
                            IS3Configurable S3SectionConfig = ImageFactory.GetS3ImageConfigSection((Globals.SectionIds)SectionId);

                            // Add canvas around image to make non-aspect-ratio source images fully croppable
                            Stream canvassedImageStream = ImageFactory.AddImageCanvas(file.InputStream, (ImageFactory.ImageTypes)Imagetype, S3SectionConfig, System.Drawing.Color.White);

                            // Put it on S3
                            string formattedFilename = string.Format(S3SectionConfig.ImageBaseFileName + ".{6}", SectionId, imageId, ((ImageFactory.ImageTypes)Imagetype).ToString(), 0, 0, 0, file.FileName.Split('.')[1].ToLower());
                            //BLL.Emailing.Emailing.EmailMark("File received : " + file.FileName + " (" + formattedFilename + "), type is " + file.ContentType + ", size is " + file.ContentLength, "");
                            bool result = AmazonHelper.PutMedia(S3SectionConfig.Bucket, S3SectionConfig.ManagedImagesS3Path + "uncropped/" + formattedFilename, canvassedImageStream, S3SectionConfig.CacheMaxAgeAWS);


                            // Write JSON back to fineUploader, using proxy if SSL
                            if (System.Web.HttpContext.Current.Request.IsSecureConnection)
                            {
                                string returnSSLFileName = UrlTools.GetServerUrl() + string.Format("/images.ashx?sectionId={0}&listingId={1}&imageType={2}&imageConfigId=0&bypasscloudfront=true&getuncropped=true&fileext={3}", SectionId, imageId, Imagetype, file.FileName.Split('.')[1].ToLower());
                                WriteFineUploadJSONResponse(context, true, returnSSLFileName, "-", imageId.ToString());
                            }
                            else
                            {
                                WriteFineUploadJSONResponse(context, true, string.Format(S3SectionConfig.CloudFrontURL, S3SectionConfig.ManagedImagesS3Path + "uncropped/" + formattedFilename), "-", imageId.ToString());
                            }
                        }
                        else
                        {
                            //BLL.Emailing.Emailing.EmailBug("Empty File received : " + file.FileName + ", type is " + file.ContentType + ", size is " + file.ContentLength, "");
                            WriteFineUploadJSONResponse(context, false, "-", "-", "0");
                        }
                    }
                    catch (Exception ex)
                    {
                        //BLL.Emailing.Emailing.EmailMark("Exception in upload", ex.Message + " : " + ex.StackTrace);
                        WriteFineUploadJSONResponse(context, false, "-", "-", "0");
                    }
                }
            }
            else
            {
                if (context.Request["delete"] != null && context.Request["delete"] == "true")
                {
                    if (context.Request["imageid"] != null)
                    {
                        // Process delete call
                        int _imageIdToDelete = 0;
                        if (int.TryParse(context.Request["imageid"].ToString(), out _imageIdToDelete))
                        {
                            DAL.DBProvider.Listings.ListingImage_Delete(_imageIdToDelete);
                        }
                    }
                } 
                else 
                {
                    try
                    {
                        // No attachments - check if we've received a jCrop JSON Directive

                        jCropJSONRequest jCropRequest = new jCropJSONRequest
                        {
                            fileToCrop = context.Request["fileToCrop"].ToString(),
                            h = (int)Math.Round(decimal.Parse(context.Request["h"].ToString()), 0),
                            w = (int)Math.Round(decimal.Parse(context.Request["w"].ToString()), 0),
                            x = (int)Math.Round(decimal.Parse(context.Request["x"].ToString()), 0),
                            y = (int)Math.Round(decimal.Parse(context.Request["y"].ToString()), 0),
                            new_h = (int)Math.Round(decimal.Parse(context.Request["new_h"].ToString()), 0),
                            new_w = (int)Math.Round(decimal.Parse(context.Request["new_w"].ToString()), 0),
                            sectionid = context.Request["sectionid"].ToString(),
                            imagetype = context.Request["imagetype"].ToString(),
                            imageid = int.Parse(context.Request["imageid"].ToString())
                        };


                        // Crop & save original cropped
                        byte[] croppedImage = ImageFactory.GetCroppedImage(jCropRequest.fileToCrop, jCropRequest.x, jCropRequest.y, jCropRequest.w, jCropRequest.h);

                        // Get SectionId, ImageTypeId from request
                        int SectionId = context.Request["sectionid"] != null ? int.Parse(context.Request["sectionid"]) : 0;
                        int Imagetype = context.Request["imagetype"] != null ? int.Parse(context.Request["imagetype"]) : 0;

                        // Get S3 Configs
                        IS3Configurable S3SectionConfig = ImageFactory.GetS3ImageConfigSection((Globals.SectionIds)SectionId);

                        // Do resizes by config
                        Triplestones.ImageConfigItem previewItem = Images.ImageFactory.ProcessListingImage(
                            S3SectionConfig.ManagedImagesStorageMode,
                            (ImageFactory.ImageTypes)Imagetype,
                            new MemoryStream(croppedImage),
                            S3SectionConfig.ManagedImagesSVPath,
                            S3SectionConfig.Bucket,
                            S3SectionConfig.CloudFrontDistributionId,
                            S3SectionConfig.CloudFrontURL,
                            S3SectionConfig.ManagedImagesS3Path,
                            S3SectionConfig.CacheMaxAgeAWS,
                            SectionId, jCropRequest.imageid,
                            S3SectionConfig.ImageBaseFileName,
                            S3SectionConfig.ImageConfigs.GetConfigsByImageType(((ImageFactory.ImageTypes)Imagetype).ToString())
                            );




                        // Write JSON back to fineUploader, using proxy if SSL
                        if (System.Web.HttpContext.Current.Request.IsSecureConnection)
                        {
                            string returnSSLFileName = UrlTools.GetServerUrl() + string.Format("/images.ashx?sectionId={0}&listingId={1}&imageType=&imageConfigId={2}&bypasscloudfront=true&getcroppreview=true&", SectionId, jCropRequest.imageid, previewItem.ImageConfigId) + DateTime.Now.Second + "-" + DateTime.Now.Millisecond;
                            WriteFineUploadJSONResponse(context, true, returnSSLFileName, returnSSLFileName, "-");
                        }
                        else
                        {
                            string displayFilename = string.Format(S3SectionConfig.ImageBaseFileName, SectionId.ToString(), jCropRequest.imageid, previewItem.ImageType, previewItem.Width, previewItem.Height, previewItem.ImageConfigId) + ".jpg";
                            WriteFineUploadJSONResponse(context, true, string.Format(S3SectionConfig.S3URL, S3SectionConfig.ManagedImagesS3Path + displayFilename + "?" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond), string.Format(S3SectionConfig.S3URL, S3SectionConfig.ManagedImagesS3Path + displayFilename + "?" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond), "-");
                        }
                    }
                    catch (Exception ex)
                    {
                        //BLL.Emailing.Emailing.EmailMark("Error reading jCrop directive or missing upload file", ex.Message + " : " + ex.StackTrace);
                        WriteFineUploadJSONResponse(context, false, "-", "-", "-");

                    }
                }
            }

            //context.Response.Write(savedFileName);



            // Process as follows:

            // If file attached
                // Get file
                // Save to storage
                // Return JSON success message to fineUpload

            // If no file attached
                // This is normally a call to resize
                // Get new origin and dimensions - if not read properly - throw error else :                
                    // Crop original image
                    // Save over original image with crop OR - do we save uncropped image too?
                    // Do ImageFactory resizes of crop as per configs and save them all
                    // Return JSON response "uploadName" with path of resized image - 



        }

        public void WriteFineUploadJSONResponse(HttpContext context, bool success, string uploadName, string croppedURL, string imageId)
        {
            imageUploaderJSONResponse response = new imageUploaderJSONResponse { success = success, uploadName = uploadName, url = croppedURL, imageId = imageId };
            JavaScriptSerializer jsonEngine = new JavaScriptSerializer();

            //context.Response.ContentType = "application/json";  // NOTE - Changed to handle known bug from FineUpload
            context.Response.ContentType = "text/html";
            context.Response.Write(jsonEngine.Serialize(response));
        }

        class imageUploaderJSONResponse
        {
            public bool success { get; set; }
            public string uploadName { get; set; }
            public string url { get; set; }
            public string imageId { get; set; }
        }

        class jCropJSONRequest
        {
            public string fileToCrop { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; }
            public int h { get; set; }
            public int new_w { get; set; }
            public int new_h { get; set; }
            public string sectionid { get; set; }
            public string imagetype { get; set; }
            public int imageid { get; set; }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
