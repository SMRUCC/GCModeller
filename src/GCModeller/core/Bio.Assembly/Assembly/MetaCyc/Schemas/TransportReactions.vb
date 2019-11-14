#Region "Microsoft.VisualBasic::5de593130ab45a59330857717d6d6d00, core\Bio.Assembly\Assembly\MetaCyc\Schemas\TransportReactions.vb"

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

    '     Class TransportReaction
    ' 
    '         Properties: ECNumber, LEFT, PrefixTransportReactionTypes, Reversible, RIGHT
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateEquatopnExpression, GetSubstrates, GetTransportReactionExpasyEntries, GetUniqueId
    '         Class CompoundSpecies
    ' 
    '             Properties: Compartment, Identifier, StoiChiometry
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: GetAttributeValue, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.Expasy.Database
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.Schema

    Public Class TransportReaction : Inherits MetaCyc.File.DataFiles.Slots.Reaction
        Implements IEquation(Of CompoundSpecies)

        Public Shared ReadOnly Property PrefixTransportReactionTypes As String() = {
            "TR-11", "TR-12", "TR-13", "TR-14", "TR-19", "TR-20", "TR-21",
            "Transport-Reactions"
        }

        Public Class CompoundSpecies : Inherits MetaCyc.Schema.PropertyAttributes
            Implements ComponentModel.EquaionModel.ICompoundSpecies

            Dim _Compartment As String

            Public ReadOnly Property Compartment As String
                Get
                    Return _Compartment
                End Get
            End Property

            Public Property Identifier As String Implements INamedValue.Key
                Get
                    Return MyBase.PropertyValue
                End Get
                Set(value As String)
                    MyBase.PropertyValue = value
                End Set
            End Property

            ''' <summary>
            ''' <see cref="TransportReaction">转运反应对象</see>的<see cref="TransportReaction.Left">左</see><see cref="TransportReaction.Right">右</see>两边的原始属性值
            ''' </summary>
            ''' <param name="strValue"></param>
            ''' <remarks></remarks>
            Sub New(strValue As String)
                Call MyBase.New(strValue)
                _Compartment = GetAttributeValue("COMPARTMENT")
                MyBase.PropertyValue = MetaCyc.File.DataFiles.Reactions.Trim(Identifier)
            End Sub

            Public Overrides Function ToString() As String
                If String.IsNullOrEmpty(_Compartment) Then
                    Return Identifier
                Else
                    Return String.Format("[{0}] {1}", _Compartment, Identifier)
                End If
            End Function

            Public Function GetAttributeValue(Key As String) As String
                Dim LQuery = (From item In _Attributes Where String.Equals(Key, item.Key) Select item.Value).ToArray
                If LQuery.IsNullOrEmpty Then
                    Return ""
                Else
                    Return LQuery.First
                End If
            End Function

            Public Property StoiChiometry As Double Implements ComponentModel.EquaionModel.ICompoundSpecies.StoiChiometry
        End Class

#Region "Shadows Property"

        Dim _ECNumber As MetaCyc.Schema.PropertyAttributes
        Dim _Left, _Right As CompoundSpecies()

        Public Shadows Property ECNumber As MetaCyc.Schema.PropertyAttributes
            Get
                Return _ECNumber
            End Get
            Set(value As MetaCyc.Schema.PropertyAttributes)
                _ECNumber = value
                MyBase.ECNumber = _ECNumber.PropertyValue
            End Set
        End Property
        Public Shadows Property LEFT As CompoundSpecies() Implements IEquation(Of CompoundSpecies).Reactants
            Get
                Return _Left
            End Get
            Set(value As CompoundSpecies())
                _Left = value
                MyBase.Left = (From item In _Left Select item.Identifier).AsList
            End Set
        End Property
        Public Shadows Property RIGHT As CompoundSpecies() Implements IEquation(Of CompoundSpecies).Products
            Get
                Return _Right
            End Get
            Set(value As CompoundSpecies())
                _Right = value
                MyBase.Right = (From item In _Right Select item.Identifier).AsList
            End Set
        End Property

        Public Shadows Property Reversible As Boolean = False Implements IEquation(Of CompoundSpecies).Reversible
#End Region

        Public Function CreateEquatopnExpression() As String
            Dim Model As New Equation With {
                .Reactants = (From item In _Left
                              Let GetUniqueId = Function() As String
                                                    If String.IsNullOrEmpty(item.Compartment) Then
                                                        Return item.Identifier
                                                    Else
                                                        Return String.Format("{0} [^COMPARTMENT - {1}]", item.Identifier, item.Compartment)
                                                    End If
                                                End Function
                              Select New CompoundSpecieReference With {.ID = GetUniqueId(), .StoiChiometry = item.StoiChiometry}).ToArray,
                .Products = (From x As CompoundSpecies In _Right
                             Select New CompoundSpecieReference With {
                                  .ID = GetUniqueId(x),
                                  .StoiChiometry = x.StoiChiometry}).ToArray,
                                  .Reversible = Reversible
            }
            Return EquationBuilder.ToString(Model)
        End Function

        Private Shared Function GetUniqueId(x As CompoundSpecies) As String
            If String.IsNullOrEmpty(x.Compartment) Then
                Return x.Identifier
            Else
                Return String.Format("{0} [^COMPARTMENT - {1}]", x.Identifier, x.Compartment)
            End If
        End Function

        Public Function GetSubstrates() As CompoundSpecies()
            Dim List As List(Of CompoundSpecies) = New List(Of CompoundSpecies)
            Call List.AddRange(LEFT)
            Call List.AddRange(RIGHT)

            Return List.ToArray
        End Function

        Sub New(Reaction As MetaCyc.File.DataFiles.Slots.Reaction)
            Call Reaction.CopyTo(Me)

            MyBase.Left = Reaction.Left
            MyBase.Right = Reaction.Right
            Me.LEFT = (From strValue As String In MyBase.Left Select New CompoundSpecies(strValue)).ToArray
            Me.RIGHT = (From strValue As String In MyBase.Right Select New CompoundSpecies(strValue)).ToArray
            Me.ECNumber = New PropertyAttributes(Reaction.ECNumber)
            MyBase.ECNumber = Reaction.ECNumber
            MyBase.EnzymaticReaction = Reaction.EnzymaticReaction
        End Sub

        Public Shared Function GetTransportReactionExpasyEntries(Transports As TransportReaction(), Expasy As NomenclatureDB) As KeyValuePair(Of String, String())()
            Dim ECNumberLQuery = (From item In Transports
                                  Let ec = item.ECNumber.PropertyValue.Replace("EC-", "").Trim
                                  Where Not String.IsNullOrEmpty(ec)
                                  Select ec
                                  Distinct).ToArray
            Dim LQuery = (From Id As String In ECNumberLQuery
                          Let entries = Expasy.GetSwissProtEntries(ECNumber:=Id, Strict:=False)
                          Where Not entries.IsNullOrEmpty
                          Select New KeyValuePair(Of String, String())(Id, entries)).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
