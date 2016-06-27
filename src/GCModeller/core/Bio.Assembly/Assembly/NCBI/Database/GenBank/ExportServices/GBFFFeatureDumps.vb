Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.GenBank

    Public Module GBFFFeatureDumps

        ''' <summary>
        ''' Dump feature sites information data into a tabular dataframe.
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <param name="features"></param>
        ''' <param name="dumpAll"></param>
        ''' <returns></returns>
        <ExportAPI("Features.Dump")>
        <Extension>
        Public Function FeatureDumps(gb As GBFF.File,
                                     Optional features As String() = Nothing,
                                     Optional dumpAll As Boolean = False) As GeneDumpInfo()
            If dumpAll Then
                Dim fs As Feature() =
                    LinqAPI.Exec(Of Feature) <= From x As Feature
                                                In gb.Features
                                                Where x.ContainsKey("gene")
                                                Select x
                Return fs.__dumpCDS
            End If

            If features Is Nothing Then features = {"5'UTR", "CDS", "regulatory", "misc_feature", "3'UTR"}

            Dim result As New List(Of GeneDumpInfo)

            For Each feature As String In features
                Dim fs As Feature() = gb.Features.ListFeatures(feature)
                result += _dumpMethods(feature)(fs)
            Next

            Return result.ToArray
        End Function

#Region "Dump Methods"

        Dim _dumpMethods As Dictionary(Of String, Func(Of Feature(), GeneDumpInfo())) =
            New Dictionary(Of String, Func(Of Feature(), GeneDumpInfo())) From {
                {"5'UTR", AddressOf __dump5UTRs},
                {"3'UTR", AddressOf __dump3UTRs},
                {"CDS", AddressOf __dumpCDS},
                {"regulatory", AddressOf __dumpRegulatory},
                {"misc_feature", AddressOf __dumpMiscFeature}
        }

        Private Function __dumpMiscFeature(features As Feature()) As GeneDumpInfo()
            Dim dump As GeneDumpInfo() =
                LinqAPI.Exec(Of Feature, GeneDumpInfo)(features) <=
                    Function(feature As Feature) New GeneDumpInfo With {
                        .COG = "misc_feature",
                        .Function = feature("note"),
                        .CommonName = feature("note"),
                        .Location = feature.Location.ContiguousRegion,
                        .LocusID = feature("locus_tag"),
                        .GeneName = feature("gene") & "_mics_feature",
                        .Translation = feature("translation"),
                        .ProteinId = feature("protein_id"),
                        .CDS = feature.SequenceData
                    }
            Return dump
        End Function

        Private Function __dumpRegulatory(features As Feature()) As GeneDumpInfo()
            Dim dump As GeneDumpInfo() = features.ToArray(
                Function(feature) New GeneDumpInfo With {
                    .COG = "regulatory",
                    .Function = feature("regulatory_class"),
                    .CommonName = feature("note"),
                    .Location = feature.Location.ContiguousRegion,
                    .LocusID = feature("locus_tag"),
                    .GeneName = feature("gene") & "_regulatory",
                    .Translation = feature("translation"),
                    .ProteinId = feature("protein_id"),
                    .CDS = feature.SequenceData
               })
            Return dump
        End Function

        <Extension>
        Private Function __dumpCDS(features As Feature()) As GeneDumpInfo()
            Dim dump As GeneDumpInfo() = features.ToArray(
                Function(feature) New GeneDumpInfo With {
                    .COG = "CDS",
                    .Function = feature("function"),
                    .CommonName = feature("note"),
                    .Location = feature.Location.ContiguousRegion,
                    .LocusID = feature("locus_tag"),
                    .GeneName = feature("gene"),
                    .Translation = feature("translation"),
                    .ProteinId = feature("protein_id"),
                    .CDS = feature.SequenceData
               })
            Return dump
        End Function

        <Extension> Private Function __dump5UTRs(features As Feature()) As GeneDumpInfo()
            Dim dump As GeneDumpInfo() = features.ToArray(
                Function(feature) New GeneDumpInfo With {
                    .COG = "5'UTR",
                    .Function = feature("function"),
                    .CommonName = feature("note"),
                    .Location = feature.Location.ContiguousRegion,
                    .LocusID = $"5'UTR_{feature.Location.ContiguousRegion.Left}..{feature.Location.ContiguousRegion.Right}",
                    .GeneName = $"5'UTR_{feature.Location.ContiguousRegion.Left}..{feature.Location.ContiguousRegion.Right}",
                    .CDS = feature.SequenceData
                })
            Return dump
        End Function

        <Extension> Private Function __dump3UTRs(features As Feature()) As GeneDumpInfo()
            Dim dump As GeneDumpInfo() = features.ToArray(
                Function(feature) New GeneDumpInfo With {
                    .COG = "3'UTR",
                    .Function = feature("function"),
                    .CommonName = feature("note"),
                    .Location = feature.Location.ContiguousRegion,
                    .LocusID = $"3'UTR_{feature.Location.ContiguousRegion.Left}..{feature.Location.ContiguousRegion.Right}",
                    .GeneName = $"3'UTR_{feature.Location.ContiguousRegion.Left}..{feature.Location.ContiguousRegion.Right}",
                    .CDS = feature.SequenceData
                })
            Return dump
        End Function
#End Region
    End Module
End Namespace