Imports Microsoft.VisualBasic.Extensions
 

Namespace EngineSystem.ObjectModels.SubSystem.ExpressionSystem

    Public Class Initializer

        ReadOnly _ExpressionRegulationNetwork As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Me._ExpressionRegulationNetwork = System
        End Sub

        Public Function Invoke() As Integer
            Dim CellSystem As SubSystem.CellSystem = _ExpressionRegulationNetwork._CellSystem
            Dim DataModel As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel = CellSystem.DataModel
            If DataModel.BacteriaGenome.TransUnits.IsNullOrEmpty Then
                Call SubSystem.ExpressionSystem.ExpressionRegulationNetwork.set_networkComponents(New [Module].CentralDogmaInstance.CentralDogma() {}, _ExpressionRegulationNetwork)
                Return -1
            End If

            Me._ExpressionRegulationNetwork.RNAPolymerase = (From Id As String
                                                          In Strings.Split(CellSystem.get_runtimeContainer.SystemVariable(LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_RNA_POLYMERASE_PROTEIN), "; ")
                                                             Select New Feature.MetabolismEnzyme With {
                                                              .EnzymeMetabolite = CellSystem.Metabolism.Metabolites.GetItem(Id),
                                                              .Identifier = Id,
                                                              .EnzymeKineticLaw = MathematicsModels.EnzymeKinetics.EnzymeCatalystKineticLaw.[New](CellSystem.DataModel.BacteriaGenome.GetExpressionKineticsLaw(Id))}).ToArray
            _ExpressionRegulationNetwork.RibosomeAssemblyCompound = New Feature.MetabolismEnzyme With {
                .EnzymeMetabolite = CellSystem.Metabolism.Metabolites.GetItem(CellSystem.get_runtimeContainer.SystemVariable(LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_RIBOSOME_ASSEMBLY_COMPLEXES)),
                .Identifier = CellSystem.get_runtimeContainer.SystemVariable(LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_RIBOSOME_ASSEMBLY_COMPLEXES),
                .EnzymeKineticLaw = MathematicsModels.EnzymeKinetics.EnzymeCatalystKineticLaw.[New](
                    CellSystem.DataModel.BacteriaGenome.GetExpressionKineticsLaw(CellSystem.get_runtimeContainer.SystemVariable(LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_RIBOSOME_ASSEMBLY_COMPLEXES)))}

            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("Create transcripts object models......", "", Logging.MSG_TYPES.INF)
            _ExpressionRegulationNetwork._InternalTranscriptsPool = (From TranscriptModelBase As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript
                                                        In CellSystem.DataModel.BacteriaGenome.Transcripts.AsParallel
                                                                     Select LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Transcript.CreateInstance(
                                                            TranscriptModelBase, CellSystem.Metabolism.Metabolites)).ToArray '创建一个RNA池集合，然后进行分配

            Dim NetworkComponents = (From TU As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit
                                                               In DataModel.BacteriaGenome.TransUnits.AsParallel
                                     Select [Module].CentralDogmaInstance.CentralDogma.CreateInstance(TransUnit:=TU, CellSystem:=CellSystem)).ToArray '初始化了TranscriptUnit表达调控过程对象，但是还没有初始化调控因子

            Call SubSystem.ExpressionSystem.ExpressionRegulationNetwork.set_NetworkComponents(NetworkComponents, _ExpressionRegulationNetwork)

            Dim CreateTranscriptsLQuery = (From ExpressionObject In _ExpressionRegulationNetwork.NetworkComponents.AsParallel
                                           Let Action = Function() As [Module].CentralDogmaInstance.CentralDogma
                                                            Dim LQueryTrans = (From Trans In _ExpressionRegulationNetwork._InternalTranscriptsPool.AsParallel
                                                                               Where IsProduct(ExpressionObject.TransUnit.ProductHandlers, Trans.Identifier)
                                                                               Select Trans Order By Trans.Handle Ascending).ToArray
                                                            ExpressionObject.Transcripts = LQueryTrans
                                                            Return ExpressionObject
                                                        End Function Select Action()).ToArray

            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("Start to create the expression flux instance...", "", Logging.MSG_TYPES.INF)

            Dim stw = Stopwatch.StartNew


            Dim ExpressionObjectInitializeLQuery = (From ExpressionObject As [Module].CentralDogmaInstance.CentralDogma
                                                    In _ExpressionRegulationNetwork.NetworkComponents.AsParallel
                                                    Select ExpressionObject.Initialize(CellSystem)).ToArray
            ExpressionObjectInitializeLQuery = ExpressionObjectInitializeLQuery.AddHandle.ToArray

            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine(String.Format("Initialization time using {0} ms", stw.ElapsedMilliseconds))
            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("End of expression regulation network initialize...", "SVD", Logging.MSG_TYPES.INF)
            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("Start to initalize the expression metabolite constraints...")

            _ExpressionRegulationNetwork.BasalExpression = New BasalExpressionKeeper(ExpressionNetwork:=_ExpressionRegulationNetwork)
            Call _ExpressionRegulationNetwork.BasalExpression.Initialize()

            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("Expression network initialize job completed!")
            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("---------------------------------------------------------------------------------------------------------------------" & vbCrLf)

            Return 0
        End Function

        Public Shared Function SetupConstraint(FluxObject As EngineSystem.ObjectModels.Module.CentralDogmaInstance.BasalExpression, MetabolismSystem As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment) As Integer
            FluxObject.UPPER_BOUND = If(Global.System.Math.Abs(FluxObject.UPPER_BOUND - 0.0R) < TOLERANCE, RandomDouble() * 10, FluxObject.UPPER_BOUND / 37)
            Call FluxObject.InitializeConstraints(MetabolismSystem)
            '     Call Randomize()
            Return 0
        End Function

        Private Shared Function IsProduct(Products As Generic.KeyValuePair(Of String, String)(), UniqueId As String) As Boolean
            For Each item In Products
                If String.Equals(item.Value, UniqueId) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Overrides Function ToString() As String
            Return _ExpressionRegulationNetwork.ToString
        End Function
    End Class
End Namespace