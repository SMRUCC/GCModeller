#Region "Microsoft.VisualBasic::38409bcce85868fbc28a9e34f593c1f2, annotations\GSEA\PFSNet\PFSNet\Algorithm\ccs_statistics.vb"

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

    '   Total Lines: 40
    '    Code Lines: 20 (50.00%)
    ' Comment Lines: 15 (37.50%)
    '    - Xml Docs: 93.33%
    ' 
    '   Blank Lines: 5 (12.50%)
    '     File Size: 1.66 KB


    ' Module ccs_statistics
    ' 
    '     Function: doStatics
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure

<HideModuleName> Module ccs_statistics

    ''' <summary>
    ''' for(i in 1:length(ccs)){ 
    '''    #try(statistics[i]&lt;-t.test(unlist(pscore[,i]),unlist(nscore[,i]),paired=TRUE)$statistic, TRUE) 
    '''    #try(pval[i]&lt;-sum(abs(tdist)>abs(statistics[i]))/length(tdist))
    '''    try(ccs[[i]]$statistics&lt;-t.test(unlist(pscore[,i]),unlist(nscore[,i]),paired=TRUE)$statistic, TRUE) 
    '''    try(ccs[[i]]$p.value&lt;-sum(abs(tdist)>abs(ccs[[i]]$statistics))/length(tdist),TRUE)
    '''    try(if(ccs[[i]]$p.value&lt;0.05){ccs.mask[i]&lt;-TRUE},TRUE)
    ''' }
    ''' </summary>
    ''' <param name="ccs"></param>
    ''' <param name="pscore"></param>
    ''' <param name="nscore"></param>
    ''' <param name="tdist"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function doStatics(ccs As PFSNetGraph(),
                              pscore As Double()(),
                              nscore As Double()(),
                              tdist As Vector) As PFSNetGraph()

        Dim nullValues As Double() = tdist _
            .ToArray _
            .Where(Function(value) Double.IsFinite(value)) _
            .ToArray

        For i As Integer = 0 To ccs.Length - 1
            Dim ccsNode As PFSNetGraph = ccs(i)
            Dim statistic As Double = PFSNetStatistics.PairedTTest(pscore(i), nscore(i))

            ccsNode.statistics = statistic

            If Not Double.IsFinite(statistic) OrElse nullValues.Length = 0 Then
                ccsNode.pvalue = 1
                ccsNode.masked = False
                Continue For
            End If

            Dim exceedances As Integer = nullValues _
                .Where(Function(value) Math.Abs(value) > Math.Abs(statistic)) _
                .Count

            ccsNode.pvalue = exceedances / CDbl(nullValues.Length)
            ccsNode.masked = ccsNode.pvalue < 0.05
        Next

        Return ccs
    End Function
End Module
