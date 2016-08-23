#Region "Microsoft.VisualBasic::280c968b33182cfe835f308509a3f80e, ..\R.Bioconductor\RDotNet.Extensions.Bioinformatics\Declares\igraph\graph_from_edgelist.vb"

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
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace igraph

    ''' <summary>
    ''' graph_from_edgelist creates a graph from an edge list. Its argument is a two-column matrix, each row defines one edge.
    ''' If it is a numeric matrix then its elements are interpreted as vertex ids.
    ''' If it is a character matrix then it is interpreted as symbolic vertex names and a vertex id will be assigned to each name, and also a name vertex attribute will be added.
    ''' </summary>
    <RFunc(NameOf(graph_from_edgelist))> Public Class graph_from_edgelist : Inherits igraph

        ''' <summary>
        ''' The edge list, a two column matrix, character or numeric.
        ''' </summary>
        ''' <returns></returns>
        Public Property el As RExpression
        ''' <summary>
        ''' Whether to create a directed graph.
        ''' </summary>
        ''' <returns></returns>
        Public Property directed As Boolean = True

    End Class

    ''' <summary>
    ''' Create a graph from an edge list matrix
    ''' </summary>
    <RFunc(NameOf(from_edgelist))> Public Class from_edgelist : Inherits graph_from_edgelist

    End Class
End Namespace
