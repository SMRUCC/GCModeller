#Region "Microsoft.VisualBasic::66dd691881adebeb9cb6e84a66bda031, core\Bio.Assembly\ComponentModel\Equations\CompoundSpecies.vb"

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

    '   Total Lines: 25
    '    Code Lines: 18 (72.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (28.00%)
    '     File Size: 616 B


    '     Class CompoundSpecies
    ' 
    '         Properties: entry, enzyme, formula, name, reactions
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace ComponentModel.EquaionModel

    Public Class CompoundSpecies

        <XmlAttribute> Public Property entry As String
        Public Property name As String
        Public Property formula As String
        <XmlElement> Public Property reactions As String()
        <XmlElement> Public Property enzyme As String()

        Sub New()
        End Sub

        Sub New(id As String)
            entry = id
        End Sub

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class
End Namespace
