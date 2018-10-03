Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports [Module] = Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute

Public Class Reader : Inherits Raw

    ReadOnly stream As BinaryDataReader
    ReadOnly moduleIndex As New Index(Of String)

    ''' <summary>
    ''' 按照时间升序排序的
    ''' </summary>
    Dim offsetIndex As OrderSelector(Of NumericTagged(Of Dictionary(Of String, Long)))

    Sub New(input As Stream)
        stream = New BinaryDataReader(input)
    End Sub

    Public Function LoadIndex() As Reader
        Dim modules = GetModules _
            .ToDictionary(Function(prop)
                              Dim modAttr = prop.GetAttribute(Of [Module])

                              If modAttr Is Nothing OrElse modAttr.Name.StringEmpty Then
                                  Return prop.Name
                              Else
                                  Return modAttr.Name
                              End If
                          End Function)

        Call stream.Seek(0, SeekOrigin.Begin)
        Call moduleIndex.Clear()

        If stream.ReadString(Magic.Length) <> Magic Then
            Throw New InvalidDataException("Invalid magic string!")
        Else
            ' read headers
            For i As Integer = 0 To modules.Count - 1
                Dim name = stream.ReadDwordLenString
                Dim n = stream.ReadInt32
                Dim list As New Index(Of String)

                For j As Integer = 0 To n - 1
                    Call list.Add(stream.ReadString(BinaryStringFormat.ZeroTerminated))
                Next

                Call moduleIndex.Add(name)
                Call modules(name).SetValue(Me, list)
            Next

            Call stream.Seek(stream.Length - 8, SeekOrigin.Begin)
        End If

        ' read index
        Dim offset& = stream.ReadInt64
        Dim indexSelector As New List(Of NumericTagged(Of Dictionary(Of String, Long)))

        ' 索引按照time降序排序，结构为
        ' 
        ' - double time
        ' - integer index，从零开始的索引号
        ' - long() 按照modules顺序排序的offset值的集合
        ' - long 当前数据块的起始的offset偏移
        '
        Call stream.Seek(offset, SeekOrigin.Begin)

        Do While True
            Dim time As Double = stream.ReadDouble
            Dim index% = stream.ReadInt32
            Dim offsets&() = stream.ReadInt64s(moduleIndex.Count)

            indexSelector += New NumericTagged(Of Dictionary(Of String, Long)) With {
                .tag = time,
                .value = moduleIndex _
                    .ToDictionary(Function(m) m.value,
                                  Function(i) offsets(i))
            }

            If index = 0 Then
                Exit Do
            End If
        Loop

        offsetIndex = New OrderSelector(Of NumericTagged(Of Dictionary(Of String, Long)))(indexSelector)

        Return Me
    End Function

    Public Function Read(time#, module$) As Dictionary(Of String, Double)
        Dim index = offsetIndex.
    End Function

    Public Iterator Function PopulateFrames() As IEnumerable(Of Dictionary(Of String, Dictionary(Of String, Double)))
        For Each timeFrame In offsetIndex

        Next
    End Function
End Class