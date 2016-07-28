#Region "Microsoft.VisualBasic::d0e794bf7789e8713a9dd86e54d94313, ..\ComputingServices\Taskhost.d\Object\MemoryHash.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Namespace TaskHost

    Public Class MemoryHash : Implements IDisposable

        ReadOnly __innerHash As New SortedDictionary(Of Long, Object)
        ReadOnly __innerAddr As New SortedDictionary(Of Long, ObjectAddress)

        Public Function GetObject(addr As Long) As Object
            If __innerHash.ContainsKey(addr) Then
                Return __innerHash(addr)
            Else
                Throw New NullReferenceException($"Address {addr} point to a invalid memory location!")
            End If
        End Function

        Public Function GetObject(addr As ObjectAddress) As Object
            Return GetObject(addr.ReferenceAddress)
        End Function

        Public Function IsNull(p As Long) As Boolean
            Return Not __innerHash.ContainsKey(p)
        End Function

        Public Function SetObject(obj As Object) As ObjectAddress
            Dim addr As ObjectAddress = ObjectAddress.AddressOf(obj)
            Dim p As Long = addr.ReferenceAddress

            If __innerHash.ContainsKey(p) Then    ' 将属性值复制给当前内存之中的对象
                Call ShadowsCopy.ShadowsCopy(obj, __innerHash(p), Me)
            Else
                Call __innerHash.Add(p, obj)
                Call __innerAddr.Add(p, addr)
            End If

            Return addr
        End Function

        Public Function GetAddress(addr As Long) As ObjectAddress
            If __innerAddr.ContainsKey(addr) Then
                Return __innerAddr(addr)
            Else
                Throw New NullReferenceException($"Address {addr} point to a invalid memory location!")
            End If
        End Function

        ''' <summary>
        ''' Gets the object reference type.
        ''' </summary>
        ''' <param name="addr"></param>
        ''' <returns></returns>
        Public Overloads Function [GetType](addr As Long) As Type
            Dim p As ObjectAddress = GetAddress(addr)
            Return System.Type.GetType(p.TypeId)
        End Function

        ''' <summary>
        ''' 按照内存指针销毁对象
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Public Function Destroy(p As Long) As Boolean
            If Not __innerHash.ContainsKey(p) Then
                Return False
            End If

            Dim obj As Object = __innerHash(p)
            Call __innerHash.Remove(p)
            Call GC.SuppressFinalize(obj)

            Dim addr As ObjectAddress = __innerAddr(p)
            Call __innerAddr.Remove(p)
            Call addr.Free

            Return True
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
