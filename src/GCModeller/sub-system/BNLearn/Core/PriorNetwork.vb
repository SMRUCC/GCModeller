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

Imports Microsoft.VisualBasic.Serialization.JSON

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

        Sub New()
        End Sub

        Sub New(copy As RegulatoryEdge)
            TF = copy.TF
            TargetGene = copy.TargetGene
            RegulationType = copy.RegulationType
            Confidence = copy.Confidence
            Evidence = copy.Evidence
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} → {1} ({2}, conf={3:F2})", TF, TargetGene, RegulationType.Description, Confidence)
        End Function

        Public Shared Function ParseEffector(type As String) As Effector
            If type.StringEmpty(, True) Then
                Return Effector.Activator
            End If

            Select Case type
                Case NameOf(Effector.Activator), "activation", "+", "↑" : Return Effector.Activator
                Case NameOf(Effector.Inhibitor), "repression", "inhibit", "suppress", "-", "↓" : Return Effector.Inhibitor
                Case "1" : Return Effector.Activator
                Case "2" : Return Effector.Inhibitor
                Case Else
                    Return Effector.Unknown
            End Select
        End Function

        Public Shared Function ParseConfidenceEnum(type As String) As Double
            If type.StringEmpty(, True) Then
                ' unknown confidence label
                Return 0
            End If

            Select Case type
                Case "High" : Return 0.95
                Case "Medium" : Return 0.7
                Case "Low" : Return 0.5
                Case Else
                    Return 0.1
            End Select
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

        Public Class Dto

            Public Property edges As RegulatoryEdge()
            Public Property TF As String()
            Public Property targets As String()

            Sub New()
            End Sub

            Sub New(prior As PriorNetwork)
                edges = prior.Edges.ToArray
                TF = prior.TFNames.ToArray
                targets = prior.TargetNames.ToArray
            End Sub

        End Class

        Sub New()
        End Sub

        Sub New(edges As IEnumerable(Of RegulatoryEdge))
            For Each edge As RegulatoryEdge In edges
                Call _Edges.Add(edge)
                Call TFNames.Add(edge.TF)
                Call TargetNames.Add(edge.TargetGene)
            Next
        End Sub

        ''' <summary>
        ''' 将当前的先验调控网络序列化为JSON文本，用于持久化存储。
        ''' </summary>
        ''' <param name="indent">
        ''' 是否输出带缩进格式的JSON文本，默认为 <see langword="True"/>，
        ''' 便于人工查阅与版本控制系统做diff。
        ''' </param>
        ''' <returns>JSON文本，可由 <see cref="ParseJson"/> 原样解析还原。</returns>
        Public Function ToJson(Optional indent As Boolean = True) As String
            Return New Dto(Me).GetJson(indent:=indent)
        End Function

        ''' <summary>
        ''' 从JSON文本之中解析还原先验调控网络对象。
        ''' </summary>
        ''' <param name="json">由 <see cref="ToJson"/> 生成的JSON文本。</param>
        ''' <returns>
        ''' 还原后的先验调控网络。输入为空白或 ``null`` 时返回空网络，
        ''' 而不是抛出异常，方便调用方对空文件容错。
        ''' </returns>
        Public Shared Function ParseJson(json As String) As PriorNetwork
            Dim dto As Dto = json.LoadJSON(Of Dto)
            Dim net As New PriorNetwork With {
                .Edges = New List(Of RegulatoryEdge)(dto.edges),
                .TargetNames = New HashSet(Of String)(dto.targets),
                .TFNames = New HashSet(Of String)(dto.TF)
            }

            Return net
        End Function

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

