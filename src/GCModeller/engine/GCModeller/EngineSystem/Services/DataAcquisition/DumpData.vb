#Region "Microsoft.VisualBasic::b1b802894a180314fc3aac4fe6019458, engine\GCModeller\EngineSystem\Services\DataAcquisition\DumpData.vb"

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

    '     Class ManageSystem
    ' 
    ' 
    '         Class DumpData
    ' 
    '             Properties: Invalid
    ' 
    '             Constructor: (+1 Overloads) Sub New
    ' 
    '             Function: GetTrigger, Invoke
    ' 
    '             Sub: InitializeTrigger, WriteDumpData
    ' 
    ' 
    ' 
    '     Class TriggerSystem
    ' 
    '         Properties: EventId, SystemLogging
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Initialize, Tick
    ' 
    '         Sub: MemoryDump, PendingTrigger
    '         Class Trigger
    ' 
    '             Function: Check, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer

Namespace EngineSystem.Services.DataAcquisition

    Partial Class ManageSystem

        Public Class DumpData

            Dim ManageSystem As ManageSystem
            Dim _DumpFile As String
            Dim DumpTime As String

            Sub New(ManageSystem As ManageSystem, DumpTime As String, DumpFileDir As String)
                Me.ManageSystem = ManageSystem
                Me._DumpFile = DumpFileDir
                Me.DumpTime = DumpTime
            End Sub

            Public ReadOnly Property Invalid As Boolean
                Get
                    Return String.IsNullOrEmpty(_DumpFile) AndAlso String.IsNullOrEmpty(DumpTime)
                End Get
            End Property

            Public Function GetTrigger() As TriggerSystem.Trigger
                Dim SystemDuration As Integer = ManageSystem.Kernel.KernelProfile.KernelLoops
                Dim Trigger As TriggerSystem.Trigger = Nothing

                If DumpTime.First() = "~"c Then 'Dump系统的最后指定秒数的所有数据
                    Trigger = New TriggerSystem.Trigger With {.TerminateTime = SystemDuration, .StartTime = SystemDuration - Val(Mid(DumpTime, 2))}
                    If Trigger.StartTime < 0 Then
                        Call Me.ManageSystem.SystemLogging.WriteLine("NEGATIVE_TRIGGER_START_POINT: " & Me.DumpTime & " <=0; start point was set to ZERO.", "DumpData::GetTrigger()", Type:=MSG_TYPES.WRN)
                        Trigger.StartTime = 0
                    End If
                ElseIf Regex.Match(Me.DumpTime, "^\d+-->\d+$").Success Then
                    Dim Tokens As String() = Strings.Split(Me.DumpTime, "-->")
                    Trigger = New TriggerSystem.Trigger With {.TerminateTime = Val(Tokens.Last), .StartTime = Val(Tokens.First)}
                Else

                End If
                Trigger.Handle = AddressOf Me.Invoke
                Call InitializeTrigger()

                Return Trigger
            End Function

            Private Sub InitializeTrigger()
                ChunkBuffer = New Dictionary(Of String, List(Of DataFlowF))

                For Each [Interface] In ManageSystem.DataAcquisitionServices
                    Dim Heads = [Interface].GetDefinitions
                    Call ChunkBuffer.Add([Interface].TableName, New List(Of DataFlowF))
                Next
            End Sub

            Dim ChunkBuffer As Dictionary(Of String, List(Of DataFlowF))

            Private Function Invoke(RTime As Integer) As Integer
                For Each [Interface] In ManageSystem.DataAcquisitionServices
                    Dim DataChunk = ChunkBuffer([Interface].TableName)
                    Call DataChunk.AddRange([Interface].GetDataChunk)
                Next
                Return 0
            End Function

            Public Sub WriteDumpData()
                For Each [Interface] In ManageSystem.DataAcquisitionServices
                    Dim [Handles] = [Interface].GetDefinitions
                    Dim Stream = New DataSerializer.Csv(String.Format("{0}/{1}.csv", Me._DumpFile, [Interface].TableName)) With {._DataFlows = ChunkBuffer([Interface].TableName).AsList}
                    Call Stream.CreateHandle([Handles], [Interface].TableName)
                    Call Stream.Close("")
                Next
            End Sub
        End Class
    End Class

    Public Class TriggerSystem : Inherits EngineSystem.ObjectModels.SubSystem.SystemObjectModel
        Implements IDrivenable

        Dim RuntimeContainer As IContainerSystemRuntimeEnvironment
        Dim RunningTriggers As List(Of Trigger)
        Dim PendingTriggers As List(Of Trigger)

        Sub New(ModellerKernel As Engine.GCModeller)
            Me.RuntimeContainer = ModellerKernel
        End Sub

        Protected Friend Overrides ReadOnly Property SystemLogging As LogFile
            Get
                Return RuntimeContainer.SystemLogging
            End Get
        End Property

        Public Overrides Function Initialize() As Integer
            RunningTriggers = New List(Of Trigger)
            PendingTriggers = New List(Of Trigger)
            Return 0
        End Function

        Public Sub PendingTrigger(Trigger As Trigger)
            Call Me.PendingTriggers.Add(Trigger)
        End Sub

        Public Function Tick(KernelCycle As Integer) As Integer Implements IDrivenable.__innerTicks
            Dim RTime As Integer = RuntimeContainer.RuntimeTicks
            Dim CheckTriggerStatusLQuery As Trigger() = (From Trigger In RunningTriggers Where Trigger.Check(RTime) = False Select Trigger).ToArray
            For Each Trigger In CheckTriggerStatusLQuery
                Call RunningTriggers.Remove(Trigger)
            Next

            CheckTriggerStatusLQuery = (From Trigger As Trigger In PendingTriggers Where Trigger.Check(RTime) = True Select Trigger).ToArray
            For Each Trigger In CheckTriggerStatusLQuery
                Call PendingTriggers.Remove(Trigger)
            Next
            Call RunningTriggers.AddRange(CheckTriggerStatusLQuery)

            Dim TriggerRunningLQuery = (From Trigger In RunningTriggers Select Trigger.Handle.Invoke(RTime)).ToArray.Sum
            Return TriggerRunningLQuery
        End Function

        Public Class Trigger
            Public StartTime, TerminateTime As Integer

            Public Function Check(RTime As Integer) As Boolean
                Return RTime >= StartTime AndAlso RTime <= TerminateTime
            End Function

            Public Handle As System.Func(Of Integer, Integer)

            Public Overrides Function ToString() As String
                Return String.Format("{0} --> {1}", StartTime, TerminateTime)
            End Function
        End Class

        Public Overrides Sub MemoryDump(Dir As String)

        End Sub

        Public ReadOnly Property EventId As String Implements IDrivenable.EventId
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class
End Namespace
