Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Friend NotInheritable Class GeneAlignmentResult

    Public Property First As DataFrameRow()
    Public Property Second As DataFrameRow()
    Public Property DroppedFromFirst As Integer
    Public Property DroppedFromSecond As Integer
End Class

Friend Module GeneAlignment

    Public Function Align(first As DataFrameRow(), second As DataFrameRow()) As GeneAlignmentResult
        Dim firstLookup As Dictionary(Of String, DataFrameRow) = ValidateAndIndex(first, NameOf(first))
        Dim secondLookup As Dictionary(Of String, DataFrameRow) = ValidateAndIndex(second, NameOf(second))
        Dim alignedFirst As New System.Collections.Generic.List(Of DataFrameRow)
        Dim alignedSecond As New System.Collections.Generic.List(Of DataFrameRow)

        For Each row As DataFrameRow In first
            Dim partner As DataFrameRow = Nothing

            If secondLookup.TryGetValue(row.geneID, partner) Then
                alignedFirst.Add(firstLookup(row.geneID))
                alignedSecond.Add(partner)
            End If
        Next

        If alignedFirst.Count = 0 Then
            Throw New ArgumentException("The two expression matrices do not share any gene identifiers.")
        End If

        Return New GeneAlignmentResult With {
            .First = alignedFirst.ToArray,
            .Second = alignedSecond.ToArray,
            .DroppedFromFirst = first.Length - alignedFirst.Count,
            .DroppedFromSecond = second.Length - alignedSecond.Count
        }
    End Function

    Private Function ValidateAndIndex(matrix As DataFrameRow(), argumentName As String) As Dictionary(Of String, DataFrameRow)
        If matrix Is Nothing Then
            Throw New ArgumentNullException(argumentName)
        ElseIf matrix.Length = 0 Then
            Throw New ArgumentException("Expression matrix cannot be empty.", argumentName)
        End If

        Dim sampleCount As Integer = matrix(0).samples
        Dim lookup As New Dictionary(Of String, DataFrameRow)(StringComparer.Ordinal)

        If sampleCount = 0 Then
            Throw New ArgumentException("Expression matrix must contain at least one sample.", argumentName)
        End If

        For Each row As DataFrameRow In matrix
            If row Is Nothing OrElse String.IsNullOrWhiteSpace(row.geneID) Then
                Throw New ArgumentException("Expression matrix contains an empty gene identifier.", argumentName)
            ElseIf row.experiments Is Nothing OrElse row.samples <> sampleCount Then
                Throw New ArgumentException($"Gene '{row.geneID}' has an inconsistent sample count.", argumentName)
            ElseIf lookup.ContainsKey(row.geneID) Then
                Throw New ArgumentException($"Expression matrix contains duplicate gene identifier '{row.geneID}'.", argumentName)
            End If

            lookup.Add(row.geneID, row)
        Next

        Return lookup
    End Function
End Module
