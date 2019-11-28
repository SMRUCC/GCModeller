
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON

<Package("JSON", Category:=APICategories.UtilityTools)>
Module JSON

    ''' <summary>
    ''' Load R# list object from a given json file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("load.list")>
    Public Function LoadObjectList(file As String) As Dictionary(Of String, String)
        Return file.LoadJsonFile(Of Dictionary(Of String, String))
    End Function
End Module
