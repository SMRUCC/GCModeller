#Region "Microsoft.VisualBasic::0317df4b284ac32f93d3b19b73f851d7, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\Provider\ImportsAPI\APIProvider.vb"

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

Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace Framework.Provider.ImportsAPI

    ''' <summary>
    ''' 只需要记住命名空间和对应的Assembly的引用就行了
    ''' </summary>
    Public Class APIProvider : Implements ISaveHandle
        Implements IDisposable

        Public Property Packages As ImportsNs()
            Get
                If __nsList Is Nothing Then
                    Return New ImportsNs() {}
                Else
                    Return __nsList.Values.ToArray
                End If
            End Get
            Set(value As ImportsNs())
                If value Is Nothing Then
                    __nsList = New Dictionary(Of String, ImportsNs)
                Else
                    __nsList = value.ToDictionary(Function(x) x.Namespace.ToLower)
                End If
            End Set
        End Property

        ''' <summary>
        ''' {lower_ns, imports_ns}
        ''' </summary>
        Dim __nsList As Dictionary(Of String, ImportsNs)

        Public Shared ReadOnly Property DefaultFile As String =
            App.ProductSharedDIR & "/API.Imports.json"

        ''' <summary>
        ''' 命名空间的大小写不敏感
        ''' </summary>
        ''' <param name="ns"></param>
        ''' <returns></returns>
        Public Overloads Function [GetType](ns As String) As Type()
            Dim key As String = ns.ToLower

            If __nsList.ContainsKey(key) Then
                Return __nsList(key).Modules.ToArray(Function(x) x.GetType)
            Else
                Return New Type() {}
            End If
        End Function

        Public Function Register(assm As Assembly) As Boolean
            Dim types As Type() = assm.GetTypes
            Dim LQuery = (From type As Type In types
                          Let ns As PackageNamespace = GetEntry(type)
                          Where Not ns Is Nothing
                          Select ns,
                              type).ToArray
            For Each type In LQuery
                Dim ns As String = type.ns.Namespace.ToLower
                If Not __nsList.ContainsKey(ns) Then
                    Call __nsList.Add(ns, New ImportsNs(type.ns))
                End If

                Call __nsList(ns).Add(type.type)
            Next
            Return True
        End Function

        Public Function Register(path As String) As Boolean
            Try
                Dim assm As Assembly = Assembly.LoadFile(path)
                Return Register(assm)
            Catch ex As Exception
                ex = New Exception(path, ex)
                Call App.LogException(ex)
                Return False
            End Try
        End Function

        Public Sub Install()
            Dim files = FileIO.FileSystem.GetFiles(App.HOME, FileIO.SearchOption.SearchTopLevelOnly, "*.exe", "*.dll")
            For Each dll As String In files
                Call Register(dll)
            Next
        End Sub

        Public Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            Return Me.GetJson.SaveTo(If(String.IsNullOrEmpty(Path), DefaultFile, Path), encoding)
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.GetEncodings)
        End Function

        Public Shared Function LoadDefault() As APIProvider
            Return Load(DefaultFile)
        End Function

        Public Shared Function Load(file As String) As APIProvider
            Try
                Return LoadJsonFile(Of APIProvider)(file)
            Catch ex As Exception
                ex = New Exception(file.ToFileURL, ex)
                Call App.LogException(ex)

                Return New APIProvider
            End Try
        End Function

        Sub Save()
            Call Save(DefaultFile, Encodings.ASCII)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call Save()
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
