Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math
Imports RDotNET.Extensions.VisualBasic.API
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' avg(A)/avg(B) = FC
''' </summary>
Public Module LabelFreeTtest

    ''' <summary>
    ''' 一次只计算出一组实验设计的结果
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="analysis"></param>
    ''' <param name="level#"></param>
    ''' <param name="pvalue#"></param>
    ''' <param name="fdrThreshold#"></param>
    ''' <returns></returns>
    <Extension>
    Public Function logFCtest(data As IEnumerable(Of DataSet),
                              analysis As AnalysisDesigner,
                              sampleInfo As SampleInfo(),
                              Optional level# = 1.5,
                              Optional pvalue# = 0.05,
                              Optional fdrThreshold# = 0.05) As DEP_iTraq()

        Dim experiment$() = sampleInfo.TakeGroup(analysis.Experimental).SampleNames
        Dim controls$() = sampleInfo.TakeGroup(analysis.Controls).SampleNames
        Dim allSamples$() = experiment.AsList + controls

        ' calc the different expression proteins
        Dim calc = data _
            .Select(Function(protein)
                        Dim FC# = protein(experiment).Average / protein(controls).Average
                        Dim log2FC# = Math.Log(FC, newBase:=2)
                        Dim p_value# = stats.Ttest(
                            protein(experiment),
                            protein(controls),
                            varEqual:=True).pvalue

                        Dim DEP As New DEP_iTraq With {
                            .ID = protein.ID,
                            .FCavg = FC,
                            .log2FC = log2FC,
                            .pvalue = pvalue,
                            .Properties = protein _
                                .SubSet(allSamples) _
                                .Properties _
                                .AsCharacter
                        }

                        Return DEP
                    End Function) _
            .ToArray

        With calc.VectorShadows



        End With

        Return calc
    End Function
End Module
