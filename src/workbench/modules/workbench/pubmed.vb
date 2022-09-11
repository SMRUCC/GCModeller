#Region "Microsoft.VisualBasic::2fccbf0bbf08557b7643b8af13693cbc, modules\workbench\pubmed.vb"

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

    '   Total Lines: 28
    '    Code Lines: 22
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.04 KB


    ' Module pubmedTools
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Parse, QueryKeyword, toTabular
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.PubMed
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("pubmed")>
Module pubmedTools

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(PubmedArticle()), AddressOf toTabular)
    End Sub

    Private Function toTabular(articles As PubmedArticle(), args As list, env As Environment) As dataframe

    End Function

    <ExportAPI("query")>
    Public Function QueryKeyword(keyword As String, Optional page As Integer = 1, Optional size As Integer = 2000) As String
        Return PubMed.QueryPubmedRaw(term:=keyword, page:=page, size:=size)
    End Function

    <ExportAPI("article")>
    Public Function Parse(text As String) As PubmedArticle()
        Return PubMed.ParseArticles(text).ToArray
    End Function
End Module
