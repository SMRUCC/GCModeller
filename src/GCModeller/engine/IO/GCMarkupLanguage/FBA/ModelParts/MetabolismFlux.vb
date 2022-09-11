#Region "Microsoft.VisualBasic::0a394237b6f9e1e622e93feb3d58de4b, GCModeller\engine\IO\GCMarkupLanguage\FBA\ModelParts\MetabolismFlux.vb"

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

    '   Total Lines: 72
    '    Code Lines: 48
    ' Comment Lines: 12
    '   Blank Lines: 12
    '     File Size: 3.19 KB


    '     Class MetabolismFlux
    ' 
    '         Properties: Get_LOWER, Get_UPPER, GetObjCoefficient, Identifier, LOWER_BOUND
    '                     Name, ObjectiveCoefficient, Products, Reactants, Reversible
    '                     UPPER_BOUND
    ' 
    '         Function: Convert, GetStoichiometry, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Namespace FBACompatibility

    Public Class MetabolismFlux : Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference)
        <XmlAttribute> Public Property Identifier As String Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Key
        <XmlAttribute> Public Property UPPER_BOUND As Double
        <XmlAttribute> Public Property LOWER_BOUND As Double
        <XmlAttribute> Public Property ObjectiveCoefficient As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0}  <- {1}, {2} ->", Identifier, LOWER_BOUND, UPPER_BOUND)
        End Function

        Public Function GetStoichiometry(Metabolite As String) As Double Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).GetStoichiometry
            Throw New NotImplementedException("This is a not necessary method")
        End Function

        Public ReadOnly Property Get_LOWER As Double Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).LOWER_BOUND
            Get
                Return LOWER_BOUND
            End Get
        End Property

        Public Property Name As String Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Name

        Public ReadOnly Property GetObjCoefficient As Integer Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).ObjectiveCoefficient
            Get
                Return ObjectiveCoefficient
            End Get
        End Property

        Public ReadOnly Property Get_UPPER As Double Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).UPPER_BOUND
            Get
                Return UPPER_BOUND
            End Get
        End Property

        Public Shared Function Convert(Flux As FLuxBalanceModel.I_ReactionModel(Of speciesReference)) As MetabolismFlux
            Return New FBACompatibility.MetabolismFlux With {
                .Identifier = Flux.Key,
                .LOWER_BOUND = Flux.LOWER_BOUND,
                .UPPER_BOUND = Flux.UPPER_BOUND,
                .Name = Flux.Name,
                .ObjectiveCoefficient = Flux.ObjectiveCoefficient
            }
        End Function

        ''' <summary>
        ''' Not implement null reference property.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore>
        Public Property Products As speciesReference() Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Products

        ''' <summary>
        ''' Not implement null reference property.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore>
        Public Property Reactants As speciesReference() Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Reactants

        Public Property Reversible As Boolean Implements IEquation(Of speciesReference).Reversible
    End Class
End Namespace
