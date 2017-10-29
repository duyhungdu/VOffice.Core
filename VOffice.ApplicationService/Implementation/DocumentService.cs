using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Model.Validators;
using VOffice.Repository;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;

namespace VOffice.ApplicationService
{
    public partial class DocumentService : BaseService, IDocumentService
    {
        protected readonly DocumentFieldRepository _documentFieldRepository;
        protected readonly DocumentTypeRepository _documentTypeRepository;
        protected readonly DocumentSignedByRepository _documentSignedByRepository;
        protected readonly DocumentReceivedRepository _documentReceivedRepository;
        protected readonly DocumentHistoryRepository _documentHistoryRepository;
        protected readonly DocumentDeliveredRepository _documentDeliveredRepository;
        protected readonly ExternalSendReceiveDivisionRepository _externalSendReceiveDivisionRepository;
        protected readonly DocumentFieldDepartmentRepository _documentFieldDepartmentRepository;
        protected readonly DocumentDocumentFieldReposity _documentDocumentFieldReposity;
        protected readonly DocumentRecipentRepository _documentRecipentRepository;
        protected readonly DepartmentRepository _departmentRepository;
        protected readonly ApplicationLoggingRepository _applicationLoggingRepository;
        protected readonly StaffRepository _staffRepository;
        protected readonly UserNotificationRepository _userNotificationRepository;
        protected readonly NotificationCenterRepository _notificationCenterRepository;

        private const string SIGNBY = "SIGNBY";
        private const string DOCUMENTTYPE = "DOCUMENTTYPE";
        private const string DOCUMENTFIELD = "DOCUMENTFIELD";
        public DocumentService()
        {
            _documentTypeRepository = new DocumentTypeRepository();
            _documentSignedByRepository = new DocumentSignedByRepository();
            _documentReceivedRepository = new DocumentReceivedRepository();
            _documentDeliveredRepository = new DocumentDeliveredRepository();
            _documentFieldRepository = new DocumentFieldRepository();
            _documentHistoryRepository = new DocumentHistoryRepository();
            _externalSendReceiveDivisionRepository = new ExternalSendReceiveDivisionRepository();
            _documentFieldDepartmentRepository = new DocumentFieldDepartmentRepository();
            _documentDocumentFieldReposity = new DocumentDocumentFieldReposity();
            _documentRecipentRepository = new DocumentRecipentRepository();
            _departmentRepository = new DepartmentRepository();
            _applicationLoggingRepository = new ApplicationLoggingRepository();
            _staffRepository = new StaffRepository();
            _userNotificationRepository = new UserNotificationRepository();
            _notificationCenterRepository = new NotificationCenterRepository();
        }

