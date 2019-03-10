#Region "Microsoft.VisualBasic::b09bc1ca1fa7e603a3047795cae771af, sub-system\FBA\FBA_DP\rFBA\rFBARModel.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'Public Class rFBARModel : Inherits FBA.lpSolveRModel

'    Protected Friend CellSystem As rFBA.DataModel.CellSystem

'#Region "data Cache for optimize the program performance"
'    ''' <summary>
'    ''' 用于<see cref="rFBARModel.fluxColumns"></see>的缓存数据
'    ''' </summary>
'    ''' <remarks></remarks>
'    Protected Friend _MetabolismFluxCounts As Integer
'    ''' <summary>
'    ''' 代谢组反应对象+转录调控对象的总数，用于函数<see cref="rFBARModel.getLowerbound"></see>
'    ''' </summary>
'    ''' <remarks></remarks>
'    Protected Friend _TotalFluxCounts As Integer

'    ReadOnly _MetaboliteConstraint As KeyValuePair(Of String, Double) = New KeyValuePair(Of String, Double)("=", "0")
'    ReadOnly _RegulatorsConstraint As KeyValuePair(Of String, Double) = New KeyValuePair(Of String, Double)(">=", "0")

'    ''' <summary>
'    ''' 每一次迭代计算后得到的流量值
'    ''' </summary>
'    ''' <remarks></remarks>
'    Protected Friend IterationFluxValue As Double()
'    Dim _UpdatedLowerBound As Double()
'    Dim _UpdatedUpperBound As Double()
'    Dim _Matrix As Double()()
'    Dim _ObjectiveFunction As Double()
'    Dim _ColumnNames As String()
'#End Region

'    Public Sub Initialize()
'        Call CellSystem.Initialize()

'        _MetabolismFluxCounts = CellSystem.MetabolismFluxs.Count
'        _TotalFluxCounts = CellSystem.MetabolismFluxs.Count + CellSystem.ExpressionRegulation.Count

'        Dim MatrixList As List(Of rFBA.DataModel.FluxObject) = New List(Of rFBA.DataModel.FluxObject)
'        Call MatrixList.AddRange(CellSystem.MetabolismFluxs)
'        Call MatrixList.AddRange(CellSystem.ExpressionRegulation)

'        _ColumnNames = (From item In MatrixList Select item.Identifier).ToArray

'        '先得到所有的代谢物列表
'        Dim Metabolites = New List(Of String)
'        For Each item In MatrixList
'            Call Metabolites.AddRange(item.Substrates)
'        Next
'        Metabolites = (From strId As String In Metabolites Select strId Distinct).AsList
'        _Matrix = (From Metabolite As String In Metabolites
'                   Let Generate = Function() As Double()
'                                      Dim ChunkBuffer As Double() = New Double(_TotalFluxCounts - 1) {}
'                                      For i As Integer = 0 To MatrixList.Count - 1
'                                          ChunkBuffer(i) = MatrixList(i).GetCoefficient(Metabolite)
'                                      Next

'                                      Return ChunkBuffer
'                                  End Function Select Generate()).ToArray

'        '初始化目标方程
'        Dim FunctionModel = CellSystem.ObjectiveFunctions
'        MyBase.CDirect = FunctionModel.Direction
'        _ObjectiveFunction = (From strid As String In Me._ColumnNames Let idx = Array.IndexOf(FunctionModel.Factors, strid) Let factor = If(idx < 0, 0.0R, 1.0R) Select factor).ToArray

'        Me._UpdatedLowerBound = (From item In MatrixList Select item.Lower_Bound).ToArray
'        Me._UpdatedUpperBound = (From item In MatrixList Select item.Upper_Bound).ToArray

'        Call InitlaizeAssociateGeneHandles()
'    End Sub

'    Private Sub InitlaizeAssociateGeneHandles()
'        For i As Integer = 0 To _MetabolismFluxCounts - 1
'            Dim GeneList = CellSystem.MetabolismFluxs(i).AssociatedRegulationGenes
'            If GeneList.Count = 1 AndAlso String.IsNullOrEmpty(GeneList.First.Identifier) Then
'                CellSystem.MetabolismFluxs(i).AssociatedRegulationGenes = New rFBA.DataModel.AssociatedGene() {}
'            Else
'                For j As Integer = 0 To GeneList.Count - 1
'                    Dim Gene = GeneList(j)
'                    Gene.Handle = Array.IndexOf(Me._ColumnNames, Gene.Identifier)
'                    Gene.RPKM = CellSystem.ExpressionRegulation(Gene.Handle - Me._MetabolismFluxCounts).Upper_Bound
'                Next
'            End If
'        Next
'    End Sub

