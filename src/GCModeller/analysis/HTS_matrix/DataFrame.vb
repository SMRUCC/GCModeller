Imports System.Runtime.CompilerServices

Public Module HTSDataFrame

    ''' <summary>
    ''' merge multiple batches data directly
    ''' </summary>
    ''' <param name="batches">
    ''' matrix in multiple batches data should be normalized at
    ''' first before calling this data batch merge function.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function MergeMultipleHTSMatrix(batches As Matrix()) As Matrix
        Dim matrix As Matrix = batches(Scan0)
        Dim geneIndex = matrix.expression.ToDictionary(Function(g) g.geneID)
        Dim sampleList As New List(Of String)(matrix.sampleID)

        For Each append As Matrix In batches.Skip(1)
            For Each gene As DataFrameRow In append.expression
                Dim v As Double() = New Double(sampleList.Count + append.sampleID.Length - 1) {}
                Dim a As Double()
                Dim b As Double() = gene.experiments

                If geneIndex.ContainsKey(gene.geneID) Then
                    a = geneIndex(gene.geneID).experiments
                Else
                    a = New Double(sampleList.Count - 1) {}
                End If

                Call Array.ConstrainedCopy(a, Scan0, v, Scan0, a.Length)
                Call Array.ConstrainedCopy(b, Scan0, v, a.Length, b.Length)

                geneIndex(gene.geneID) = New DataFrameRow With {
                    .experiments = v,
                    .geneID = gene.geneID
                }
            Next

            Call sampleList.AddRange(append.sampleID)
        Next

        Return New Matrix With {
            .expression = geneIndex.Values.ToArray,
            .sampleID = sampleList.ToArray,
            .tag = batches _
                .Select(Function(m) m.tag) _
                .JoinBy("+")
        }
    End Function
End Module