
CREATE VIEW [dbo].[RegionalMachineCounts]
AS
SELECT 
	LogName, 
	ProviderName, 
	TruncatedMessage,
	CountryCode,
	Region, 
	COUNT(*) AS MachineCount
FROM  (
	SELECT DISTINCT 
		el.LogName, 
		el.ProviderName, 
		el.TruncatedMessage,
		machData.MachineName,
		machData.CountryCode,
		machData.Region
    FROM EventLogsClean el
		JOIN MachineMetaData machData
			ON el.MachineName = machData.MachineName
	) AS el
GROUP BY 
	LogName, 
	ProviderName, 
	TruncatedMessage,
	CountryCode,
	Region