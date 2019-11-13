#Region "Microsoft.VisualBasic::bfc1bdc36b363a90ce4295fd752dd16a, engine\GCModeller\EngineSystem\ObjectModels\ExperimentSystem\TriggerSystem\TriggerSystem.vb"

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

    '     Class TriggerSystem
    ' 
    '         Properties: EventId
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Initialize, ParseExpression, Tick, TriggerInvokes, TrimExpression
    ' 
    '         Sub: MemoryDump
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Framework.DynamicCode.VBC

Namespace EngineSystem.ObjectModels.ExperimentSystem

    ''' <summary>
    ''' 由于需要动态构建代码并进行编译，由于Trigger进行条件测试的时候需要动态引用子系统部件的实例，故而TriggerSystem需要在CellSystem初始化完毕之后才可以初始化 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TriggerSystem : Inherits SubSystem.SystemObjectModel
        Implements IDrivenable

        '   Private Shared ReadOnly TriggerConditionExpressionParser As Linq.Parser.Parser = New Linq.Parser.Parser

        Dim _Triggers As Triggers.ConditionalTrigger()
        Dim _CellSystem As SubSystem.CellSystem

        ''' <summary>
        ''' 列表中的对象的顺序按照<see cref="Prefix.PrefixTypeReference"></see>中的对象的顺序进行排列
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _ComponentSource As Object()

        Sub New(Kernel As EngineSystem.Engine.GCModeller)
            MyBase.I_RuntimeContainer = Kernel
            Me._CellSystem = Kernel.KernelModule
        End Sub

        Private Function ParseExpression(s As String) As CodeDom.CodeExpression

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' vbc Code:
        ''' 
        ''' Namespace TriggerConditionTest
        '''    Public Class ConditionTest
        '''         
        '''       Public Function TestCondition(Prefix1, Prefix2, ...) As Boolean
        '''          Return [strTriggerCondition]
        '''       End Function
        '''    End Class
        ''' End Namespace
        ''' </remarks>
        Public Overrides Function Initialize() As Integer
            Call SystemLogging.WriteLine("-------------------------------------------------------------------------")
            Call SystemLogging.WriteLine("Start to initialize the trigger system for the conditional experiments...")

            Dim ExperimentFile As String = MyBase.I_RuntimeContainer.GetArguments("-experiments")

            If String.IsNullOrEmpty(ExperimentFile) Then
                Call SystemLogging.WriteLine("User not specific the trigger experiment....", "", Type:=MSG_TYPES.INF)
                _Triggers = New ExperimentSystem.Triggers.ConditionalTrigger() {}
                Return 0
            End If

            Dim VBC As DynamicCompiler = New DynamicCompiler(_CellSystem, Settings.GetSettings(".net_sdk"))
            Dim ExperimentModels = ExperimentFile.LoadCsv(Of Experiment)(False)
            Dim LQuery = (From Experiment In (From Handle As Integer In ExperimentModels.Sequence Let ItemObject = ExperimentModels(Handle) Select New With {.Handle = Handle, .Value = ItemObject}).ToArray
                          Let InitializeCondition As String = Experiment.Value.TriggedCondition.Trim
                          Where Regex.Match(InitializeCondition, "^TEST\(.+\)$", RegexOptions.IgnoreCase And RegexOptions.Multiline).Success
                          Let Expression = ParseExpression(TrimExpression(InitializeCondition))
                          Select New KeyValuePair(Of Long, CodeDom.CodeExpression)(Experiment.Handle, Expression)).ToArray

            If LQuery.IsNullOrEmpty Then '没有定义条件触发的实验，则不必再初始化触发器系统
                _Triggers = New Triggers.ConditionalTrigger() {}
                Call SystemLogging.WriteLine("No conditional experiments trigger was define....")
                Return 0
            Else
                Call SystemLogging.WriteLine(String.Format("User have define {0} trigger in the experiment data.", LQuery.Count), "TriggerSystem->Initialize()", Type:=MSG_TYPES.INF)
            End If

            For Each Expression In LQuery
                Call VBC.AddTestModel(Expression.Key, Expression.Value)
            Next

            Call SystemLogging.WriteLine("Start to compile the trigger system into a dynamic assembly module!", "", Type:=MSG_TYPES.INF)

            Dim DynamicAssembly As Global.System.Reflection.Assembly = VBC.Compile
            Dim TestMethods As Global.System.Reflection.MethodInfo() =
                DynamicAssembly.GetType(String.Format("{0}.{1}", Framework.DynamicCode.VBC.DynamicCompiler.CONDITION_TEST_NAMESPACE, Framework.DynamicCode.VBC.DynamicCompiler.CONDITION_TEST_MODULE_NAME)) _
                    .GetMethods(Reflection.BindingFlags.Public Or Reflection.BindingFlags.Static)
            Dim ScriptCompiler As Prefix.ActionScript = New Prefix.ActionScript(Me.I_RuntimeContainer)

            Call SystemLogging.WriteLine("Link the action script to the trigger...", "", Type:=MSG_TYPES.INF)

            Me._Triggers = (From TestMethod In TestMethods Select EngineSystem.ObjectModels.ExperimentSystem.Triggers.ConditionalTrigger.CreateObject(TestMethod, TriggerSystem:=Me)).ToArray '触发条件在这里初始化完毕，在接下来的代码中则仅需要根据Handle值连接动作模型即可
            For Each Trigger In _Triggers
                Dim Handle = Trigger.Handle
                Dim Experiment = ExperimentModels(Handle)
                Trigger._strCondition = String.Format("[{0}]     {1} ==> {2}", Handle, Experiment.TriggedCondition, Experiment.TargetAction)

                Call SystemLogging.WriteLine(String.Format("    Initialize trigger done:  {0}", Trigger._strCondition), "", Type:=MSG_TYPES.INF)

                Dim ActionScript As String = Experiment.TargetAction.Replace("""""", """")
                Dim Actions As Action() = ScriptCompiler.GetActions(ActionScript)
                Trigger.Invoke = Function() TriggerInvokes(Actions)
            Next

            Call SystemLogging.WriteLine("TriggerSystem initialize job completed!", "", Type:=MSG_TYPES.INF)
            Call SystemLogging.WriteLine("")

            Return 0
        End Function

        Private Shared Function TriggerInvokes(ActionList As Action()) As Integer
            For i As Integer = 0 To ActionList.Count - 1
                Call ActionList(i)()
            Next
            Return 0
        End Function

        Public Shared Function TrimExpression(Expression As String) As String
            Dim sBuilder As StringBuilder = New StringBuilder(Expression.ToLower, 1024)
            Call sBuilder.Replace("and", "&&")
            Call sBuilder.Replace("or", "||")
            Call sBuilder.Replace("<>", "!=")

            Dim p As Integer = InStr(sBuilder.ToString, "(")
            Call sBuilder.Remove(0, p)
            p = InStrRev(sBuilder.ToString, ")") - 1
            Call sBuilder.Remove(p, 1)

            Return sBuilder.ToString.Trim.Replace("""""", """")
        End Function

        Public Function Tick(KernelCycle As Integer) As Integer Implements IDrivenable.__innerTicks
            Me._ComponentSource = New Object() {Me.I_RuntimeContainer, Me._CellSystem.ExpressionRegulationNetwork, Me._CellSystem.Metabolism, Me._CellSystem}
            Dim RunningTriggersLQuery = (From Trigger As EngineSystem.ObjectModels.ExperimentSystem.Triggers.ConditionalTrigger In Me._Triggers
                                         Where Trigger.TriggerTest = True
                                         Select Trigger.Invoke()).ToArray
            Return RunningTriggersLQuery.Sum
        End Function

        Public Overrides Sub MemoryDump(Dir As String)
            'Do NOTHING
        End Sub

        Public ReadOnly Property EventId As String Implements IDrivenable.EventId
            Get
                Return "Trigger Driver"
            End Get
        End Property
    End Class
End Namespace
