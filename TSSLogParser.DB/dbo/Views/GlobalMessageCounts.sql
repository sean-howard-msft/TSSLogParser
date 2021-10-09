
CREATE VIEW [dbo].[GlobalMessageCounts]
AS
SELECT 
	LogName, 
	ProviderName, 
	TruncatedMessage,
	COUNT(*) AS MessageCount
FROM EventLogsClean
GROUP BY 
	LogName, 
	ProviderName,
	TruncatedMessage
HAVING (COUNT(*) > 1)
