#Region "Microsoft.VisualBasic::45b734056fcc42b0f7208c4524890d88, ..\GCModeller\engine\GCModeller\EngineSystem\ObjectModels\SubSystem\ExpressionSystem\BasalExpressionKeeper.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.SubSystem.ExpressionSystem

    ''' <summary>
    ''' 对于<see cref="SubSystem.ExpressionSystem.ExpressionRegulationNetwork"></see>模块之中的对象而言，其仅计算具备
    ''' 调控作用的基因的表达情况，故而需要使用这个对象来计算本地表达水平和看家基因
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BasalExpressionKeeper : Inherits SubSystemContainer(Of [Module].CentralDogmaInstance.BasalExpression)
        Implements PlugIns.ISystemFrameworkEntry.ISystemFramework

        ''' <summary>
        ''' 对整个基因组都有效
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Property BasalExpressionFluxes As ObjectModels.Module.CentralDogmaInstance.BasalExpression()
        <DumpNode> Public Property BasalTranslationFluxs As ObjectModels.Module.CentralDogmaInstance.Translation()

        Sub New(ExpressionNetwork As ExpressionRegulationNetwork)
            Call MyBase.New(ExpressionNetwork._CellSystem)
            _CellSystem = ExpressionNetwork._CellSystem
        End Sub

        Public Overrides Function CreateServiceSerials() As Services.MySQL.IDataAcquisitionService()
            Dim ServiceList As List(Of Services.MySQL.IDataAcquisitionService) = New List(Of Services.MySQL.IDataAcquisitionService)
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.BasalTranscription(Me), IRuntimeContainer))
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.BasalTranslationFlux(Me), IRuntimeContainer))
            Return ServiceList.ToArray
        End Function

        Public Overrides Function Initialize() As Integer
            Dim LevelmRNA As Double = Val(IRuntimeContainer.SystemVariable(var:=GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.PARA_MRNA_BASAL_LEVEL))
            Dim LeveltRNA As Double = Val(IRuntimeContainer.SystemVariable(var:=GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.PARA_TRNA_BASAL_LEVEL))
            Dim LevelrRNA As Double = Val(IRuntimeContainer.SystemVariable(var:=GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.PARA_RRNA_BASAL_LEVEL))

            Dim GetBasalValue As Dictionary(Of GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.TranscriptTypes, Double) =
                New Dictionary(Of GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.TranscriptTypes, Double) From
                {
                    {GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.TranscriptTypes.mRNA, LevelmRNA},
                    {GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.TranscriptTypes.rRNA, LevelrRNA},
                    {GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.TranscriptTypes.tRNA, LeveltRNA}}

            Dim Transcripts = _CellSystem.ExpressionRegulationNetwork._InternalTranscriptsPool

            Call SystemLogging.WriteLine("Creating the basal expression fluxes...")
            BasalExpressionFluxes = (
                From Transcript As Entity.Transcript
                In Transcripts.AsParallel
                Select New ObjectModels.Module.CentralDogmaInstance.BasalExpression With {
                    .Product = Transcript, .Template = New Feature.Gene With {.Identifier = Transcript.ModelId},
                    .CompositionVector = Transcript._TranscriptModelBase.CompositionVector.T,
                    .CompositionDelayEffect = Global.System.Math.Log10(Transcript._TranscriptModelBase.CompositionVector.T.Sum + 10),
                    .BasalLevel = GetBasalValue(Transcript._TranscriptModelBase.TranscriptType),
                    .ExpressedTranscript = Transcript}).ToArray

            BasalTranslationFluxs = (From Transcript As Entity.Transcript
                                     In Transcripts
                                     Where Not Transcript._TranscriptModelBase.PolypeptideCompositionVector.T.IsNullOrEmpty
                                     Select New ObjectModels.Module.CentralDogmaInstance.Translation With
                                            {
                                                .Identifier = Transcript.Identifier,
                                                .Template = Transcript,
                                                .Product = Transcript.ProductMetabolite,
                                                .CompositionVector = Transcript._TranscriptModelBase.PolypeptideCompositionVector.T}).ToArray

            Call SystemLogging.WriteLine("Creating action complete!")
            Call SystemLogging.WriteLine("Apply the metabolite constraints to the created expression flux object instance....")
            Call SystemLogging.WriteLine("   ---> Apply constraints to the basal expression events", "", Type:=Logging.MSG_TYPES.INF)

            Dim InitializeConstraintsLQuery = (From FluxObject In BasalExpressionFluxes.AsParallel Select Initializer.SetupConstraint(FluxObject, _CellSystem.Metabolism)).ToArray
            Call SystemLogging.WriteLine("   ---> Apply constraints to the translation events", "", Type:=Logging.MSG_TYPES.INF)

            InitializeConstraintsLQuery = (From FluxObject In BasalTranslationFluxs.AsParallel Select FluxObject.InitializeConstraints(_CellSystem.Metabolism)).ToArray

            Call _CellSystem._InternalEventDriver.JoinEvents(BasalExpressionFluxes)
            Call _CellSystem._InternalEventDriver.JoinEvents(BasalTranslationFluxs)

            Return 0
        End Function

        Public Overrides Sub MemoryDump(Dir As String)

        End Sub

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
            Return 0
        End Function
    End Class
End Namespace
