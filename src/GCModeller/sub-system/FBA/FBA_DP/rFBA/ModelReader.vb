#Region "Microsoft.VisualBasic::1e4ca1898d532066ab4ba9862c09583a, sub-system\FBA\FBA_DP\rFBA\ModelReader.vb"

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

    '     Class MetabolismFlux
    ' 
    '         Properties: AssociatedEnzymeGenes, CommonName, Equation, Lower_Bound, UniqueId
    '                     Upper_Bound
    ' 
    '         Function: CreateObject, ToString
    ' 
    '     Class GeneExpression
    ' 
    '         Properties: AccessionId, BasalExpression, Regulators, RPKM
    ' 
    '         Function: CreateObject
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace ModelReader
    Public Class MetabolismFlux
        Public Property Equation As String
        <Column("Lower Bound")> Public Property Lower_Bound As Double
        <Column("Upper Bound")> Public Property Upper_Bound As Double
        Public Property CommonName As String

        ''' <summary>
        ''' 催化本反应过程的基因或者调控因子(列)，请注意，由于在前半部分为代谢流对象，故而Key的值不是从零开始的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <CollectionAttribute("Enzymes")> Public Property AssociatedEnzymeGenes As String()

        Public Property UniqueId As String

        Public Function CreateObject() As rFBA.DataModel.FluxObject
            Dim FluxObject As rFBA.DataModel.FluxObject = New rFBA.DataModel.FluxObject
            FluxObject.Identifier = UniqueId
            FluxObject.Upper_Bound = Upper_Bound
            FluxObject.Lower_Bound = Lower_Bound
            FluxObject.AssociatedRegulationGenes = (From strId As String In AssociatedEnzymeGenes Select New rFBA.DataModel.AssociatedGene With {.Identifier = strId}).ToArray

            '下面开始解析反应方程
            'Reversible:  <==>
            'Not Reversible:  -->
            Dim Tokens As String()

            If InStr(Equation, "<==>") Then
                FluxObject.Reversible = True
                Tokens = Strings.Split(Me.Equation, "<==>")
            Else
                FluxObject.Reversible = False
                Tokens = Strings.Split(Me.Equation, "-->")
            End If
            FluxObject.Left = (From strData As String In Strings.Split(Tokens(0), " + ") Let T = strData.Split Select New KeyValuePair(Of Double, String)(Val(T(0)), T(1))).ToArray
            FluxObject.Right = (From strData As String In Strings.Split(Tokens(1), " + ") Let T = strData.Split Select New KeyValuePair(Of Double, String)(Val(T(0)), T(1))).ToArray

            Return FluxObject
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}  [ {1} ]", UniqueId, Equation)
        End Function
    End Class

    Public Class GeneExpression
        ''' <summary>
        ''' RegulatorId|Pcc|{Effectors, ...}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <CollectionAttribute("Regulators")> Public Property Regulators As String()
        <Column("RPKM")> Public Property RPKM As Double
        Public Property AccessionId As String
        <Column("Basal Expression")>
        Public Property BasalExpression As Double

        ''' <summary>
        ''' 需要在后续的步骤中将Regulator筛选出来
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateObject() As rFBA.DataModel.FluxObject
            Dim FluxObject As rFBA.DataModel.FluxObject = New rFBA.DataModel.FluxObject
            FluxObject.Lower_Bound = BasalExpression
            FluxObject.Upper_Bound = RPKM
            FluxObject.Identifier = AccessionId
            FluxObject.Reversible = False

            Dim LQuery = (From strData As String In Regulators
                          Let Generate = Function() As KeyValuePair(Of Double, String)()
                                             If String.IsNullOrEmpty(strData.Trim) Then
                                                 Return New KeyValuePair(Of Double, String)() {}
                                             Else
                                                 Dim Tokens As String() = strData.Split(CChar("|"))
                                                 Dim Effectors As String() = Strings.Split(Tokens.Last, ", ")
                                                 Dim Pcc As Double = Val(Tokens(1))
                                                 Dim ChunkBuffer As KeyValuePair(Of Double, String)() = New KeyValuePair(Of Double, String)(Effectors.Count) {}
                                                 ChunkBuffer(0) = New KeyValuePair(Of Double, String)(Pcc, Tokens(0))
                                                 For i As Integer = 0 To Effectors.Count - 1
                                                     ChunkBuffer(i + 1) = New KeyValuePair(Of Double, String)(Pcc, Effectors(i))
                                                 Next

                                                 Return ChunkBuffer
                                             End If
                                         End Function Select Generate()).ToArray
            Dim List As List(Of KeyValuePair(Of Double, String)) = New List(Of KeyValuePair(Of Double, String))
            For Each item In LQuery
                Call List.AddRange(item)
            Next
            FluxObject.Left = (From item As KeyValuePair(Of Double, String) In List Where item.Key < 0 Select item).ToArray
            FluxObject.Right = (From item As KeyValuePair(Of Double, String) In List Where item.Key > 0 Select item).ToArray

            Return FluxObject
        End Function
    End Class
End Namespace
