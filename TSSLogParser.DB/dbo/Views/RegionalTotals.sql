CREATE VIEW [dbo].[RegionalTotals]
AS 

SELECT 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage, 
	msgCnt.LevelDisplayName,
	machCnt.CountryCode,
	machCnt.Region,
	machCnt.MachineCount, 
	SUM(msgCnt.MessageCount) AS MessageCount
FROM RegionalMessageCounts AS msgCnt 
	INNER JOIN RegionalMachineCounts AS machCnt 
		ON msgCnt.LogName = machCnt.LogName 
		AND msgCnt.ProviderName = machCnt.ProviderName 
		AND msgCnt.TruncatedMessage = machCnt.TruncatedMessage 
		AND msgCnt.CountryCode = machCnt.CountryCode
		AND msgCnt.Region = machCnt.Region
WHERE (msgCnt.MessageCount > 1)
GROUP BY 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage, 
	msgCnt.LevelDisplayName,
	machCnt.CountryCode,
	machCnt.Region,
	machCnt.MachineCount