#Region "Microsoft.VisualBasic::74c82487b9e26330f3634c51cfc134b6, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\DataServices\StorageInterface\API.vb"

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
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Language

Namespace DataStorage.FileModel

    ''' <summary>
    ''' GCModeller的内核驱动程序的数据服务
    ''' </summary>
    ''' <remarks></remarks>
    <Package("Kernel_Driver.Data_Services", Publisher:="GCModeller Virtual Cell System")>
    Public Module Extensions

        ''' <summary>
        ''' 加载<see cref="Integer"></see>类型的计算数据
        ''' </summary>
        ''' <param name="Csv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Load.Csv.Integer_Samples")>
        <Extension> Public Function LoadData_Integer(Csv As File) As DataSerials(Of Integer)()
            Dim LQuery As DataSerials(Of Integer)() = LinqAPI.Exec(Of DataSerials(Of Integer)) <=
 _
                From row As RowObject
                In Csv.Skip(1).ToArray.AsParallel
                Let sample As Integer() = (From s As String In row.Skip(1).ToArray Let n As Integer = CInt(Val(s)) Select n).ToArray
                Let x As DataSerials(Of Integer) = New DataSerials(Of Integer) With {
                    .UniqueId = row.First,
                    .Samples = sample
                }
                Select x

            Return LQuery.WriteAddress.ToArray
        End Function

        <ExportAPI("Load.Csv.Double_Samples")>
        <Extension> Public Function LoadData_Double(Csv As File) As DataSerials(Of Double)()
            Dim LQuery As DataSerials(Of Double)() = (
        From item
        In Csv.Skip(1).ToArray.AsParallel
        Select New DataSerials(Of Double) With {
            .UniqueId = item.First,
            .Samples = (From s As String In item.Skip(1) Select Val(s)).ToArray}).ToArray
            Return LQuery.WriteAddress.ToArray
        End Function

        <ExportAPI("Load.Csv.Boolean_Samples")>
        <Extension> Public Function LoadData_Boolean(Csv As File) As DataSerials(Of Boolean)()
            Dim LQuery As DataSerials(Of Boolean)() = (
        From item
        In Csv.Skip(1).ToArray.AsParallel
        Select New DataSerials(Of Boolean) With {
            .UniqueId = item.First,
            .Samples = (From s As String In item.Skip(1) Select CBool(s)).ToArray}).ToArray
            Return LQuery.WriteAddress.ToArray
        End Function

        <Extension> Public Function TakeSamples(Of T)(data As IEnumerable(Of DataSerials(Of T)),
                                                  start As Integer,
                                                  Optional counts As Integer = -1) As DataSerials(Of T)()
            Return DataSerials(Of T).GetRanges(data, start, counts)
        End Function

        ''' <summary>
        ''' 获取实验数据的采样数目
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function SampleCounts(Of T)(data As IEnumerable(Of DataSerials(Of T))) As Integer
            If data.IsNullOrEmpty Then
                Return 0
            End If
            Return data.First.Samples.DataCounts
        End Function
    End Module
End Namespace
