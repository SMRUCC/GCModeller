Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.RQL.Linq

Namespace API

    ''' <summary>
    ''' RQL Client API.
    ''' </summary>
    ''' <remarks>
    ''' 数据对象在申明创建之后并没有立即执行，而是在调用迭代器之后才会执行查询
    ''' </remarks>
    Public Class Repository(Of T)
        Implements IDisposable
        Implements IEnumerable(Of T)

        Public ReadOnly Property Repository As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source">
        ''' URL, examples as:  http://Linq.gcmodeller.org/kegg/ssdb
        ''' 
        ''' Where, the remote server http://Linq.gcmodeller.org implements the RQL services.
        ''' And the repository resource name on the server is "/kegg/ssdb"
        ''' </param>
        Sub New(source As String)
            Repository = source
        End Sub

        Public Overrides Function ToString() As String
            Return Repository
        End Function

        ''' <summary>
        ''' 在这里会打开一个新的查询
        ''' </summary>
        ''' <param name="assertions">断言</param>
        ''' <returns></returns>
        Public Function Where(assertions As String) As LinqEntry
            Return __innerQuery(Repository & "?where=" & assertions)
        End Function

        Public Function Where(predication As Func(Of T, Boolean)) As LinqEntry

        End Function

        Private Shared Function __innerQuery(url As String) As LinqEntry
            Dim source As String = url.GetRequest
            Dim linq As LinqEntry = source.LoadObject(Of LinqEntry)
            Return linq
        End Function

#Region "Implements IEnumerable(Of T)"

        Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For Each x As T In __innerQuery(Repository).AsLinq(Of T)
                If Not disposedValue Then
                    Yield x
                Else
                    Exit For
                End If
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

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

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
