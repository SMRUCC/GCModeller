
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.Proteomics.FoldChangeMatrix
Imports Microsoft.VisualBasic.Math

<Package("proteomics.labelfree")>
Module labelFree

    <ExportAPI("sample.normalize")>
    Public Function totalSumNormalize(data As DataSet(), Optional byMedianQuantile As Boolean = False, Optional samples As String() = Nothing) As DataSet()
        Return data.TotalSumNormalize(byMedianQuantile, samples).ToArray
    End Function

    <ExportAPI("sample.normalize.correlation")>
    Public Function normalizationCorrelation(data As DataSet(), Optional samples As String() = Nothing) As Dictionary(Of String, Double)
        Dim totalSum = data.TotalSumNormalize(byMedianQuantile:=False, samples:=samples).ToArray
        Dim medianNor = data.TotalSumNormalize(byMedianQuantile:=True, samples:=samples).ToArray
        Dim cor As Dictionary(Of String, Double) = totalSum.PropertyNames _
            .ToDictionary(Function(name) name,
                          Function(name)
                              Dim ts As Double() = totalSum.Vector(name)
                              Dim md As Double() = totalSum.Vector(name)

                              Return Correlations.GetPearson(ts, md)
                          End Function)
        Return cor
    End Function
End Module
