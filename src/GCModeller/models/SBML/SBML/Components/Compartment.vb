#Region "Microsoft.VisualBasic::d9854d2df20bd22f73c9936fa37541a9, models\SBML\SBML\Components\Compartment.vb"

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

    '     Class Compartment
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Components

    ''' <summary>
    ''' The space region in a cell.(细胞内部的一个空间区域)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlType("compartment", [Namespace]:="sbml/components")>
    Public Class Compartment : Inherits IPartsBase

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", id, name)
        End Function
    End Class
End Namespace
