#Region "Microsoft.VisualBasic::43bd4345b190ce5af741037417601564, ..\Circos\Circos\ConfFiles\ComponentModel\Interface.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Configurations

    Public MustInherit Class CircosDocument : Implements ICircosDocNode
        MustOverride Function GenerateDocument(IndentLevel As Integer) As String Implements ICircosDocNode.GenerateDocument
    End Class

    ''' <summary>
    ''' This object can be convert to text document by using method <see cref="GenerateDocument"/>
    ''' </summary>
    Public Interface ICircosDocNode
        Function GenerateDocument(indentLevel As Integer) As String
    End Interface

    ''' <summary>
    ''' This object can be save as a text doc for the circos plot
    ''' </summary>
    Public Interface ICircosDocument : Inherits ICircosDocNode, ISaveHandle
    End Interface
End Namespace
