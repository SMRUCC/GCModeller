Imports RDotNet.Extensions.VisualBasic

Namespace hierfstat

    Public Module API

        ''' <summary>
        ''' Wrapper for fst estimator from hierfstat package (from adegenet)
        ''' 
        ''' ```R
        ''' pairwise.fst(x, pop = NULL, res.type = c("dist", "matrix"))
        ''' ```
        ''' 
        ''' The function fstat is a wrapper for varcomp.glob of the package hierfstat. For Fst, Fis and Fit, an alternative is offered by 
        ''' Fst from the pagas package (see example).
        ''' Let A And B be two populations of population sizes n_A And n_B, with expected heterozygosity (averaged over loci) Hs(A) And Hs(B), 
        ''' respectively. We denote Ht the expected heterozygosity of a population pooling A And B. Then, the pairwise Fst between A And B 
        ''' Is computed as
        ''' 
        ''' ```
        ''' Fst(A,B) = \frac{(Ht - (n_A Hs(A) + n_B Hs(B))/(n_A + n_B) )}{Ht} 
        ''' ```
        ''' </summary>
        ''' <param name="x">an object of class genind.(其实在这里是R对象的变量名称)</param>
        ''' <param name="pop">a factor giving the 'population' of each individual. If NULL, pop is seeked from pop(x). Note that the term population refers in fact to any grouping of individuals'.</param>
        ''' <param name="resType">the type of result to be returned: a dist object, or a symmetric matrix</param>
        ''' <returns>A vector, a matrix, or a dist object containing F statistics.</returns>
        Public Function pairwise_fst(x As String, Optional pop As String = NULL, Optional resType As String = "c(""dist"", ""matrix"")")
            Dim out As SymbolicExpression = $"pairwise.fst({x}, pop = {pop}, res.type = {resType})".ζ

        End Function
    End Module
End Namespace