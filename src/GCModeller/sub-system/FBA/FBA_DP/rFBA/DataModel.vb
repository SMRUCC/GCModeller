#Region "Microsoft.VisualBasic::9a9f86ca422b4aaa52c961b06fdc61f8, sub-system\FBA\FBA_DP\rFBA\DataModel.vb"

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

    '         Class FluxObject
    ' 
    '             Properties: AssociatedRegulationGenes, Handle, Identifier, Left, Lower_Bound
    '                         Reversible, Right, Substrates, Upper_Bound
    ' 
    '             Function: GetCoefficient
    ' 
    '             Sub: Assign
    ' 
    '         Class ObjectiveFunction
    ' 
    '             Properties: Direction, Factors
    ' 
    '         Class AssociatedGene
    ' 
    '             Properties: Effectors, Handle, Identifier, RPKM
    ' 
    '             Function: ToString
    ' 
    '             Sub: Assign
    ' 
    '         Class CellSystem
    ' 
    '             Properties: ExpressionRegulation, MetabolismFluxs, MetabolismModel, Metabolites, ObjectiveFunctionModel
    '                         ObjectiveFunctions, TranscriptionModel
    ' 
    '             Sub: Initialize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace rFBA

    Namespace DataModel

        ''' <summary>
        ''' 一个代谢反应对象或者转录调控过程
        ''' </summary>
        ''' <remarks></remarks>
        Public Class FluxObject : Implements IAddressOf, INamedValue

            Public Property Left As KeyValuePair(Of Double, String)()
            Public Property Right As KeyValuePair(Of Double, String)()
            Public Property Reversible As Boolean
            Public Property Lower_Bound As Double
            Public Property Upper_Bound As Double

            Public Property Handle As Integer Implements IAddressOf.Address

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
                Dim LQuery = (From item In Left Where String.Equals(Metabolite, item.Value) Select item).ToArray
                If LQuery.IsNullOrEmpty Then
                    LQuery = (From item In Right Where String.Equals(Metabolite, item.Value) Select item).ToArray
                    If LQuery.IsNullOrEmpty Then
                        Return 0
                    Else
                        Return LQuery.First.Key
                    End If
                Else
                    Return LQuery.First.Key * -1
                End If
            End Function

            Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
                Me.Handle = address
            End Sub

            Public ReadOnly Property Substrates As String()
                Get
                    Dim List As List(Of String) = New List(Of String)
                    Call List.AddRange((From item In Left Select item.Value).ToArray)
                    Call List.AddRange((From item In Right Select item.Value).ToArray)

                    Return List.ToArray
                End Get
            End Property

            Public Property Identifier As String Implements INamedValue.Key
        End Class

        Public Class ObjectiveFunction
            Public Property Direction As String
            <CollectionAttribute("Factors")> Public Property Factors As String()
        End Class

        Public Class AssociatedGene : Implements IAddressOf, INamedValue
            Public Property RPKM As Double
            Public Property Identifier As String Implements INamedValue.Key
            Public Property Handle As Integer Implements IAddressOf.Address

            ''' <summary>
            ''' 仅适用于调控过程，酶促反应过程不会使用到本属性{Handle, Id}
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Effectors As KeyValuePair(Of Integer, String)()

            Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
                Me.Handle = address
            End Sub

            Public Overrides Function ToString() As String
                Return Identifier
            End Function
        End Class

        ''' <summary>
        ''' 本计算模型中的所构建的细胞中的基本系统：代谢组和转录组
        ''' </summary>
        ''' <remarks></remarks>
        Public Class CellSystem : Inherits ModelBaseType

            <XmlIgnore> Public Property Metabolites As String()
            <XmlIgnore> Public Property MetabolismFluxs As FluxObject()
            <XmlIgnore> Public Property ExpressionRegulation As FluxObject()

            Public Property MetabolismModel As Href
            Public Property TranscriptionModel As Href
            Public Property ObjectiveFunctionModel As Href

            <XmlIgnore> Public Property ObjectiveFunctions As DataModel.ObjectiveFunction

            Public Sub Initialize()
                MetabolismFluxs = (From item As ModelReader.MetabolismFlux
                                   In MetabolismModel.Value.AsLinq(Of ModelReader.MetabolismFlux)
                                   Select item.CreateObject).ToArray
                ExpressionRegulation = (From item As ModelReader.GeneExpression
                                        In Me.TranscriptionModel.Value.AsLinq(Of ModelReader.GeneExpression)
                                        Select item.CreateObject).ToArray
                ObjectiveFunctions = Me.ObjectiveFunctionModel.Value.AsLinq(Of ObjectiveFunction).FirstOrDefault
            End Sub

            Public Overloads Shared Widening Operator CType(sPath As String) As CellSystem
                Return sPath.LoadXml(Of CellSystem)()
            End Operator
        End Class
    End Namespace
End Namespace
