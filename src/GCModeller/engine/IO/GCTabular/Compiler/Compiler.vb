#Region "Microsoft.VisualBasic::e92cd58aaea8d21d73fad462790c5704, engine\IO\GCTabular\Compiler\Compiler.vb"

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

    '     Class Compiler
    ' 
    '         Function: _____createTranscript, _createProteinAssembly, AnalysisTransmenbraneFlux, Compile, CreateNewTransmembraneModel
    '                   GetGeneId, GetProteinSequenceData, GetTranscriptSequenceData, Link, PreCompile
    ' 
    '         Sub: __createProteinObjects, __Initialize_MetaCyc, _createEnzymeObjects, _createRibosomeAssembly, _createTranscripts
    '              CreatePathwayObjects, Dispose, GenerateCompositionVectors, LinkEffectors
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Expasy.AnnotationsTool
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.SabiorkKineticLaws.TabularDump
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.Model.Network.STRING.Models
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Level2
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace Compiler

    Public Class Compiler : Inherits Compiler(Of CellSystemXmlModel)
        Implements IDisposable

#Region "Module variable fields"

        Protected _MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
        Protected Friend _ModelIO As FileStream.IO.XmlresxLoader
        Protected _MetabolismNetwork As Level2.XmlFile
        Dim _RegpreciseRegulators As TranscriptionFactors
        Dim _TranscriptRegulations As TranscriptRegulation()
        Protected _argvs_Compile As CommandLine.CommandLine
        Protected _Door As SMRUCC.genomics.Assembly.DOOR.DOOR
        Protected _CrossTalks As File
        Dim _ECProfiles As SMRUCC.genomics.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT()

#End Region

