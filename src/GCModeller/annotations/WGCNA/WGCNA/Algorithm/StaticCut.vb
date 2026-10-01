#Region "Microsoft.VisualBasic::StaticCut, annotations\WGCNA\WGCNA\Algorithm\StaticCut.vb"

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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 静态树剪切：按树总距离的百分比做一刀切
''' </summary>
''' <remarks>
''' 这是 WGCNA 项目原有的剪切方式：把整棵树的 <see cref="Cluster.TotalDistance"/>
''' 乘以一个比例 <c>distCut</c> 得到距离阈值，凡是子树总距离不超过该阈值的
''' 就直接成为一个模块。
''' 
''' <para>
''' 它的优点是结果稳定、无需不相似度矩阵、参数直观；缺点是「一刀切」对
''' 尺度差异很大的分支不友好，因此新流程默认改用 <see cref="DynamicTreeCut"/>，
''' 这里保留实现以便对照与向后兼容。
''' </para>
''' </remarks>
Public Module StaticCut

    ''' <summary>
    ''' 从层次聚类树创建模块（按树总距离百分比做静态剪切）
    ''' </summary>
    ''' <param name="tree">层次聚类树</param>
    ''' <param name="distCut">a percentage threshold value in range ``[0,1]``</param>
    ''' <returns>模块集合</returns>
    <Extension>
    Public Function CreateModules(tree As Cluster, Optional distCut As Double = 0.6) As IEnumerable(Of NamedCollection(Of String))
        Return CreateModulesInternal(tree, distCut:=tree.TotalDistance * distCut) _
            .Where(Function(m) m.Count > 0) _
            .ToArray
    End Function

    ''' <summary>
    ''' 静态剪切并直接返回「模块名 → 基因列表」字典
    ''' </summary>
    ''' <param name="tree">层次聚类树</param>
    ''' <param name="distCut">树总距离的百分比阈值</param>
    ''' <returns>模块字典</returns>
    Public Function Cutree(tree As Cluster, Optional distCut As Double = 0.6) As Dictionary(Of String, String())
        Return tree _
            .CreateModules(distCut) _
            .ToDictionary(Function(m) m.name,
                          Function(m)
                              Return m.ToArray
                          End Function)
    End Function

    <Extension>
    Private Iterator Function CreateModulesInternal(tree As Cluster, distCut As Double) As IEnumerable(Of NamedCollection(Of String))
        If tree.TotalDistance <= distCut Then
            Dim items As New List(Of String)
            Dim name As String = tree.Name

            If tree.isLeaf Then
                items.Add(tree.Name)
            Else
                For Each child As Cluster In tree.Children
                    Call child _
                        .CreateModulesInternal(distCut) _
                        .Select(Function(m) m.value) _
                        .IteratesALL _
                        .DoCall(AddressOf items.AddRange)
                Next
            End If

            Yield New NamedCollection(Of String)(name, items)
        Else
            For Each child As Cluster In tree.Children
                For Each m As NamedCollection(Of String) In child.CreateModulesInternal(distCut)
                    Yield m
                Next
            Next
        End If
    End Function
End Module
