#Region "Microsoft.VisualBasic::c98fba97eeb4268adbfb8ccf266c59a6, engine\IO\GCTabular\DataModels\DataModel.vb"

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

    '     Class FluxObject
    ' 
    '         Properties: AssociatedRegulationGenes, Identifier, K1, K2, LeftSides
    '                     Lower_Bound, Reversible, RightSide, Substrates, Upper_Bound
    ' 
    '         Function: GetCoefficient
    ' 
    '     Class AssociatedGene
    ' 
    '         Properties: Effectors, Identifier, RPKM
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace DataModel

    ''' <summary>
    ''' 一个代谢反应对象或者转录调控过程
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FluxObject : Implements INamedValue
        Implements IEquation(Of MetaCyc.Schema.Metabolism.Compound)

        Public Property Lower_Bound As Double
        Public Property Upper_Bound As Double
        Public Property Identifier As String Implements INamedValue.Key
        Public Property LeftSides As MetaCyc.Schema.Metabolism.Compound() Implements IEquation(Of MetaCyc.Schema.Metabolism.Compound).Reactants
        Public Property Reversible As Boolean Implements IEquation(Of MetaCyc.Schema.Metabolism.Compound).Reversible
        Public Property RightSide As MetaCyc.Schema.Metabolism.Compound() Implements IEquation(Of MetaCyc.Schema.Metabolism.Compound).Products

        Public Property K1 As Double
        Public Property K2 As Double

        ''' <summary>
        ''' 催化本反应过程的基因或者调控因子(列)，请注意，由于在前半部分为代谢流对象，故而Key的值不是从零开始的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AssociatedRegulationGenes As AssociatedGene()

        ''' <summary>
        ''' Left为消耗，负值；Right为合成项，正值，当不存在的时候返回零
        ''' </summary>
        ''' <param name="Metabolite"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCoefficient(Metabolite As String) As Double
            Dim LQuery = (From item In LeftSides Where String.Equals(Metabolite, item.Identifier) Select item).ToArray
            If LQuery.IsNullOrEmpty Then
                LQuery = (From item In RightSide Where String.Equals(Metabolite, item.Identifier) Select item).ToArray
                If LQuery.IsNullOrEmpty Then
                    Return 0
                Else
                    Return LQuery.First.StoiChiometry
                End If
            Else
                Return LQuery.First.StoiChiometry * -1
            End If
        End Function

        Public ReadOnly Property Substrates As String()
            Get
                Dim List As List(Of String) = New List(Of String)
                Call List.AddRange((From item In LeftSides Select item.Identifier).ToArray)
                Call List.AddRange((From item In RightSide Select item.Identifier).ToArray)

                Return List.ToArray
            End Get
        End Property
    End Class

    Public Class AssociatedGene : Implements INamedValue

        Public Property RPKM As Double
        Public Property Identifier As String Implements INamedValue.Key

        ''' <summary>
        ''' 仅适用于调控过程，酶促反应过程不会使用到本属性{Handle, Id}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Effectors As KeyValuePair(Of Integer, String)()

        Public Overrides Function ToString() As String
            Return Identifier
        End Function
    End Class
End Namespace
