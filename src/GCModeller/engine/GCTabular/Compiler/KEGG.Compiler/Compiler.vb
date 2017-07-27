#Region "Microsoft.VisualBasic::f5e14349c0680311fd4eaf54cc533aca, ..\GCModeller\engine\GCTabular\Compiler\KEGG.Compiler\Compiler.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.Expasy.AnnotationsTool
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Model.Network.STRING.Models
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.Model.SBML

Namespace KEGG.Compiler

    <Package("GCModeller.KEGG.Compiler", Category:=APICategories.UtilityTools,
                        Description:="For the first time of the model compiles operation, a active network connection to the KEGG database server maybe required.")>
    Public Class Compiler : Inherits GCTabular.Compiler.Compiler

        Dim MetaCycAll As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
        Dim _RegpreciseRegulatorBh As RegpreciseMPBBH()
        Dim PccMatrix As PccMatrix

        ''' <summary>
        ''' 预先将整个KEGG数据库之中的数据读取进入内存之中
        ''' </summary>
        ''' <param name="argvs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Argument("-kegg.compounds", Description:="The data directory of the kegg compounds for your modelling species.")>
        <Argument("-kegg.reactions")>
        <Argument("-mist2")>
        <Argument("-export")>
        <Argument("-door")>
        <Argument("-footprints")>
        <Argument("-kegg.pathways", Description:="The data directory of the kegg pathways data for your modelling species")>
        <Argument("-kegg.modules")>
        <Argument("-metacyc_all")>
        <Argument("-chipdata")>
        Public Overrides Function PreCompile(argvs As CommandLine.CommandLine) As Integer
            Dim LogFile As String = argvs("-logging")

            If Not String.IsNullOrEmpty(LogFile) Then
                Me._Logging = New Logging.LogFile(LogFile, True) With {.SuppressError = False, .SuppressWarns = False, .ColorfulOutput = False}
            Else
                Me._Logging = New Logging.LogFile(String.Format("{0}/gcmodeller_kegg.compiler__{1}.log", Settings.LogDIR, Logging.LogFile.NowTimeNormalizedString), True) With {
                    .SuppressError = False,
                    .SuppressWarns = False,
                    .ColorfulOutput = False
                }
            End If

            Call CheckRequiredParameter(argvs, New String() {"-kegg.compounds", "-kegg.reactions", "-mist2", "-export", "-door", "-footprints", "-kegg.pathways", "-kegg.modules", "-metacyc_all"}, head:="KEGG.Compiler::PreCompile()")



            '+-------------------------------------------PreCompile-----------------------------------------------+

            Me._ModelIO = FileStream.IO.XmlresxLoader.CreateObject
            Me._ModelIO.MisT2 = argvs("-mist2").LoadXml(Of MiST2.MiST2)()
            Me._ModelIO.SetExportDirectory(argvs("-export"))
            Me._ModelIO.SystemVariables = SystemVariables.CreateDefault.AsList

            Dim Door = SMRUCC.genomics.Assembly.DOOR.Load(path:=argvs("-door"))
            Dim Footprints = argvs("-footprints").LoadCsv(Of RegulatesFootprints)(False)

            'Me.PccMatrix = SMRUCC.genomics.Toolkits.RNASeq.ChipData.LoadChipData(argvs("-chipdata")).CalculatePccMatrix
            Me._ModelIO.DoorOperon = (From Operon In Door.DOOROperonView.Operons Select Operon.ConvertToCsvData).ToArray
            Me._ModelIO.CellSystemModel.OperonCounts = _ModelIO.DoorOperon.Count
            Me._Door = Door

            Call GCTabular.RegulationNetworkFromFootprints.CompileFootprintsData(data:=Footprints,
                                                                                  PccMatrix:=PccMatrix,
                                                                                  OperonData:=Door.DOOROperonView,
                                                                                  TranscriptUnits:=Me._ModelIO.TranscriptionModel,
                                                                                  Motifs:=Me._ModelIO.Motifs,
                                                                                  Regulators:=Me._ModelIO.Regulators)
            Dim sPath As String = argvs("-kegg.compounds")
            Call _Logging.WriteLine("Start to load compounds data from filesystem, this may take a while....")
            Dim KEGGCompounds = (From path As String
                                 In FileIO.FileSystem.GetFiles(sPath, FileIO.SearchOption.SearchAllSubDirectories, "C*.xml").AsParallel
                                 Select path.LoadXml(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound)()).AsList
            Call KEGGCompounds.AddRange((From path As String
                                         In FileIO.FileSystem.GetFiles(sPath, FileIO.SearchOption.SearchAllSubDirectories, "G*.xml").AsParallel
                                         Select path.LoadXml(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Glycan)().ToCompound).ToArray)

            Me.KEGGCompounds = New KeyValuePair(Of String, SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound())(sPath, KEGGCompounds.ToArray)

            Call _Logging.WriteLine("[DONE!]")
            Call _Logging.WriteLine("Start to load reactions data from filesystem, this may take a while....")

            sPath = argvs("-kegg.reactions")
            KEGGReactions = New KeyValuePair(Of String, SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction())(
                sPath, (From File As String
                        In FileIO.FileSystem.GetFiles(sPath, FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel
                        Let Model = LoadReactionModel(File, Me._Logging)
                        Where Not Model Is Nothing
                        Select Model).ToArray)

            Call _Logging.WriteLine("[DONE!]")

            Me.KEGGPathways = (From File As String In FileIO.FileSystem.GetFiles(argvs("-kegg.pathways"), FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel Select File.LoadXml(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway)()).ToArray
            Me.KEGGModules = (From File As String In FileIO.FileSystem.GetFiles(argvs("-kegg.modules"), FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel Select File.LoadXml(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Module)()).ToArray
            Me.MetaCycAll = SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(argvs("-metacyc_all"), False)

            Return 0
        End Function

        <Argument("-carmen")>
        <Argument("-ec")>
        <Argument("-ptt", Description:="The ptt data directory.")>
        <Argument("-myva_cog", True, Description:="")>
        <Argument("-metacyc")>
        <Argument("-regprecise")>
        Public Overrides Function Compile(Optional argvs As CommandLine.CommandLine = Nothing) As FileStream.XmlFormat.CellSystemXmlModel
            Call Me.CheckRequiredParameter(argvs, New String() {"-carmen", "-ec", "-mist2_strp", "-ptt", "-cross_talks", "-myva_cog", "-string-db", "-regulator_bh", "-metacyc", "-regprecise", "-species_code"}, "KEGG.Compiler::Compile()")

            Call __Initialize_MetaCyc(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(argvs("-metacyc")))

            '代谢物在数据库之间的映射
            Me._ModelIO.MetabolitesModel = KEGG.Compiler.Compound.Compile(KEGGCompounds.Value).ToDictionary
            Me._ModelIO.MetabolismModel = KEGG.Compiler.Reactions.CompileCARMEN(KEGGReactions.Value, argvs("-carmen"),
                                                                                Me._ModelIO,
                                                                                KEGGReactions.Key,
                                                                                KEGGCompounds.Key,
                                                                                Me._Logging).AsList
            Me._ModelIO.EffectorMapping = Effectors.MappingEffectors(MetaCycAll, _ModelIO.MetabolitesModel.Values.AsList, argvs("-regprecise").LoadXml(Of TranscriptionFactors))
            Me._RegpreciseRegulatorBh = argvs("-regulator_bh").LoadCsv(Of RegpreciseMPBBH)(False).ToArray
            'Me._ModelIO.EffectorMapping = MappingKEGGCompoundsRegprecise(KEGGCompounds:=_ModelIO.MetabolitesModel.Values.ToArray, Regprecise:=_RegpreciseRegulatorBh)

            Me._ModelIO.StringInteractions = argvs("-string-db").LoadXml(Of SimpleCsv.Network)()
            Me._CrossTalks = IO.File.Load(argvs("-cross_talks"))
            Me._ModelIO.STrPModel = argvs("-mist2_strp").LoadXml(Of Network)()
            Me._MetabolismNetwork = Level2.XmlFile.Load(Me._MetaCyc.SBMLMetabolismModel)
            Me._ModelIO.ProteinAssembly = _createProteinAssembly(Me._ModelIO.Regulators, Me._ModelIO.MetabolitesModel)

            Call GCTabular.RegulationNetworkFromFootprints.MappingEffector(_ModelIO.Regulators, _ModelIO.EffectorMapping, _RegpreciseRegulatorBh)
            Call GCTabular.RegulationNetworkFromFootprints.TCS__RR(Me._ModelIO.Regulators, _ModelIO.MisT2)

            Dim MyvaCog = If(argvs Is Nothing OrElse String.IsNullOrEmpty(argvs("-myva_cog")),
                             New MyvaCOG() {},
                             argvs("-myva_cog").AsDataSource(Of MyvaCOG)(, False))
            Dim EC = argvs("-ec").LoadCsv(Of SMRUCC.genomics.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT)(False)

            Using MappingCreator = New Mapping(_MetaCyc, Me._ModelIO.MetabolitesModel.Values.ToArray)
                Me._ModelIO.EnzymeMapping = MappingCreator.CreateEnzrxnGeneMap.AsList
                Call _Logging.WriteLine(Me._ModelIO.EnzymeMapping.Count & " enzymatic reaction mapping was created!")
            End Using

            Call Me._ModelIO.MetabolismModel.AddRange(GCTabular.FileStream.MetabolismFlux.CreateObject(Me._MetabolismNetwork, MetabolismEnzymeLink:=Me._ModelIO.EnzymeMapping, MetaCycReactions:=_MetaCyc.GetReactions))
            Call GCTabular.Compiler.Components.Extract_SBML_GeneralSubstrates.Analysis(Me._ModelIO.MetabolitesModel, Me._MetaCyc, Me._ModelIO, Me._Logging)

            Call Me._ModelIO.MetabolismModel.AddRange(KEGG.Compiler.Reactions.CompileExpasy(EC,
                                                                                            KEGGReactions.Value,
                                                                                            Me._ModelIO, _MetaCyc,
                                                                                            Me._MetabolismNetwork,
                                                                                            KEGGCompounds.Key,
                                                                                            KEGGReactions.Key,
                                                                                            Me._Logging))
            Call Me.CompileGenome(argvs("-ptt"), MyvaCog, EC)

            Me._ModelIO.KEGG_Pathways =
                SMRUCC.genomics.Assembly.KEGG.Archives.Xml.Compile(
                    Me.KEGGPathways,
                    Me.KEGGModules,
                    Me.KEGGReactions.Value,
                    argvs("-species_code"))

            Call Link()

            Me.CompiledModel = Me._ModelIO.CellSystemModel

            Return Me.CompiledModel
        End Function

        ''' <summary>
        ''' 将没有涉及任何反应过程的代谢物移除,并将重复的代谢反应去除
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function Link() As Integer
            Call RemoveReactionDuplications()

            Me._ModelIO.TransmembraneTransportation = AnalysisTransmenbraneFlux()
            Me._ModelIO.Pathway = CompilePathwayInformations()

            Call RemoveNotUsedCompounds()
            Call LinkEffectors(Me._CrossTalks)

            Call _createEnzymeObjects()

            '通过MetaCyc编号来连接KEGGCompound
            Me._ModelIO.ConstraintMetabolites = ConstraintMetaboliteMap.CreateObjectsWithMetaCyc.AsList
            For i As Integer = 0 To Me._ModelIO.ConstraintMetabolites.Count - 1
                Dim CM = Me._ModelIO.ConstraintMetabolites(i)
                Dim Metabolites = (From item In Me._ModelIO.MetabolitesModel.AsParallel
                                   Where String.Equals(item.Value.MetaCycId, CM.ModelId) OrElse
                                       String.Equals(item.Value.Identifier, CM.ModelId)
                                   Select item).ToArray '首先通过MetaCyc编号来查找对象
                If Metabolites.IsNullOrEmpty Then
                    Call _Logging.WriteLine(String.Format("Could not found the link for the mapping: {0}, this may cause the error in the GCModeller initialize....!", CM.ToString), "Link()", Type:=Logging.MSG_TYPES.WRN)
                    Call _ModelIO.MetabolitesModel.Add(CM.ModelId, New FileStream.Metabolite With {.Identifier = CM.ModelId, .InitialAmount = 10, .CommonNames = {CM.ConstraintId}})
                    Call _Logging.WriteLine("TRY_FIX_LINK_ERROR::added " & CM.ToString)
                Else
                    CM.ModelId = Metabolites.First.Value.Identifier   '找到对象之后，将Map的UniqueID替换为找到的代谢物的UniqueID
                End If
            Next

            Return 0
        End Function

        Private Function MappingKEGGCompoundsRegprecise(KEGGCompounds As IEnumerable(Of FileStream.Metabolite),
                                                        Regprecise As RegpreciseMPBBH()) As List(Of MetaCyc.Schema.EffectorMap)

            Dim Compounds = FileStream.Metabolite.MappingComponentModel.GenerateCompoundMappingModel(KEGGCompounds)
            Return Mapping.EffectorMapping(Regprecise, Compounds)
        End Function

        Private Sub CompileGenome(PTT As String, MyvaCOG As MyvaCOG(), ECProfiles As IEnumerable(Of T_EnzymeClass_BLAST_OUT))
            Dim PttBrief As New PTTDbLoader(PTT)
            Me._ModelIO.GenomeAnnotiation = FileStream.GeneObject.CreateObject(PttBrief.Values, MyvaCOG)

            Call _createTranscripts(GeneSequence:=(From item In PttBrief.GeneFastas Select New KeyValuePair(Of String, String)(item.Key, item.Value.SequenceData.ToUpper)).ToArray,
                                    ProteinSequence:=(From item In PttBrief.Proteins Select New KeyValuePair(Of String, String)(item.Key, item.Value.SequenceData.ToUpper)).ToArray)
            Call MyBase._createRibosomeAssembly(PTT)
            Call MyBase.__createProteinObjects(ECProfiles)
        End Sub

        Private Sub RemoveReactionDuplications()
            Dim ReactionIdList As String() = (From item In Me._ModelIO.MetabolismModel Select item.Identifier Distinct).ToArray
            Dim ReactionList As List(Of FileStream.MetabolismFlux) = New List(Of FileStream.MetabolismFlux)

            For Each strId As String In ReactionIdList
                Dim LQuery = (From item In Me._ModelIO.MetabolismModel.AsParallel Where String.Equals(item.Identifier, strId) Select item).ToArray

                If LQuery.Count = 1 Then
                    Call ReactionList.Add(LQuery.First)
                    Continue For
                End If

                Dim EnzymeList As List(Of String) = New List(Of String)
                For Each item In LQuery

                    If Not item.Enzymes.IsNullOrEmpty Then
                        Call EnzymeList.AddRange(item.Enzymes)
                    End If

                Next

                Dim Reaction = LQuery.First
                Reaction.Enzymes = EnzymeList.Distinct.ToArray

                Call ReactionList.Add(Reaction)
            Next

            Me._ModelIO.MetabolismModel = (From item In ReactionList Select item Order By item.EnzymeClass Ascending).AsList
        End Sub

        Private Sub RemoveNotUsedCompounds()
            Call GCTabular.Compiler.Components.CheckConsistent(_ModelIO, _Logging)
            Call GCTabular.Compiler.Components.AssociatedFluxAnalysis.ApplyAnalysis(Me._ModelIO)

            'Dim RemovedList = (From item
            '                   In Me._ModelIO.MetabolitesModel.Values
            '                   Where item.MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Compound AndAlso item.n_FluxAssociated = 0
            '                   Select item).ToArray '移除操作仅对Compound有效，其他类型的生物分子不做考虑
            'For Each item In RemovedList
            '    Call Me._ModelIO.MetabolitesModel.Remove(item.UniqueId)
            'Next

            'Call Me._Logging.WriteLine(String.Format("{0} metabolites was removed due to it is never used in the compiled model.", RemovedList.Count))
        End Sub

        ''' <summary>
        ''' 由于可能需要下载一些残缺的数据，故而KEY的作用是指明原始文件夹以存放所下载的数据
        ''' </summary>
        ''' <remarks></remarks>
        Dim KEGGCompounds As KeyValuePair(Of String, bGetObject.Compound())
        ''' <summary>
        ''' 由于可能需要下载一些残缺的数据，故而KEY的作用是指明原始文件夹以存放所下载的数据
        ''' </summary>
        ''' <remarks></remarks>
        Dim KEGGReactions As KeyValuePair(Of String, bGetObject.Reaction())
        Dim KEGGPathways As bGetObject.Pathway()
        Dim KEGGModules As bGetObject.Module()

        Public Sub New()
            If Not Settings.Initialized Then Settings.Initialize()
        End Sub

        Private Function CompilePathwayInformations() As List(Of FileStream.Pathway)
            Dim ModulesInformation = (From [mod] In KEGGModules
                                      Where Not [mod].Reaction.IsNullOrEmpty
                                      Select ModuleId = [mod].EntryId,
                                          ReactionIdlist = (From item In [mod].Reaction Select item.Key).ToArray).ToArray
            Dim PathwayInformation = (From pwy In KEGGPathways Where Not pwy.Modules.IsNullOrEmpty
                                      Select New With {
                                          .PathwayId = pwy.EntryId,
                                          .ModuleList = (From item In pwy.Modules Select item.Key).ToArray,
                                          .Comments = pwy.Description}).ToArray

            Dim ModuleDictionary As Dictionary(Of String, String()) = New Dictionary(Of String, String()) 'ModuleId, ReactionIdlist

            For Each item In ModulesInformation
                Call ModuleDictionary.Add(item.ModuleId, item.ReactionIdlist)
            Next

            Dim LQuery = (From Pathway In PathwayInformation Select __pathway(Pathway.PathwayId, Pathway.Comments, Pathway.ModuleList, ModuleDictionary)).AsList
            Return LQuery
        End Function

        Private Shared Function __pathway(pathwayId As String, comments As String, modList As String(), ModuleDictionary As Dictionary(Of String, String())) As FileStream.Pathway
            Dim PathwayModel = New FileStream.Pathway With {.Identifier = pathwayId, .Comment = comments}
            Dim List As List(Of String) = New List(Of String)
            For Each ModuleId As String In modList
                If Not ModuleDictionary.ContainsKey(ModuleId) Then
                    Continue For
                End If

                Call List.AddRange(ModuleDictionary(ModuleId))
            Next

            PathwayModel.MetabolismFlux = List.ToArray

            Return PathwayModel
        End Function

        Private Function CheckRequiredParameter(argvs As CommandLine.CommandLine, list As String(), head As String) As Boolean
            Dim required As String() = argvs.CheckMissingRequiredArguments(list)

            Call _Logging.WriteLine(argvs.GetCommandsOverview)

            If Not required.IsNullOrEmpty Then
                Call _Logging.WriteLine(String.Format("These required parameter ""{0}"" is missing!", String.Join(",", required)), head, Logging.MSG_TYPES.WRN, True)
                Return False
            Else
                Return True
            End If
        End Function

        Private Shared Function LoadReactionModel(path As String, LogFile As Logging.LogFile) As bGetObject.Reaction
            Dim XmlModel = path.LoadXml(Of bGetObject.Reaction)()
            If XmlModel Is Nothing Then
                Call LogFile.WriteLine(String.Format("[DEBUG::XML_MODEL_IS_NULL] {0}", path), "CARMEN->LoadReactionModel()") '似乎CARMEN软件所使用的KEGG数据库的版本和KEGG数据库网站上的版本不一致，故而会导致这个错误，在这里只能尽量忽略这种错误
            End If
            Return XmlModel
        End Function

#Region "ShellScript API"

        <ExportAPI("write.xml.csv_model", Info:="saveto parameter is the xml file name of the target compiled model.")>
        Public Shared Function SaveModel(op As Compiler, saveto As String) As Boolean
            Return op._ModelIO.SaveTo(saveto)
        End Function

        <ExportAPI("precompile")>
        <Argument("-kegg.compounds", Description:="The data directory of the kegg compounds for your modelling species.")>
        <Argument("-kegg.reactions")>
        <Argument("-mist2")>
        <Argument("-export")>
        <Argument("-door")>
        <Argument("-kegg.pathways", Description:="The data directory of the kegg pathways data for your modelling species")>
        <Argument("-kegg.modules")>
        <Argument("-metacyc_all")>
        <Argument("-chipdata")>
        <Argument("-footprints", Description:="The predicted footprint regulation data for the target bacteria genome.")>
        Public Overloads Shared Function PreCompile([operator] As Compiler, argvs As CommandLine.CommandLine) As Integer
            Try
                Return [operator].PreCompile(argvs)
            Catch ex As Exception
                Call [operator].WriteLog()
                Throw
            End Try
        End Function

        <ExportAPI("compile")>
        <Argument("-carmen")>
        <Argument("-ec")>
        <Argument("-ptt", Description:="The ptt data directory.")>
        <Argument("-myva_cog", True, Description:="")>
        <Argument("-metacyc")>
        <Argument("-regprecise")>
        Public Overloads Shared Function Compile([operator] As Compiler, argvs As CommandLine.CommandLine) As FileStream.XmlFormat.CellSystemXmlModel
            Try
                Return [operator].Compile(argvs)
            Catch ex As Exception
                Call [operator].WriteLog()
                Throw
            End Try
        End Function

        <ExportAPI("session.new")>
        Public Shared Function NewSession() As Compiler
            Return New Compiler
        End Function

        <ExportAPI("write.xml.csv_model_io")>
        Public Shared Function SaveModel(model As FileStream.IO.XmlresxLoader, saveto As String) As Boolean
            Return model.SaveTo(saveto)
        End Function

        <ExportAPI("get.model_stream")>
        Public Shared Function GetModelStream([operator] As Compiler) As FileStream.IO.XmlresxLoader
            Return [operator]._ModelIO
        End Function

        <ExportAPI("get.logs")>
        Public Shared Function get_LogsData(compiler As Compiler) As Microsoft.VisualBasic.Logging.LogFile
            Return compiler._Logging
        End Function
#End Region
    End Class
End Namespace
