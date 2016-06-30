Namespace DataModels.Files.XML

    Public Class DataModel : Implements GNU.Linux.VisualBasic.Compatibility.Data.Modelling.IHandler

        ''' <summary>
        ''' This datamodel object's position ID in a list object.
        ''' (本数据模型对象在一个列表对象之中的位置编号)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <System.Xml.Serialization.XmlIgnore>
        Public Property Handle As Long _
            Implements GNU.Linux.VisualBasic.Compatibility.Data.Modelling.IHandler.Handle

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

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
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

    End Class
End Namespace