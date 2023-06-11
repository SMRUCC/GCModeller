#Region "Microsoft.VisualBasic::a2823a6f84b0ded8a349f34a84e90594, GCModeller\models\SBML\SBML\Level3\reaction.vb"

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

    '   Total Lines: 41
    '    Code Lines: 28
    ' Comment Lines: 3
    '   Blank Lines: 10
    '     File Size: 1.40 KB


    '     Class Reaction
    ' 
    '         Properties: annotation, fast, listOfModifiers, listOfProducts, listOfReactants
    '                     notes, reversible
    ' 
    '         Function: ToString
    ' 
    '     Class modifierSpeciesReference
    ' 
    '         Properties: species
    ' 
    '         Function: ToString
    ' 
    '     Class SpeciesReference
    ' 
    '         Properties: Constant
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML.Components

Namespace Level3

    ''' <summary>
    ''' the base element model of the sbml
    ''' </summary>
    <XmlType("reaction", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class Reaction : Inherits IPartsBase

        <XmlAttribute> Public Property reversible As Boolean
        <XmlAttribute> Public Property fast As Boolean
        <XmlAttribute> Public Property compartment As String

        Public Property notes As Notes
        Public Property annotation As annotation

        Public Property listOfReactants As List(Of SpeciesReference)
        Public Property listOfProducts As List(Of SpeciesReference)
        Public Property listOfModifiers As List(Of modifierSpeciesReference)

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    Public Class modifierSpeciesReference

        <XmlAttribute>
        Public Property species As String

        Public Overrides Function ToString() As String
            Return species
        End Function
    End Class

    <XmlType("speciesReference", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class SpeciesReference : Inherits Level2.Elements.speciesReference
        <XmlAttribute("constant")> Public Property Constant As Boolean
    End Class
End Namespace
