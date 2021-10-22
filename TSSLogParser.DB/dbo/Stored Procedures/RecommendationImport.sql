CREATE PROCEDURE [dbo].[RecommendationImport]
	@ProviderName NVARCHAR(MAX),
	@LogName NVARCHAR(MAX),
	@TruncatedMessage NVARCHAR(MAX),
	@MsftDocsSearch NVARCHAR(MAX),
	@MsftDocsTopResult NVARCHAR(255),
	@WebSearch NVARCHAR(MAX),
	@WebTopResult NVARCHAR(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE dbo.EventLogs
	SET [MsftDocsSearch] = @MsftDocsSearch
      ,[MsftDocsTopResult] = @MsftDocsTopResult
      ,[WebSearch] = @WebSearch
      ,[WebTopResult] = @WebTopResult
	WHERE LEFT([Message], 
		IIF(CHARINDEX(':', [Message]) >= 50, 
			CHARINDEX(':', [Message]), 
			IIF(CHARINDEX('.', [Message]) >= 50, CHARINDEX('.', [Message]), 50))
		) = @TruncatedMessage
	and LogName = @LogName
	and ProviderName = @ProviderName
END