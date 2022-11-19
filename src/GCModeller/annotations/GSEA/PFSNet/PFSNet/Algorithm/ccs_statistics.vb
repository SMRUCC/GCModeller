#Region "Microsoft.VisualBasic::b225a748007ca893dc907df9dc3a3a59, GCModeller\annotations\GSEA\PFSNet\PFSNet\Algorithm\ccs_statistics.vb"

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
    '    Code Lines: 20
    ' Comment Lines: 15
    '   Blank Lines: 5
    '     File Size: 1.62 KB


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

        For i As Integer = 0 To ccs.Length - 1
            Dim ccs_node = ccs(i)
            Dim x = pscore(i), y = nscore(i)

            ccs_node.statistics = t.Test(pscore(i), nscore(i)).TestValue
            ccs_node.pvalue = (tdist.Abs > Math.Abs(ccs_node.statistics)).Sum / tdist.Length
            ccs_node.masked = ccs_node.pvalue < 0.05
        Next

        Return ccs
    End Function
End Module
