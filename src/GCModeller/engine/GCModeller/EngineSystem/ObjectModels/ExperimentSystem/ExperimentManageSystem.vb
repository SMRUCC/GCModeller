Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects

Namespace EngineSystem.ObjectModels.ExperimentSystem

    ''' <summary>
    ''' 周期性实验的管理系统
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ExperimentManageSystem : Inherits SubSystem.SystemObjectModel
        Implements IDrivenable

        Dim Target As EngineSystem.ObjectModels.SubSystem.CellSystem
        Dim NullExperimentList As Boolean

        Protected Friend PendingExperiments As List(Of ExperimentManageSystem.FactorVariables)
        Protected Friend RunningExperiments As List(Of ExperimentManageSystem.FactorVariables)

        Dim ConditionExperimentManagementSystem As TriggerSystem

        Sub New(Cell As EngineSystem.ObjectModels.SubSystem.CellSystem)
            Me.Target = Cell
            MyBase.I_RuntimeContainer = Cell.get_runtimeContainer
        End Sub

        Public Function Tick(KernelCycle As Integer) As Integer Implements IDrivenable.__innerTicks
            Call ConditionExperimentManagementSystem.Tick(KernelCycle)

            If NullExperimentList = True Then  '由于本参数会用于判断是否还有处于等待队列中的实验，所以请在本语句之前进行其他触发器和脚本的执行
                Return -1
            End If
            If PendingExperiments.IsNullOrEmpty AndAlso RunningExperiments.IsNullOrEmpty Then
                NullExperimentList = True
            End If

            'Quene pending
            Dim ExperimentList As ExperimentSystem.ExperimentManageSystem.FactorVariables() = (
                From experiment As ExperimentManageSystem.FactorVariables
                In PendingExperiments
                Where experiment.Start >= Me.I_RuntimeContainer.RuntimeTicks
                Select experiment).ToArray
            Call RunningExperiments.AddRange(ExperimentList)
            For i As Integer = 0 To ExperimentList.Count - 1
                Call PendingExperiments.Remove(ExperimentList(i))
            Next
            For i As Integer = 0 To RunningExperiments.Count - 1
                Call RunningExperiments(i).Tick(I_RuntimeContainer.RuntimeTicks)
            Next

            ExperimentList = (From experiment As ExperimentManageSystem.FactorVariables
                              In RunningExperiments
                              Where experiment.Kicks = 0
                              Select experiment).ToArray
            For Each Experiment As ExperimentManageSystem.FactorVariables In ExperimentList
                Call PendingExperiments.Remove(Experiment)
            Next

            Return 0
        End Function

        Public Overrides Sub MemoryDump(Dir As String)
            Call Me.I_CreateDump.SaveTo(String.Format("{0}/{1}.log", Dir, Me.GetType.Name))
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DataFile">CSV格式的实验配置数据文件的文件路径</param>
        ''' <remarks></remarks>
        Public Sub LoadExperimentData(DataFile As String)
            If String.IsNullOrEmpty(DataFile) Then
                Call Printf("[INFO] not found any experiment data file, experiment system will be hold.")
                NullExperimentList = True
                Return
            ElseIf Not FileIO.FileSystem.FileExists(DataFile) Then
                Call Printf("[WARNNING] Could not find the experiment data file ""%s"", please check out the data file path parameter!", DataFile)
                NullExperimentList = True
                Return
            End If

            PendingExperiments = New List(Of ExperimentManageSystem.FactorVariables)
            RunningExperiments = New List(Of ExperimentManageSystem.FactorVariables)

            Dim DataModel = DataFile.LoadCsv(Of LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment)(False)
            Call PendingExperiments.AddRange((From item As Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment In DataModel
                                              Let ExperimentObject = ExperimentManageSystem.FactorVariables.CreateObject(item, Target.Metabolism)
                                              Where Not ExperimentObject Is Nothing
                                              Select ExperimentObject
                                              Order By ExperimentObject.Start Ascending).ToArray)
        End Sub

        Public Overrides Function Initialize() As Integer
            ConditionExperimentManagementSystem = New TriggerSystem(DirectCast(Me.I_RuntimeContainer, EngineSystem.Engine.GCModeller))
            Call ConditionExperimentManagementSystem.Initialize()
            Return 0
        End Function

        Protected Friend Overrides ReadOnly Property SystemLogging As Logging.LogFile
            Get
                Return Target.SystemLogging
            End Get
        End Property

        Public ReadOnly Property EventId As String Implements IDrivenable.EventId
            Get
                Return "Experiment Driver"
            End Get
        End Property
    End Class
End Namespace