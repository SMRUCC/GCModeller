#Region "Microsoft.VisualBasic::6ab20a029ac976f53348fa92421bafa0, engine\IO\GCMarkupLanguage\v2\Xml\Metabolism\Reaction.vb"

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

'   Total Lines: 219
'    Code Lines: 144 (65.75%)
' Comment Lines: 43 (19.63%)
'    - Xml Docs: 97.67%
' 
'   Blank Lines: 32 (14.61%)
'     File Size: 8.07 KB


'     Class Transportation
' 
'         Properties: enzymes, guid, membrane
' 
'         Function: ToString
' 
'     Class ReactionGroup
' 
'         Properties: enzymatic, none_enzymatic, size, transportation
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: CompoundLinks, GenericEnumerator
' 
'     Class Reaction
' 
'         Properties: baseline, bounds, compartment, ec_number, equation
'                     ID, is_enzymatic, name, note, product
'                     substrate
' 
'         Function: CheckTransportation, GenericEnumerator, ToString
' 
'     Class CompoundFactor
' 
'         Properties: cid, compartment, compound, factor
' 
'         Constructor: (+3 Overloads) Sub New
'         Function: factorString, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel

Namespace v2

    ''' <summary>
    ''' the reaction graph model
    ''' </summary>
    <XmlType("reaction", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Reaction : Implements INamedValue, Enumeration(Of CompoundFactor)

        ''' <summary>
        ''' unique reference id of current reaction link
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlElement> Public Property name As String
        <XmlElement> Public Property note As String
        ''' <summary>
        ''' 这个反应模型是否是需要酶促才会发生了生化反应过程？
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property is_enzymatic As Boolean
        ''' <summary>
        ''' [forward, reverse] boundary of the reaction speed
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property bounds As Double()
        <XmlAttribute> Public Property baseline As Double()

        <XmlElement> Public Property ec_number As String()
        ''' <summary>
        ''' the compartment location of the reaction
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property compartment As String()
        <XmlElement> Public Property substrate As CompoundFactor()
        <XmlElement> Public Property product As CompoundFactor()

        <XmlElement> Public Property gibbs As Double

        ''' <summary>
        ''' the debug view of the current equation model
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property equation As String
            Get
                Dim left As String = substrate.Select(Function(a) a.factorString).JoinBy(" + ")
                Dim right As String = product.Select(Function(a) a.factorString).JoinBy(" + ")
                Dim arrow As String = If(CheckReversible(),
                    EquationBuilder.EQUATION_DIRECTIONS_REVERSIBLE,
                    EquationBuilder.EQUATION_DIRECTIONS_INREVERSIBLE)

                Return left & " " & arrow & " " & right
            End Get
        End Property

        Public Function CheckTransportation() As Boolean
            Return substrate.Any(Function(s) product.Any(Function(a) a.compound = s.compound))
        End Function

        Public Function CheckReversible() As Boolean
            Dim lb As Double = bounds(0)
            Dim rb As Double = bounds(1)

            If lb > 0 AndAlso rb > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"({ID}: {name}) {equation}"
        End Function

        Public Function BuildModel() As MetabolicReaction
            Dim left = substrate.Select(Function(c) New CompoundSpecieReference(c, c.compartment)).ToArray
            Dim right = product.Select(Function(c) New CompoundSpecieReference(c, c.compartment)).ToArray

            Return New MetabolicReaction With {
                .description = note,
                .ECNumbers = ec_number,
                .gibbs = gibbs,
                .id = ID,
                .is_reversible = CheckReversible(),
                .is_spontaneous = ec_number.IsNullOrEmpty,
                .name = name,
                .left = left,
                .right = right
            }
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of CompoundFactor) Implements Enumeration(Of CompoundFactor).GenericEnumerator
            For Each c As CompoundFactor In substrate.SafeQuery
                Yield c
            Next
            For Each c As CompoundFactor In product.SafeQuery
                Yield c
            Next
        End Function
    End Class

End Namespace
