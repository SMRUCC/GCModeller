#Region "Microsoft.VisualBasic::d26834f9fbfe0e8ac450f01110046644, GCModeller\models\SBML\SBML\Level2\FluxModel\Reaction.vb"

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

    '   Total Lines: 94
    '    Code Lines: 70
    ' Comment Lines: 12
    '   Blank Lines: 12
    '     File Size: 4.42 KB


    '     Class Reaction
    ' 
    '         Properties: id, kineticLaw, LowerBound, name, Notes
    '                     ObjectiveCoefficient, Products, Reactants, reversible, UpperBound
    ' 
    '         Function: __equals, GetCoEfficient, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Model.SBML.Components
Imports SMRUCC.genomics.Model.SBML.FLuxBalanceModel.IFBA
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Level2.Elements

    'http://sbml.org/Software/libSBML/docs/java-api/org/sbml/libsbml/Reaction.html

    ''' <summary>
    ''' A reaction represents any transformation, transport or binding process, typically a chemical reaction, 
    ''' that can change the quantity of one or more species. In SBML, a reaction is defined primarily in terms 
    ''' of the participating reactants and products (and their corresponding stoichiometries), along with 
    ''' optional modifier species, an optional rate at which the reaction takes place, and optional parameters.
    ''' </summary>
    ''' <remarks></remarks>
    <XmlType("reaction")> Public Class Reaction : Inherits Equation(Of speciesReference)
        Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference)
        Implements IReadOnlyId

        <XmlIgnore()> Public Handle As Integer

        <Escaped> <XmlAttribute()>
        Public Overrides Property id As String Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Key, IReadOnlyId.Identity

        ''' <summary>
        ''' Name property is the UniqueId in the MetaCyc database.(reactions.dat)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute()> Public Property name As String Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Name
        <XmlAttribute()> Public Overrides Property reversible As Boolean Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Reversible
        <XmlArray("listOfReactants")> Public Overrides Property Reactants As speciesReference() Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Reactants
            Get
                Return MyBase.Reactants
            End Get
            Set(value As speciesReference())
                MyBase.Reactants = value
            End Set
        End Property
        <XmlArray("listOfProducts")> Public Overrides Property Products As speciesReference() Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Products
            Get
                Return MyBase.Products
            End Get
            Set(value As speciesReference())
                MyBase.Products = value
            End Set
        End Property
        Public Property kineticLaw As kineticLaw
        <XmlElement("notes")> Public Property Notes As Notes

        Public ReadOnly Property LowerBound As Double Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).LOWER_BOUND
            Get
                Dim param = kineticLaw.GetParameter(LOWER_BOUND)
                If param Is Nothing Then
                    Return 0
                Else
                    Return param.value
                End If
            End Get
        End Property

        Public ReadOnly Property UpperBound As Double Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).UPPER_BOUND
            Get
                Dim param = kineticLaw.GetParameter(UPPER_BOUND)
                If param Is Nothing Then
                    Return 0
                Else
                    Return param.value
                End If
            End Get
        End Property

        Public Overrides Function GetCoEfficient(ID As String) As Double Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).GetStoichiometry
            Return MyBase.GetCoEfficient(ID)
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]{1}; reversible={2}", id, name, reversible)
        End Function

        Protected Overrides Function __equals(a As speciesReference, b As speciesReference, strict As Boolean) As Object
            Return Equivalence.Equals(a, b, strict)
        End Function

        Public ReadOnly Property ObjectiveCoefficient As Integer Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).ObjectiveCoefficient
            Get
                Return kineticLaw.ObjectiveCoefficient
            End Get
        End Property
    End Class
End Namespace
