#Region "Microsoft.VisualBasic::c204ce452087e28df3bc1db32db57438, ..\sciBASIC.ComputingServices\ComputingServices\Taskhost.d\Object\ObjectAddress.vb"

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

Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TaskHost

    ''' <summary>
    ''' Object reference address on the remote
    ''' </summary>
    Public Class ObjectAddress : Implements IDisposable

        Public Property TypeId As String
            Get
                Return _typeId
            End Get
            Set(value As String)
                _typeId = value
                Call __initHashCode()
            End Set
        End Property

        ''' <summary>
        ''' <see cref="Object.GetHashCode"/> from local
        ''' </summary>
        ''' <returns></returns>
        Public Property HashCode As String
            Get
                Return _hashCode
            End Get
            Set(value As String)
                _hashCode = value
                Call __initHashCode()
            End Set
        End Property

        Dim _typeId As String
        Dim _hashCode As String
        Dim __md5 As String
        Dim __hash As Long

        Private Sub __initHashCode()
            __md5 = SecurityString.GetMd5Hash(_typeId & _hashCode)
            __hash = SecurityString.ToLong(__md5)
        End Sub

        Public Function ReferenceAddress() As Long
            Return __hash
        End Function

        ''' <summary>
        ''' Object is equals on the remote and local
        ''' </summary>
        ''' <param name="addr"></param>
        ''' <returns></returns>
        Public Overloads Function ReferenceEquals(addr As ObjectAddress) As Boolean
            Return __hash = addr.__hash
        End Function

        Public Overloads Function ReferenceEquals(obj As Object) As Boolean
            Dim addr As ObjectAddress = ObjectAddress.AddressOf(obj)
            Return __hash = addr.__hash
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function [AddressOf](x As Object) As ObjectAddress
            Return New ObjectAddress With {
                .HashCode = x.GetHashCode,
                .TypeId = x.GetType.FullName
            }
        End Function

        Public Shared Operator =(a As ObjectAddress, b As ObjectAddress) As Boolean
            Return a.__hash = b.__hash
        End Operator

        Public Shared Operator <>(a As ObjectAddress, b As ObjectAddress) As Boolean
            Return a.__hash <> b.__hash
        End Operator

        Public Shared Operator =(a As ObjectAddress, b As Object) As Boolean
            Return a.__hash = [AddressOf](b).__hash
        End Operator

        Public Shared Operator <>(a As ObjectAddress, b As Object) As Boolean
            Return a.__hash <> [AddressOf](b).__hash
        End Operator

        Public Shared Operator =(a As Object, b As ObjectAddress) As Boolean
            Return [AddressOf](a).__hash = b.__hash
        End Operator

        Public Shared Operator <>(a As Object, b As ObjectAddress) As Boolean
            Return [AddressOf](a).__hash <> b.__hash
        End Operator

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
