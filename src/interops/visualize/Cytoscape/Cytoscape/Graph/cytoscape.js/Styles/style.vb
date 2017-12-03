#Region "Microsoft.VisualBasic::dd646b2cc07638160aca764a20459c68, ..\interops\visualize\Cytoscape\Cytoscape\Graph\cytoscape.js\Styles\style.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.Cyjs.style

    ''' <summary>
    ''' Style for cytoscape.js (*.json)
    ''' </summary>
    Public Class JSON : Implements INamedValue

        Public Property format_version As String
        Public Property generated_by As String = "GCModeller"
        Public Property target_cytoscapejs_version As String = "~2.1"
        Public Property title As String Implements INamedValue.Key
        Public Property style As style()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function Load(path$) As Dictionary(Of JSON)
            Return path _
                .ReadAllText _
                .LoadObject(Of JSON()) _
                .ToDictionary
        End Function
    End Class

    Public Class style

        Public Property selector As String
        Public Property css As Dictionary(Of String, String)

        Public Function MySelector() As Selector
            Return New Selector(selector)
        End Function

        Public Function GetStyle() As CSSTranslator
            Return New CSSTranslator(css)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
