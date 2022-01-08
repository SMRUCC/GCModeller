Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Public Class GSVA

    Public Function gsva(expr As Matrix, gsetIdxList As Background, Optional method As Methods = Methods.gsva)

    End Function

    ''' <summary>
    ''' filter out genes with constant expression values
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="method"></param>
    ''' <returns></returns>
    Public Function filterFeatures(expr As Matrix, method As Methods) As Matrix
        Dim sdGenes As Vector = expr.rowSds.Values.AsVector

        sdGenes(sdGenes < 0.0000000001) = Vector.Zero

        If sdGenes.Any(Function(x) x = 0.0 OrElse x.IsNaNImaginary) Then
            Call $"{(sdGenes = 0.0 OrElse sdGenes.IsNaNImaginary).Sum} genes with constant expression values throuhgout the samples.".Warning

            If method <> Methods.ssgsea Then
                Call "Since argument method!='ssgsea', genes with constant expression values are discarded.".Warning

                expr = expr(sdGenes > 0 & Not sdGenes.IsNaNImaginary)
            End If
        End If

        If expr.size < 2 Then
            Throw New InvalidProgramException("Less than two genes in the input assay object")
        Else
            Return expr
        End If
    End Function
End Class

Public Enum Methods
    gsva
    ssgsea
    zscore
    plage
End Enum