Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class TreeScan : Inherits SeedScanner

    ReadOnly hsp As New List(Of HSP)

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
        End If

        Dim seeds As HSP() = MotifSeeds.pairwiseSeeding(a, b, param).ToArray

        Call hsp.AddRange(seeds)

        If seeds.Length = 0 Then
            Return -1
        Else
            Return 1
        End If
    End Function

    Public Overrides Function GetSeeds(regions() As FastaSeq) As IEnumerable(Of HSP)
        Dim tree = CreateTree()

        For Each seq As FastaSeq In regions
            Call tree.Add(key:=seq)
        Next

        Return hsp
    End Function
End Class
