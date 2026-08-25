#Region "Microsoft.VisualBasic::93104b11e1a2c0586d77c0223a83dd6b, sub-system\BNLearn\Core\PriorNetwork.vb"

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

'   Total Lines: 103
'    Code Lines: 54 (52.43%)
' Comment Lines: 28 (27.18%)
'    - Xml Docs: 78.57%
' 
'   Blank Lines: 21 (20.39%)
'     File Size: 4.13 KB


'     Class RegulatoryEdge
' 
'         Properties: Confidence, Evidence, RegulationType, TargetGene, TF
' 
'         Function: ToString
' 
'     Class PriorNetwork
' 
'         Properties: Edges, TargetNames, TFNames
' 
'         Function: GetAllGeneNames, GetRegulators, GetTargets, ToWhitelist
' 
'         Sub: AddEdge
' 
' 
' /********************************************************************************/

#End Region

' ============================================================
' PriorNetwork.vb - 先验调控网络
' ============================================================
' 从 TF 注释和 TFBS motif 扫描得到的转录调控网络
' 作为 bnlearn 结构学习的白名单先验知识
' ============================================================

Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix

Namespace Core

    ''' <summary>
    ''' 转录调控关系
    ''' </summary>
    Public Class RegulatoryEdge

        ''' <summary>transcript factor protein/rna id.</summary>
        ''' <remarks>
        ''' 转录因子名称
        ''' </remarks>
        Public Property TF As String = ""

        ''' <summary>靶基因名称</summary>
        Public Property TargetGene As String = ""

        ''' <summary>调控类型（激活/抑制）</summary>
        Public Property RegulationType As Effector = Effector.Activator

        ''' <summary>置信度分数（0-1）</summary>
        Public Property Confidence As Double = 1.0

        ''' <summary>证据来源</summary>
        Public Property Evidence As String = ""

        Public Overrides Function ToString() As String
            Return String.Format("{0} → {1} ({2}, conf={3:F2})", TF, TargetGene, RegulationType.Description, Confidence)
        End Function

    End Class

    ''' <summary>
    ''' 先验调控网络
    ''' </summary>
    Public Class PriorNetwork

        ''' <summary>调控关系列表</summary>
        Public Property Edges As New List(Of RegulatoryEdge)()

        ''' <summary>所有转录因子名称</summary>
        Public Property TFNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>所有靶基因名称</summary>
        Public Property TargetNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>添加调控关系</summary>
        Public Sub AddEdge(tf As String, target As String, regType As Effector, confidence As Double, evidence As String)
            Edges.Add(New RegulatoryEdge() With {
                .TF = tf, .TargetGene = target,
                .RegulationType = regType, .Confidence = confidence, .Evidence = evidence})
            TFNames.Add(tf)
            TargetNames.Add(target)
        End Sub

        ''' <summary>获取指定靶基因的所有上游TF</summary>
        Public Function GetRegulators(targetGene As String) As List(Of RegulatoryEdge)
            Return Edges.Where(Function(e) String.Equals(e.TargetGene, targetGene, StringComparison.OrdinalIgnoreCase)).ToList()
        End Function

        ''' <summary>获取指定TF的所有靶基因</summary>
        Public Function GetTargets(tf As String) As List(Of RegulatoryEdge)
            Return Edges.Where(Function(e) String.Equals(e.TF, tf, StringComparison.OrdinalIgnoreCase)).ToList()
        End Function

        ''' <summary>转换为白名单边列表（使用基因表达矩阵的索引）</summary>
        Public Function ToWhitelist(geneNames As String()) As IEnumerable(Of (fromIdx%, toIdx%))
            Dim nameMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For i = 0 To geneNames.Length - 1
                nameMap(geneNames(i)) = i
            Next

            Dim wl As New List(Of (Integer, Integer))()
            For Each edge In Edges
                Dim fromIdx As Integer = -1, toIdx As Integer = -1
                nameMap.TryGetValue(edge.TF, fromIdx)
                nameMap.TryGetValue(edge.TargetGene, toIdx)
                If fromIdx >= 0 AndAlso toIdx >= 0 AndAlso fromIdx <> toIdx Then
                    wl.Add((fromIdx, toIdx))
                End If
            Next
            Return wl
        End Function

        ''' <summary>获取所有涉及的基因名</summary>
        Public Function GetAllGeneNames() As String()
            Dim names As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each e In Edges
                names.Add(e.TF)
                names.Add(e.TargetGene)
            Next
            Return names.ToArray()
        End Function

    End Class

End Namespace

