
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.PanGenome
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("pangenome")>
Module pangenome

    Sub Main()
        Call Converts.makeDataframe.addHandler(GetType(PAVTable()), AddressOf pav_df)
        Call Converts.makeDataframe.addHandler(GetType(SVTable()), AddressOf sv_df)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function sv_df(sv As SVTable(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .rownames = sv.Select(Function(a) a.SV_ID).ToArray,
            .columns = New Dictionary(Of String, Array)
        }

        Call df.add(NameOf(SVTable.Type), From svi As SVTable In sv Let label As String = svi.Type.ToString Select label)
        Call df.add(NameOf(SVTable.GenomeName), From svi As SVTable In sv Select svi.GenomeName)
        Call df.add(NameOf(SVTable.FamilyID), From svi As SVTable In sv Select svi.FamilyID)
        Call df.add(NameOf(SVTable.RelatedGenes), From svi As SVTable In sv Select svi.RelatedGenes.JoinBy("; "))
        Call df.add(NameOf(SVTable.Description), From svi As SVTable In sv Select svi.Description)

        Return df
    End Function

    <RGenericOverloads("as.data.frame")>
    Private Function pav_df(pav As PAVTable(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .rownames = pav.Select(Function(a) a.FamilyID).ToArray,
            .columns = New Dictionary(Of String, Array)
        }
        Dim genome_names As String() = pav _
            .Select(Function(a) a.PAV.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray

        Call df.add("cluster_genes", From a As PAVTable In pav Select a.ClusterGenes.JoinBy("; "))
        Call df.add("size", From a As PAVTable In pav Select a.ClusterGenes.Length)

        For Each genome_name As String In genome_names
            Call df.add(genome_name, From a As PAVTable In pav Select a(genome_name))
        Next

        Return df
    End Function

    ''' <summary>
    ''' Load the pangenome analysis context
    ''' </summary>
    ''' <param name="genomes">should be a collection of the genome GFF feature tables</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("build_context")>
    <RApiReturn(GetType(GenomeAnalyzer))>
    Public Function build_context(<RRawVectorArgument> genomes As Object,
                                  Optional soft_core_threshold As Double = 0.95,
                                  Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of GFFTable)(genomes, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim genomeList As IEnumerable(Of GFFTable) = pull.populates(Of GFFTable)(env)
        Dim context As New GenomeAnalyzer(genomeList) With {
            .SoftCoreThreshold = soft_core_threshold
        }

        Return context
    End Function

    <ExportAPI("analysis")>
    <RApiReturn(GetType(PanGenomeResult))>
    Public Function analysis(pangenome As GenomeAnalyzer, <RRawVectorArgument> orthologSet As list, Optional env As Environment = Nothing) As Object
        Dim orthologDict As New Dictionary(Of String, BiDirectionalBesthit())

        For Each compareMap In orthologSet.slotKeys
            Dim linkSet As BiDirectionalBesthit() = orthologSet _
                .getValue(Of BiDirectionalBesthit())(compareMap, env) _
                .Where(Function(link) link.level <> Levels.NA AndAlso link.level <> Levels.SBH) _
                .ToArray

            For i As Integer = 0 To linkSet.Length - 1
                linkSet(i).QueryName = HeaderFormats.TrimAccessionVersion(linkSet(i).QueryName)
                linkSet(i).HitName = HeaderFormats.TrimAccessionVersion(linkSet(i).HitName)
            Next

            Call orthologDict.Add(compareMap, linkSet)
        Next

        Return pangenome.AnalyzePanGenome(orthologDict)
    End Function

    <ExportAPI("source_id")>
    Public Function set_sourceID(genome As GFFTable, source_name As String) As GFFTable
        genome.species = source_name
        Return genome
    End Function

    <ExportAPI("report_html")>
    Public Function report_html(result As PanGenomeResult) As String
        Return PanGenomeReportGenerator.GenerateReport(result, PanGenomeReportGenerator.DefaultHtmlTemplate)
    End Function

    <ExportAPI("sv_table")>
    Public Function sv_table(result As PanGenomeResult) As SVTable()
        Return result.SVTable.ToArray
    End Function

    <ExportAPI("pav_table")>
    Public Function pav_table(result As PanGenomeResult) As PAVTable()
        Return result.PAVTable.ToArray
    End Function
End Module
