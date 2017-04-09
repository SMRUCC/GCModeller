#Region "Microsoft.VisualBasic::725187b8b14403aac00951e3557ae951, ..\GCModeller\engine\GCModeller\EngineSystem\ObjectModels\SubSystem\CellSystem\CellSystem.vb"

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
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL

Namespace EngineSystem.ObjectModels.SubSystem

    Public NotInheritable Class CellSystem : Inherits SystemObjectModel
        Implements IDrivenable
        Implements Global.System.IDisposable

        Protected _GCML_DataModel As BacterialModel

        ''' <summary>
        ''' 本细胞对象的数据模型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend ReadOnly Property DataModel As BacterialModel
            Get
                Return Me._GCML_DataModel
            End Get
        End Property

        Protected Friend Overrides ReadOnly Property SystemLogging As Logging.LogFile
            Get
                Return I_RuntimeContainer.SystemLogging
            End Get
        End Property

        <DumpNode> Public Property Metabolism As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment
        <DumpNode> Public Property ExpressionRegulationNetwork As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork
        <DumpNode> Public Property SignalTransductionNetwork As EngineSystem.ObjectModels.SubSystem.SignalTransductionNetwork

        Protected Friend _InternalEventDriver As EngineSystem.ObjectModels.I_SystemEventDriver

        Sub New(Modeller As Engine.GCModeller)
            _GCML_DataModel = Modeller.get_DataModel
            I_RuntimeContainer = Modeller
        End Sub

#Region "System Activity Load"

        <DumpNode> Protected Friend ReadOnly Property get_TranscriptionActivity() As Double
            Get
                Dim sum = (From item In Me.ExpressionRegulationNetwork._InternalEvent_Transcriptions Select Global.System.Math.Abs(item.FluxValue)).Sum
                Dim value As String = Global.System.Math.Log(sum, 2).ToString
                Return Val(value)
                'Return Val(sum)
            End Get
        End Property

        <DumpNode> Protected Friend ReadOnly Property get_TranslationActivity() As Double
            Get
                Dim sum = (From item In Me.ExpressionRegulationNetwork._InternalEvent_Translations__ Select Global.System.Math.Abs(item.FluxValue)).Sum
                Dim value As String = Global.System.Math.Log(sum, 2).ToString
                Return Val(value)
                '  Return Val(sum)
            End Get
        End Property

        <DumpNode> Protected Friend ReadOnly Property get_MetabolismActivity() As Double
            Get
                Dim sum = (From item In Me.Metabolism.DelegateSystem.NetworkComponents Select Global.System.Math.Abs(item.FluxValue)).Sum
                Dim value As String = Global.System.Math.Log10(sum).ToString
                Return Val(value)
            End Get
        End Property

        <DumpNode> Protected Friend ReadOnly Property get_SignalTransductionActivity() As Double
            Get
                Dim sum = (From item In Me.SignalTransductionNetwork.DataSource Select Global.System.Math.Abs(item.value)).Sum
                Dim value As String = Global.System.Math.Log10(sum).ToString
                Return Val(value)
            End Get
        End Property
