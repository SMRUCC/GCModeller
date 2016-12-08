#Region "Microsoft.VisualBasic::75db6bc17ee32233819855d22bc572d8, ..\sciBASIC.ComputingServices\LINQ\LINQ\Script\Runtime.vb"

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

Imports System.Dynamic
Imports sciBASIC.ComputingServices.Linq.Framework
Imports sciBASIC.ComputingServices.Linq.Framework.ObjectModel
Imports sciBASIC.ComputingServices.Linq.Framework.Provider
Imports sciBASIC.ComputingServices.Linq.Framework.Provider.ImportsAPI
Imports sciBASIC.ComputingServices.Linq.LDM.Statements

Namespace Script

    ''' <summary>
    ''' LINQ脚本数据源查询运行时环境
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DynamicsRuntime : Inherits DynamicObject
        Implements IDisposable

        Dim _varsHash As Dictionary(Of String, Variable) = New Dictionary(Of String, Variable)

        Public ReadOnly Property Types As TypeRegistry
        Public ReadOnly Property Compiler As DynamicCode.DynamicCompiler
        Public ReadOnly Property API As APIProvider

        Sub New(entity As TypeRegistry, api As APIProvider)
            Me.API = api
            Me.Types = entity
            Me.Compiler = New DynamicCode.DynamicCompiler(entity, api)
        End Sub

        Sub New()
            Call Me.New(TypeRegistry.LoadDefault, APIProvider.LoadDefault)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="script"></param>
        ''' <returns>
        ''' If the Return statement is presents, then the variable of the returns will be return from the function, and this is a Function in VisualBasic 
        ''' If not, then viod value will be returns, and this is a Sub in VisualBasic
        ''' </returns>
        Public Function Evaluate(script As String) As IEnumerable
            Return New Script(script, Me).Evaluate
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Public Function GetResource(source As String) As IEnumerable
            If String.IsNullOrEmpty(source) Then
                Throw New Exception("source name can not be null!")
            End If

            Dim key As String = source.ToLower

            If _varsHash.ContainsKey(key) Then
                Return _varsHash(key).Data
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="source"></param>
        ''' <remarks></remarks>
        Public Sub SetObject(name As String, source As IEnumerable)
            Dim var As New Variable With {
                .Data = source,
                .Name = name
            }
            _varsHash += var
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} variables in the LINQ runtime.", _varsHash.Count)
        End Function

        ''' <summary>
        ''' 执行一个LINQ查询脚本文件
        ''' </summary>
        ''' <param name="path">LINQ脚本文件的文件路径</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 脚本要求：
        ''' Imports Namespace
        ''' var result = &lt;Linq>
        ''' </remarks>
        Public Function Source(path As String) As IEnumerable
            Return Evaluate(IO.File.ReadAllText(path))
        End Function

        ''' <summary>
        ''' Execute a compiled LINQ statement object model to query a object-orientale database.
        ''' </summary>
        ''' <param name="statement"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Dim List As List(Of Object) = New List(Of Object)
        ''' 
        ''' For Each [Object] In LINQ.GetCollection(Statement)
        '''    Call SetObject([Object])
        '''    If True = Test() Then
        '''        List.Add(SelectConstruct())
        '''    End If
        ''' Next
        ''' Return List.ToArray
        ''' </remarks>
        Public Function EXEC(statement As LinqStatement) As IEnumerable
            Using linq As ObjectModel.Linq = __new(statement)
                Return linq.EXEC
            End Using
        End Function

        Private Function __new(statement As LinqStatement) As ObjectModel.Linq
            Return If(statement.source.IsParallel, New ParallelLinq(statement, Me), New ObjectModel.Linq(statement, Me))
        End Function

        ''' <summary>
        ''' 执行单条查询表达式
        ''' </summary>
        ''' <param name="linq"></param>
        ''' <returns></returns>
        Public Function EXEC(linq As String) As IEnumerable
            Return EXEC(LinqStatement.TryParse(linq, Types))
        End Function
    End Class
End Namespace
