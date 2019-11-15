Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Class RNAComposition : Implements IEnumerable(Of NamedValue(Of Double))

    Public Property geneID As String
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property A As Integer
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property U As Integer
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property G As Integer
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property C As Integer

    Public Overrides Function ToString() As String
        Return geneID
    End Function

    ''' <summary>
    ''' 因为这个是RNA序列，所以其构成应该是其基因模板的互补
    ''' </summary>
    ''' <param name="nt">DNA模板序列</param>
    ''' <returns></returns>
    Public Shared Function FromNtSequence(nt As String, geneID As String) As RNAComposition
        Dim RNA As String = NucleicAcid.Complement(nt)
        Dim composition As Dictionary(Of String, Integer) = RNA _
            .GroupBy(Function(c) c) _
            .ToDictionary(Function(c)
                              Return c.Key.ToString
                          End Function,
                          Function(c) c.Count)

        Return New RNAComposition With {
            .geneID = geneID,
            .A = composition.TryGetValue("A"),
            .C = composition.TryGetValue("C"),
            .G = composition.TryGetValue("G"),
            .U = composition.TryGetValue("T")
        }
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of NamedValue(Of Double)) Implements IEnumerable(Of NamedValue(Of Double)).GetEnumerator
        Yield New NamedValue(Of Double)("A", A)
        Yield New NamedValue(Of Double)("U", U)
        Yield New NamedValue(Of Double)("G", G)
        Yield New NamedValue(Of Double)("C", C)
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class