'    Protected Friend Overrides Function fluxColumns() As String()
'        Return Me._ColumnNames
'    End Function

'    ''' <summary>
'    ''' 获取每一个代谢物的（行）的约束条件
'    ''' </summary>
'    ''' <param name="idx"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Protected Friend Overrides Function getConstraint(idx As String) As KeyValuePair(Of String, Double)
'        If idx < _MetabolismFluxCounts Then '在FBA模型矩阵的上半部分为代谢组，下半部分为表达调控模型
'            Return _MetaboliteConstraint
'        Else
'            Return _RegulatorsConstraint
'        End If
'    End Function

'    Protected Friend Overrides Function getLowerbound() As Double()
'        Return _UpdatedLowerBound
'    End Function

'    ''' <summary>
'    ''' 对于可逆代谢反应，根据代谢酶的表达量来计算约束，对于不可逆反应，则恒为零，对于表达调控也恒为零
'    ''' </summary>
'    ''' <remarks></remarks>
'    Friend Sub UpdateFluxConstraints()
'        Dim LowerBoundChunkBuffer As Double() = New Double(_TotalFluxCounts - 1) {}
'        Dim UpperBoundChunkBuffer As Double() = New Double(_TotalFluxCounts - 1) {}

'        Dim MetabolismFluxs = CellSystem.MetabolismFluxs

'        For i As Integer = 0 To Me._MetabolismFluxCounts - 1
'            Dim Flux = MetabolismFluxs(i)

'            '当有酶分子的时候，则进行修改，当没有的时候，则直接赋值
'            If Not Flux.AssociatedRegulationGenes.IsNullOrEmpty Then
'                Dim ExpressionSum As Double = 0

'                If Flux.Reversible Then
'                    For Each item In Flux.AssociatedRegulationGenes
'                        ExpressionSum += IterationFluxValue(item.Handle) / item.RPKM
'                    Next
'                    ExpressionSum /= Flux.AssociatedRegulationGenes.Count
'                    LowerBoundChunkBuffer(i) = ExpressionSum * Flux.Lower_Bound
'                    UpperBoundChunkBuffer(i) = ExpressionSum * Flux.Upper_Bound
'                Else
'                    UpperBoundChunkBuffer(i) = Flux.Upper_Bound
'                End If
'            Else
'                LowerBoundChunkBuffer(i) = Flux.Lower_Bound
'                UpperBoundChunkBuffer(i) = Flux.Upper_Bound
'            End If
'        Next

'        For i As Integer = _MetabolismFluxCounts To _TotalFluxCounts - 1
'            LowerBoundChunkBuffer(i) = 10 '本底表达水平
'            Dim Flux = CellSystem.ExpressionRegulation(i - _MetabolismFluxCounts)

'            If Not Flux.AssociatedRegulationGenes.IsNullOrEmpty Then
'                Dim UpperBoundValue As Double = 0

'                For Each item In Flux.AssociatedRegulationGenes
'                    UpperBoundValue += IterationFluxValue(item.Handle) / item.RPKM
'                Next

'                UpperBoundChunkBuffer(i) = Flux.Upper_Bound * UpperBoundValue
'            Else
'                UpperBoundChunkBuffer(i) = 10
'            End If
'        Next

'        Me._UpdatedLowerBound = LowerBoundChunkBuffer
'        Me._UpdatedUpperBound = UpperBoundChunkBuffer
'    End Sub

'    ''' <summary>
'    ''' 网络结构不应该有变化
'    ''' </summary>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Protected Friend Overrides Function getMatrix() As Double()()
'        Return _Matrix
'    End Function

'    ''' <summary>
'    ''' 目标方程也不会变
'    ''' </summary>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Protected Friend Overrides Function getObjectFunction() As Double()
'        Return _ObjectiveFunction
'    End Function

'    Protected Friend Overrides Function getUpbound() As Double()
'        Return _UpdatedUpperBound
'    End Function

'    Public Overrides Sub SetObjectiveFunc(factors() As String)
'        _ObjectiveFunction = (From strid As String In Me._ColumnNames Let idx = Array.IndexOf(factors, strid) Let factor = If(idx < 0, 0.0R, 1.0R) Select factor).ToArray
'    End Sub
'End Class
