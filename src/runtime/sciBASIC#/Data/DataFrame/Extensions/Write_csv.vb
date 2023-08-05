Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.ComponentModels
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text
Imports File_csv = Microsoft.VisualBasic.Data.csv.IO.File

Public Module Write_csv

    ''' <summary>
    ''' Save the object collection data dump into a csv file.
    ''' (将一个对象数组之中的对象保存至一个Csv文件之中，请注意:
    ''' + 这个方法仅仅会保存简单的基本数据类型的属性值
    ''' + 并且这个方法仅适用于小型数据集, 如果需要保存大型数据集, 请使用Linq版本的拓展函数)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source">应该是List, Array或者Collection, 不应该是一个Linq拓展表达式</param>
    ''' <param name="path"></param>
    ''' <param name="strict">
    ''' If true then all of the simple data type property its value will be save to the data file,
    ''' if not then only save the property with the <see cref="ColumnAttribute"></see>
    ''' </param>
    ''' <param name="encoding"></param>
    ''' <param name="maps">``{meta_define -> custom}``</param>
    ''' <param name="layout">可以通过这个参数来进行列顺序的重排，值越小表示排在越前面</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function SaveTo(Of T)(source As IEnumerable(Of T), path As String,
                                 Optional strict As Boolean = False,
                                 Optional encoding As Encoding = Nothing,
                                 Optional metaBlank As String = "",
                                 Optional nonParallel As Boolean = False,
                                 Optional maps As Dictionary(Of String, String) = Nothing,
                                 Optional reorderKeys As Integer = 0,
                                 Optional layout As Dictionary(Of String, Integer) = Nothing,
                                 Optional tsv As Boolean = False,
                                 Optional transpose As Boolean = False,
                                 Optional silent As Boolean = False) As Boolean
        Try
            path = FileIO.FileSystem.GetFileInfo(path).FullName
        Catch ex As Exception
            Throw New Exception("Probably invalid path value: " & path, ex)
        End Try

        Dim objSeq As Object() = source _
            .Select(Function(o) DirectCast(o, Object)) _
            .ToArray

        If Not silent Then
            Call EchoLine($"[CSV.Reflector::{GetType(T).FullName}]")
            Call EchoLine($"Save data to file:///{path}")
            Call EchoLine($"[CSV.Reflector] Reflector have {objSeq.Length} lines of data to write.")
        End If

        Dim csv As IEnumerable(Of RowObject) = Reflector.GetsRowData(
            source:=objSeq,
            type:=GetType(T),
            strict:=strict,
            maps:=maps,
            parallel:=Not nonParallel,
            metaBlank:=metaBlank,
            reorderKeys:=reorderKeys,
            layout:=layout
        )

        If transpose Then
            csv = csv _
                .Select(Function(r) r.ToArray) _
                .MatrixTranspose _
                .Select(Function(r) New RowObject(r)) _
                .ToArray
        End If

        Dim success = csv.SaveDataFrame(
            path:=path,
            encoding:=encoding,
            tsv:=tsv,
            silent:=silent
        )

        If success AndAlso Not silent Then
            Call "CSV saved!".EchoLine
        End If

        Return success
    End Function
End Module
