Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Trinity
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

Namespace ModelLoader

    Public Class TranslationEvents : Implements Enumeration(Of String)

        ReadOnly cdLoader As CentralDogmaFluxLoader
        ReadOnly cell As CellularModule

        Dim proteinMatrix As Dictionary(Of String, ProteinComposition)
        Dim polypeptides As New List(Of String)
        Dim charged_tRNA As Dictionary(Of String, String)
        Dim uncharged_tRNA As Dictionary(Of String, String)

        Public ReadOnly Property MassTable As MassTable
            Get
                Return cdLoader.MassTable
            End Get
        End Property

        Private ReadOnly Property loader As Loader
            Get
                Return cdLoader.loader
            End Get
        End Property

        Sub New(cdLoader As CentralDogmaFluxLoader)
            Me.charged_tRNA = cdLoader.charged_tRNA
            Me.uncharged_tRNA = cdLoader.uncharged_tRNA
            Me.cdLoader = cdLoader
            Me.cell = cdLoader.cellModel
            Me.proteinMatrix = ProteinMatrixIndex(cell.Genotype.ProteinMatrix)
        End Sub

        Private Shared Function ProteinMatrixIndex(p As IEnumerable(Of ProteinComposition)) As Dictionary(Of String, ProteinComposition)
            Dim proteinGroups = p.GroupBy(Function(r) r.proteinID)
            Dim index As New Dictionary(Of String, ProteinComposition)
            Dim duplicateds As New List(Of String)

            For Each group As IGrouping(Of String, ProteinComposition) In proteinGroups
                If group.Count > 1 Then
                    Call duplicateds.Add(group.Key)
                End If

                Call index.Add(group.Key, group.First)
            Next

            If duplicateds.Any Then
                Dim uniq = duplicateds.Distinct.ToArray

                If redirectWarning() Then
                    Call $"found {uniq.Length} duplicated protein peptide chains object: {uniq.JoinBy(", ")}!".warning
                    Call $"found {uniq.Length} duplicated protein peptide chains object: {uniq.Concatenate(",", max_number:=13)}!".debug
                Else
                    Call $"found {uniq.Length} duplicated protein peptide chains object: {uniq.Concatenate(",", max_number:=13)}!".warning
                End If
            End If

            Return index
        End Function

        Public Iterator Function GetEvents(cd As CentralDogma, cellular_id As String) As IEnumerable(Of Channel)
            Dim r70s_mRNA As String = $"70s_mRNA({cd.geneID})"

            Call MassTable.addNew(r70s_mRNA, MassRoles.compound, cellular_id)

            ' 30s + mRNA + 50s + GTP = 70s_mRNA + GDP + Pi
            Dim phase1 = initial_assembling(cd, r70s_mRNA, cellular_id)
            ' 70s_mRNA + N * charged-aa-tRNA = 70s_mRNA + polypeptide + N * aa-tRNA + N * Pi
            Dim phase2 = peptide_elongation(cd, r70s_mRNA, cellular_id)
            ' 70s_mRNA = 30s + mRNA + 50s + Pi
            Dim phase3 = ribosomal_recycle(cd, r70s_mRNA, cellular_id)

            polypeptides += cd.polypeptide

            loader.fluxIndex("translation").Add(phase1.ID)
            loader.fluxIndex("translation").Add(phase2.ID)
            loader.fluxIndex("translation").Add(phase3.ID)

            Yield phase1
            Yield phase2
            Yield phase3
        End Function

        Private Function initial_assembling(cd As CentralDogma, r70s_mRNA$, cellular_id As String) As Channel
            ' 30s + mRNA + 50s + GTP = 70s_mRNA + GDP + Pi
            Dim rba30s As Variable = MassTable.variable(RibosomeAssembly.Ribosomal30s, cellular_id)
            Dim mRNA As Variable = MassTable.variable(cd.RNAName, cellular_id)
            Dim rba50s As Variable = MassTable.variable(RibosomeAssembly.Ribosomal50s, cellular_id)
            Dim GTP As Variable = MassTable.variable(loader.define.GTP, cellular_id)
            Dim GDP As Variable = MassTable.variable(loader.define.GDP, cellular_id)
            Dim Pi As Variable = MassTable.variable(loader.define.PI, cellular_id)
            Dim rba70s_mRNA As Variable = MassTable.variable(r70s_mRNA, cellular_id)

            Return New Channel({rba30s, mRNA, rba50s, GTP}, {rba70s_mRNA, GDP, Pi}) With {
                .ID = $"[initial_assembling] 30s + mRNA({cd.geneID}) + 50s + GTP = 70s_mRNA({cd.geneID}) + GDP + Pi",
                .forward = Controls.StaticControl(100),
                .reverse = Controls.StaticControl(0),
                .bounds = New Boundary With {
                    .forward = loader.dynamics.translationCapacity,
                    .reverse = 0
                },
                .name = $"Initial assembling of 70s ribosome complex on mRNA {cd.RNAName} in cell {cellular_id}"
            }
        End Function

        Private Function peptide_elongation(gene As CentralDogma, r70s_mRNA$, cellular_id As String) As Channel
            Dim composit As ProteinComposition = If(proteinMatrix.ContainsKey(gene.geneID),
                proteinMatrix(gene.geneID),
                proteinMatrix.TryGetValue(gene.translation))

            If composit Is Nothing Then
                composit = MissingAAComposition(gene)
            End If

            Dim AAVector As NamedValue(Of Double)() = composit.Where(Function(i) i.Value > 0).ToArray
            ' 70s_mRNA + N * charged-aa-tRNA = 70s_mRNA + polypeptide + N * aa-tRNA + N * Pi
            Dim rba70s_mRNA As Variable = MassTable.variable(r70s_mRNA, cellular_id)
            Dim AAtRNA = AAVector _
                .Select(Function(aa)
                            Return MassTable.variable(charged_tRNA(aa.Name), cellular_id, aa.Value)
                        End Function) _
                .AsList
            Dim output As Variable() = translationUncharged(gene, gene.polypeptide)

            ' mRNA模板加上氨基酸消耗，请注意，在这里并不是直接消耗的氨基酸，而是消耗的已经荷载的tRNA分子
            Return New Channel(AAtRNA, output) With {
                .ID = $"[peptide_elongation] 70s_mRNA({gene.geneID}) + N * charged-aa-tRNA = 70s_mRNA({gene.geneID}) + polypeptide({gene.geneID}) + N * aa-tRNA + N * Pi",
                .name = $"Peptide elongation of protein {gene.polypeptide} for gene {gene.geneID} in cell {cellular_id}",
                .forward = New AdditiveControls With {.activation = {rba70s_mRNA}, .baseline = 1},
                .reverse = Controls.StaticControl(0),
                .bounds = New Boundary With {
                    .forward = loader.dynamics.translationCapacity,
                    .reverse = 0
                }
            }
        End Function

        Private Function ribosomal_recycle(cd As CentralDogma, r70s_mRNA$, cellular_id As String) As Channel
            ' 70s_mRNA = 30s + mRNA + 50s + Pi
            Dim rba70s_mRNA As Variable = MassTable.variable(r70s_mRNA, cellular_id)
            Dim rba30s As Variable = MassTable.variable(RibosomeAssembly.Ribosomal30s, cellular_id)
            Dim mRNA As Variable = MassTable.variable(cd.RNAName, cellular_id)
            Dim rba50s As Variable = MassTable.variable(RibosomeAssembly.Ribosomal50s, cellular_id)
            Dim Pi As Variable = MassTable.variable(loader.define.PI, cellular_id)

            Return New Channel({rba70s_mRNA}, {rba30s, mRNA, rba50s, Pi}) With {
                .ID = $"[ribosomal_recycle] 70s_mRNA({cd.geneID}) = 30s + mRNA({cd.geneID}) + 50s + Pi",
                .name = $"Ribosomal recycle of 70s ribosome complex with mRNA of gene {cd.geneID} in cell {cellular_id}",
                .forward = Controls.StaticControl(100),
                .reverse = Controls.StaticControl(0),
                .bounds = New Boundary With {
                    .forward = 100,
                    .reverse = 0
                }
            }
        End Function

        Private Function MissingAAComposition(gene As CentralDogma) As ProteinComposition
            Dim warn As String = $"missing protein translation composition for gene: {gene.geneID}"

            Call warn.warning
            Call warn.debug

            Return New ProteinComposition With {
                .A = 1,
                .C = 1,
                .D = 1,
                .E = 1,
                .F = 1,
                .G = 1,
                .H = 1,
                .I = 1,
                .K = 1,
                .L = 1,
                .M = 1,
                .N = 1,
                .O = 0,
                .P = 1,
                .proteinID = gene.translation,
                .Q = 1,
                .R = 1,
                .S = 1,
                .T = 1,
                .U = 1,
                .V = 1,
                .W = 1,
                .Y = 1
            }
        End Function

        Private Function translationUncharged(gene As CentralDogma, peptide$) As Variable()
            Dim cellular_id As String = cell.CellularEnvironmentName
            Dim composit As ProteinComposition = If(
                proteinMatrix.ContainsKey(gene.geneID),
                proteinMatrix(gene.geneID),
                proteinMatrix.TryGetValue(gene.translation))

            If composit Is Nothing Then
                composit = MissingAAComposition(gene)
            End If

            Dim AAVector As NamedValue(Of Double)() = composit.Where(Function(i) i.Value > 0).ToArray
            Dim AAtRNA = AAVector _
                .Select(Function(aa)
                            Return MassTable.variable(uncharged_tRNA(aa.Name), cellular_id, aa.Value)
                        End Function) _
                .AsList
            Dim mRNA As String = gene.RNAName
            Dim N As Integer = Aggregate aa As NamedValue(Of Double)
                               In AAVector
                               Into Sum(aa.Value)
            ' 20250831
            ' template of mRNA is not working in ODEs
            ' restore the mRNA in product list at here
            Return AAtRNA + MassTable.variable("*" & peptide, cellular_id) + MassTable.variable(loader.define.PI, cellular_id, N)
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of String) Implements Enumeration(Of String).GenericEnumerator
            For Each id As String In polypeptides
                Yield id
            Next
        End Function
    End Class
End Namespace