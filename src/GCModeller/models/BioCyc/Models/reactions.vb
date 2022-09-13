#Region "Microsoft.VisualBasic::1e2df3c5e1cc0769102b2f7588a55a25, GCModeller\models\BioCyc\Models\reactions.vb"

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

    '   Total Lines: 79
    '    Code Lines: 73
    ' Comment Lines: 1
    '   Blank Lines: 5
    '     File Size: 3.15 KB


    ' Class reactions
    ' 
    '     Properties: atomMappings, cannotBalance, ec_number, enzymaticReaction, equation
    '                 gibbs0, inPathway, left, orphan, physiologicallyRelevant
    '                 reactionBalanceStatus, reactionDirection, reactionList, reactionLocations, right
    '                 signal, species, spontaneous, systematicName
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

<Xref("reactions.dat")>
Public Class reactions : Inherits Model

    <AttributeField("EC-NUMBER")>
    Public Property ec_number As ECNumber
    <AttributeField("ENZYMATIC-REACTION")>
    Public Property enzymaticReaction As String()
    <AttributeField("GIBBS-0")>
    Public Property gibbs0 As Double
    <AttributeField("IN-PATHWAY")>
    Public Property inPathway As String()
    <AttributeField("LEFT")>
    Public Property left As CompoundSpecieReference()
    <AttributeField("RIGHT")>
    Public Property right As CompoundSpecieReference()
    <AttributeField("PHYSIOLOGICALLY-RELEVANT?")>
    Public Property physiologicallyRelevant As Boolean
    <AttributeField("REACTION-BALANCE-STATUS")>
    Public Property reactionBalanceStatus As String
    <AttributeField("REACTION-DIRECTION")>
    Public Property reactionDirection As ReactionDirections
    <AttributeField("REACTION-LIST")>
    Public Property reactionList As String()
    <AttributeField("RXN-LOCATIONS")>
    Public Property reactionLocations As String()
    <AttributeField("SPONTANEOUS?")>
    Public Property spontaneous As Boolean
    <AttributeField("ATOM-MAPPINGS")>
    Public Property atomMappings As String()
    <AttributeField("ORPHAN?")>
    Public Property orphan As String
    <AttributeField("SYSTEMATIC-NAME")>
    Public Property systematicName As String
    <AttributeField("CANNOT-BALANCE?")>
    Public Property cannotBalance As Boolean
    <AttributeField("SIGNAL")>
    Public Property signal As String
    <AttributeField("SPECIES")>
    Public Property species As String

    Public ReadOnly Property equation As Equation
        Get
            Select Case reactionDirection
                Case ReactionDirections.IrreversibleLeftToRight, ReactionDirections.LeftToRight, ReactionDirections.PhysiolLeftToRight
                    Return New Equation With {
                        .Id = MyBase.ToString,
                        .reversible = False,
                        .Reactants = left,
                        .Products = right
                    }
                Case ReactionDirections.Reversible
                    Return New Equation With {
                        .Id = MyBase.ToString,
                        .Reactants = left,
                        .Products = right,
                        .reversible = True
                    }
                Case Else
                    ' right to left
                    Return New Equation With {
                        .Id = MyBase.ToString,
                        .reversible = False,
                        .Reactants = right,
                        .Products = left
                    }
            End Select
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return equation.ToString
    End Function

End Class

