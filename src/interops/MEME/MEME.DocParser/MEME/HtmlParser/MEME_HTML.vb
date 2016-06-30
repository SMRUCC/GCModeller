Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic

Namespace DocumentFormat.MEME.HTML

    Public Class MEMEHtml

        Public Property Motifs As Motif()
        Public Property ObjectId As String

        ''' <summary>
        ''' 获取所有发现的位点信息
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMatchedSites() As Site()
            Dim List As List(Of Site) = New List(Of Site)
            For Each Motif In Motifs
                Call List.AddRange((From site In Motif.MatchedSites Select New Site(site).Copy(Motif)).ToArray)
            Next

            Return List.ToArray
        End Function
    End Class
End Namespace