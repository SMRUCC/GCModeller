Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module Extensions

    ''' <summary>
    ''' ``ExperimentDesigner`` category for the <see cref="TemplateAttribute"/>
    ''' </summary>
    Public Const ExperimentDesigner$ = NameOf(ExperimentDesigner)

    ''' <summary>
    ''' 为成对数据的T检验设计的帮助函数
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <param name="analysisDesign"></param>
    ''' <param name="sampleTuple"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function PairedAnalysisSamples(sampleInfo As IEnumerable(Of SampleInfo),
                                                   analysisDesign As IEnumerable(Of AnalysisDesigner),
                                                   sampleTuple As IEnumerable(Of SampleTuple)) As IEnumerable(Of NamedCollection(Of AnalysisDesigner))

        With sampleInfo.DataAnalysisDesign(analysisDesign)

            For Each group In .ref.IterateNameCollections

                ' 将成对比较的标签选出来
                Dim designer = group _
                    .Value _
                    .Where(Function(ad As AnalysisDesigner)
                               For Each tuple As SampleTuple In sampleTuple
                                   If ad.EqualsToTuple(tuple) Then
                                       Return True
                                   End If
                               Next

                               Return False
                           End Function) _
                    .ToArray

                Yield New NamedCollection(Of AnalysisDesigner) With {
                    .Name = group.Name,
                    .Value = designer
                }
            Next
        End With
    End Function

    <Extension>
    Private Function EqualsToTuple(ad As AnalysisDesigner, tuple As SampleTuple) As Boolean
        If ad.Controls = tuple.Sample1 AndAlso ad.Experimental = tuple.Sample2 Then
            Return True
        ElseIf ad.Experimental = tuple.Sample1 AndAlso ad.Controls = tuple.Sample2 Then
            Return True
        Else
            Return False
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function TakeGroup(sampleInfo As IEnumerable(Of SampleInfo), groupLabel$) As SampleInfo()
        Return sampleInfo _
            .Where(Function(sample) sample.sample_group = groupLabel) _
            .ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ToCategory(sampleInfo As IEnumerable(Of SampleInfo)) As Dictionary(Of NamedCollection(Of String))
        Return sampleInfo _
            .GroupBy(Function(sample) sample.sample_group) _
            .Select(Function(group)
                        Return New NamedCollection(Of String) With {
                            .Name = group.Key,
                            .Value = group.SampleNames
                        }
                    End Function) _
            .ToDictionary
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function SampleNames(sampleInfo As IEnumerable(Of SampleInfo)) As String()
        Return sampleInfo _
            .Select(Function(sample) sample.sample_name) _
            .ToArray
    End Function

    ''' <summary>
    ''' ``<see cref="SampleInfo.ID"/> -> <see cref="SampleInfo.sample_group"/>``
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function SampleGroupInfo(sampleInfo As IEnumerable(Of SampleInfo)) As Dictionary(Of String, String)
        Return sampleInfo.ToDictionary(
            Function(sample) sample.ID,
            Function(sample) sample.sample_group)
    End Function

    <Extension>
    Public Function SampleGroupColor(sampleInfo As IEnumerable(Of SampleInfo)) As Dictionary(Of String, Color)

    End Function

    ''' <summary>
    ''' 将组别比对标记转换为样品比对标记
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <param name="analysis">
    ''' <see cref="AnalysisDesigner.Controls"/>和<see cref="AnalysisDesigner.Experimental"/>都是组别名称
    ''' </param>
    ''' <returns>
    ''' 经过这个函数转换之后，<see cref="AnalysisDesigner.Controls"/>和<see cref="AnalysisDesigner.Experimental"/>
    ''' 都分别被转换为样品标记了
    ''' </returns>
    ''' <remarks>
    ''' 例如，存在下面的样品信息
    ''' 
    ''' + PCCa组	
    '''   + M-P-CCa1
    '''	  + M-P-CCa2
    '''	  + M-P-CCa3
    ''' 
    ''' + PCCaT组	
    '''   + M-P-CCaT1
    '''	  + M-P-CCaT2
    '''	  + M-P-CCaT3
    '''	
    ''' 而实验数据分析设计则是：``PCCa组 vs PCCaT组``
    ''' 
    ''' 故而需要使用这个函数将组别标记转换为实际计算分析所要使用到的样品标记
    ''' </remarks>
    <Extension>
    Public Function DataAnalysisDesign(sampleInfo As IEnumerable(Of SampleInfo), analysis As IEnumerable(Of AnalysisDesigner)) As Dictionary(Of String, AnalysisDesigner())
        Dim sampleGroups = sampleInfo _
            .GroupBy(Function(label) label.sample_group) _
            .ToDictionary(Function(x) x.Key,
                          Function(g) g.ToArray)
        Dim designs = analysis.ToDictionary(
            Function(name) name.ToString,
            Function(designer)
                Dim control = sampleGroups(designer.Controls)
                Dim experimentals = sampleGroups(designer.Experimental)

                ' 对照 vs 处理 
                Return control _
                    .Select(Function(c)
                                Return experimentals _
                                    .Select(Function(e)
                                                Return New AnalysisDesigner With {
                                                    .Controls = c.sample_name,
                                                    .Experimental = e.sample_name,
                                                    .Reversed = designer.Reversed
                                                }
                                            End Function)
                            End Function) _
                    .IteratesALL _
                    .ToArray
            End Function)

        Return designs
    End Function

    ''' <summary>
    ''' Ensure all of the name label in <paramref name="allSamples"/> were paired in groups.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sampleInfo">Probably this is the part of the sample name collection</param>
    ''' <param name="allSamples$">Contains all sample names, <paramref name="sampleInfo"/> maybe is the subset of it.</param>
    ''' <param name="groupCreated$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function EnsureGroupPaired(Of T As {New, SampleGroup})(sampleInfo As IEnumerable(Of T), allSamples$(), Optional groupCreated$ = "Control") As NamedCollection(Of T)()
        Dim vector = sampleInfo.ToArray
        Dim groups = vector _
            .GroupBy(Function(s) s.sample_group) _
            .Select(Function(g)
                        Return New NamedCollection(Of T) With {
                            .Name = g.Key,
                            .Value = g.ToArray
                        }
                    End Function) _
            .ToArray
        Dim controls$() = allSamples - vector.Keys

        If controls.Length > 0 Then

            ' returns case + control
            Return groups.AsList + New NamedCollection(Of T) With {
                .Name = groupCreated,
                .Value = controls _
                    .Select(Function(name)
                                Return New T With {
                                    .sample_name = name,
                                    .sample_group = groupCreated
                                }
                            End Function) _
                    .ToArray
            }
        Else
            Return groups
        End If
    End Function
End Module