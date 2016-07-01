#Region "Microsoft.VisualBasic::50f3ee9f7bd38446c22ddc8cbe10be0d, ..\GCModeller\CLI_tools\VirtualFootprint\CLI\Sites.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Partial Module CLI

    <ExportAPI("/Trim.Strand",
               Info:="Removes all of the sites which is on the different strand with the tag gene.",
               Usage:="/Trim.Strand /in <segments.Csv> /PTT <genome.ptt> [/out <out.csv>]")>
    Public Function TrimStrand(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & "-" & PTT.BaseName & ".Csv")
        Dim sites As SimpleSegment() = [in].LoadCsv(Of SimpleSegment)
        Dim genome As PTT = TabularFormat.PTT.Load(PTT)

        sites = sites.TrimStranded(genome, Function(x) x.ID.Split(":"c).First).ToArray

        Return sites.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Merge.Sites",
               Info:="Merge the segment loci sites within the specific length offset ranges.",
               Usage:="/Merge.Sites /in <segments.Csv> [/nt <nt.fasta> /out <out.csv> /offset <10>]")>
    Public Function MergeSites(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim offset As Integer = args.GetValue("/offset", 10)
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & "-offsets-" & offset & ".Csv")
        Dim sites As SimpleSegment() = [in].LoadCsv(Of SimpleSegment)
        Dim seq As String = args - "/nt"

        sites = sites.MergeLocis(offset,
                                 Function(x) CInt(x.ID.Split(":"c).Last),
                                 Function(x) x.ID.Split(":"c).First).ToArray

        If seq.FileExists Then
            Dim nt As FastaToken = FastaToken.LoadNucleotideData(seq)
            Dim reader As New SegmentReader(nt)

            For Each site As SimpleSegment In sites
                site.SequenceData = reader.TryParse(site.MappingLocation).SequenceData
            Next
        End If

        Return sites.SaveTo(out).CLICode
    End Function
End Module
