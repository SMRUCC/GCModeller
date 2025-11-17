#Region "Microsoft.VisualBasic::8e0f26bd4d04a665e9d8e76968922b01, analysis\SequenceToolkit\MotifFinder\Seeds\FullScan.vb"

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

'   Total Lines: 61
'    Code Lines: 41 (67.21%)
' Comment Lines: 7 (11.48%)
'    - Xml Docs: 42.86%
' 
'   Blank Lines: 13 (21.31%)
'     File Size: 2.17 KB


' Class FullScan
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: GetSeeds
' 
' Class TaskPayload
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: Seeding
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' Create seeds via pairwise alignment between all sequence input
''' </summary>
Public Class FullScan : Inherits SeedScanner

    Public Sub New(param As PopulatorParameter, debug As Boolean)
        MyBase.New(param, debug)
    End Sub

    Public Overrides Iterator Function GetSeeds(regions As FastaSeq()) As IEnumerable(Of HSP)
        Call param.logText("create parallel task payload...")

        Dim payloads As TaskPayload()() = regions _
            .Select(Function(q) New TaskPayload(q, regions, param)) _
            .Split(partitionSize:=regions.Length / (App.CPUCoreNumbers * 2)) _
            .ToArray

        Call param.logText($"run task on {payloads.Length} parallel process!")
        Call param.logText($"there are {Aggregate x In payloads Into Average(x.Length)} task in each parallel process.")
        Call param.logText("seeding...")

        ' 先进行两两局部最优比对，得到最基本的种子
        ' 2018-3-2 在这里应该选取的是短的高相似度的序列
        Dim seeds As HSP()() = payloads _
            .Populate(parallel:=Not debug) _
            .Select(Function(q)
                        Return q.Select(Function(qi) qi.Seeding).IteratesALL.ToArray
                    End Function) _
            .ToArray

        For Each i As HSP() In seeds
            For Each j As HSP In i
                Yield j
            Next
        Next
    End Function

End Class

Friend Class TaskPayload

    Public q As FastaSeq
    Public regions As FastaSeq()
    Public param As PopulatorParameter

    Sub New(q As FastaSeq, regions As IEnumerable(Of FastaSeq), param As PopulatorParameter)
        Me.q = New FastaSeq With {.Headers = q.Headers, .SequenceData = q.SequenceData}
        Me.regions = regions.ToArray
        '.Select(Function(qi) New FastaSeq(fa:=qi)) _
        '.ToArray
        Me.param = New PopulatorParameter(param)
    End Sub

    Public Function Seeding() As HSP()
        Return regions.LocalSeeding(q, param).ToArray
    End Function

End Class
