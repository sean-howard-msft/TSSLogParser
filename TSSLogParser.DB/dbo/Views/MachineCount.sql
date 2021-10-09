
CREATE VIEW [dbo].[MachineCount]
AS
SELECT 
	LogName, 
	ProviderName, 
	TruncatedMessage, 
	COUNT(*) AS MachineCount
FROM  (
	SELECT DISTINCT 
		LogName, 
		ProviderName, 
		TruncatedMessage, 
		MachineName
    FROM EventLogsClean
	) AS el
GROUP BY 
	LogName, 
	ProviderName, 
	TruncatedMessage
