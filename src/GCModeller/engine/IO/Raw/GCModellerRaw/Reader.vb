#Region "Microsoft.VisualBasic::bc5c43abef65a72bcfa0dc687855cc79, GCModeller\engine\IO\Raw\GCModellerRaw\Reader.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 145
    '    Code Lines: 102
    ' Comment Lines: 16
    '   Blank Lines: 27
    '     File Size: 5.66 KB


    '     Class Reader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: AllTimePoints, LoadIndex, PopulateFrames, Read, ReadModule
    ' 
    '         Sub: readIndex
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language

Namespace Raw

    Public Class Reader : Inherits CellularModules

        ReadOnly stream As BinaryDataReader

        ''' <summary>
        ''' 按照时间升序排序的
        ''' </summary>
        Dim offsetIndex As OrderSelector(Of NumericTagged(Of Dictionary(Of String, Long)))

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AllTimePoints() As IEnumerable(Of Double)
            Return offsetIndex.Select(Function(t) t.tag)
        End Function

        Sub New(input As Stream)
            stream = New BinaryDataReader(input)
        End Sub

        Public Function LoadIndex() As Reader
            Dim modules As Dictionary(Of String, PropertyInfo) = Me.GetModuleReader

            Call stream.Seek(0, SeekOrigin.Begin)
            Call moduleIndex.Clear()
            Call Me.modules.Clear()

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
                Call readIndex()
            End If

            Return Me
        End Function

        Private Sub readIndex()
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
                                      Function(i)
                                          Return offsets(i)
                                      End Function)
                }
                offset = offset - 8

                If index = 0 Then
                    Exit Do
                Else
                    ' offset chain block
                    Call stream.Seek(offset, SeekOrigin.Begin)
                    Call offset.SetValue(stream.ReadInt64)
                End If
            Loop

            offsetIndex = New OrderSelector(Of NumericTagged(Of Dictionary(Of String, Long)))(indexSelector)
        End Sub

        Public Function Read(time#, module$) As Dictionary(Of String, Double)
            Dim index = offsetIndex.Find(time, Function(t) t.tag)
            Dim offset As Long = index.value([module])

            Return ReadModule(offset).data
        End Function

        Public Function ReadModule(offset As Long) As (time#, data As Dictionary(Of String, Double))
            ' - double time 时间值
            ' - byte 在header之中的module的索引号
            ' - double() data块，每一个值的顺序是和header之中的id排布顺序是一样的，长度和header之中的id列表保持一致
            Dim time As Double = stream.ReadDouble
            Dim moduleIndex As Integer = stream.ReadByte
            Dim list As Index(Of String) = modules(Me.moduleIndex(index:=moduleIndex))
            Dim data#() = stream.ReadDoubles(list.Count)
            Dim values As Dictionary(Of String, Double) = list _
                .ToDictionary(Function(id) id.value,
                              Function(i)
                                  Return data(i)
                              End Function)

            Return (time, values)
        End Function

        Public Iterator Function PopulateFrames() As IEnumerable(Of (time#, frame As Dictionary(Of DataSet)))
            For Each timeFrame In offsetIndex
                Dim time# = timeFrame.tag
                Dim frame As New Dictionary(Of DataSet)

                For Each [module] As String In moduleIndex.Objects
                    Dim offset& = timeFrame.value([module])
                    Dim data = ReadModule(offset).data

                    frame([module]) = New DataSet With {
                        .ID = [module],
                        .Properties = data
                    }
                Next

                Yield (time, frame)
            Next
        End Function
    End Class
End Namespace
