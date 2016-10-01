#Region "Microsoft.VisualBasic::0519782f408b6043e8209f6751505913, ..\interops\visualize\Circos\Circos\ConfFiles\Ideogram.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports System.Text

Namespace Configurations

    ''' <summary>
    ''' The ``&lt;ideogram>`` block defines the position, size, labels and other
    ''' properties Of the segments On which data are drawn. These segments
    ''' are usually chromosomes, but can be any Integer axis.
    ''' </summary>
    Public Class Ideogram : Inherits CircosConfig
        Implements ICircosDocument

        Public Property Ideogram As Nodes.Ideogram = New Nodes.Ideogram

        Sub New(Circos As Circos)
            Call MyBase.New(IdeogramConf, Circos)
        End Sub

        Protected Overrides Function Build(IndentLevel As Integer) As String
            Return Ideogram.Build(IndentLevel)
        End Function
    End Class
End Namespace
