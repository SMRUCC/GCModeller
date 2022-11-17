Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data

''' <summary>
''' KEGG API is a REST-style Application Programming Interface to the KEGG database resource.
''' </summary>
<Package("kegg_api")>
Public Module kegg_api

    <ExportAPI("listing")>
    Public Function listing(database As String, Optional [option] As String = Nothing) As Object
        Return KEGGApi.List(database, [option])
    End Function

    <ExportAPI("get")>
    Public Function [get](id As String) As String
        Return KEGGApi.GetObject(id)
    End Function
End Module
