
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.Proteomics.FoldChangeMatrix
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

<Package("proteomics.labelfree")>
Module labelFree

    <ExportAPI("sample.normalize")>
    Public Function totalSumNormalize(data As DataSet(), Optional byMedianQuantile As Boolean = False, Optional samples As String() = Nothing) As DataSet()
        Return data.TotalSumNormalize(byMedianQuantile, samples).ToArray
    End Function

    <Extension>
    Private Function aggregateByGroupLabels(data As IEnumerable(Of DataSet), groups As NamedCollection(Of String)()) As Dictionary(Of String, DataSet)
        Return data.ToDictionary(Function(r) r.ID,
                                 Function(r)
                                     Return New DataSet With {
                                        .ID = r.ID,
                                        .Properties = groups _
                                            .ToDictionary(Function(group) group.name,
                                                          Function(group)
                                                              Return r(group).Average
                                                          End Function)
                                     }
                                 End Function)
    End Function

    <ExportAPI("sample.normalize.correlation")>
    Public Function normalizationCorrelation(data As DataSet(), Optional samples As String() = Nothing) As DataSet()
        Dim rownames As String() = data.Keys.ToArray
        Dim sampleNames As String() = data.PropertyNames
        Dim groups = sampleNames.GuessPossibleGroups.ToArray
        Dim totalSum = data.TotalSumNormalize(byMedianQuantile:=False, samples:=samples).aggregateByGroupLabels(groups)
        Dim medianNor = data.TotalSumNormalize(byMedianQuantile:=True, samples:=samples).aggregateByGroupLabels(groups)
        Dim cor As Dictionary(Of String, (pcc#, pvalue1#, pvalue2#)) = groups _
            .Keys _
            .ToDictionary(Function(name) name,
                          Function(name)
                              Dim ts As Double() = rownames.Select(Function(id) totalSum(id)(name)).ToArray
                              Dim md As Double() = rownames.Select(Function(id) medianNor(id)(name)).ToArray
                              Dim pvalue1 As Double = 0
                              Dim pvalue2 As Double = 0
                              Dim corVal As Double

                              Try
                                  corVal = Correlations.GetPearson(ts, md, pvalue1, pvalue2)
                              Catch ex As Exception
                                  corVal = 0
                                  pvalue1 = 1
                                  pvalue2 = 1
                              End Try

                              Return (corVal, pvalue1, pvalue2)
                          End Function)
        Dim output As New List(Of DataSet)
        Dim aggregate As New DataSet With {.ID = "*Aggregate"}
        Dim vector As Double()
        Dim rawMatrix = data.ToDictionary(Function(r) r.ID)

        For Each sample As String In sampleNames
            vector = data.Vector(sample)
            aggregate.Add($"{sample}.sum", vector.Sum)
            aggregate.Add($"{sample}.median", vector.Quartile.Q2)
        Next

        output.Add(aggregate)
        aggregate = New DataSet With {.ID = "*Correlation"}

        For Each sample As String In sampleNames
            aggregate.Add($"{sample}.sum", cor(sample).pcc)
            aggregate.Add($"{sample}.median", 0.0)
        Next

        output.Add(aggregate)

        aggregate = New DataSet With {.ID = "*p-value1"}

        For Each sample As String In sampleNames
            aggregate.Add($"{sample}.sum", cor(sample).pvalue1)
            aggregate.Add($"{sample}.median", 0.0)
        Next

        output.Add(aggregate)

        aggregate = New DataSet With {.ID = "*p-value2"}

        For Each sample As String In sampleNames
            aggregate.Add($"{sample}.sum", cor(sample).pvalue2)
            aggregate.Add($"{sample}.median", 0.0)
        Next

        output.Add(aggregate)

        For Each id As String In rownames
            aggregate = New DataSet With {.ID = id}

            For Each sample As String In sampleNames
                aggregate.Add($"{sample}.sum", totalSum(id)(sample))
                aggregate.Add($"{sample}.median", medianNor(id)(sample))
            Next

            output.Add(aggregate)
        Next

        Return output.ToArray
    End Function
End Module
