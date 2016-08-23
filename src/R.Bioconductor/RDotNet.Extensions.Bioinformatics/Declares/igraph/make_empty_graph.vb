#Region "Microsoft.VisualBasic::b25c56a482a554be48e7c724e1f6fd11, ..\R.Bioconductor\RDotNet.Extensions.Bioinformatics\Declares\igraph\make_empty_graph.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder

Namespace igraph

    ''' <summary>
    ''' A graph with no edges
    ''' </summary>
    <RFunc("make_empty_graph")> Public Class make_empty_graph : Inherits igraph

        ''' <summary>
        ''' Number of vertices.
        ''' </summary>
        ''' <returns></returns>
        Public Property n As Integer = 0
        ''' <summary>
        ''' Whether to create a directed graph.
        ''' </summary>
        ''' <returns></returns>
        Public Property directed As Boolean = True
    End Class

    <RFunc(NameOf(empty_graph))> Public Class empty_graph : Inherits make_empty_graph

    End Class
End Namespace
