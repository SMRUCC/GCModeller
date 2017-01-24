#Region "Microsoft.VisualBasic::90a546a4e41b9ab56534e47adc9bab01, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\DataServices\StorageInterface\DataSerials(Of T).vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace DataStorage.FileModel

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"><see cref="Integer"></see>; <see cref="Double"></see>; <see cref="Boolean"></see></typeparam>
    ''' <remarks></remarks>
    Public Class DataSerials(Of T) : Implements IAddressHandle

        Public Property UniqueId As String
        Public Property Samples As T()
        Public Property Handle As Integer Implements IAddressHandle.Address

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}   ", Handle, UniqueId) & String.Join(",", (From obj In Samples Select s = obj.ToString).ToArray)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="start"></param>
        ''' <param name="counts">小于零表示取完<paramref name="start"></paramref>之后的所有的数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRanges(data As IEnumerable(Of DataSerials(Of T)),
                                         start As Integer,
                                         Optional counts As Integer = -1) As DataSerials(Of T)()

            Dim LQuery As DataSerials(Of T)() =
                LinqAPI.Exec(Of DataSerials(Of T)) <= From x As DataSerials(Of T)
                                                      In data.AsParallel
                                                      Select New DataSerials(Of T) With {
                                                          .Handle = x.Handle,
                                                          .UniqueId = x.UniqueId,
                                                          .Samples = x.Samples.Skip(start).ToArray
                                                      }
            If counts > 0 Then
                If counts >= data.First.Samples.Length - start Then
                    counts = data.First.Samples.Length - start
                End If

                Dim setValue = New SetValue(Of DataSerials(Of T))().GetSet(NameOf(DataSerials(Of T).Samples))

                LQuery = LinqAPI.Exec(Of DataSerials(Of T)) <=
                    From x As DataSerials(Of T)
                    In LQuery.AsParallel
                    Let array As T() = x.Samples.Take(counts).ToArray
                    Select setValue(x, array)
            End If

            Return (From x As DataSerials(Of T) In LQuery Select x Order By x.Handle Ascending).ToArray
        End Function

        ''' <summary>
        ''' 将计算数据转换为Csv文件进行存储
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ToCsv(data As IEnumerable(Of DataSerials(Of T))) As File
            Dim File As File = New File
            Dim head As List(Of String) = {"GeneId/Time-Serials"}.Join((From i As Integer In data.First.Samples.Sequence Select CStr(i)).ToArray)
            Dim LQuery = (From x As DataSerials(Of T) In data
                          Let xSet As String() = (From n As T In x.Samples Let strValue As String = n.ToString Select strValue).ToArray
                          Let strVector As String() = {x.UniqueId}.Join(xSet).ToArray
                          Select strVector.ToCsvRow).ToArray

            Call File.Add(New RowObject(head))
            Call File.AppendRange(LQuery)

            Return File
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

    End Class
End Namespace
