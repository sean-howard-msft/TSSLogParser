

CREATE VIEW [dbo].[RegionalMachineCounts]
AS
SELECT 
	LogName, 
	ProviderName, 
	TruncatedMessage,
	CountryCode,
	Region,
	InfraCode, 
	COUNT(*) AS MachineCount
FROM  (
	SELECT DISTINCT 
		LogName, 
		ProviderName, 
		TruncatedMessage,
		MachineName,
		CountryCode,
		Region,
		InfraCode
    FROM [EventLogsFresh]
	) AS el
GROUP BY 
	LogName, 
	ProviderName, 
	TruncatedMessage,
	CountryCode,
	Region,
	InfraCode