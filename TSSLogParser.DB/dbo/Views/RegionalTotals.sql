CREATE VIEW [dbo].[RegionalTotals]
AS 

SELECT 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage, 
	msgCnt.LevelDisplayName,
	msgCnt.CountryCode,
	msgCnt.Region,
	msgCnt.AppCode,
	msgCnt.InfraCode,
	machCnt.MachineCount,
	msgCnt.MessageCount,
	msgCnt.FullMessage,
	msgCnt.MsftDocsSearch,
    msgCnt.MsftDocsTopResult,
    msgCnt.WebSearch,
    msgCnt.WebTopResult
FROM RegionalMessageCounts AS msgCnt 
	INNER JOIN RegionalMachineCounts AS machCnt 
		ON msgCnt.LogName = machCnt.LogName 
		AND msgCnt.ProviderName = machCnt.ProviderName 
		AND msgCnt.TruncatedMessage = machCnt.TruncatedMessage 
		AND msgCnt.CountryCode = machCnt.CountryCode
		AND msgCnt.Region = machCnt.Region
		AND msgCnt.InfraCode = machCnt.InfraCode
WHERE (msgCnt.MessageCount > 1)