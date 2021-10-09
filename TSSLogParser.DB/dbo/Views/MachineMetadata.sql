
CREATE VIEW [dbo].[MachineMetadata]
AS
SELECT DISTINCT 
         LEFT(MachineName, 2) AS CountryCode, 
		 SUBSTRING(MachineName, 3, 3) AS Region, 
		 SUBSTRING(MachineName, 6, 3) AS MachineType, 
		 SUBSTRING(MachineName, 9, 3) AS AppCode, 
		 SUBSTRING(MachineName, 12, 2) AS InfraCode, 
		 SUBSTRING(MachineName, 14, 2) AS InstanceNum, 
		 SUBSTRING(MachineName, 
		 CHARINDEX('.', MachineName, 0) + 1, LEN(MachineName)) AS Domain, 
         MachineName
FROM  dbo.EventLogs
