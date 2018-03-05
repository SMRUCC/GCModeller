#Region "Microsoft.VisualBasic::df05c674fca3480428adbe356e17b55a, RNA-Seq\Assembler\.NET Bio\Assembler.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'Namespace MBF

'    Public Class Assembler

'        Public Function Assembling(Fq As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
'            Dim Assembler As New Bio.Algorithms.Assembly.OverlapDeNovoAssembler
'            Dim Fastq = New Bio.IO.FastQ.FastQParser().Parse(New IO.FileStream(Fq, IO.FileMode.Open))
'            Dim resultBuffer = Assembler.Assemble(Fastq)
'            Dim LQuery = (From sequence In resultBuffer.AssembledSequences.AsParallel Select sequence.ToFasta).ToArray
'            Return LQuery
'        End Function



'        Sub map(fq As String)
'            Dim mapper As New Bio.Algorithms.Assembly.MatePairMapper
'            Dim asfds As New Bio.Algorithms.Assembly.Padena.Scaffold.ReadContigMap
'            Dim Fastq = New Bio.IO.FastQ.FastQParser().Parse(New IO.FileStream(fq, IO.FileMode.Open))

'        End Sub

'    End Class
'End Namespace
