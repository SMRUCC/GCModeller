#Region "Microsoft.VisualBasic::2788889523674d6208987f4a3b37710c, analysis\SequenceToolkit\MotifFinder\Seeds\MotifSeeds.vb"

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

'   Total Lines: 35
'    Code Lines: 19 (54.29%)
' Comment Lines: 11 (31.43%)
'    - Xml Docs: 72.73%
' 
'   Blank Lines: 5 (14.29%)
'     File Size: 1.41 KB


' Module MotifSeeds
' 
'     Function: LocalSeeding, PairwiseSeeding
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.SequenceModel.FASTA

Module MotifSeeds

    ''' <summary>
    ''' create seeds via pairwise alignment, use 
    ''' the smith-waterman HSP as motif seeds.
    ''' </summary>
    ''' <param name="regions"></param>
    ''' <param name="q"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    <Extension>
    Friend Function LocalSeeding(regions As IEnumerable(Of FastaSeq), q As FastaSeq, param As PopulatorParameter) As IEnumerable(Of HSP)
        Dim seeds As New List(Of HSP)

        ' the object reference is breaked at parallel
        ' use the title for test equals instead of test equals via hashcode
        ' at here
        For Each s As FastaSeq In regions.Where(Function(seq) Not seq.Title = q.Title)
            seeds += PairwiseSeeding(q, s, param)
        Next

        Return seeds
    End Function

    Public Function PairwiseSeeding(q As FastaSeq, s As FastaSeq, param As PopulatorParameter) As IEnumerable(Of HSP)
        Dim smithWaterman As New SequenceAlignment.BestLocalAlignment.SmithWaterman(q.SequenceData, s.SequenceData, New DNAMatrix)
        Call smithWaterman.BuildMatrix()
        Dim result = smithWaterman.GetOutput(param.seedingCutoff, param.minW)
        Return result.HSP.Where(Function(seed) seed.LengthHit <= param.maxW)
    End Function
End Module
