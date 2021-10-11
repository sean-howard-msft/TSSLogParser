Param (
    [Parameter(Mandatory=$true)][string] $eventLog,        
    [Parameter(Mandatory=$true)][string] $exportCSV
)
Process {           
    Get-WinEvent -Path $eventLog -FilterXPath "*[System[(Level=1 or Level=2 or Level=3)]]" | 
    select RecordId,MachineName,LogName,ProviderName,TimeCreated,LevelDisplayName,Level,ID,Message,ContainerLog |  
    Export-Csv $exportCSV -NoTypeInformation
}