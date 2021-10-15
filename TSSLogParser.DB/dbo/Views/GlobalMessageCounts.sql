
CREATE VIEW [dbo].[GlobalMessageCounts]
AS
SELECT 
	LogName, 
	ProviderName, 
	LevelDisplayName,
	TruncatedMessage,	
	MAX([Message]) FullMessage,
	MAX(MsftDocsSearch) MsftDocsSearch, 
	MAX(MsftDocsTopResult) MsftDocsTopResult, 
	MAX(WebSearch) WebSearch, 
	MAX(WebTopResult) WebTopResult,
	COUNT(*) AS MessageCount
FROM EventLogsClean
GROUP BY 
	LogName, 
	ProviderName,
	TruncatedMessage,
	LevelDisplayName