#Region "Microsoft.VisualBasic::006c6f34f3ff78ea30106906f00e33ce, ..\GCModeller\engine\GCModeller\EngineSystem\Engine\Run.vb"

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

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration

Namespace EngineSystem.Engine

    Public Module Run

        Public Class KernelInitializer

            Dim ModelFile As String
            Dim FileFormat As String
            Public Logging As LogFile

            Dim _OriginalARGV As Microsoft.VisualBasic.CommandLine.CommandLine
            Dim ExternalModuleRegistry As SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleRegistry
            Dim _configurationsData As ConfigReader

            'run -i <model_file> -mysql <mysql_connection_string> [-t <time> -metabolism <assembly_path> -expression <assembly_path> -cultivation_mediums <medium_csv_file>]
            ''' <summary>
            ''' Configuration 之中的配置数据总是会被非空的命令行参数所复写
            ''' </summary>
            ''' <param name="ExternalModuleRegistry"></param>
            ''' <param name="CommandLine"></param>
            ''' <param name="Configuration"></param>
            ''' <remarks></remarks>
            Sub New(ExternalModuleRegistry As PlugIns.ModuleRegistry, CommandLine As CommandLine.CommandLine, Configuration As Engine.Configuration.Configurations)
                Me._configurationsData = New Configuration.ConfigReader(Configuration)
                Me.ModelFile = CommandLine("-i")
                Me.Logging = New LogFile(
                    Path:=Settings.LogDIR & String.Format("GCModeller__{0}_{1}.log", FileIO.FileSystem.GetFileInfo(ModelFile).Name.ToUpper, LogFile.NowTimeNormalizedString))
                Me.Logging.ColorfulOutput = String.Equals(CommandLine("-color_message").ToUpper, "T")

                Me.Logging.SuppressError = _configurationsData.SuppressErrorMessage
                Me.Logging.SuppressWarns = _configurationsData.SuppressWarnMessage

                GCModellerCommons.LoggingClient = Logging

                Logging.WriteLine(String.Format("Call gchost.exe {0}", CommandLine.ToString), "gchost -> main()")
                Logging.WriteLine(String.Format("Model file: {0}", Me.ModelFile), "gchost -> main()")

                Dim Loops = Val(CommandLine("-t"))
                If Loops > 0 Then Configuration.KernelCycles = Loops
                Dim ModuleMetabolismSystem = CommandLine("-metabolism")
                If String.IsNullOrEmpty(ModuleMetabolismSystem) Then
                    If String.IsNullOrEmpty(Configuration.MetabolismModel) Then
                        Configuration.MetabolismModel = ""
                    End If
                Else
                    Configuration.MetabolismModel = ModuleMetabolismSystem
                End If

                Dim ExpressionRegulationSystem = CommandLine("-expression")
                If String.IsNullOrEmpty(ExpressionRegulationSystem) Then
                    If String.IsNullOrEmpty(Configuration.ExpressionRegulationNetwork) Then
                        Configuration.ExpressionRegulationNetwork = ""
                    End If
                Else
                    Configuration.ExpressionRegulationNetwork = ExpressionRegulationSystem
                End If

                Dim CultivatingMediums = CommandLine("-cultivation_mediums")
                If Not String.IsNullOrEmpty(CultivatingMediums) Then
                    _configurationsData.Configs.CultivationMediums = CultivatingMediums
                End If

                Dim DataStorageURL = CommandLine("-url")
                If Not String.IsNullOrEmpty(DataStorageURL) Then
                    Configuration.DataStorageUrl = DataStorageURL
                End If

                Dim MutationData As String = CommandLine("-mutation_genes")
                If Not String.IsNullOrEmpty(MutationData) Then
                    Configuration.GeneMutations = MutationData
                End If

                Dim CommitInterval = Val(CommandLine("-interval")) : If CommitInterval > 0 Then Configuration.CommitLoopsInterval = CommitInterval
                If Configuration.CommitLoopsInterval < 1 Then Configuration.CommitLoopsInterval = 10
                Me.FileFormat = If(String.Equals(CommandLine("-f").ToLower, "gcml"), "GCML", "CSV_TABULAR")
                Me._OriginalARGV = CommandLine
            End Sub

            Private Function CheckArguments() As Integer
                If String.IsNullOrEmpty(ModelFile) Then
                    Logging.WriteLine("User not specific the model file, could not start the calculation!", "gchost -> main()", Type:=MSG_TYPES.ERR)
                    Logging.Save()
                    Return -1
                ElseIf Not FileIO.FileSystem.FileExists(ModelFile) Then
                    Logging.WriteLine(String.Format("Could not start the modelling engine, operation aborted! (_FILE_NOT_FOUND_ ""{0}"")", ModelFile), "gchost -> main()", Type:=MSG_TYPES.ERR)
                    Logging.Save()
                    Return -2
                End If

                If String.IsNullOrEmpty(Me._configurationsData.DataStorageURL.Value) Then
                    Logging.WriteLine("Data storage connection url string was not specific by user, could not initialize the data acquisition service.",
                                  "gchost -> main()",
                                  MSG_TYPES.ERR)
                    Return -3
                End If

                Return 0
            End Function

            Private Function InitializeKernel(EngineKernel As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller) As Integer
                Call LoadModules(EngineKernel)  '向计算框架之中加载计算模块
                Call EngineKernel.Initialize()   '对整个计算引擎做最基本的初始化操作

                If EngineKernel.ConnectDataService(ServiceURL:=Me._configurationsData.DataStorageURL.Value) < 0 Then
                    Logging.WriteLine("ERROR in connect to the data storage service.", "gchost -> main()", MSG_TYPES.ERR)
                    Return -11
                Else
                    Logging.WriteLine("Modeller engine connect to data storage service successfully!", "gchost -> main()")
                End If

                Logging.WriteLine(String.Format("Setup loop count to {0}", _configurationsData.CommitLoopsInterval), "gchost -> main()")

                Return 0
            End Function

            Private Function LoadModules(EngineKernel As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller)
                Dim DisabledModule As List(Of String) = New List(Of String)
                Dim LoadModule As List(Of String) = New List(Of String)

                If String.Equals(_configurationsData.Model_MetabolismSystem, "N/A") Then
                    DisabledModule.Add("metabolism")
                Else
                    LoadModule.Add(_configurationsData.Model_MetabolismSystem)
                End If
                If String.Equals(_configurationsData.Model_ExpressionNetwork, "N/A") Then
                    DisabledModule.Add("expression")
                Else
                    LoadModule.Add(_configurationsData.Model_ExpressionNetwork)
                End If

                Call EngineKernel.LoadSystemModule((From strId As String In LoadModule Where Not String.IsNullOrEmpty(strId) Select ExternalModuleRegistry.GetModule(strId)).ToArray, DisabledModule)
                Return 0
            End Function

            Public Function InitializeModellerEngine() As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller
                Dim SuccessFlag As Integer = Me.CheckArguments

                If SuccessFlag Then
                    Call Logging.WriteLine("INITIALIZE_ARGUMENTS_ERROR, program terminated!", "", Type:=MSG_TYPES.ERR)
                    Call Logging.Save()
                    Return Nothing
                End If

                Logging.WriteLine(String.Format("Kernel load model data from file:{0}'{1}'", vbCrLf, ModelFile), "gchost -> main()")

                Dim ModellerEngine As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller = Nothing

                Try
                    If String.Equals(Me.FileFormat, "GCML") Then
                        ModellerEngine = SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller.Load(ModelFile, Me.Logging, Me._OriginalARGV)
                    Else

                    End If

                    Logging.WriteLine("Initializing the engine kernel...", "gchost -> main()")
                    ModellerEngine.KernelProfile = Me._configurationsData
                    SuccessFlag = Me.InitializeKernel(EngineKernel:=ModellerEngine)
                Catch ex As Exception
                    Call Logging.WriteLine("Engine kernel was not success initialize due to an unexpected exception:" & vbCrLf & ex.ToString, "", Type:=MSG_TYPES.ERR)
                    Call Logging.WriteLine("INITIALIZE_ENGINE_KERNEL_NOT_SUCCESSFUL, gchost program exit.", "gchost -> main()")
                    Call Logging.Save()
                    Throw
                End Try

                If SuccessFlag Then
                    Call Logging.WriteLine("INITIALIZE_ENGINE_KERNEL_NOT_SUCCESSFUL, gchost program exit.", "gchost -> main()")
                    Call Logging.Save()
                    ModellerEngine = Nothing
                End If

                Return ModellerEngine
            End Function
        End Class

        Public Function Invoke(ExternalModuleRegistry As SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleRegistry,
                               Configurations As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration.Configurations,
                               argvs As CommandLine.CommandLine) As Integer

            Call Settings.Session.Initialize()

            Dim ModellerEngine = New KernelInitializer(ExternalModuleRegistry, argvs, Configurations).InitializeModellerEngine

            If ModellerEngine Is Nothing Then
                Return -1
            End If

            Dim Logging As LogFile = ModellerEngine.SystemLogging

            Logging.WriteLine("Start gcmodeller engine main thread, gchost main thread wait for exit", "gchost -> main()")
            Logging.WriteLine("Kernel Running...", "gchost -> main()")

#If Not Debug Then
            Try
#End If

#If DEBUG Then
            Call ModellerEngine.Run()
#Else

                Try
                    Call ModellerEngine.Run()
                Catch ex As Exception
                    Call Logging.WriteLine("A critical unhandle exception was occured while the gcmodell kernel main thread was running and the engine was unable recovery from this error:" &
                                  vbCrLf & ex.ToString, "gchost -> main() UNHANDLE_EXCEPTION_THREAD_HOST", MSG_TYPES.ERR)
                    If String.IsNullOrEmpty(ModellerEngine.ConfigurationData.DumpData) Then
                        Call New Threading.Thread(Sub() Call ModellerEngine.MemoryDump(Settings.TEMP & "/GCHOST_ERROR-MemoryDump.xml")).Start()
                    Else
                        Call New Threading.Thread(Sub() Call ModellerEngine.MemoryDump(ModellerEngine.ConfigurationData.DumpData & "/GCHOST_ERROR-MemoryDump.xml")).Start()
                    End If

                    GoTo [Exit]
                End Try
#End If
                Logging.WriteLine("GCModeller engine main thread exit, gchost main thread continute", "gchost -> main()")
                Logging.WriteLine("Job complete!", "gchost -> main()")
[Exit]:         Logging.WriteLine("gchost program exit.", "gchost -> main()")


#If Not Debug Then
            Catch ex As Exception
                Call Logging.WriteLine("GCModeller engine main thread exit, gchost main thread continute", "gchost -> main()")
                Call Logging.WriteLine(Format("An unexpect error occur!\nDebug information for developer:\n%s", ex.ToString), "gchost -> main()", MSG_TYPES.ERR)
                Call Logging.WriteLine("gchost program exit.", "gchost -> main()")
            End Try
#End If
            Call Logging.Save()
            Return 0
        End Function
    End Module
End Namespace
