#Region "Microsoft.VisualBasic::c338a598c453c01706be0d2e0f3f20b3, RNA-Seq\Rockhopper\API\DeNovolTranscript.vb"

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

    '     Module DeNovolTranscript
    ' 
    '         Function: LoadDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace AnalysisAPI

    Public Module DeNovolTranscript

        Public Function LoadDocument(Path As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Call Console.WriteLine("[Load] " & Path.ToFileURL)

            Dim ChunkBuffer As String() = System.IO.File.ReadAllLines(Path).Skip(1).ToArray
            Dim LQuery = (From i As Integer In ChunkBuffer.Sequence.AsParallel
                          Let s As String = ChunkBuffer(i)
                          Let Tokens As String() = Strings.Split(s, vbTab)
                          Select New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With
                              {.SequenceData = Tokens(0),
                                .Attributes = New String() {$"hash={i}", $"Expression={Tokens(2)}"}}).ToArray
            Return CType(LQuery, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
        End Function
    End Module
End Namespace
