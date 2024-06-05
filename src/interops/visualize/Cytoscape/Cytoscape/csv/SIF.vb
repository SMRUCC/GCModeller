﻿#Region "Microsoft.VisualBasic::277cc90f374d7f3563ec28ca362e58ef, visualize\Cytoscape\Cytoscape\csv\SIF.vb"

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

    '   Total Lines: 32
    '    Code Lines: 13 (40.62%)
    ' Comment Lines: 15 (46.88%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (12.50%)
    '     File Size: 927 B


    '     Class SIF
    ' 
    '         Properties: interaction, source, target
    ' 
    '         Function: ToString, ToText
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Tables

    ''' <summary>
    ''' the cytoscape simple interaction format table
    ''' </summary>
    Public Class SIF

        ''' <summary>
        ''' the source node id
        ''' </summary>
        ''' <returns></returns>
        Public Property source As String
        ''' <summary>
        ''' the iteraction type
        ''' </summary>
        ''' <returns></returns>
        Public Property interaction As String
        ''' <summary>
        ''' the target node id
        ''' </summary>
        ''' <returns></returns>
        Public Property target As String

        Public Overrides Function ToString() As String
            Return $"{source}{vbTab}{interaction}{vbTab}{target}"
        End Function

        Public Shared Function ToText(network As IEnumerable(Of SIF)) As String
            Return network.JoinBy(vbLf)
        End Function
    End Class
End Namespace
