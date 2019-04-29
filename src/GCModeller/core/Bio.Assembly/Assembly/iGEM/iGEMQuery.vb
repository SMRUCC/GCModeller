Imports Microsoft.VisualBasic.Text.Xml
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.iGEM

    Public Class iGEMQuery : Inherits WebQuery(Of String)

        Sub New(cache As String)
            Call MyBase.New(AddressOf urlCreator, Function(id) id, AddressOf partListParser, cache:=cache)
        End Sub

        Private Shared Function partListParser(text As String, type As Type) As Object
            Return text.RemoveXmlComments.LoadFromXml(type,)
        End Function

        Private Shared Function urlCreator(partId As String) As String
            Return $"http://parts.igem.org/cgi/xml/part.cgi?part={partId}"
        End Function

        Public Function FetchByIDList(id As IEnumerable(Of String)) As IEnumerable(Of String)
            Return Me.queryText(id, "Xml")
        End Function
    End Class
End Namespace