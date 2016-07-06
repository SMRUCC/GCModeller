Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.Framework.DirichletDistribution
Imports Microsoft.VisualBasic.Linq

Public Module PosteriorProbability

    ''' <summary>
    ''' For all ``3-mer`` count vectors in a crude cluster ``c``, **we
    ''' assume the probability To observe any ``3-mer`` Is the same.**
    ''' 
    ''' Here we denote the probabilities To observe the ``3-mers`` In
    ''' cluster ``c`` As ``(pc1, pc2, ... , pc64)``. Then, the conditional 
    ''' likelihood of the data Is defined as
    ''' 
    ''' ```
    ''' p(y{n}|D,S) = ∏{c,1->k}∏{j,1->64}pcj
    ''' ```
    ''' </summary>
    ''' <param name="S"></param>
    ''' <returns></returns>
    <Extension>
    Public Function PartitionProbability(S As IEnumerable(Of I3merVector)) As Double
        Dim a As Double = S.Select(Function(c) gamma1 / lgamma(I3Mersx.Select(Function(j) lambda_cj + S.nj(j)).Sum)).π
        Dim b As Double = I3Mersx.Select(Function(j) lgamma(lambda_cj + S.nj(j)) / gammaj).π
        Dim o As Double = a * b

        Return o
    End Function

    ReadOnly gamma1 As Double = lgamma(1.0R)
    ReadOnly gammaj As Double = lgamma(lambda_cj)

    Const lambda_cj As Double = 1.0R / 64.0R

    ''' <summary>
    ''' Where ``ncj = ∑yij`` is the total count Of the j-th ``3-mer`` in cluster ``<paramref name="c"/>``.
    ''' </summary>
    ''' <param name="c"></param>
    ''' <param name="j"></param>
    ''' <returns></returns>
    <Extension>
    Public Function nj(c As IEnumerable(Of I3merVector), j As I3Mers) As Integer
        Return c.Select(Function(x) x.Vector(j)).Sum
    End Function
End Module
