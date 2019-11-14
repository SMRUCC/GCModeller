#Region "Microsoft.VisualBasic::4b64a82e57371de55948d3c82413b638, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\ExpressionSystem\Initializer.vb"

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

    '     Class Initializer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Invoke, IsProduct, SetupConstraint, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.GCModeller.Assembly

Namespace EngineSystem.ObjectModels.SubSystem.ExpressionSystem

    Public Class Initializer

        ReadOnly _ExpressionRegulationNetwork As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Me._ExpressionRegulationNetwork = System
        End Sub

        Public Function Invoke() As Integer
            Dim CellSystem As SubSystem.CellSystem = _ExpressionRegulationNetwork._CellSystem
            Dim DataModel As GCMarkupLanguage.BacterialModel = CellSystem.DataModel
            If DataModel.BacteriaGenome.TransUnits.IsNullOrEmpty Then
                Call SubSystem.ExpressionSystem.ExpressionRegulationNetwork.set_NetworkComponents(New [Module].CentralDogmaInstance.CentralDogma() {}, _ExpressionRegulationNetwork)
                Return -1
            End If

            'Me._ExpressionRegulationNetwork.RNAPolymerase = (From Id As String
            '                                              In Strings.Split(CellSystem.get_runtimeContainer.SystemVariable(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_RNA_POLYMERASE_PROTEIN), "; ")
            '                                                 Select New Feature.MetabolismEnzyme With {
            '                                                  .EnzymeMetabolite = CellSystem.Metabolism.Metabolites.GetItem(Id),
            '                                                  .Identifier = Id,
            '                                                  .EnzymeKineticLaw = MathematicsModels.EnzymeKinetics.EnzymeCatalystKineticLaw.[New](CellSystem.DataModel.BacteriaGenome.GetExpressionKineticsLaw(Id))}).ToArray
            '_ExpressionRegulationNetwork.RibosomeAssemblyCompound = New Feature.MetabolismEnzyme With {
            '    .EnzymeMetabolite = CellSystem.Metabolism.Metabolites.GetItem(CellSystem.get_runtimeContainer.SystemVariable(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_RIBOSOME_ASSEMBLY_COMPLEXES)),
            '    .Identifier = CellSystem.get_runtimeContainer.SystemVariable(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_RIBOSOME_ASSEMBLY_COMPLEXES),
            '    .EnzymeKineticLaw = MathematicsModels.EnzymeKinetics.EnzymeCatalystKineticLaw.[New](
            '        CellSystem.DataModel.BacteriaGenome.GetExpressionKineticsLaw(CellSystem.get_runtimeContainer.SystemVariable(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_RIBOSOME_ASSEMBLY_COMPLEXES)))}

            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("Create transcripts object models......", "", MSG_TYPES.INF)
            _ExpressionRegulationNetwork._InternalTranscriptsPool = (From TranscriptModelBase As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript
                                                        In CellSystem.DataModel.BacteriaGenome.Transcripts.AsParallel
                                                                     Select SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Transcript.CreateInstance(
                                                            TranscriptModelBase, CellSystem.Metabolism.Metabolites)).ToArray '创建一个RNA池集合，然后进行分配

            Dim NetworkComponents = (From TU As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit
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

            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("Start to create the expression flux instance...", "", MSG_TYPES.INF)

            Dim stw = Stopwatch.StartNew


            Dim ExpressionObjectInitializeLQuery = (From ExpressionObject As [Module].CentralDogmaInstance.CentralDogma
                                                    In _ExpressionRegulationNetwork.NetworkComponents.AsParallel
                                                    Select ExpressionObject.Initialize(CellSystem)).ToArray
            ExpressionObjectInitializeLQuery = ExpressionObjectInitializeLQuery.WriteAddress.ToArray

            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine(String.Format("Initialization time using {0} ms", stw.ElapsedMilliseconds))
            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("End of expression regulation network initialize...", "SVD", MSG_TYPES.INF)
            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("Start to initalize the expression metabolite constraints...")

            _ExpressionRegulationNetwork.BasalExpression = New BasalExpressionKeeper(ExpressionNetwork:=_ExpressionRegulationNetwork)
            Call _ExpressionRegulationNetwork.BasalExpression.Initialize()

            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("Expression network initialize job completed!")
            Call _ExpressionRegulationNetwork.SystemLogging.WriteLine("---------------------------------------------------------------------------------------------------------------------" & vbCrLf)

            Return 0
        End Function

        Public Shared Function SetupConstraint(FluxObject As EngineSystem.ObjectModels.Module.CentralDogmaInstance.BasalExpression, MetabolismSystem As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment) As Integer
            FluxObject.UPPER_BOUND = If(Global.System.Math.Abs(FluxObject.UPPER_BOUND - 0.0R) < TOLERANCE, Rnd() * 10, FluxObject.UPPER_BOUND / 37)
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
