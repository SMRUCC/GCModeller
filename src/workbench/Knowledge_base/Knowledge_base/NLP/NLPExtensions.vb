#Region "Microsoft.VisualBasic::160e8a1c3700fdfbcc0d93aa9f7463c4, ..\workbench\Knowledge_base\Knowledge_base\NLP\NLPExtensions.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Webservices.Bing.Academic
Imports Microsoft.VisualBasic.Data.NLP

Public Module NLPExtensions

    <Extension>
    Public Function InformationAbstract(kb As IEnumerable(Of ArticleProfile), Optional minWeight# = 0.05) As Dictionary(Of String, Double)
        Dim text$ = kb _
            .Select(Function(a) a.abstract) _
            .Where(Function(s) Not s.StringEmpty) _
            .JoinBy(ASCII.LF)
        Dim abstract = text.TextGraph.Abstract(minWeight:=minWeight)

        Return abstract
    End Function
End Module
