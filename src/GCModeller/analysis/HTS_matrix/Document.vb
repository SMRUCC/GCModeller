Imports Microsoft.VisualBasic.Text

Public Module Document

    Public Function LoadMatrixDocument(file As String) As Matrix
        Dim text As String() = file.LineIterators.ToArray
        Dim sampleIds As String() = text(Scan0).Split(ASCII.TAB).Skip(1).ToArray
        Dim matrix As DataFrameRow() = text _
            .Skip(1) _
            .Select(Function(line)
                        Dim tokens = line.Split(ASCII.TAB)
                        Dim data As Double() = tokens _
                            .Skip(1) _
                            .Select(AddressOf Val) _
                            .ToArray

                        Return New DataFrameRow With {
                            .experiments = data,
                            .geneID = tokens(Scan0)
                        }
                    End Function) _
            .ToArray

        Return New Matrix With {
            .expression = matrix,
            .sampleID = sampleIds
        }
    End Function
End Module
