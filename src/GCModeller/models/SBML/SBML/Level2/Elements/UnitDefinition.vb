#Region "Microsoft.VisualBasic::718a977c28e989e99d8b851c099888e0, GCModeller\models\SBML\SBML\Level2\Elements\UnitDefinition.vb"

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

    '   Total Lines: 27
    '    Code Lines: 21
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 911 B


    '     Class unitDefinition
    ' 
    '         Properties: id, listOfUnits
    ' 
    '         Function: ToString
    ' 
    '     Structure Unit
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

Namespace Level2.Elements

    Public Class unitDefinition : Implements IReadOnlyId

        <XmlAttribute> Public Property id As String Implements IReadOnlyId.Identity
        Public Property listOfUnits As List(Of Unit)

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    <XmlType("unit")>
    Public Structure Unit
        <XmlAttribute> Dim kind As String
        <XmlAttribute> Dim scale, exponent As Integer
        <XmlAttribute> Dim multiplier As Double

        Public Overrides Function ToString() As String
            Return String.Format("kind={0}; scale={1}; multipiler={2}; exponent={3}", kind, scale, multiplier, exponent)
        End Function
    End Structure
End Namespace
