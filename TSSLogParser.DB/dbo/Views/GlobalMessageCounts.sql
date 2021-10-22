
CREATE VIEW [dbo].[GlobalMessageCounts]
AS
SELECT 
	AppService,
	LogName, 
	ProviderName, 
	Id,
	LevelDisplayName,
	TruncatedMessage,	
	MAX([Message]) FullMessage,
	MAX(MsftDocsSearch) MsftDocsSearch, 
	MAX(MsftDocsTopResult) MsftDocsTopResult, 
	MAX(WebSearch) WebSearch, 
	MAX(WebTopResult) WebTopResult,
	COUNT(*) AS MessageCount
FROM dbo.EventLogsClean
GROUP BY 
	AppService,
	LogName, 
	ProviderName,
	Id,
	TruncatedMessage,
	LevelDisplayName