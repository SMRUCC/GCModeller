Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' BioCyc 的 ``*.col`` 或者 ``*-links.dat`` 表格文件之中的一行数据
''' </summary>
Public Class ColumnRow

    ''' <summary>
    ''' 第一列通常是数据库对象的唯一编号
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String
    ''' <summary>
    ''' 除第一列之外的其余列的数据
    ''' </summary>
    ''' <returns></returns>
    Public Property columns As String()

    Default Public ReadOnly Property Item(index As Integer) As String
        Get
            If columns Is Nothing OrElse index >= columns.Length Then
                Return Nothing
            End If

            Return columns(index)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{ID}: {columns.SafeQuery.JoinBy(" | ")}"
    End Function

End Class

''' <summary>
''' 用于读取BioCyc数据库的表格文件：``transporters.col``、``genes.col``以及
''' ``compound-links.dat``、``protein-links.dat``等等。这些文件使用制表符号(TAB)
''' 分隔列，并且以 ``#`` 开始的注释行可以出现在文件的任意位置。
''' </summary>
Public Module ColumnTable

    Private Function openReader(file As String) As StreamReader
        Return New StreamReader(file.OpenReadonly, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False, throwOnInvalidBytes:=False))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadFile(file As String) As ColumnRow()
        Using reader As StreamReader = openReader(file)
            Return LoadData(reader).ToArray
        End Using
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadData(text As String) As ColumnRow()
        Using reader As New StringReader(text)
            Return LoadData(reader).ToArray
        End Using
    End Function

    ''' <summary>
    ''' 逐行解析表格数据，返回第一列作为主键的行对象
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <returns></returns>
    Public Iterator Function LoadData(reader As TextReader) As IEnumerable(Of ColumnRow)
        Dim line As String
        Dim tokens As String()

        Do
            line = reader.ReadLine

            If line Is Nothing Then
                Exit Do
            End If

            line = line.Trim()

            If line.Length = 0 OrElse line.StartsWith("#") Then
                Continue Do
            End If

            tokens = line.Split(ControlChars.Tab)

            If tokens.Length = 0 OrElse tokens(0).StringEmpty Then
                Continue Do
            End If

            Yield New ColumnRow With {
                .ID = tokens(0).Trim(),
                .columns = tokens.Skip(1).ToArray
            }
        Loop
    End Function

End Module