#End Region

        Public Sub set_CultivationMedium(CultivationMedium As ObjectModels.SubSystem.CultivationMediums)
            Call Me._InternalEventDriver.ConnectCultivationMediumSystem(CultivationMedium)
        End Sub

        Public Function Tick(KernelCycle As Integer) As Integer Implements IDrivenable.__innerTicks
            Call _InternalEventDriver.InvokeEvents()
            Return 0
        End Function

        Public Overrides Sub MemoryDump(Dir As String)
            Call Metabolism.MemoryDump(Dir)
            Call ExpressionRegulationNetwork.MemoryDump(Dir)
            Call SignalTransductionNetwork.MemoryDump(Dir)
        End Sub

        ''' <summary>
        ''' 先加载系统模块在进行系统的初始化操作
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Initialize() As Integer

            Call SystemLogging.WriteLine("Start to initialize the gc kernel...", "GCModeller -> kernel_thread")
            Call SystemLogging.WriteLine(String.Format("ID:= ""{0}""", DataModel.ModelProperty.SpecieId), "GCModeller -> kernel_thread")
            Call SystemLogging.WriteLine(String.Format("Description/Name:= ""{0}""", DataModel.ModelProperty.Name), "GCModeller -> kernel_thread")

            Call SystemLogging.WriteLine("---------------------------------------------------------------------------------------------------------" & vbCrLf & vbCrLf)
            Call SystemLogging.WriteLine("Start to initialize the cell system components....")

            Dim stopwatch = Diagnostics.Stopwatch.StartNew
            Dim n As Double = 0

            _InternalEventDriver = New I_SystemEventDriver(Me)

            Call Metabolism.Initialize() : Call SystemLogging.WriteLine(String.Format("    ==> Metabolism system initialize elapsed time: {0}ms...", stopwatch.ElapsedMilliseconds - n), "", Type:=Logging.MSG_TYPES.INF) : n = stopwatch.ElapsedMilliseconds
            Call ExpressionRegulationNetwork.Initialize() : Call SystemLogging.WriteLine(String.Format("    ==> Expression Regulation Network system initialize elapsed time: {0}ms...", stopwatch.ElapsedMilliseconds - n), "", Type:=Logging.MSG_TYPES.INF) : n = stopwatch.ElapsedMilliseconds

            SignalTransductionNetwork = New EngineSystem.ObjectModels.SubSystem.SignalTransductionNetwork(Me)
            Call SignalTransductionNetwork.Initialize()
            Call Me.Metabolism.InitalizeTrimedHandles() : Call SystemLogging.WriteLine(String.Format("    ==> Metabolism system InitalizeTrimedHandles() elapsed time: {0}ms...", stopwatch.ElapsedMilliseconds - n), "", Type:=Logging.MSG_TYPES.INF) : n = stopwatch.ElapsedMilliseconds

            Call InitializeGeneObjectMutation() : Call SystemLogging.WriteLine(String.Format("    ==> Expression Regulation Network system InitializeGeneObjectMutation() elapsed time: {0}ms...", stopwatch.ElapsedMilliseconds - n), "", Type:=Logging.MSG_TYPES.INF) : n = stopwatch.ElapsedMilliseconds
            Call _InternalEventDriver.Initialize()
            Call SystemLogging.WriteLine("Kernel initialization complete!", "GCModeller -> kernel_thread")

            Return 1
        End Function

        Protected Friend Sub InitializeGeneObjectMutation()
            Dim MutationARGV As String = Me.I_RuntimeContainer.GetArguments("-gene_mutations")
            If Not String.IsNullOrEmpty(MutationARGV) Then
                Call SystemLogging.WriteLine("Setup gene mutations on the cell model...")

                Using MutationCreator = EngineSystem.ObjectModels.ExperimentSystem.Mutations.TryParse(MutationARGV)
                    Call MutationCreator.ApplyMutation(Me)
                    Call SystemLogging.WriteLine(String.Format("Job completed: {0}", MutationCreator.ToString))
                End Using
            Else
                Call SystemLogging.WriteLine("There no gene mutation settings was found.")
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return DataModel.ModelProperty.SpecieId
        End Function

        Public Function CreateServiceSerials() As IDataAcquisitionService()
            Dim ServicesList = Me.Metabolism.CreateServiceSerials.AsList
            Call ServicesList.AddRange(Me.SignalTransductionNetwork.CreateServiceSerials)
            Call ServicesList.AddRange(ExpressionRegulationNetwork.CreateServiceSerials)
            Return ServicesList.ToArray
        End Function

        ''' <summary>
        ''' 当满足下面两个条件的时候，认为细胞死亡：
        ''' 1. 转录和翻译的流量总和小于1
        ''' 2. RNA分子和多肽链分子的数量总和小于1
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CellDeathDetection() As Boolean
            Get
                Dim ExpressionActivity = (From Transcription In ExpressionRegulationNetwork._InternalEvent_Transcriptions Select Transcription.FluxValue).ToArray.Sum + (From Translation In ExpressionRegulationNetwork._InternalEvent_Translations__ Select Translation.FluxValue).ToArray.Sum
                Dim Level = (From Transcript In ExpressionRegulationNetwork._InternalTranscriptsPool Select Transcript.DataSource.value).ToArray.Sum + (From Polypeptide In Metabolism.ProteinCPLXAssemblies.Proteins Select Polypeptide.DataSource.value).ToArray.Sum

                ExpressionActivity += (From BasalTranscription In ExpressionRegulationNetwork.BasalExpression.BasalExpressionFluxes Select BasalTranscription.FluxValue).ToArray.Sum
                ExpressionActivity += (From BasalTranslation In ExpressionRegulationNetwork.BasalExpression.BasalTranslationFluxs Select BasalTranslation.FluxValue).ToArray.Sum

                Return ExpressionActivity < 1 AndAlso Level < 1
            End Get
        End Property

        Public ReadOnly Property EventId As String Implements IDrivenable.EventId
            Get
                Return "Cell Event Driver"
            End Get
        End Property

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Sub Dispose(disposing As Boolean)
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
