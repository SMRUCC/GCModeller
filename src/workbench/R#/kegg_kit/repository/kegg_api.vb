Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.Data

''' <summary>
''' KEGG API is a REST-style Application Programming Interface to the KEGG database resource.
''' </summary>
<Package("kegg_api")>
Public Module kegg_api

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("listing")>
    Public Function listing(database As String,
                            Optional [option] As String = Nothing,
                            Optional cache As IHttpGet = Nothing) As Object

        Return New KEGGApi(proxy:=cache).List(database, [option])
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("get")>
    Public Function [get](id As String, Optional cache As IHttpGet = Nothing) As String
        Return New KEGGApi(proxy:=cache).GetObject(id)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("parseForm")>
    Public Function parseWebForm(text As String) As WebForm
        Return New WebForm(text)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("as.pathway")>
    Public Function convertToPathway(form As WebForm) As Pathway
        Return PathwayTextParser.ParsePathway(form)
    End Function
End Module
