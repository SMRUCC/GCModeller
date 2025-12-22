#Region "Microsoft.VisualBasic::c9a8998197392b31a396483992349625, engine\BootstrapLoader\CentralDogmaFluxLoader.vb"

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

'   Total Lines: 423
'    Code Lines: 314 (74.23%)
' Comment Lines: 45 (10.64%)
'    - Xml Docs: 55.56%
' 
'   Blank Lines: 64 (15.13%)
'     File Size: 19.14 KB


'     Class CentralDogmaFluxLoader
' 
'         Properties: componentRNA, mRNA, polypeptides
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateFlux, GetMassSet, MissingAAComposition, ProteinMatrixIndex, ribosomeAssembly
'                   RnaMatrixIndexing, transcriptionTemplate, translationTemplate, translationUncharged, tRNAProcess
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Trinity
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace ModelLoader

    ''' <summary>
    ''' 先构建一般性的中心法则过程
    ''' 在这里面包含所有类型的RNA转录
    ''' 以及蛋白序列的翻译
    ''' </summary>
    Public Class CentralDogmaFluxLoader : Inherits FluxLoader

#Region "降解的对象列表"

        Public ReadOnly Property mRNA As String()
        ''' <summary>
        ''' tRNA+rRNA+mics RNA
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property componentRNA As String()
        Public ReadOnly Property polypeptides As String()
#End Region

        ReadOnly pull As New List(Of String)

        Public Sub New(loader As Loader)
            Call MyBase.New(loader)

            Call loader.fluxIndex.Add(NameOf(tRNAProcess), New List(Of String))
            Call loader.fluxIndex.Add(NameOf(ribosomeAssembly), New List(Of String))
            Call loader.fluxIndex.Add("translation", New List(Of String))
            Call loader.fluxIndex.Add("transcription", New List(Of String))
        End Sub

        Friend ReadOnly charged_tRNA As New Dictionary(Of String, String)
        Friend ReadOnly uncharged_tRNA As New Dictionary(Of String, String)
        Friend ReadOnly charged_names As New Dictionary(Of String, String)

        Shared ReadOnly aaIndex As Dictionary(Of String, String) = SequenceModel.Polypeptides.Abbreviate

        ''' <summary>
        ''' tRNA charge process
        ''' </summary>
        ''' <returns></returns>
        Private Iterator Function tRNAProcess(cd As CentralDogma) As IEnumerable(Of Channel)
            Dim chargeName As String = "*" & cd.RNAName
            Dim AAKey As String = cd.RNA.Description.Replace("tRNA", "").Trim("-"c)
            Dim AA As String
            Dim cellular_id As String = cell.CellularEnvironmentName

            If aaIndex.ContainsKey(AAKey) Then
                AA = aaIndex(AAKey)
                ' 20250731 patched for the biocyc met-tRNA object
            ElseIf AAKey = "initiation-trnamet" Then
                AAKey = "Met"
                AA = aaIndex(AAKey)
            ElseIf AAKey.TextEquals("other") Then
                ' unknown amino acid type
                ' not treated as tRNA
                Return
            Else
                Throw New MissingPrimaryKeyException($"missing the amino acid mapping of '{AAKey}'!")
            End If

            ' tRNA基因会存在多个拷贝
            ' 但是实际的反应只需要一个就好了，在这里跳过已经重复出现的tRNA拷贝
            If charged_tRNA.ContainsKey(AA) Then
                Return
            Else
                charged_tRNA.Add(AA, chargeName)
                uncharged_tRNA.Add(AA, cd.RNAName)
                MassTable.addNew(chargeName, MassRoles.tRNA, cellular_id)
            End If

            Dim left As Variable() = {
                MassTable.variable(cd.RNAName, cellular_id),
                MassTable.variable(loader.define.ATP, cellular_id),
                MassTable.variable(loader.define.AminoAcid(AA), cellular_id)
            }
            Dim right As Variable() = {
                MassTable.variable(chargeName, cellular_id),
                MassTable.variable(loader.define.ADP, cellular_id)
            }
            Dim flux As New Channel(left, right) With {
               .ID = $"{cd.RNAName}@{cellular_id}[tRNA-Charge]",
               .bounds = New Boundary() With {
                   .forward = loader.dynamics.tRNAChargeCapacity
               },
               .reverse = Controls.StaticControl(0),
               .forward = Controls.StaticControl(loader.dynamics.tRNAChargeBaseline),
               .name = $"tRNA charge of {cd.RNAName} in cell {cellular_id}"
            }

            If flux.isBroken Then
                Throw New InvalidDataException(String.Format(flux.Message, flux.ID))
            End If

            loader.fluxIndex(NameOf(Me.tRNAProcess)).Add(flux.ID)

            Yield flux
        End Function

        ''' <summary>
        ''' is a reversiable process
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' https://stack.xieguigang.me/2025/modelling-virtualcell-translation-event/
        ''' </remarks>
        Private Iterator Function ribosomeAssembly(rRNA As Dictionary(Of String, List(Of String))) As IEnumerable(Of Channel)
            Dim cellular_id As String = cell.CellularEnvironmentName
            Dim left As New List(Of Variable)
            Dim flux As Channel
            Dim transcript As Variable
            Dim generic As Variable

            ' 5s + 23s + 34 * L = 50s
            ' 16s + 21 * S = 30s
            ' 30s + mRNA + 50s + GTP = 70s_mRNA + GDP + Pi
            ' 70s_mRNA + N * charged-aa-tRNA = 70s_mRNA + polypeptide + N * aa-tRNA + N * Pi
            ' 70s_mRNA = 30s + mRNA + 50s + Pi

            For Each type As KeyValuePair(Of String, List(Of String)) In rRNA
                Dim rRNA_key As String = $"{type.Key}_rRNA"

                Call MassTable.addNew(rRNA_key, MassRoles.rRNA, cellular_id)

                generic = MassTable.variable(rRNA_key, cellular_id)
                left.Add(generic)

                For Each id As String In type.Value
                    Call MassTable.addNew(id, MassRoles.RNA, cellular_id)

                    transcript = MassTable.variable(id, cellular_id)
                    flux = New Channel(transcript, generic) With {
                        .ID = $"{rRNA_key}<->{id}@{cellular_id}",
                        .bounds = New Boundary(100, 100),
                        .forward = Controls.StaticControl(100),
                        .reverse = Controls.StaticControl(100)
                    }

                    If flux.isBroken Then
                        Throw New InvalidDataException(String.Format(flux.Message, flux.ID))
                    Else
                        Yield flux
                    End If
                Next
            Next

            MassTable.addNew(NameOf(ribosomeAssembly), MassRoles.protein, cellular_id)
            flux = New Channel(left, {MassTable.variable(NameOf(ribosomeAssembly), cellular_id)}) With {
                .ID = NameOf(ribosomeAssembly) & "@" & cellular_id,
                .bounds = New Boundary With {
                    .forward = loader.dynamics.ribosomeAssemblyCapacity,
                    .reverse = loader.dynamics.ribosomeDisassemblyCapacity
                },
                .forward = Controls.StaticControl(loader.dynamics.ribosomeAssemblyBaseline),
                .reverse = Controls.StaticControl(loader.dynamics.ribosomeDisassemblyBaseline),
                .name = $"Ribosome assembly in cell {cellular_id}"
            }

            loader.fluxIndex(NameOf(Me.ribosomeAssembly)).Add(flux.ID)

            If flux.isBroken Then
                Throw New InvalidDataException(String.Format(flux.Message, flux.ID))
            End If

            Yield flux
        End Function

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

        Protected Overrides Iterator Function CreateFlux() As IEnumerable(Of Channel)
            Dim cellular_id As String = cell.CellularEnvironmentName
            Dim mRNA As New List(Of String)
            Dim componentRNA As New List(Of String)
            Dim proteinList As New Dictionary(Of String, String)
            Dim proteinComplex As Dictionary(Of String, String) = loader.massLoader.proteinComplex
            Dim tRNA As New Dictionary(Of String, List(Of String))
            Dim rRNA As New Dictionary(Of String, List(Of String))

            Call VBDebugger.WaitOutput()
            Call VBDebugger.EchoLine("initialize of the mass environment for central dogma")

            Dim RNAList As NamedValue(Of RNATypes)() = cell.Genotype.centralDogmas _
                .Select(Function(c) c.RNA) _
                .ToArray

            For Each cd As CentralDogma In TqdmWrapper.Wrap(cell.Genotype.centralDogmas, wrap_console:=App.EnableTqdm)
                If cd.isChargedtRNA Then
                    charged_names($"*tRNA{cd.RNA.Description.Replace("charged", "")}") = cd.RNAName
                End If
            Next

            Dim bar As Tqdm.ProgressBar = Nothing
            Dim duplicatedGenes As New List(Of String)

            ' 在这里分开两个循环来完成构建
            ' 第一步需要一次性的将所有的元素对象都加入到mass table之中
            ' 否则在构建的过程中会出现很多的key not found 的错误
            For Each cd As CentralDogma In TqdmWrapper.Wrap(cell.Genotype.centralDogmas, bar:=bar, wrap_console:=App.EnableTqdm)
                ' if the gene template mass value is set to ZERO
                ' that means no transcription activity that it will be
                ' A deletion mutation was created
                Call MassTable.addNew($"[{cd.geneID}]", MassRoles.gene, cellular_id)

                If Not cd.polypeptide Is Nothing Then
                    Call MassTable.addNew("*" & cd.polypeptide, MassRoles.polypeptide, cellular_id)
                    Call mRNA.Add(cd.geneID)

                    If proteinList.ContainsKey(cd.geneID) Then
                        Call duplicatedGenes.Add(cd.geneID)
                        ' the translated polypeptide is a protein
                    ElseIf proteinComplex.ContainsKey(cd.polypeptide) Then
                        Call proteinList.Add(cd.geneID, proteinComplex(cd.polypeptide))
                    Else
                        ' the translated polypeptide is one of the components of a protein complex
                        If loader.massLoader.peptideToProteinComplex.ContainsKey(cd.polypeptide) Then
                            ' has already been used as one of the components of a protein complex
                            ' skip this peptide
                            ' do nothing
                        Else
                            Throw New MissingPrimaryKeyException($"missing protein link for polypeptide: {cd.polypeptide}, source gene id: {cd.geneID}")
                        End If
                    End If

                    Call MassTable.addNew(cd.RNAName, MassRoles.mRNA, cellular_id)
                Else
                    Call componentRNA.Add(cd.geneID)

                    If Not cd.RNA.Description.StringEmpty Then
                        Select Case cd.RNA.Value
                            Case RNATypes.ribosomalRNA
                                If Not rRNA.ContainsKey(cd.RNA.Description) Then
                                    rRNA.Add(cd.RNA.Description, New List(Of String))
                                End If

                                rRNA(cd.RNA.Description).Add(cd.RNA.Name)
                                MassTable.addNew(cd.RNA.Name, MassRoles.rRNA, cellular_id)
                                MassTable.addNew(cd.RNAName, MassRoles.rRNA, cellular_id)
                            Case RNATypes.tRNA
                                If Not cd.isChargedtRNA Then
                                    If Not tRNA.ContainsKey(cd.RNA.Description) Then
                                        tRNA.Add(cd.RNA.Description, New List(Of String))
                                    End If

                                    tRNA(cd.RNA.Description).Add(cd.RNAName)
                                    MassTable.addNew(cd.RNAName, MassRoles.tRNA, cellular_id)

                                    For Each proc As Channel In tRNAProcess(cd)
                                        Yield proc
                                    Next
                                End If
                            Case Else
                                ' add RNA molecule to mass table
                                Call MassTable.addNew(cd.RNAName, MassRoles.RNA, cellular_id)
                        End Select
                    Else
                        ' add RNA molecule to mass table
                        Call MassTable.addNew(cd.RNAName, MassRoles.RNA, cellular_id)
                    End If
                End If
            Next

            If duplicatedGenes.Any Then
                Dim uniq = duplicatedGenes.Distinct.ToArray
                Dim warn = $"found {uniq.Length} duplicated gene models: {uniq.JoinBy(", ")}!"

                Call warn.warning
                Call warn.debug
            End If

            If rRNA.IsNullOrEmpty Then
                ' do nothing 
            ElseIf rRNA.Count = 3 Then
                For Each flux As Channel In ribosomeAssembly(rRNA)
                    Yield flux
                Next
            Else
                Throw New InvalidCastException($"missing some of the rRNA components! current assembly components: {rRNA.Keys.ToArray.GetJson}")
            End If

            Call VBDebugger.EchoLine("build biological process for central dogmas...")

            For Each [event] As Channel In transcriptionEvents()
                Yield [event]
            Next

            _mRNA = mRNA
            _componentRNA = componentRNA
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' https://stack.xieguigang.me/2025/modelling-virtualcell-transcription-event/
        ''' </remarks>
        Private Iterator Function transcriptionEvents() As IEnumerable(Of Channel)
            Dim cellular_id As String = cell.CellularEnvironmentName
            Dim templateRNA As Variable()
            Dim productsPro As Variable()
            Dim translation As Channel
            Dim rnaMatrix As Dictionary(Of String, RNAComposition) = RnaMatrixIndexing(cell.Genotype.RNAMatrix)
            Dim proteinMatrix = ProteinMatrixIndex(cell.Genotype.ProteinMatrix)
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
            Dim polypeptides As New List(Of String)

            Call MassTable.AddOrUpdate(cellular_growth, cellular_growth.ID, cellular_id)
            Call MassTable.AddOrUpdate(New StatusMapFactor(id:="RNAp", mass:=$"cellular_growth@{cellular_id}", cellular_id, MassTable), $"RNAp@{cellular_id}", cellular_id)
            Call MassTable.AddOrUpdate(New StatusMapFactor(id:="DNAp", mass:=$"cellular_growth@{cellular_id}", cellular_id, MassTable), $"DNAp@{cellular_id}", cellular_id)

            Dim RNAp As Variable = MassTable.variable($"RNAp@{cellular_id}", cellular_id, 1 / cell.Genotype.ProteinMatrix.Length)
            Dim DNAp As Variable = MassTable.variable($"DNAp@{cellular_id}", cellular_id, 1 / cell.Genotype.ProteinMatrix.Length)
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
                    templateRNA = translationTemplate(cd, proteinMatrix)
                    productsPro = translationUncharged(cd, cd.polypeptide, proteinMatrix)
                    polypeptides += cd.polypeptide

                    ' 针对mRNA对象，创建翻译过程
                    translation = New Channel(templateRNA, productsPro) With {
                        .ID = DataHelper.GetTranslationId(cd, cellular_id),
                        .forward = New AdditiveControls With {
                            .baseline = 0,
                            .activation = {MassTable.variable(NameOf(ribosomeAssembly), cellular_id)}
                        },
                        .reverse = Controls.StaticControl(0),
                        .bounds = New Boundary With {
                            .forward = loader.dynamics.translationCapacity,
                            .reverse = 0  ' RNA can not be revsered to DNA
                        },
                        .name = $"Translation from mRNA {cd.RNAName} to polypeptide {cd.polypeptide} in cell {cellular_id}"
                    }

                    If translation.isBroken Then
                        Throw New InvalidDataException(String.Format(translation.Message, translation.ID))
                    End If

                    loader.fluxIndex("translation").Add(translation.ID)

                    Yield translation
                End If

                loader.fluxIndex("transcription").Add(phase1.ID)
                loader.fluxIndex("transcription").Add(phase2.ID)
                loader.fluxIndex("transcription").Add(phase3.ID)
            Next

            _polypeptides = polypeptides
        End Function

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

        '       ATP + AA   + ADP
        ' cd -> tRNA -> charged-tRNA

        ''' <summary>
        ''' mRNA模板加上氨基酸消耗，请注意，在这里并不是直接消耗的氨基酸，而是消耗的已经荷载的tRNA分子
        ''' </summary>
        ''' <param name="gene">The name of the mRNA molecule</param>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        Private Function translationTemplate(gene As CentralDogma, matrix As Dictionary(Of String, ProteinComposition)) As Variable()
            Dim cellular_id As String = cell.CellularEnvironmentName
            Dim composit = If(matrix.ContainsKey(gene.geneID), matrix(gene.geneID), matrix.TryGetValue(gene.translation))

            If composit Is Nothing Then
                composit = MissingAAComposition(gene)
            End If

            Dim AAVector = composit.Where(Function(i) i.Value > 0).ToArray
            Dim AAtRNA = AAVector _
                .Select(Function(aa)
                            Return MassTable.variable(charged_tRNA(aa.Name), cellular_id, aa.Value)
                        End Function) _
                .AsList
            Dim mRNA As String = gene.RNAName

            Return AAtRNA + MassTable.template(mRNA, cellular_id) + MassTable.variable(loader.define.ATP, cellular_id)
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

        Private Function translationUncharged(gene As CentralDogma, peptide$, matrix As Dictionary(Of String, ProteinComposition)) As Variable()
            Dim cellular_id As String = cell.CellularEnvironmentName
            Dim composit = If(matrix.ContainsKey(gene.geneID), matrix(gene.geneID), matrix.TryGetValue(gene.translation))

            If composit Is Nothing Then
                composit = MissingAAComposition(gene)
            End If

            Dim AAVector = composit.Where(Function(i) i.Value > 0).ToArray
            Dim AAtRNA = AAVector _
                .Select(Function(aa)
                            Return MassTable.variable(uncharged_tRNA(aa.Name), cellular_id, aa.Value)
                        End Function) _
                .AsList
            Dim mRNA As String = gene.RNAName

            ' 20250831
            ' template of mRNA is not working in ODEs
            ' restore the mRNA in product list at here
            Return AAtRNA + MassTable.variable("*" & peptide, cellular_id) + MassTable.variable(mRNA, cellular_id) + MassTable.variable(loader.define.ADP, cellular_id)
        End Function

        Protected Overrides Function GetMassSet() As IEnumerable(Of String)
            Return pull
        End Function
    End Class
End Namespace
