Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998

Public Class RulerAlignment

    ReadOnly rulers As ScanRuler()
    ReadOnly win_size As Integer
    ReadOnly steps As Integer

    ''' <summary>
    ''' 对标尺进行分段扫描生成机器学习的训练数据集
    ''' </summary>
    ''' <param name="nt"></param>
    ''' <returns></returns>
    Public Function Scan(nt As String) As Double()
        Dim buffer As New List(Of Double)
        Dim target As New NucleicAcid(nt)

        For Each ruler In rulers
            For Each fragment As NucleicAcid In ruler
                Call DifferenceMeasurement.Sigma(fragment, nt).DoCall(AddressOf buffer.Add)
            Next
        Next

        Return buffer.ToArray
    End Function

    Public Overrides Function ToString() As String
        Return $"{rulers.Length} reference rulers with [win_size={win_size}bits and steps={steps}bits]"
    End Function

End Class

Public Class ScanRuler : Implements IEnumerable(Of NucleicAcid)

    ReadOnly framents As NucleicAcid()

    Sub New(ruler As NucleicAcid, win_size As Integer, steps As Integer)
        framents = ruler.CreateFragments(win_size, steps).ToArray
    End Sub

    Public Iterator Function GetEnumerator() As IEnumerator(Of NucleicAcid) Implements IEnumerable(Of NucleicAcid).GetEnumerator
        For Each region In framents
            Yield region
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class