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
