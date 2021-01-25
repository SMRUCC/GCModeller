Imports Microsoft.VisualBasic.Math.Distributions
Imports stdnum = System.Math

Public Class Impact

    Public Property classLabel As String
    Public Property impactFactors As Double()

    Public Function GetSampleModel() As SampleDistribution
        Return New SampleDistribution(impactFactors)
    End Function

    Public Overrides Function ToString() As String
        Return classLabel
    End Function

    Public Shared Function FromDEGList(genes As IEnumerable(Of DEGModel)) As Impact()
        Return genes _
            .GroupBy(Function(g) g.class) _
            .Select(Function(group)
                        Return New Impact With {
                            .classLabel = group.Key,
                            .impactFactors = group _
                                .Select(Function(deg)
                                            Return deg.VIP * (-stdnum.Log10(deg.pvalue)) * (stdnum.Abs(deg.logFC))
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray
    End Function

End Class
