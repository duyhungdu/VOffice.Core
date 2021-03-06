
GO
/****** Object:  StoredProcedure [doc].[SPGetDocument]    Script Date: 6/19/2017 9:57:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT  EXISTS(SELECT * FROM sys.columns
WHERE Name = N'Assign' AND OBJECT_ID = OBJECT_ID(N'task.Task'))
BEGIN
ALTER TABLE task.Task ADD Assign BIT NOT NULL DEFAULT 0
END  

Go

IF  EXISTS(SELECT * FROM sysobjects AS s
WHERE s.Id = OBJECT_ID(N'[doc].[SPGetDocument]'))
Drop PROCEDURE [doc].[SPGetDocument]
Go
--EXEC [doc].[SPGetDocument] '0','','2017-06-01','2017-06-30','c8df8541-be47-4a62-b217-e4da7bf89f7a',',28,',15,0,1000,0
CREATE PROCEDURE [doc].[SPGetDocument]
	@type NVARCHAR(10),
	@keyword NVARCHAR(500),
	@startDate SMALLDATETIME,
	@endDate SMALLDATETIME,
	@userId NVARCHAR(500),
	@listSubDepartmentId NVARCHAR(500),
	@departmentId INT,
	@start INT,
	@limit INT,
	@total INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	--- Count items ghi ---
	IF @type = '1'
	BEGIN
	    SET @total = (
	            SELECT COUNT(1)
	            FROM   (
	                       SELECT DISTINCT
	                              d.Id
	                       FROM   [doc].DocumentReceived d
	                              JOIN [doc].DocumentRecipent r
	                                   ON  d.Id = r.DocumentId 
	                       WHERE  d.Deleted = 0
	                              AND d.Active = 1
	                              AND d.DepartmentId = @departmentId
	                              AND r.AddedDocumentBook = 1
	                              AND r.ForSending = 0
	                              AND r.ReceivedDocument = 1
	                              AND (
	                                      r.UserId = @userId
	                                      OR r.DepartmentId IN (SELECT VALUE
	                                                            FROM   fn_Split(@listSubDepartmentId, ','))
	                                      OR r.DepartmentId = @departmentId
	                                  )
	                              AND d.DocumentDate BETWEEN @startDate AND @endDate
	                              AND (
	                                      d.Title LIKE N'%' + @keyword + '%'
	                                      OR d.DocumentNumber LIKE N'%' + @keyword + '%'
	                                      OR d.SignedBy LIKE N'%' + @keyword + '%'
	                                      OR d.RecipientsDivision LIKE N'%' + @keyword + '%'
	                                  )
	                   ) documentData
	        );
	    
	    ---- Fetch Data ---
	    SELECT *
	    FROM   (
	               SELECT DISTINCT 'HistoryId'= (SELECT dh.Id FROM doc.DocumentHistory AS dh WHERE dh.DocumentId=d.Id AND dh.UserId=@userId),
	               'TaskId'=(SELECT TOP (1) td.TaskId
	                           FROM task.TaskDocuments AS td JOIN task.Task AS t ON td.TaskId=t.Id WHERE t.[Deleted]=0 AND t.[Active]=1 AND td.DocumentId=d.Id AND td.ReceivedDocument=1),
	                           'TaskCode'=(SELECT TOP (1) t.Code
	                           FROM task.TaskDocuments AS td JOIN task.Task AS t ON td.TaskId=t.Id WHERE t.[Deleted]=0 AND t.[Active]=1 AND td.DocumentId=d.Id AND td.ReceivedDocument=1),
	                       d.Id,d.Title,d.DocumentNumber,d.ReceivedNumber,d.ExternalFromDivision,d.SignedBy,documentDate,d.AttachmentName,d.AttachmentPath, ReceivedDocument=1,d.CreatedOn
	               FROM   [doc].DocumentReceived d
	                      JOIN [doc].DocumentRecipent r
	                           ON  d.Id = r.DocumentId
	               WHERE  d.Deleted = 0
	                      AND d.Active = 1
	                      AND d.DepartmentId = @departmentId
	                      AND r.AddedDocumentBook = 1
	                      
	                      AND r.ForSending = 0
	                      AND r.ReceivedDocument = 1
	                      AND (
	                              r.UserId = @userId
	                              OR r.DepartmentId IN (SELECT VALUE
	                                                    FROM   fn_Split(@listSubDepartmentId, ','))
	                              OR r.DepartmentId = @departmentId
	                          )
	                      AND d.DocumentDate BETWEEN @startDate AND @endDate
	                      AND (
	                              d.Title LIKE N'%' + @keyword + '%'
	                              OR d.DocumentNumber LIKE N'%' + @keyword + '%'
	                              OR d.SignedBy LIKE N'%' + @keyword + '%'
	                              OR d.RecipientsDivision LIKE N'%' + @keyword + '%'
	                          )
	           ) documentData
	    ORDER BY
	           documentData.DocumentDate DESC
	           OFFSET @start ROWS
	    
	    FETCH NEXT @limit ROWS ONLY;
	END
	ELSE
	BEGIN
	    IF @type = '2'
	    BEGIN
	        SET @total = (
	                SELECT DISTINCT COUNT(1)
	                FROM   (
	                           SELECT
	                                  d.Id
	                           FROM   [doc].DocumentDelivered d
	                                  JOIN [doc].DocumentRecipent r
	                                       ON  d.Id = r.DocumentId
	                           WHERE  d.Deleted = 0
	                                  AND d.Active = 1
	                                  AND d.DepartmentId = @departmentId
	                                  AND r.ForSending = 0
	                                  AND r.ReceivedDocument = 0
	                                  AND (
	                                          r.UserId = @userId
	                                          OR r.DepartmentId IN (SELECT VALUE
	                                                                FROM   fn_Split(@listSubDepartmentId, ','))
	                                          OR r.DepartmentId = @departmentId
	                                      )
	                                  AND d.DocumentDate BETWEEN @startDate AND @endDate
	                                  AND (
	                                          d.Title LIKE N'%' + @keyword + '%'
	                                          OR d.DocumentNumber LIKE N'%' + @keyword + '%'
	                                          OR d.SignedBy LIKE N'%' + @keyword + '%'
	                                          OR d.RecipientsDivision LIKE N'%' + @keyword + '%'
	                                      )
	                       ) documentData
	            );
	        
	        ---- Fetch Data ---
	        SELECT *
	        FROM   (
	                   SELECT DISTINCT 'HistoryId'= (SELECT dh.Id FROM doc.DocumentHistory AS dh WHERE dh.DocumentId=d.Id AND dh.UserId=@userId),
	                   'TaskId'=(SELECT TOP (1) td.TaskId
	                           FROM task.TaskDocuments AS td JOIN task.Task AS t ON td.TaskId=t.Id WHERE t.[Deleted]=0 AND t.[Active]=1 AND td.DocumentId=d.Id AND td.ReceivedDocument=0),
	                           'TaskCode'=(SELECT TOP (1) t.Code
	                           FROM task.TaskDocuments AS td JOIN task.Task AS t ON td.TaskId=t.Id WHERE t.[Deleted]=0 AND t.[Active]=1 AND td.DocumentId=d.Id AND td.ReceivedDocument=0),
	                          d.Id,d.Title,d.DocumentNumber,'ReceivedNumber'='',d.ExternalReceiveDivision as ExternalFromDivision ,d.SignedBy,documentDate,d.AttachmentName,d.AttachmentPath,ReceivedDocument=0,d.CreatedOn
	                   FROM   [doc].DocumentDelivered d
	                          JOIN [doc].DocumentRecipent r
	                               ON  d.Id = r.DocumentId
	                   WHERE  d.Deleted = 0
	                          AND d.Active = 1
	                          AND d.DepartmentId = @departmentId
	                          AND r.ForSending = 0
	                          AND r.ReceivedDocument = 0
	                          AND (
	                                  r.UserId = @userId
	                                  OR r.DepartmentId IN (SELECT VALUE
	                                                        FROM   fn_Split(@listSubDepartmentId, ','))
	                                  OR r.DepartmentId = @departmentId
	                              )
	                          AND d.DocumentDate BETWEEN @startDate AND @endDate
	                          AND (
	                                  d.Title LIKE N'%' + @keyword + '%'
	                                  OR d.DocumentNumber LIKE N'%' + @keyword + '%'
	                                  OR d.SignedBy LIKE N'%' + @keyword + '%'
	                                  OR d.RecipientsDivision LIKE N'%' + @keyword + '%'
	                              )
	               ) documentData
	        ORDER BY
	               documentData.DocumentDate DESC
	               OFFSET @start ROWS
	        
	        FETCH NEXT @limit ROWS ONLY;
	    END
	    ELSE
	    BEGIN
	        SET @total = (
	        -- start count item
	        SELECT Count(1) FROM (SELECT * FROM (
	                   SELECT DISTINCT
	                          d.Id,d.Title,d.DocumentNumber,d.ReceivedNumber,d.ExternalFromDivision,d.SignedBy,d.documentDate,d.AttachmentName,d.AttachmentPath,d.CreatedOn
	                   FROM   [doc].DocumentReceived d
	                          JOIN [doc].DocumentRecipent r
	                               ON  d.Id = r.DocumentId
	                         
	                   WHERE  d.Deleted = 0
	                          AND d.Active = 1
	                          AND d.DepartmentId = @departmentId
	                          AND r.AddedDocumentBook = 1
	                          AND r.ForSending = 0
	                          AND r.ReceivedDocument = 1
	                          AND (
	                                  r.UserId = @userId
	                                  OR r.DepartmentId IN (SELECT VALUE
	                                                        FROM   fn_Split(@listSubDepartmentId, ','))
	                                  OR r.DepartmentId = @departmentId
	                              )
	                          AND d.DocumentDate BETWEEN @startDate AND @endDate
	                          AND (
	                                  d.Title LIKE N'%' + @keyword + '%'
	                                  OR d.DocumentNumber LIKE N'%' + @keyword + '%'
	                                  OR d.SignedBy LIKE N'%' + @keyword + '%'
	                                  OR d.RecipientsDivision LIKE N'%' + @keyword + '%'
	                              )
	               ) receivedDocumentData
	        UNION
	        SELECT *
	        FROM   (
	                   SELECT DISTINCT
	                          d.Id,d.Title,d.DocumentNumber,'ReceivedNumber'='',d.ExternalReceiveDivision as ExternalFromDivision ,d.SignedBy,d.documentDate,d.AttachmentName,d.AttachmentPath,d.CreatedOn
	                   FROM   [doc].DocumentDelivered d
	                          JOIN [doc].DocumentRecipent r
	                               ON  d.Id = r.DocumentId
	                   WHERE  d.Deleted = 0
	                          AND d.Active = 1
	                          AND d.DepartmentId = @departmentId
	                          AND r.ForSending = 0
	                          AND r.ReceivedDocument = 0
	                          AND (
	                                  r.UserId = @userId
	                                  OR r.DepartmentId IN (SELECT VALUE
	                                                        FROM   fn_Split(@listSubDepartmentId, ','))
	                                  OR r.DepartmentId = @departmentId
	                              )
	                          AND d.DocumentDate BETWEEN @startDate AND @endDate
	                          AND (
	                                  d.Title LIKE N'%' + @keyword + '%'
	                                  OR d.DocumentNumber LIKE N'%' + @keyword + '%'
	                                  OR d.SignedBy LIKE N'%' + @keyword + '%'
	                                  OR d.RecipientsDivision LIKE N'%' + @keyword + '%'
	                              )
	               ) deliveredDocumentData
	        ) documentData
	        	--finish count item
	        );
	        SELECT DISTINCT documentData.* FROM (SELECT * FROM (
	                   SELECT 'HistoryId'= (SELECT dh.Id FROM doc.DocumentHistory AS dh WHERE dh.DocumentId=d.Id AND dh.UserId=@userId),
	                   'TaskId'=(SELECT TOP (1) td.TaskId
	                           FROM task.TaskDocuments AS td JOIN task.Task AS t ON td.TaskId=t.Id WHERE t.[Deleted]=0 AND t.[Active]=1 AND td.DocumentId=d.Id AND td.ReceivedDocument=1),
	                           'TaskCode'=(SELECT TOP (1) t.Code
	                           FROM task.TaskDocuments AS td JOIN task.Task AS t ON td.TaskId=t.Id WHERE t.[Deleted]=0 AND t.[Active]=1 AND td.DocumentId=d.Id AND td.ReceivedDocument=1),
	                          d.Id,d.Title,d.DocumentNumber,d.ReceivedNumber,d.ExternalFromDivision,d.SignedBy,d.documentDate,d.AttachmentName,d.AttachmentPath,ReceivedDocument=1,d.CreatedOn
	                   FROM   [doc].DocumentReceived d
	                          JOIN [doc].DocumentRecipent r
	                               ON  d.Id = r.DocumentId
	                   WHERE  d.Deleted = 0
	                          AND d.Active = 1
	                          AND d.DepartmentId = @departmentId
	                          AND r.AddedDocumentBook = 1
	                          AND r.ForSending = 0
	                          AND r.ReceivedDocument = 1
	                          AND (
	                                  r.UserId = @userId
	                                  OR r.DepartmentId IN (SELECT VALUE
	                                                        FROM   fn_Split(@listSubDepartmentId, ','))
	                                  OR r.DepartmentId = @departmentId
	                              )
	                          AND d.DocumentDate BETWEEN @startDate AND @endDate
	                          AND (
	                                  d.Title LIKE N'%' + @keyword + '%'
	                                  OR d.DocumentNumber LIKE N'%' + @keyword + '%'
	                                  OR d.SignedBy LIKE N'%' + @keyword + '%'
	                                  OR d.RecipientsDivision LIKE N'%' + @keyword + '%'
	                              )
	               ) receivedDocumentData
	        UNION
	        SELECT *
	        FROM   (
	                   SELECT 'HistoryId'= (SELECT dh.Id FROM doc.DocumentHistory AS dh WHERE dh.DocumentId=d.Id AND dh.UserId=@userId),
	                   'TaskId'=(SELECT TOP (1) td.TaskId
	                           FROM task.TaskDocuments AS td JOIN task.Task AS t ON td.TaskId=t.Id WHERE t.[Deleted]=0 AND t.[Active]=1 AND td.DocumentId=d.Id AND td.ReceivedDocument=0),
	                           'TaskCode'=(SELECT TOP (1) t.Code
	                           FROM task.TaskDocuments AS td JOIN task.Task AS t ON td.TaskId=t.Id WHERE t.[Deleted]=0 AND t.[Active]=1 AND td.DocumentId=d.Id AND td.ReceivedDocument=0),
	                          d.Id,d.Title,d.DocumentNumber,'ReceivedNumber'='',d.ExternalReceiveDivision as ExternalFromDivision ,d.SignedBy,d.documentDate,d.AttachmentName,d.AttachmentPath,ReceivedDocument=0,d.CreatedOn
	                   FROM   [doc].DocumentDelivered d
	                          JOIN [doc].DocumentRecipent r
	                               ON  d.Id = r.DocumentId
	                   WHERE  d.Deleted = 0
	                          AND d.Active = 1
	                          AND d.DepartmentId = @departmentId
	                          AND r.ForSending = 0
	                          AND r.ReceivedDocument = 0
	                          AND (
	                                  r.UserId = @userId
	                                  OR r.DepartmentId IN (SELECT VALUE
	                                                        FROM   fn_Split(@listSubDepartmentId, ','))
	                                  OR r.DepartmentId = @departmentId
	                              )
	                          AND d.DocumentDate BETWEEN @startDate AND @endDate
	                          AND (
	                                  d.Title LIKE N'%' + @keyword + '%'
	                                  OR d.DocumentNumber LIKE N'%' + @keyword + '%'
	                                  OR d.SignedBy LIKE N'%' + @keyword + '%'
	                                  OR d.RecipientsDivision LIKE N'%' + @keyword + '%'
	                              )
	               ) deliveredDocumentData
	        ) documentData
	        ORDER BY documentData.DocumentDate DESC
	          OFFSET @start ROWS
	        
	        FETCH NEXT @limit ROWS ONLY;
	    END
	END
	SET NOCOUNT OFF;
END;

Go
 
IF EXISTS(
       SELECT *
       FROM   sysobjects AS s
       WHERE  s.id = OBJECT_ID('[doc].[SPGetDocumentAdvance]')
   )
    DROP PROCEDURE [doc].[SPGetDocumentAdvance]
GO
CREATE PROCEDURE [doc].[SPGetDocumentAdvance]
	@DocumentReceived BIT,
	@DocumentDelivered BIT,
	@LegalDocument BIT,
	@Keyword NVARCHAR(4000),
	@DocumentDateStart DATETIME,
	@DocumentDateEnd DATETIME,
	@DocumentDateRDStart DATETIME,
	@DocumentDateRDEnd DATETIME,
	@DocumentSign NVARCHAR(4000),
	@DocumentField NVARCHAR(4000),
	@DocumentType NVARCHAR(4000),
	@DocumentSecretLevel NVARCHAR(4000),
	@DocumentUrgencyLevel NVARCHAR(4000),
	@DepartmentId INT,
	@UserId NVARCHAR(128),
	@ListSubDepartmentId NVARCHAR(MAX),
	@Start INT,
	@Limit INT,
	@Total INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	--SET @DocumentDelivered = 1
	--SET @DocumentReceived = 1
	--SET @userId = 'c8df8541-be47-4a62-b217-e4da7bf89f7a'
	--SET @listSubDepartmentId = ',28,'
	--SET @DepartmentId = 15
	--SET @Start = 0
	--SET @Limit = 10
	
	DECLARE @TbReturn AS TABLE (
				HistoryId INT,
	            Id INT,
	            Title NVARCHAR(4000),
	            DocumentNumber NVARCHAR(100),
	            ReceivedNumber NVARCHAR(100),
	            ExternalFromDivision NVARCHAR(1000),
	            SignedBy NVARCHAR(1000),
	            DocumentDate DATETIME,
	            AttachmentName NVARCHAR(1000),
	            AttachmentPath NVARCHAR(1000),
	            ReceivedDocument INT,
	            CreatedOn DATETIME,
	            OriginalSavingPlace NVARCHAR(256),
	            Note NVARCHAR(500),
	            NumberOfCopies INT
	        )
	DECLARE @SqlReceved NVARCHAR(MAX)
	DECLARE @SQLDelivered NVARCHAR(MAX)
	
		
	SET @SqlReceved = 'SELECT NULL,'
	SET @SqlReceved = @SqlReceved + ' dr.Id,'
	SET @SqlReceved = @SqlReceved + ' dr.Title,'
	SET @SqlReceved = @SqlReceved + ' dr.DocumentNumber, '
	SET @SqlReceved = @SqlReceved + ' dr.ReceivedNumber, '
	SET @SqlReceved = @SqlReceved + ' dr.ExternalFromDivision, '
	SET @SqlReceved = @SqlReceved + ' dr.SignedBy, '
	SET @SqlReceved = @SqlReceved + ' dr.DocumentDate, '
	SET @SqlReceved = @SqlReceved + ' dr.AttachmentName, '
	SET @SqlReceved = @SqlReceved + ' dr.AttachmentPath, '
	SET @SqlReceved = @SqlReceved + ' 1, '
	SET @SqlReceved = @SqlReceved + ' dr.CreatedOn, '
	SET @SqlReceved = @SqlReceved + ' dr.OriginalSavingPlace, '
	SET @SqlReceved = @SqlReceved + ' dr.Note, '
	SET @SqlReceved = @SqlReceved + ' dr.NumberOfCopies '
	SET @SqlReceved = @SqlReceved + ' FROM   doc.DocumentReceived       AS dr '
	SET @SqlReceved = @SqlReceved + ' JOIN doc.DocumentRecipent  AS dr2 '
	SET @SqlReceved = @SqlReceved + ' ON  dr.Id = dr2.DocumentId '
	SET @SqlReceved = @SqlReceved + ' WHERE  dr.[Deleted] = 0' 
	SET @SqlReceved = @SqlReceved + ' AND dr.[Active] = 1 '
	SET @SqlReceved = @SqlReceved + ' AND dr2.ForSending = 0 '
	SET @SqlReceved = @SqlReceved + ' AND dr2.AddedDocumentBook = 1 '
	SET @SqlReceved = @SqlReceved + ' AND dr2.ReceivedDocument = 1 '
	SET @SqlReceved = @SqlReceved + ' AND dr.DepartmentId =' + CONVERT(NVARCHAR(10), @DepartmentId)
	SET @SqlReceved = @SqlReceved + ' AND ( '
	SET @SqlReceved = @SqlReceved + ' dr2.UserId = ''' + @userId + ''''
	SET @SqlReceved = @SqlReceved + ' OR dr2.DepartmentId IN (SELECT VALUE '
	SET @SqlReceved = @SqlReceved + ' FROM   fn_Split(''' + @listSubDepartmentId + ''', '',''))'
	SET @SqlReceved = @SqlReceved + ' OR dr2.DepartmentId = ' + CONVERT(nvarchar(10), @departmentId)
	SET @SqlReceved = @SqlReceved + ' ) '
	
	IF ISNULL(@Keyword, '') <> ''
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '
		SET @SqlReceved = @SqlReceved + ' dr.Title LIKE N''%' + @keyword + '%'''
		SET @SqlReceved = @SqlReceved + ' OR dr.DocumentNumber LIKE N''%' + @keyword + '%'''
		SET @SqlReceved = @SqlReceved + ' OR dr.SignedBy LIKE N''%' + @keyword + '%'''
		SET @SqlReceved = @SqlReceved + ' OR dr.RecipientsDivision LIKE N''%' + @keyword + '%'''
		SET @SqlReceved = @SqlReceved + ' OR dr.ExternalFromDivision LIKE N''%' + @Keyword + '%'''
		SET @SqlReceved = @SqlReceved + ')'	
	END
	IF @DocumentDateStart IS NOT NULL
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '               
	    SET @SqlReceved = @SqlReceved + ' dbo.DateOnly(dr.DocumentDate) >= dbo.DateOnly(''' + CONVERT(NVARCHAR(10), @DocumentDateStart, 102)+ ''')'
		SET @SqlReceved = @SqlReceved + ')'		 
	END
	IF @DocumentDateEnd IS NOT NULL
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '               
	    SET @SqlReceved = @SqlReceved + ' dbo.DateOnly(dr.DocumentDate) <= dbo.DateOnly(''' + CONVERT(NVARCHAR(10), @DocumentDateEnd, 102)+ ''')'
		SET @SqlReceved = @SqlReceved + ')'		 
	END
	IF @DocumentDateRDStart IS NOT NULL
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '               
	    SET @SqlReceved = @SqlReceved + ' dbo.DateOnly(dr.ReceivedDate) >= dbo.DateOnly(''' + CONVERT(NVARCHAR(10), @DocumentDateRDStart, 102)+ ''')'
		SET @SqlReceved = @SqlReceved + ')'		 
	END		    
	IF @DocumentDateRDEnd IS NOT NULL
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '               
	    SET @SqlReceved = @SqlReceved + ' dbo.DateOnly(dr.ReceivedDate) <= dbo.DateOnly(''' + CONVERT(NVARCHAR(10), @DocumentDateRDEnd, 102)+ ''')'
		SET @SqlReceved = @SqlReceved + ')'		 
	END	  
	IF ISNULL(@DocumentField, '') <> ''
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '   
	    SET @SqlReceved = @SqlReceved + ' dr.Id IN (SELECT ddf.DocumentId '
		SET @SqlReceved = @SqlReceved + ' FROM   doc.DocumentDocumentFields AS ddf '
		SET @SqlReceved = @SqlReceved + ' WHERE  ddf.ReceivedDocument = 1 '
		SET @SqlReceved = @SqlReceved + ' AND ddf.DocumentFieldDepartmentId IN (SELECT fs.[value] '
		SET @SqlReceved = @SqlReceved + ' FROM   dbo.fn_Split(''' + @DocumentField +''', '','') AS fs))'
		SET @SqlReceved = @SqlReceved + ' )'
	END
	IF ISNULL(@DocumentType, '') <> ''		
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '
		SET @SqlReceved = @SqlReceved + ' dr.DocumentTypeId IN (SELECT fs.[value]'
	    SET @SqlReceved = @SqlReceved + ' FROM   dbo.fn_Split(''' + @DocumentType + ''', '','') AS fs)'
		SET @SqlReceved = @SqlReceved + ' )'
	END
	IF ISNULL(@DocumentSign, '') <> ''
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '
		SET @SqlReceved = @SqlReceved + ' dr.SignedBy IN (SELECT fs.[value]'
		SET @SqlReceved = @SqlReceved + ' FROM   dbo.fn_SplitText(''' + @DocumentSign + ''', '','') AS fs)'
		SET @SqlReceved = @SqlReceved + ' ) '
	END
	IF ISNULL(@DocumentUrgencyLevel, '') <> ''
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '
		SET @SqlReceved = @SqlReceved + ' dr.UrgencyLevel IN (SELECT fs.[value] '
		SET @SqlReceved = @SqlReceved + ' FROM   dbo.fn_Split(''' + @DocumentUrgencyLevel + ''', '','') AS fs)'
		SET @SqlReceved = @SqlReceved + ' )'
	END
	IF ISNULL(@DocumentSecretLevel, '') <> ''
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '
		SET @SqlReceved = @SqlReceved + ' dr.SecretLevel IN (SELECT fs.[value] '
		SET @SqlReceved = @SqlReceved + ' FROM   dbo.fn_Split(''' + @DocumentSecretLevel + ''', '','') AS fs)'
		SET @SqlReceved = @SqlReceved + ' )'
	END       
	IF ISNULL(@LegalDocument, '') <>  ''
	BEGIN
		SET @SqlReceved = @SqlReceved + ' AND ( '
		SET @SqlReceved = @SqlReceved + ' dr.LegalDocument = ' + convert(nvarchar(10), isnull(@LegalDocument,0))
		SET @SqlReceved = @SqlReceved + ' )'
	END
	    
	SET @SqlReceved = @SqlReceved + ' GROUP BY '
	SET @SqlReceved = @SqlReceved + ' dr.Id, '
	SET @SqlReceved = @SqlReceved + ' dr.Title,' 
	SET @SqlReceved = @SqlReceved + ' dr.DocumentNumber,'
	SET @SqlReceved = @SqlReceved + ' dr.ReceivedNumber,'
	SET @SqlReceved = @SqlReceved + ' dr.ExternalFromDivision,'
	SET @SqlReceved = @SqlReceved + ' dr.SignedBy, '
	SET @SqlReceved = @SqlReceved + ' dr.DocumentDate, '
	SET @SqlReceved = @SqlReceved + ' dr.AttachmentName, '
	SET @SqlReceved = @SqlReceved + ' dr.AttachmentPath, '
	SET @SqlReceved = @SqlReceved + ' dr.CreatedOn, '
	SET @SqlReceved = @SqlReceved + ' dr.OriginalSavingPlace, '
	SET @SqlReceved = @SqlReceved + ' dr.Note, '
	SET @SqlReceved = @SqlReceved + ' dr.NumberOfCopies'
	
	INSERT INTO @TbReturn
	EXEC(@SqlReceved)
	
	
    SET @SQLDelivered= ' SELECT NULL, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.Id, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.Title, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.DocumentNumber, '
	SET @SQLDelivered = @SQLDelivered +  ''''','
	SET @SQLDelivered = @SQLDelivered +  ' dr.ExternalReceiveDivision, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.SignedBy, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.DocumentDate, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.AttachmentName, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.AttachmentPath,'
	SET @SQLDelivered = @SQLDelivered +  ' 0, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.CreatedOn, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.OriginalSavingPlace, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.Note, '
	SET @SQLDelivered = @SQLDelivered +  ' dr.NumberOfCopies '
	SET @SQLDelivered = @SQLDelivered +  ' FROM   doc.DocumentDelivered      AS dr ' 
	SET @SQLDelivered = @SQLDelivered +  ' JOIN doc.DocumentRecipent  AS dr2 '
	SET @SQLDelivered = @SQLDelivered +  ' ON  dr.Id = dr2.DocumentId '
	SET @SQLDelivered = @SQLDelivered +  ' WHERE  dr.[Deleted] = 0'
	SET @SQLDelivered = @SQLDelivered +  ' AND dr.[Active] = 1 '
	SET @SQLDelivered = @SQLDelivered +  ' AND dr2.ForSending = 0 '
	SET @SQLDelivered = @SQLDelivered +  ' AND dr2.AddedDocumentBook = 1 '
	SET @SQLDelivered = @SQLDelivered +  ' AND dr2.ReceivedDocument = 0'
	SET @SQLDelivered = @SQLDelivered +  ' AND dr.DepartmentId = ' + CONVERT(NVARCHAR(10),  @DepartmentId)
	SET @SQLDelivered = @SQLDelivered +  ' AND ( '
	SET @SQLDelivered = @SQLDelivered +  ' dr2.UserId = ''' + @userId + '''' 
	SET @SQLDelivered = @SQLDelivered +  ' OR dr2.DepartmentId IN (SELECT VALUE '
	SET @SQLDelivered = @SQLDelivered +  ' FROM   fn_Split(''' + @listSubDepartmentId + ''', '',''))'
	SET @SQLDelivered = @SQLDelivered +  ' OR dr2.DepartmentId = ' + CONVERT(NVARCHAR(10),  @departmentId)
	SET @SQLDelivered = @SQLDelivered +  ' )'
	IF ISNULL(@Keyword, '' ) <> ''
	BEGIN
		SET @SQLDelivered = @SQLDelivered +  ' AND ('
	    SET @SQLDelivered = @SQLDelivered +  ' dr.Title LIKE N''%' + @keyword + '%'''
		SET @SQLDelivered = @SQLDelivered +  ' OR dr.DocumentNumber LIKE N''%' + @keyword + '%'''
		SET @SQLDelivered = @SQLDelivered +  ' OR dr.SignedBy LIKE N''%' + @keyword + '%'''
		SET @SQLDelivered = @SQLDelivered +  ' OR dr.RecipientsDivision LIKE N''%' + @keyword + '%'''
		SET @SQLDelivered = @SQLDelivered +  ' OR dr.ExternalReceiveDivision LIKE N''%' + @Keyword + '%'''
		SET @SQLDelivered = @SQLDelivered +  ' )'
	END
	IF @DocumentDateStart IS NOT NULL
	BEGIN
		SET @SQLDelivered = @SQLDelivered +  ' AND ( '
	    SET @SQLDelivered = @SQLDelivered +  ' dbo.DateOnly(dr.DocumentDate) >= dbo.DateOnly(''' +  CONVERT(NVARCHAR(10), @DocumentDateStart, 102)   + ''')'
		SET @SQLDelivered = @SQLDelivered +  ')'
	END 
	IF @DocumentDateEnd IS NOT NULL
	BEGIN
		SET @SQLDelivered = @SQLDelivered +  ' AND ( '
		SET @SQLDelivered = @SQLDelivered +  ' dbo.DateOnly(dr.DocumentDate) <= dbo.DateOnly(''' + CONVERT(NVARCHAR(10), @DocumentDateEnd, 102) + ''')'
	    SET @SQLDelivered = @SQLDelivered +  ' )'
	END   
	IF @DocumentDateRDStart IS NOT NULL
	BEGIN
		SET @SQLDelivered = @SQLDelivered +  ' AND ( '
		SET @SQLDelivered = @SQLDelivered +  ' dbo.DateOnly(dr.DeliveredDate) >= dbo.DateOnly(''' + CONVERT(NVARCHAR(10), @DocumentDateRDStart, 102) + ''')'
		SET @SQLDelivered = @SQLDelivered  + ' )' 
	END	      
	IF @DocumentDateRDEnd IS NOT NULL
	BEGIN
		SET @SQLDelivered = @SQLDelivered +  ' AND ( '
		SET @SQLDelivered = @SQLDelivered +  ' dbo.DateOnly(dr.DeliveredDate) <= dbo.DateOnly(''' + CONVERT(NVARCHAR(10), @DocumentDateRDEnd, 102) + ''')'
		SET @SQLDelivered = @SQLDelivered  + ' )'
	END
	IF ISNULL(@DocumentField, '') <> ''
	BEGIN
		SET @SQLDelivered = @SQLDelivered  + ' AND ( '
		SET @SQLDelivered = @SQLDelivered  + ' dr.Id IN (SELECT ddf.DocumentId '
		SET @SQLDelivered = @SQLDelivered  + ' FROM   doc.DocumentDocumentFields AS ddf '
		SET @SQLDelivered = @SQLDelivered  + ' WHERE  ddf.ReceivedDocument = 1 '
		SET @SQLDelivered = @SQLDelivered  + ' AND ddf.DocumentFieldDepartmentId IN (SELECT fs.[value] '
		SET @SQLDelivered = @SQLDelivered  + ' FROM   dbo.fn_Split(''' + @DocumentField + ''', '','') AS fs))'
		SET @SQLDelivered = @SQLDelivered  + ' )'
	END
	IF ISNULL(@DocumentType, '') <> ''
	BEGIN
		SET @SQLDelivered = @SQLDelivered  + ' AND ( '
		SET @SQLDelivered = @SQLDelivered  + ' dr.DocumentTypeId IN (SELECT fs.[value] '
		SET @SQLDelivered = @SQLDelivered  + ' FROM   dbo.fn_Split(''' + @DocumentType + ''', '','') AS fs) '
		SET @SQLDelivered = @SQLDelivered  + ' )'
	END
	IF ISNULL(@DocumentSignDelivered, '') <> ''
	BEGIN
		SET @SQLDelivered = @SQLDelivered  + ' AND (' 
		SET @SQLDelivered = @SQLDelivered  + ' dr.SignedById IN (SELECT fs.[value] '
		SET @SQLDelivered = @SQLDelivered  + ' FROM   dbo.fn_Split( '''  + @DocumentSignDelivered + ''', '','') AS fs)'		
	    SET @SQLDelivered = @SQLDelivered  + ' ) '
	END 
	IF ISNULL(@DocumentUrgencyLevel, '') <> ''
	BEGIN
		SET @SQLDelivered = @SQLDelivered  + ' AND ( '
		SET @SQLDelivered = @SQLDelivered  + ' dr.UrgencyLevel IN (SELECT fs.[value] '
		SET @SQLDelivered = @SQLDelivered  + ' FROM   dbo.fn_Split(''' + @DocumentUrgencyLevel + ''', '','') AS fs)'
		SET @SQLDelivered = @SQLDelivered  + ' )'
	END
	IF ISNULL(@DocumentSecretLevel, '') <> ''
	BEGIN
		SET @SQLDelivered = @SQLDelivered  + ' AND ( '
		SET @SQLDelivered = @SQLDelivered  + ' dr.SecretLevel IN (SELECT fs.[value] '
		SET @SQLDelivered = @SQLDelivered  + ' FROM   dbo.fn_Split(''' + @DocumentSecretLevel + ''', '','') AS fs) '
		SET @SQLDelivered = @SQLDelivered  + ' ) '
	END
	    
	SET @SQLDelivered = @SQLDelivered  + ' AND ( '
	SET @SQLDelivered = @SQLDelivered  + ' dr.LegalDocument = ' + CONVERT(NVARCHAR(10), ISNULL(@LegalDocument, 0)) 
	SET @SQLDelivered = @SQLDelivered  + ' )'
	
	SET @SQLDelivered = @SQLDelivered  + ' GROUP BY '
	SET @SQLDelivered = @SQLDelivered  + ' dr.Id,'
	SET @SQLDelivered = @SQLDelivered  + ' dr.Title,'
	SET @SQLDelivered = @SQLDelivered  + ' dr.DocumentNumber, '
	SET @SQLDelivered = @SQLDelivered  + ' dr.ExternalReceiveDivision,'
	SET @SQLDelivered = @SQLDelivered  + ' dr.SignedBy, '
	SET @SQLDelivered = @SQLDelivered  + ' dr.DocumentDate, '
	SET @SQLDelivered = @SQLDelivered  + ' dr.AttachmentName,'
	SET @SQLDelivered = @SQLDelivered  + ' dr.AttachmentPath,'
	SET @SQLDelivered = @SQLDelivered  + ' dr.CreatedOn,'
	SET @SQLDelivered = @SQLDelivered  + ' dr.OriginalSavingPlace, '
	SET @SQLDelivered = @SQLDelivered  + ' dr.Note,'
	SET @SQLDelivered = @SQLDelivered  + ' dr.NumberOfCopies'
	
	INSERT INTO @TbReturn
	EXEC(@SQLDelivered)
	
	UPDATE @TbReturn
	SET HistoryId = tmp.idhis
	FROM (SELECT dh.Id idhis, dh.DocumentId, dh.ReceivedDocument received
	      FROM doc.DocumentHistory AS dh WHERE dh.UserId = @UserId) AS tmp
	WHERE tmp.DocumentId = Id AND ReceivedDocument = tmp.received
	
	
	IF ( ISNULL(@DocumentReceived,0) = 1 AND ISNULL(@DocumentDelivered,0) = 0)
	BEGIN
	    SET @Total = (
	            SELECT COUNT(1)
	            FROM   @TbReturn
	            WHERE  ReceivedDocument = 1
	        )
	    
	    SELECT *
	    FROM   @TbReturn
	    WHERE  ReceivedDocument = 1
	    ORDER BY
	           DocumentDate
	           OFFSET @Start ROWS
	    
	    FETCH NEXT @Limit ROWS ONLY
	END
	ELSE 
	IF (ISNULL(@DocumentReceived,0) = 0 AND ISNULL(@DocumentDelivered,0) = 1)
	BEGIN
	    SET @Total = (
	            SELECT COUNT(1)
	            FROM   @TbReturn
	            WHERE  ReceivedDocument = 0
	        )
	    
	    SELECT *
	    FROM   @TbReturn
	    WHERE  ReceivedDocument = 0
	    ORDER BY
	           DocumentDate
	           OFFSET @Start ROWS
	    
	    FETCH NEXT @Limit ROWS ONLY
	END
	ELSE
	BEGIN
	    SET @Total = (
	            SELECT COUNT(1)
	            FROM   @TbReturn
	        )
	    
	    SELECT *
	    FROM   @TbReturn
	    ORDER BY
	           DocumentDate
	           OFFSET @Start ROWS
	    
	    FETCH NEXT @Limit ROWS ONLY
	END
	
	
	SET NOCOUNT OFF;
END

go

 IF EXISTS(SELECT * FROM sysobjects AS s WHERE s.id = OBJECT_ID('[doc].[SPGetDetailDocument]'))
 DROP PROCEDURE [doc].[SPGetDetailDocument]
GO
CREATE PROC [doc].[SPGetDetailDocument]
    @type NVARCHAR(50) ,
    @int INT
AS
    BEGIN
        IF ( @type = 'received' )
            BEGIN
                SELECT  dr.Id ,
                        dr.DocumentNumber ,
                        dr.ReceivedNumber ,
                        dr.Title ,
                        dr.DocumentDate ,
                        dr.ReceivedDate ,
                        'ReceivedDocument' = 1 ,
                        dr.DepartmentId ,
                        dr.ExternalFromDivision ,
                        dr.RecipientsDivision ,
                        dr.SignedBy ,
                        dr.DocumentBookAddedOn ,
                        dr.NumberOfCopies ,
                        dr.NumberOfPages ,
                        dr.Note ,
                        dr.LegalDocument ,
                        dr.AttachmentName ,
                        dr.AttachmentPath ,
                        dr.CreatedOn ,
                        dr.EditedOn ,
                        dr.OriginalSavingPlace ,
                        dt.Title AS 'docType' ,
                        au.UserName ,
                        st.FullName ,
                        'documentBookAdded' = ( SELECT  st1.FullName
                                                FROM    doc.DocumentReceived
                                                        AS dr1
                                                        INNER JOIN dbo.AspNetUsers
                                                        AS au1 ON dr1.DocumentBookAddedBy = au1.Id
                                                        LEFT JOIN organiz.Staff
                                                        AS st1 ON st1.UserId = au1.Id
                                                WHERE   dr1.Id = @int
                                                        AND dr1.Deleted = 0
                                                        AND dr1.Active = 1
                                                        AND au1.Deleted = 0
                                                        AND st1.Active = 1
                                                        AND st1.Deleted = 0
                                              ) ,
                        'editByText' = ( SELECT st1.FullName
                                         FROM   doc.DocumentReceived AS dr1
                                                INNER JOIN dbo.AspNetUsers AS au1 ON dr1.EditedBy = au1.Id
                                                LEFT JOIN organiz.Staff AS st1 ON st1.UserId = au1.Id
                                         WHERE  dr1.Id = @int
                                                AND dr1.Deleted = 0
                                                AND dr1.Active = 1
                                                AND au1.Deleted = 0
                                                AND st1.Active = 1
                                                AND st1.Deleted = 0
                                       ) ,
                        'secretLevelText' = CASE WHEN dr.SecretLevel = 0
                                                 THEN N'Thường'
                                                 WHEN dr.SecretLevel = 1
                                                 THEN N'Mật'
                                                 WHEN dr.SecretLevel = 2
                                                 THEN N'Tối mật'
                                                 ELSE N'Tuyệt mật'
                                            END ,
                        'urgencyLevelText' = CASE WHEN dr.SecretLevel = 0
                                                  THEN N'Thường'
                                                  WHEN dr.SecretLevel = 1
                                                  THEN N'Khẩn'
                                                  WHEN dr.SecretLevel = 2
                                                  THEN N'Thượng khẩn'
                                                  ELSE N'Hỏa tốc'
                                             END
                FROM    doc.DocumentReceived AS dr
                        INNER JOIN doc.DocumentType AS dt ON dt.Id = dr.DocumentTypeId
                        INNER JOIN dbo.AspNetUsers AS au ON dr.CreatedBy = au.Id
                        LEFT JOIN organiz.Staff AS st ON st.UserId = au.Id
                WHERE   dr.Id = @int
                        AND dr.Deleted = 0
                        AND dr.Active = 1
                        AND dt.Active = 1
                        AND dt.Deleted = 0
                        AND au.Deleted = 0
                        AND st.Active = 1
                        AND st.Deleted = 0;
            END;
        ELSE
            BEGIN
                SELECT  dr.Id ,
                        dr.DocumentNumber ,
                        'ReceivedNumber' = '' ,
                        dr.Title ,
                        dr.DocumentDate ,
                        'ReceivedDate' = GETDATE() ,
                        'ReceivedDocument' = 0 ,
                        dr.DepartmentId ,
                        'ExternalFromDivision' = dr.ExternalReceiveDivision,
                        dr.RecipientsDivision ,
                        dr.SignedBy ,
                        'DocumentBookAddedOn' = NULL ,
                        dr.NumberOfCopies ,
                        dr.NumberOfPages ,
                        dr.Note ,
                        dr.LegalDocument ,
                        dr.AttachmentName ,
                        dr.AttachmentPath ,
                        dr.CreatedOn ,
                        dr.EditedOn ,
                        dr.OriginalSavingPlace ,
                        dt.Title AS 'docType' ,
                        au.UserName ,
                        st.FullName ,
                        'documentBookAdded' = NULL ,
                        'editByText' = ( SELECT st1.FullName
                                         FROM   doc.DocumentDelivered AS dr1
                                                INNER JOIN dbo.AspNetUsers AS au1 ON dr1.EditedBy = au1.Id
                                                LEFT JOIN organiz.Staff AS st1 ON st1.UserId = au1.Id
                                         WHERE  dr1.Id = @int
                                                AND dr1.Deleted = 0
                                                AND dr1.Active = 1
                                                AND au1.Deleted = 0
                                                AND st1.Active = 1
                                                AND st1.Deleted = 0
                                       ) ,
                        'secretLevelText' = CASE WHEN dr.SecretLevel = 0
                                                 THEN N'Thường'
                                                 WHEN dr.SecretLevel = 1
                                                 THEN N'Mật'
                                                 WHEN dr.SecretLevel = 2
                                                 THEN N'Tối mật'
                                                 ELSE N'Tuyệt mật'
                                            END ,
                        'urgencyLevelText' = CASE WHEN dr.UrgencyLevel = 0
                                                  THEN N'Thường'
                                                  WHEN dr.UrgencyLevel = 1
                                                  THEN N'Khẩn'
                                                  WHEN dr.UrgencyLevel = 2
                                                  THEN N'Thượng khẩn'
                                                  ELSE N'Hỏa tốc'
                                             END
                FROM    doc.DocumentDelivered AS dr
                        INNER JOIN doc.DocumentType AS dt ON dt.Id = dr.DocumentTypeId
                        INNER JOIN dbo.AspNetUsers AS au ON dr.CreatedBy = au.Id
                        LEFT JOIN organiz.Staff AS st ON st.UserId = au.Id
                WHERE   dr.Id = @int
                        AND dr.Deleted = 0
                        AND dr.Active = 1
                        AND dt.Active = 1
                        AND dt.Deleted = 0
                        AND au.Deleted = 0
                        AND st.Active = 1
                        AND st.Deleted = 0;
            END;
    END;
   -- EXEC doc.SPGetDetailDocument @type = 'delivered', @int = 58;
go
 
IF EXISTS(
       SELECT *
       FROM   sysobjects AS s
       WHERE  s.id = OBJECT_ID('[doc].[SPGetTotalDocumentReport]')
   )
    DROP PROCEDURE [doc].[SPGetTotalDocumentReport]
GO
CREATE PROCEDURE [doc].[SPGetTotalDocumentReport]
	@fromDate DATETIME,
	@toDate DATETIME,
	@ListDepartmentId VARCHAR(MAX)
AS
BEGIN
	 
	
	SELECT tmp.DepartmentId,
	       tmp.Name,
	       SUM(tmp.TotalDocumentDelivered) TotalDocumentDelivered,
	       SUM(tmp.TotalDocumentReceived) TotalDocumentReceived,
	       SUM(tmp.AddedDocumentBook)        AddedDocumentBook,
	       SUM(tmp.NotAddedDocumentBook)     NotAddedDocumentBook
	FROM   (
	           SELECT dr.DepartmentId,
	                  d.Name,
	                  0            TotalDocumentDelivered,
	                  COUNT(1)     TotalDocumentReceived,
	                  0            AddedDocumentBook,
	                  0            NotAddedDocumentBook
	           FROM   doc.DocumentReceived AS dr
	                  JOIN organiz.Department AS d
	                       ON  d.Id = dr.DepartmentId
	           WHERE  dr.[Deleted] = 0
	                  AND dr.[Active] = 1
	                  AND dbo.DateOnly(dr.DocumentDate) BETWEEN dbo.DateOnly(@fromDate) AND dbo.DateOnly(@toDate)
	                  AND ( dr.DepartmentId IN (SELECT fs.[value]
	                                          FROM   dbo.fn_Split(@ListDepartmentId, ',') AS fs) 
	                                          OR ISNULL(@ListDepartmentId, '') ='')
	           GROUP BY
	                  dr.DepartmentId,
	                  d.Name
	           
	           UNION ALL
	           SELECT dd.DepartmentId,
	                  d.Name,
	                  COUNT(1)     TotalDocuemntDelivered,
	                  0            TotalDocumentReceived,
	                  0            AddedDocumentBook,
	                  0            NotAddedDocumentBook
	           FROM   doc.DocumentDelivered AS dd
	                  JOIN organiz.Department AS d
	                       ON  d.Id = dd.DepartmentId
	           WHERE  dd.[Deleted] = 0
	                  AND dd.[Active] = 1
	                  AND dbo.DateOnly(dd.DocumentDate) BETWEEN dbo.DateOnly(@fromDate) AND dbo.DateOnly(@toDate)
	                  AND ( dd.DepartmentId IN (SELECT fs.[value]
	                                          FROM   dbo.fn_Split(@ListDepartmentId, ',') AS fs) 
	                                          OR ISNULL(@ListDepartmentId, '') ='')
	           GROUP BY
	                  dd.DepartmentId,
	                  d.Name
	           UNION ALL
	           SELECT dr.DepartmentId,
	                  d.Name,
	                  0            TotalDocumentDelivered,
	                  0            TotalDocumentReceived,
	                  COUNT(1)     AddedDocumentBook,
	                  0            NotAddedDocumentBook
	           FROM   doc.DocumentReceived AS dr
	                  JOIN organiz.Department AS d
	                       ON  d.Id = dr.DepartmentId
	           WHERE  dr.[Deleted] = 0
	                  AND dr.[Active] = 1
	                  AND dr.AddedDocumentBook = 1
	                  AND dbo.DateOnly(dr.DocumentDate) BETWEEN dbo.DateOnly(@fromDate) AND dbo.DateOnly(@toDate)
	                  AND ( dr.DepartmentId IN (SELECT fs.[value]
	                                          FROM   dbo.fn_Split(@ListDepartmentId, ',') AS fs) 
	                                          OR ISNULL(@ListDepartmentId, '') ='')
	           GROUP BY
	                  dr.DepartmentId,
	                  d.Name
	           UNION ALL
	           SELECT dr.DepartmentId,
	                  d.Name,
	                  0            TotalDocumentDelivered,
	                  0            TotalDocumentReceived,
	                  0            AddedDocumentBook,
	                  COUNT(1)     NotAddedDocumentBook
	           FROM   doc.DocumentReceived AS dr
	                  JOIN organiz.Department AS d
	                       ON  d.Id = dr.DepartmentId
	           WHERE  dr.[Deleted] = 0
	                  AND dr.[Active] = 1
	                  AND dr.AddedDocumentBook = 0
	                  AND dbo.DateOnly(dr.DocumentDate) BETWEEN dbo.DateOnly(@fromDate) AND dbo.DateOnly(@toDate)
	                  AND ( dr.DepartmentId IN (SELECT fs.[value]
	                                          FROM   dbo.fn_Split(@ListDepartmentId, ',') AS fs) 
	                                          OR ISNULL(@ListDepartmentId, '') ='')
	           GROUP BY
	                  dr.DepartmentId,
	                  d.Name
	       )                              AS tmp
	GROUP BY
	       tmp.DepartmentId,
	       tmp.Name
END

GO
IF EXISTS(SELECT * FROM sysobjects AS s WHERE s.id = OBJECT_ID('[doc].[SPGetTotalDocumentReportList]'))
DROP PROCEDURE [doc].[SPGetTotalDocumentReportList]

GO

CREATE PROCEDURE [doc].[SPGetTotalDocumentReportList]
	@fromDate DATETIME,
	@toDate DATETIME,
	@ListDepartmentId VARCHAR(MAX),
	@start INT ,
	@limit INT ,
	@total INT OUTPUT
AS
BEGIN
	DECLARE @tbReturn AS TABLE (
	            departmentId INT,
	            [NAME] NVARCHAR(500),
	            TotalDocumentDelivered INT,
	            TotalDocumentReceived INT,
	            AddedDocumentBook INT,
	            NotAddedDocumentBook INT,
	            [ORDER] INT
	        )
	
	INSERT INTO @tbReturn
	SELECT tmp.DepartmentId,
	       tmp.Name,
	       SUM(tmp.TotalDocumentDelivered) TotalDocumentDelivered,
	       SUM(tmp.TotalDocumentReceived) TotalDocumentReceived,
	       SUM(tmp.AddedDocumentBook)        AddedDocumentBook,
	       SUM(tmp.NotAddedDocumentBook)     NotAddedDocumentBook,
	       0
	FROM   (
	           SELECT dr.DepartmentId,
	                  d.Name,
	                  0            TotalDocumentDelivered,
	                  COUNT(1)     TotalDocumentReceived,
	                  0            AddedDocumentBook,
	                  0            NotAddedDocumentBook
	           FROM   doc.DocumentReceived AS dr
	                  JOIN organiz.Department AS d
	                       ON  d.Id = dr.DepartmentId
	           WHERE  dr.[Deleted] = 0
	                  AND dr.[Active] = 1
	                  AND dbo.DateOnly(dr.DocumentDate) BETWEEN dbo.DateOnly(@fromDate) AND dbo.DateOnly(@toDate)
	                  AND (
	                          dr.DepartmentId IN (SELECT fs.[value]
	                                              FROM   dbo.fn_Split(@ListDepartmentId, ',') AS fs)
	                          OR ISNULL(@ListDepartmentId, '') = ''
	                      )
	           GROUP BY
	                  dr.DepartmentId,
	                  d.Name
	           
	           UNION ALL
	           SELECT dd.DepartmentId,
	                  d.Name,
	                  COUNT(1)     TotalDocuemntDelivered,
	                  0            TotalDocumentReceived,
	                  0            AddedDocumentBook,
	                  0            NotAddedDocumentBook
	           FROM   doc.DocumentDelivered AS dd
	                  JOIN organiz.Department AS d
	                       ON  d.Id = dd.DepartmentId
	           WHERE  dd.[Deleted] = 0
	                  AND dd.[Active] = 1
	                  AND dbo.DateOnly(dd.DocumentDate) BETWEEN dbo.DateOnly(@fromDate) AND dbo.DateOnly(@toDate)
	                  AND (
	                          dd.DepartmentId IN (SELECT fs.[value]
	                                              FROM   dbo.fn_Split(@ListDepartmentId, ',') AS fs)
	                          OR ISNULL(@ListDepartmentId, '') = ''
	                      )
	           GROUP BY
	                  dd.DepartmentId,
	                  d.Name
	           UNION ALL
	           SELECT dr.DepartmentId,
	                  d.Name,
	                  0            TotalDocumentDelivered,
	                  0            TotalDocumentReceived,
	                  COUNT(1)     AddedDocumentBook,
	                  0            NotAddedDocumentBook
	           FROM   doc.DocumentReceived AS dr
	                  JOIN organiz.Department AS d
	                       ON  d.Id = dr.DepartmentId
	           WHERE  dr.[Deleted] = 0
	                  AND dr.[Active] = 1
	                  AND dr.AddedDocumentBook = 1
	                  AND dbo.DateOnly(dr.DocumentDate) BETWEEN dbo.DateOnly(@fromDate) AND dbo.DateOnly(@toDate)
	                  AND (
	                          dr.DepartmentId IN (SELECT fs.[value]
	                                              FROM   dbo.fn_Split(@ListDepartmentId, ',') AS fs)
	                          OR ISNULL(@ListDepartmentId, '') = ''
	                      )
	           GROUP BY
	                  dr.DepartmentId,
	                  d.Name
	           UNION ALL
	           SELECT dr.DepartmentId,
	                  d.Name,
	                  0            TotalDocumentDelivered,
	                  0            TotalDocumentReceived,
	                  0            AddedDocumentBook,
	                  COUNT(1)     NotAddedDocumentBook
	           FROM   doc.DocumentReceived AS dr
	                  JOIN organiz.Department AS d
	                       ON  d.Id = dr.DepartmentId
	           WHERE  dr.[Deleted] = 0
	                  AND dr.[Active] = 1
	                  AND dr.AddedDocumentBook = 0
	                  AND dbo.DateOnly(dr.DocumentDate) BETWEEN dbo.DateOnly(@fromDate) AND dbo.DateOnly(@toDate)
	                  AND (
	                          dr.DepartmentId IN (SELECT fs.[value]
	                                              FROM   dbo.fn_Split(@ListDepartmentId, ',') AS fs)
	                          OR ISNULL(@ListDepartmentId, '') = ''
	                      )
	           GROUP BY
	                  dr.DepartmentId,
	                  d.Name
	       )                              AS tmp
	GROUP BY
	       tmp.DepartmentId,
	       tmp.Name
 
	UPDATE @tbReturn
	SET    [ORDER] = tmp.[Order]
	FROM   (
	           SELECT *
	           FROM   organiz.Department AS d
	       ) AS tmp
	WHERE  tmp.Id = departmentId
	
	SET @total = (
	        SELECT COUNT(1)
	        FROM   @tbReturn
	    )
	
	SELECT *
	FROM   @tbReturn
	ORDER BY
	       [ORDER]
	       OFFSET @start ROWS
	
	FETCH NEXT @limit ROWS ONLY;
END
GO

IF EXISTS(
       SELECT *
       FROM   sysobjects AS s
       WHERE  s.id = OBJECT_ID('[doc].[SPGetComplexCount]')
   )
    DROP PROCEDURE [doc].[SPGetComplexCount]
 GO
 CREATE PROCEDURE [doc].[SPGetComplexCount]
		@UserId NVARCHAR(128),
        @DepartmentId INT
		AS
BEGIN
	DECLARE @TbReturn AS TABLE (TotalEventNotify INT, TotalDocumentNotAddedBook INT)
	
	INSERT INTO @TbReturn
	SELECT COUNT(1)                     TotalEventNotify,
	       0
	FROM   cld.[Event]               AS e
	       JOIN cld.EventUserNotify  AS eun
	            ON  eun.EventId = e.Id
	WHERE  e.[Deleted] = 0
	       AND e.[Active] = 1
	       AND e.Accepted = 1
	       AND eun.UserId = @UserId
	       AND e.DepartmentId = @DepartmentId
	       AND e.OccurDate > GETDATE()
	UNION ALL
	SELECT 0,
	       COUNT(1)                 TotalDocumentNotAddedBook
	FROM   doc.DocumentReceived  AS dr
	WHERE  dr.AddedDocumentBook = 0
	       AND dr.[Deleted] = 0
	       AND dr.[Active] = 1
	       AND dr.DepartmentId = @DepartmentId
	
	SELECT SUM(TotalEventNotify) TotalEventNotify,
	       SUM(TotalDocumentNotAddedBook) TotalDocumentNotAddedBook
	FROM   @TbReturn
END

 
GO
IF  EXISTS(SELECT * FROM sysobjects AS s
WHERE s.Id = OBJECT_ID(N'[doc].[SPGetListDocumentFieldDepartment]'))
Drop PROCEDURE [doc].[SPGetListDocumentFieldDepartment]
Go

--exec  [doc].[SPGetListDocumentFieldDepartment] 15,'',0,1000,0
create PROCEDURE [doc].[SPGetListDocumentFieldDepartment]
    @departmentID INT ,
    @keyword NVARCHAR(500) ,
    @start INT ,
    @limit INT ,
    @total INT OUTPUT
AS
    BEGIN
        SET NOCOUNT ON;
        
        SET @total = ( SELECT   COUNT(1)
                       FROM     doc.DocumentField df
                                LEFT JOIN doc.DocumentFieldDepartment t ON t.FieldId = df.Id
                       WHERE    ( t.DepartmentId IS NULL
                                  OR t.DepartmentId = @departmentID
                                  OR @departmentID IS NULL
                                )
                                AND ( @keyword = ''
                                      OR t.Code LIKE N'%' + @keyword + '%'
                                      OR t.Title LIKE N'%' + @keyword + '%'
                                      OR df.Code LIKE N'%' + @keyword + '%'
                                      OR df.Title LIKE N'%' + @keyword + '%'
                                    )
                                and ( t.Deleted = 0
                                      OR t.Deleted IS NULL
                                    )
                                AND ( df.Deleted = 0
                                      OR df.Deleted IS NULL
                                    )
                     );
	---- L?y Data ---
        SELECT  df.Id AS 'DocumentFieldId' ,
                df.Title AS 'DocumentFieldTitle' ,
                df.Code AS 'DocumentFieldCode' ,
				df.AllowClientEdit,
                t.Id ,
                t.Code ,
                t.Title ,
                t.Active ,
                t.EditedOn
        FROM    doc.DocumentField df
                LEFT JOIN doc.DocumentFieldDepartment t ON t.FieldId = df.Id
        WHERE   ( t.DepartmentId IS NULL
                  OR t.DepartmentId = @departmentID
                  OR @departmentID IS NULL
                )
                AND ( @keyword = ''
                      OR t.Code LIKE N'%' + @keyword + '%'
                      OR t.Title LIKE N'%' + @keyword + '%'
                      OR df.Code LIKE N'%' + @keyword + '%'
                      OR df.Title LIKE N'%' + @keyword + '%'
                    )
                AND ( t.Deleted = 0
                      OR t.Deleted IS NULL
                    )
                AND ( df.Deleted = 0
                      OR df.Deleted IS NULL
                    )
        ORDER BY df.CreatedOn DESC
                OFFSET @start ROWS
	FETCH NEXT @limit ROWS ONLY;
        SET NOCOUNT OFF;
    END;

	go

 

IF EXISTS(
       SELECT *
       FROM   sysobjects AS s
       WHERE  s.id = OBJECT_ID('[doc].[SPGetDocumentDeliveredStatistics]')
   )
    DROP PROCEDURE [doc].[SPGetDocumentDeliveredStatistics]
 GO
CREATE PROCEDURE [doc].[SPGetDocumentDeliveredStatistics]
	@Keyword NVARCHAR(500),
	@StartDate DATETIME,
	@EndDate DATETIME,
	@DepartmentId INT,
	@ListSignById NVARCHAR(MAX),
	@ListDocumentFieldID NVARCHAR(MAX),
	@ListDocumentTypeId NVARCHAR(MAX),
	@Start INT,
	@Limit INT,
	@Total  INT OUTPUT
AS
BEGIN
	--SET @Start = 1
	--SET @Limit = 5
	--SET @StartDate = '1/1/2017'
	--SET @EndDate = '6/30/2017'
	--SET @DepartmentId = 15	
	DECLARE @TBReturn AS TABLE (
	            SignedById INT,
	            DocumentFieldId INT,
	            DocumentTypeId INT,
	            GroupTitle NVARCHAR(256),
	            DocumentNumber NVARCHAR(256),
	            Title NVARCHAR(256),
	            DocumentDate DATETIME,
	            CreateDate DATETIME,
	            DocumentField NVARCHAR(256),
	            DocumentType NVARCHAR(256),
	            DocumentSignBy NVARCHAR(256),
	            Position NVARCHAR(256),
	            DocumentId INT
	        )
	
	DECLARE @SqlString AS NVARCHAR(MAX)
	SET @SqlString = 'SELECT dd.SignedById, '
	SET @SqlString = @SqlString + ' ddf.DocumentFieldDepartmentId, '
	SET @SqlString = @SqlString + ' dd.DocumentTypeId, '
	SET @SqlString = @SqlString + ' dd.SignedBy, '
	SET @SqlString = @SqlString + ' dd.DocumentNumber, '
	SET @SqlString = @SqlString + ' dd.Title, '
	SET @SqlString = @SqlString + ' dd.DocumentDate, '
	SET @SqlString = @SqlString + ' dd.DeliveredDate,'
	SET @SqlString = @SqlString + ' dt.Title, '
	SET @SqlString = @SqlString + ' dd.SignedBy, '
	SET @SqlString = @SqlString + ' dfd.Title, '
	SET @SqlString = @SqlString + ' dsb.Position,'
	SET @SqlString = @SqlString + ' dd.Id '
	SET @SqlString = @SqlString + '	FROM   doc.DocumentDelivered       AS dd'
	SET @SqlString = @SqlString + ' LEFT JOIN doc.DocumentSignedBy AS dsb '
	SET @SqlString = @SqlString + ' ON  dd.SignedById = dsb.Id '
	SET @SqlString = @SqlString + ' LEFT JOIN doc.DocumentType  AS dt '
	SET @SqlString = @SqlString + ' ON  dd.DocumentTypeId = dt.Id '
	SET @SqlString = @SqlString + ' LEFT JOIN doc.DocumentDocumentFields AS ddf'
	SET @SqlString = @SqlString + ' ON  dd.Id = ddf.DocumentId '
	SET @SqlString = @SqlString + ' JOIN doc.DocumentFieldDepartment AS dfd '
	SET @SqlString = @SqlString + ' ON  ddf.DocumentFieldDepartmentId = dfd.Id '
	SET @SqlString = @SqlString + ' WHERE  dd.[Deleted] = 0 '
	SET @SqlString = @SqlString + ' AND dd.[Active] = 1 '
	SET @SqlString = @SqlString + ' AND ddf.ReceivedDocument = 0 '	
	SET @SqlString = @SqlString + ' AND dd.DepartmentId = ' + CONVERT(NVARCHAR(10), @DepartmentId)
	IF ISNULL(@Keyword, '') <> ''
	BEGIN
	    SET @SqlString = @SqlString + ' AND ( '	
	    SET @SqlString = @SqlString + ' dd.Title LIKE N''%' + ISNULL(@Keyword, '') + '%'''		
	    SET @SqlString = @SqlString + ' OR dd.SignedBy LIKE N''%' + ISNULL(@Keyword, '') + '%'''
	    SET @SqlString = @SqlString + ' OR dd.DocumentNumber LIKE N''%' + ISNULL(@Keyword, '') + '%'''
	    SET @SqlString = @SqlString + ' OR dd.ExternalReceiveDivision LIKE N''%' + ISNULL(@Keyword, '') + '%'''
	    SET @SqlString = @SqlString + ' OR dd.RecipientsDivision LIKE N''%' + ISNULL(@Keyword, '') + '%'''
	    SET @SqlString = @SqlString + ' OR dd.Note LIKE N''%' + ISNULL(@Keyword, '') + '%'''
	    SET @SqlString = @SqlString + ' OR dd.OriginalSavingPlace LIKE N''%' + ISNULL(@Keyword, '') + '%'''
	    SET @SqlString = @SqlString + ' ) '
	END	
	
	IF @StartDate IS NOT NULL
	BEGIN
	    SET @SqlString = @SqlString + ' AND ( '
	    SET @SqlString = @SqlString + ' dbo.DateOnly(dd.DocumentDate) >= dbo.DateOnly(''' + CONVERT(NVARCHAR(10), @StartDate, 102) 
	        + ''') '
	    
	    SET @SqlString = @SqlString + ')'
	END
	
	IF @EndDate IS NOT NULL
	BEGIN
	    SET @SqlString = @SqlString + ' AND ( '
	    SET @SqlString = @SqlString + ' dbo.DateOnly(dd.DocumentDate) <= dbo.DateOnly(''' + CONVERT(NVARCHAR(10), @EndDate, 102) 
	        + ''') '
	    
	    SET @SqlString = @SqlString + ')'
	END
	
	IF ISNULL(@ListSignById, '') <> ''
	BEGIN
	    SET @SqlString = @SqlString + ' AND ( '
	    SET @SqlString = @SqlString + ' dd.SignedById IN (SELECT fs.[value]'
	    SET @SqlString = @SqlString + ' FROM   dbo.fn_Split(''' + @ListSignById + ''', '','') AS fs)'
	    SET @SqlString = @SqlString + ' )'
	END
	
	IF ISNULL(@ListDocumentTypeId, '') <> ''
	BEGIN
	    SET @SqlString = @SqlString + ' AND ( '
	    SET @SqlString = @SqlString + ' dd.DocumentTypeId IN (SELECT fs.[value] '
	    SET @SqlString = @SqlString + ' FROM   dbo.fn_Split(''' + @ListDocumentTypeId + ''', '','') AS fs)'	
	    SET @SqlString = @SqlString + ' )	'
	END
	
	IF ISNULL(@ListDocumentFieldID, '') <> ''
	BEGIN
	    SET @SqlString = @SqlString + ' AND ('
	    SET @SqlString = @SqlString + ' ddf.DocumentFieldDepartmentId IN (SELECT fs.[value]'
	    SET @SqlString = @SqlString + ' FROM   dbo.fn_Split(''' + @ListDocumentFieldID + ''', '','') AS fs)'
	    SET @SqlString = @SqlString + ')'
	END
	
	INSERT INTO @TBReturn
	  (
	    SignedById,
	    DocumentFieldId,
	    DocumentTypeId,
	    GroupTitle,
	    DocumentNumber,
	    Title,
	    DocumentDate,
	    CreateDate,
	    DocumentType,
	    DocumentSignBy,
	    DocumentField,
	    Position,
	    DocumentId
	  )
	EXEC (@SqlString)
	
	SET @Total = (
	        SELECT COUNT(1)
	        FROM   @TBReturn
	    )
	
	SELECT *
	FROM   @TBReturn s
	ORDER BY
	       s.DocumentId DESC
	       OFFSET @start ROWS
	
	FETCH NEXT @limit ROWS ONLY;
END
go
IF EXISTS(SELECT * FROM sysobjects AS s WHERE s.id = OBJECT_ID('[dbo].[fn_SplitText]'))
DROP FUNCTION [dbo].[fn_SplitText]
GO
CREATE FUNCTION [dbo].[fn_SplitText](@text varchar(8000), @delimiter varchar(20) = ' ')
RETURNS @Strings TABLE
(   
  position int IDENTITY PRIMARY KEY,
  value NVARCHAR(256) 
)
AS
BEGIN
DECLARE @index int
SET @index = -1
WHILE (LEN(@text) > 0)
 BEGIN 
    SET @index = CHARINDEX(@delimiter , @text) 
    IF (@index = 0) AND (LEN(@text) > 0) 
      BEGIN  
        INSERT INTO @Strings VALUES (@text)
          BREAK 
      END 
    IF (@index > 1) 
      BEGIN  
        INSERT INTO @Strings VALUES (LEFT(@text, @index - 1))  
        SET @text = RIGHT(@text, (LEN(@text) - @index)) 
      END 
    ELSE
      SET @text = RIGHT(@text, (LEN(@text) - @index))
    END
  RETURN
END

go

IF EXISTS(SELECT * FROM sysobjects AS s WHERE s.id = OBJECT_ID('[share].[SPGetNotificationCenter]'))
DROP PROCEDURE [share].[SPGetNotificationCenter]
GO
--- exec [share].[SPGetNotificationCenter] 'g',0,10,0
CREATE PROCEDURE [share].[SPGetNotificationCenter]
	@userId INT,	
	@DeviceId INT,
	@start INT,
	@limit INT,
	@total INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	--- Count records ---
	
	SET @total = (
	        SELECT COUNT(1)
	        FROM   share.NotificationCenter s
	        WHERE  s.ReceivedUserId = @userId
	    );
	
	SELECT s2.FullName, s2.Avatar, s.*
	 FROM   share.NotificationCenter s
	 JOIN organiz.Staff AS s2 ON s.CreatedBy = s2.UserId
	        WHERE  s.ReceivedUserId = @userId
	        AND s.DeviceId = @DeviceId
	ORDER BY
	       s.CreatedOn  DESC
	       OFFSET @start ROWS
	FETCH NEXT @limit ROWS ONLY;
END



GO


IF EXISTS(SELECT * FROM sysobjects AS s WHERE s.id = OBJECT_ID('[share].[SPGetUserNotification]'))
DROP PROCEDURE [share].[SPGetUserNotification]
GO
--- exec [share].[SPGetNotificationCenter] 'g',0,10,0
CREATE PROCEDURE [share].[SPGetUserNotification]
	@userId NVARCHAR(128) 
AS
BEGIN
	SET NOCOUNT ON;
 	
	SELECT s.*
	 FROM   share.UserNotification AS s
	        WHERE  s.UserId= @userId
	 
END


GO


