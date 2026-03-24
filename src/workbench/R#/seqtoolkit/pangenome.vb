
Imports Microsoft.VisualBasic.CommandLine.Reflection
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

    ''' <summary>
    ''' Load the pangenome analysis context
    ''' </summary>
    ''' <param name="genomes">should be a collection of the genome GFF feature tables</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("build_context")>
    <RApiReturn(GetType(GenomeAnalyzer))>
    Public Function build_context(<RRawVectorArgument> genomes As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of GFFTable)(genomes, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim genomeList As IEnumerable(Of GFFTable) = pull.populates(Of GFFTable)(env)
        Dim context As New GenomeAnalyzer(genomeList)

        Return context
    End Function

    <ExportAPI("analysis")>
    <RApiReturn(GetType(PanGenomeResult))>
    Public Function analysis(pangenome As GenomeAnalyzer, <RRawVectorArgument> orthologSet As list, Optional env As Environment = Nothing) As Object
        Dim orthologDict As New Dictionary(Of String, BiDirectionalBesthit())

        For Each compareMap In orthologSet.slotKeys
            Dim linkSet As BiDirectionalBesthit() = orthologSet _
                .getValue(Of BiDirectionalBesthit())(compareMap, env) _
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
End Module
