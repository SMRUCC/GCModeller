Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text

Public Module Document

    Public Function LoadMatrixDocument(file As String, excludes As Index(Of String)) As Matrix
        Dim text As String() = file.LineIterators.ToArray
        Dim sampleIds As String() = text(Scan0).Split(ASCII.TAB).Skip(1).ToArray
        Dim takeIndex As Integer()

        If excludes Is Nothing Then
            takeIndex = sampleIds.Sequence.ToArray
        Else
            takeIndex = sampleIds _
                .Select(Function(name, i) (i, Not name Like excludes)) _
                .Where(Function(a) a.Item2 = True) _
                .Select(Function(a) a.i) _
                .ToArray
        End If

        Dim matrix As DataFrameRow() = text _
            .Skip(1) _
            .Select(Function(line)
                        Dim tokens = line.Split(ASCII.TAB)
                        Dim data As Double() = tokens _
                            .Skip(1) _
                            .Select(AddressOf Val) _
                            .ToArray

                        If Not excludes Is Nothing Then
                            data = takeIndex _
                                .Select(Function(i) data(i)) _
                                .ToArray
                        End If

                        Return New DataFrameRow With {
                            .experiments = data,
                            .geneID = tokens(Scan0)
                        }
                    End Function) _
            .ToArray

        Return New Matrix With {
            .expression = matrix,
            .sampleID = takeIndex _
                .Select(Function(i) sampleIds(i)) _
                .ToArray
        }
    End Function
End Module
