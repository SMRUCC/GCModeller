Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure

<HideModuleName> Module ccs_statistics

    Public Function Internal_statics(ccs As PFSNetGraph(),
                                     pscore As Double()(),
                                     nscore As Double()(),
                                     tdist As Vector) As PFSNetGraph()

        For i As Integer = 0 To ccs.Length - 1
            Dim ccs_node = ccs(i)
            Dim x = pscore(i), y = nscore(i)
            ccs_node.statistics = t.Test(pscore(i), nscore(i)).TestValue
            ccs_node.pvalue = (tdist.Abs > ccs_node.statistics).Sum / tdist.Length
            ccs_node.masked = ccs_node.pvalue < 0.05
        Next

        Return ccs
    End Function
End Module
