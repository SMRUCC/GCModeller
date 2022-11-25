#Region "Microsoft.VisualBasic::f435cc180218f203b9ebcaef02068e1f, GCModeller\models\SBML\SBML\Level3\unitDefinition.vb"

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

    '   Total Lines: 20
    '    Code Lines: 13
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 512 B


    '     Class unitDefinition
    ' 
    '         Properties: listOfUnits
    ' 
    '     Class unit
    ' 
    '         Properties: exponent, kind, multiplier, scale
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML.Components

Namespace Level3

    Public Class unitDefinition : Inherits IPartsBase

        Public Property listOfUnits As unit()

    End Class

    Public Class unit

        <XmlAttribute> Public Property scale As Double
        <XmlAttribute> Public Property exponent As Double
        <XmlAttribute> Public Property multiplier As Double
        <XmlAttribute> Public Property kind As String

    End Class
End Namespace
