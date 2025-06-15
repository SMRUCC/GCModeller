
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Namespace PubMed

    ''' <summary>
    ''' the json export of the pubchem literal search result
    ''' </summary>
    Public Class PubChemTextJSON

        Public Property pmid As String

        Public Property meshheadings As String()
        Public Property meshsubheadings As String()
        Public Property meshcodes As String()
        Public Property cids As UInteger()

        Public Property articletitle As String
        Public Property articleabstract As String
        Public Property articlejourname As String
        ''' <summary>
        ''' authors names
        ''' </summary>
        ''' <returns></returns>
        Public Property articleauth As String
        Public Property articleaffil As String
        Public Property citation As String
        Public Property doi As String

        Public Overrides Function ToString() As String
            Return articletitle
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="json">
        ''' could be a json file its file path or the json text content data.
        ''' </param>
        ''' <returns></returns>
        Public Shared Function ParseJSON(json As String) As PubChemTextJSON()
            Dim list As JsonArray = JSONTextParser.ParseJson(json.SolveStream, False)
            Dim articles As PubChemTextJSON() = list _
                .AsObjects _
                .Select(Function(j)
                            Return j.CreateObject(Of PubChemTextJSON)(True)
                        End Function) _
                .ToArray

            Return articles
        End Function

    End Class
End Namespace