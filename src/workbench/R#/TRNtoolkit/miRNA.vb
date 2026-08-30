#Region "Microsoft.VisualBasic::f9265fe3cb6a71533f5259238d4746c2, R#\TRNtoolkit\miRNA.vb"

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

    '   Total Lines: 79
    '    Code Lines: 56 (70.89%)
    ' Comment Lines: 9 (11.39%)
    '    - Xml Docs: 88.89%
    ' 
    '   Blank Lines: 14 (17.72%)
    '     File Size: 3.30 KB


    ' Module miRNA
    ' 
    '     Function: intersect_targets, miRNA_targets, psRNATarget_clr, TargetFinder
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' miRNA/siRNA target gene prediction toolkit
''' </summary>
''' 
''' <remarks>
''' This R# package module provides the toolkit for predict the target genes of 
''' the miRNA/siRNA small RNA sequence:
''' 
''' + ``psRNATarget`` and ``TargetFinder``: create the miRNA target site match 
'''   algorithm object;
''' + ``miRNA_targets``: run the target site match of the given miRNA sequence 
'''   against the candidate target mRNA/CDS sequence collection;
''' + ``intersect_targets``: take the intersection of the two algorithm result for 
'''   create the high confidence target site set.
''' 
''' the generated match result is a collection of the <see cref="siRNAHit"/> 
''' object, which can be converted to a data frame via the ``as.data.frame`` api, 
''' or be saved as a csv table file via the ``write.csv`` api.
''' </remarks>
<Package("miRNA")>
Module miRNA

    ''' <summary>
    ''' Initialize the internal environment of this R# package module
    ''' </summary>
    ''' 
    ''' <remarks>
    ''' this function is invoked automatically at the start of the R# runtime 
    ''' environment, it just registers the data frame cast handler of the 
    ''' <see cref="siRNAHit"/> match result collection, so that the match result can 
    ''' be converted to a data frame via the ``as.data.frame`` api.
    ''' </remarks>
    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(siRNAHit()), AddressOf targetMatchesResult)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function targetMatchesResult(hits As siRNAHit(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call df.add("miRNA", From h As siRNAHit In hits Select h.miRNA)
        Call df.add("target", From h As siRNAHit In hits Select h.Target)
        Call df.add("start", From h As siRNAHit In hits Select h.StartSite)
        Call df.add("end", From h As siRNAHit In hits Select h.EndSite)
        Call df.add("length", From h As siRNAHit In hits Select h.Length)
        Call df.add("expectation", From h As siRNAHit In hits Select h.Expectation)
        Call df.add("mismatch", From h As siRNAHit In hits Select h.MismatchCount)
        Call df.add("wobble", From h As siRNAHit In hits Select h.WobbleCount)
        Call df.add("gaps", From h As siRNAHit In hits Select h.GapCount)
        Call df.add("alignment", From h As siRNAHit In hits Select h.Alignment)
        Call df.add("translation_inhibition", From h As siRNAHit In hits Select h.TranslationInhibition)
        Call df.add("source", From h As siRNAHit In hits Select h.Source)

        Return df
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="version"></param>
    ''' <param name="max_expectation"></param>
    ''' <returns></returns>
    <ExportAPI("psRNATarget")>
    Public Function psRNATarget_clr(Optional version As psRNATarget.Schema = psRNATarget.Schema.V2_2017, Optional max_expectation As Double = 5.0) As psRNATarget
        Return New psRNATarget With {.Version = version, .MaxExpectation = max_expectation}
    End Function

    <ExportAPI("TargetFinder")>
    Public Function TargetFinder(Optional score_cutoff As Double = 5.0) As TargetFinder
        Return New TargetFinder With {.ScoreCutoff = score_cutoff}
    End Function

    ''' <summary>
    ''' make matches of the miRNA target genes
    ''' </summary>
    ''' <param name="mapper"></param>
    ''' <param name="miRNAs">a collection of the miRNA sequence</param>
    ''' <param name="targets">a collection of the mRNA/CDS sequence of the candidate genes</param>
    ''' <param name="env"></param>
    ''' <returns>a set of the miRNA to target gene matches result, a match result network edges with match score as weights</returns>
    ''' <example>
    ''' imports "miRNA" from "TRNtoolkit";
    ''' imports "bioseq.fasta" from "seqtoolkit";
    ''' 
    ''' let sirna = fasta("UGACGUGACUGACGUGACUGA", attrs = c("demo-miRNA"));
    ''' let genes = read.fasta("candidates.fa");
    ''' 
    ''' let psr = miRNA_targets(psRNATarget(), sirna, targets = genes);
    ''' let tfd = miRNA_targets(TargetFinder(), sirna, targets = genes);
    ''' 
    ''' let hi_conf = as.data.frame(intersect_targets(psr, tfd)); 
    ''' 
    ''' print(hi_conf, max.print = 6);
    ''' 
    ''' write.csv(hi_conf, file = "miRNA_targets_high_confidence.csv");
    ''' </example>
    <ExportAPI("miRNA_targets")>
    <RApiReturn(GetType(siRNAHit))>
    Public Function miRNA_targets(mapper As miRNAMapper,
                                  <RRawVectorArgument(GetType(FastaSeq))> miRNAs As Object,
                                  <RRawVectorArgument(GetType(FastaSeq))> targets As Object,
                                  Optional env As Environment = Nothing) As Object

        Dim miRNAList = GetFastaSeq(miRNAs, env)
        Dim targetDb = GetFastaSeq(targets, env).SafeQuery.ToArray

        If miRNAList Is Nothing Then
            Return Nothing
        ElseIf targetDb.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim result As New List(Of siRNAHit)

        For Each miRNA As FastaSeq In miRNAList
            Call result.AddRange(mapper.Run(miRNA, targetDb))
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' --- High-confidence intersection (psRNATarget ∩ TargetFinder) ---
    ''' </summary>
    ''' <param name="psRNATarget"></param>
    ''' <param name="TargetFinder"></param>
    ''' <param name="site_tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("intersect_targets")>
    <RApiReturn(GetType(siRNAHit))>
    Public Function intersect_targets(<RRawVectorArgument(GetType(siRNAHit))> psRNATarget As Object,
                                      <RRawVectorArgument(GetType(siRNAHit))> TargetFinder As Object,
                                      Optional site_tolerance As Integer = 3,
                                      Optional env As Environment = Nothing) As Object
        ' ---- 交集（高置信靶标）----
        Dim merger As New Intersection() With {.SiteTolerance = site_tolerance}
        Dim psrnaHits As PipeIterator(Of siRNAHit) = pipeline.Stream(Of siRNAHit)(psRNATarget, env)
        Dim tfHits As PipeIterator(Of siRNAHit) = pipeline.Stream(Of siRNAHit)(TargetFinder, env)

        If psrnaHits.isError Then
            Return psrnaHits.getError
        ElseIf tfHits.isError Then
            Return tfHits.getError
        End If

        Dim intersect As siRNAHit() = merger.Merge(psrnaHits, tfHits).ToArray

        Return intersect
    End Function
End Module

