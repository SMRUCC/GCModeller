#Region "Microsoft.VisualBasic::d5c170102553c7a2bcf8d759a32a12a7, GCModeller\engine\BootstrapLoader\CentralDogmaFluxLoader.vb"

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

    '   Total Lines: 305
    '    Code Lines: 219
    ' Comment Lines: 43
    '   Blank Lines: 43
    '     File Size: 14.31 KB


    '     Class CentralDogmaFluxLoader
    ' 
    '         Properties: componentRNA, mRNA, polypeptides
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateFlux, ribosomeAssembly, transcriptionTemplate, translationTemplate, translationUncharged
    '                   tRNAProcess
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

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

        Public Sub New(loader As Loader)
            Call MyBase.New(loader)

            Call loader.fluxIndex.Add(NameOf(tRNAProcess), New List(Of String))
            Call loader.fluxIndex.Add(NameOf(ribosomeAssembly), New List(Of String))
            Call loader.fluxIndex.Add("translation", New List(Of String))
            Call loader.fluxIndex.Add("transcription", New List(Of String))
        End Sub

        Dim charged_tRNA As New Dictionary(Of String, String)
        Dim uncharged_tRNA As New Dictionary(Of String, String)

        ''' <summary>
        ''' tRNA charge process
        ''' </summary>
        ''' <returns></returns>
        Private Iterator Function tRNAProcess(cd As CentralDogma) As IEnumerable(Of Channel)
            Dim chargeName As String = "*" & cd.RNAName
            Dim AA As String = SequenceModel.Polypeptides.Abbreviate(cd.RNA.Description)

            ' tRNA基因会存在多个拷贝
            ' 但是实际的反应只需要一个就好了，在这里跳过已经重复出现的tRNA拷贝
            If charged_tRNA.ContainsKey(AA) Then
                Return
            Else
                charged_tRNA.Add(AA, chargeName)
                uncharged_tRNA.Add(AA, cd.RNAName)
                MassTable.AddNew(chargeName, MassRoles.tRNA)
            End If

            Dim left As Variable() = {
                MassTable.variable(cd.RNAName),
                MassTable.variable(loader.define.ATP),
                MassTable.variable(loader.define.AminoAcid(AA))
            }
            Dim right As Variable() = {
                MassTable.variable(chargeName),
                MassTable.variable(loader.define.ADP)
            }
            Dim flux As New Channel(left, right) With {
               .ID = $"chargeOf_{cd.RNAName}",
               .bounds = New Boundary() With {
                   .forward = loader.dynamics.tRNAChargeCapacity
               },
               .reverse = Controls.StaticControl(0),
               .forward = Controls.StaticControl(loader.dynamics.tRNAChargeBaseline)
            }

            loader.fluxIndex(NameOf(Me.tRNAProcess)).Add(flux.ID)

            Yield flux
        End Function

        Private Function ribosomeAssembly(rRNA As String()) As Channel
            Dim left As Variable() = rRNA.Select(Function(ref) MassTable.variable(ref)).ToArray
            Dim flux As Channel

            MassTable.AddNew(NameOf(ribosomeAssembly), MassRoles.protein)
            flux = New Channel(left, {MassTable.variable(NameOf(ribosomeAssembly))}) With {
                .ID = NameOf(ribosomeAssembly),
                .bounds = New Boundary With {
                    .forward = loader.dynamics.ribosomeAssemblyCapacity,
                    .reverse = loader.dynamics.ribosomeDisassemblyCapacity
                },
                .forward = Controls.StaticControl(loader.dynamics.ribosomeAssemblyBaseline),
                .reverse = Controls.StaticControl(loader.dynamics.ribosomeDisassemblyBaseline)
            }

            loader.fluxIndex(NameOf(Me.ribosomeAssembly)).Add(flux.ID)

            Return flux
        End Function

        Public Overrides Iterator Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)
            Dim templateDNA As Variable()
            Dim productsRNA As Variable()
            Dim templateRNA As Variable()
            Dim productsPro As Variable()
            Dim rnaMatrix = cell.Genotype.RNAMatrix.ToDictionary(Function(r) r.geneID)
            Dim proteinMatrix = cell.Genotype.ProteinMatrix.ToDictionary(Function(r) r.proteinID)
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

            Dim mRNA As New List(Of String)
            Dim componentRNA As New List(Of String)
            Dim polypeptides As New List(Of String)
            Dim transcription As Channel
            Dim translation As Channel
            Dim trKey, tlKey As String
            Dim regulations As Regulation()
            Dim proteinList As New Dictionary(Of String, String)
            Dim proteinComplex = loader.massLoader.proteinComplex
            Dim tRNA As New Dictionary(Of String, List(Of String))
            Dim rRNA As New Dictionary(Of String, List(Of String))

            ' 在这里分开两个循环来完成构建
            ' 第一步需要一次性的将所有的元素对象都加入到mass table之中
            ' 否则在构建的过程中会出现很多的key not found 的错误
            For Each cd As CentralDogma In cell.Genotype.centralDogmas
                ' if the gene template mass value is set to ZERO
                ' that means no transcription activity that it will be
                ' A deletion mutation was created
                Call MassTable.AddNew(cd.geneID, MassRoles.gene)

                If Not cd.polypeptide Is Nothing Then
                    Call MassTable.AddNew(cd.polypeptide, MassRoles.popypeptide)
                    Call mRNA.Add(cd.geneID)
                    Call proteinList.Add(cd.geneID, proteinComplex(cd.polypeptide))
                    Call MassTable.AddNew(cd.RNAName, MassRoles.mRNA)
                Else
                    Call componentRNA.Add(cd.geneID)

                    If Not cd.RNA.Description.StringEmpty Then
                        Select Case cd.RNA.Value
                            Case RNATypes.ribosomalRNA
                                If Not rRNA.ContainsKey(cd.RNA.Description) Then
                                    rRNA.Add(cd.RNA.Description, New List(Of String))
                                End If

                                rRNA(cd.RNA.Description).Add(cd.RNAName)
                                MassTable.AddNew(cd.RNAName, MassRoles.rRNA)
                            Case RNATypes.tRNA
                                If Not tRNA.ContainsKey(cd.RNA.Description) Then
                                    tRNA.Add(cd.RNA.Description, New List(Of String))
                                End If

                                tRNA(cd.RNA.Description).Add(cd.RNAName)
                                MassTable.AddNew(cd.RNAName, MassRoles.tRNA)

                                For Each proc As Channel In tRNAProcess(cd)
                                    Yield proc
                                Next
                        End Select
                    End If
                End If
            Next

            Yield ribosomeAssembly(rRNA.Values.IteratesALL.Distinct.ToArray)

            ' 在这里创建针对每一个基因的从转录到翻译的整个过程
            ' 之中的不同阶段的生物学过程的模型对象
            For Each cd As CentralDogma In cell.Genotype.centralDogmas
                ' cd.RNA.Name属性值是基因的id，会产生对象引用错误 
                templateDNA = transcriptionTemplate(cd.geneID, rnaMatrix)
                productsRNA = {
                    MassTable.variable(cd.RNAName),
                    MassTable.variable(loader.define.ADP)
                }

                ' 转录和翻译的反应过程都是不可逆的

                ' 翻译模板过程只针对CDS基因
                If Not cd.polypeptide Is Nothing Then
                    templateRNA = translationTemplate(cd.geneID, cd.RNAName, proteinMatrix)
                    productsPro = translationUncharged(cd.geneID, cd.polypeptide, proteinMatrix)
                    polypeptides += cd.polypeptide

                    ' 针对mRNA对象，创建翻译过程
                    translation = New Channel(templateRNA, productsPro) With {
                        .ID = cd.DoCall(AddressOf Loader.GetTranslationId),
                        .forward = New AdditiveControls With {
                            .baseline = 0,
                            .activation = {MassTable.variable(NameOf(ribosomeAssembly))}
                        },
                        .reverse = Controls.StaticControl(0),
                        .bounds = New Boundary With {
                            .forward = loader.dynamics.translationCapacity,
                            .reverse = 0
                        }
                    }

                    loader.fluxIndex("translation").Add(translation.ID)

                    Yield translation
                End If

                trKey = cd.ToString
                regulations = TFregulations.TryGetValue(trKey).SafeQuery.ToArray

                Dim activeReg As Variable() = regulations _
                    .Where(Function(r) r.effects > 0) _
                    .Select(Function(r)
                                Return MassTable.variable(proteinList(r.regulator), r.effects)
                            End Function) _
                    .ToArray
                Dim suppressReg As Variable() = regulations _
                    .Where(Function(r) r.effects < 0) _
                    .Select(Function(r)
                                Return MassTable.variable(proteinList(r.regulator), r.effects)
                            End Function) _
                    .ToArray

                ' 针对所有基因对象，创建转录过程
                ' 转录是以DNA为模板产生RNA分子
                transcription = New Channel(templateDNA, productsRNA) With {
                    .ID = cd.DoCall(AddressOf Loader.GetTranscriptionId),
                    .forward = New AdditiveControls With {
                        .baseline = loader.dynamics.transcriptionBaseline,
                        .activation = activeReg,
                        .inhibition = suppressReg
                    },
                    .reverse = Controls.StaticControl(0),
                    .bounds = New Boundary With {
                        .forward = loader.dynamics.transcriptionCapacity,
                        .reverse = 0
                    }
                }

                loader.fluxIndex("transcription").Add(transcription.ID)

                Yield transcription
            Next

            _mRNA = mRNA
            _componentRNA = componentRNA
            _polypeptides = polypeptides
        End Function

        ''' <summary>
        ''' DNA模板加上碱基消耗
        ''' </summary>
        ''' <param name="geneID$"></param>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        Private Function transcriptionTemplate(geneID$, matrix As Dictionary(Of String, RNAComposition)) As Variable()
            Return matrix(geneID) _
                .Where(Function(i) i.Value > 0) _
                .Select(Function(base)
                            Dim baseName = loader.define.NucleicAcid(base.Name)
                            Return MassTable.variable(baseName, base.Value)
                        End Function) _
                .AsList + MassTable.template(geneID) + MassTable.variable(loader.define.ATP)
        End Function

        '       ATP + AA   + ADP
        ' cd -> tRNA -> charged-tRNA

        ''' <summary>
        ''' mRNA模板加上氨基酸消耗，请注意，在这里并不是直接消耗的氨基酸，而是消耗的已经荷载的tRNA分子
        ''' </summary>
        ''' <param name="mRNA">The name of the mRNA molecule</param>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        Private Function translationTemplate(geneID$, mRNA$, matrix As Dictionary(Of String, ProteinComposition)) As Variable()
            Dim AAVector = matrix(geneID).Where(Function(i) i.Value > 0).ToArray
            Dim AAtRNA = AAVector _
                .Select(Function(aa)
                            Return MassTable.variable(charged_tRNA(aa.Name), aa.Value)
                        End Function) _
                .AsList

            Return AAtRNA + MassTable.template(mRNA) + MassTable.variable(loader.define.ATP)
        End Function

        Private Function translationUncharged(geneID$, peptide$, matrix As Dictionary(Of String, ProteinComposition)) As Variable()
            Dim AAVector = matrix(geneID).Where(Function(i) i.Value > 0).ToArray
            Dim AAtRNA = AAVector _
                .Select(Function(aa)
                            Return MassTable.variable(uncharged_tRNA(aa.Name), aa.Value)
                        End Function) _
                .AsList

            Return AAtRNA + MassTable.template(peptide) + MassTable.variable(loader.define.ADP)
        End Function
    End Class
End Namespace
