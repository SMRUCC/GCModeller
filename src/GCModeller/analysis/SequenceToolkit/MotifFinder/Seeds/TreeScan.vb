#Region "Microsoft.VisualBasic::b14d1f284b04f39244c9af3c869e90e6, analysis\SequenceToolkit\MotifFinder\Seeds\TreeScan.vb"

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


' Code Statistics:

'   Total Lines: 70
'    Code Lines: 49 (70.00%)
' Comment Lines: 6 (8.57%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 15 (21.43%)
'     File Size: 2.27 KB


' Class TreeScan
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: Compares, CreateTree, GetSeeds
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class TreeScan : Inherits SeedScanner

    Dim counter As Integer = 0
    Dim temp As New List(Of HSP)

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
        End If

        Dim seeds = MotifSeeds _
            .PairwiseSeeding(a, b, param) _
            .ToArray

        temp.AddRange(seeds)
        counter += seeds.Length

        If seeds.Length = 0 Then
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

            Call temp.Clear()

            If ++i Mod d = 0 Then
                Dim dt As TimeSpan = Now - t0
                Dim speed As String = (CInt(i) / dt.TotalSeconds).ToString("F2")
                Dim speed2 As String = (counter / dt.TotalSeconds).ToString("F2")

                Call param.logText($"[{i}/{regions.Length}, {dt.FormatTime} | {speed}sequence/sec | {speed2}seeds/sec] {(i / regions.Length * 100).ToString("F0")}% {counter} seeds | {seq.Title}")
            End If
        Next
    End Function
End Class
