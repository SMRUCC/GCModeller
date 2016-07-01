Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports Microsoft.VisualBasic.Logging

Namespace EngineSystem.ObjectModels.SubSystem

    Public MustInherit Class SystemObject : Inherits RuntimeObject
        Protected Friend MustOverride ReadOnly Property SystemLogging As LogFile

        Public Interface I_SystemModel
            ReadOnly Property SystemLogging As Logging.LogFile

            Sub MemoryDump(Dir As String)
            Function get_RuntimeContainer() As IContainerSystemRuntimeEnvironment
            Function Initialize() As Integer
        End Interface
    End Class

    Public MustInherit Class SystemObjectModel : Inherits SystemObject
        Implements SystemObject.I_SystemModel

        ''' <summary>
        ''' 当前子系统模块的上一层系统模块
        ''' </summary>
        ''' <remarks></remarks>
        Protected I_RuntimeContainer As IContainerSystemRuntimeEnvironment

        Public MustOverride Function Initialize() As Integer Implements SystemObject.I_SystemModel.Initialize

        Protected Friend Overrides ReadOnly Property SystemLogging As Logging.LogFile Implements SystemObject.I_SystemModel.SystemLogging
            Get
                Return I_RuntimeContainer.SystemLogging
            End Get
        End Property

        MustOverride Sub MemoryDump(Dir As String) Implements SystemObject.I_SystemModel.MemoryDump

        Public Overridable Function get_runtimeContainer() As IContainerSystemRuntimeEnvironment Implements SystemObject.I_SystemModel.get_RuntimeContainer
            Return I_RuntimeContainer
        End Function
    End Class
End Namespace