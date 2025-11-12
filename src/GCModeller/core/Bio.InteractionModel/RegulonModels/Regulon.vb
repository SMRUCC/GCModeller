#Region "Microsoft.VisualBasic::aba995e442f245093d0385c3e0cb1bba, core\Bio.InteractionModel\RegulonModels\Regulon.vb"

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

'   Total Lines: 109
'    Code Lines: 52 (47.71%)
' Comment Lines: 39 (35.78%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 18 (16.51%)
'     File Size: 3.50 KB


'     Interface IRegulon
' 
'         Properties: RegulatedGenes, TFlocusId
' 
'     Interface IRegulatorRegulation
' 
'         Properties: LocusId, Regulators
' 
'     Class RegulatorRegulation
' 
'         Properties: LocusId, Regulators
' 
'     Interface ISpecificRegulation
' 
'         Properties: LocusId, Regulator
' 
'     Class Regulon
' 
'         Properties: Id, RegulatedGenes, Regulator
' 
'     Interface IRegulationDatabase
' 
'         Function: GetRegulatesSites, GetRegulators, IsRegulates, listRegulators
' 
'     Structure RelationshipScore
' 
'         Properties: InteractorA, InteractorB, Score, Type
' 
'         Function: GetConnectedId, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph

Namespace Regulon

    ''' <summary>
    ''' Regulon的模型的抽象
    ''' </summary>
    Public Interface IRegulon

        Property TFlocusId As String
        Property RegulatedGenes As String()

    End Interface

    ''' <summary>
    ''' 调控作用的抽象
    ''' </summary>
    Public Interface IRegulatorRegulation

        ''' <summary>
        ''' Gene id
        ''' </summary>
        ''' <returns></returns>
        Property LocusId As String
        ''' <summary>
        ''' Regulators gene id
        ''' </summary>
        ''' <returns></returns>
        Property Regulators As String()

    End Interface

    Public Class RegulatorRegulation
        Implements IRegulatorRegulation

        Public Property LocusId As String Implements IRegulatorRegulation.LocusId
        Public Property Regulators As String() Implements IRegulatorRegulation.Regulators
    End Class

    ''' <summary>
    ''' 1对1的特定的基因对基因的调控模型
    ''' </summary>
    Public Interface ISpecificRegulation
        Property LocusId As String
        Property Regulator As String
    End Interface

    Public MustInherit Class Regulon : Implements IRegulon

        Public Property Id As String
        Public Property RegulatedGenes As String() Implements IRegulon.RegulatedGenes
        Public Property Regulator As String Implements IRegulon.TFlocusId
    End Class

    ''' <summary>
    ''' 用于生成调控数据的数据库的接口
    ''' </summary>
    Public Interface IRegulationDatabase
        ''' <summary>
        ''' 得到数据库之中的所有的调控因子的编号
        ''' </summary>
        ''' <returns></returns>
        Function listRegulators() As String()
        Function IsRegulates(regulator As String, site As String) As Boolean
        Function GetRegulators(site As String) As String()
        Function GetRegulatesSites(regulator As String) As String()
    End Interface

    ''' <summary>
    ''' 带有分值的互做关系
    ''' </summary>
    Public Structure RelationshipScore
        Implements IInteraction
        Implements INetworkEdge

        Public Property Type As String Implements INetworkEdge.Interaction

        ''' <summary>
        ''' 通常为Regulator
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InteractorA As String Implements IInteraction.source
        ''' <summary>
        ''' 通常为目标调控对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InteractorB As String Implements IInteraction.target
        Public Property Score As Double Implements INetworkEdge.value

        Public Function GetConnectedId(Id As String) As String
            If String.Equals(InteractorA, Id) Then
                Return InteractorB
            ElseIf String.Equals(InteractorB, Id) Then
                Return InteractorA
            Else
                Return ""
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"{InteractorA}  ({Type}, {Score})    {InteractorB}"
        End Function
    End Structure
End Namespace
