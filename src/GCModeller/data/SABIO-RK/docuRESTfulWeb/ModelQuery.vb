Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Data.SABIORK.SBML.SBMLReaction)

Public Class ModelQuery : Inherits WebQuery(Of Dictionary(Of QueryFields, String))

    Public Sub New(<CallerMemberName>
                   Optional cache As String = Nothing,
                   Optional interval As Integer = -1,
                   Optional offline As Boolean = False)

        MyBase.New(
            url:=AddressOf CreateQueryURL,
            contextGuid:=AddressOf cacheGuid,
            parser:=AddressOf parseSBML,
            prefix:=Function(guid) guid.First,
            cache:=cache,
            interval:=interval,
            offline:=offline
        )
    End Sub

    Private Shared Function cacheGuid(q As Dictionary(Of QueryFields, String)) As String
        Return q.GetJson.MD5
    End Function

    Public Shared Function CreateQueryURL(q As Dictionary(Of QueryFields, String)) As String
        Dim searches As String() = q _
            .Select(Function(t)
                        Return $"{t.Key.Description}:""{t.Value}"""
                    End Function) _
            .ToArray
        Dim query As String = searches.JoinBy(" AND ").UrlEncode
        Dim url As String = $"http://sabiork.h-its.org/sabioRestWebServices/searchKineticLaws/sbml?q={query}"

        Return url
    End Function

    Public Shared Function parseSBML(xml As String, schema As Type) As Object
        Return xml.LoadFromXml(Of sbXML)(throwEx:=False)
    End Function
End Class
