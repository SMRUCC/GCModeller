﻿#Region "Microsoft.VisualBasic::9c738a2bd82d9ec0044b77e35d89c0b9, models\SBML\SBML\Level3\reaction.vb"

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

'   Total Lines: 42
'    Code Lines: 29 (69.05%)
' Comment Lines: 3 (7.14%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 10 (23.81%)
'     File Size: 1.45 KB


'     Class Reaction
' 
'         Properties: annotation, compartment, fast, listOfModifiers, listOfProducts
'                     listOfReactants, notes, reversible
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
            Return $"[{id}] {Format(listOfReactants)} {If(reversible, "=", "=>")} {Format(listOfProducts)}"
        End Function

        Private Shared Function Format(listOf As IEnumerable(Of SpeciesReference)) As String
            Return listOf _
                .Select(Function(s)
                            Return If(s.stoichiometry = 1.0, "", s.stoichiometry & " ") & s.species
                        End Function) _
                .JoinBy(" + ")
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

        <XmlAttribute("constant")>
        Public Property Constant As Boolean

    End Class
End Namespace
