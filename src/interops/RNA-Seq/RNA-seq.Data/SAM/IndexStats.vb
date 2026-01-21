Imports System.IO
Imports Microsoft.VisualBasic.Language

Namespace SAM

    ''' <summary>
    ''' gene abundance result
    ''' </summary>
    Public Class GeneData

        Public Property GeneID As String
        Public Property Length As Double
        Public Property RawCount As Double
        Public Property RPK As Double
        Public Property TPM As Double

    End Class

    ''' <summary>
    ''' A row of the samtool indexstats output
    ''' </summary>
    Public Class IndexStats

        Public Property GeneID As String
        Public Property Length As Integer
        Public Property RawCount As Integer
        Public Property UnmappedBases As Integer

        Public Shared Iterator Function Parse(file As Stream) As IEnumerable(Of IndexStats)
            Using str As New StreamReader(file)
                Dim line As Value(Of String) = ""

                Do While (line = str.ReadLine) IsNot Nothing
                    If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("*") OrElse line.StartsWith("@") Then
                        Continue Do
                    End If

                    Dim fields As String() = line.Split(vbTab)
                    Dim gene_count As New IndexStats With {
                        .GeneID = fields(0),
                        .Length = CInt(fields(1)),
                        .RawCount = CInt(fields(2)),
                        .UnmappedBases = CInt(Val(fields.ElementAtOrNull(3)))
                    }

                    Yield gene_count
                Loop
            End Using
        End Function

        Public Shared Iterator Function ConvertCountsToTPM(stats As IEnumerable(Of IndexStats)) As IEnumerable(Of GeneData)
            Dim genes As New List(Of GeneData)()
            Dim totalRPK As Double = 0.0

            For Each hit As IndexStats In stats
                Dim gene As New GeneData() With {
                    .GeneID = hit.GeneID,
                    .Length = hit.Length,
                    .RawCount = hit.RawCount,
                    .RPK = (.RawCount * 1000.0) / .Length
                }

                genes.Add(gene)
                totalRPK += gene.RPK
            Next

            ' --- 第二步：根据 totalRPK 计算 TPM ---
            If totalRPK = 0 Then
                Call "Warning: Total RPK is 0. All TPM values will be 0.".warning
            End If

            For Each gene As GeneData In genes
                If totalRPK > 0 Then
                    gene.TPM = (gene.RPK / totalRPK) * 1000000.0
                Else
                    gene.TPM = 0.0
                End If

                Yield gene
            Next
        End Function

    End Class
End Namespace