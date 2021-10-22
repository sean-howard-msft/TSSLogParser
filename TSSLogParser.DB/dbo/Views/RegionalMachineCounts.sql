

CREATE VIEW [dbo].[RegionalMachineCounts]
AS
SELECT 
	AppService,
	LogName, 
	ProviderName, 
	Id,
	TruncatedMessage,
	CountryCode,
	Region,
	InfraCode, 
	COUNT(*) AS MachineCount
FROM  (
	SELECT DISTINCT 
		AppService,
		LogName, 
		ProviderName, 
		Id,
		TruncatedMessage,
		MachineName,
		CountryCode,
		Region,
		InfraCode
    FROM dbo.[EventLogsFresh]
	) AS el
GROUP BY 
	AppService,
	LogName, 
	ProviderName, 
	Id,
	TruncatedMessage,
	CountryCode,
	Region,
	InfraCode