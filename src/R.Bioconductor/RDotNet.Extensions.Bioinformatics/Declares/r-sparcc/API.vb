Imports RDotNet.Extensions.VisualBasic.RSystem
Imports RDotNet.Extensions.VisualBasic

Namespace sparcc

    ''' <summary>
    ''' R package computes correlation for relative abundances
    ''' </summary>
    Public Module API

        ''' <summary>
        ''' 装载计算脚本
        ''' </summary>
        Sub New()
            Call RServer.Evaluate(My.Resources.sparcc)
        End Sub

        ''' <summary>
        ''' count matrix x should be samples on the rows and OTUs on the colums,
        ''' 
        ''' ```R
        ''' assuming dim(x) -> samples by OTUs
        ''' ```
        ''' </summary>
        ''' <param name="x">The data file path</param>
        ''' <param name="maxIter"></param>
        ''' <param name="th"></param>
        ''' <param name="exiter"></param>
        ''' <returns></returns>
        Public Function sparcc(x As String, Optional maxIter As Integer = 20, Optional th As Double = 0.1, Optional exiter As Double = 10)
            Dim out As String() = RServer.WriteLine($"tab <- read.table(""{x.UnixPath}"",header=T);")
            out = RServer.WriteLine($"thisX <- sparcc(tab, {maxIter}, {th}, {exiter});")
        End Function
    End Module
End Namespace