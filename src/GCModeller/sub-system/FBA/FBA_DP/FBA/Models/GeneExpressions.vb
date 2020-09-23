#Region "Microsoft.VisualBasic::d863c44fde95545031942cadda74659d, sub-system\FBA\FBA_DP\FBA\Models\GeneExpressions.vb"

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

'Imports SMRUCC.genomics.Toolkits
'Imports SMRUCC.genomics.Toolkits.RNA_Seq
'Imports SMRUCC.genomics.Toolkits.RNA_Seq.dataExprMAT
'Imports Microsoft.VisualBasic.DocumentFormat.Csv

'Namespace Models

'    Public Class GeneExpressions : Inherits lpSolveRModel

'        Dim ChipData As MatrixFrame
'        Dim PccMatrix As PccMatrix

'        Private Shared ReadOnly Constraint_System_Unstable_Status As KeyValuePair(Of String, Double) = New KeyValuePair(Of String, Double)(">", 0)
'        Private Shared ReadOnly Constraint_System_Stablize_Status As KeyValuePair(Of String, Double) = New KeyValuePair(Of String, Double)("=", 0)

'        Dim Constraint As KeyValuePair(Of String, Double)
'        Dim ObjectiveFunction As String()
'        Dim PccCutoff As Double

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="ChipData"></param>
'        ''' <param name="SystemStableStatus"></param>
'        ''' <param name="ExperimentId">芯片数据里面的实验名称或者列的标号</param>
'        ''' <param name="ObjectiveFunction">假若本编号列表为空的话，则默认将目标方程设置为整个基因组</param>
'        ''' <remarks></remarks>
'        Sub New(ChipData As IO.File,
'                SystemStableStatus As Boolean,
'                ExperimentId As String,
'                Optional PccCutoff As Double = 0.65,
'                Optional ObjectiveFunction As String() = Nothing)

'            Call Me.New(ChipDataObject:=RNA_Seq.dataExprMAT.MatrixFrame.Load(ChipData),
'                        SystemStableStatus:=SystemStableStatus,
'                        ExperimentId:=ExperimentId, PccCutoff:=PccCutoff, ObjectiveFunction:=ObjectiveFunction)
'        End Sub

'        Sub New(ChipDataObject As MatrixFrame, SystemStableStatus As Boolean, ExperimentId As String, Optional PccCutoff As Double = 0.65, Optional ObjectiveFunction As String() = Nothing)
'            Me.ChipData = ChipDataObject
'            Me.PccMatrix = MatrixAPI.CreatePccMAT(ChipDataObject.GetOriginalMatrix, True)

'            Constraint = If(SystemStableStatus, Constraint_System_Stablize_Status, Constraint_System_Unstable_Status)
'            Call Me.ChipData.SetColumnAuto(ExperimentId)

'            If ObjectiveFunction.IsNullOrEmpty Then
'                Me.ObjectiveFunction = PccMatrix.lstGenes
'            Else
'                Me.ObjectiveFunction = ObjectiveFunction
'            End If

'            Me.PccCutoff = PccCutoff
'        End Sub

'        Protected Friend Overrides Function fluxColumns() As String()
'            Return PccMatrix.lstGenes
'        End Function

'        Protected Friend Overrides Function getConstraint(idx As String) As KeyValuePair(Of String, Double)
'            Return Constraint
'        End Function

'        Protected Friend Overrides Function getLowerbound() As Double()
'            Return New Double(PccMatrix.lstGenes.Count - 1) {}
'        End Function

'        ''' <summary>
'        ''' 对于正的Pcc值而言，其必须要大于阈值，否则为零，对于小于零的Pcc值而言，其只需要小于-0.6即可
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Protected Friend Overrides Function getMatrix() As Double()()
'            Dim LQuery = (From Line In PccMatrix.PccValues
'                          Select (From n As Double In Line.Values
'                                  Let GetValue = Function() As Double
'                                                     If n > 0 Then
'                                                         If n > Me.PccCutoff Then
'                                                             Return n
'                                                         Else
'                                                             Return 0
'                                                         End If
'                                                     Else
'                                                         If n < -0.6 Then
'                                                             Return n
'                                                         Else
'                                                             Return 0
'                                                         End If
'                                                     End If
'                                                 End Function Select GetValue()).ToArray).ToArray
'            Return LQuery
'        End Function

'        Protected Friend Overrides Function getObjectFunction() As Double()
'            Dim GeneIdlist As String() = PccMatrix.lstGenes
'            Dim LQuery = (From Id In GeneIdlist
'                          Let FUNC = Function() As Double
'                                         If Array.IndexOf(ObjectiveFunction, Id) > -1 Then
'                                             Return 1
'                                         Else
'                                             Return 0
'                                         End If
'                                     End Function Select FUNC()).ToArray
'            Return LQuery
'        End Function

'        Protected Friend Overrides Function getUpbound() As Double()
'            Dim GeneObjects As String() = PccMatrix.lstGenes
'            Dim LQuery = (From item In GeneObjects Select ChipData.GetValue(item)).ToArray
'            Return LQuery
'        End Function

'        Public Overrides Sub SetObjectiveFunc(factors() As String)
'            Me.ObjectiveFunction = factors
'        End Sub
'    End Class
'End Namespace
