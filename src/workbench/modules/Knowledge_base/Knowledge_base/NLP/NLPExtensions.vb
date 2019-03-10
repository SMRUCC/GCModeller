﻿#Region "Microsoft.VisualBasic::7690bf5d2079397cd710fb590299b93c, Knowledge_base\Knowledge_base\NLP\NLPExtensions.vb"

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

    ' Module NLPExtensions
    ' 
    '     Function: InformationAbstract, TextGraphWeights
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.PageRank
Imports Microsoft.VisualBasic.Data.NLP
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Webservices.Bing.Academic

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

    <Extension>
    Public Function TextGraphWeights(kb As IEnumerable(Of ArticleProfile)) As Dictionary(Of String, Double)
        Dim text$ = kb _
            .Select(Function(a) a.abstract) _
            .Where(Function(s) Not s.StringEmpty) _
            .JoinBy(ASCII.LF)
        Dim abstract = text.TextGraph.Rank

        Return abstract
    End Function
End Module
