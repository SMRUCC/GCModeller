Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace ComponentModel.Loci

    ''' <summary>
    ''' Custom parser for csv field
    ''' </summary>
    Public Class NucleotideLocationParser
        Implements IParser

        Public Overloads Function ToString(obj As Object) As String Implements IParser.ToString
            If obj Is Nothing Then
                Return ""
            Else
                Return DirectCast(obj, NucleotideLocation).ToString
            End If
        End Function

        Public Function TryParse(cell As String) As Object Implements IParser.TryParse
            Return NucleotideLocation.Parse(cell)
        End Function
    End Class
End Namespace