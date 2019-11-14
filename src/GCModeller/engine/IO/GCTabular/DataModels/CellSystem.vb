#Region "Microsoft.VisualBasic::3b7111259609e6f185119fe5aaf893c5, engine\IO\GCTabular\DataModels\CellSystem.vb"

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

    '     Class CellSystem
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: __createReference, CreateSignalTransductionNetwork, CreateTranscriptUnits, CreateTransmembraneTransportation, CreateTransportationFlux
    '                   get_Motifs, GetMetabolismNetwork, Internal_Process, InternalCreate_TU_MODEL, LoadAction
    '                   (+2 Overloads) LoadModel, ToString
    ' 
    '         Sub: CreateTranscripts, (+2 Overloads) Dispose, LoadSystemVariables
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.SabiorkKineticLaws.TabularDump
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat
Imports SMRUCC.genomics.Model.Network.STRING
Imports SMRUCC.genomics.Model.Network.STRING.Models

Namespace DataModel

    ''' <summary>
    ''' Data reader for the model file: <see cref="CellSystemXmlModel"></see>
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <[Namespace]("csv_model.stream_service")>
    Public Class CellSystem : Implements System.IDisposable

        Dim _Logging As LogFile
        Dim _CellSystemModel As FileStream.IO.XmlresxLoader

        Sub New(CellSystem As String, SystemLogging As LogFile)
            Me._Logging = SystemLogging
            Call _Logging.WriteLine($"Load csv model data from ""{CellSystem}"" and create new object stream!",
                                    "CellSystemDataReader->Constructor()",
                                    MSG_TYPES.INF)
            Me._CellSystemModel = New FileStream.IO.XmlresxLoader(CellSystem)
        End Sub

        Sub New(CellSystem As FileStream.IO.XmlresxLoader, SystemLogging As LogFile)
            Me._Logging = SystemLogging
            Me._CellSystemModel = CellSystem
        End Sub

        Public Function GetMetabolismNetwork() As DataModel.FluxObject()
            Call _Logging.WriteLine("Start to generate the metabolism network!", "GetMetabolismNetwork()", MSG_TYPES.INF)

            Dim LQuery = (From item In Me._CellSystemModel.MetabolismModel.AsParallel Select item.CreateObject).ToArray
            Return LQuery
        End Function

        Private Sub CreateTranscripts(TargetModel As BacterialModel)
            Dim Transcripts = (From Transcript As FileStream.Transcript
                               In Me._CellSystemModel.Transcripts.AsParallel
                               Let cv = New GCMarkupLanguage.SequenceModel.CompositionVector With {
                                   .T = Transcript.TranscriptCompositionVectors
                               }
                               Let pcv = New GCMarkupLanguage.SequenceModel.CompositionVector With {
                                   .T = Transcript.PolypeptideCompositionVector
                               }
                               Select New Transcript With {
                                   .CompositionVector = cv,
                                   .PolypeptideCompositionVector = pcv,
                                   .Lamda = Transcript.Lamda,
                                   .Identifier = Transcript.UniqueId,
                                   .Template = Transcript.Template,
                                   .Product = Transcript.Product,
                                   .TranscriptType = Transcript.TranscriptType
                               }).ToArray

            Dim TranscriptMetabolites As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite() = (
                From Transcript As Transcript In Transcripts.AsParallel
                Select New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite With {
                    .CommonName = Transcript.Identifier,
                    .InitialAmount = 1000,
                    .Identifier = Transcript.Identifier
                }).ToArray

            TargetModel.BacteriaGenome.Transcripts = Transcripts
            TargetModel.Metabolism.Metabolites += TranscriptMetabolites
        End Sub

        Private Sub LoadSystemVariables(Model As GCMarkupLanguage.BacterialModel, DataModel As FileStream.IO.XmlresxLoader)
            'Dim ChunkBuffer = _CellSystemModel.SystemVariables.AsList
            'Call ChunkBuffer.Add(New SMRUCC.genomics.ComponentModel.KeyValuePair With {
            '                     .Key = SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables._URL_CHIPDATA,
            '                     .Value = DataModel.CellSystemModel.ChipData.GetFullPath(Me._CellSystemModel.ModelParentDir)})
            'Model.SystemVariables = ChunkBuffer.ToArray
        End Sub

        <ExportAPI("stream.from_file", Info:="Load the csv tabular format virtual cell model file as GCML format model file, due to the reason of the GCModeller is only support the GCML format modl file." &
            "argument ""model"" is the main xml file of the virtual cell model file.")>
        Public Shared Function LoadModel(model As String, Optional logging_at As String = "") As GCMarkupLanguage.BacterialModel
            If String.IsNullOrEmpty(logging_at) Then
                logging_at = String.Format("{0}/csv_compiler__{1}.log", Settings.DataCache, LogFile.NowTimeNormalizedString)
            End If
            Return New CellSystem(model, New LogFile(Path:=logging_at)).LoadAction
        End Function

        <ExportAPI("stream.from_model")>
        Public Shared Function LoadModel(model As FileStream.IO.XmlresxLoader, Optional logging_at As String = "") As GCMarkupLanguage.BacterialModel
            If String.IsNullOrEmpty(logging_at) Then
                logging_at = String.Format("{0}/csv_compiler__{1}.log", Settings.DataCache, LogFile.NowTimeNormalizedString)
            End If
            Return New CellSystem(model, New LogFile(Path:=logging_at)).LoadAction
        End Function

        Public Function LoadAction() As GCMarkupLanguage.BacterialModel

            Call _Logging.WriteLine("Start to streaming csv data into gcml data model!", "LoadAction()", MSG_TYPES.INF)

            Dim Dispositions = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant() {
                New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant With {
                    .Enzymes = Strings.Split(Me._CellSystemModel.SystemVariables.GetItem(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_POLYPEPTIDE_DISPOSE_CATALYST).Value, "; "),
                    .GeneralType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant.GENERAL_TYPE_ID_POLYPEPTIDE,
                    .UPPER_BOUND = 1000},
                New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant With {
                    .Enzymes = Strings.Split(Me._CellSystemModel.SystemVariables.GetItem(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_TRANSCRIPT_DISPOSE_CATALYST).Value, "; "),
                    .GeneralType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant.GENERAL_TYPE_ID_TRANSCRIPTS,
                    .UPPER_BOUND = 1000}}

            Dim Model As GCMarkupLanguage.BacterialModel =
                New GCMarkupLanguage.BacterialModel With
                {
                    .ModelProperty = Me._CellSystemModel.CellSystemModel.ModelProperty,
                    .IteractionLoops = Me._CellSystemModel.CellSystemModel.IteractionLoops}

            Model.Metabolism = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolism With
                               {
                                   .Compartments = New List(Of GCMarkupLanguage.GCML_Documents.ComponentModels.Compartment)}
            Model.BacteriaGenome = New GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.BacterialGenome With
                                   {
                                       .Genes = (From ItemGene
                                                 In Me._CellSystemModel.GenomeAnnotiation.AsParallel
                                                 Select New GeneObject With
                                                        {
                                                            .AccessionId = ItemGene.Identifier,
                                                            .CommonName = ItemGene.GeneName,
                                                            .TranscriptProduct = ItemGene.TranscriptProduct,
                                                            .Identifier = ItemGene.Identifier}).ToArray}
            Model.BacteriaGenome.TransUnits = CreateTranscriptUnits(Model.BacteriaGenome.Genes)

            Model.Metabolism.MetabolismNetwork = (From item In Me.GetMetabolismNetwork.AsParallel
                                                  Let rnts = (From m In item.LeftSides
                                                              Select New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With
                                                                     {
                                                                         .Identifier = m.Identifier, .StoiChiometry = m.StoiChiometry}).ToArray
                                                  Let prdts = (From m In item.RightSide
                                                               Select New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With
                                                                      {
                                                                          .Identifier = m.Identifier, .StoiChiometry = m.StoiChiometry}).ToArray
                                                  Let regulators = (From enz
                                                                    In item.AssociatedRegulationGenes
                                                                    Select New GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator With
                                                                           {
                                                                               .Identifier = enz.Identifier, .CommonName = enz.Identifier, .Activation = True}).AsList
                                                  Select New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With
                                                         {
                                                             .LOWER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With
                                                                            {
                                                                                .Value = item.Lower_Bound},
                                                             .Name = item.Identifier,
                                                             .ObjectiveCoefficient = 1,
                                                             .Reversible = item.Reversible,
                                                             .p_Dynamics_K_1 = item.K1,
                                                             .p_Dynamics_K_2 = item.K2,
                                                             .Identifier = item.Identifier,
                                                             .Reactants = rnts,
                                                             .Products = prdts,
                                                             .UPPER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With
                                                                            {
                                                                                .Value = item.Upper_Bound},
                                                             .Enzymes = (From strId In item.AssociatedRegulationGenes Select strId.Identifier).ToArray,
                                                             .DynamicsRegulators = regulators}).AsList

            '    Model.Metabolism.MetabolismEnzymes = Me._CellSystemModel.Enzymes.ToArray

            Call LoadSystemVariables(Model, Me._CellSystemModel)

            Model.Metabolism.Metabolites = (From item As FileStream.Metabolite
                                            In Me._CellSystemModel.MetabolitesModel.Values.AsParallel
                                            Select New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite With
                                                   {
                                                       .Identifier = item.Identifier, .InitialAmount = item.InitialAmount,
                                                       .NumOfFluxAssociated = item.n_FluxAssociated,
                                                       .MetaboliteType = item.MetaboliteType}).AsList
            '  Model.Proteins = (From item In Me._CellSystemModel.Proteins.AsParallel Select New SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage. GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Protein With {.UniqueId = item.UniqueId, .Lamda = item.Lambda, .CommonName = item.UniqueId}).ToArray
            Model.Metabolism.ConstraintMetaboliteMaps = Me._CellSystemModel.ConstraintMetabolites.ToArray
            Model.ProteinAssemblies = (From assemblyEntry In Me._CellSystemModel.ProteinAssembly
                                       Let assembly = assemblyEntry.Value
                                       Select New GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly With {
                                           .Identifier = assembly.ProteinComplexes, .p_Dynamics_K_1 = assembly.p_Dynamics_K, .p_Dynamics_K_2 = assembly.Lambda,
                                           .UPPER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = assembly.Upper_Bound},
                                           .Reversible = True,
                                           .ComplexComponents = assembly.ProteinComponents,
                                           .Reactants = (From component In assembly.ProteinComponents
                                                         Select New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {
                                                             .Identifier = component, .StoiChiometry = 1}),
                                           .Products = {
                                               New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {
                                                   .Identifier = assembly.ProteinComplexes, .StoiChiometry = 1}}}).AsList

            Call CreateTranscripts(Model) '转录组分目标对象的形成需要代谢底物的句柄，故而目标对象的初始化需要在代谢组部分初始化完毕之后进行，而多肽链分子对象的构造则需要转录组对象，故而本对象的构造需要在构造多肽链分子对象之前完成

            Model.CultivationMediums = New GCMarkupLanguage.CultivationMediums With {.Uptake_Substrates = Me._CellSystemModel.CultivationMediums.ToArray}
            Model.DispositionModels = Dispositions
            Model.Polypeptides = (From PolypeptideModel As FileStream.Protein In _CellSystemModel.Proteins.AsParallel
                                  Let cv = New GCMarkupLanguage.SequenceModel.CompositionVector With {.T = PolypeptideModel.PolypeptideCompositionVector}
                                  Let Polypeptide = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Polypeptide With
                                                    {
                                                        .ProteinType = PolypeptideModel.ProteinType,
                                                        .CompositionVector = cv,
                                                        .DynamicsLamda = PolypeptideModel.Lambda,
                                                        .Identifier = PolypeptideModel.Identifier}
                                  Select Polypeptide
                                  Order By Polypeptide.Identifier Ascending).ToArray

            Model.SignalTransductionPathway = CreateSignalTransductionNetwork(Model)
            Model.Metabolism.Pathways = (From PathwayObject As FileStream.Pathway
                                         In Me._CellSystemModel.Pathway.AsParallel
                                         Where Not PathwayObject.MetabolismFlux.IsNullOrEmpty
                                         Let network = (From Id As String In PathwayObject.MetabolismFlux
                                                        Let HandleItem = Model.Metabolism.MetabolismNetwork.GetItem(Id)
                                                        Where Not HandleItem Is Nothing
                                                        Select HandleItem.Identifier).ToArray
                                         Select New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Pathway With
                                                {
                                                    .Comment = PathwayObject.Comment, .Name = PathwayObject.Identifier,
                                                    .Identifier = PathwayObject.Identifier,
                                                    .MetabolismNetwork = network}).ToArray
            Model.TransmembraneTransportation = CreateTransmembraneTransportation(_CellSystemModel.TransmembraneTransportation)

            Model.RibosomeAssembly = (From assembly In Me._CellSystemModel.RibosomeAssembly
                                      Let RxnId As String = If(String.IsNullOrEmpty(assembly.Comments), assembly.ProteinComplexes, String.Format("{0}.{1}", assembly.ProteinComplexes, assembly.Comments.Split.First))
                                      Select New GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly With {
                                          .ComplexComponents = assembly.ProteinComponents,
                                          .Identifier = RxnId, .UPPER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = assembly.Upper_Bound},
                                          .Reversible = False, .p_Dynamics_K_1 = assembly.p_Dynamics_K, .p_Dynamics_K_2 = assembly.Lambda,
                                          .Reactants = (From component In assembly.ProteinComponents
                                                        Select New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {
                                                            .Identifier = component, .StoiChiometry = 1}).ToArray,
                                          .Products = {
                                              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {
                                                  .Identifier = assembly.ProteinComplexes, .StoiChiometry = 1}}}).AsList

            Model.RNAPolymerase = (From assembly In Me._CellSystemModel.RNAPolymerase
                                   Let RxnId As String = If(String.IsNullOrEmpty(assembly.Comments), assembly.ProteinComplexes, String.Format("{0}.{1}", assembly.ProteinComplexes, assembly.Comments.Split.First))
                                   Select New GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.ProteinAssembly With {
                                       .ComplexComponents = assembly.ProteinComponents,
                                         .Identifier = RxnId, .UPPER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = assembly.Upper_Bound},
                                         .Reversible = False, .p_Dynamics_K_1 = assembly.p_Dynamics_K, .p_Dynamics_K_2 = assembly.Lambda,
                                         .Reactants = (From component In assembly.ProteinComponents
                                                       Select New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {
                                                           .Identifier = component, .StoiChiometry = 1}).ToArray,
                                         .Products = {
                                             New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {
                                                 .Identifier = assembly.ProteinComplexes, .StoiChiometry = 1}}}).AsList

            Dim Metabolites = (From item In Model.Metabolism.Metabolites Select item.Identifier Distinct Order By Identifier Ascending).ToArray
            Model.Metabolism.Metabolites = (From Id As String In Metabolites Select Model.Metabolism.Metabolites.GetItem(Id)).AsList

            'Model.BacteriaGenome.ExpressionKinetics = (From item In _CellSystemModel.ExpressionKinetics
            '                                           Select New EnzymeCatalystKineticLaw With {
            '                                               .Enzyme = item.ProteinComplex,
            '                                               .Km = item.Km,
            '                                               .PH = item.pH,
            '                                               .Temperature = item.Temperature}).ToArray
            Model.BacteriaGenome.OperonCounts = Me._CellSystemModel.CellSystemModel.OperonCounts
            Model.SystemVariables = Me._CellSystemModel.SystemVariables.ToArray
            Model.Metabolism.ConstraintMetaboliteMaps = _CellSystemModel.ConstraintMetabolites.ToArray
            Call _Logging.WriteLine("Streaming job done!", "LoadAction()", MSG_TYPES.INF)

            Return Model
        End Function

        Private Function CreateTransmembraneTransportation(Model As List(Of FileStream.MetabolismFlux)) As List(Of XmlElements.Metabolism.TransportationReaction)
            Dim LQuery = (From item In Model.AsParallel Select CreateTransportationFlux(item)).AsList
            Return LQuery
        End Function

        Private Shared Function __createReference(Data As MetaCyc.Schema.Metabolism.Compound()) As ComponentModels.CompoundSpeciesReference()
            Dim LQuery = (From m In Data
                          Let [property] = New MetaCyc.Schema.PropertyAttributes(m.Identifier)
                          Select New ComponentModels.CompoundSpeciesReference With {
                              .Identifier = [property].PropertyValue,
                              .StoiChiometry = m.StoiChiometry,
                              .CompartmentId = [property].Property("COMPARTMENT")}).ToArray
            Return LQuery
        End Function

        Private Shared Function CreateTransportationFlux(Model As FileStream.MetabolismFlux) As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.TransportationReaction
            Dim FluxModel = Model.CreateObject
            Dim Reactants = __createReference(FluxModel.LeftSides)
            Dim products = __createReference(FluxModel.RightSide)

            Dim DataModel = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.TransportationReaction With {
                                       .LOWER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = Model.LOWER_Bound},
                                       .Name = Model.CommonName, .ObjectiveCoefficient = 1, .Reversible = Model.Reversible, .Identifier = Model.Identifier,
                                       .Reactants = Reactants,
                                       .Products = products, .p_Dynamics_K_1 = FluxModel.K1, .p_Dynamics_K_2 = FluxModel.K2,
                                       .UPPER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = Model.UPPER_Bound},
                                       .Enzymes = Model.Enzymes}

            If Model.Enzymes.IsNullOrEmpty Then
                DataModel.DynamicsRegulators = New List(Of GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator)
            Else
                DataModel.DynamicsRegulators = (From enz In Model.Enzymes
                                                Select New GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator With
                                                       {
                                                           .Identifier = enz, .CommonName = enz, .Activation = True}).AsList
            End If

            Return DataModel
        End Function

        ''' <summary>
        ''' 创建信号转到网络的模型
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreateSignalTransductionNetwork(Model As BacterialModel) _
            As GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.SignalTransductionNetwork

            Model.SignalTransductionPathway = New GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.SignalTransductionNetwork

            Dim Inducers = Me._CellSystemModel.ChemotaxisSensing
            Dim SubstrateList As List(Of String) = New List(Of String)

            Dim ChemotaxisSensing As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.TransportationReaction) = New List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.TransportationReaction)
            Dim HKAutoPhosphorus As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) = New List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction)

            For Each Item As TCS.SensorInducers In Inducers

                If Item.Inducers.IsNullOrEmpty Then
                    Call _Logging.WriteLine(String.Format("Chemotaxis sensing profile for MCP ""{0}"" is null!", Item.SensorId), "CreateSignalTransductionNetwork", Type:=MSG_TYPES.WRN)
                    Continue For
                End If

                Dim LQuery = (From strInducerId As String
                              In Item.Inducers
                              Select New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.TransportationReaction With
                                     {
                                         .Enzymes = New String() {},
                                         .p_Dynamics_K_1 = 1, .p_Dynamics_K_2 = 1, .LOWER_BOUND = -100, .Reversible = True, .UPPER_BOUND = 100,
                                         .Identifier = String.Format("{0}.{1}", Item.SensorId, strInducerId),
                                         .Reactants = {
                                             New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With
                                             {
                                                 .StoiChiometry = 1, .Identifier = Item.SensorId, .CompartmentId = "CCO-IN"},
                                             New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With
                                             {
                                                 .StoiChiometry = 1, .Identifier = strInducerId, .CompartmentId = "CCO-OUT"}},
                                         .Products = {
                                             New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With
                                             {
                                                 .StoiChiometry = 1, .Identifier = String.Format("{0}[{1}]", Item.SensorId, strInducerId), .CompartmentId = "CCO-IN"}}}).ToArray

                Call ChemotaxisSensing.AddRange(LQuery)
                Call HKAutoPhosphorus.AddRange(collection:=(From HkAp In Me._CellSystemModel.HkAutoPhosphorus.AsParallel
                                                            Let Enzymes = (From id In HkAp.Inducers Select id.Split(CChar("=")).First).ToArray
                                                            Let MCPs = (From rxn In LQuery Select rxn.Products.Last.Identifier).ToArray
                                                            Select New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With
                                                                   {
                                                                       .Identifier = String.Format("{0}.AUTO-PHOSPHATE[{1}]", HkAp.SensorId, Item.SensorId),
                                                                       .Reversible = False, .UPPER_BOUND = 100, .p_Dynamics_K_1 = 1,
                                                                       .Enzymes = (From Id As String In MCPs Where (From iii In Enzymes Where InStr(Id, iii) > 0 Select iii).ToArray.Count > 0 Select Id).ToArray,
                                                                       .Reactants = {
                                                                           New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = "ATP"},
                                                                           New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = HkAp.SensorId}},
                                                                       .Products = {
                                                                           New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = "ADP"},
                                                                           New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = String.Format("[{0}][{1}]", HkAp.SensorId, "PI")}}
                                                                   }).ToArray)
            Next

            For Each item In ChemotaxisSensing
                Call SubstrateList.AddRange((From ref In item.Reactants Select ref.Identifier).ToArray)
                Call SubstrateList.AddRange((From ref In item.Products Select ref.Identifier).ToArray)
            Next
            For Each item In HKAutoPhosphorus
                Call SubstrateList.AddRange((From ref In item.Reactants Select ref.Identifier).ToArray)
                Call SubstrateList.AddRange((From ref In item.Products Select ref.Identifier).ToArray)
            Next

            SubstrateList = SubstrateList.Distinct.AsList

            Dim AppendedMetabolites = (From Id As String In SubstrateList
                                       Select Model.Metabolism.AppendNewMetabolite(Id, If(InStr(Id, "[") > 0 AndAlso InStr(Id, "]") > 0,
                                                                                         GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes,
                                                                                        GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Compound))).ToArray

            Dim TempChunk = (From item As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit
                             In Model.BacteriaGenome.TransUnits
                             Where Not item.RegulatedMotifs.IsNullOrEmpty
                             Select TFList = (From rn In (From regulator In item.RegulatedMotifs Select regulator.Regulators).ToArray.ToVector Select rn.Identifier).ToArray,
                                    TranscriptionModel = item).ToArray

            Call SubstrateList.Clear()

            Dim TFActive As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) =
                New List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction)

            Model.SignalTransductionPathway.OCSSensing = (From rxn In _CellSystemModel.OCSSensing Select rxn.CreateGCMLModel).ToArray

            Dim ii = (From RegulationModel In TempChunk Select Internal_Process(RegulationModel.TFList, SubstrateList, RegulationModel.TranscriptionModel, Model, TFActive)).ToArray

            Dim Index As String() = (From item In ChemotaxisSensing Select item.Identifier Distinct Order By Identifier).ToArray
            ChemotaxisSensing = (From strIndex In Index Select ChemotaxisSensing.GetItem(strIndex)).AsList
            Index = (From item In HKAutoPhosphorus Select item.Identifier Distinct Order By Identifier).ToArray
            HKAutoPhosphorus = (From strIndex As String In Index Select HKAutoPhosphorus.GetItem(strIndex)).AsList

            Model.SignalTransductionPathway.CheBMethylesterase = (From rxn In _CellSystemModel.CheBMethylesterase.AsParallel Select rxn.CreateGCMLModel).ToArray
            Model.SignalTransductionPathway.CheBPhosphate = (From rxn In _CellSystemModel.CheBPhosphate.AsParallel Select rxn.CreateGCMLModel).ToArray
            Model.SignalTransductionPathway.ChemotaxisSensing = ChemotaxisSensing.ToArray
            Model.SignalTransductionPathway.CheRMethyltransferase = (From rxn In _CellSystemModel.CheRMethyltransferase.AsParallel Select rxn.CreateGCMLModel).ToArray
            Model.SignalTransductionPathway.CrossTalk = (From rxn In _CellSystemModel.CrossTalk.AsParallel Select rxn.CreateGCMLModel).ToArray
            Model.SignalTransductionPathway.HkAutoPhosphorus = HKAutoPhosphorus.ToArray
            Model.SignalTransductionPathway.OCSSensing = (From rxn In _CellSystemModel.OCSSensing.AsParallel Select rxn.CreateGCMLModel).ToArray

            SubstrateList = SubstrateList.Distinct.AsList
            AppendedMetabolites = (From Id As String In SubstrateList Select Model.Metabolism.AppendNewMetabolite(Id, If(InStr(Id, "[") > 0 AndAlso InStr(Id, "]") > 0,
                                                                                     GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.ProteinComplexes,
                                                                                          GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Compound))).ToArray

            Call _Logging.WriteLine("Finalize the signal transduction network initialization!")

            Index = (From item In TFActive.AsParallel Where Not item Is Nothing Select item.Identifier Distinct Order By Identifier).ToArray
            TFActive = (From item In TFActive.AsParallel Where Not item Is Nothing Select item).AsList
            Model.SignalTransductionPathway.TFActive = (From strIndex As String In Index.AsParallel Select TFActive.GetItem(strIndex)).ToArray

            Call _Logging.WriteLine("End intialize the signal transduction network!")

            Return Model.SignalTransductionPathway
        End Function

        Private Function Internal_Process(TFList As String(),
                                          SubstrateList As List(Of String),
                                          TranscriptionModel As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit,
                                          Model As GCMarkupLanguage.BacterialModel,
                                          TFActive As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction)) As Integer

            For Each Tf In _CellSystemModel.STrPModel.Pathway

                If Array.IndexOf(TFList, Tf.TF) = -1 Then
                    Continue For
                End If

                If Not Tf.NotAcceptStrPSignal Then '如果不接受任何信号，则可以跳过

                    If Tf.TF_MiST2Type = Pathway.TFSignalTypes.TwoComponentType Then  '连接双组分信号系统值调控模型  将磷酸化的TF和调控关系进行对接

                        Dim Regulator = (From item In TranscriptionModel.get_Regulators Where String.Equals(item.Identifier, Tf.TF) Select item).First.Clone
                        Regulator.Identifier = String.Format("[{0}][PI]", Tf.TF)  '在这里讲信号转导网络与调控模型之间建立连接
                        Regulator.ProteinAssembly = Regulator.Identifier
                        Call SubstrateList.Add(Regulator.ProteinAssembly)
                        Call TranscriptionModel._add_Regulator("", Regulator)
                        Call _Logging.WriteLine(String.Format("Link TCS {0} with regulation model {1}", Tf.TF, TranscriptionModel.ToString), "", Type:=MSG_TYPES.INF)
                    End If

                    If Not Tf.TCSSystem.IsNullOrEmpty Then '磷酸化之后的RR与TF形成复合物
                        Dim Complexes = (From RR In Tf.TCSSystem Select String.Format("[{0}][PI]", RR.RR)).ToArray

                        For Each PC In Complexes
                            Dim Regulator = (From item In TranscriptionModel.get_Regulators Where String.Equals(item.Identifier, Tf.TF) Select item).First
                            Regulator = Regulator.Clone
                            Regulator.Identifier = String.Format("{0}[{1}]", PC, Tf.TF)  '在这里讲信号转导网络与调控模型之间建立连接
                            Regulator.ProteinAssembly = Regulator.Identifier
                            Call SubstrateList.Add(Regulator.ProteinAssembly)
                            Call TranscriptionModel._add_Regulator("", Regulator)

                            Dim FLUX = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With
                                       {
                                           .Identifier = String.Format("{0}=>{1}", PC, Tf.TF),
                                           .p_Dynamics_K_1 = 1, .p_Dynamics_K_2 = 1,
                                           .Reversible = True,
                                           .UPPER_BOUND = 100, .LOWER_BOUND = 100,
                                           .Reactants =
                                           {
                                               New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = PC},
                                               New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = Tf.TF}},
 _
                                           .Products =
                                           {
                                               New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = Regulator.ProteinAssembly}}
                                       }
                            Call TFActive.Add(FLUX)
                            Call _Logging.WriteLine(String.Format("Link TCS {0} with regulation model {1}", Tf.TF, TranscriptionModel.ToString), "", Type:=MSG_TYPES.INF)
                        Next

                    End If

                    If Not Tf.OCS.IsNullOrEmpty Then  '激活后的OCS与TF形成复合物

                        Dim ChunkTemp = (From OCS In Tf.OCS
                                         Select PC = (From item In Model.SignalTransductionPathway.OCSSensing Let PC = item.Products.First.Identifier Where InStr(PC, OCS.Key) > 0 Select PC).ToArray,
                                                Conf = OCS.Value).ToArray
                        Dim Complexes As List(Of KeyValuePair(Of Double, String)) = New List(Of KeyValuePair(Of Double, String))

                        For Each Line In ChunkTemp
                            Call Complexes.AddRange((From item In Line.PC Select New KeyValuePair(Of Double, String)(Line.Conf, item)).ToArray)
                        Next

                        Complexes = Complexes.Distinct.AsList

                        Dim TU_Regulators = TranscriptionModel.get_Regulators

                        For Each PC In Complexes
                            Dim Regulator = (From item In TU_Regulators Where String.Equals(item.Identifier, Tf.TF) Select item).First.Clone
                            Regulator.Identifier = String.Format("{0}[{1}]", PC.Value, Tf.TF)  '在这里讲信号转导网络与调控模型之间建立连接
                            Regulator.ProteinAssembly = Regulator.Identifier
                            Call SubstrateList.Add(Regulator.ProteinAssembly)
                            Call TranscriptionModel._add_Regulator("", Regulator)
                            Call TFActive.Add(New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With {
                                              .Identifier = String.Format("{0}=>{1}", PC.Value, Tf.TF),
                                              .p_Dynamics_K_1 = PC.Key, .p_Dynamics_K_2 = PC.Key, .Reversible = True, .UPPER_BOUND = 100, .LOWER_BOUND = 100,
                                              .Reactants = {New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = PC.Value},
                                                                                                                             New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = Tf.TF}},
                                              .Products = {New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.StoiChiometry = 1, .Identifier = Regulator.ProteinAssembly}}})
                            '  Call _Logging.WriteLine(String.Format("Link TCS {0} with regulation model {1}", TF.TF, RegulationModel.ToString), "", Type:=Logging.MSG_TYPES.INF, WriteToScreen:=False)
                        Next
                    End If

                    '由于TF是需要接受信号来激活的，在这里讲原来旧的的调控关系删除
                    Dim OldR_LQuery = (From item In TranscriptionModel.get_Regulators Where Not String.IsNullOrEmpty(item.Effector) AndAlso String.Equals(item.Identifier, Tf.TF) Select item).ToArray
                    If Not OldR_LQuery.IsNullOrEmpty Then Call TranscriptionModel.RemoveRegulator(OldR_LQuery.First)
                End If
            Next

            Return 0
        End Function

        ''' <summary>
        ''' 从加载的模型资源数据之中获取motif信息和调控因子的信息
        ''' </summary>
        ''' <param name="TranscriptUnit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function get_Motifs(TranscriptUnit As FileStream.TranscriptUnit) As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.MotifSite)

            Dim MotifList = (From item
                             In _CellSystemModel.Motifs
                             Where String.Equals(TranscriptUnit.TU_GUID, item.TU_MODEL_GUID)
                             Select item).ToArray '得到了motif数据
            Dim motifRegulations = (From motif In MotifList
                                    Let regulators = (From item In _CellSystemModel.Regulators
                                                      Where String.Equals(item.RegulatesMotif, motif.Internal_GUID, StringComparison.OrdinalIgnoreCase)
                                                      Select item).ToArray
                                    Select motif, regulators).ToArray     '再根据motif信息得到motif的调控因子的信息

            Dim LQuery = (From regulation In motifRegulations
                          Let regulators = (From item In regulation.regulators
                                            Let weight = New Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairObject(Of String, Double) With {.Key = regulation.motif.TU_MODEL_GUID, .Value = item.Pcc}
                                            Let __createRegulators = Function() As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator)
                                                                         Dim PCs = item.get_PCs
                                                                         Dim motifId As String = item.RegulatesMotif

                                                                         If PCs.IsNullOrEmpty Then  '不需要效应物，则没有蛋白质复合物
                                                                             Return {
                                                                                 New GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator With
                                                                                 {
                                                                                     .CommonName = item.ProteinId,
                                                                                     .PhysiologicallyRelevant = True,
                                                                                     .ProteinAssembly = "",
                                                                                     .Activation = item.Pcc > 0,
                                                                                     .Types = New String() {"Regulation-of-Translation"},
                                                                                     .K_Dynamics = 1,
                                                                                     .Identifier = item.ProteinId,
                                                                                     .Weight = weight,
                                                                                     .Regulates = motifId}
                                                                             }.AsList
                                                                         End If

                                                                         Return (From pc_id In PCs Select New GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator With
                                                                                                          {
                                                                                                              .Effector = pc_id.Value.Last,
                                                                                                              .CommonName = item.ProteinId,
                                                                                                              .PhysiologicallyRelevant = True,
                                                                                                              .ProteinAssembly = pc_id.Key,
                                                                                                              .Activation = item.Pcc > 0,
                                                                                                              .Types = New String() {"Regulation-of-Translation"},
                                                                                                              .K_Dynamics = 1,
                                                                                                              .Identifier = pc_id.Key,
                                                                                                              .Weight = weight,
                                                                                                              .Regulates = motifId}).AsList
                                                                     End Function()
                                            Select __createRegulators).ToArray.Unlist
                          Select New GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.MotifSite With
                                 {
                                     .MotifName = regulation.motif.Internal_GUID, .SitePosition = regulation.motif.Position, .Regulators = regulators}).AsList     '生成motif数据并返回
            Return LQuery
        End Function

        Private Function CreateTranscriptUnits(GeneObjects As Generic.IEnumerable(Of GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.GeneObject)) As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit)
            Call Me._Logging.WriteLine("Start to create the bacteria genome transcription unit models....")

            Dim TransUnits = (From TranscriptUnit As FileStream.TranscriptUnit
                              In _CellSystemModel.TranscriptionModel.AsParallel
                              Let motifs = get_Motifs(TranscriptUnit)
                              Let TU_Model = InternalCreate_TU_MODEL(motifs, GeneObjects, TranscriptUnit)
                              Select TU_Model).AsList
            Return TransUnits
        End Function

        Private Shared Function InternalCreate_TU_MODEL(motifs As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.MotifSite),
                                                        GeneObjects As Generic.IEnumerable(Of GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.GeneObject),
                                                        TranscriptUnit As FileStream.TranscriptUnit) _
            As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit

            Dim Model As TranscriptUnit = New TranscriptUnit
            Model.RegulatedMotifs = motifs
            Model.Identifier = TranscriptUnit.TU_GUID
            Model.Name = TranscriptUnit.TU_GUID
            Model.BasalLevel = 1
            Model.PromoterGene = (From item In GeneObjects Where String.Equals(TranscriptUnit.PromoterGene, item.AccessionId) Select item).First
            Model.GeneCluster = (From item In GeneObjects
                                 Where Array.IndexOf(TranscriptUnit.OperonGenes, item.AccessionId) > -1
                                 Select New KeyValuePair With
                                        {
                                            .Value = item.CommonName, .Key = item.AccessionId}).ToArray
            Return Model
        End Function

        Public Overrides Function ToString() As String
            Return Me._CellSystemModel.CellSystemModel.ToString
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
