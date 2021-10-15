

CREATE VIEW [dbo].[GlobalTotals]
AS
SELECT 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.LevelDisplayName, 
	msgCnt.TruncatedMessage, 
	machCnt.MachineCount,
	SUM(msgCnt.MessageCount) AS MessageCount,
	msgCnt.FullMessage,
	msgCnt.MsftDocsSearch,
    msgCnt.MsftDocsTopResult,
    msgCnt.WebSearch,
    msgCnt.WebTopResult
FROM GlobalMessageCounts AS msgCnt 
	INNER JOIN GlobalMachineCounts AS machCnt 
		ON msgCnt.LogName = machCnt.LogName 
		AND msgCnt.ProviderName = machCnt.ProviderName 
		AND msgCnt.TruncatedMessage = machCnt.TruncatedMessage 
GROUP BY 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage, 
	machCnt.MachineCount, 
	msgCnt.FullMessage,
	msgCnt.LevelDisplayName,
	msgCnt.MsftDocsSearch,
    msgCnt.MsftDocsTopResult,
    msgCnt.WebSearch,
    msgCnt.WebTopResult