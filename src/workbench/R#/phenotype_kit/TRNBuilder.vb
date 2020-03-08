Imports Microsoft.VisualBasic.CommandLine.Reflection
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

        Return Iterator Function() As IEnumerable(Of RegulationFootprint)

               End Function() _
 _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function
End Module
