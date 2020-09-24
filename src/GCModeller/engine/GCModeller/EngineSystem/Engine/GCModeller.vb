#Region "Microsoft.VisualBasic::9b2050aaa52226621f9e5c7108f04c65, engine\GCModeller\EngineSystem\Engine\GCModeller.vb"

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

    '     Class GCModeller
    ' 
    '         Properties: args, ConfigurationData, CultivatingMediums, DataAcquisitionService, EventId
    '                     KernelModule, KernelProfile, MemoryDumpTime, PhysicalMemory, RuntimeTicks
    '                     SystemLogging, SystemVariable
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __innerTicks, (+2 Overloads) ConnectDataService, get_DataModel, GetArguments, Initialize
    '                   Load, RegisterEvent, Run, ToString
    ' 
    '         Sub: ConnectLoggingClient, (+2 Overloads) Dispose, InternalRecordActivityStatus, LoadKernel, LoadSystemModule
    '              MemoryDump
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.MemoryDump
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects

Namespace EngineSystem.Engine

    <XmlRoot("GCModeller-MemoryDump", Namespace:="http://code.google.com/p/genome-in-code/")>
    Public NotInheritable Class GCModeller : Inherits IterationMathEngine(Of BacterialModel)
        Implements ISystemDriver
        Implements System.IDisposable
        Implements IContainerSystemRuntimeEnvironment

        Protected Friend _DataAcquisitionService As EngineSystem.Services.DataAcquisition.ManageSystem
        Protected Friend _CultivatingMediums As EngineSystem.ObjectModels.SubSystem.CultivationMediums

        ''' <summary>
        ''' 整个系统中的计算核心模块
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property KernelModule As CellSystem

        Public Sub LoadKernel(kernel As CellSystem)
            _KernelModule = kernel
        End Sub

        <DumpNode> <XmlIgnore> Public ReadOnly Property DataAcquisitionService As EngineSystem.Services.DataAcquisition.ManageSystem
            Get
                Return _DataAcquisitionService
            End Get
        End Property
        <DumpNode> Public ReadOnly Property CultivatingMediums As EngineSystem.ObjectModels.SubSystem.CultivationMediums
            Get
                Return _CultivatingMediums
            End Get
        End Property

        <DumpNode>
        Dim ExperimentSystem As EngineSystem.ObjectModels.ExperimentSystem.ExperimentManageSystem

        ''' <summary>
        ''' 程序最开始输入得到的命令行参数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode>
        Public Property args As CommandLine

        Dim ShellScriptDevie As ShellScript

        ''' <summary>
        ''' 计算引擎的配置数据
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement>
        Public Property KernelProfile As EngineSystem.Engine.Configuration.ConfigReader

        Dim SystemActivityRecordList As IO.File
        Dim _InternalDictSystemVariables As Dictionary(Of String, String)

        Public ReadOnly Property SystemLogging As LogFile Implements IContainerSystemRuntimeEnvironment.SystemLogging

        <DumpNode> Private ReadOnly Property MemoryDumpTime As String
            Get
                Return Now.ToUniversalTime
            End Get
        End Property

        <DumpNode> Private ReadOnly Property PhysicalMemory As String
            Get
                Return My.Application.Info.WorkingSet / 1024 / 1024 & " MB"
            End Get
        End Property

        Sub New(Model As BacterialModel)
            Call MyBase.New(Model)
        End Sub

        Protected Friend Function get_DataModel() As BacterialModel
            Return Me._innerDataModel
        End Function

        ''' <summary>
        ''' 加载系统模块，本操作要先于Initialize操作执行
        ''' </summary>
        ''' <param name="ModuleAssembly">外部系统模块列表</param>
        ''' <remarks></remarks>
        Public Sub LoadSystemModule(ModuleAssembly As String(), DisabledModules As List(Of String))
            Using ModuleLoader As SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleLoader = New PlugIns.ModuleLoader(Kernel:=Me)
                Call ModuleLoader.DisableModule(DisabledModules.ToArray)
                Call ModuleLoader.LoadModules(ModuleAssembly)
            End Using
        End Sub

        Public Overrides Function Initialize() As Integer
            Call _SystemLogging.WriteLine("Load system variables from the model data...")
            Call _SystemLogging.WriteLine(Format_Prints(_innerDataModel.SystemVariables), "SYSTEM_VARIABLES")

            Me._InternalDictSystemVariables = _innerDataModel.SystemVariables.ToDictionary(keySelector:=Function(item) item.Key, elementSelector:=Function(item) item.Value)

            Dim CultivationMediumData As String = KernelProfile.CultivationMediumPath  '对于Ph_Temperature的初始化需要CultivationMediums对象，故而培养基对象要优先于细胞对象的初始化
            If String.IsNullOrEmpty(CultivationMediumData) Then
                Call _SystemLogging.WriteLine("Custom cultivation medium data was not specific, using model default compiled data.")
                Me._CultivatingMediums = New ObjectModels.SubSystem.CultivationMediums(Me._innerDataModel.CultivationMediums, RuntimeContainer:=Me)
            Else
                Call _SystemLogging.WriteLine("User specific a custom cultivation medium data in file:" & vbCrLf & CultivationMediumData)
                Me._CultivatingMediums = New ObjectModels.SubSystem.CultivationMediums(New GCMarkupLanguage.CultivationMediums With {.Uptake_Substrates = CultivationMediumData.LoadCsv(Of I_SubstrateRefx)(False).ToArray}, RuntimeContainer:=Me)
            End If
            Call Me._CultivatingMediums.Initialize()
            Call KernelModule.Initialize()

            Call Me._SystemLogging.WriteLine(" ===========================*** Start to Initializing the rumtime enviroment... ***====================================", "Modeller -> Initializer()", Type:=MSG_TYPES.INF)

            '  Me._SystemLogging.SuppressWarns = String.Equals(Me.System_argvs("-suppress_warn"), "T", StringComparison.OrdinalIgnoreCase)
            ' Me._SystemLogging.SuppressError = String.Equals(Me.System_argvs("-suppress_error"), "T", StringComparison.OrdinalIgnoreCase)

            My.Computer.FileSystem.CurrentDirectory = My.Application.Info.DirectoryPath
            Me.ExperimentSystem = New ObjectModels.ExperimentSystem.ExperimentManageSystem(KernelModule)

            Me.SystemActivityRecordList = New IO.File
            Me.ShellScriptDevie = New ShellScript(Me)

            Call Me.SystemActivityRecordList.Add(New String() {"#TIME", "Transcription", "Translation", "Metabolism", "SUM()", "pH", "SignalTransductionNetwork"})
            Call Me._SystemLogging.WriteLine("  =====>  Initialize experiment system...", "Modeller -> Initializer()", Type:=MSG_TYPES.INF)
            Call Me.ExperimentSystem.LoadExperimentData(KernelProfile.ExperimentData)
            Call Me._SystemLogging.WriteLine("  =====>  Initialize data acquisition service...", "Modeller -> Initializer()", Type:=MSG_TYPES.INF)
            Call Me.DataAcquisitionService.Join(KernelModule.CreateServiceSerials)
            Call Me.DataAcquisitionService.Join(CultivatingMediums.CreateServiceSerials)
            Call Me._SystemLogging.WriteLine("  =====>  Initialize cultivation mediums system...", "Modeller -> Initializer()", Type:=MSG_TYPES.INF)
            Call Me._CultivatingMediums.Add(KernelModule)
            Call Me.DataAcquisitionService.Initialize()
            Call Me.ExperimentSystem.Initialize()
            Call Me.ShellScriptDevie.Initialize()

            Call Me._SystemLogging.WriteLine("  =====>  Enviroment Initializing complete!", "Modeller -> Initializer()", Type:=MSG_TYPES.INF)
            Call Me._SystemLogging.WriteLine("  <<<<<<  Back to the main thread!", "Modeller -> Initializer()", Type:=MSG_TYPES.INF)

            Return -1
        End Function

        ''' <summary>
        ''' Setup the connection between the data acquisition service to the data storage service.
        ''' </summary>
        ''' <param name="ServiceURL"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ConnectDataService(ServiceURL As String) As Integer
            Call _SystemLogging.WriteLine("Setup the connection between the data acquisition service to the data storage service.", "connect_data_service()", Type:=MSG_TYPES.INF)
            Return DataAcquisitionService.Connect(ServiceURL)  '数据采集服务连接至数据存储服务
        End Function

        Public Function ConnectDataService(Services As Services.DataAcquisition.ManageSystem) As GCModeller
            Me._DataAcquisitionService = Services
            Return Me
        End Function

        Public Overrides Function Run() As Integer
            Dim Stopwatch As Stopwatch = Stopwatch.StartNew

            Call SystemLogging.WriteLine(String.Format("Start kernel loop at {0}", Now.ToString), "GCModeller -> main_thread")

#If Not DEBUG Then
            Try
#End If
                For Me._RTime = 0 To KernelProfile.KernelLoops
                    If __innerTicks(_RTime) < 0 Then Exit For
                Next
#If Not DEBUG Then
            Catch ex As Exception
                Call _SystemLogging.WriteLine(ex.ToString, "GCModeller->RUN()", Type:=MSG_TYPES.ERR)
                Call _SystemLogging.WriteLine("------------------------------------------------------------------------------------------------------------")
                Call _SystemLogging.WriteLine("A system crash has been detacted by GCModeller......", "", Type:=MSG_TYPES.INF)

                GoTo EXIT_
            End Try
#End If

EXIT_:      Call DataAcquisitionService.CloseStorageService()

            Call SystemLogging.WriteLine(String.Format("Kernel loop duration time '{0}'", Stopwatch.ElapsedMilliseconds), "GCModeller -> main_thread")
            Call SystemLogging.WriteLine(String.Format("Exit kernel loop at {0}", Now.ToString), "GCModeller -> main_thread")

            Dim SavedDir As String = args("-activity_export")
            If String.IsNullOrEmpty(SavedDir) Then
                If ConfigurationData.DataStorageURL.Key = Services.DataAcquisition.ManageSystem.DataStorageServiceTypes.CSV Then
                    SavedDir = ConfigurationData.DataStorageURL.Value
                Else
                    SavedDir = My.Computer.FileSystem.SpecialDirectories.Desktop
                End If
            End If

            Dim SavedPath As String = String.Format("{0}/{1}.csv", SavedDir, FileIO.FileSystem.GetName(Me.args("-i")))
            Call Me.SystemActivityRecordList.Save(SavedPath, False)
            Call SystemLogging.WriteLine(String.Format("System Activity Status Record was saved to ""{0}"", you can view the detail data in the csv data file.", SavedPath))

            Return -1
        End Function

        ''' <summary>
        ''' 将计算引擎的工作情况做一次快照
        ''' </summary>
        ''' <param name="DumpFile"></param>
        ''' <remarks></remarks>
        Public Sub MemoryDump(DumpFile As String) Implements IContainerSystemRuntimeEnvironment.MemoryDump
            Call Me.SystemLogging.WriteLine("Trying to create memory dump of GCModeller engine kernel for developer debugging.....", "GCModeller->MemoryDump()", MSG_TYPES.INF)
            Try
                Call Me.KernelModule.MemoryDump(Dir:=DumpFile)
            Catch ex As Exception
                Call _SystemLogging.WriteLine("Error occur while trying to create memory dump!", "MemoryDump(DumpFile As String)", Type:=MSG_TYPES.ERR)
                Call _SystemLogging.WriteLine(ex.ToString, "MemoryDump(DumpFile As String)", Type:=MSG_TYPES.ERR)
                Throw
            End Try
            Call Me.SystemLogging.WriteLine("Memory dump was created at file location: """ & DumpFile & """!", "GCModeller->MemoryDump()", MSG_TYPES.INF)
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("Kernel Loop: {0}  --> {1}", _RTime, KernelModule.ToString)
        End Function

        ''' <summary>
        ''' 从数据模型文件之中初始化计算系统
        ''' </summary>
        ''' <param name="ModelFile">The data model for the target modelling cell system.(所要执行模拟计算的目标细胞的数据模型)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(ModelFile As String, LogFile As LogFile, argvs As Microsoft.VisualBasic.CommandLine.CommandLine) As Engine.GCModeller
            Dim Model As BacterialModel =
                Microsoft.VisualBasic.LoadXml(Of BacterialModel)(ModelFile)
            Dim EngineSystem = New GCModeller(Model) With {._SystemLogging = LogFile, .args = argvs}
            Dim KernelModule = New ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem(EngineSystem)
            EngineSystem.LoadKernel(KernelModule)
            EngineSystem._DataAcquisitionService = New Services.DataAcquisition.ManageSystem(ModellerKernel:=EngineSystem)
            Return EngineSystem
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer Implements IDrivenable.__innerTicks
            Dim stw = Stopwatch.StartNew

            Call _CultivatingMediums.TICK()
            Call DataAcquisitionService.Tick(_RTime)
            Call ExperimentSystem.Tick(_RTime)
            Call Me.SystemLogging.WriteLine(String.Format(" TICK()  TIME_LAST:={0}ms", stw.ElapsedMilliseconds), "gcmodeller -> ticker()", Type:=MSG_TYPES.INF)
            Call Me.InternalRecordActivityStatus()
            Call Me.ShellScriptDevie.Tick(_RTime)

            If CultivatingMediums.CellObjects.Count = 0 Then
                Call _SystemLogging.WriteLine("No Cell object contains in the cultivation mediums, thread exit!")
                Return -1
            Else
                Return 0
            End If
        End Function

        Private Sub InternalRecordActivityStatus()
            Dim Transcription As Double = KernelModule.get_TranscriptionActivity
            Dim Translation As Double = KernelModule.get_TranslationActivity
            Dim Metabolism As Double = KernelModule.get_MetabolismActivity
            Dim pH As Double = KernelModule.Metabolism.Get_currentPH
            Dim STrN As Double = KernelModule.get_SignalTransductionActivity
            Dim Prints As String = String.Format(" [{0}] SystemActivityLoad [pH:={1}] ===>> [ Transcription:={2}; Translation:={3}; Metabolism:={4}; SignalTransductionNetwork:={5} ]",
                                                 MyBase._RTime, pH,
                                                 Transcription,
                                                 Translation,
                                                 Metabolism, STrN)

            Call SystemLogging.WriteLine(Prints, "gcmodeller -> RecordActivityStatus()")
            Call Me.SystemActivityRecordList.Add(New String() {MyBase._RTime, Transcription, Translation, Metabolism, Transcription + Translation + Metabolism, pH, STrN})
        End Sub

        Public Function GetArguments(Name As String) As String Implements IContainerSystemRuntimeEnvironment.GetArguments
            Return Me.args(Name)
        End Function

        Public ReadOnly Property SystemVariable(var As String) As String Implements IContainerSystemRuntimeEnvironment.SystemVariable
            Get
                If _InternalDictSystemVariables.ContainsKey(var) Then
                    Return _InternalDictSystemVariables(var)
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property ConfigurationData As Configuration.ConfigReader Implements IContainerSystemRuntimeEnvironment.ConfigurationData
            Get
                Return KernelProfile
            End Get
        End Property

        Public Function RegisterEvent([Event] As IDrivenable) As Object Implements ISystemDriver.RegisterEvent
            Throw New NotImplementedException
        End Function

        Public ReadOnly Property EventId As String Implements IDrivenable.EventId
            Get
                Throw New NotImplementedException
            End Get
        End Property

        Public Sub ConnectLoggingClient(client As LogFile)
            Me._SystemLogging = client
        End Sub

        Public Overrides ReadOnly Property RuntimeTicks As Long Implements IContainerSystemRuntimeEnvironment.RuntimeTicks
            Get
                Return Me._RTime
            End Get
        End Property
    End Class
End Namespace