#Region "Public Methods"

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="argvs">[<seealso cref="Microsoft.VisualBasic.CommandLine.CommandLine">MetaCyc database data directory|Export directory|RegpreciseRegulators</seealso>] -
        ''' -precompile -metacyc "<seealso cref="Compiler._MetaCyc"></seealso>" -regprecise_regulator "<seealso cref="Compiler._RegpreciseRegulators"></seealso>" -export "ModelParentDir"
        ''' 假若-transcript_regulation参数为空的话，则使用MetaCyc数据库中的Regulation关系数据表</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("-precompile", Info:="Data preparations for the compiler.",
            Usage:="-precompile -metacyc <metacyc_database_data_dir> -regprecise_regulator <regprecise_regulator_xml_filepath> -export <exported_directory> [-transcript_regulation <regulation_file> -logging <log_file_output>]",
            Example:="")>
        Public Overloads Overrides Function PreCompile(argvs As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
            Dim LogFile As String = argvs("-logging")

            If Not String.IsNullOrEmpty(LogFile) Then
                Me._Logging = New LogFile(LogFile)
            Else
                Me._Logging = New LogFile(My.Computer.FileSystem.SpecialDirectories.Temp & "/gcmodeller_compiler.log")
            End If

            Call Me._Logging.WriteLine(argvs.GetCommandsOverview, "CSV_COMPILER::PRE_COMPILE", Type:=MSG_TYPES.INF)
            Call __Initialize_MetaCyc(DatabaseLoadder.CreateInstance(argvs("-metacyc")))

            MyBase.CompiledModel = New CellSystemXmlModel
            Me._ModelIO = New FileStream.IO.XmlresxLoader(MyBase.CompiledModel)

            Dim MetaCycCompounds = _MetaCyc.GetCompounds
            Me._MetabolismNetwork = XmlFile.Load(Me._MetaCyc.Database.DataDIR & "/metabolic-reactions.sbml")
            Me._ModelIO.MetabolitesModel = (From Metabolite
                                            In Me._MetabolismNetwork.Model.listOfSpecies
                                            Let ModelData = FileStream.Metabolite.CreateObject(Metabolite, MetaCycCompounds)
                                            Select ModelData
                                            Order By ModelData.Identifier Ascending).ToDictionary
            Me._ModelIO.MisT2 = argvs("-mist2").LoadXml(Of SMRUCC.genomics.Assembly.MiST2.MiST2)()
            Me._RegpreciseRegulators = Regprecise.TranscriptionFactors.Load(argvs("-regprecise_regulator"))
            Me._ModelIO.SetExportDirectory(argvs("-export"))
            Me._ModelIO.STrPModel = argvs("-mist2_strp").LoadXml(Of Network)()
#If Not DEBUG Then
        Try
#End If
            Me._TranscriptRegulations = argvs("-transcript_regulation").LoadCsv(Of TranscriptRegulation).ToArray
            Me._ModelIO.StringInteractions = argvs("-string-db").LoadXml(Of SimpleCsv.Network)()

            Using MappingCreator = New Mapping(_MetaCyc, Me._ModelIO.MetabolitesModel.Values.ToArray)
                Call _Logging.WriteLine("Start to create the effector mapping between the regprecise database and metacyc database...")
                Me._ModelIO.EffectorMapping = MappingCreator.EffectorMapping(Me._RegpreciseRegulators)
                Call _Logging.WriteLine(String.Format("Create {0} effectors in total!", Me._ModelIO.EffectorMapping.Count))
                Me._ModelIO.EnzymeMapping = New List(Of Mapping.EnzymeGeneMap)(MappingCreator.CreateEnzrxnGeneMap)
                Call _Logging.WriteLine(Me._ModelIO.EnzymeMapping.Count & " enzymatic reaction mapping was created!")
            End Using

#If Not DEBUG Then
        Catch ex As Exception
                Call App.LogException(ex)
                Return -1
        End Try
#End If

            Return 0
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="ModelProperty">本参数里面除了模型属性的参数外，还有基因组的注释数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("-compile", Usage:="-door <doorFile> -ec <ec_file> [-myva_cog <myva_cog_file>]")>
        Public Overrides Function Compile(Optional ModelProperty As CommandLine.CommandLine = Nothing) As CellSystemXmlModel
            If MyBase.CompiledModel Is Nothing Then _
                Throw New InvalidOperationException("PreCompile operation maybe failed or you haven't call the method yet!")

            Call _Logging.WriteLine("[DEBUG] Here is the compile arguments list that user input:" & vbCrLf)
            For Each item As NamedValue(Of String) In ModelProperty
                Call _Logging.WriteLine(String.Format("{0} --> {1}", item.Name, item.Value))
            Next

            '     MyBase.CompiledModel.ChipData = New Microsoft.VisualBasic.ComponentModel.Href With {.Value = ModelProperty("-chipdata")}

            Me._argvs_Compile = ModelProperty
            Me._ECProfiles = ModelProperty("-ec").LoadCsv(Of T_EnzymeClass_BLAST_OUT).ToArray
            Call MyBase.WriteProperty(ModelProperty, MyBase.CompiledModel)

            Dim MyvaCog = If(ModelProperty Is Nothing OrElse String.IsNullOrEmpty(ModelProperty("-myva_cog")),
                             New MyvaCOG() {},
                             (ModelProperty <= "-myva_cog").AsDataSource(Of MyvaCOG)(, False))

            Dim SabiorkCompounds As String = ModelProperty("-sabiork")
            If Not String.IsNullOrEmpty(SabiorkCompounds) Then
                Call _Logging.WriteLine("Merger sabiork compounds from ""{0}""", SabiorkCompounds)
                Call New Components.MergeSabiork(_ModelIO, SabiorkCompounds).InvokeMergeCompoundSpecies()
            End If

            Dim KEGGCOmpounds As String = ModelProperty("-kegg_compounds")
            If Not String.IsNullOrEmpty(KEGGCOmpounds) Then
                Call _Logging.WriteLine("Merge kegg compounds from ""{0}""", KEGGCOmpounds)
                Call New Components.MergeKEGGCompounds(_ModelIO, KEGGCOmpounds).InvokeMergeCompoundSpecies()
            End If

            Dim Door = SMRUCC.genomics.Assembly.DOOR.Load(path:=ModelProperty("-door"))
            _ModelIO.DoorOperon = (From Operon In Door.DOOROperonView.Operons Select Operon.ConvertToCsvData).ToArray
            _ModelIO.CellSystemModel.OperonCounts = _ModelIO.DoorOperon.Count
            _Door = Door

            Me.CompiledModel.FilePath = String.Format("{0}/cell_model.xml", Me._ModelIO.ModelParentDIR)
            Me._ModelIO.MetabolismModel = FileStream.MetabolismFlux.CreateObject(Me._MetabolismNetwork, MetabolismEnzymeLink:=Me._ModelIO.EnzymeMapping, MetaCycReactions:=_MetaCyc.GetReactions)
            Me._ModelIO.GenomeAnnotiation = FileStream.GeneObject.CreateObject(Me._MetaCyc.GetGenes, MyvaCog)
            Me._ModelIO.TranscriptionModel = FileStream.TranscriptUnit.CreateObject(Me._TranscriptRegulations, Door.DOOROperonView)
            Me._ModelIO.CultivationMediums = New List(Of I_SubstrateRefx)(CultivationMediums.MetaCycDefault.Uptake_Substrates)
            Me._CrossTalks = IO.File.Load(ModelProperty("-cross_talks"))

            'Sub New(Compiler As Compiler, KEGGReactionsCsv As String, KEGGCompoundsCsv As String, CARMENCsv As String)
            Dim KEGGReactions As String = ModelProperty("-kegg_reactions")
            If Not String.IsNullOrEmpty(KEGGCOmpounds) AndAlso Not String.IsNullOrEmpty(KEGGReactions) AndAlso Not String.IsNullOrEmpty(ModelProperty("-carmen")) Then
                Call New Components.MergeKEGGReactions(_ModelIO, KEGGReactions, KEGGCOmpounds, ModelProperty("-carmen")).InvokeMethods()
            End If

            Dim SabiorkKinetics As String = ModelProperty("-sabiork_kinetics")
            Dim SabiorkEnzymeModifierKinetics As String = ModelProperty("-enzyme_modify_kinetics")
            Dim Expasy As String = ModelProperty("-expasy_class")
            If Not String.IsNullOrEmpty(SabiorkKinetics) AndAlso
                Not String.IsNullOrEmpty(SabiorkEnzymeModifierKinetics) AndAlso
                Not String.IsNullOrEmpty(Expasy) Then

                Call New Components.SabiorkKinetics(_ModelIO, SabiorkKinetics, SabiorkEnzymeModifierKinetics,
                                                    Expasy.LoadCsv(Of T_EnzymeClass_BLAST_OUT)(False).ToArray) _
                        .InvokeMethod(_MetaCyc,
                                      KEGGCOmpounds.LoadCsv(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound)(False).ToArray,
                                      KEGGReactions.LoadCsv(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction)(False).ToArray)
            End If

            Dim LinkFlag = Me.Link
            If LinkFlag = 0 Then
                MyBase.CompiledModel = _ModelIO.CellSystemModel
                Return MyBase.CompiledModel
            Else
                Return Nothing
            End If
        End Function
#End Region

#Region ""

        ''' <summary>
        ''' 所有与MetaCyc数据库相关的模块变量请在这里初始化
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub __Initialize_MetaCyc(MetaCyc As DatabaseLoadder)
            MetaCyc_GeneId_Mapping = GetGeneId(MetaCyc)
            Me._MetaCyc = MetaCyc
        End Sub

        Protected Sub _createRibosomeAssembly(PttDir As String)
            Dim Ptt As String = FileIO.FileSystem.GetFiles(PttDir, FileIO.SearchOption.SearchTopLevelOnly, "*.ptt").First
            Dim Rnt As String = FileIO.FileSystem.GetFiles(PttDir, FileIO.SearchOption.SearchTopLevelOnly, "*.rnt").First
            Call New GCTabular.Compiler.Components.RibosomeAssembly(Ptt, Rnt, Me._ModelIO).Invoke()
        End Sub

        Protected MetaCyc_GeneId_Mapping As Dictionary(Of String, String)

        ''' <summary>
        ''' 将MetaCyc的基因号映射为NCBI上面的基因号
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetGeneId(MetaCyc As DatabaseLoadder) As Dictionary(Of String, String)
            Dim MetaCycGene = MetaCyc.GetGenes
            Dim LQuery As Dictionary(Of String, String) = (From GeneObject As Slots.Gene
                                                           In MetaCycGene.Values.AsParallel
                                                           Let getId As String = GeneObject.Identifier & "-MONOMER"
                                                           Select getId,
                                                               GeneObject.Accession1) _
                                                               .ToDictionary(Function(item) item.getId,
                                                                             Function(item) item.Accession1)
            Return LQuery
        End Function

        ''' <summary>
        ''' 这个方法主要是用于对于有Effector的TF生成TF的复合物，从而产生代谢组对基因表达调控的约束
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function _createProteinAssembly(ByRef Regulators As List(Of FileStream.Regulator),
                                                  ByRef Metabolites As Dictionary(Of FileStream.Metabolite)) As Dictionary(Of String, FileStream.ProteinAssembly)
            Dim AssemblyList = New Dictionary(Of String, FileStream.ProteinAssembly)
            Dim ProteinComplexe = (From Protein In _MetaCyc.GetProteins.AsParallel Where Protein.Types.IndexOf("Polypeptides") = -1 Select Protein).ToArray

            For Each PC As Slots.Protein In ProteinComplexe  '添加MetaCyc数据库之中的蛋白质复合物
                Dim PC_ID As String = PC.Identifier

                If Not AssemblyList.ContainsKey(PC_ID) Then

                    Dim Metabolite = New FileStream.Metabolite With {
                        .Identifier = PC_ID,
                        .InitialAmount = 1000,
                        .CommonNames = New String() {PC_ID},
                        .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes
                    }

                    Dim Assembly As FileStream.ProteinAssembly = New FileStream.ProteinAssembly With {
                        .ProteinComplexes = PC_ID,
                        .Upper_Bound = 10,
                        .Lambda = 0.8,
                        .p_Dynamics_K = 1,
                        .ProteinComponents = (From s As String In PC.Components Let getId = If(MetaCyc_GeneId_Mapping.ContainsKey(s), MetaCyc_GeneId_Mapping(s), s) Select getId).ToArray
                    }
                    Call AssemblyList.InsertOrUpdate(Assembly)
                    Call Metabolites.InsertOrUpdate(Metabolite) ',
                    '                                .DBLinks = New String() {
                    'New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "AUTO", .AccessionId = Id}.GetFormatValue}})
                End If
            Next

            Dim RegulatorPC_IDs = (From Regulator In Regulators.AsParallel Where Not Regulator.Effectors.IsNullOrEmpty Select Regulator.get_PCs).ToArray.ToVector.Distinct.ToArray    '添加调控因子蛋白质复合物

            For Each PC In RegulatorPC_IDs
                Dim PC_ID As String = PC.Key

                If Not AssemblyList.ContainsKey(PC_ID) Then

                    Dim Metabolite = New FileStream.Metabolite With
                                     {
                                         .Identifier = PC_ID, .InitialAmount = 1000,
                                         .CommonNames = New String() {PC_ID},
                                         .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes}

                    Dim Assembly As FileStream.ProteinAssembly = New FileStream.ProteinAssembly With
                                                                 {
                                                                     .ProteinComplexes = PC_ID,
                                                                     .Upper_Bound = 10,
                                                                     .Lambda = 0.8,
                                                                     .p_Dynamics_K = 1,
                                                                     .ProteinComponents = PC.Value}
                    Call AssemblyList.InsertOrUpdate(Assembly)
                    Call Metabolites.InsertOrUpdate(Metabolite) ',
                    '                                .DBLinks = New String() {
                    'New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "AUTO", .AccessionId = Id}.GetFormatValue}})
                End If
            Next

            Return AssemblyList
        End Function

        Protected Sub __createProteinObjects(EcProfiles As IEnumerable(Of T_EnzymeClass_BLAST_OUT))
            Dim ProfileDicts = EcProfiles.ToDictionary(Function(matched) matched.ProteinId)
            Me._ModelIO.Proteins = LinqAPI.MakeList(Of FileStream.Protein) <=
                From Transcript
                In Me._ModelIO.Transcripts
                Where Not Transcript.PolypeptideCompositionVector.IsNullOrEmpty
                Let ECValue = If(ProfileDicts.ContainsKey(Transcript.Template), ProfileDicts(Transcript.Template).Class, "")
                Let Type = If(String.IsNullOrEmpty(ECValue),
                    GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Polypeptide.ProteinTypes.Polypeptide,
                    GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Polypeptide.ProteinTypes.Enzyme)
                Select New FileStream.Protein With {
                    .Lambda = 0.85,
                    .PolypeptideCompositionVector = Transcript.PolypeptideCompositionVector,
                    .Identifier = Transcript.Template,
                    .ECNumber = ECValue,
                    .ProteinType = Type
                }
        End Sub

        Protected Sub _createEnzymeObjects()
            Dim EnzymeIdArray = (From enz As FileStream.MetabolismFlux
                                 In Me._ModelIO.MetabolismModel
                                 Where Not enz.Enzymes.IsNullOrEmpty
                                 Select ReactionId = enz.Identifier, Enzymes = enz.Enzymes, Metabolite = enz.get_Metabolites.First).ToArray
            Dim rand As New Random
            Dim LQuery = (From item In EnzymeIdArray.AsParallel
                          Let enzCollection = (From EnzymeId As String
                                               In item.Enzymes
                                               Select New EnzymeCatalystKineticLaw With {
                                                          .Enzyme = EnzymeId, .KineticRecord = item.ReactionId,
                                                          .PH = 7, .Temperature = 28,
                                                          .Km = Math.Max(rand.NextDouble, 0.5),
                                                          .Kcat = Math.Max(5000, 10000 * rand.NextDouble),
                                                          .Metabolite = item.Metabolite}).ToArray
                          Select enzCollection).ToArray
            Me._ModelIO.Enzymes = LQuery.Unlist

            EnzymeIdArray = (From enz
                In Me._ModelIO.TransmembraneTransportation
                             Where Not enz.Enzymes.IsNullOrEmpty
                             Select ReactionId = enz.Identifier, Enzymes = enz.Enzymes, Metabolite = enz.get_Metabolites.First).ToArray

            For Each TrModel In EnzymeIdArray
                _ModelIO.Enzymes += From Id
                                    In TrModel.Enzymes
                                    Select New EnzymeCatalystKineticLaw With {
                                        .Enzyme = Id, .KineticRecord = TrModel.ReactionId,
                                        .PH = 7,
                                        .Temperature = 28,
                                        .Km = Math.Max(rand.NextDouble, 0.5),
                                        .Kcat = Math.Max(5000, 10000 * rand.NextDouble()),
                                        .Metabolite = TrModel.Metabolite
                                    }
            Next

            Dim EnzymeGenes = New List(Of String)
            For Each item In EnzymeIdArray
                Call EnzymeGenes.AddRange(item.Enzymes)
            Next
            EnzymeGenes = LinqAPI.MakeList(Of String) <= From strId As String
                                                         In EnzymeGenes
                                                         Select strId
                                                         Distinct
                                                         Order By strId Ascending
            For Each strId As String In EnzymeGenes
                Dim ProteinPolypeptide = _ModelIO.Proteins.Take(uniqueId:=strId)
                If ProteinPolypeptide Is Nothing Then '可能为蛋白质复合物
                    Call _Logging.WriteLine(String.Format("ASSIGN_PROTEIN_TYPES: {0} is not a polypeptide and it may be protein complex...", strId))
                    Continue For
                End If
                ProteinPolypeptide.ProteinType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Polypeptide.ProteinTypes.Enzyme
            Next
        End Sub

        Private Function GetTranscriptSequenceData(FsaObject As FastaObjects.GeneObject, MetaCycGenes As Genes) As KeyValuePair(Of String, String)
            Dim UniqueId As String = FsaObject.UniqueId
            Dim Gene = MetaCycGenes.Item(UniqueId)
            If Gene Is Nothing Then
                Call Me._Logging.WriteLine(String.Format("Could not found Metacyc gene for {0}", UniqueId),
                                           "GenerateCompositionVectors(Transcripts As FileStream.Transcript())",
                                           MSG_TYPES.WRN)
                UniqueId = FsaObject.AccessionId
            Else
                UniqueId = Gene.Accession1
            End If

            Return New KeyValuePair(Of String, String)(UniqueId, FsaObject.SequenceData.ToUpper)
        End Function

        Private Function GetProteinSequenceData(FsaObject As FastaObjects.Proteins, MetaCycGenes As Genes) As KeyValuePair(Of String, String)
            Dim UniqueId As String = FsaObject.UniqueId
            Dim LQuery = (From ItemGene As Slots.Gene
                          In MetaCycGenes
                          Where ItemGene.Product.IndexOf(UniqueId) > -1
                          Select ItemGene.Accession1).ToArray

            If LQuery.IsNullOrEmpty Then
                Call Me._Logging.WriteLine(String.Format("Could not found Metacyc protein for {0}", UniqueId),
                                           "GenerateCompositionVectors(Transcripts As FileStream.Transcript())",
                                           Type:=MSG_TYPES.WRN)
                UniqueId = FsaObject.UniqueId
            Else
                UniqueId = LQuery.First
            End If

            Return New KeyValuePair(Of String, String)(UniqueId, FsaObject.SequenceData.ToUpper)
        End Function

        Private Sub GenerateCompositionVectors(Transcripts As List(Of FileStream.Transcript), GeneSequences As KeyValuePair(Of String, String)(), ProteinSequences As KeyValuePair(Of String, String)())
            Dim PossibleRNAGene As List(Of String) = New List(Of String)

            For i As Integer = 0 To Transcripts.Count - 1
                Dim TranscriptObject = Transcripts(i)
                Dim AccessionId As String = TranscriptObject.Template
                Dim GeneSequence = (From item In GeneSequences Where String.Equals(item.Key, AccessionId) Select item).ToArray
                Dim ProteinSequence = (From item In ProteinSequences Where String.Equals(item.Key, AccessionId) Select item).ToArray

                If Not GeneSequence.IsNullOrEmpty Then
                    Dim SequenceData = GeneSequence.First.Value

                    TranscriptObject.TranscriptCompositionVectors = SMRUCC.genomics.SequenceModel.NucleotideModels.GetCompositionVector(SequenceData)
                    TranscriptObject.Lamda = 0.65 * (1 + GCContent(SequenceData)) / Math.Log10(Len(SequenceData) + 10)
                Else
                    Call Me._Logging.WriteLine(String.Format("{0}, Gene sequence not found!", AccessionId),
                                               "GenerateCompositionVectors(Transcripts As FileStream.Transcript())",
                                               MSG_TYPES.ERR)
                End If
                If Not ProteinSequence.IsNullOrEmpty Then
                    TranscriptObject.PolypeptideCompositionVector = Polypeptide.GetCompositionVector(ProteinSequence.First.Value)
                Else
                    TranscriptObject.Product = ""
                    Call PossibleRNAGene.Add(AccessionId)
                End If
            Next

            Call Me._Logging.WriteLine(String.Format("{0}, polypeptide sequence not found! They may be RNA coding genes...", CollectionAttribute.CreateObject(PossibleRNAGene, "; ")),
                                          "GenerateCompositionVectors(Transcripts As FileStream.Transcript())",
                                          MSG_TYPES.ERR)
        End Sub

        ''' <summary>
        ''' 连接效应物到调控因子之上
        ''' </summary>
        ''' <param name="CrossTalks"></param>
        ''' <remarks></remarks>
        Protected Sub LinkEffectors(CrossTalks As IO.File)
            Dim FilteredList As New List(Of FileStream.Regulator)
            Dim EffectorMapping = Me._ModelIO.EffectorMapping

            Call New Components.SignalTransductionNetwork(Me._ModelIO, _ModelIO.StringInteractions, _Logging).Invoke(Me._ModelIO.TranscriptionModel, _Door, CrossTalks:=CrossTalks)

            For Each Regulator In Me._ModelIO.Regulators

                If Regulator.Effectors.IsNullOrEmpty Then   '对于这些调控因子没有Effector，则直接添加进入列表中，因为这些调控因子不需要Effector
                    Call FilteredList.Add(Regulator)
                    Continue For
                End If

                Dim Effectors As String() = (From str As String In Regulator.Effectors Where Not String.IsNullOrEmpty(str) Select str).ToArray '获取对应的调控因子的Effector列表

                '在这将Regprecise中的regulator编号转换为MetaCyc中的物质编号
                Effectors = (From strEffector As String
                             In Effectors
                             Let map = EffectorMapping.GetItem(strEffector.ToLower)
                             Where Not map Is Nothing AndAlso Not String.IsNullOrEmpty(map.MetaCycId)
                             Select map.MetaCycId Distinct).ToArray

                Regulator.Effectors = Effectors

                If Not Regulator.Effectors.IsNullOrEmpty Then '生成蛋白质复合物
                    Dim PCs = (From s As String In Regulator.Effectors Select ID = String.Format("[{0}][{1}]", Regulator.ProteinId, s), Components = {s, Regulator.ProteinId}).ToArray
                    For Each Id In PCs

                        If Not Me._ModelIO.ProteinAssembly.ContainsKey(Id.ID) Then
                            Dim PC As New FileStream.ProteinAssembly With {.Comments = "Regulator-Effector Protein Complexes", .p_Dynamics_K = 1, .Lambda = 0.8, .ProteinComplexes = Id.ID, .ProteinComponents = Id.Components, .Upper_Bound = 10}
                            Call Me._ModelIO.ProteinAssembly.Add(PC.ProteinComplexes, PC)
                        End If
                    Next
                End If

                Call FilteredList.Add(Regulator)
            Next

            For Each Regulator In FilteredList   '检查数据的一致性，假若调控因子不存在于代谢物列表之中，则新建一个模型对象

                Dim collection = Regulator.get_PCs

                If collection.IsNullOrEmpty Then
                    Dim UniqueID As String = Regulator.ProteinId

                    If Not Me._ModelIO.MetabolitesModel.ContainsKey(UniqueID) Then
                        Dim RegulatorMetabolite As FileStream.Metabolite = New FileStream.Metabolite With
                                                                           {
                                                                               .CommonNames = New String() {Regulator.ProteinId},
                                                                               .Identifier = UniqueID,
                                                                               .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Polypeptide,
                                                                               .InitialAmount = 1000}
                        Call Me._Logging.WriteLine(UniqueID & " is not exists in the metabolites, added!")
                        Call Me._ModelIO.MetabolitesModel.Add(UniqueID, RegulatorMetabolite)
                    End If

                    Continue For
                End If

                For Each PC_ID In collection

                    Dim UniqueID As String = PC_ID.Key

                    If Not Me._ModelIO.MetabolitesModel.ContainsKey(UniqueID) Then
                        Dim RegulatorMetabolite As FileStream.Metabolite = New FileStream.Metabolite With
                                                                           {
                                                                               .CommonNames = New String() {Regulator.ProteinId},
                                                                               .Identifier = UniqueID,
                                                                               .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes,
                                                                               .InitialAmount = 1000}
                        Call Me._Logging.WriteLine(UniqueID & " is not exists in the metabolites, added!")
                        Call Me._ModelIO.MetabolitesModel.Add(UniqueID, RegulatorMetabolite)
                    End If
                Next
            Next

            Me._ModelIO.Regulators = FilteredList
        End Sub

        Private Sub CreatePathwayObjects()
            Dim Pathways = Me._MetaCyc.GetPathways
            Me._ModelIO.Pathway = LinqAPI.MakeList(Of FileStream.Pathway) <=
                From item
                In Pathways
                Select New FileStream.Pathway With {
                    .Identifier = item.Identifier,
                    .Comment = item.Comment,
                    .MetabolismFlux = item.ReactionList.ToArray
                }
        End Sub

        ''' <summary>
        ''' 执行连接操作并将临时数据保存至Exported文件夹
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>请注意每一步的函数调用之间是有顺序</remarks>
        Protected Overrides Function Link() As Integer
            Dim Metabolites = Me._ModelIO.MetabolitesModel

            Call CreatePathwayObjects()

            For Each MetabolismFlux In Me._ModelIO.MetabolismModel
                If MetabolismFlux.Enzymes.IsNullOrEmpty Then
                    Continue For
                End If

                Dim i As Integer = 0
                For Each EnzymeId As String In MetabolismFlux.Enzymes

                    If Not Metabolites.ContainsKey(EnzymeId) Then
                        Dim Metabolite = New FileStream.Metabolite With
                                     {
                                         .CommonNames = New String() {EnzymeId}, .Identifier = EnzymeId,
                                         .InitialAmount = 1000,
                                         .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes}
                        Call Metabolites.InsertOrUpdate(Metabolite) ',
                        '                               .DBLinks = New String() {
                        'New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "AUTO", .AccessionId = EnzymeId}.GetFormatValue}})
                    End If

                    i += 1
                Next
            Next

            Dim MetaCycGenes = _MetaCyc.GetGenes
            Dim GeneSequences As KeyValuePair(Of String, String)() = (From FsaObject In _MetaCyc.Database.FASTAFiles.DNAseq Select GetTranscriptSequenceData(FsaObject, MetaCycGenes)).ToArray
            Dim ProteinSequences As KeyValuePair(Of String, String)() = (From FsaObject In _MetaCyc.Database.FASTAFiles.protseq Select GetProteinSequenceData(FsaObject, MetaCycGenes)).ToArray

            Call _createTranscripts(GeneSequences, ProteinSequences)
            Call LinkEffectors(Me._CrossTalks)
            Call __createProteinObjects(Me._ECProfiles)
            Call _createRibosomeAssembly(_argvs_Compile("-ptt_dir"))

            Me._ModelIO.ProteinAssembly = _createProteinAssembly(Me._ModelIO.Regulators, Metabolites)
            Me._ModelIO.ConstraintMetabolites = New List(Of GCML_Documents.ComponentModels.ConstraintMetaboliteMap)(
                GCMarkupLanguage.GCML_Documents.ComponentModels.ConstraintMetaboliteMap.CreateObjectsWithMetaCyc)
            Me._ModelIO.TransmembraneTransportation = AnalysisTransmenbraneFlux()

            Call _createEnzymeObjects()
            Call Components.AssociatedFluxAnalysis.ApplyAnalysis(_ModelIO)
            Call New Components.MolecularWeightCalculator().CalculateK(ModelLoader:=Me._ModelIO)

            Dim Regulators = (From item In _ModelIO.Regulators Where item.Effectors.IsNullOrEmpty Select item.ProteinId Distinct).ToArray
            For Each strId As String In Regulators
                Dim Regulator = _ModelIO.Proteins.Take(uniqueId:=strId)
                Regulator.ProteinType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Polypeptide.ProteinTypes.TranscriptFactor
            Next

            Call Me._ModelIO.SaveTo(My.Computer.FileSystem.SpecialDirectories.Temp & "/_________temp_gcmodeller_model.xml")

            Return 0
        End Function

        ''' <summary>
        ''' {UniqueId, SequenceData.ToUpper}
        ''' </summary>
        ''' <param name="GeneSequence"></param>
        ''' <param name="ProteinSequence"></param>
        ''' <remarks></remarks>
        Protected Sub _createTranscripts(GeneSequence As KeyValuePair(Of String, String)(), ProteinSequence As KeyValuePair(Of String, String)())
            Me._ModelIO.Transcripts = LinqAPI.MakeList(Of FileStream.Transcript) <=
                From GeneObject
                In Me._ModelIO.GenomeAnnotiation
                Select _____createTranscript(GeneObject)

            Call GenerateCompositionVectors(Me._ModelIO.Transcripts, GeneSequence, ProteinSequence)
        End Sub

        Private Function _____createTranscript(GeneObject As FileStream.GeneObject) As FileStream.Transcript
            Dim UniqueId As String = GeneObject.Identifier & ".TRANSCRIPT"
            Dim Transcript As FileStream.Transcript = New FileStream.Transcript With {
                .UniqueId = UniqueId, .Template = GeneObject.Identifier,
                .Product = GeneObject.Identifier,
                .Comments = GeneObject.GeneName & ".TRANSCRIPT",
                .TranscriptType = GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.TranscriptTypes.mRNA}

            GeneObject.TranscriptProduct = UniqueId

            Dim ProductMetabolite As New FileStream.Metabolite With {
                .CommonNames = {Transcript.Product},
                .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Polypeptide,
                .Identifier = Transcript.Product,
                .InitialAmount = 1000
            } ',
            '    .DBLinks = New String() {New SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "NCBI", .AccessionId = Transcript.Product}.GetFormatValue}}

            Call _ModelIO.MetabolitesModel.InsertOrUpdate(ProductMetabolite)

            Return Transcript
        End Function

        Protected Overrides Sub Dispose(Disposing As Boolean)
            Call _Logging.Save(True)
            MyBase.Dispose(Disposing)
        End Sub

        ''' <summary>
        ''' 本函数会将<see cref="FileStream.IO.XmlresxLoader.TransmembraneTransportation"></see>解析完毕，并使用<see cref="FileStream.MetabolismFlux.Identifier"></see>属性
        ''' 从<see cref="FileStream.IO.XmlresxLoader.MetabolismModel"></see>列表之中移除相对应的过程
        ''' </summary>
        ''' <remarks></remarks>
        Protected Function AnalysisTransmenbraneFlux() As List(Of FileStream.MetabolismFlux)
            Call _Logging.WriteLine("Start to analysis the transportation flux object...")

            Dim LQuery = LinqAPI.MakeList(Of SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction) <=
                From item As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Reaction
                In Me._MetaCyc.GetReactions.AsParallel
                Where item.IsTransportReaction
                Select New SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction(item) '查询出所有的跨膜转运过程

            Dim EnzymeReactions = _MetaCyc.GetEnzrxns
            Dim TrModel As List(Of FileStream.MetabolismFlux) = New List(Of FileStream.MetabolismFlux)
            Dim CultivationMetabolites As List(Of SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction.CompoundSpecies) =
                New List(Of SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction.CompoundSpecies)

            For Each SchemaModel In LQuery
                Dim PreModel = _ModelIO.MetabolismModel.GetItem(SchemaModel.Identifier)

                If Not PreModel Is Nothing Then
                    Call _ModelIO.MetabolismModel.Remove(PreModel)
                End If

                Dim Enzymes As String() = (From Id As String In SchemaModel.EnzymaticReaction Select EnzymeReactions.Item(Id).Enzyme).ToArray
                Call TrModel.Add(CreateNewTransmembraneModel(SchemaModel, Enzymes))
                Call CultivationMetabolites.AddRange(SchemaModel.GetSubstrates)
            Next

            CultivationMetabolites = New List(Of genomics.Assembly.MetaCyc.Schema.TransportReaction.CompoundSpecies)(
                From item
                In CultivationMetabolites
                Where String.Equals(item.Compartment, "CCO-OUT", StringComparison.OrdinalIgnoreCase)
                Select item)

            _ModelIO.CultivationMediums = LinqAPI.MakeList(Of I_SubstrateRefx) <=
                From MetaboliteId
                In (From item In CultivationMetabolites Select item.Identifier Distinct)
                Select New I_SubstrateRefx With {
                    .Identifier = MetaboliteId.ToUpper,
                    .InitialQuantity = 100
                }

            For Each Substrate In _ModelIO.CultivationMediums
                If Not _ModelIO.MetabolitesModel.ContainsKey(Substrate.Identifier) Then
                    Dim Metabolite As New FileStream.Metabolite With {
                        .Identifier = Substrate.Identifier,
                        .InitialAmount = 1000,
                        .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Compound
                    }

                    Call _ModelIO.MetabolitesModel.InsertOrUpdate(Metabolite)
                    Call _Logging.WriteLine(String.Format("{0} was not found in the metabolism model and add into the model right now!", Substrate.Identifier))
                End If
            Next

            Call _Logging.WriteLine(String.Format("There are {0} transmembrane flux was analysised!", TrModel.Count))
            Call _Logging.WriteLine("analysis job done!")

            Return TrModel
        End Function

        Private Function CreateNewTransmembraneModel(Model As SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction, Enzymes As String()) As FileStream.MetabolismFlux
            Dim DataModle As FileStream.MetabolismFlux = New FileStream.MetabolismFlux With {.Identifier = Model.Identifier, .CommonName = Model.CommonName}
            DataModle.LOWER_Bound = -100
            DataModle.UPPER_Bound = 100
            DataModle.p_Dynamics_K_1 = 1
            DataModle.p_Dynamics_K_2 = 1
            DataModle.Enzymes = (From s As String In Enzymes Let id As String = If(MetaCyc_GeneId_Mapping.ContainsKey(s), MetaCyc_GeneId_Mapping(s), s) Select id).ToArray
            DataModle.Equation = Model.CreateEquatopnExpression.ToUpper
            Return DataModle
        End Function
#End Region
    End Class
End Namespace
