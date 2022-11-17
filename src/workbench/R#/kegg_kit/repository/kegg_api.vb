Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data

''' <summary>
''' KEGG API is a REST-style Application Programming Interface to the KEGG database resource.
''' </summary>
<Package("kegg_api")>
Public Module kegg_api

    <ExportAPI("listing")>
    Public Function listing(database As String,
                            Optional [option] As String = Nothing,
                            Optional cache As IHttpGet = Nothing) As Object

        Return New KEGGApi(proxy:=cache).List(database, [option])
    End Function

    <ExportAPI("get")>
    Public Function [get](id As String, Optional cache As IHttpGet = Nothing) As String
        Return New KEGGApi(proxy:=cache).GetObject(id)
    End Function
End Module
