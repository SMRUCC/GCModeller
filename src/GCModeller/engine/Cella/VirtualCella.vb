Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.ModelLoader
Imports SMRUCC.genomics.Metagenomics

Public Class VirtualCella

    Public Property taxonomy_info As Taxonomy
    Public Property grn As GeneRegulatoryNetwork
    Public Property metabolic As MetabolicNetwork
    Public Property translation As TranslationSystem
    Public Property transportation As TransportSystem
    Public Property turnover As TurnoverSystem

    Sub New()
    End Sub

    Sub RunStep(iteration As Integer)
        If iteration Mod 3 = 0 Then
            Call grn.RunStep()
        End If
        If iteration Mod 2 = 0 Then
            Call translation.RunStep()
        End If

        Call metabolic.RunStep()
        Call transportation.RunStep()
        Call turnover.RunStep()
    End Sub

    Public Shared Function FromModel(cell As VirtualCell, define As Definition, dynamics As FluxBaseline) As VirtualCella
        Dim grn As New List(Of RegulatoryLink)
        Dim cella As New VirtualCella With {
            .taxonomy_info = cell.taxonomy
        }
        Dim operonIndex As Dictionary(Of String, TranscriptUnit) = cell.genome.GetAllOperon
        Dim loader As New Loader(define, dynamics)
        Dim modelData = cell.CreateModel
        Dim metabNetwork = loader.GetMetabolismNetworkLoader.CreateFlux(modelData).ToDictionary(Function(a) a.ID)
        Dim metabolites = loader.GetMetabolismNetworkLoader.MassTable
        Dim fluxIndex = loader.GetFluxIndex
        Dim transport = fluxIndex(MetabolismNetworkLoader.MembraneTransporter).Select(Function(id) metabNetwork(id)).ToArray
        Dim metabolic = fluxIndex(NameOf(MetabolismNetworkLoader)).Select(Function(id) metabNetwork(id)).ToArray

        For Each trn As transcription In cell.genome.regulations.SafeQuery
            Call grn.Add(New RegulatoryLink With {
                .target_operon = trn.operonId,
                .TFBS_id = trn.motif.ToString,
                .TF_family = trn.motif.family,
                .TF_id = trn.regulator,
                .effector = New Dictionary(Of String, Effector) From {
                    {trn.effector, If(trn.mode = "activator", Effector.Activator, Effector.Inhibitor)}
                },
                .regulate_genes = operonIndex(trn.operonId).genes _
                    .Select(Function(g) g.locus_tag) _
                    .ToArray
            })
        Next

        cella.transportation = New TransportSystem(metabolites, transport, cella)
        cella.metabolic = New MetabolicNetwork(metabolites, metabolic, cella)
        cella.grn = New GeneRegulatoryNetwork(cella, grn)

        Return cella
    End Function

End Class
