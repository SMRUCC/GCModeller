Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Regtransbase.WebServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports TF = SMRUCC.genomics.Data.Regprecise.Regulator

Public Class RegPreciseScan

    ''' <summary>
    ''' regulation mapping
    ''' 
    ''' [tfbs -> TF]
    ''' </summary>
    ReadOnly RegPrecise As New Dictionary(Of String, String)

    Private Sub New()
    End Sub

    Public Iterator Function CreateFootprints(regulators As IEnumerable(Of RegpreciseBBH), tfbs As IEnumerable(Of MotifMatch)) As IEnumerable(Of RegulationFootprint)
        Dim TFmapping As Dictionary(Of String, RegpreciseBBH()) = regulators.DoCall(AddressOf createMapping)

        For Each site As MotifMatch In tfbs
            For Each id As String In site.seeds
                Dim tfMaps As RegpreciseBBH() = TFmapping.TryGetValue(RegPrecise.TryGetValue(id))

                For Each tf As RegpreciseBBH In tfMaps.SafeQuery
                    Yield New RegulationFootprint With {
                        .biological_process = tf.pathway,
                        .effector = tf.effectors.JoinBy("; "),
                        .family = tf.family,
                        .identities = tf.identities * site.identities,
                        .site = id,
                        .mode = tf.regulationMode,
                        .regprecise = tf.HitName,
                        .regulated = site.title,
                        .regulator = tf.QueryName,
                        .replicon = "chr1",
                        .regulog = tf.term,
                        .sequenceData = site.segment
                    }
                Next
            Next
        Next
    End Function

    Private Shared Function createMapping(regulators As IEnumerable(Of RegpreciseBBH)) As Dictionary(Of String, RegpreciseBBH())
        Return regulators _
            .Where(Function(tf)
                       Return tf.HitName <> BBHParser.HITS_NOT_FOUND
                   End Function) _
            .GroupBy(Function(map) map.HitName.Split(":"c).Last) _
            .ToDictionary(Function(tf) tf.Key,
                          Function(maps)
                              Return maps.ToArray
                          End Function)
    End Function

    Public Shared Function CreateFromRegPrecise(regDb As IEnumerable(Of BacteriaRegulome)) As RegPreciseScan
        Dim load As New RegPreciseScan

        For Each genome As BacteriaRegulome In regDb
            For Each regulator As TF In genome.regulome.regulators.Where(Function(reg) reg.type = Types.TF)
                For Each site As MotifFasta In regulator.regulatorySites
                    load.RegPrecise($"{site.locus_tag}:{site.position}") = regulator.LocusId
                Next
            Next
        Next

        Return load
    End Function
End Class
