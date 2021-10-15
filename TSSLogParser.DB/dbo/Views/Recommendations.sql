CREATE VIEW [dbo].[Recommendations]
	AS 
SELECT TOP 100
	LogName, 
	ProviderName, 
	LevelDisplayName,
	MessageCount,	
	FullMessage,
	MsftDocsSearch, 
	MsftDocsTopResult, 
	WebSearch, 
	WebTopResult,
	REPLACE(SUBSTRING(
		(SELECT ',' + el2.MachineName
		FROM EventLogs el2
		WHERE (mc.MsftDocsTopResult = el2.MsftDocsTopResult
		or mc.WebTopResult = el2.WebTopResult)
	    AND mc.LogName = el2.LogName 
		AND mc.ProviderName = el2.ProviderName
		GROUP BY el2.MachineName
		ORDER BY el2.MachineName
		FOR XML PATH('')),2,200000), ',', CHAR(13) + CHAR(10)) AS Machines
FROM GlobalMessageCounts mc
WHERE (MsftDocsTopResult IS NOT NULL) OR
      (WebTopResult IS NOT NULL)