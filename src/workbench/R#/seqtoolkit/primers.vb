#Region "Microsoft.VisualBasic::d2268173d8317ff39ad311c73580132e, R#\seqtoolkit\primers.vb"

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

    '   Total Lines: 103
    '    Code Lines: 90 (87.38%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 13 (12.62%)
    '     File Size: 5.47 KB


    ' Module primers
    ' 
    '     Function: CandidateTable, find_primers_region
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.PrimerDesigner
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Slicer
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' PCR primer design tools
''' </summary>
''' 
''' <remarks>
''' This R# package module provides the api for find the candidate PCR primer 
''' design regions from the blastn web search hits:
''' 
''' + ``primer_regions``: find the candidate PCR primer design regions from a 
'''   given set of the blastn hit records, and then annotate the gene context 
'''   information of each candidate region via a given genomics feature 
'''   annotation table(GFF).
''' </remarks>
<Package("primers")>
Module primers

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(CandidateRegion), AddressOf CandidateTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function CandidateTable(candidate As CandidateRegion, args As list, env As Environment) As Object
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim core_id = candidate.GenesInCoreRegion.Select(Function(g) g.ID)
        Dim ext_id = candidate.GenesInExtendedRegion.Select(Function(g) g.ID)
        Dim core_left = candidate.GenesInCoreRegion.Select(Function(g) g.left)
        Dim ext_left = candidate.GenesInExtendedRegion.Select(Function(g) g.left)
        Dim core_right = candidate.GenesInCoreRegion.Select(Function(g) g.right)
        Dim ext_right = candidate.GenesInExtendedRegion.Select(Function(g) g.right)
        Dim core_len = candidate.GenesInCoreRegion.Select(Function(g) g.Length)
        Dim ext_len = candidate.GenesInExtendedRegion.Select(Function(g) g.Length)
        Dim core_strand = candidate.GenesInCoreRegion.Select(Function(g) g.strand.Description)
        Dim ext_strand = candidate.GenesInExtendedRegion.Select(Function(g) g.strand.Description)
        Dim hits As String = candidate.SupportingHits.Select(Function(h) $"{h.QueryID}({h.SubjectStart}|{h.SubjectEnd})").JoinBy("; ")
        Dim core_type = candidate.GenesInCoreRegion.Select(Function(any) "core")
        Dim ext_type = candidate.GenesInExtendedRegion.Select(Function(any) "extended")
        Dim fna As ChunkedNtFasta = args.getBySynonyms("fna")

        Call df.add("chr", scalar:=candidate.Chr)
        Call df.add("primer_start", scalar:=candidate.CoreStart)
        Call df.add("primer_ends", scalar:=candidate.CoreEnd)
        Call df.add("core_span", scalar:=StringFormats.Lanudry(candidate.Span))
        Call df.add("extends_start", scalar:=candidate.ExtendedStart)
        Call df.add("extends_end", scalar:=candidate.ExtendedEnd)
        Call df.add("extends_span", scalar:=StringFormats.Lanudry(candidate.ExtensionLength))
        Call df.add("gene_id", core_id.JoinIterates(ext_id))
        Call df.add("gene_left", core_left.JoinIterates(ext_left))
        Call df.add("gene_right", core_right.JoinIterates(ext_right))
        Call df.add("gene_length", core_len.JoinIterates(ext_len))
        Call df.add("gene_strand", core_strand.JoinIterates(ext_strand))
        Call df.add("type", core_type.JoinIterates(ext_type))
        Call df.add("num_primer_hits", scalar:=candidate.SupportingHits.Count)
        Call df.add("primer_hits", scalar:=hits)

        If fna IsNot Nothing Then
            With New ChunkSlicer(fna)
                Call df.add("gene_seq", candidate.GenesInCoreRegion _
                       .JoinIterates(candidate.GenesInExtendedRegion) _
                       .Select(Function(gene)
                                   Return .SliceRegionSite(gene.left, gene.Length)
                               End Function))
            End With
        End If

        Return df
    End Function

    ''' <summary>
    ''' find the candidate PCR primer design regions from a set of the blastn hits
    ''' </summary>
    ''' <param name="blastHits">
    ''' a collection of the blastn hit records for find the candidate primer 
    ''' design regions, which can be a pipeline object or a vector of the 
    ''' <see cref="HitRecord"/> object.
    ''' </param>
    ''' <param name="maxCoreSpan">
    ''' the maximum span size in nucleotide of the core primer region.
    ''' </param>
    ''' <param name="eval_cutoff">
    ''' the maximum e-value threshold of the accepted blastn hit records.
    ''' </param>
    ''' <param name="primerIds">
    ''' a character vector of the primer id for filter the input blastn hits. 
    ''' If this parameter is not specified, then all of the input blastn hits 
    ''' will be used for find the candidate regions.
    ''' </param>
    ''' <param name="genome">
    ''' a <see cref="GFFTable"/> genomics feature annotation table, which is 
    ''' used for annotate the gene context information of the candidate primer 
    ''' regions. If this parameter is not specified, then no gene context 
    ''' information will be calculated.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="CandidateRegion"/> object, each element in 
    ''' the returned vector is one candidate PCR primer design region with its 
    ''' gene context information;
    ''' 
    ''' this function returns a R# error message object if the input blastn hits 
    ''' data can not be cast to a collection of the <see cref="HitRecord"/> 
    ''' object.
    ''' </returns>
    <ExportAPI("primer_regions")>
    <RApiReturn(GetType(CandidateRegion))>
    Public Function find_primers_region(<RRawVectorArgument>
                                        blastHits As Object,
                                        Optional maxCoreSpan As Integer = ISequenceModel.MB,
                                        Optional eval_cutoff As Double = 1,
                                        <RRawVectorArgument>
                                        Optional primerIds As Object = Nothing,
                                        Optional genome As GFFTable = Nothing,
                                        Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of HitRecord)(blastHits, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim siteHits As IEnumerable(Of HitRecord) = pull.populates(Of HitRecord)(env)
        Dim candidates As CandidateRegion() = CandidateRegion.FindCandidateRegions(
            siteHits, maxCoreSpan, eval_cutoff, CLRVector.asCharacter(primerIds))
        Dim context As Dictionary(Of String, GenomeContext(Of Feature)) = genome _
            .GetChromosomes(mRNA:=True) _
            .ToDictionary(Function(c) c.name,
                          Function(c)
                              Return New GenomeContext(Of Feature)(c, c.name)
                          End Function)

        Call CandidateRegion.CalculateExtensions(candidates, context)

        Return candidates
    End Function

End Module

