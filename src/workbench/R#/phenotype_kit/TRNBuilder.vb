Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("TRN.builder")>
Module TRNBuilder

    <ExportAPI("read.regprecise")>
    Public Function readRegPrecise(file As String) As TranscriptionFactors
        Return file.LoadXml(Of TranscriptionFactors)
    End Function

    <ExportAPI("read.footprints")>
    Public Function readFootprintSites(file As String) As FootprintSite()
        Return file.LoadCsv(Of FootprintSite)
    End Function

    <ExportAPI("write.regulations")>
    Public Function writeRegulationFootprints(regulationFootprints As Object, file$, Optional env As Environment = Nothing) As Object
        If regulationFootprints Is Nothing Then
            Return Internal.debug.stop("no content data provides!", env)
        ElseIf file.StringEmpty Then
            Return Internal.debug.stop("no file write information provides!", env)
        End If

        If TypeOf regulationFootprints Is RegulationFootprint() Then
            Return DirectCast(regulationFootprints, RegulationFootprint()).SaveTo(file)
        ElseIf TypeOf regulationFootprints Is pipeline AndAlso DirectCast(regulationFootprints, pipeline).elementType Like GetType(RegulationFootprint) Then
            Using writer As New WriteStream(Of RegulationFootprint)(file)
                For Each edge As RegulationFootprint In DirectCast(regulationFootprints, pipeline).populates(Of RegulationFootprint)
                    Call writer.Flush(edge)
                Next
            End Using

            Return True
        Else
            Return Internal.debug.stop($"invalid data type for write: {regulationFootprints.GetType.FullName }", env)
        End If
    End Function

    <ExportAPI("motif.raw")>
    Public Function exportRegPrecise(regprecise As TranscriptionFactors) As list
        Return regprecise _
            .ExportByFamily _
            .ToDictionary(Function(name) name.Key,
                          Function(family)
                              Return CObj(family.Value)
                          End Function) _
            .DoCall(Function(data)
                        Return New list With {.slots = data}
                    End Function)
    End Function

    <ExportAPI("regulation.footprint")>
    Public Function RegulationFootprint(<RRawVectorArgument>
                                        regulators As Object,
                                        motifLocis As FootprintSite(),
                                        regprecise As TranscriptionFactors,
                                        Optional env As Environment = Nothing) As pipeline
        If regulators Is Nothing Then
            Return Nothing
        End If

        Dim regulatorMaps As BestHit()

        If TypeOf regulators Is BestHit() Then
            regulatorMaps = DirectCast(regulators, BestHit())
        ElseIf TypeOf regulators Is pipeline AndAlso DirectCast(regulators, pipeline).elementType Like GetType(BestHit) Then
            regulatorMaps = DirectCast(regulators, pipeline) _
                .populates(Of BestHit) _
                .ToArray
        Else
            Return Internal.debug.stop($"invalid regulator maps: '{regulators.GetType.FullName }'!", env)
        End If

        Dim regulatorTable As New Dictionary(Of String, List(Of (genome As BacteriaRegulome, Regulator)))
        Dim family$
        Dim regulatorMapTable As Dictionary(Of String, BestHit()) = regulatorMaps _
            .GroupBy(Function(map)
                         Return map.HitName.Split(":"c).Last
                     End Function) _
            .ToDictionary(Function(hit) hit.Key,
                          Function(group)
                              Return group.ToArray
                          End Function)

        For Each genome As BacteriaRegulome In regprecise.AsEnumerable
            For Each regulon As Regulator In genome.regulome _
                .AsEnumerable _
                .Where(Function(reg)
                           Return reg.type = Types.TF
                       End Function)

                family = regulon.family _
                    .Split("/"c, "\"c) _
                    .First

                If Not regulatorTable.ContainsKey(family) Then
                    regulatorTable.Add(family, New List(Of (genome As BacteriaRegulome, Regulator)))
                End If

                regulatorTable(family).Add((genome, regulon))
            Next
        Next

        Return Iterator Function() As IEnumerable(Of RegulationFootprint)
                   Dim regulatorList As List(Of (BacteriaRegulome, Regulator))
                   Dim regulation As RegulationFootprint
                   Dim edgeKeyIndex As New Index(Of String)
                   Dim edgeKey$

                   For Each gene As FootprintSite In motifLocis
                       For Each familyName As String In gene.src
                           regulatorList = regulatorTable(familyName)

                           For Each regulator As (genome As BacteriaRegulome, reg As Regulator) In regulatorList _
                               .Where(Function(reg)
                                          Return regulatorMapTable.ContainsKey(reg.Item2.LocusId)
                                      End Function)

                               For Each hit As BestHit In regulatorMapTable(regulator.reg.LocusId)
                                   edgeKey = $"{hit.QueryName}->{gene.gene}"

                                   If edgeKey Like edgeKeyIndex Then
                                       Continue For
                                   Else
                                       edgeKeyIndex.Add(edgeKey)
                                   End If

                                   regulation = New RegulationFootprint With {
                                       .family = familyName,
                                       .effector = regulator.reg.effector,
                                       .regulator = hit.QueryName,
                                       .biological_process = regulator.reg.biological_process.JoinBy(", "),
                                       .mode = regulator.reg.regulationMode,
                                       .regprecise = regulator.reg.regulog.name,
                                       .regulog = regulator.reg.regulog.name,
                                       .regulated = gene.gene,
                                       .species = regulator.genome.genome.name,
                                       .identities = hit.identities
                                   }

                                   Yield regulation
                               Next
                           Next
                       Next
                   Next
               End Function() _
 _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function
End Module