        #region DocumentType
        public BaseResponse<DocumentType> GetDocumentTypeById(int id)
        {
            var response = new BaseResponse<DocumentType>();
            try
            {
                response.Value = _documentTypeRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetDocumentType_Result> FilterDocumentType(DocumentTypeQuery query)
        {
            var response = new BaseListResponse<SPGetDocumentType_Result>();
            int count = 0;
            try
            {
                response.Data = _documentTypeRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<DocumentType> GetAllDocumentType()
        {
            var response = new BaseListResponse<DocumentType>();
            try
            {
                var result = _documentTypeRepository.GetAll().Where(x => x.Deleted == false && x.Active == true).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentType> AddDocumentType(DocumentType model)
        {
            var response = new BaseResponse<DocumentType>();
            var errors = Validate<DocumentType>(model, new DocumentTypeValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentType> errResponse = new BaseResponse<DocumentType>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            var listDocumentType = _documentTypeRepository.GetAll().Where(x => x.Code.ToLower() == model.Code.ToLower()).ToList();
            if (listDocumentType.Count() > 0)
            {
                response.IsSuccess = false;
                response.Message = "Mã loại đã tồn tại";
                return response;
            }
            else
            {
                try
                {
                    model.CreatedOn = DateTime.Now;
                    response.Value = _documentTypeRepository.Add(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentType", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                }
            }
            return response;
        }
        public BaseResponse<DocumentType> UpdateDocumentType(DocumentType model)
        {
            BaseResponse<DocumentType> response = new BaseResponse<DocumentType>();
            var errors = Validate<DocumentType>(model, new DocumentTypeValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentType> errResponse = new BaseResponse<DocumentType>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var doctype = _documentTypeRepository.GetById(model.Id);
                if (doctype.Code != model.Code)
                {
                    IEnumerable<DocumentType> listType = _documentTypeRepository.FindBy(x => x.Code.ToLower() == model.Code.ToLower() && x.Deleted == false);
                    if (listType.Count() > 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Mã loại đã tồn tại";
                        return response;
                    }
                    else
                    {
                        model.EditedOn = DateTime.Now;
                        response.Value = _documentTypeRepository.Edit(model);
                    }
                }
                else
                {
                    model.EditedOn = DateTime.Now;
                    response.Value = _documentTypeRepository.Edit(model);
                }
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentType", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalDocumentType(int id)
        {
            BaseResponse response = new BaseResponse();
            DocumentType model = _documentTypeRepository.GetById(id);
            try
            {
                model.Deleted = true;
                model.EditedOn = DateTime.Now;
                _documentTypeRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "DocumentType", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        #endregion AS_DOC_Type

        #region DocumentSignedBy
        public BaseResponse<DocumentSignedBy> GetDocumentSignedByById(int id)
        {
            var response = new BaseResponse<DocumentSignedBy>();
            try
            {
                response.Value = _documentSignedByRepository.GetById(id);

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetDocumentSignedBy_Result> FilterDocumentSignedBy(DocumentSignedByQuery query)
        {
            var response = new BaseListResponse<SPGetDocumentSignedBy_Result>();
            int count = 0;
            try
            {
                response.Data = _documentSignedByRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<DocumentSignedBy> FilterDocumentSignedByByDepartmentId(int departmentId)
        {
            var response = new BaseListResponse<DocumentSignedBy>();
            try
            {
                var result = _documentSignedByRepository.GetAll().Where(x => x.Deleted == false && x.Active == true && x.DepartmentId == departmentId).OrderBy(x => x.FullName).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<DocumentSignedBy> GetAllDocumentSignedBy()
        {
            var response = new BaseListResponse<DocumentSignedBy>();
            try
            {
                var result = _documentSignedByRepository.GetAll().Where(x => x.Deleted == false && x.Active == true).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentSignedBy> AddDocumentSignedBy(DocumentSignedBy model)
        {
            var response = new BaseResponse<DocumentSignedBy>();
            var errors = Validate<DocumentSignedBy>(model, new DocumentSignedByValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentSignedBy> errResponse = new BaseResponse<DocumentSignedBy>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _documentSignedByRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentSignedBy", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentSignedBy> UpdateDocumentSignedBy(DocumentSignedBy model)
        {
            BaseResponse<DocumentSignedBy> response = new BaseResponse<DocumentSignedBy>();
            var errors = Validate<DocumentSignedBy>(model, new DocumentSignedByValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentSignedBy> errResponse = new BaseResponse<DocumentSignedBy>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _documentSignedByRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentSignedBy", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalDocumentSignedBy(int id)
        {
            BaseResponse response = new BaseResponse();
            DocumentSignedBy model = _documentSignedByRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _documentSignedByRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "DocumentSignedBy", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        #endregion DocumentSignedBy 

        #region ExternalSendReceiveDivision
        public BaseResponse<ExternalSendReceiveDivision> GetExternalSendReceiveDivisionById(int id)
        {
            var response = new BaseResponse<ExternalSendReceiveDivision>();
            try
            {
                response.Value = _externalSendReceiveDivisionRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetExternalSendReceiveDivision_Result> FilterExternalSendReceiveDivision(ExternalSendReceiveDivisionQuery query)
        {
            var response = new BaseListResponse<SPGetExternalSendReceiveDivision_Result>();
            int count = 0;
            try
            {
                response.Data = _externalSendReceiveDivisionRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<ExternalSendReceiveDivision> FilterExternalSendReceiveDivisionByDepartmentId(int departmentId)
        {
            var response = new BaseListResponse<ExternalSendReceiveDivision>();
            try
            {
                var result = _externalSendReceiveDivisionRepository.GetAll().Where(x => x.Deleted == false && x.Active == true && x.DepartmentId == departmentId).OrderBy(x => x.Title).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<ExternalSendReceiveDivision> GetAllExternalSendReceiveDivision()
        {
            var response = new BaseListResponse<ExternalSendReceiveDivision>();
            try
            {
                var result = _externalSendReceiveDivisionRepository.GetAll().Where(x => x.Deleted == false && x.Active == true).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<ExternalSendReceiveDivision> AddExternalSendReceiveDivision(ExternalSendReceiveDivision model)
        {
            var response = new BaseResponse<ExternalSendReceiveDivision>();
            var errors = Validate<ExternalSendReceiveDivision>(model, new ExternalSendReceiveDivisionValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<ExternalSendReceiveDivision> errResponse = new BaseResponse<ExternalSendReceiveDivision>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _externalSendReceiveDivisionRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "ExternalSendReceiveDivision", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<ExternalSendReceiveDivision> UpdateExternalSendReceiveDivision(ExternalSendReceiveDivision model)
        {
            BaseResponse<ExternalSendReceiveDivision> response = new BaseResponse<ExternalSendReceiveDivision>();
            var errors = Validate<ExternalSendReceiveDivision>(model, new ExternalSendReceiveDivisionValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<ExternalSendReceiveDivision> errResponse = new BaseResponse<ExternalSendReceiveDivision>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _externalSendReceiveDivisionRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "ExternalSendReceiveDivision", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalExternalSendReceiveDivision(int id)
        {
            BaseResponse response = new BaseResponse();
            ExternalSendReceiveDivision model = _externalSendReceiveDivisionRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _externalSendReceiveDivisionRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "ExternalSendReceiveDivision", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        #endregion ExternalSendReceiveDivision

        #region Document
        #region DocumentReceived
        public BaseListResponse<SPGetDocument_Result> FilterDocument(DocumentReceivedQuery query)
        {
            var response = new BaseListResponse<SPGetDocument_Result>();
            int count = 0;
            try
            {
                response.Data = _documentReceivedRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<int> CountNewDocument(DocumentReceivedQuery query)
        {
            var response = new BaseResponse<int>();
            try
            {
                response.Value = _documentReceivedRepository.CountNewDocument(query);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPSearchListDocument_Result> SearchListDocument(DocumentReceivedQuery query)
        {
            var response = new BaseListResponse<SPSearchListDocument_Result>();
            int count = 0;
            try
            {
                response.Data = _documentReceivedRepository.SearchListDocument(query);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentReceived> AddDocumentReceived(DocumentReceived model)
        {
            var response = new BaseResponse<DocumentReceived>();
            var errors = Validate<DocumentReceived>(model, new DocumentReceivedValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentReceived> errResponse = new BaseResponse<DocumentReceived>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            if (CheckReceivedNumber(model.DepartmentId, model.ReceivedNumber))
            {
                response.Message = "Số văn bản đến đã tồn tại";
                response.IsSuccess = false;
                return response;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _documentReceivedRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentReceived", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<DocumentReceived> AddSetOfDocumentReceived(List<ComplexDocumentReceived> model)
        {
            var response = new BaseListResponse<DocumentReceived>();
            var listCostomFields = new List<CustomDocumentField>();
            var listDocumentFields = new List<DocumentDocumentField>();

            foreach (var item in model)
            {
                listCostomFields.Clear();
                listDocumentFields.Clear();
                var cusFields = item.CustomDocumentFileds;
                var docFields = item.DocumentFields;
                if (cusFields == null && docFields == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Tạo mới văn bản không thành công: Lĩnh vực văn bản không để trống";
                    return response;
                }
                if (cusFields != null)
                {
                    listCostomFields = item.CustomDocumentFileds.ToList();
                }
                if (docFields != null)
                {
                    listDocumentFields = item.DocumentFields.ToList();
                }
                var documentReceived = new DocumentReceived();
                documentReceived.DocumentNumber = item.DocumentNumber;
                documentReceived.ReceivedNumber = item.ReceivedNumber;
                documentReceived.Title = item.Title;
                documentReceived.DocumentDate = item.DocumentDate;
                documentReceived.ReceivedDate = item.ReceivedDate;
                documentReceived.DepartmentId = item.DepartmentId;
                documentReceived.InternalFromDivisionId = item.InternalFromDivisionId;
                documentReceived.ExternalFromDivision = item.ExternalFromDivision;
                documentReceived.ExternalFromDivisionId = item.ExternalFromDivisionId;
                documentReceived.RecipientsDivision = item.RecipientsDivision;
                documentReceived.SignedBy = item.SignedBy;
                documentReceived.SignedById = item.SignedById;
                documentReceived.DocumentTypeId = item.DocumentTypeId;
                documentReceived.AddedDocumentBook = item.AddedDocumentBook;
                documentReceived.DocumentBookAddedOn = item.DocumentBookAddedOn;
                documentReceived.DocumentBookAddedBy = item.DocumentBookAddedBy;
                documentReceived.NumberOfCopies = item.NumberOfCopies;
                documentReceived.NumberOfPages = item.NumberOfPages;
                documentReceived.SecretLevel = item.SecretLevel;
                documentReceived.UrgencyLevel = item.UrgencyLevel;
                documentReceived.Note = item.Note;
                documentReceived.OriginalSavingPlace = item.OriginalSavingPlace;
                documentReceived.LegalDocument = item.LegalDocument;
                documentReceived.AttachmentName = item.AttachmentName;
                documentReceived.AttachmentPath = item.AttachmentPath;
                documentReceived.DeliveredDocumentId = item.DeliveredDocumentId;
                documentReceived.ReceivedDocumentId = item.ReceivedDocumentId;
                documentReceived.Active = item.Active;
                documentReceived.Deleted = item.Deleted;
                documentReceived.CreatedBy = item.CreatedBy;
                documentReceived.CreatedOn = DateTime.Now;
                documentReceived.EditedBy = item.EditedBy;
                documentReceived.EditedOn = DateTime.Now;
                documentReceived.SendOut = item.SendOut;

                var errors = Validate<DocumentReceived>(documentReceived, new DocumentReceivedValidator());
                if (errors.Count() > 0)
                {

                    BaseListResponse<DocumentReceived> errResponse = new BaseListResponse<DocumentReceived>();
                    errResponse.IsSuccess = false;
                    return errResponse;
                }

                var addDocumentReceivedResponse = _documentReceivedRepository.Add(documentReceived);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentReceived", documentReceived.Id.ToString(), "", "", documentReceived, "", HttpContext.Current.Request.UserHostAddress, documentReceived.CreatedBy);
                }
                catch { }

                if (addDocumentReceivedResponse == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Không thể tạo văn bản đến. Xin vui lòng thử lại.";
                    return response;
                }
                else
                {
                    response.Data.Add(addDocumentReceivedResponse);
                }

                // add recipent
                List<DocumentRecipent> listDocumentRecipent = item.DocumentRecipents.ToList();
                if (listDocumentRecipent.Count > 0)
                {
                    foreach (var itemRecipent in listDocumentRecipent)
                    {
                        try
                        {
                            itemRecipent.CreatedOn = DateTime.Now;
                            itemRecipent.DocumentId = addDocumentReceivedResponse.Id;
                            _documentRecipentRepository.Add(itemRecipent);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentRecipent", itemRecipent.Id.ToString(), "", "", itemRecipent, "", HttpContext.Current.Request.UserHostAddress, itemRecipent.CreatedBy);
                            }
                            catch { }
                        }
                        catch
                        {
                            response.IsSuccess = false;
                            response.Message = "Không thể tạo chi tiết cho văn bản đến. Xin vui lòng thử lại.";
                        }
                    }

                }

                #region send notifcation
                if (item.AddedDocumentBook)
                {
                    var listUserSendNotifi = new List<SPGetUserNotificationForDocumentAndEvent_Result>();
                    var listRecipents = listDocumentRecipent;
                    foreach (var re in listRecipents)
                    {
                        if (re.DepartmentId != null || re.DepartmentId != 0)
                        {
                            listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByDepartmentId(re.DepartmentId.Value));
                        }
                        else
                        {
                            var listUser = _userNotificationRepository.GetUserNotificationByUserId(re.UserId);
                            listUserSendNotifi.AddRange(_userNotificationRepository.ConvertToNotificationDocumentAndEvent(listUser));
                        }
                    }
                    NotificationCenter resultNotifiCenter = null;
                    NotificationCenter notifiFirst = null;
                    foreach (var re in listUserSendNotifi)
                    {
                        FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                        fcmNotificationCenter.Avatar = re.Avatar;
                        fcmNotificationCenter.FullName = re.FullName;
                        fcmNotificationCenter.Content = "đã cập nhật một văn bản mới: \n" + item.Title;
                        fcmNotificationCenter.Title = "vOffice";
                        fcmNotificationCenter.RecordNumber = item.DocumentNumber;
                        fcmNotificationCenter.RecordId = addDocumentReceivedResponse.Id;
                        fcmNotificationCenter.Type = (int)NotificationCode.Document;
                        fcmNotificationCenter.CreatedBy = addDocumentReceivedResponse.CreatedBy;
                        fcmNotificationCenter.CreatedOn = addDocumentReceivedResponse.CreatedOn;
                        fcmNotificationCenter.HaveSeen = false;
                        fcmNotificationCenter.ReceivedUserId = re.UserId;
                        fcmNotificationCenter.DeviceId = re.DeviceId;
                        if (notifiFirst != null && notifiFirst.ReceivedUserId == re.UserId)
                        {
                            fcmNotificationCenter.GroupId = notifiFirst.Id;
                        }
                        var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                        resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                        fcmNotificationCenter.Id = resultNotifiCenter.Id;
                        if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                        FCMPushNotification fcmPushNotification = new FCMPushNotification();
                        fcmPushNotification.SendNotification(fcmNotificationCenter, re.ClientId);
                    }
                }
                #endregion
                // add document document filed


                if (listDocumentFields.Count > 0)
                {
                    try
                    {
                        foreach (var docField in listDocumentFields)
                        {
                            docField.CreatedOn = DateTime.Now;
                            docField.DocumentId = addDocumentReceivedResponse.Id;
                            _documentDocumentFieldReposity.Add(docField);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDocumentField", "", "", "", docField, "", HttpContext.Current.Request.UserHostAddress, docField.CreatedBy);
                            }
                            catch { }
                        }
                    }
                    catch
                    {
                        response.IsSuccess = false;
                        response.Message = "Thêm mới lĩnh vực văn bản không thành công";
                        return response;
                    }
                }
                else if (listCostomFields.Count > 0)
                {
                    var listCustomFieldReturn = GetListDocFieldDepartmentFromSystem(listCostomFields);
                    if (listCustomFieldReturn.Data.Count > 0)
                    {
                        try
                        {
                            foreach (var docField in listCustomFieldReturn.Data)
                            {
                                DocumentDocumentField doc2doc = new DocumentDocumentField();
                                doc2doc.DocumentId = addDocumentReceivedResponse.Id;
                                doc2doc.DocumentFieldDepartmentId = docField.DocumentFieldId;
                                doc2doc.ReceivedDocument = true;
                                doc2doc.CreatedBy = item.CreatedBy;
                                doc2doc.CreatedOn = DateTime.Now;
                                _documentDocumentFieldReposity.Add(doc2doc);
                                try
                                {
                                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDocumentField", "", "", "", doc2doc, "", HttpContext.Current.Request.UserHostAddress, doc2doc.CreatedBy);
                                }
                                catch { }
                            }
                        }
                        catch
                        {
                            response.IsSuccess = false;
                            response.Message = "Không thể tạo chi tiết cho lĩnh vực văn bản. Xin vui lòng thử lại.";
                        }
                    }
                }

            }

            return response;
        }
        public BaseResponse<DocumentReceived> UpdateDocumentReceived(DocumentReceived model)
        {
            var response = new BaseResponse<DocumentReceived>();
            var errors = Validate<DocumentReceived>(model, new DocumentReceivedValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentReceived> errResponse = new BaseResponse<DocumentReceived>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            var modelOrigin = _documentReceivedRepository.GetById(model.Id);
            if (modelOrigin.ReceivedNumber != model.ReceivedNumber)
            {
                if (CheckReceivedNumber(model.DepartmentId, model.ReceivedNumber))
                {
                    BaseResponse<DocumentReceived> errResponse = new BaseResponse<DocumentReceived>(model, errors);
                    errResponse.Message = "Số văn bản đến đã tồn tại";
                    errResponse.IsSuccess = false;
                    return errResponse;
                }
            }

            try
            {
                var document = _documentReceivedRepository.GetById(model.Id);
                model.EditedOn = DateTime.Now;
                response.Value = _documentReceivedRepository.Update(document, model);

                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentReceived", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }

                /// send notification
                #region send notification
                if (model.AddedDocumentBook)
                {
                    var listUserSendNotifi = new List<SPGetUserNotificationForDocumentAndEvent_Result>();
                    var listRecipents = _documentRecipentRepository.GetRecipentsByDocIdAndReceivedDoc(model.Id, true);
                    foreach (var item in listRecipents)
                    {
                        if (item.DepartmentId != null)
                        {
                            listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByDepartmentId(item.DepartmentId.Value));
                        }
                        else
                        {
                            var listUser = _userNotificationRepository.GetUserNotificationByUserId(item.UserId);
                            listUserSendNotifi.AddRange(_userNotificationRepository.ConvertToNotificationDocumentAndEvent(listUser));
                        }
                    }
                    NotificationCenter resultNotifiCenter = null;
                    NotificationCenter notifiFirst = null;
                    foreach (var item in listUserSendNotifi)
                    {
                        FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                        fcmNotificationCenter.Avatar = item.Avatar;
                        fcmNotificationCenter.FullName = item.FullName;
                        fcmNotificationCenter.Content = "đã cập nhật một văn bản mới: \n" + model.Title;
                        fcmNotificationCenter.Title = "vOffice";
                        fcmNotificationCenter.RecordNumber = model.DocumentNumber;
                        fcmNotificationCenter.RecordId = model.Id;
                        fcmNotificationCenter.Type = (int)NotificationCode.Document;
                        fcmNotificationCenter.CreatedBy = model.CreatedBy;
                        fcmNotificationCenter.CreatedOn = model.CreatedOn;
                        fcmNotificationCenter.HaveSeen = false;
                        fcmNotificationCenter.ReceivedUserId = item.UserId;
                        fcmNotificationCenter.DeviceId = item.DeviceId;
                        if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                        {
                            fcmNotificationCenter.GroupId = notifiFirst.Id;
                        }
                        var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                        resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                        fcmNotificationCenter.Id = resultNotifiCenter.Id;
                        if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                        FCMPushNotification fcmPushNotification = new FCMPushNotification();
                        fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalDocument(int id, bool receivedDocument, string retrievedText)
        {
            BaseResponse response = new BaseResponse();
            if (receivedDocument)
            {
                try
                {
                    DocumentReceived model = _documentReceivedRepository.GetById(id);
                    model.Deleted = true;
                    model.EditedOn = DateTime.Now;
                    model.RetrieveText = retrievedText;
                    _documentReceivedRepository.Edit(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "DELETE", "DocumentReceived", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch { }

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                }
            }

            else
            {
                try
                {
                    DocumentDelivered model = _documentDeliveredRepository.GetById(id);
                    model.Deleted = true;
                    model.EditedOn = DateTime.Now;
                    _documentDeliveredRepository.Edit(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "DELETE", "DocumentReceived", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                }
            }
            return response;
        }
        public BaseResponse RetrieveDocument(int id, string retrieveText, bool receivedDocument)
        {
            BaseResponse response = new BaseResponse();
            if (!receivedDocument)
            {
                try
                {
                    //Select list DocumentReceived from this DocumentDelivered
                    List<SPGetDocumentReceivedByDocumentDeliveredId_Result> listDocumentReceived = new List<SPGetDocumentReceivedByDocumentDeliveredId_Result>();
                    listDocumentReceived = _documentReceivedRepository.GetDocumentReceivedByDocumentDeliveredId(id, receivedDocument);
                    //each one, delete
                    foreach (var item in listDocumentReceived)
                    {
                        if (item != null)
                        {
                            DocumentReceived model = _documentReceivedRepository.GetById(item.Id);
                            model.Deleted = true;
                            model.Retrieved = true;
                            model.RetrieveText = retrieveText;
                            model.EditedOn = DateTime.Now;
                            _documentReceivedRepository.Edit(model);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "DELETE", "DocumentReceived", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                            }
                            catch { }
                        }
                    }
                    //delete this DocumentDelivered
                    DocumentDelivered documentDelivered = _documentDeliveredRepository.GetById(id);
                    documentDelivered.Deleted = true;
                    documentDelivered.Retrieved = true;
                    documentDelivered.RetrieveText = retrieveText;
                    documentDelivered.EditedOn = DateTime.Now;
                    _documentDeliveredRepository.Edit(documentDelivered);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "DELETE", "DocumentDelivered", documentDelivered.Id.ToString(), "", "", documentDelivered, "", HttpContext.Current.Request.UserHostAddress, documentDelivered.CreatedBy);
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                }
            }

            else
            {
                try
                {
                    //Select list DocumentDelivered from this DocumentReceived
                    List<SPGetDocumentDeliveredByDocumentReceivedId_Result> listDocumentDelivered = new List<SPGetDocumentDeliveredByDocumentReceivedId_Result>();
                    listDocumentDelivered = _documentDeliveredRepository.GetDocumentDeliveredByDocumentReceivedId(id, receivedDocument);


                    //each one
                    foreach (var item in listDocumentDelivered)
                    {
                        if (item != null)
                        {
                            DocumentDelivered model = _documentDeliveredRepository.GetById(item.Id);

                            //Select listDocumentReceived from this item (DocumentDelivered)
                            List<SPGetDocumentReceivedByDocumentDeliveredId_Result> listDocumentReceived = new List<SPGetDocumentReceivedByDocumentDeliveredId_Result>();
                            listDocumentReceived = _documentReceivedRepository.GetDocumentReceivedByDocumentDeliveredId(model.Id, receivedDocument);
                            //each one, delete
                            foreach (var oneItem in listDocumentReceived)
                            {
                                if (oneItem != null)
                                {
                                    DocumentReceived oneDocumentReceived = _documentReceivedRepository.GetById(oneItem.Id);
                                    oneDocumentReceived.Deleted = true;
                                    oneDocumentReceived.Retrieved = true;
                                    oneDocumentReceived.RetrieveText = retrieveText;
                                    oneDocumentReceived.EditedOn = DateTime.Now;
                                    _documentReceivedRepository.Edit(oneDocumentReceived);
                                    try
                                    {
                                        _applicationLoggingRepository.Log("EVENT", "DELETE", "DocumentReceived", oneDocumentReceived.Id.ToString(), "", "", oneDocumentReceived, "", HttpContext.Current.Request.UserHostAddress, oneDocumentReceived.CreatedBy);
                                    }
                                    catch { }
                                }
                            }

                            model.Deleted = true;
                            model.EditedOn = DateTime.Now;
                            model.Retrieved = true;
                            model.RetrieveText = retrieveText;
                            _documentDeliveredRepository.Edit(model);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "DELETE", "DocumentDelivered", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                            }
                            catch { }
                        }
                    }
                    //delete this DocumentReceived
                    DocumentReceived documentReceived = _documentReceivedRepository.GetById(id);
                    documentReceived.Deleted = true;
                    documentReceived.EditedOn = DateTime.Now;
                    documentReceived.Retrieved = true;
                    documentReceived.RetrieveText = retrieveText;
                    _documentReceivedRepository.Edit(documentReceived);
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                }
            }
            return response;
        }
        public BaseListResponse<DocumentRecipent> ForwardDocument(List<DocumentRecipent> listRecipent)
        {
            var response = new BaseListResponse<DocumentRecipent>();
            DocumentCheckReadableQuery query = new DocumentCheckReadableQuery();
            BaseResponse<DocumentRecipent> recipent = new BaseResponse<DocumentRecipent>();
            try
            {
                foreach (var item in listRecipent)
                {
                    query.UserId = item.UserId;
                    query.DocumentId = item.DocumentId;
                    query.ReceivedDocument = item.ReceivedDocument;
                    if (CheckUserDocumentReadable(query) == false)
                    {
                        recipent = AddDocumentRecipent(item);
                        try
                        {
                            _applicationLoggingRepository.Log("EVENT", "CREATE", "Receipent", recipent.Value.Id.ToString(), "", "", recipent, "", System.Web.HttpContext.Current.Request.UserHostAddress, item.UserId);
                        }
                        catch
                        { }
                    }
                }
            }
            catch { return response; }
            return response;
        }
        public BaseResponse<DocumentReceived> GetDocumentReceivedById(int id)
        {
            var response = new BaseResponse<DocumentReceived>();
            try
            {
                response.Value = _documentReceivedRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public bool CheckUserDocumentReadable(DocumentCheckReadableQuery query)
        {
            try
            {
                return _documentReceivedRepository.CheckUserDocumentReadable(query.UserId, query.DocumentId, query.ReceivedDocument);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckReceivedNumber(int departmentId, string receivedNumber)
        {
            try
            {
                return _documentReceivedRepository.CheckReceivedNumber(departmentId, receivedNumber);
            }
            catch
            {
                return false;
            }
        }
        public BaseResponse<SPGetDetailDocument_Result> GetDetailDocumentReceived(string type, int id)
        {
            var response = new BaseResponse<SPGetDetailDocument_Result>();
            try
            {
                response.Value = _documentReceivedRepository.GetDetailDocumentReceived(type, id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        #endregion DocumentReceived

        #region DocumentDelivered
        public BaseResponse<DocumentDelivered> GetDocumentDeliveredById(int id)
        {
            var response = new BaseResponse<DocumentDelivered>();
            try
            {
                response.Value = _documentDeliveredRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse<DocumentDelivered> AddComplexDocumentDelivered(ComplexDocumentDelivered item)
        {
            var response = new BaseResponse<DocumentDelivered>();
            var customField = item.CustomDocumentFileds;
            var docFields = item.DocumentFields;

            if (customField == null && docFields == null)
            {
                response.IsSuccess = false;
                response.Message = "Thêm văn bản đi không thành công: lĩnh vực văn bản không được để trống";
                return response;
            }

            var listCostomFields = new List<CustomDocumentField>();
            var listDocumentFields = new List<DocumentDocumentField>();

            if (customField != null)
            {
                listCostomFields = customField;
            }
            if (docFields != null)
            {
                listDocumentFields = docFields.ToList();
            }

            DocumentDelivered documentDelivered = new DocumentDelivered();
            documentDelivered.DocumentNumber = item.DocumentNumber;
            documentDelivered.Title = item.Title;
            documentDelivered.DocumentDate = item.DocumentDate;
            documentDelivered.DeliveredDate = item.DeliveredDate;
            documentDelivered.DepartmentId = item.DepartmentId;
            documentDelivered.ExternalReceiveDivisionId = item.ExternalReceiveDivisionId;
            documentDelivered.ExternalReceiveDivision = item.ExternalReceiveDivision;
            documentDelivered.RecipientsDivision = item.RecipientsDivision;
            documentDelivered.SignedById = item.SignedById;
            documentDelivered.SignedBy = item.SignedBy;
            documentDelivered.DocumentTypeId = item.DocumentTypeId;
            documentDelivered.NumberOfCopies = item.NumberOfCopies;
            documentDelivered.NumberOfPages = item.NumberOfPages;
            documentDelivered.SecretLevel = item.SecretLevel;
            documentDelivered.UrgencyLevel = item.UrgencyLevel;
            documentDelivered.Note = item.Note;
            documentDelivered.OriginalSavingPlace = item.OriginalSavingPlace;
            documentDelivered.LegalDocument = item.LegalDocument;
            documentDelivered.AttachmentName = item.AttachmentName;
            documentDelivered.AttachmentPath = item.AttachmentPath;
            documentDelivered.ReceivedDocumentId = item.ReceivedDocumentId;
            documentDelivered.Active = item.Active;
            documentDelivered.Deleted = item.Deleted;
            documentDelivered.CreatedOn = DateTime.Now;
            documentDelivered.CreatedBy = item.CreatedBy;
            documentDelivered.EditedBy = item.EditedBy;
            documentDelivered.EditedOn = DateTime.Now;
            documentDelivered.SendOut = item.SendOut;

            var errors = Validate<DocumentDelivered>(documentDelivered, new DocumentDeliveredValidator());
            if (errors.Count() > 0)
            {

                BaseResponse<DocumentDelivered> errResponse = new BaseResponse<DocumentDelivered>();
                errResponse.IsSuccess = false;
                errResponse.BrokenRules.AddRange(errors);
                return errResponse;
            }

            var addDocumentDeliveredResponse = _documentDeliveredRepository.Add(documentDelivered);
            try
            {
                _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDelivered", documentDelivered.Id.ToString(), "", "", documentDelivered, "", HttpContext.Current.Request.UserHostAddress, documentDelivered.CreatedBy);
            }
            catch { }

            if (addDocumentDeliveredResponse == null)
            {
                response.IsSuccess = false;
                response.Message = "Không thể tạo văn bản đi. Xin vui lòng thử lại.";
                return response;
            }
            else
            {
                response.Value = addDocumentDeliveredResponse;
            }

            // add recipent
            List<DocumentRecipent> listDocumentRecipent = item.DocumentRecipents.ToList();
            if (listDocumentRecipent.Count > 0)
            {
                foreach (var itemRecipent in listDocumentRecipent)
                {
                    try
                    {
                        itemRecipent.CreatedOn = DateTime.Now;
                        itemRecipent.DocumentId = addDocumentDeliveredResponse.Id;
                        _documentRecipentRepository.Add(itemRecipent);
                        try
                        {
                            _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentRecipent", itemRecipent.Id.ToString(), "", "", itemRecipent, "", HttpContext.Current.Request.UserHostAddress, itemRecipent.CreatedBy);
                        }
                        catch { }
                    }
                    catch
                    {
                        response.IsSuccess = false;
                        response.Message = "Không thể tạo chi tiết cho văn bản đến. Xin vui lòng thử lại.";
                    }
                }

            }

            #region send notification
            var listUserSendNotifi = new List<SPGetUserNotificationForDocumentAndEvent_Result>();
            var listRecipents = listDocumentRecipent;
            foreach (var re in listRecipents)
            {
                if (re.DepartmentId != null)
                {
                    listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByDepartmentId(re.DepartmentId.Value));
                }
                else
                {
                    var listUser = _userNotificationRepository.GetUserNotificationByUserId(re.UserId);
                    listUserSendNotifi.AddRange(_userNotificationRepository.ConvertToNotificationDocumentAndEvent(listUser));
                }
            }

            NotificationCenter resultNotifiCenter = null;
            NotificationCenter notifiFirst = null;
            foreach (var u in listUserSendNotifi)
            {
                var user = _staffRepository.GetStaffByUserId(item.CreatedBy);
                FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                fcmNotificationCenter.Avatar = user.Avatar;
                fcmNotificationCenter.FullName = user.FullName;
                fcmNotificationCenter.Content = "đã cập nhật một văn bản mới: \n" + item.Title;
                fcmNotificationCenter.Title = "vOffice";
                fcmNotificationCenter.RecordNumber = item.DocumentNumber;
                fcmNotificationCenter.RecordId = item.Id;
                fcmNotificationCenter.Type = (int)NotificationCode.Document;
                fcmNotificationCenter.CreatedBy = item.CreatedBy;
                fcmNotificationCenter.CreatedOn = item.CreatedOn;
                fcmNotificationCenter.HaveSeen = false;
                fcmNotificationCenter.ReceivedUserId = u.UserId;
                fcmNotificationCenter.DeviceId = u.DeviceId;
                if (notifiFirst != null && notifiFirst.ReceivedUserId == u.UserId)
                {
                    fcmNotificationCenter.GroupId = notifiFirst.Id;
                }
                var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                fcmNotificationCenter.Id = resultNotifiCenter.Id;
                if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                FCMPushNotification fcmPushNotification = new FCMPushNotification();
                fcmPushNotification.SendNotification(fcmNotificationCenter, u.ClientId);
            }

            #endregion
            // add document document filed                       
            if (listDocumentFields.Count > 0)
            {
                try
                {
                    foreach (var docField in listDocumentFields)
                    {
                        docField.CreatedOn = DateTime.Now;
                        docField.DocumentId = addDocumentDeliveredResponse.Id;
                        _documentDocumentFieldReposity.Add(docField);
                        try
                        {
                            _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDocumentField", "", "", "", docField, "", HttpContext.Current.Request.UserHostAddress, docField.CreatedBy);
                        }
                        catch { }
                    }
                }
                catch
                {
                    response.IsSuccess = false;
                    response.Message = "Không thể tạo chi tiết cho văn bản đi.";
                }
            }
            else if (listCostomFields.Count > 0)
            {
                var listCustomFieldReturn = GetListDocFieldDepartmentFromSystem(listCostomFields);
                if (listCustomFieldReturn.Data.Count > 0)
                {
                    try
                    {
                        foreach (var docField in listCustomFieldReturn.Data)
                        {
                            DocumentDocumentField doc2doc = new DocumentDocumentField();
                            doc2doc.DocumentId = addDocumentDeliveredResponse.Id;
                            doc2doc.DocumentFieldDepartmentId = docField.DocumentFieldId;
                            doc2doc.ReceivedDocument = false;
                            doc2doc.CreatedBy = item.CreatedBy;
                            doc2doc.CreatedOn = DateTime.Now;
                            _documentDocumentFieldReposity.Add(doc2doc);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDocumentField", "", "", "", doc2doc, "", HttpContext.Current.Request.UserHostAddress, doc2doc.CreatedBy);
                            }
                            catch { }
                        }
                    }
                    catch
                    {
                        response.IsSuccess = false;
                        response.Message = "Không thể tạo chi tiết cho lĩnh vực văn bản. Xin vui lòng thử lại.";
                    }
                }
            }

            return response;
        }
        public BaseResponse<DocumentDelivered> UpdateComplexDocumentDelivered(ComplexDocumentDelivered item)
        {
            var response = new BaseResponse<DocumentDelivered>();
            if (item != null)
            {

                var customField = item.CustomDocumentFileds;
                var docFields = item.DocumentFields;

                if (customField == null && docFields == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Cập nhật văn bản đi không thành công: lĩnh vực văn bản không được để trống";
                    return response;
                }

                var listCostomFields = new List<CustomDocumentField>();
                var listDocumentFields = new List<DocumentDocumentField>();

                if (customField != null)
                {
                    listCostomFields = customField;
                }
                if (docFields != null)
                {
                    listDocumentFields = docFields.ToList();
                }

                var documentDelivered = _documentDeliveredRepository.GetById(item.Id);
                documentDelivered.DocumentNumber = item.DocumentNumber;
                documentDelivered.Title = item.Title;
                documentDelivered.DocumentDate = item.DocumentDate;
                documentDelivered.DeliveredDate = item.DeliveredDate;
                documentDelivered.DepartmentId = item.DepartmentId;
                documentDelivered.ExternalReceiveDivisionId = item.ExternalReceiveDivisionId;
                documentDelivered.ExternalReceiveDivision = item.ExternalReceiveDivision;
                documentDelivered.RecipientsDivision = item.RecipientsDivision;
                documentDelivered.SignedById = item.SignedById;
                documentDelivered.SignedBy = item.SignedBy;
                documentDelivered.DocumentTypeId = item.DocumentTypeId;
                documentDelivered.NumberOfCopies = item.NumberOfCopies;
                documentDelivered.NumberOfPages = item.NumberOfPages;
                documentDelivered.SecretLevel = item.SecretLevel;
                documentDelivered.UrgencyLevel = item.UrgencyLevel;
                documentDelivered.Note = item.Note;
                documentDelivered.OriginalSavingPlace = item.OriginalSavingPlace;
                documentDelivered.LegalDocument = item.LegalDocument;
                documentDelivered.AttachmentName = item.AttachmentName;
                documentDelivered.AttachmentPath = item.AttachmentPath;
                documentDelivered.ReceivedDocumentId = item.ReceivedDocumentId;
                documentDelivered.Active = item.Active;
                documentDelivered.Deleted = item.Deleted;
                documentDelivered.EditedBy = item.EditedBy;
                documentDelivered.EditedOn = DateTime.Now;

                var addDocumentDeliveredResponse = _documentDeliveredRepository.Edit(documentDelivered);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentDelivered", addDocumentDeliveredResponse.Id.ToString(), "", "", addDocumentDeliveredResponse, "", HttpContext.Current.Request.UserHostAddress, addDocumentDeliveredResponse.CreatedBy);
                }
                catch { }

                if (addDocumentDeliveredResponse == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Không thể tạo văn bản đi. Xin vui lòng thử lại.";
                    return response;
                }
                else
                {
                    response.Value = addDocumentDeliveredResponse;
                }

                /// delete recipent, documentfield
                _documentRecipentRepository.DeleteMulti(x => x.DocumentId == item.Id && x.ReceivedDocument == false);
                _documentDocumentFieldReposity.DeleteMulti(x => x.DocumentId == item.Id && x.ReceivedDocument == false);

                // add recipent, documentfield
                List<DocumentRecipent> listDocumentRecipent = item.DocumentRecipents.ToList();
                if (listDocumentRecipent.Count > 0)
                {
                    foreach (var itemRecipent in listDocumentRecipent)
                    {
                        try
                        {
                            itemRecipent.CreatedOn = DateTime.Now;
                            itemRecipent.DocumentId = addDocumentDeliveredResponse.Id;
                            _documentRecipentRepository.Add(itemRecipent);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentRecipent", itemRecipent.Id.ToString(), "", "", itemRecipent, "", HttpContext.Current.Request.UserHostAddress, itemRecipent.CreatedBy);
                            }
                            catch { }

                        }
                        catch
                        {
                            response.IsSuccess = false;
                            response.Message = "Không thể tạo chi tiết cho văn bản đến. Xin vui lòng thử lại.";
                        }
                    }

                }

                // add document document filed                       
                if (listDocumentFields.Count > 0)
                {
                    try
                    {
                        foreach (var docField in listDocumentFields)
                        {
                            docField.CreatedOn = DateTime.Now;
                            docField.DocumentId = addDocumentDeliveredResponse.Id;
                            _documentDocumentFieldReposity.Add(docField);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDocumentField", "", "", "", docField, "", HttpContext.Current.Request.UserHostAddress, docField.CreatedBy);
                            }
                            catch { }
                        }
                    }
                    catch
                    {
                        response.IsSuccess = false;
                        response.Message = "Không thể tạo chi tiết cho văn bản đi.";
                    }
                }
                else if (listCostomFields.Count > 0)
                {
                    var listCustomFieldReturn = GetListDocFieldDepartmentFromSystem(listCostomFields);
                    if (listCustomFieldReturn.Data.Count > 0)
                    {
                        try
                        {
                            foreach (var docField in listCustomFieldReturn.Data)
                            {
                                DocumentDocumentField doc2doc = new DocumentDocumentField();
                                doc2doc.DocumentId = addDocumentDeliveredResponse.Id;
                                doc2doc.DocumentFieldDepartmentId = docField.DocumentFieldId;
                                doc2doc.ReceivedDocument = false;
                                doc2doc.CreatedBy = item.CreatedBy;
                                doc2doc.CreatedOn = DateTime.Now;
                                _documentDocumentFieldReposity.Add(doc2doc);
                                try
                                {
                                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDocumentField", "", "", "", doc2doc, "", HttpContext.Current.Request.UserHostAddress, doc2doc.CreatedBy);
                                }
                                catch { }
                            }
                        }
                        catch
                        {
                            response.IsSuccess = false;
                            response.Message = "Không thể tạo chi tiết cho lĩnh vực văn bản. Xin vui lòng thử lại.";
                        }
                    }
                }

            }
            return response;
        }
        public BaseResponse<DocumentDelivered> UpdateDocumentDelivered(DocumentDelivered model)
        {
            BaseResponse<DocumentDelivered> response = new BaseResponse<DocumentDelivered>();
            var errors = Validate<DocumentDelivered>(model, new DocumentDeliveredValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentDelivered> errResponse = new BaseResponse<DocumentDelivered>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _documentDeliveredRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentDelivered", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentDelivered> AddDocumentDelivered(DocumentDelivered model)
        {
            var response = new BaseResponse<DocumentDelivered>();
            var errors = Validate<DocumentDelivered>(model, new DocumentDeliveredValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentDelivered> errResponse = new BaseResponse<DocumentDelivered>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _documentDeliveredRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDelivered", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        #endregion DocumentDelivered

        public BaseListResponse<SPGetDocumentAdvance_Result> SearchDocument(DocumentReceivedQuery query)
        {
            var response = new BaseListResponse<SPGetDocumentAdvance_Result>();
            int count = 0;
            try
            {
                response.Data = _documentReceivedRepository.SearchDocument(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<string> DownloadDocumentBook(DocumentReceivedQuery query)
        {
            int count = 0;
            var listSource = _documentReceivedRepository.SearchDocument(query, out count, int.MaxValue);
            var response = new BaseResponse<string>();
            StringBuilder content = new StringBuilder();
            content.Append("<html>");
            content.Append("<body>");
            content.Append("<table style='width:100%;border-collapse: collapse;' border='1' cellspacing='0'>");
            content.Append("<tr >");
            content.Append("<td style='padding:5px 5px;' align=center v-align=top><strong>Số VB</strong></td>");
            content.Append("<td align=center><strong>Ngày VB</strong></td>");
            content.Append("<td align=center><strong>Trích yếu</strong></td>");
            content.Append("<td align=center><strong>Người ký</strong></td>");
            content.Append("<td align=center><strong>Nơi nhận</strong></td>");
            content.Append("<td align=center><strong>Nơi lưu</strong></td>");
            content.Append("<td align=center><strong>Số bản</strong></td>");
            content.Append("<td align=center><strong>Ghi chú</strong></td>");
            content.Append("</tr>");

            foreach (var item in listSource)
            {
                content.Append("<tr>");
                content.Append("<td style='padding:0 2px' >").Append(item.DocumentNumber).Append("</td>");
                content.Append("<td style='padding:0 2px'>").Append(string.Format("{0:dd/MM/yyyy}", item.DocumentDate)).Append("</td>");
                content.Append("<td style='padding:0 2px'>").Append(item.Title).Append("</td>");
                content.Append("<td style='padding:0 2px'>").Append(item.SignedBy).Append("</td>");
                content.Append("<td style='padding:0 2px'>").Append(item.ExternalFromDivision).Append("</td>");
                content.Append("<td style='padding:0 2px'>").Append(item.OriginalSavingPlace).Append("</td>");
                content.Append("<td  >").Append(item.NumberOfCopies.Value).Append("</td>");
                content.Append("<td  align=left >").Append(item.Note).Append("</td>");
                content.Append("</tr>");
            }
            content.Append("</table>");
            content.Append("</body>");
            content.Append("</html>");
            response.Value = Util.CreateDocument(content.ToString(), "documentBook", "documentBook1", 20, true);
            return response;
        }
        public BaseResponse<string> DownloadTotalDocument(DateTime fromDate, DateTime toDate, string listDepartmentId, int departmentId)
        {
            int count = 0;
            var listSource = _documentReceivedRepository.DownloadTotalDocument(fromDate, toDate, listDepartmentId);
            var response = new BaseResponse<string>();
            StringBuilder content = new StringBuilder();
            var rootDeparment = _departmentRepository.FindBy(x => x.ParentId == 0).FirstOrDefault();
            var deparment = _departmentRepository.FindBy(x => x.Id == departmentId).FirstOrDefault();
            content.Append("<table style='width:100%;border-collapse: collapse;'   cellspacing='0'>");
            content.Append("<tr>");
            content.Append("<td align=center><span style='font-size:11pt'><b>").Append(rootDeparment.Name.ToUpper()).Append("</b> </span></td>");
            content.Append("<td align=center><span style='font-size:11pt'><b>").Append("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM").Append("</b></span></td>");
            content.Append("</tr>");
            content.Append("<tr>");
            content.Append("<td align=center><span style='font-size:11pt'><b>").Append(deparment.Name.ToUpper()).Append("</b> </span></td>");
            content.Append("<td align=center><span style='font-size:11pt'><b>").Append("ĐỘC LẬP - TỰ DO - HẠNH PHÚC").Append("</b></span></td>");
            content.Append("</tr>");
            content.Append("<tr></tr>");
            content.Append("<tr>");
            content.Append("<td colspan='2' align=center><span style='font-size:14pt'><b>THỐNG KÊ TÌNH  HÌNH GỬI/NHẬN VĂN BẢN</b></span></td>");
            content.Append("</tr>");
            content.Append("<tr>");
            content.Append("<td colspan='2' align=center>").Append(string.Format("<i>(Từ ngày {0:dd/MM/yyyy} đến ngày {1:dd/MM/yyyy})</i>", fromDate, toDate)).Append("</td>");
            content.Append("</tr>");
            content.Append("</table> <br /> ");
            content.Append("<table style='width:100%;border-collapse: collapse;' border='1' cellspacing='0'>");
            content.Append("<tr>");
            content.Append("<td align=center style='padding:15px 5px 15px 5px'><strong>STT</strong></td>");
            content.Append("<td align=center><strong>ĐƠN VỊ</strong></td>");
            content.Append("<td align=center><strong>TỔNG SỐ VB ĐI</strong></td>");
            content.Append("<td align=center><strong>TỔNG SỐ VB ĐẾN</strong></td>");
            content.Append("<td align=center><strong>ĐÃ VÀO SỔ</strong></td>");
            content.Append("<td align=center><strong>CHƯA VÀO SỔ</strong></td>");
            content.Append("</tr>");
            content.Append("<tr>");
            content.Append("<td align=center style='padding:5px;'><strong>(1)</strong></td>");
            content.Append("<td align=center><strong>(2)</strong></td>");
            content.Append("<td align=center><strong>(3)</strong></td>");
            content.Append("<td align=center><strong>(4)</strong></td>");
            content.Append("<td align=center><strong>(5)</strong></td>");
            content.Append("<td align=center><strong>(6)</strong></td>");
            content.Append("</tr>");
            foreach (var item in listSource)
            {
                count++;
                content.Append("<tr>");
                content.Append("<td align=center style='padding:4px;'>").Append(count).Append("</td>");
                content.Append("<td align=left style='padding-left:2px;'>").Append(item.Name).Append("</td>");
                content.Append("<td align=right style='padding-right:4px;'>").Append(item.TotalDocumentDelivered).Append("</td>");
                content.Append("<td align=right style='padding-right:4px;'>").Append(item.TotalDocumentReceived).Append("</td>");
                content.Append("<td align=right style='padding-right:4px;'>").Append(item.AddedDocumentBook).Append("</td>");
                content.Append("<td align=right style='padding-right:4px;'>").Append(item.NotAddedDocumentBook).Append("</td>");
                content.Append("</tr>");
            }
            content.Append("</table>");
            response.Value = Util.CreateDocument(content.ToString(), "totalDocumentBook", "totalDocumentBook", 20, true);
            return response;
        }
        public BaseListResponse<SPGetTotalDocumentReportList_Result> DownloadTotalDocumentList(DocumentReceivedQuery query)
        {
            var response = new BaseListResponse<SPGetTotalDocumentReportList_Result>();
            int count = 0;
            try
            {
                response.Data = _documentReceivedRepository.DownloadTotalDocumentList(query, out count).ToList();
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<SPGetComplexCount_Result> GetComplexCount(string userId, int departmentId)
        {
            var response = new BaseResponse<SPGetComplexCount_Result>();
            try
            {
                response.Value = _documentReceivedRepository.GetComplexCount(userId, departmentId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public BaseResponse<UserDocumentsAnalystic> CountUserDocument(int NumberOfMonths, DocumentReceivedQuery query)
        {
            var result = new BaseResponse<UserDocumentsAnalystic>();
            UserDocumentsAnalystic userDocumentAnalystic = new UserDocumentsAnalystic();

            DocumentReceivedQuery receivedQuery = new DocumentReceivedQuery();
            receivedQuery.Type = "1";
            receivedQuery.StartDate = query.StartDate;
            receivedQuery.EndDate = query.EndDate;
            receivedQuery.UserId = query.UserId;
            receivedQuery.ListSubDepartmentId = query.ListSubDepartmentId;
            receivedQuery.DepartmentId = query.DepartmentId;
            receivedQuery.NumberOfMonthsOrWeeks = query.NumberOfMonthsOrWeeks;
            receivedQuery.ScopeType = query.ScopeType;

            DocumentReceivedQuery deliveredQuery = new DocumentReceivedQuery();
            deliveredQuery.Type = "2";
            deliveredQuery.StartDate = query.StartDate;
            deliveredQuery.EndDate = query.EndDate;
            deliveredQuery.UserId = query.UserId;
            deliveredQuery.ListSubDepartmentId = query.ListSubDepartmentId;
            deliveredQuery.DepartmentId = query.DepartmentId;
            deliveredQuery.NumberOfMonthsOrWeeks = query.NumberOfMonthsOrWeeks;
            deliveredQuery.ScopeType = query.ScopeType;


            DocumentReceivedQuery mixHaventReadQuery = new DocumentReceivedQuery();
            mixHaventReadQuery.Type = "0";
            mixHaventReadQuery.StartDate = query.StartDate;
            mixHaventReadQuery.EndDate = query.EndDate;
            mixHaventReadQuery.UserId = query.UserId;
            mixHaventReadQuery.ListSubDepartmentId = query.ListSubDepartmentId;
            mixHaventReadQuery.DepartmentId = query.DepartmentId;
            mixHaventReadQuery.NumberOfMonthsOrWeeks = query.NumberOfMonthsOrWeeks;
            mixHaventReadQuery.ScopeType = query.ScopeType;


            var currentMonth = DateTime.Now.Month;


            List<int> receivedDoc = new List<int>();
            List<int> deliveredDoc = new List<int>();
            List<int> numberDocumentHaveNotRead = new List<int>();
            List<int> numberDocumentHaveNotAddedDocumentBook = new List<int>();

            int currentYear = DateTime.Now.Year;


            if (query.TypeOfTime == "MONTH")
            {
                receivedDoc = new List<int>();
                deliveredDoc = new List<int>();
                numberDocumentHaveNotRead = new List<int>();
                for (int i = currentMonth - (NumberOfMonths - 1); i <= currentMonth; i++)
                {
                    receivedQuery.StartDate = new DateTime(currentYear, i, 1);
                    receivedQuery.EndDate = new DateTime(currentYear, i, DateTime.DaysInMonth(currentYear, i));
                    receivedDoc.Add(_documentReceivedRepository.CountUserDocument(receivedQuery));

                    deliveredQuery.StartDate = new DateTime(currentYear, i, 1);
                    deliveredQuery.EndDate = new DateTime(currentYear, i, DateTime.DaysInMonth(currentYear, i));
                    deliveredDoc.Add(_documentReceivedRepository.CountUserDocument(deliveredQuery));

                    mixHaventReadQuery.StartDate = new DateTime(currentYear, i, 1);
                    mixHaventReadQuery.EndDate = new DateTime(currentYear, i, DateTime.DaysInMonth(currentYear, i));
                    numberDocumentHaveNotRead.Add(_documentReceivedRepository.CountUserDocumentHaventRead(mixHaventReadQuery));

                    var complexCount = _documentReceivedRepository.DownloadTotalDocument(new DateTime(currentYear, i, 1), new DateTime(currentYear, i, DateTime.DaysInMonth(currentYear, i)), query.DepartmentId.ToString()).FirstOrDefault();
                    int notadded = 0;
                    if (complexCount != null)
                    {
                        notadded = complexCount.NotAddedDocumentBook != null ? complexCount.NotAddedDocumentBook.Value : 0;
                    }
                    numberDocumentHaveNotAddedDocumentBook.Add(notadded);
                }
            }
            else
            {
                receivedDoc = new List<int>();
                deliveredDoc = new List<int>();
                numberDocumentHaveNotRead = new List<int>();
                numberDocumentHaveNotAddedDocumentBook = new List<int>();
                List<DateTime> listMondays = new List<DateTime>();
                if (query.TypeOfTime == "WEEK")
                {

                    for (int i = 0; i < NumberOfMonths; i++)
                    {
                        DateTime dateToCount = DateTime.Now.AddDays(0 - i * 7);
                        DateTime monday = dateToCount.GetFirstDayOfWeek();
                        listMondays.Add(monday);
                    }
                }
                listMondays.Reverse();
                foreach (DateTime monday in listMondays)
                {
                    DateTime sunday = new DateTime();
                    sunday = monday.AddDays(6);
                    receivedQuery.StartDate = new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0);
                    receivedQuery.EndDate = new DateTime(sunday.Year, sunday.Month, sunday.Day, 23, 59, 00);
                    receivedDoc.Add(_documentReceivedRepository.CountUserDocument(receivedQuery));


                    deliveredQuery.StartDate = new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0);
                    deliveredQuery.EndDate = new DateTime(sunday.Year, sunday.Month, sunday.Day, 23, 59, 00);
                    deliveredDoc.Add(_documentReceivedRepository.CountUserDocument(deliveredQuery));


                    mixHaventReadQuery.StartDate = new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0);
                    mixHaventReadQuery.EndDate = new DateTime(sunday.Year, sunday.Month, sunday.Day, 23, 59, 00);
                    numberDocumentHaveNotRead.Add(_documentReceivedRepository.CountUserDocumentHaventRead(mixHaventReadQuery));


                    var complexCount = _documentReceivedRepository.DownloadTotalDocument(new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0), new DateTime(sunday.Year, sunday.Month, sunday.Day, 23, 59, 00), query.DepartmentId.ToString()).FirstOrDefault();
                    int notadded = 0;
                    if (complexCount != null)
                    {
                        notadded = complexCount.NotAddedDocumentBook != null ? complexCount.NotAddedDocumentBook.Value : 0;

                    }
                    numberDocumentHaveNotAddedDocumentBook.Add(notadded);
                }
            }





            userDocumentAnalystic.NumberDocumentReceived = receivedDoc;
            userDocumentAnalystic.NumberDocumentDelivered = deliveredDoc;
            userDocumentAnalystic.NumberDocumentHaveNotRead = numberDocumentHaveNotRead;
            userDocumentAnalystic.NumberDocumentHaventAddedDocumentBook = numberDocumentHaveNotAddedDocumentBook;
            result.Value = userDocumentAnalystic;
            return result;
        }
        public BaseListResponse<SPGetDocument_Result> GetDocumentUnRead(DocumentReceivedQuery query)
        {
            var response = new BaseListResponse<SPGetDocument_Result>();
            int count = 0;
            try
            {
                var listResult = _documentReceivedRepository.Filter(query, out count);
                var results = listResult.Where(x => x.HistoryId == null).ToList();
                response.Data = results.Take(5).ToList();
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetDocumentDeliveredStatistics_Result> GetDocumentDeliveredStatisticsList(DocumentDeliveredStatisticsQuery query)
        {
            var response = new BaseListResponse<SPGetDocumentDeliveredStatistics_Result>();
            int count = 0;
            try
            {
                response.Data = _documentDeliveredRepository.GetDocumentDeliveredStatisticsList(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<string> GetDocumentDeliveredStatisticsDownload(DocumentDeliveredStatisticsQuery query)
        {
            var response = new BaseResponse<string>();
            var staff = _staffRepository.GetStaffByUserId(query.UserId);

            int count = 0;
            try
            {
                var listAllSource = _documentDeliveredRepository.GetDocumentDeliveredStatisticsDownload(query, out count);
                StringBuilder content = new StringBuilder();
                var rootDeparment = _departmentRepository.FindBy(x => x.ParentId == 0).FirstOrDefault();
                var deparment = _departmentRepository.FindBy(x => x.Id == query.DepartmentId).FirstOrDefault();
                var listTitle = new List<SPGetDocumentDeliveredStatistics_Result>();

                switch (query.GroupBy)
                {
                    case SIGNBY:
                        listTitle = listAllSource.GroupBy(x => new { x.SignedById, x.DocumentSignBy, x.Position })
                                            .OrderBy(z => z.Key.DocumentSignBy)
                                            .Select(y => new SPGetDocumentDeliveredStatistics_Result
                                            {
                                                Title = y.Key.SignedById.ToString(),
                                                GroupTitle = !string.IsNullOrEmpty(y.Key.Position) ? y.Key.DocumentSignBy + " - " + y.Key.Position : y.Key.DocumentSignBy
                                            }).ToList();
                        break;
                    case DOCUMENTTYPE:
                        listTitle = listAllSource.GroupBy(x => new { x.DocumentTypeId, x.DocumentType })
                                            .OrderBy(z => z.Key.DocumentType)
                                            .Select(y => new SPGetDocumentDeliveredStatistics_Result
                                            {
                                                Title = y.Key.DocumentTypeId.ToString(),
                                                GroupTitle = y.Key.DocumentType
                                            }).ToList();
                        break;
                    case DOCUMENTFIELD:
                        listTitle = listAllSource.GroupBy(x => new { x.DocumentFieldId, x.DocumentField })
                                           .OrderBy(z => z.Key.DocumentField)
                                           .Select(y => new SPGetDocumentDeliveredStatistics_Result
                                           {
                                               Title = y.Key.DocumentFieldId.ToString(),
                                               GroupTitle = y.Key.DocumentField
                                           }).ToList();
                        break;
                }

                content.Append("<table style='width:100%; border-collapse: collapse;' cellspacing='0'>");
                content.Append("<tr>");
                content.Append("<td align=center><span style='font-size:11pt'><b>").Append(rootDeparment.Name.ToUpper()).Append("</b> </span></td>");
                content.Append("<td align=center><span style='font-size:11pt'><b>").Append("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM").Append("</b></span></td>");
                content.Append("</tr>");
                content.Append("<tr>");
                content.Append("<td align=center><span style='font-size:11pt'><b>").Append(deparment.Name.ToUpper()).Append("</b> </span></td>");
                content.Append("<td align=center><span style='font-size:11pt'><b>").Append("ĐỘC LẬP - TỰ DO - HẠNH PHÚC").Append("</b></span></td>");
                content.Append("</tr>");
                content.Append("<tr></tr>");
                content.Append("<tr>");
                content.Append("<td colspan='2' align=center><span style='font-size:14pt'><b>THỐNG KÊ TÌNH HÌNH KÍ VĂN BẢN ĐI</b></span></td>");
                content.Append("</tr>");
                content.Append("<tr>");
                content.Append("<td colspan='2' align=center>");
                if (query.StartDate != null && query.EndDate != null)
                {
                    content.Append(string.Format("<i>(Từ ngày {0:dd/MM/yyyy} đến ngày {1:dd/MM/yyyy})</i>", query.StartDate, query.EndDate));
                }
                content.Append("</td>");
                content.Append("</tr>");
                content.Append("</table> <br /> ");
                content.Append("<table style='width:100%;border-collapse: collapse;' border='1' cellspacing='0'>");
                content.Append("<tr>");
                content.Append("<td align=center style='padding:15px 5px 15px 5px'><strong>STT</strong></td>");
                content.Append("<td align=center><strong>NGƯỜI KÝ/SỐ VĂN BẢN - TRÍCH YẾU</strong></td>");
                content.Append("<td align=center><strong>NGÀY VĂN BẢN</strong></td>");
                content.Append("<td align=center><strong>NGÀY CẬP NHẬT HỆ THỐNG</strong></td>");
                switch (query.GroupBy)
                {
                    case DOCUMENTTYPE:
                        content.Append("<td align=center><strong>LĨNH VỰC VĂN BẢN</strong></td>");
                        content.Append("<td align=center><strong>NGƯỜI KÝ</strong></td>");
                        break;
                    case DOCUMENTFIELD:
                        content.Append("<td align=center><strong>NGƯỜI KÝ</strong></td>");
                        content.Append("<td align=center><strong>LOẠI VĂN BẢN</strong></td>");
                        break;
                    default:
                        content.Append("<td align=center><strong>LĨNH VỰC VĂN BẢN</strong></td>");
                        content.Append("<td align=center><strong>LOẠI VĂN BẢN</strong></td>");
                        break;
                }

                content.Append("</tr>");
                content.Append("<tr>");
                content.Append("<td align=center style='padding:5px;'><strong>(1)</strong></td>");
                content.Append("<td align=center><strong>(2)</strong></td>");
                content.Append("<td align=center><strong>(3)</strong></td>");
                content.Append("<td align=center><strong>(4)</strong></td>");
                content.Append("<td align=center><strong>(5)</strong></td>");
                content.Append("<td align=center><strong>(6)</strong></td>");
                content.Append("</tr>");
                foreach (var item in listTitle)
                {
                    count++;
                    content.Append("<tr>");
                    content.Append("<td align=left style='padding:4px;height:40px;' colspan='6'><b>").Append(item.GroupTitle).Append("</b></td>");
                    content.Append("</tr>");
                    var listSouceContent = new List<SPGetDocumentDeliveredStatistics_Result>();
                    switch (query.GroupBy)
                    {
                        case SIGNBY:

                            listSouceContent = listAllSource.Where(x => x.SignedById.ToString() == item.Title)
                                .GroupBy(x => new { x.DocumentDate, x.DocumentNumber, x.Title, x.CreateDate, x.DocumentType, x.DocumentId })
                                .Select(y => new SPGetDocumentDeliveredStatistics_Result
                                {
                                    DocumentDate = y.Key.DocumentDate,
                                    DocumentNumber = y.Key.DocumentNumber,
                                    Title = y.Key.Title,
                                    CreateDate = y.Key.CreateDate,
                                    DocumentType = y.Key.DocumentType,
                                    DocumentField = string.Join(",", listAllSource.Where(x => x.SignedById.ToString() == item.Title && x.DocumentId == y.Key.DocumentId)
                                                                .Select(a => a.DocumentField).ToArray())
                                }).ToList();

                            if (listSouceContent.Count > 0)
                            {
                                count = 0;
                                foreach (var detail in listSouceContent)
                                {
                                    count++;
                                    content.Append("<tr>");
                                    content.Append("<td align=center>").Append(count).Append("</td>");
                                    content.Append("<td align=left style='padding:5px;width:50%;'><i>").Append(detail.DocumentNumber).Append("</i>").Append(string.Format("{0}", detail.DocumentNumber == null ? "" : "<br />")).Append(detail.Title).Append("</td>");
                                    content.Append("<td align=center style='width:10%;'>").Append(string.Format("{0:dd/MM/yyyy}", detail.DocumentDate)).Append("</td>");
                                    content.Append("<td align=center style='width:10%;'>").Append(string.Format("{0:dd/MM/yyyy}", detail.CreateDate)).Append("</td>");
                                    content.Append("<td align=center>").Append(detail.DocumentField).Append("</td>");
                                    content.Append("<td align=center>").Append(detail.DocumentType).Append("</td>");
                                    content.Append("</tr>");
                                }

                            }
                            break;
                        case DOCUMENTTYPE:
                            listSouceContent = listAllSource.Where(x => x.DocumentTypeId.ToString() == item.Title)
                                .GroupBy(x => new { x.DocumentDate, x.DocumentNumber, x.Title, x.CreateDate, x.DocumentSignBy, x.DocumentId })
                                .Select(y => new SPGetDocumentDeliveredStatistics_Result
                                {
                                    DocumentDate = y.Key.DocumentDate,
                                    DocumentNumber = y.Key.DocumentNumber,
                                    Title = y.Key.Title,
                                    CreateDate = y.Key.CreateDate,
                                    DocumentSignBy = y.Key.DocumentSignBy,
                                    DocumentField = string.Join(",", listAllSource.Where(x => x.DocumentTypeId.ToString() == item.Title && x.DocumentId == y.Key.DocumentId)
                                                                .Select(a => a.DocumentField).ToArray())
                                }).ToList();
                            if (listSouceContent.Count > 0)
                            {
                                count = 0;
                                foreach (var detail in listSouceContent)
                                {
                                    count++;
                                    content.Append("<tr>");
                                    content.Append("<td align=center>").Append(count).Append("</td>");
                                    content.Append("<td align=left style='padding:5px;width:50%;'><i>").Append(detail.DocumentNumber).Append("</i>").Append(string.Format("{0}", detail.DocumentNumber == null ? "" : "<br />")).Append(detail.Title).Append("</td>");
                                    content.Append("<td align=center style='width:10%;'>").Append(string.Format("{0:dd/MM/yyyy}", detail.DocumentDate)).Append("</td>");
                                    content.Append("<td align=center style='width:10%;'>").Append(string.Format("{0:dd/MM/yyyy}", detail.CreateDate)).Append("</td>");
                                    content.Append("<td align=center>").Append(detail.DocumentField).Append("</td>");
                                    content.Append("<td align=center style='width:15%;'>").Append(detail.DocumentSignBy).Append("</td>");
                                    content.Append("</tr>");
                                }

                            }
                            break;
                        case DOCUMENTFIELD:
                            listSouceContent = listAllSource.Where(x => x.DocumentFieldId.ToString() == item.Title)
                               .Select(y => new SPGetDocumentDeliveredStatistics_Result
                               {
                                   DocumentDate = y.DocumentDate,
                                   DocumentNumber = y.DocumentNumber,
                                   Title = y.Title,
                                   CreateDate = y.CreateDate,
                                   DocumentSignBy = y.DocumentSignBy,
                                   DocumentType = y.DocumentType
                               }).ToList();
                            if (listSouceContent.Count > 0)
                            {
                                count = 0;
                                foreach (var detail in listSouceContent)
                                {
                                    count++;
                                    content.Append("<tr>");
                                    content.Append("<td align=center>").Append(count).Append("</td>");
                                    content.Append("<td align=left style='padding:5px;width:50%;'><i>").Append(detail.DocumentNumber).Append("</i>").Append(string.Format("{0}", detail.DocumentNumber == null ? "" : "<br />")).Append(detail.Title).Append("</td>");
                                    content.Append("<td align=center style='width:10%;'>").Append(string.Format("{0:dd/MM/yyyy}", detail.DocumentDate)).Append("</td>");
                                    content.Append("<td align=center style='width:10%;'>").Append(string.Format("{0:dd/MM/yyyy}", detail.CreateDate)).Append("</td>");
                                    content.Append("<td align=center>").Append(detail.DocumentSignBy).Append("</td>");
                                    content.Append("<td align=center>").Append(detail.DocumentType).Append("</td>");
                                    content.Append("</tr>");
                                }

                            }
                            break;
                    }

                }
                content.Append("</table><br/>");
                content.Append("<table style='width:100%'>");
                content.Append("<tr>");
                content.Append("<td style='width:80%'>");
                content.Append("</td>");
                content.Append("<td  align=center style=' padding-bottom:40px; width:200px'>");
                content.Append("<b>NGƯỜI LẬP BIỂU</b>");
                content.Append("</td>");
                content.Append("</tr>");
                content.Append("<tr>");
                content.Append("<td  style='width:80%'>");
                content.Append("</td>");
                content.Append("<td style='height:70px'>");
                content.Append("</td>");
                content.Append("</tr>");
                content.Append("<tr>");
                content.Append("<td  style='width:80%'>");
                content.Append("</td>");
                content.Append("<td align=center>");
                content.Append("<b>" + staff.FullName.ToUpper() + "</b>");
                content.Append("</td>");
                content.Append("</tr>");
                content.Append("</table>");
                response.Value = Util.CreateDocument(content.ToString(), "totalDocumentBook", "totalDocumentBook", 20, true);
                return response;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<bool> CheckPermissionUserDocument(string userId, int documentId, bool receivedDocument, string listIdDepartment)
        {
            BaseResponse<bool> result = new BaseResponse<bool>();
            bool IsApprove = false;
            IsApprove = _documentReceivedRepository.CheckPermissionUserDocument(userId,documentId,receivedDocument, listIdDepartment);
            result.Value = IsApprove;
            return result;
        }
        #endregion

        #region DocumentHistory
        public BaseResponse<DocumentHistory> AddDocumentHistory(DocumentHistory model)
        {
            var response = new BaseResponse<DocumentHistory>();
            var errors = Validate<DocumentHistory>(model, new DocumentHistoryValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentHistory> errResponse = new BaseResponse<DocumentHistory>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.AttempOn = DateTime.Now;
                var exist = _documentHistoryRepository.FindBy(x => x.DocumentId == model.DocumentId && x.ReceivedDocument == model.ReceivedDocument && x.UserId == model.UserId).FirstOrDefault();
                if (exist == null)
                {
                    response.Value = _documentHistoryRepository.Add(model);
                }
                else
                {
                    response.Value = exist;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        #endregion DocumentHistory

        #region DocumentField
        public BaseResponse<DocumentField> GetDocumentFieldById(int id)
        {
            var response = new BaseResponse<DocumentField>();
            try
            {
                response.Value = _documentFieldRepository.GetById(id);

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetDocumentField_Result> FilterDocumentField(DocumentFieldQuery query)
        {
            var response = new BaseListResponse<SPGetDocumentField_Result>();
            int count = 0;
            try
            {
                response.Data = _documentFieldRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<DocumentField> GetAllDocumentField()
        {
            var response = new BaseListResponse<DocumentField>();
            try
            {
                var result = _documentFieldRepository.GetAll().Where(x => x.Deleted == false && x.Active == true).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentField> AddDocumentField(DocumentField model)
        {
            var response = new BaseResponse<DocumentField>();
            var errors = Validate<DocumentField>(model, new DocumentFieldValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentField> errResponse = new BaseResponse<DocumentField>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            var listDocField = _documentFieldRepository.GetAll().Where(x => x.Code.ToLower() == model.Code.ToLower()).ToList();
            if (listDocField.Count > 0)
            {
                response.IsSuccess = false;
                response.Message = "Mã lĩnh vực đã tồn tại";
            }
            try
            {
                if (listDocField.Count == 0)
                {
                    model.CreatedOn = DateTime.Now;
                    response.Value = _documentFieldRepository.Add(model);
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentField", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentField> AddDocumentFieldSystem(DocumentField model)
        {
            var response = new BaseResponse<DocumentField>();
            var errors = Validate<DocumentField>(model, new DocumentFieldValidator());
            DepartmentQuery query = new DepartmentQuery();
            query.Keyword = "";
            query.ParentId = 0;
            query.Active = true;
            List<SPGetDepartment_Result> arrDepartments = _departmentRepository.Filter(query).Where(n => n.ParentId != 0).ToList();
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentField> errResponse = new BaseResponse<DocumentField>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                response.Value = _documentFieldRepository.Add(model);
                DocumentFieldDepartment documentFieldDepartment;
                if (arrDepartments.Count > 0)
                {
                    foreach (var item in arrDepartments)
                    {
                        documentFieldDepartment = new DocumentFieldDepartment();
                        documentFieldDepartment.Code = model.Code;
                        documentFieldDepartment.Title = model.Title;
                        documentFieldDepartment.FieldId = response.Value.Id;
                        documentFieldDepartment.DepartmentId = item.Id;
                        documentFieldDepartment.CreatedOn = DateTime.Now;
                        documentFieldDepartment.CreatedBy = model.CreatedBy;
                        documentFieldDepartment.EditedOn = DateTime.Now;
                        documentFieldDepartment.EditedBy = model.EditedBy;
                        documentFieldDepartment.Active = true;
                        documentFieldDepartment.Deleted = false;
                        _documentFieldDepartmentRepository.Add(documentFieldDepartment);
                        try
                        {
                            _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentFieldDepartment", documentFieldDepartment.Id.ToString(), "", "", documentFieldDepartment, "", HttpContext.Current.Request.UserHostAddress, documentFieldDepartment.CreatedBy);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPCopyDocumentField_Result> GetDocumentFieldaNotInDepartment(int departmentId)
        {
            var response = new BaseListResponse<SPCopyDocumentField_Result>();
            try
            {
                var result = _documentFieldRepository.GetDocumentFieldaNotInDepartment(departmentId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse CloneDocumentFieldSystem(string listDepartmentId)
        {
            BaseResponse<DocumentField> response = new BaseResponse<DocumentField>();
            var result = new BaseListResponse<SPCopyDocumentField_Result>();
            string[] arr = listDepartmentId.Split(',');
            if (arr.Count() > 0)
            {
                foreach (var item_arr in arr)
                {
                    if (!string.IsNullOrEmpty(item_arr.Replace(',', ' ')))
                    {
                        result.Data = _documentFieldRepository.GetDocumentFieldaNotInDepartment(int.Parse(item_arr));
                        foreach (var item in result.Data)
                        {
                            DocumentFieldDepartment model = new DocumentFieldDepartment();
                            model.FieldId = item.Id;
                            model.Title = item.Title;
                            model.Code = item.Code;
                            model.DepartmentId = int.Parse(item_arr);
                            model.CreatedOn = DateTime.Now;
                            model.CreatedBy = item.CreatedBy;
                            model.EditedOn = DateTime.Now;
                            model.EditedBy = item.EditedBy;
                            model.Active = item.Active;
                            model.Deleted = false;
                            _documentFieldDepartmentRepository.Add(model);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentFieldDepartment", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                            }
                            catch { }
                        }
                    }
                }
            }
            return response;
        }
        public BaseResponse<DocumentField> UpdateDocumentField(DocumentField model)
        {
            BaseResponse<DocumentField> response = new BaseResponse<DocumentField>();
            var errors = Validate<DocumentField>(model, new DocumentFieldValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentField> errResponse = new BaseResponse<DocumentField>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var docField = _documentFieldRepository.GetById(model.Id);
                if (docField.Code != model.Code)
                {
                    IEnumerable<DocumentField> listDocField = _documentFieldRepository.FindBy(x => x.Code.ToLower() == model.Code.ToLower() && x.Deleted == false);
                    if (listDocField.Count() > 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Mã lĩnh vực đã tồn tại.";
                    }
                    else
                    {
                        model.EditedOn = DateTime.Now;
                        response.Value = _documentFieldRepository.Update(docField, model);
                    }
                }
                else
                {
                    model.EditedOn = DateTime.Now;
                    response.Value = _documentFieldRepository.Update(docField, model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentField", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalDocumentField(int id)
        {
            BaseResponse response = new BaseResponse();
            DocumentField model = _documentFieldRepository.GetById(id);
            try
            {
                model.Deleted = true;
                model.EditedOn = DateTime.Now;
                _documentFieldRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentField", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        #endregion DocumentField

        #region DocumentFielDepartment
        public BaseResponse<DocumentFieldDepartment> GetDocumentFieldDepartmentById(int id)
        {
            var response = new BaseResponse<DocumentFieldDepartment>();
            try
            {
                response.Value = _documentFieldDepartmentRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetDocumentFieldDepartment_Result> FilterDocumentFieldDepartment(int departmentID)
        {
            var response = new BaseListResponse<SPGetDocumentFieldDepartment_Result>();
            try
            {
                response.Data = _documentFieldDepartmentRepository.Filter(departmentID);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetListDocumentFieldDepartment_Result> GetListDocumentFieldDepartment(DocumentFieldDepartmentQuery query)
        {
            var response = new BaseListResponse<SPGetListDocumentFieldDepartment_Result>();
            int count = 0;
            try
            {
                response.Data = _documentFieldDepartmentRepository.GetList(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<DocumentFieldDepartment> GetAllDocumentFieldDepartment()
        {
            var response = new BaseListResponse<DocumentFieldDepartment>();
            try
            {
                var result = _documentFieldDepartmentRepository.GetAll().Where(x => x.Deleted == false && x.Active == true).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentFieldDepartment> AddDocumentFieldDepartment(DocumentFieldDepartment model)
        {
            var response = new BaseResponse<DocumentFieldDepartment>();
            var errors = Validate<DocumentFieldDepartment>(model, new DocumentFieldDepartmentValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentFieldDepartment> errResponse = new BaseResponse<DocumentFieldDepartment>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                IEnumerable<DocumentFieldDepartment> lstDocumentFieldDepartment = _documentFieldDepartmentRepository.FindBy(n => n.DepartmentId == model.DepartmentId && n.Code.ToLower() == model.Code.ToLower() && n.Deleted == false);
                if (lstDocumentFieldDepartment.Count() > 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã lĩnh vực đã tồn tại";
                    return response;
                }
                else
                {
                    model.CreatedOn = DateTime.Now;
                    response.Value = _documentFieldDepartmentRepository.Add(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentFieldDepartment", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentFieldDepartment> UpdateDocumentFieldDepartment(DocumentFieldDepartment model)
        {
            BaseResponse<DocumentFieldDepartment> response = new BaseResponse<DocumentFieldDepartment>();
            var errors = Validate<DocumentFieldDepartment>(model, new DocumentFieldDepartmentValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentFieldDepartment> errResponse = new BaseResponse<DocumentFieldDepartment>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var docField = _documentFieldDepartmentRepository.GetById(model.Id);
                if (docField.Code.ToLower() != model.Code.ToLower())
                {
                    IEnumerable<DocumentFieldDepartment> listDocumentFieldDepartment = _documentFieldDepartmentRepository.FindBy(x => x.Code.ToLower() == model.Code.ToLower() && x.DepartmentId == model.DepartmentId && x.Deleted == false);
                    if (listDocumentFieldDepartment.Count() > 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Mã lĩnh vực đã tồn tại.";
                        return response;
                    }
                    else
                    {
                        model.EditedOn = DateTime.Now;
                        response.Value = _documentFieldDepartmentRepository.Update(docField, model);
                        try
                        {
                            _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentFieldDepartment", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                        }
                        catch { }
                    }
                }
                else
                {
                    model.EditedOn = DateTime.Now;
                    response.Value = _documentFieldDepartmentRepository.Update(docField, model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentFieldDepartment", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch { }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalDocumentFieldDepartment(int id)
        {
            BaseResponse response = new BaseResponse();
            DocumentFieldDepartment model = _documentFieldDepartmentRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _documentFieldDepartmentRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "DocumentFieldDepartment", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<CustomDocumentField> GetListDocFieldDepartmentFromSystem(List<CustomDocumentField> listInputDocumentFieldDepartment)
        {
            BaseListResponse<CustomDocumentField> response = new BaseListResponse<CustomDocumentField>();
            List<CustomDocumentField> lstCustomerDocField = new List<CustomDocumentField>();
            CustomDocumentField model;
            DocumentFieldDepartment docFieldDepartment = new DocumentFieldDepartment();
            List<DocumentFieldDepartment> listDocFieldDepartment = new List<DocumentFieldDepartment>();
            try
            {
                foreach (var item in listInputDocumentFieldDepartment)
                {
                    docFieldDepartment = GetDocumentFieldDepartmentById(item.DocumentFieldId).Value;
                    listDocFieldDepartment = _documentFieldDepartmentRepository.GetAll().Where(x => x.Deleted == false && x.Active == true && x.FieldId == docFieldDepartment.FieldId && x.DepartmentId == item.DepartmentId).ToList();
                    if (listDocFieldDepartment.Count > 0)
                    {
                        foreach (var obj in listDocFieldDepartment)
                        {
                            model = new CustomDocumentField();
                            model.DocumentFieldId = obj.Id;
                            model.DepartmentId = obj.DepartmentId;
                            lstCustomerDocField.Add(model);
                        }
                    }
                }
                response.Data = lstCustomerDocField;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetDocFieldDepartmentByDocIdAndReceivedDoc_Result> GetDocFieldDeaprtmentByDocIdAndReceivedDoc(int docId, bool receivedDoc)
        {
            var response = new BaseListResponse<SPGetDocFieldDepartmentByDocIdAndReceivedDoc_Result>();
            try
            {
                response.Data = _documentFieldDepartmentRepository.GetListByDocIdAndReceivedDoc(docId, receivedDoc);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        #endregion

        #region DocumentDocumentField
        public bool GetDocDocFieldByDocIdDocFieldDepartmentId(int docId, int docFieldDepartmentId, bool receivedDocument)
        {
            bool response = false;
            try
            {
                var result = _documentDocumentFieldReposity.GetAll().Where(n => n.DocumentId == docId && n.DocumentFieldDepartmentId == docFieldDepartmentId && n.ReceivedDocument == receivedDocument);
                if (result.Count() > 0)
                {
                    response = true;
                }
                else
                    response = false;

            }
            catch (Exception ex)
            {
            }
            return response;
        }
        public BaseListResponse<SPGetDocumentDocumentField_Result> FilterDocumentDocumentField(DocumentDocumentFieldQuery query)
        {
            var response = new BaseListResponse<SPGetDocumentDocumentField_Result>();
            int count = 0;
            try
            {
                response.Data = _documentDocumentFieldReposity.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<DocumentDocumentField> GetAllDocumentDocumentField()
        {
            var response = new BaseListResponse<DocumentDocumentField>();
            try
            {
                var result = _documentDocumentFieldReposity.GetAll().ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DocumentDocumentField> AddDocumentDocumentField(DocumentDocumentField model)
        {
            var response = new BaseResponse<DocumentDocumentField>();
            var errors = Validate<DocumentDocumentField>(model, new DocumentDocumentFieldValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentDocumentField> errResponse = new BaseResponse<DocumentDocumentField>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _documentDocumentFieldReposity.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDocumentField", "", "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<DocumentDocumentField> AddDocumentDocumentFields(List<DocumentDocumentField> models)
        {
            var response = new BaseListResponse<DocumentDocumentField>();
            List<DocumentDocumentField> resultSet = new List<DocumentDocumentField>();
            foreach (var model in models)
            {
                var errors = Validate<DocumentDocumentField>(model, new DocumentDocumentFieldValidator());
                if (errors.Count() > 0)
                {
                    BaseListResponse<DocumentDocumentField> errResponse = new BaseListResponse<DocumentDocumentField>();
                    errResponse.IsSuccess = false;
                    return errResponse;
                }
                if (GetDocDocFieldByDocIdDocFieldDepartmentId(model.DocumentId, model.DocumentFieldDepartmentId, model.ReceivedDocument) == true)
                {
                    response.IsSuccess = false;
                    return response;
                }
            }
            foreach (var model in models)
            {
                model.CreatedOn = DateTime.Now;
                resultSet.Add(_documentDocumentFieldReposity.Add(model));
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentDocumentField", "", "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            response.Data = resultSet;
            return response;
        }
        public BaseResponse<DocumentDocumentField> UpdateDocumentDocumentField(DocumentDocumentField model)
        {
            BaseResponse<DocumentDocumentField> response = new BaseResponse<DocumentDocumentField>();
            var errors = Validate<DocumentDocumentField>(model, new DocumentDocumentFieldValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentDocumentField> errResponse = new BaseResponse<DocumentDocumentField>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                response.Value = _documentDocumentFieldReposity.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentDocumentField", "", "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalDocumentDocumentField(int id)
        {
            BaseResponse response = new BaseResponse();
            return response;
        }
        public BaseResponse DeleteDocumentDocumentFields(List<DocumentDocumentField> models)
        {
            BaseResponse response = new BaseResponse();
            foreach (var model in models)
            {
                _documentDocumentFieldReposity.Delete(model);
            }
            return response;
        }
        public BaseResponse DeleteDocDocumentFieldsByDocIdAndReceivedDoc(CustomDocumentRecipent model)
        {
            BaseResponse response = new BaseResponse();
            _documentDocumentFieldReposity.DeleteMulti(x => x.DocumentId == model.DocumentId && x.ReceivedDocument == model.ReceivedDocument);
            return response;
        }
        #endregion DocumentDocumentField

        #region DocumentRecipent

        public BaseResponse<DocumentRecipent> AddDocumentRecipent(DocumentRecipent model)
        {
            var response = new BaseResponse<DocumentRecipent>();
            var errors = Validate<DocumentRecipent>(model, new DocumentRecipentValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DocumentRecipent> errResponse = new BaseResponse<DocumentRecipent>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _documentRecipentRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentRecipent", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<DocumentRecipent> AddDocumentRecipents(List<DocumentRecipent> models)
        {
            var response = new BaseListResponse<DocumentRecipent>();
            List<DocumentRecipent> resultSet = new List<DocumentRecipent>();
            foreach (var model in models)
            {
                var errors = Validate<DocumentRecipent>(model, new DocumentRecipentValidator());
                if (errors.Count() > 0)
                {
                    BaseListResponse<DocumentRecipent> errResponse = new BaseListResponse<DocumentRecipent>();
                    errResponse.IsSuccess = false;
                    return errResponse;
                }
            }
            foreach (var model in models)
            {
                model.CreatedOn = DateTime.Now;
                resultSet.Add(_documentRecipentRepository.Add(model));
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentRecipent", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }


            }
            response.Data = resultSet;

            #region send notification

            var listUserSendNotifi = new List<SPGetUserNotificationForDocumentAndEvent_Result>();
            var listRecipents = resultSet;
            foreach (var item in listRecipents)
            {
                if (item.DepartmentId != null)
                {
                    listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByDepartmentId(item.DepartmentId.Value));
                }
                else
                {
                    var listUser = _userNotificationRepository.GetUserNotificationByUserId(item.UserId);
                    listUserSendNotifi.AddRange(_userNotificationRepository.ConvertToNotificationDocumentAndEvent(listUser));
                }
            }
            var document = _documentReceivedRepository.GetById(listRecipents[0].DocumentId);
            NotificationCenter notifiFirst = null;
            NotificationCenter resultNotifiCenter = null;
            foreach (var item in listUserSendNotifi)
            {
                var user = _staffRepository.GetStaffByUserId(document.CreatedBy);
                FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                fcmNotificationCenter.Avatar = user.Avatar;
                fcmNotificationCenter.FullName = user.FullName;
                fcmNotificationCenter.Content = "đã cập nhật một văn bản mới: \n" + document.Title;
                fcmNotificationCenter.Title = "vOffice";
                fcmNotificationCenter.RecordNumber = document.DocumentNumber;
                fcmNotificationCenter.RecordId = document.Id;
                fcmNotificationCenter.Type = (int)NotificationCode.Document;
                fcmNotificationCenter.CreatedBy = document.CreatedBy;
                fcmNotificationCenter.CreatedOn = document.CreatedOn;
                fcmNotificationCenter.HaveSeen = false;
                fcmNotificationCenter.ReceivedUserId = item.UserId;
                fcmNotificationCenter.DeviceId = item.DeviceId;
                if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                {
                    fcmNotificationCenter.GroupId = notifiFirst.Id;
                }
                var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                fcmNotificationCenter.Id = resultNotifiCenter.Id;
                if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                FCMPushNotification fcmPushNotification = new FCMPushNotification();
                fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
            }

            #endregion



            return response;
        }
        public BaseResponse<DocumentRecipent> UpdateDocumentRecipentByDocIdAndReceivedDoc(DocumentRecipent model)
        {
            BaseResponse<DocumentRecipent> response = new BaseResponse<DocumentRecipent>();
            try
            {
                BaseListResponse<SPGetDocumentRecipentByDocIdAndRecivedDoc_Result> listRecipent = GetDocRecipentsByDocIdAndReceived(model.DocumentId, model.ReceivedDocument);
                DocumentRecipent recipent;
                foreach (var item in listRecipent.Data)
                {
                    recipent = new DocumentRecipent();
                    recipent.Id = item.Id;
                    recipent.DocumentId = model.DocumentId;
                    recipent.UserId = item.UserId;
                    recipent.DepartmentId = item.DepartmentId;
                    recipent.ReceivedDocument = model.ReceivedDocument;
                    recipent.Forwarded = item.Forwarded;
                    recipent.Assigned = item.Assigned;
                    recipent.ForSending = item.ForSending;
                    recipent.CreatedBy = item.CreatedBy;
                    recipent.AddedDocumentBook = true;
                    response.Value = _documentRecipentRepository.Edit(recipent);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "DocumentRecipent", recipent.Id.ToString(), "", "", recipent, "", HttpContext.Current.Request.UserHostAddress, recipent.CreatedBy);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteDocumentRecipents(List<DocumentRecipent> models)
        {
            BaseResponse response = new BaseResponse();
            foreach (var model in models)
            {
                _documentRecipentRepository.Delete(model);
            }
            return response;
        }
        public BaseListResponse<SPGetDocumentRecipentByDocIdAndRecivedDoc_Result> GetDocRecipentsByDocIdAndReceived(int documentId, bool receivedDocument)
        {
            var response = new BaseListResponse<SPGetDocumentRecipentByDocIdAndRecivedDoc_Result>();
            try
            {
                response.Data = _documentRecipentRepository.GetRecipentsByDocIdAndReceivedDoc(documentId, receivedDocument);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteRecipentsByDocIdAndReceivedDoc(CustomDocumentRecipent model)
        {
            BaseResponse response = new BaseResponse();
            _documentRecipentRepository.DeleteMulti(x => x.DocumentId == model.DocumentId && x.ReceivedDocument == model.ReceivedDocument);
            return response;
        }
        #endregion

        #region AddesDocumentBook
        public BaseListResponse<SPGetAddedDocumentBook_Result> FilterAddedDocumentBook(DocumentReceivedQuery query)
        {
            var response = new BaseListResponse<SPGetAddedDocumentBook_Result>();
            int count = 0;
            try
            {
                response.Data = _documentReceivedRepository.FilterAddedDocumentBook(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }


        #endregion

        #region HistoryDocument
        public bool checkRecipent(string userId, List<SPGetHistoryDocument_Result> lst)
        {
            int count = 0;
            foreach (var item in lst)
            {
                if (item.UserId != userId)
                    count++;
            }
            if (count == lst.Count)
                return false;
            else
                return true;
        }
        public BaseListResponse<SPGetHistoryDocument_Result> GetHistoryDocument(int documentId, bool receivedDocument)
        {
            var response = new BaseListResponse<SPGetHistoryDocument_Result>();
            List<SPGetDocumentRecipentByDocIdAndRecivedDoc_Result> listRecipents = _documentRecipentRepository.GetRecipentsByDocIdAndReceivedDoc(documentId, receivedDocument).Where(n => n.ForSending == false && n.AddedDocumentBook == true).ToList();
            List<SPGetHistoryDocument_Result> listHistory = new List<SPGetHistoryDocument_Result>();
            List<SPGetHistoryDocument_Result> result = new List<SPGetHistoryDocument_Result>();
            foreach (var item in listRecipents)
            {
                if (!string.IsNullOrEmpty(item.UserId))
                {
                    listHistory = _documentHistoryRepository.GetHistoryDocument(documentId, receivedDocument, 0, item.UserId);
                    foreach (var obj in listHistory)
                    {
                        if (checkRecipent(obj.UserId, result) == false)
                        {
                            obj.AttempOn = obj.AttempOn;
                            result.Add(obj);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(item.DepartmentId.ToString()))
                {
                    listHistory = _documentHistoryRepository.GetHistoryDocument(documentId, receivedDocument, (int)item.DepartmentId, "");
                    foreach (var obj1 in listHistory)
                    {
                        if (checkRecipent(obj1.UserId, result) == false)
                        {
                            obj1.AttempOn = obj1.AttempOn;
                            result.Add(obj1);
                        }

                    }
                }
            }
            response.Data = result;
            return response;
        }
        #endregion

        #region HistoryAddedBook
        public BaseListResponse<SPGetHistoryAddedBookDocument_Result> GetHistoryAddedDocument(int documentId, bool receivedDocument)
        {
            var response = new BaseListResponse<SPGetHistoryAddedBookDocument_Result>();
            response.Data = _documentReceivedRepository.GetHistoryAddedBookDocument(documentId, receivedDocument);
            return response;
        }
        public BaseResponse<bool> CheckHistoryAddedBookDoc(int documentId, bool receivedDocument)
        {
            var response = new BaseResponse<bool>();
            try
            {
                response.Value = _documentReceivedRepository.CheckHistoryAddedBookDoc(documentId, receivedDocument);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }


        #endregion
    }
}

