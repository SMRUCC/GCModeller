#Region "Microsoft.VisualBasic::ddc9e517f922be488ace7744c16dde5e, CLI_tools\c2\NetworkVisualization\SignalTransductionNetwork.vb"

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

    '     Class SignalTransductionNetwork
    ' 
    '         Properties: InteractionData
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ConvertId, CreateNetwork, Expend, PrepareData
    ' 
    '         Sub: Generate, Save, TrimNetwork
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat

Namespace NetworkVisualization

    Public Class SignalTransductionNetwork

        Dim NetworkData As LANS.SystemsBiology.Assembly.MiST2.MiST2
        Dim DOMINE As LANS.SystemsBiology.Assembly.DOMINE.Database
        Dim SignalDomains As LANS.SystemsBiology.Assembly.MiST2.Domain() = LANS.SystemsBiology.Assembly.MiST2.Domain.Load

        Sub New(MiST2 As LANS.SystemsBiology.Assembly.MiST2.MiST2, DOMINE As LANS.SystemsBiology.Assembly.DOMINE.Database)
            Me.NetworkData = MiST2
            Me.DOMINE = DOMINE
        End Sub

        Dim _InteractionData As List(Of LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin))

        Public ReadOnly Property InteractionData As LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin)()
            Get
                Return _InteractionData.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' 1. TCS: 假设所有的对象都会发生CrossTalk， 最后使用计算的得分矩阵进行修剪
        ''' 2. OCS, Chemotaxis, ECF, Other: 根据DOMINE数据库进行网络的创建 
        ''' </remarks>
        Public Sub Generate()
            _InteractionData = New List(Of LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin))

            For Each replicon In NetworkData.MajorModules
                Dim itrs = (CreateNetwork(replicon))
                Call _InteractionData.AddRange(itrs)
            Next
        End Sub

        Public Sub Save(FilePath As String)
            Dim LQuery = (From itr In Me._InteractionData Let str = Function() itr.ToString Select str()).ToArray
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendLine("ProteinNode1" & vbTab & "interaction" & vbTab & "ProteinNode2")

            For Each item In LQuery
                Call sBuilder.AppendLine(item)
            Next

            Call sBuilder.ToString.SaveTo(FilePath)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Replicon"></param>
        ''' <remarks>
        ''' TCS -> HK + RR 之间发生信号传导
        ''' TCS -> HHK, HRR 信号在自身发生传导，故而HHK和HRR都只与Input和Output发生作用
        ''' TCS -> Others 未知， 根据DOMINE数据库进行构建
        ''' </remarks>
        Private Function CreateNetwork(Replicon As LANS.SystemsBiology.Assembly.MiST2.Replicon) _
            As List(Of LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin))

            Dim TCS = Replicon.TwoComponent
            Dim Interaction As List(Of LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin)) =
                New List(Of LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin))
            Dim SignalProteins = Replicon.SignalProteinCollection

            'TCS Cross-Talk
            For Each HK In TCS.HisK
                Dim LQuery = (From RR In TCS.RR Select New LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin) With {.ObjectA = HK, .ObjectB = RR, .Interaction = "Cross-Talk"}).ToArray
                Call Interaction.AddRange(LQuery)
            Next

            Dim MiST2Inputs = (From dm In Me.SignalDomains Where String.Equals(dm.Type, "input", StringComparison.OrdinalIgnoreCase) Select dm.PfamId).ToArray
            Dim MiST2Outputs = (From dm In Me.SignalDomains Where String.Equals(dm.Type, "output", StringComparison.OrdinalIgnoreCase) Select dm.PfamId).ToArray

            Dim TryCreate As System.Action(Of LANS.SystemsBiology.Assembly.MiST2.Transducin()) =
                Sub(collection As LANS.SystemsBiology.Assembly.MiST2.Transducin())
                    For Each Protein In collection
                        If (Protein.Inputs.IsNullOrEmpty(strict:=True) AndAlso Protein.Outputs.IsNullOrEmpty(strict:=True)) Then
                            Call Interaction.Add(New LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin) With {.ObjectA = Protein})

                        ElseIf Protein.Inputs.IsNullOrEmpty(strict:=True) Then '根据DOMINE数据库和缺失的情况做构建
                            Dim InteractedInputDomainList As List(Of String) = New List(Of String)
                            For Each OutputDomain In Protein.Outputs
                                Call InteractedInputDomainList.AddRange(DOMINE.GetInteractionDomains(OutputDomain))
                            Next
                            InteractedInputDomainList = InteractedInputDomainList.Distinct.ToList '得到可能互作的结构域，下一步进行筛选
                            InteractedInputDomainList = (From domain In InteractedInputDomainList Where Array.IndexOf(MiST2Inputs, domain) > -1 Select domain).ToList
                            '筛选出具备列表中的结构域的蛋白质
                            Dim ProteinList = (From p In SignalProteins Where Not p.ContainsAnyDomain(InteractedInputDomainList).IsNullOrEmpty(strict:=True) Select p).ToArray
                            Dim LQuery = (From inputProtein In ProteinList Select New LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin) With {.ObjectA = inputProtein, .ObjectB = Protein, .Interaction = "Signal_input"}).ToArray
                            Call Interaction.AddRange(LQuery)

                        ElseIf Protein.Outputs.IsNullOrEmpty(strict:=True) Then
                            Dim InteractedOutputDomainList As List(Of String) = New List(Of String)
                            For Each InputputDomain In Protein.Inputs
                                Call InteractedOutputDomainList.AddRange(DOMINE.GetInteractionDomains(InputputDomain))
                            Next
                            InteractedOutputDomainList = InteractedOutputDomainList.Distinct.ToList '得到可能互作的结构域，下一步进行筛选
                            InteractedOutputDomainList = (From domain In InteractedOutputDomainList Where Array.IndexOf(MiST2Inputs, domain) > -1 Select domain).ToList
                            '筛选出具备列表中的结构域的蛋白质
                            Dim ProteinList = (From p In SignalProteins Where Not p.ContainsAnyDomain(InteractedOutputDomainList).IsNullOrEmpty(strict:=True) Select p).ToArray
                            Dim LQuery = (From outputProtein In ProteinList Select New LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin) With {.ObjectA = outputProtein, .ObjectB = Protein, .Interaction = "Signal_output"}).ToArray
                            Call Interaction.AddRange(LQuery)

                        Else
                            Call Interaction.Add(New LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin) With {.ObjectA = Protein})
                        End If
                    Next
                End Sub

            Call TryCreate(Replicon.Chemotaxis)
            Call TryCreate(Replicon.ECF)
            Call TryCreate(Replicon.OneComponent)
            Call TryCreate(Replicon.Other)
            Call TryCreate(Replicon.TwoComponent.Other)
            Call TryCreate(Replicon.TwoComponent.HHK)
            Call TryCreate(Replicon.TwoComponent.HRR)
            Call TryCreate(Replicon.TwoComponent.HisK)
            Call TryCreate(Replicon.TwoComponent.RR)

            Return Interaction
        End Function

        ''' <summary>
        ''' 从MiST2数据库之中所下载的数据中的结构域编号不是Pfam的标准编号，在这里进行转换
        ''' </summary>
        ''' <param name="MiST2"></param>
        ''' <param name="CddPfam"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function PrepareData(
                                          MiST2 As LANS.SystemsBiology.Assembly.MiST2.MiST2,
                                          CddPfam As LANS.SystemsBiology.Assembly.NCBI.CDD.DomainInfo.CDDLoader) _
            As LANS.SystemsBiology.Assembly.MiST2.MiST2

            Dim Pfam = CddPfam.GetPfam.SmpData.CreateDictionary(Function(obj As LANS.SystemsBiology.Assembly.NCBI.CDD.SmpFile) obj.CommonName) 'MiST2数据库之中的Pfam标识号为非标准的标识号
            Static Dim TrimProteinPorts As Action(Of String()) = Sub(collection As String())
                                                                     If Not collection.IsNullOrEmpty Then
                                                                         For j As Integer = 0 To collection.Count - 1
                                                                             Dim Domain = collection(j)
                                                                             If Pfam.ContainsKey(Domain) Then
                                                                                 collection(j) = Pfam(Domain).Identifier
                                                                             End If
                                                                         Next
                                                                     End If
                                                                 End Sub

            For i As Integer = 0 To MiST2.MajorModules.Count - 1
                Dim Replicon = MiST2.MajorModules(i)
                Dim Proteins = Replicon.SignalProteinCollection

                For idx As Integer = 0 To Proteins.Count - 1
                    Dim Protein = Proteins(idx)
                    Call Protein.GetDomainArchitecture()

                    For j As Integer = 0 To Protein.Domains.Count - 1
                        Dim Domain = Protein.Domains(j)
                        If Pfam.ContainsKey(Domain.Identifier) Then
                            Domain.Identifier = Pfam(Domain.Identifier).Identifier
                        End If
                    Next

                    Call TrimProteinPorts(Protein.Inputs)
                    Call TrimProteinPorts(Protein.Outputs)
                Next
            Next

            Return MiST2
        End Function

        ''' <summary>
        ''' 使用双组份系统之间的Cross-Talk数据来修剪网络
        ''' </summary>
        ''' <param name="CrossTalk"></param>
        ''' <remarks></remarks>
        Public Sub TrimNetwork(CrossTalk As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
            Dim HK As String() = (From row In CrossTalk.Skip(1) Select row.First).ToArray
            Dim RR As String() = CrossTalk.First.Skip(1).ToArray

            For Each row In CrossTalk.Skip(1)
                Dim HkId As String = row.First
                For colIdx As Integer = 1 To RR.Count - 1
                    Dim RRId As String = RR(colIdx - 1)
                    Dim score As Double = Val(row(colIdx))

                    If score = 0.0R Then
                        '这一个HK/RR对之间没有CrossTalk，进行移除
                        Dim LQuery = (From itr In Me._InteractionData Where Not itr.Broken AndAlso itr.Equals(HkId, RRId, False) Select itr).ToArray
                        For Each item In LQuery
                            Call Me._InteractionData.Remove(item)
                        Next
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 使用MetaCyc数据库中的标识符替换Interaction中的标识符
        ''' </summary>
        ''' <param name="Interactions"></param>
        ''' <param name="MetaCyc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ConvertId(Interactions As LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin)(),
                                         MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) _
            As LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin)()

            Dim Collection = (From iter In Interactions Where Not iter.ObjectA Is Nothing Select iter.ObjectA).ToList
            Call Collection.AddRange((From iter In Interactions Where Not iter.ObjectB Is Nothing Select iter.ObjectB).ToArray)

            Dim Genes = MetaCyc.GetGenes
            Dim DBLink As Dictionary(Of String, String) = New Dictionary(Of String, String)
            For Each Gene In Genes
                Call DBLink.Add(Gene.Accession1, Gene.Product.First)
            Next

            For i As Integer = 0 To Collection.Count - 1
                Dim item = Collection(i)
                item.Identifier = DBLink(item.Identifier)
            Next

            'For i As Integer = 0 To Interactions.Count - 1
            '    Dim Iter = Interactions(i)
            '    If Iter.Broken Then
            '        Continue For
            '    End If
            '    Iter.ObjectA.UniqueId = DBLink(Iter.ObjectA.UniqueId)
            '    Iter.ObjectB.UniqueId = DBLink(Iter.ObjectB.UniqueId)
            'Next

            Return Interactions
        End Function

        Public Shared Function Expend(Model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel,
                                      Data As LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin)(),
                                      MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) _
            As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel

            Dim LQuery = From iter As LANS.SystemsBiology.InteractionModel.Interaction(Of LANS.SystemsBiology.Assembly.MiST2.Transducin)
                         In Data
                         Where Not iter.Broken
                         Select New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With {
                             .Identifier = iter.ToString,
                             .Reversible = False,
                             .Reactants = New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference() {
                                 New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = iter.ObjectA.Identifier & "[PI]", .StoiChiometry = 1},
                                 New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = iter.ObjectB.Identifier, .StoiChiometry = 1}}.ToList,
                             .Products = New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference() {
                                 New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = iter.ObjectA.Identifier, .StoiChiometry = 1},
                                 New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = iter.ObjectB.Identifier & "[PI]", .StoiChiometry = 1}}.ToList} '
            Dim result = LQuery.ToArray 'TCS Cross-Talk

            Model.ProteinAssemblies.AddRange(result)
            Model.ProteinAssemblies = Model.ProteinAssemblies.ToList

            Dim ProteinList As List(Of String) = New List(Of String)
            For Each iter In Data
                If Not iter.ObjectA Is Nothing Then Call ProteinList.Add(iter.ObjectA.Identifier)
                If Not iter.ObjectB Is Nothing Then Call ProteinList.Add(iter.ObjectB.Identifier)
            Next

            Dim Metabolites = (From id As String
                               In ProteinList.Distinct
                               Let Name As String = id & "[PI]"
                               Select New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite With {
                                   .BoundaryCondition = False,
                                   .CommonName = Name,
                                   .InitialAmount = 10,
                                   .Identifier = Name}).ToArray
            Call Model.Metabolism.Metabolites.AddRange(Metabolites)
            Model.Metabolism.Metabolites = Model.Metabolism.Metabolites

            Return Model
        End Function
    End Class
End Namespace
