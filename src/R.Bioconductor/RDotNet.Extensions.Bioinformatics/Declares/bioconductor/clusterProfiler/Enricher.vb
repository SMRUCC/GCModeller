#Region "Microsoft.VisualBasic::f5ac38cfa85691a960a7bc6fb8328519, RDotNet.Extensions.Bioinformatics\Declares\bioconductor\clusterProfiler\Enricher.vb"

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

    '     Class Enricher
    ' 
    '         Properties: go2name
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) Enrich
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.API.utils
Imports RDotNET.Extensions.VisualBasic.API.base
Imports RDotNET.Extensions.VisualBasic

Namespace clusterProfiler

    Public Class Enricher

        Public ReadOnly Property go2name As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="goBrief$">``go_brief.csv``</param>
        Sub New(goBrief$, Optional goID$ = "goID", Optional name$ = "name")
            Dim header$ = c({goID, name}, stringVector:=True)

            goBrief = read.csv(goBrief)
            go2name = App.NextTempName

            require("clusterProfiler")

            SyncLock R
                With R
                    .call = $"{go2name} <- {goBrief}[, {header}]"
                End With
            End SyncLock
        End Sub

        ''' <summary>
        ''' GO富集操作
        ''' </summary>
        ''' <param name="DEGs"></param>
        ''' <param name="term2gene$">
        ''' term2gene should provide background annotation of the whole genome
        ''' </param>
        ''' <param name="save$"></param>
        ''' <param name="noCuts"></param>
        ''' <returns></returns>
        Public Function Enrich(DEGs As IEnumerable(Of String), backgrounds As IEnumerable(Of String), term2gene$, save$, Optional noCuts As Boolean = False) As Boolean
            Dim deg$ = c(DEGs, stringVector:=True)
            Dim universe$ = c(backgrounds, stringVector:=True)
            Dim t2g$ = read.table(term2gene, header:=False)
            Dim result$

            If noCuts Then
                result = clusterProfiler.enricher(deg, universe, t2g, pvalueCutoff:=1, minGSSize:=1, qvalueCutoff:=1, TERM2NAME:=go2name)
            Else
                result = clusterProfiler.enricher(deg, universe, t2g, TERM2NAME:=go2name)
            End If

            Call write.csv(summary(result), save, rowNames:=False)

            Return save.FileExists(ZERO_Nonexists:=True)
        End Function

        Public Function Enrich(DEGs$, backgrounds$, term2gene$, save$, Optional noCuts As Boolean = False) As Boolean
            Return Enrich(DEGs.ReadAllLines, backgrounds.ReadAllLines, term2gene, save, noCuts)
        End Function
    End Class
End Namespace
