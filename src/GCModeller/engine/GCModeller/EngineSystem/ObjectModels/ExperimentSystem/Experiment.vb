#Region "Microsoft.VisualBasic::f850b8518ed23e40314656f3f4edfd6d, engine\GCModeller\EngineSystem\ObjectModels\ExperimentSystem\Experiment.vb"

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

    '     Class ExperimentManageSystem
    ' 
    ' 
    '         Class FactorVariables
    ' 
    '             Function: CreateObject, ToString
    ' 
    '             Sub: Invoke, Tick
    '             Structure SetMetaboliteAction
    ' 
    '                 Function: CreateObject
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports SMRUCC.genomics.GCModeller.Assembly

Namespace EngineSystem.ObjectModels.ExperimentSystem

    Partial Class ExperimentManageSystem

        ''' <summary>
        ''' 本类型的实验是通过控制代谢物的浓度来进行的，故而<see cref="FactorVariables.Target"></see>所指向的目标对象为<see cref="EngineSystem.ObjectModels.SubSystem.MetabolismCompartment.Metabolites"></see>
        ''' </summary>
        ''' <remarks></remarks>
        Public Class FactorVariables

            ''' <summary>
            ''' 顺序与<see cref="GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment.Types">Types</see>相对应：
            ''' Increase
            ''' Decrease
            ''' Multiplying
            ''' Decay
            ''' Mod
            ''' ChangeTo
            ''' </summary>
            ''' <remarks></remarks>
            Public Shared ReadOnly Methods As Func(Of Double, Double, Double)() =
                {
                    Function(Var As Double, Delta As Double) Var + Delta,
                    Function(Var As Double, Delta As Double) Var - Delta,
                    Function(var As Double, Delta As Double) var * Delta,
                    Function(var As Double, delta As Double) var / delta,
                    Function(var As Double, delta As Double) var Mod delta,
                    Function(Var As Double, Delta As Double) Delta}

            ''' <summary>
            ''' The name Id of the target.
            ''' (目标的名称)
            ''' </summary>
            ''' <remarks></remarks>
            <XmlAttribute> Public Id As String

            ''' <summary>
            ''' The start time of this disturb.
            ''' (这个干扰动作的开始时间)
            ''' </summary>
            ''' <remarks></remarks>
            <XmlAttribute> Public Start As Double
            ''' <summary>
            ''' The interval ticks between each kick.
            ''' (每次干扰动作执行的时间间隔)
            ''' </summary>
            ''' <remarks></remarks>
            <XmlAttribute> Public Interval As Double
            ''' <summary>
            ''' The counts of the kicks.
            ''' (执行的次数)
            ''' </summary>
            ''' <remarks></remarks>
            <XmlAttribute> Public Kicks As Integer
            <XmlAttribute> Public DisturbType As GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment.Types
            <XmlAttribute> Public Value As Double

            <XmlIgnore> Dim NextTime As Double

            ''' <summary>
            ''' Method for set the value for target 
            ''' </summary>
            ''' <remarks></remarks>
            Dim TargetInvoke_SetValue As Action(Of Double)
            ''' <summary>
            ''' Method for get the current value of target 
            ''' </summary>
            ''' <remarks></remarks>
            Dim TargetInvoke_GetValue As Func(Of Double)

            ''' <summary>
            ''' Target  
            ''' </summary>
            ''' <remarks></remarks>
            Public Target As EngineSystem.ObjectModels.Entity.Compound

            Dim _SystemLogging As LogFile
            Dim _SuppressPeriodicMessage As Boolean = False

            Public Sub Tick(RtTime As Integer)
                If RtTime >= NextTime Then
                    Call _SystemLogging.WriteLine(String.Format("Start experiment on {0}: {0} {1} {2}", Target.Identifier, Me.DisturbType.ToString, Value), "ExperimentSystem->TICK()", Type:=MSG_TYPES.INF)
                    Call TargetInvoke_SetValue(Methods(DisturbType)(TargetInvoke_GetValue(), Value))
                    NextTime = Interval + RtTime
                    Kicks -= 1
                End If
            End Sub

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ModelBase">目标对象之中的<see cref="GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment.TriggedCondition">触发条件</see>为一个纯数字</param>
            ''' <param name="MetabolismSystem"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function CreateObject(ModelBase As GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment, MetabolismSystem As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment) As FactorVariables
                If Not Regex.Match(ModelBase.TriggedCondition.Trim, "^\d+$", RegexOptions.Multiline).Success Then  '目标对象不是一个数字，则说明可能是条件触发类型的
                    Return Nothing
                End If

                Dim ARGV = SetMetaboliteAction.CreateObject(Microsoft.VisualBasic.CommandLine.TryParse(ModelBase.TargetAction)("-set_metabolite"))
                Dim PeriodicBehaviors = Microsoft.VisualBasic.CommandLine.TryParse(ModelBase.PeriodicBehavior)
                Dim ExperimentObject As FactorVariables = New FactorVariables With {.DisturbType = ARGV.DisturbingType, .Id = ARGV.Metabolite, .Interval = PeriodicBehaviors("-interval"),
                                                                                    .Kicks = PeriodicBehaviors("-ticks"), .Start = Val(ModelBase.TriggedCondition),
                                                                                 .Value = ARGV.value, ._SystemLogging = MetabolismSystem.SystemLogging}
                Dim Compound = MetabolismSystem.Metabolites.GetItem(ARGV.Metabolite.ToUpper)
                If Compound Is Nothing Then
                    Call MetabolismSystem.SystemLogging.WriteLine("OBJECT_NOT_FOUND, your data contains error: could not found target compound object base on its unique-id.", "Experiment->CreateObject()", Type:=MSG_TYPES.ERR)
                    Return Nothing
                Else
                    ExperimentObject.Target = Compound
                    ExperimentObject.NextTime = ExperimentObject.Start
                    ExperimentObject.TargetInvoke_GetValue = Function() Compound.DataSource.value
                    ExperimentObject.TargetInvoke_SetValue = Sub(value As Double) Compound.Quantity = value
                End If

                Return ExperimentObject
            End Function

            Public Shared Sub Invoke(Model As SetMetaboliteAction, MetabolismSystem As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
                Dim Compound = MetabolismSystem.Metabolites.GetItem(Model.Metabolite.ToUpper)

                If Compound Is Nothing Then
                    Call MetabolismSystem.SystemLogging.WriteLine("OBJECT_NOT_FOUND, your data contains error: could not found target compound object base on its unique-id.", "Experiment->CreateObject()", Type:=MSG_TYPES.ERR)
                Else
                    Dim ExperimentObject As FactorVariables = New FactorVariables With {.DisturbType = Model.DisturbingType, .Id = Model.Metabolite, .Value = Model.value, ._SystemLogging = MetabolismSystem.SystemLogging}

                    ExperimentObject.Target = Compound
                    ExperimentObject.TargetInvoke_GetValue = Function() Compound.DataSource.value
                    ExperimentObject.TargetInvoke_SetValue = Sub(value As Double) Compound.Quantity = value

                    Call ExperimentObject.TargetInvoke_SetValue(Methods(ExperimentObject.DisturbType)(ExperimentObject.TargetInvoke_GetValue(), ExperimentObject.Value))
                End If
            End Sub

            Public Structure SetMetaboliteAction
                Dim Metabolite As String, value As Double
                Dim DisturbingType As GCMarkupLanguage.GCML_Documents.ComponentModels.Experiment.Types

                Public Shared Function CreateObject(ActionScript As String) As SetMetaboliteAction
                    Dim Tokens As String() = Strings.Split(ActionScript, "=>")
                    Dim ARGV = Microsoft.VisualBasic.CommandLine.TryParse("p " & Tokens.Last.Trim)
                    Return New SetMetaboliteAction With {.Metabolite = Tokens.First.Trim.ToUpper, .value = Val(ARGV("-value")), .DisturbingType = Val(ARGV("-type"))}
                End Function
            End Structure

            Public Overrides Function ToString() As String
                Return Target
            End Function
        End Class
    End Class
End Namespace
