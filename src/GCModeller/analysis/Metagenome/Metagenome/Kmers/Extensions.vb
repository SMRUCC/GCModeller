#Region "Microsoft.VisualBasic::235f2c855f5e00de41528c69763bf449, analysis\Metagenome\Metagenome\Kmers\Extensions.vb"

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

'   Total Lines: 29
'    Code Lines: 18 (62.07%)
' Comment Lines: 7 (24.14%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 4 (13.79%)
'     File Size: 1.21 KB


'     Module Extensions
' 
'         Function: QueryKmers
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Namespace Kmers

    Public Module Extensions

        ''' <summary>
        ''' Get k-mer raw data from all of the sample reads data
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="kmers_db"></param>
        ''' <param name="sample">all reads data in a given sample</param>
        ''' <returns>use this function prepares for make sample quantification</returns>
        <Extension>
        Public Iterator Function QueryKmers(Of T As ISequenceProvider)(kmers_db As DatabaseReader, sample As IEnumerable(Of T)) As IEnumerable(Of KmerSeed)
            For Each reads As T In TqdmWrapper.Wrap(sample.ToArray)
                For Each kmer As KmerSeed In KSeq.KmerSpans(reads.GetSequenceData, kmers_db.k).Select(Function(k) kmers_db.GetKmer(k))
                    If Not kmer Is Nothing Then
                        Yield kmer
                    End If
                Next
            Next
        End Function

    End Module
End Namespace
