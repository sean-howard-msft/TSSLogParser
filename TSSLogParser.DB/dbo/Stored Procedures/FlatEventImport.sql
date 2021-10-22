CREATE PROCEDURE [dbo].[FlatEventImport]
	@AppServiceName NVARCHAR(255)
AS
BEGIN
INSERT INTO [dbo].[EventLogs]
           ([RecordId]
           ,[MachineName]
           ,[LogName]
           ,[TimeCreated]
           ,[LevelDisplayName]
           ,[Level]
           ,[Id]
           ,[ProviderName]
           ,[Message]
           ,AppService
		   ,ContainerLog)
SELECT 
    e.Event_Id,
    e.Computer,
    e.Channel,
    e.SystemTime,
    CASE WHEN e.Level = 1 THEN 'Critical' 
    WHEN e.Level = 2 THEN 'Error' 
    WHEN e.Level = 3 THEN 'Warning' 
    WHEN e.Level = 4 THEN 'Information' 
    ELSE '' END AS LevelDisplayName,
    e.[Level],
    e.EventID,
    e.[Name],
    MAX(e.[Message]),
    @AppServiceName,
	''
FROM dbo.FlatEvent e
    LEFT JOIN dbo.EventLogs el
        ON e.Event_Id = el.[RecordId]
        AND e.Computer = el.[MachineName]
        AND e.Channel = el.[LogName]
WHERE el.[RecordId] IS NULL
AND e.Level IN (1, 2)
AND e.SystemTime > '2021-09-01'
GROUP BY 
    e.Event_Id,
    e.Computer,
    e.Channel,
    e.SystemTime,
    CASE WHEN e.Level = 1 THEN 'Critical' 
    WHEN e.Level = 2 THEN 'Error' 
    WHEN e.Level = 3 THEN 'Warning' 
    WHEN e.Level = 4 THEN 'Information' 
    ELSE '' END,
    e.[Level],
    e.EventID,
    e.[Name],
    e.[Message];

DELETE FROM dbo.[Event] WHERE 1=1;
DELETE FROM dbo.[EventData] WHERE 1=1;
DELETE FROM dbo.[Data] WHERE 1=1;
DELETE FROM dbo.[System] WHERE 1=1;
DELETE FROM dbo.[Provider] WHERE 1=1;
DELETE FROM dbo.[TimeCreated] WHERE 1=1;

END 