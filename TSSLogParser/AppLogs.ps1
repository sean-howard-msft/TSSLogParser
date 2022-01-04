Param (
    [Parameter(Mandatory=$true)][string] $eventLog,        
    [Parameter(Mandatory=$true)][string] $exportCSV
)
Process {           
    Get-WinEvent -Path "$eventLog" -FilterXPath "*[System[(Level=1 or Level=2) and TimeCreated[@SystemTime>='2021-09-01T14:50:33.000Z']]]" | 
    select RecordId,MachineName,LogName,ProviderName,TimeCreated,LevelDisplayName,Level,ID,Message,ContainerLog |  
    Export-Csv "$exportCSV" -NoTypeInformation
}