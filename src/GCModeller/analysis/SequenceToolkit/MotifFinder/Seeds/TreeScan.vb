Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class TreeScan : Inherits SeedScanner

    Dim counter As Integer = 0
    Dim temp As HSP()

    ReadOnly t0 As Date = Now

    Public Sub New(param As PopulatorParameter, debug As Boolean)
        MyBase.New(param, debug)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function CreateTree() As AVLClusterTree(Of FastaSeq)
        Return New AVLClusterTree(Of FastaSeq)(AddressOf Compares, views:=Function(fa) fa.Title)
    End Function

    ''' <summary>
    ''' 建树的过程中会完成种子的提取
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    Private Function Compares(a As FastaSeq, b As FastaSeq) As Integer
        If a.SequenceData = b.SequenceData Then
            Return 0
        Else
            temp = MotifSeeds _
                .PairwiseSeeding(a, b, param) _
                .ToArray
            counter += temp.Length
        End If

        If temp.Length = 0 Then
            Return -1
        Else
            Return 1
        End If
    End Function

    Public Overrides Iterator Function GetSeeds(regions() As FastaSeq) As IEnumerable(Of HSP)
        Dim tree = CreateTree()
        Dim i As i32 = Scan0
        Dim d As Integer = regions.Length / 100

        For Each seq As FastaSeq In regions
            Call tree.Add(key:=seq)

            For Each seed As HSP In temp
                Yield seed
            Next

            If ++i Mod d = 0 Then
                Dim dt As TimeSpan = Now - t0
                Dim speed As String = (CInt(i) / dt.TotalSeconds).ToString("F2")
                Dim speed2 As String = (counter / dt.TotalSeconds).ToString("F2")

                Call param.logText($"[{i}/{regions.Length}, {dt.FormatTime} | {speed}sequence/sec | {speed2}seeds/sec] {(i / regions.Length * 100).ToString("F0")}% {counter} seeds | {seq.Title}")
            End If
        Next
    End Function
End Class
