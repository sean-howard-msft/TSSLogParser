
CREATE VIEW [dbo].[GlobalMessageCounts]
AS
SELECT 
	LogName, 
	ProviderName, 
	TruncatedMessage,
	LevelDisplayName,
	COUNT(*) AS MessageCount
FROM EventLogsClean
GROUP BY 
	LogName, 
	ProviderName,
	TruncatedMessage,
	LevelDisplayName
HAVING (COUNT(*) > 1)
