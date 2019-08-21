Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 对于一个很大的矩阵而言, 因为XML文件是字符串格式, 序列化和反序列化都需要大量进行字符串的解析或者生成等操作, 
''' I/O性能会非常差, 并且生成的模型文件会很大 
''' 
''' 所以对于大矩阵而言, 在这里使用CDF文件来存储, 从而减小模型文件以及提升IO性能
''' </summary>
Public Module GridMatrixCDF

    ReadOnly doubleFullName$ = GetType(Double).FullName

    Public Sub WriteCDF(genome As GridSystem, cdf$, Optional names$() = Nothing)
        Dim attr As [Variant](Of attribute, attribute())
        Dim cor As Correlation

        If names.IsNullOrEmpty Then
            names = genome.A _
                .Sequence _
                .Select(Function(i) $"X{i + 1}") _
                .ToArray
        End If

        Using cdfWriter = New CDFWriter(cdf).Dimensions(
            Dimension.Double,
            Dimension.Integer
        )

            Call cdfWriter.GlobalAttributes(
                New attribute With {.name = "dataset", .type = CDFDataTypes.CHAR, .value = GetType(GridSystem).FullName},
                New attribute With {.name = "size", .type = CDFDataTypes.INT, .value = names.Length},
                New attribute With {.name = "create-time", .type = CDFDataTypes.CHAR, .value = Now.ToString}
            )

            attr = New attribute With {
                .name = "const",
                .type = CDFDataTypes.DOUBLE,
                .value = genome.AC
            }

            Call cdfWriter.AddVariable("cor.factor", genome.A, doubleFullName, attr)

            For i As Integer = 0 To names.Length - 1
                cor = genome.C(i)
                attr = New attribute With {
                    .name = "const",
                    .type = CDFDataTypes.DOUBLE,
                    .value = cor.BC
                }

                Call cdfWriter.AddVariable(names(i), cor.B, doubleFullName, attr)
            Next
        End Using
    End Sub
End Module
