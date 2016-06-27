Imports System.Xml.Serialization

Namespace Assembly.NCBI.GenBank.TabularFormat

    ''' <summary>
    ''' sequence-region  (##sequence-region &lt;seqname> &lt;start> &lt;end>)
    ''' To indicate that this file only contains entries for the specified subregion of a sequence.
    ''' 
    ''' [##sequence-region CP000050.1 1 5148708]
    ''' </summary>
    Public Class SeqRegion
        <XmlAttribute> Public Property AccessId As String
        <XmlAttribute> Public Property Start As Integer
        <XmlAttribute> Public Property Ends As Integer

        Public Shared Function Parser(s As String) As SeqRegion
            If String.IsNullOrWhiteSpace(s) Then
                Return New SeqRegion
            End If

            Dim Tokens As String() = s.Split
            Dim acc As String = Tokens(Scan0)
            Dim start As Integer = CInt(Val(Tokens(1)))
            Dim ends As Integer = CInt(Val(Tokens(2)))

            Return New SeqRegion With {
                .AccessId = acc,
                .Start = start,
                .Ends = ends
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{AccessId} {Start} {Ends}"
        End Function
    End Class
End Namespace