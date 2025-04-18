﻿#Region "Microsoft.VisualBasic::f67460c2a271cfdb9ccf55107a2cc11c, visualize\Circos\Circos\ConfFiles\Ideogram.vb"

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

    '     Class Ideogram
    ' 
    '         Properties: Ideogram
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Build
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel

Namespace Configurations

    ''' <summary>
    ''' The ``&lt;ideogram>`` block defines the position, size, labels and other
    ''' properties Of the segments On which data are drawn. These segments
    ''' are usually chromosomes, but can be any Integer axis.
    ''' </summary>
    Public Class Ideogram : Inherits CircosConfig
        Implements ICircosDocument

        Public Property Ideogram As Nodes.Ideogram = New Nodes.Ideogram

        Sub New(circos As Circos)
            Call MyBase.New(IdeogramConf, circos)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function Build(IndentLevel As Integer, directory$) As String
            Return Ideogram.Build(IndentLevel, directory)
        End Function
    End Class
End Namespace
