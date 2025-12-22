Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

Namespace ModelLoader

    Public Class TranscriptionEvents

        ReadOnly cdLoader As CentralDogmaFluxLoader
        ReadOnly cell As CellularModule

        Private ReadOnly Property loader As Loader
            Get
                Return cdLoader.loader
            End Get
        End Property

        Private ReadOnly Property MassTable As MassTable
            Get
                Return loader.massTable
            End Get
        End Property

        Public ReadOnly Property polypeptides As String()

        Sub New(cdLoader As CentralDogmaFluxLoader)
            Me.cdLoader = cdLoader
            Me.cell = cdLoader.cellModel
        End Sub

        Private Shared Function RnaMatrixIndexing(m As IEnumerable(Of RNAComposition)) As Dictionary(Of String, RNAComposition)
            Dim geneGroups = m.GroupBy(Function(g) g.geneID)
            Dim index As New Dictionary(Of String, RNAComposition)
            Dim duplicateds As New List(Of String)

            For Each group As IGrouping(Of String, RNAComposition) In geneGroups
                If group.Count > 1 Then
                    Call duplicateds.Add(group.Key)
                End If

                Call index.Add(group.Key, group.First)
            Next

            If duplicateds.Any Then
                Dim uniq = duplicateds.Distinct.ToArray
                Dim warn As String = $"found {uniq.Length} duplicated RNA object: {uniq.JoinBy(", ")}!"

                Call warn.warning
                Call warn.debug
            End If

            Return index
        End Function

        Public Iterator Function GetEvents() As IEnumerable(Of Channel)
            Dim cellular_id As String = cell.CellularEnvironmentName
            Dim rnaMatrix As Dictionary(Of String, RNAComposition) = RnaMatrixIndexing(cell.Genotype.RNAMatrix)
            Dim TFregulations = cell.Regulations _
                .Where(Function(reg) reg.type = Processes.Transcription) _
                .GroupBy(Function(reg) reg.process) _
                .ToDictionary(Function(reg) reg.Key,
                              Function(reg)
                                  Return reg.ToArray
                              End Function)
            Dim TLregulations = cell.Regulations _
                .Where(Function(reg) reg.type = Processes.Translation) _
                .GroupBy(Function(reg) reg.process) _
                .ToDictionary(Function(reg) reg.Key,
                              Function(reg)
                                  Return reg.ToArray
                              End Function)
            ' try to defiane the total proteins as cellular growth status
            Dim cellular_growth As New StatusMapFactor(
                id:="cellular_growth",
                mass:=MassTable _
                    .GetRole(MassRoles.protein) _
                    .Where(Function(c) c.cellular_compartment = cellular_id) _
                    .Keys,
                compart_id:=cellular_id,
                env:=MassTable)
            Dim totalProteinCount As Integer = cell.Genotype.ProteinMatrix.Length
            Dim translation As New TranslationEvents(cdLoader)

            Call MassTable.AddOrUpdate(cellular_growth, cellular_growth.ID, cellular_id)
            Call MassTable.AddOrUpdate(New StatusMapFactor(id:="RNAp", mass:=$"cellular_growth@{cellular_id}", cellular_id, MassTable) With {.coefficient = 1 / totalProteinCount}, $"RNAp@{cellular_id}", cellular_id)
            Call MassTable.AddOrUpdate(New StatusMapFactor(id:="DNAp", mass:=$"cellular_growth@{cellular_id}", cellular_id, MassTable) With {.coefficient = 1 / totalProteinCount}, $"DNAp@{cellular_id}", cellular_id)

            Dim RNAp As Variable = MassTable.variable($"RNAp@{cellular_id}", cellular_id, 1)
            Dim DNAp As Variable = MassTable.variable($"DNAp@{cellular_id}", cellular_id, 1)
            Dim PPi As String = loader.define.PPI

            ' 在这里创建针对每一个基因的从转录到翻译的整个过程
            ' 之中的不同阶段的生物学过程的模型对象
            For Each cd As CentralDogma In TqdmWrapper.Wrap(cell.Genotype.centralDogmas, wrap_console:=App.EnableTqdm)
                Dim RPo_id As String = "RPo-" & cd.geneID
                Dim RPo_RNA_id As String = $"{RPo_id}-RNA_n"
                Dim regulations = TFregulations _
                    .TryGetValue(cd.transcript_unit) _
                    .JoinIterates(TFregulations.TryGetValue(cd.geneID)) _
                    .ToArray

                Call MassTable.addNew(RPo_id, MassRoles.compound, cellular_id)
                Call MassTable.addNew(RPo_RNA_id, MassRoles.compound, cellular_id)

                Dim RPo As Variable = MassTable.variable(RPo_id, cellular_id, 1)
                Dim RPo_RNA As Variable = MassTable.variable(RPo_RNA_id, cellular_id, 1)

                ' RNAP + DNA + DNA_P = RPo
                Dim phase1 = RPoGenerator(cd, regulations, RNAp, DNAp, RPo)
                ' RPo + n NTP = RPo·RNA_n + n PPi
                Dim phase2 = RNAElongation(cd, RPo, RPo_RNA, PPi, rnaMatrix)
                ' RPo·RNA_n = RPo + RNA
                Dim phase3 = terminateDisassembled(cd, RPo, RPo_RNA)

                Yield phase1
                Yield phase2
                Yield phase3

                ' 转录和翻译的反应过程都是不可逆的
                ' 翻译模板过程只针对CDS基因
                If Not cd.polypeptide Is Nothing Then
                    For Each flux As Channel In translation.GetEvents(cd, cellular_id)
                        loader.fluxIndex("translation").Add(flux.ID)
                        Yield flux
                    Next
                End If

                loader.fluxIndex("transcription").Add(phase1.ID)
                loader.fluxIndex("transcription").Add(phase2.ID)
                loader.fluxIndex("transcription").Add(phase3.ID)
            Next

            _polypeptides = translation.AsEnumerable.ToArray
        End Function

        ''' <summary>
        ''' phase 3
        ''' </summary>
        ''' <param name="cd"></param>
        ''' <param name="RPo"></param>
        ''' <param name="RPo_RNA"></param>
        ''' <returns></returns>
        Public Function terminateDisassembled(cd As CentralDogma, RPo As Variable, RPo_RNA As Variable) As Channel
            Dim cellular_id As String = RPo.mass.cellular_compartment
            Dim geneId As String = cd.geneID
            Dim productRNA As Variable = MassTable.variable(cd.RNAName, cellular_id)

            ' RPo·RNA_n = RPo + RNA
            RPo = MassTable.variable(RPo.mass.ID, cellular_id, RPo.coefficient)
            RPo_RNA = MassTable.variable(RPo_RNA.mass.ID, cellular_id, RPo_RNA.coefficient)

            Return New Channel({RPo_RNA}, {RPo, productRNA}) With {
                .ID = $"[termination] RPo·RNA_n({geneId}) = RPo({geneId}) + RNA",
                .forward = Controls.StaticControl(5),
                .reverse = Controls.StaticControl(0),
                .bounds = New Boundary With {
                    .forward = loader.dynamics.transcriptionCapacity,
                    .reverse = 0
                },
                .name = $"Termination of gene {cd.geneID} transcription in cell {cellular_id}"
            }
        End Function

        ''' <summary>
        ''' phase 2
        ''' </summary>
        ''' <param name="cd"></param>
        ''' <param name="RPo"></param>
        ''' <param name="RPo_RNA"></param>
        ''' <param name="PPi"></param>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        Private Function RNAElongation(cd As CentralDogma, RPo As Variable, RPo_RNA As Variable, PPi As String, matrix As Dictionary(Of String, RNAComposition)) As Channel
            Dim cellular_id As String = RPo.mass.cellular_compartment
            Dim geneId As String = cd.geneID
            Dim rna As RNAComposition = If(matrix.ContainsKey(geneId), matrix(geneId), New RNAComposition With {
                .A = 1,
                .C = 1,
                .G = 1,
                .U = 1,
                .geneID = geneId
            })
            Dim n As Integer = rna.Length
            Dim nPPi As Variable = MassTable.variable(PPi, cellular_id, n)

            ' RPo + n NTP = RPo·RNA_n + n PPi
            RPo = MassTable.variable(RPo.mass.ID, cellular_id, RPo.coefficient)
            RPo_RNA = MassTable.variable(RPo_RNA.mass.ID, cellular_id, RPo_RNA.coefficient)

            Dim NTPs As List(Of Variable) = rna _
                .Where(Function(i) i.Value > 0) _
                .Select(Function(base)
                            Dim baseName = loader.define.NucleicAcid(base.Name)
                            Return MassTable.variable(baseName, cellular_id, base.Value)
                        End Function).AsList

            Return New Channel(RPo + NTPs, {RPo_RNA, nPPi}) With {
                .ID = $"[elongation] RPo({geneId}) + n NTP = RPo·RNA_n({geneId}) + n PPi",
                .forward = Controls.StaticControl(5),
                .reverse = Controls.StaticControl(0),
                .bounds = New Boundary With {
                    .forward = loader.dynamics.transcriptionCapacity,
                    .reverse = 0
                },
                .name = $"Elongation transcription of gene {cd.geneID} in cell {cellular_id}"
            }
        End Function

        ''' <summary>
        ''' phase 1
        ''' </summary>
        ''' <param name="cd"></param>
        ''' <param name="regulations"></param>
        ''' <param name="RNAp"></param>
        ''' <param name="DNAp"></param>
        ''' <param name="RPo"></param>
        ''' <returns></returns>
        Private Function RPoGenerator(cd As CentralDogma, regulations As Regulation(), RNAp As Variable, DNAp As Variable, RPo As Variable) As Channel
            Dim cellular_id As String = RPo.mass.cellular_compartment
            ' RNAP + DNA + DNA_P = RPo
            ' 可逆过程
            Dim activeReg As Variable() = regulations _
                .Where(Function(r) r.effects > 0) _
                .Select(Function(r)
                            Return MassTable.variable(r.regulator, cellular_id, r.effects)
                        End Function) _
                .ToArray
            Dim suppressReg As Variable() = regulations _
                .Where(Function(r) r.effects < 0) _
                .Select(Function(r)
                            Return MassTable.variable(r.regulator, cellular_id, r.effects)
                        End Function) _
                .ToArray

            RNAp = MassTable.variable(RNAp.mass.ID, cellular_id, RNAp.coefficient)
            DNAp = MassTable.variable(DNAp.mass.ID, cellular_id, DNAp.coefficient)
            RPo = MassTable.variable(RPo.mass.ID, cellular_id, RPo.coefficient)

            Dim geneDNA As Variable = MassTable.variable($"[{cd.geneID}]", cellular_id)

            Return New Channel({RNAp, geneDNA, DNAp}, {RPo}) With {
                .ID = $"[initiation] RNAP + DNA({cd.geneID}) + DNA_P = RPo({cd.geneID})",
                .forward = New AdditiveControls With {
                    .baseline = loader.dynamics.transcriptionBaseline * cd.expression_level,
                    .activation = activeReg,
                    .inhibition = suppressReg
                },
                .reverse = Controls.StaticControl(5),
                .bounds = New Boundary With {
                    .forward = loader.dynamics.transcriptionCapacity * cd.expression_level,
                    .reverse = 5
                },
                .name = $"Initial transcription of gene {cd.geneID} to RPo-complex in cell {cellular_id}"
            }
        End Function
    End Class
End Namespace