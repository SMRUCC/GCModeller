Namespace Java

    ''' <summary>
    ''' *********************************
    ''' **********   RNA CLASS   **********
    ''' </summary>
    Friend Class RNA

        Public start As Integer
        Public [stop] As Integer
        Public strand As Char

        Public Sub New(start As Integer, [stop] As Integer, strand As Char)
            Me.start = start
            Me.[stop] = [stop]
            Me.strand = strand
        End Sub

        Public Overridable Overloads Function ToString() As String
            Return start & vbTab & [stop] & vbTab & strand
        End Function
    End Class

End Namespace