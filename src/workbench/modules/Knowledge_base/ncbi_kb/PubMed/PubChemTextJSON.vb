
Imports Microsoft.VisualBasic.Serialization.JSON

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
            Return json.SolveStream.LoadJSON(Of PubChemTextJSON())
        End Function

    End Class
End Namespace