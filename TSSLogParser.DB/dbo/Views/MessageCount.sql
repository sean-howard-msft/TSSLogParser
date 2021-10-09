
CREATE VIEW [dbo].[MessageCount]
AS
SELECT 
	LogName, 
	ProviderName, 
	TruncatedMessage, 
	MachineName, 
	COUNT(*) AS MessageCount
FROM EventLogsClean
GROUP BY 
	LogName, 
	ProviderName, 
	MachineName, 
	TruncatedMessage
HAVING (COUNT(*) > 1)
