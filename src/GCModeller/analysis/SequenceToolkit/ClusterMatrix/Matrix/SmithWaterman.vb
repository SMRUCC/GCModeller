Imports System.Collections.Specialized
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module Matrix

    <Extension>
    Public Function SimilarityMatrix(source As FastaFile,
                                     Optional args As NameValueCollection = Nothing,
                                     Optional method As MatrixMethods = MatrixMethods.NeedlemanWunsch,
                                     Optional ByRef out As StreamWriter = Nothing) As DataSet()
        If args Is Nothing Then
            args = New NameValueCollection
        End If

        Select Case method
            Case MatrixMethods.NeedlemanWunsch
                Return source.NeedlemanWunsch(out)
            Case Else
                Return source.NeedlemanWunsch(out)
        End Select
    End Function
End Module