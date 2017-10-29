using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using VOffice.Core.Messages;
using VOffice.Model;

namespace VOffice.API.Controllers.Api.Share
{
    /// <summary>
    /// FileUpload
    /// </summary>
    public class FileUploadController : ApiController
    {
        /// <summary>
        /// Post Document Files
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> PostAsync()
        {
            BaseResponse result = new BaseResponse();
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/Temp");
                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(streamProvider);
                string messages = "";
                result.Message = messages;
                foreach (var file in streamProvider.FileData)
                {
                    FileInfo fi = new FileInfo(file.LocalFileName);
                    bool allowFileType = false;
                    if (FileTypeDetective.Detective.IsExcel(fi) || FileTypeDetective.Detective.IsWord(fi) || FileTypeDetective.Detective.IsPdf(fi) || FileTypeDetective.Detective.IsRar(fi) || FileTypeDetective.Detective.IsPpt(fi) || FileTypeDetective.Detective.IsPng(fi) || FileTypeDetective.Detective.IsJpeg(fi) || FileTypeDetective.Detective.IsGif(fi) || FileTypeDetective.Detective.IsZip(fi))
                    {
                        allowFileType = true;
                    }
                    if (fi.Length > 4194304)
                    {
                        allowFileType = false;
                    }
                    if (allowFileType)
                    {
                        string directoryPath = file.Headers.ContentDisposition.Name.Split('-')[0].Replace("\"", string.Empty);
                        try
                        {
                            string directory = HttpContext.Current.Server.MapPath("~/Uploads/" + directoryPath);
                            string fileName = Guid.NewGuid() + fi.Extension;
                            if (Directory.Exists(directory))
                            {
                                File.Copy(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name), directory + "/" + fileName, true);
                                File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                            }
                            else
                            {
                                Directory.CreateDirectory(directory);
                                File.Copy(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name), directory + "/" + fileName, true);
                                File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                            }
                            result.Message = "Uploads/" + directoryPath + "/" + fileName;
                        }
                        catch
                        {
                            messages = "Không thể tải tệp tin.";
                            result.IsSuccess = false;
                            result.Message = messages;
                        }
                    }
                    else
                    {
                        File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                        messages = "Tệp tin không đúng định dạng hoặc dung lượng quá lớn.";
                        result.IsSuccess = false;
                        result.Message = messages;
                    }
                }
                return result;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Có lỗi xảy ra trong quá trình tải tệp tin.";
                return result;
            }
        }

        /// <summary>
        /// Post avata file
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> PostAvatarAsync()
        {
            BaseResponse result = new BaseResponse();
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/Temp");
                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(streamProvider);
                string messages = "";
                result.Message = messages;
                foreach (var file in streamProvider.FileData)
                {
                    FileInfo fi = new FileInfo(file.LocalFileName);
                    bool allowFileType = false;
                    if (FileTypeDetective.Detective.IsPng(fi) || FileTypeDetective.Detective.IsJpeg(fi) || FileTypeDetective.Detective.IsGif(fi))
                    {
                        allowFileType = true;
                    }
                    if (fi.Length > 4194304)
                    {
                        allowFileType = false;
                    }
                    if (allowFileType)
                    {
                        string directoryPath = file.Headers.ContentDisposition.Name.Split('-')[0].Replace("\"", string.Empty);
                        try
                        {
                            string directory = HttpContext.Current.Server.MapPath("~/Uploads/" + directoryPath);
                            string fileName = Guid.NewGuid() + fi.Extension;
                            if (Directory.Exists(directory))
                            {
                                File.Copy(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name), directory + "/" + fileName, true);
                                File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                            }
                            else
                            {
                                Directory.CreateDirectory(directory);
                                File.Copy(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name), directory + "/" + fileName, true);
                                File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                            }
                            result.Message = "Uploads/" + directoryPath + "/" + fileName;
                        }
                        catch
                        {
                            messages = "Không thể tải tệp tin.";
                            result.IsSuccess = false;
                            result.Message = messages;
                        }
                    }
                    else
                    {
                        File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                        messages = "Tệp tin không đúng định dạng hoặc dung lượng quá lớn.";
                        result.IsSuccess = false;
                        result.Message = messages;
                    }
                }
                return result;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Có lỗi xảy ra trong quá trình tải tệp tin.";
                return result;
            }
        }
        /// <summary>
        /// Post Task Files
        /// </summary>
        /// <returns></returns>
        public async Task<BaseListResponse<UploadHelper>> PostAsyncTaskAttachment()
        {
            BaseListResponse<UploadHelper> result = new BaseListResponse<UploadHelper>();
            List<UploadHelper> uploadHelpers = new List<UploadHelper>();
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/Temp");
                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(streamProvider);
                string messages = "";
                result.Message = messages;
                foreach (var file in streamProvider.FileData)
                {
                    UploadHelper helper = new UploadHelper();
                    FileInfo fi = new FileInfo(file.LocalFileName);
                    bool allowFileType = false;
                    if (FileTypeDetective.Detective.IsPpt(fi) || FileTypeDetective.Detective.IsExcel(fi) || FileTypeDetective.Detective.IsWord(fi) || FileTypeDetective.Detective.IsPdf(fi) || FileTypeDetective.Detective.IsRar(fi) || FileTypeDetective.Detective.IsPpt(fi) || FileTypeDetective.Detective.IsPng(fi) || FileTypeDetective.Detective.IsJpeg(fi) || FileTypeDetective.Detective.IsGif(fi) || FileTypeDetective.Detective.IsZip(fi))
                    {
                        allowFileType = true;
                    }
                    else
                    {
                        if (fi.Extension == ".pptx")
                        {
                            allowFileType = true;
                        }
                    }
                    if (fi.Length > 4194304)
                    {
                        allowFileType = false;
                    }
                    if (allowFileType)
                    {
                        string directoryPath = file.Headers.ContentDisposition.Name.Split('-')[0].Replace("\"", string.Empty);
                        try
                        {
                            string directory = HttpContext.Current.Server.MapPath("~/Uploads/" + directoryPath);
                            string fileName = Guid.NewGuid() + fi.Extension;
                            if (Directory.Exists(directory))
                            {
                                File.Copy(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name), directory + "/" + fileName, true);
                                File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                            }
                            else
                            {
                                Directory.CreateDirectory(directory);
                                File.Copy(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name), directory + "/" + fileName, true);
                                File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                            }
                            result.Message = "";
                            helper.FilePath = "Uploads/" + directoryPath + "/" + fileName;
                            helper.FileName = fileName;
                            uploadHelpers.Add(helper);

                        }
                        catch
                        {
                            messages = "Không thể tải tệp tin.";
                            result.IsSuccess = false;
                            result.Message = messages;
                        }
                    }
                    else
                    {
                        File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                        messages = "Tệp tin không đúng định dạng hoặc dung lượng quá lớn.";
                        result.IsSuccess = false;
                        result.Message = messages;
                    }
                }
                
                result.Data = uploadHelpers;
                return result;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Có lỗi xảy ra trong quá trình tải tệp tin.";
                return result;
            }
        }
        /// <summary>
        /// Post TaskOpinion Files
        /// </summary>
        /// <returns></returns>
        public async Task<BaseListResponse<UploadHelper>> PostAsyncTaskOpinionAttachment()
        {
            BaseListResponse<UploadHelper> result = new BaseListResponse<UploadHelper>();
            List<UploadHelper> uploadHelpers = new List<UploadHelper>();
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/Temp");
                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(streamProvider);
                string messages = "";
                result.Message = messages;
                foreach (var file in streamProvider.FileData)
                {
                    UploadHelper helper = new UploadHelper();
                    FileInfo fi = new FileInfo(file.LocalFileName);
                    bool allowFileType = false;
                    if (FileTypeDetective.Detective.IsPpt(fi) || FileTypeDetective.Detective.IsExcel(fi) || FileTypeDetective.Detective.IsWord(fi) || FileTypeDetective.Detective.IsPdf(fi) || FileTypeDetective.Detective.IsRar(fi) || FileTypeDetective.Detective.IsPpt(fi) || FileTypeDetective.Detective.IsPng(fi) || FileTypeDetective.Detective.IsJpeg(fi) || FileTypeDetective.Detective.IsGif(fi) || FileTypeDetective.Detective.IsZip(fi))
                    {
                        allowFileType = true;
                    }
                    else
                    {
                        if (fi.Extension == ".pptx")
                        {
                            allowFileType = true;
                        }
                    }
                    if (fi.Length > 4194304)
                    {
                        allowFileType = false;
                    }
                    if (allowFileType)
                    {
                        string directoryPath = file.Headers.ContentDisposition.Name.Split('-')[0].Replace("\"", string.Empty);
                        try
                        {
                            string directory = HttpContext.Current.Server.MapPath("~/Uploads/" + directoryPath);
                            string fileName = Guid.NewGuid() + fi.Extension;
                            if (Directory.Exists(directory))
                            {
                                File.Copy(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name), directory + "/" + fileName, true);
                                File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                            }
                            else
                            {
                                Directory.CreateDirectory(directory);
                                File.Copy(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name), directory + "/" + fileName, true);
                                File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                            }
                            result.Message = "";
                            helper.FilePath = "Uploads/" + directoryPath + "/" + fileName;
                            helper.FileName = fileName;
                            uploadHelpers.Add(helper);

                        }
                        catch
                        {
                            messages = "Không thể tải tệp tin.";
                            result.IsSuccess = false;
                            result.Message = messages;
                        }
                    }
                    else
                    {
                        File.Delete(HttpContext.Current.Server.MapPath("~/Temp/" + fi.Name));
                        messages = "Tệp tin không đúng định dạng hoặc dung lượng quá lớn.";
                        result.IsSuccess = false;
                        result.Message = messages;
                    }
                }
                result.Data = uploadHelpers;
                return result;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Có lỗi xảy ra trong quá trình tải tệp tin.";
                return result;
            }
        }
    }
    /// <summary>
    /// Working with stream provider
    /// </summary>
    public class MyStreamProvider : MultipartFormDataStreamProvider
    {
        /// <summary>
        /// stream provider
        /// </summary>
        /// <param name="uploadPath"></param>
        public MyStreamProvider(string uploadPath)
            : base(uploadPath)
        {

        }

        /// <summary>
        /// Get Local FileName
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string fileName = headers.ContentDisposition.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = Guid.NewGuid().ToString() + ".data";
            }
            return fileName.Replace("\"", string.Empty);
        }
    }
}
