#Region "Microsoft.VisualBasic::f6fc13690fbbd6d55ad6ba646ba91e40, modules\ExperimentDesigner\Extensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 306
    '    Code Lines: 192
    ' Comment Lines: 89
    '   Blank Lines: 25
    '     File Size: 12.82 KB


    ' Module Extensions
    ' 
    '     Function: DataAnalysisDesign, EnsureGroupPaired, EqualsToTuple, PairedAnalysisSamples, SampleGroupColor
    '               SampleGroupInfo, SampleIDs, SampleNames, SetNames, TakeGroup
    '               ToCategory
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

<HideModuleName>
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
                                                   sampleTuple As IEnumerable(Of SampleTuple)) As IEnumerable(Of NamedCollection(Of SampleTuple))

        For Each group In sampleInfo.DataAnalysisDesign(analysisDesign).IterateNameCollections

            ' 将成对比较的标签选出来
            Dim designer = group _
                .value _
                .Where(Function(ad As AnalysisDesigner)
                           For Each tuple As SampleTuple In sampleTuple
                               If ad.EqualsToTuple(tuple) Then
                                   Return True
                               End If
                           Next

                           Return False
                       End Function) _
                .Select(Function(ad)
                            Return New SampleTuple With {
                                .sample1 = ad.controls,
                                .sample2 = ad.treatment
                            }
                        End Function) _
                .ToArray

            Yield New NamedCollection(Of SampleTuple) With {
                .name = group.name,
                .value = designer
            }
        Next
    End Function

    <Extension>
    Private Function EqualsToTuple(ad As AnalysisDesigner, tuple As SampleTuple) As Boolean
        If ad.controls = tuple.sample1 AndAlso ad.treatment = tuple.sample2 Then
            Return True
        ElseIf ad.treatment = tuple.sample1 AndAlso ad.controls = tuple.sample2 Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' 从所给定的样本信息列表之中,取出指定组别编号的所有的样本信息
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <param name="groupLabel"><see cref="SampleInfo.sample_info"/></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function TakeGroup(Of T As SampleGroup)(sampleInfo As IEnumerable(Of T), groupLabel$) As T()
        Return sampleInfo _
            .Where(Function(sample) sample.sample_info = groupLabel) _
            .ToArray
    End Function

    ''' <summary>
    ''' 按照<see cref="SampleGroup.sample_info"/>进行分组，读取出每一个分组之中的
    ''' <see cref="SampleGroup.sample_name"/>
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ToCategory(Of T As SampleGroup)(sampleInfo As IEnumerable(Of T)) As Dictionary(Of NamedCollection(Of String))
        Return sampleInfo _
            .GroupBy(Function(sample) sample.sample_info) _
            .Select(Function(group)
                        Return New NamedCollection(Of String) With {
                            .name = group.Key,
                            .value = group.SampleNames
                        }
                    End Function) _
            .ToDictionary
    End Function

    ''' <summary>
    ''' 取出所有的<see cref="SampleGroup.sample_name"/>
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function SampleNames(sampleInfo As IEnumerable(Of SampleGroup)) As String()
        Return sampleInfo _
            .Select(Function(sample) sample.sample_name) _
            .ToArray
    End Function

    ''' <summary>
    ''' 取出<see cref="SampleInfo.ID"/>
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function SampleIDs(sampleInfo As IEnumerable(Of SampleInfo), Optional group$ = Nothing) As List(Of String)
        Return sampleInfo _
            .Where(Function(sample)
                       If group.StringEmpty Then
                           Return True
                       Else
                           Return sample.sample_info = group
                       End If
                   End Function) _
            .Select(Function(sample) sample.ID) _
            .AsList
    End Function

    ''' <summary>
    ''' ``<see cref="SampleInfo.ID"/> -> <see cref="SampleInfo.sample_info"/>``
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function SampleGroupInfo(sampleInfo As IEnumerable(Of SampleInfo)) As Dictionary(Of String, String)
        Return sampleInfo.ToDictionary(
            Function(sample) sample.ID,
            Function(sample)
                Return sample.sample_info
            End Function)
    End Function

    ''' <summary>
    ''' 相同分组的sample都会被分配到相同的颜色，这个函数主要是为一些分类相关的绘图准备的，例如PCA之类的绘图
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sampleInfo"></param>
    ''' <param name="colors">Color for <see cref="SampleGroup.sample_info"/></param>
    ''' <returns></returns>
    <Extension>
    Public Function SampleGroupColor(Of T As SampleGroup)(sampleInfo As IEnumerable(Of T), colors As Color()) As Dictionary(Of String, Color)
        With sampleInfo.ToArray
            Dim colorList As New LoopArray(Of Color)(colors)
            Dim groups = .Select(Function(sample)
                                     Return sample.sample_info
                                 End Function) _
                .Distinct _
                .ToArray
            Dim groupColors = groups.ToDictionary(Function(label) label,
                                                  Function()
                                                      Return colorList.Next
                                                  End Function)

            Return .ToDictionary(Function(sample) sample.sample_name,
                                 Function(sample)
                                     Return groupColors(sample.sample_info)
                                 End Function)
        End With
    End Function

    ''' <summary>
    ''' 将组别比对标记转换为样品比对标记
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <param name="analysis">
    ''' <see cref="AnalysisDesigner.Controls"/>和<see cref="AnalysisDesigner.Treatment"/>都是组别名称
    ''' </param>
    ''' <returns>
    ''' 经过这个函数转换之后，<see cref="AnalysisDesigner.Controls"/>和<see cref="AnalysisDesigner.Treatment"/>
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
            .GroupBy(Function(label) label.sample_info) _
            .ToDictionary(Function(x) x.Key,
                          Function(g)
                              Return g.ToArray
                          End Function)
        Dim translation = sampleInfo.ToDictionary(Function(sample) sample.sample_name)
        Dim designs = analysis.ToDictionary(
            Function(name) name.ToString,
            Function(designer)
                Dim control = sampleGroups(designer.controls)
                Dim experimentals = sampleGroups(designer.treatment)

                ' 对照 vs 处理 
                Return control _
                    .Select(Function(c)
                                Return experimentals _
                                    .Select(Function(e)
                                                Return New AnalysisDesigner With {
                                                    .controls = c.sample_name,
                                                    .treatment = e.sample_name,
                                                    .note = translation(.treatment).ID & "/" & translation(.controls).ID
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
            .GroupBy(Function(s) s.sample_info) _
            .Select(Function(g)
                        Return New NamedCollection(Of T) With {
                            .name = g.Key,
                            .value = g.ToArray
                        }
                    End Function) _
            .ToArray
        Dim controls$() = allSamples - vector.Keys

        If controls.Length > 0 Then

            ' returns case + control
            Return groups.AsList + New NamedCollection(Of T) With {
                .name = groupCreated,
                .value = controls _
                    .Select(Function(name)
                                Return New T With {
                                    .sample_name = name,
                                    .sample_info = groupCreated
                                }
                            End Function) _
                    .ToArray
            }
        Else
            Return groups
        End If
    End Function

    ''' <summary>
    ''' 将<see cref="SampleInfo.ID"/>映射为对应的<see cref="SampleInfo.sample_name"/>
    ''' </summary>
    ''' <param name="matrix">
    ''' 属性的键名称应该都是<see cref="SampleInfo.ID"/>
    ''' </param>
    <Extension>
    Public Function SetNames(Of DataSet As {INamedValue, DynamicPropertyBase(Of Double)})(matrix As DataSet(), value As SampleInfo()) As SampleInfo()
        For Each data As DataSet In matrix
            Dim row As Dictionary(Of String, Double) = data.Properties

            For Each sample As SampleInfo In value
                If row.ContainsKey(sample.ID) Then
                    Dim x As Double = row(sample.ID)

                    Call row.Remove(sample.ID)
                    Call row.Add(sample.sample_name, x)
                End If
            Next
        Next

        Return value
    End Function
End Module
