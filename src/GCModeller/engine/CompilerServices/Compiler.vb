#Region "Microsoft.VisualBasic::6874ed4fca48cd5bbb9c469eb2bd9e57, engine\CompilerServices\Compiler.vb"

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

    ' Class Compiler
    ' 
    '     Properties: [Return], CompileLogging, Version
    ' 
    '     Function: Compile, Link, PreCompile, ToString, WriteLog
    '               WriteProperty
    ' 
    '     Sub: (+2 Overloads) Dispose, Initialize
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Model file of class type <see cref="ModelBaseType"></see> compiler.
''' </summary>
''' <typeparam name="TModel"></typeparam>
''' <remarks></remarks>
Public MustInherit Class Compiler(Of TModel As ModelBaseType)
    Implements IDisposable

    Protected Friend m_compiledModel As TModel
    Protected Friend m_logging As LogFile

    Public Overridable ReadOnly Property Version As Version
        Get
            Return My.Application.Info.Version
        End Get
    End Property

    Public ReadOnly Property CompileLogging As LogFile
        Get
            Return m_logging
        End Get
    End Property

    Public Overridable ReadOnly Property [Return] As TModel
        Get
            Return m_compiledModel
        End Get
    End Property

    Protected Overridable Sub Initialize(args As CommandLine)
        m_logging = (args("--log") Or $"{App.LocalDataTemp}/v2MarkupCompiler.{LogFile.NowTimeNormalizedString}.log") _
            .DoCall(Function(logpath)
                        Return New LogFile(logpath, autoFlush:=False, append:=False)
                    End Function)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="args">
    ''' Property definition parameters for <see cref="ModelBaseType.properties"></see>, the override function of 
    ''' this mustOverride method should call method <see cref="WriteProperty"></see> to write the property into the 
    ''' compiled model file.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Compile(Optional args As CommandLine = Nothing) As TModel
        Dim errorCode As Value(Of Integer) = 0

        If args Is Nothing Then
            args = New CommandLine
        End If

        Call Initialize(args)
        Call m_logging.WriteLine($"pre-compile target model {GetType(TModel).FullName}")

        If (errorCode = PreCompile(args)) <> 0 Then
            Call m_logging.WriteLine("pre-compile with error code: " & errorCode.Value)
            Return Nothing
        End If

        Call m_logging.WriteLine("run compiler process!")

        If (errorCode = CompileImpl(args)) <> 0 Then
            Call m_logging.WriteLine("run model compiler with error code: " & errorCode.Value)
            Return Nothing
        End If

        Call m_logging.WriteLine("link model components.")

        If (errorCode = Link()) <> 0 Then
            Call m_logging.WriteLine("link model component with error code: " & errorCode.Value)
            Return Nothing
        End If

        Call m_logging.WriteLine(m_compiledModel.ToString)
        Call m_logging.WriteLine("model compile success!")

        Return m_compiledModel
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="args"><see cref="CommandLine.cli"></see></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function PreCompile(args As CommandLine) As Integer
        Return 0
    End Function

    Protected MustOverride Function CompileImpl(args As CommandLine) As Integer

    Protected Overridable Function Link() As Integer
        Return 0
    End Function

    Protected Function WriteProperty(args As CommandLine, model As TModel) As TModel
        Call m_logging.WriteLine(vbCrLf & "Write model property into the compiled model file.")

        If model.properties Is Nothing Then _
           model.properties = New [Property]

        If String.IsNullOrEmpty(model.properties.guid) Then
            model.properties.guid = Guid.NewGuid.ToString
        End If
        If String.IsNullOrEmpty(model.properties.compiled) Then
            model.properties.compiled = Now.ToString
        End If
        If model.properties.reversion = 0 Then
            model.properties.reversion = 1
        End If
        If model.properties.URLs.IsNullOrEmpty Then
            model.properties.URLs = New List(Of String) From {
                "http://gcmodeller.org/"
            }
        Else
            Call model.properties.URLs.Add("http://gcmodeller.org/")
        End If

        If model.properties.authors.IsNullOrEmpty Then
            model.properties.authors = New List(Of String) From {
                "SMRUCC.genomics.GCModeller"
            }
        End If

        If Not args Is Nothing Then
            ' 请先使用If判断是否为空，因为不知道本方法的调用顺序，不使用if判断可能会丢失已经在调用之前就写入的属性数据

        End If

        Return model
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("ICompiler: {0}, version: {1}", GetType(Compiler(Of TModel)).FullName, Version.ToString)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 检测冗余的调用

    ' IDisposable
    Protected Overridable Sub Dispose(Disposing As Boolean)
        If Not Me.disposedValue Then
            If Disposing Then
                ' TODO:  释放托管状态(托管对象)。
                Call WriteLog()
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

    Public Function WriteLog() As Boolean
        If Not m_logging Is Nothing Then
            Call m_logging.Dispose()
        End If

        Return True
    End Function
End Class
