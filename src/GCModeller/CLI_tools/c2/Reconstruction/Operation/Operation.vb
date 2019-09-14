#Region "Microsoft.VisualBasic::af000306f4e071523b68fed2317f86cd, CLI_tools\c2\Reconstruction\Operation\Operation.vb"

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

    '     Class Operation
    ' 
    '         Properties: HomologousProteins, LocalBLAST, Reconstructed, Subject, Workspace
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Performance
    ' 
    '         Sub: (+2 Overloads) Dispose
    '         Class ReconstructedList
    ' 
    '             Properties: ListData
    ' 
    '             Function: GetEntity, GetEnumerator, GetEnumerator1, ToString, ToTable
    ' 
    '             Sub: Add
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Extensions

Namespace Reconstruction

    ''' <summary>
    ''' 模型数据库的重建方法
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class Operation : Implements System.IDisposable

        Protected Friend WorkSession As c2.Reconstruction.Operation.OperationSession

        ''' <summary>
        ''' 目标待重建的MetaCyc数据库
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend ReadOnly Property Reconstructed As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
            Get
                Return WorkSession.ReconstructedMetaCyc
            End Get
        End Property

        ''' <summary>
        ''' 重建待重建目标数据库的参考数据源
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend ReadOnly Property Subject As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
            Get
                Return WorkSession.Subject
            End Get
        End Property

        ''' <summary>
        ''' 获取两个基因组之间同源的蛋白质列表，这个列表是一系列操作的基础 {Reconstructed, Subject}
        ''' </summary>
        ''' <value></value>
        ''' <returns>{Reconstructed, Subject}</returns>
        ''' <remarks></remarks>
        Protected Friend ReadOnly Property HomologousProteins As OperationSession.HomologousProteinsF
            Get
                Return WorkSession.HomologousProteins
            End Get
        End Property

        ''' <summary>
        ''' 获取用于重建工作的临时文件夹
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overridable ReadOnly Property Workspace As String
            Get
                Return WorkSession.WorkDir
            End Get
        End Property

        Protected Friend ReadOnly Property LocalBLAST As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.InteropService
            Get
                Return WorkSession.LocalBLAST
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Session">本次重构操作的会话空间</param>
        ''' <remarks></remarks>
        Sub New(Session As c2.Reconstruction.Operation.OperationSession)
            Me.WorkSession = Session
        End Sub

        ''' <summary>
        ''' 调用这个重建操作执行具体的重建操作过程
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Performance() As Integer
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' {Subject_Entity_UniqueId, Reconstructed_Entity}
        ''' 即Subject数据库中的UniqueId所指向的对象是与程序所重建出来的对象是相同的
        ''' </summary>
        ''' <typeparam name="TEntity"></typeparam>
        ''' <remarks></remarks>
        Public Class ReconstructedList(Of TEntity As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object)
            Implements Generic.IEnumerable(Of KeyValuePair(Of String, TEntity))

            ''' <summary>
            ''' {Subject_Entity_UniqueId, Reconstructed_Entity}
            ''' </summary>
            ''' <remarks></remarks>
            Dim _ListData As List(Of KeyValuePair(Of String, TEntity)) = New List(Of KeyValuePair(Of String, TEntity))

            Public ReadOnly Property ListData As List(Of KeyValuePair(Of String, TEntity))
                Get
                    Return _ListData
                End Get
            End Property

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="Id">Subject Entity UniqueId</param>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Default Public ReadOnly Property Entity(Id As String) As TEntity
                Get
                    Dim LQuery = From Item In _ListData Where String.Equals(Id, Item.Key) Select Item.Value '
                    Dim Result = LQuery.ToArray
                    If Result.IsNullOrEmpty Then
                        Return Nothing
                    Else
                        Return Result.First
                    End If
                End Get
            End Property

            ''' <summary>
            ''' 通过键值对右边的实体对象的UniqueId来查找出对应的实体
            ''' </summary>
            ''' <param name="UniqueId"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetEntity(UniqueId As String) As TEntity
                Dim LQuery = From Item In _ListData Where String.Equals(UniqueId, Item.Value.Identifier) Select Item.Value  '
                Dim Result = LQuery.ToArray
                If Result.IsNullOrEmpty Then
                    Return Nothing
                Else
                    Return Result.First
                End If
            End Function

            Public Sub Add(SubjectId As String, Entity As TEntity)
                Call _ListData.Add(New KeyValuePair(Of String, TEntity)(SubjectId, Entity))
            End Sub

            Public Overrides Function ToString() As String
                Return String.Format("List(Of {0})::{1} Items.", GetType(TEntity).FullName, _ListData.Count)
            End Function

            Public Function ToTable() As List(Of TEntity)
                Dim LQuery = From Item In _ListData Select Item.Value '
                Return LQuery.ToList
            End Function

            Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, TEntity)) Implements IEnumerable(Of KeyValuePair(Of String, TEntity)).GetEnumerator
                For i As Integer = 0 To _ListData.Count - 1
                    Yield _ListData(i)
                Next
            End Function

            Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
                Yield GetEnumerator()
            End Function
        End Class

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose(ByVal disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose(ByVal disposing As Boolean)中。
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

    End Class
End Namespace
