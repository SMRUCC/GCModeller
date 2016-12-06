#Region "Microsoft.VisualBasic::51af9c32d7498efa3d875f331751ecc8, ..\ComputingServices\SharedMemory\MemoryServices.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SharedMemory

    ''' <summary>
    ''' Shared the memory with the remote machine.
    ''' </summary>
    Public Class MemoryServices : Implements IDisposable
        Implements IObjectModel_Driver

        ''' <summary>
        ''' Gets the memory data from remote machine.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="name">建议使用NameOf来设置或者获取参数值</param>
        ''' <returns></returns>
        Public Function GetValue(Of T)(name As String) As T
            Return __remote.ReadValue(Of T)(name)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="name">建议使用NameOf来设置或者获取参数值</param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function SetValue(Of T)(name As String, value As T) As Boolean
            Return __remote.WriteValue(name, value)
        End Function

        Public Function [TypeOf](name As String) As Type
            Return __remote.TypeOf(name)
        End Function

        Public Function IsCompatible(Of T)(name As String, x As T) As Boolean
            Dim type As Type = [TypeOf](name)
            Dim ref As Type = GetType(T)
            Return type.Equals(ref) OrElse ref.IsInheritsFrom(type)
        End Function

        ReadOnly __remote As IPEndPoint
        ReadOnly __localSvr As SharedSvr

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="remote"></param>
        ''' <param name="local">local services port that provides to the remote</param>
        Sub New(remote As IPEndPoint, local As Integer)
            __remote = remote
            __localSvr = New SharedSvr(local)
        End Sub

        Public Function Allocate(name As String, value As Object, Optional [overrides] As Boolean = False) As Boolean
            Return __localSvr.Allocate(name, value, [overrides])
        End Function

        Public Overrides Function ToString() As String
            Return __remote.GetJson
        End Function

        Public Function Run() As Integer Implements IObjectModel_Driver.Run
            Return __localSvr.Run
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call __localSvr.Dispose()
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
