Imports System.Runtime.CompilerServices

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
    ''' <param name="part"></param>
    ''' <returns></returns>
    <Extension>
    Public Function PartitionProbability(part As IEnumerable(Of Dictionary(Of I3Mers, Integer))) As Double

    End Function

    ''' <summary>
    ''' Where ``ncj = ∑yij`` is the total count Of the j-th ``3-mer`` in cluster ``<paramref name="c"/>``.
    ''' </summary>
    ''' <param name="c"></param>
    ''' <param name="j"></param>
    ''' <returns></returns>
    <Extension>
    Public Function nj(c As IEnumerable(Of Dictionary(Of I3Mers, Integer)), j As I3Mers) As Integer
        Return c.Select(Function(x) x(j)).Sum
    End Function
End Module
