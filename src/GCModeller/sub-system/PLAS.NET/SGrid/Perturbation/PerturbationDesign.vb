Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports std = System.Math

Namespace Perturbation

    ''' <summary>
    ''' The type of the virtual perturbation experiment.
    ''' (虚拟扰动实验的类型)
    ''' </summary>
    Public Enum PerturbationTypes
        ''' <summary>
        ''' 敲低：把目标的表达量按照比例下调
        ''' </summary>
        KnockDown = 0
        ''' <summary>
        ''' 敲除：把目标的表达量直接置零
        ''' </summary>
        KnockOut = 1
        ''' <summary>
        ''' 过表达：把目标的表达量按照倍数上调
        ''' </summary>
        OverExpress = 2
        ''' <summary>
        ''' 自定义：直接改变到指定的数值
        ''' </summary>
        Custom = 3
    End Enum

    ''' <summary>
    ''' The design of a virtual perturbation experiment which will be applied on the fitted S-system model.
    ''' (加载到拟合模型之上的虚拟扰动实验设计)
    ''' </summary>
    Public Class PerturbationDesign : Implements INamedValue

        ''' <summary>
        ''' 扰动的目标基因（规整化之后的编号）
        ''' </summary>
        ''' <returns></returns>
        Public Property TargetGene As String Implements INamedValue.Key
        ''' <summary>
        ''' 扰动的类型
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As PerturbationTypes = PerturbationTypes.KnockDown
        ''' <summary>
        ''' 扰动的强度：
        ''' 敲低时为下调的比例（0..1，默认 0.8 表示降低到原来的 20%）；
        ''' 过表达时为上调的倍数（默认 2 表示增加两倍当前表达量）
        ''' </summary>
        ''' <returns></returns>
        Public Property Magnitude As Double = 0.8R
        ''' <summary>
        ''' 第一次施加扰动的时间点
        ''' </summary>
        ''' <returns></returns>
        Public Property Start As Double = 0
        ''' <summary>
        ''' 两次扰动之间的时间间隔
        ''' </summary>
        ''' <returns></returns>
        Public Property Interval As Double = 1
        ''' <summary>
        ''' 扰动的施加次数
        ''' </summary>
        ''' <returns></returns>
        Public Property Kicks As Integer = 1
        ''' <summary>
        ''' 自定义扰动模式之下所期望的目标数值
        ''' </summary>
        ''' <returns></returns>
        Public Property Value As Double = 0

        ''' <summary>
        ''' 生成 S-system 求解器可以直接使用的扰动实验对象
        ''' </summary>
        ''' <param name="baseline">目标基因的基线表达量（通常取时间序列的初始值）</param>
        ''' <returns></returns>
        Public Function ToExperiment(Optional baseline As Double = 1) As Experiment
            Dim experiment As New Experiment With {
                .Id = TargetGene,
                .Start = Start,
                .Interval = Interval,
                .Kicks = Kicks
            }

            If baseline <= 0 Then
                baseline = 1
            End If

            Select Case Type
                Case PerturbationTypes.KnockDown
                    experiment.DisturbType = Types.Decrease
                    experiment.Value = baseline * std.Max(std.Min(Magnitude, 1.0R), 0)
                Case PerturbationTypes.KnockOut
                    experiment.DisturbType = Types.Decrease
                    experiment.Value = baseline
                Case PerturbationTypes.OverExpress
                    experiment.DisturbType = Types.Increase
                    experiment.Value = baseline * std.Max(Magnitude, 0)
                Case Else
                    experiment.DisturbType = Types.ChangeTo
                    experiment.Value = Value
            End Select

            Return experiment
        End Function

        Public Overrides Function ToString() As String
            Return $"{Type.ToString}({TargetGene}, magnitude={Magnitude}, start={Start}, kicks={Kicks})"
        End Function

        ''' <summary>
        ''' 为每一个基因生成一个同类型的扰动实验设计
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="type"></param>
        ''' <param name="magnitude"></param>
        ''' <param name="start"></param>
        ''' <param name="kicks"></param>
        ''' <returns></returns>
        Public Shared Function Batch(genes As IEnumerable(Of String),
                                     Optional type As PerturbationTypes = PerturbationTypes.KnockDown,
                                     Optional magnitude As Double = 0.8R,
                                     Optional start As Double = 0,
                                     Optional kicks As Integer = 1) As PerturbationDesign()

            Return genes _
                .SafeQuery() _
                .Select(Function(gene)
                            Return New PerturbationDesign With {
                                .TargetGene = gene,
                                .Type = type,
                                .Magnitude = magnitude,
                                .Start = start,
                                .Kicks = kicks
                            }
                        End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' 依据表达矩阵之中的基因列表生成批量的扰动实验设计
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="type"></param>
        ''' <param name="magnitude"></param>
        ''' <returns></returns>
        Public Shared Function FromMatrix(matrix As Matrix,
                                          Optional type As PerturbationTypes = PerturbationTypes.KnockDown,
                                          Optional magnitude As Double = 0.8R) As PerturbationDesign()

            Dim genes As String() = matrix _
                .expression _
                .SafeQuery() _
                .Select(Function(row) GRN.ExpressionTimeSeries.SanitizeGeneId(row.geneID)) _
                .ToArray

            Return Batch(genes, type, magnitude)
        End Function
    End Class
End Namespace
