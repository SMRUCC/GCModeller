#Region "Microsoft.VisualBasic::24da5862c3e2abcde05277a818b7c50b, CLI_tools\NCBI_tools\CLI\GCAassembly.vb"

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

    ' Module CLI
    ' 
    '     Function: gpff2Fasta
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/gpff.fasta")>
    <Usage("/gpff.fasta /in <gpff.txt> [/out <out.fasta>]")>
    Public Function gpff2Fasta(args As CommandLine) As Integer
        Using out As StreamWriter = args.OpenStreamOutput("/out", Encodings.ASCII)
            For Each seq In GBFF.File.LoadDatabase(args <= "/in")
                Dim fasta As New FastaSeq With {
                    .Headers = {
                        seq.Locus.AccessionID,
                        seq.Definition.Value
                    },
                    .SequenceData = seq.Origin.SequenceData
                }

                Call out.WriteLine(fasta.GenerateDocument(120))
            Next
        End Using

        Return 0
    End Function
End Module
