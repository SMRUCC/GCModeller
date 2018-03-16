#Region "Microsoft.VisualBasic::1f149f1b3d2520da541b687a697deb33, engine\GCModeller\EngineSystem\ObjectModels\ExperimentSystem\TriggerSystem\vbc\Prefix.vb"

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

    ' Class Prefix
    ' 
    '     Properties: PrefixTypeReference
    '     Class ActionScript
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ChangeMetabolite, GeneMutations, GetActions, MemoryDump
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
''' <summary>
''' 预设制的一些系统类型
''' </summary>
''' <remarks></remarks>
Public Class Prefix

    Public Shared ReadOnly Property PrefixTypeReference As KeyValuePair(Of String, Type)() =
        New KeyValuePair(Of String, Type)() {
            New KeyValuePair(Of String, Type)(key:="kernel", value:=GetType(EngineSystem.Engine.GCModeller)),
            New KeyValuePair(Of String, Type)(key:="transcription", value:=GetType(EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)),
            New KeyValuePair(Of String, Type)(key:="metabolism", value:=GetType(EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)),
            New KeyValuePair(Of String, Type)(key:="cell", value:=GetType(EngineSystem.ObjectModels.SubSystem.CellSystem))}

    Public Class ActionScript

        Dim EngineKernel As EngineSystem.Engine.GCModeller

        Sub New(Kernel As EngineSystem.Engine.GCModeller)
            EngineKernel = Kernel
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="argvs"><see cref="EngineSystem.Engine.GCModeller.MemoryDump">DUMP数据</see>的存放文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function MemoryDump(argvs As String) As Action
            Dim Dir As String = If(String.Equals(argvs, "/cache", StringComparison.OrdinalIgnoreCase), Settings.DataCache, argvs)
            Try
                Call FileIO.FileSystem.CreateDirectory(Dir)
            Catch ex As Exception
                Call EngineKernel.SystemLogging.WriteLine(String.Format("Could not create directory at location ""{0}""{1}{2}", Dir, vbCrLf, ex.ToString), "MemoryDump(ARGV As String) As Action", Type:=MSG_TYPES.ERR)
                Dir = Settings.DataCache
            End Try

            Dim Action As Action = Sub() Call EngineKernel.MemoryDump(String.Format("{0}/MEMORY_DUMP_{1}/", Dir, LogFile.NowTimeNormalizedString))
#If DEBUG Then
            Action = Sub() Console.WriteLine("Trigger Start!")
#End If
            Return Action
        End Function

        Private Function GeneMutations(ARGV As String) As Action
            Dim Mutations = EngineSystem.ObjectModels.ExperimentSystem.Mutations.TryParse(ARGV)
            Dim Action As Action = Sub() Call Mutations.ApplyMutation(EngineKernel.KernelModule)
            Return Action
        End Function

        Private Function ChangeMetabolite(ARGV As String) As Action
            Dim Model = EngineSystem.ObjectModels.ExperimentSystem.ExperimentManageSystem.FactorVariables.SetMetaboliteAction.CreateObject(ARGV)
            Dim Action As Action = Sub() Call EngineSystem.ObjectModels.ExperimentSystem.ExperimentManageSystem.FactorVariables.Invoke(Model, EngineKernel.KernelModule.Metabolism)
            Return Action
        End Function

        Dim PreifxActions As Dictionary(Of String, Func(Of String, Action)) = New Dictionary(Of String, Func(Of String, Action)) From
            {
                {"-gene_mutations", AddressOf GeneMutations},
                {"-set_metabolite", AddressOf ChangeMetabolite},
                {"-memory_dump", AddressOf MemoryDump}}

        Public Function GetActions(Script As String) As Action()
            Dim ARGV = Microsoft.VisualBasic.CommandLine.TryParse(Script).ToArray
            Dim ActionsLQuery As Action() =
                LinqAPI.Exec(Of Action) <= From token As NamedValue(Of String)
                                           In ARGV
                                           Let Action As Action = PreifxActions(token.Name)(arg:=token.Value)
                                           Select Action
            Return ActionsLQuery
        End Function
    End Class
End Class
