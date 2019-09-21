#Region "Microsoft.VisualBasic::60642ade0501ea47ffc76755857af6c1, RDotNet.Extensions.Bioinformatics\Declares\hierfstat\API.vb"

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

    '     Module API
    ' 
    '         Function: pairwise_fst
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
            Dim out As SymbolicExpression = $"pairwise.fst({x}, pop = {pop}, res.type = {resType})".__call

        End Function
    End Module
End Namespace
