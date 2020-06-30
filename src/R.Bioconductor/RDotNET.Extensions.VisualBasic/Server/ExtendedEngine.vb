#Region "Microsoft.VisualBasic::4d9cd6e8f954c5c97738ed39ea78646d, RDotNET.Extensions.VisualBasic\Server\ExtendedEngine.vb"

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

    ' Class ExtendedEngine
    ' 
    '     Properties: [call]
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: __init, Evaluate, hasSlot, MyDefault
    ' 
    '     Sub: __cleanHook, Dispose, Reset
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Text
Imports RDotNET.Extensions.VisualBasic.API

Public Class ExtendedEngine : Inherits REngine

    ''' <summary>
    ''' Evaluates a R statement in the given string.
    ''' (由于这个函数并不返回数据，所以只推荐无返回值的方法调用或者变量初始化的时候是用本只写属性)
    ''' </summary>
    Public WriteOnly Property [call] As String
        Set(value As String)
            Call __logs.WriteLine(value)
            Call __logs.Flush()
            Call Evaluate(statement:=value)
        End Set
    End Property

    ''' <summary>
    ''' Language syntax supports for argument list.(将一个变量的结果值赋值给另外一个变量)
    ''' </summary>
    ''' <param name="name"></param>
    Default Public Property Assign(name As String) As ArgumentReference
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return New ArgumentReference With {
                .name = name
            }
        End Get
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Set(value As ArgumentReference)
            [call] = $"{name} <- {value.name}"
        End Set
    End Property

    ''' <summary>
    ''' 返回一个逻辑值类型的变量指针
    ''' </summary>
    ''' <param name="object$">An object from a formally defined class.</param>
    ''' <param name="name$">
    ''' The name of the slot. The operator takes a fixed name, which can be unquoted if it is syntactically a name in the language. 
    ''' A slot name can be any non-empty string, but if the name is not made up of letters, numbers, and ., it needs to be quoted 
    ''' (by backticks or single or double quotes).
    ''' In the case of the slot function, name can be any expression that evaluates to a valid slot in the class definition. 
    ''' Generally, the only reason to use the functional form rather than the simpler operator Is because the slot name has 
    ''' to be computed.</param>
    ''' <returns></returns>
    Public Function hasSlot(object$, name$) As String
        Dim var$ = RDotNetGC.Allocate

        SyncLock Me
            With Me
                .call = $"{var} <- .hasSlot({object$}, {name});"
            End With
        End SyncLock

        Return var
    End Function

    ' 20181211 将脚本日志文件放在系统数据文件夹, 会导致系统空间被过多占用
    ' 现修改为将脚本日志放置于当前的工作区之中
    Friend ReadOnly __logs As StreamWriter

    Sub New(id As String, dll As String)
        MyBase.New(id, dll)

        Dim logfile As String = ($"{App.CurrentDirectory}/.R_logs/{Now.ToNormalizedPathString}_pid={App.PID}_logs.R")
        __logs = logfile.OpenWriter(Encodings.UTF8)

        Call App.AddExitCleanHook(hook:=AddressOf __cleanHook)
        Call Me.GetType.Assembly.Location.__DEBUG_ECHO
    End Sub

    Public Overrides Function Evaluate(statement As String) As SymbolicExpression
        Try
            Return MyBase.Evaluate(statement)
        Catch ex As Exception
            ex = New Exception(vbCrLf & vbCrLf &
                               statement &
                               vbCrLf & vbCrLf, ex)
            Call App.LogException(ex)

            Throw ex
        End Try
    End Function

    Private Sub __cleanHook()
        Call __logs.WriteLine()
        Call __logs.WriteLine()
        Call __logs.WriteLine("# Show warnings():")
        Call __logs.WriteLine()
        Call __logs.WriteLine(Evaluate("str(warnings())").ToStrings.Select(Function(s) "# " & s).JoinBy(ASCII.LF))

        Call __logs.WriteLine()
        Call __logs.WriteLine($"#### ======================{App.PID} {App.CommandLine.ToString}===========================")
        Call __logs.Flush()
        Call __logs.Close()
        Call __logs.Dispose()

        Call "Execute R server logs clean job done!".__INFO_ECHO
    End Sub

    ''' <summary>
    ''' Reset current R environment
    ''' </summary>
    Public Sub Reset()
        ' rm(list=ls(all=TRUE))
        Call base.rm(list:=base.ls(allnames:=True))
        Call base.gc()
    End Sub

    Shared Sub New()
    End Sub

    Public Shared Function MyDefault() As [Default](Of  ExtendedEngine)
        Return RSystem.R.AsDefault
    End Function

    Friend Shared Function __init(id$, Optional dll$ = Nothing) As ExtendedEngine
        If id Is Nothing Then
            Throw New ArgumentNullException("id", "Empty ID is not allowed.")
        End If
        If id = String.Empty Then
            Throw New ArgumentException("Empty ID is not allowed.", "id")
        End If
        'if (instances.ContainsKey(id))
        '{
        '   throw new ArgumentException();
        '}

        Dim engine As New ExtendedEngine(id, dll:=ProcessRDllFileName(dll)) With {
            .AutoPrint = False
        }
        'instances.Add(id, engine);
        Return engine
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        ' 2019-03-19 RDotNet在销毁实例之后会改变当前的工作区到R_HOME
        ' 在这里改回来
        With App.CurrentDirectory
            Call __cleanHook()
            Call MyBase.Dispose(disposing)
            Call Directory.SetCurrentDirectory(.ByRef)
        End With
    End Sub
End Class
