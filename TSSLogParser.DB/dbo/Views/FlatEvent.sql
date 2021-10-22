CREATE VIEW [dbo].[FlatEvent]
AS
SELECT e.Event_Id,
       ed.EventData_Id,
       ed.Binary,
       s.System_Id,
       s.EventID,
       s.Level,
       s.Task,
       s.Keywords,
       s.EventRecordID,
       s.Channel,
       s.Computer,
       s.Security,
       p.Name,
       t.SystemTime,
       REPLACE(SUBSTRING(
               (
                   SELECT ',' + d.[Data]
                   FROM dbo.[Data] d
                   WHERE (d.EventData_Id = ed.EventData_Id)
                   FOR XML PATH('')
               ),
               2,
               200000
                        ),
               ',',
               CHAR(13) + CHAR(10)
              ) AS [Message]
FROM dbo.[Event] e
    INNER JOIN dbo.[EventData] ed
        ON e.Event_Id = ed.Event_Id
    INNER JOIN dbo.[System] s
        ON s.Event_Id = e.Event_Id
    INNER JOIN dbo.[Provider] p
        ON p.System_Id = s.System_Id
    INNER JOIN dbo.TimeCreated t
        ON t.System_Id = s.System_Id;