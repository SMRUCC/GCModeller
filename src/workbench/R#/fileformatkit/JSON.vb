
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON

<Package("JSON", Category:=APICategories.UtilityTools)>
Module JSON

    <ExportAPI("load.list")>
    Public Function LoadObjectList(file As String) As Dictionary(Of String, String)
        Return file.LoadJsonFile(Of Dictionary(Of String, String))
    End Function
End Module
