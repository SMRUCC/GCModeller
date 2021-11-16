#Region "Microsoft.VisualBasic::7ec66feeb4c1e8b321fefdb2ba6b6fe6, analysis\RNA-Seq\TSSsTools\ReadsCount.vb"

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

    ' Class ReadsCount
    ' 
    '     Properties: Index, NT, ReadsMinus, ReadsPlus, SharedMinus
    '                 SharedPlus
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: getCache, LoadDb, ToString, WriteDb
    ' 
    '     Sub: Serialize
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization

Public Class ReadsCount : Inherits RawStream

    Public Property NT As Char
    Public Property ReadsPlus As Long
    Public Property ReadsMinus As Long
    ''' <summary>
    ''' 请注意，这里的index位置是序列上面的位置，下标是从1开始的，而程序之中的数组则是从0开始的，所以需要减1才能够得到数组之中的正确位置
    ''' </summary>
    ''' <returns></returns>
    Public Property Index As Long
    <Column("5'-Plus")> Public Property SharedPlus As Integer
    <Column("5'-Minus")> Public Property SharedMinus As Integer

    Public Overrides Function ToString() As String
        Return $"[{Index}]{NT}  {ReadsPlus}  {ReadsMinus}  {SharedPlus}  {SharedMinus}"
    End Function

    Sub New()
    End Sub

    Sub New(raw As Byte())
        Dim temp As Byte(), p As i32 = 0

        ' NT
        temp = New Byte(INT32 - 1) {}
        Call Array.ConstrainedCopy(raw, p + temp.Length, temp, Scan0, temp.Length)
        NT = ChrW(BitConverter.ToInt32(temp, Scan0))

        ' ReadsPlus
        temp = New Byte(INT64 - 1) {}
        Call Array.ConstrainedCopy(raw, p + temp.Length, temp, Scan0, temp.Length)
        ReadsPlus = BitConverter.ToInt64(temp, Scan0)

        ' ReadsMinus
        Call Array.ConstrainedCopy(raw, p + temp.Length, temp, Scan0, temp.Length)
        ReadsMinus = BitConverter.ToInt64(temp, Scan0)

        ' Index
        Call Array.ConstrainedCopy(raw, p + temp.Length, temp, Scan0, temp.Length)
        Index = BitConverter.ToInt64(temp, Scan0)

        ' SharedPlus
        temp = New Byte(INT32 - 1) {}
        Call Array.ConstrainedCopy(raw, p + temp.Length, temp, Scan0, temp.Length)
        SharedPlus = BitConverter.ToInt32(temp, Scan0)

        ' SharedMinus
        Call Array.ConstrainedCopy(raw, p + temp.Length, temp, Scan0, temp.Length)
        SharedMinus = BitConverter.ToInt32(temp, Scan0)
    End Sub

    Const __BUFFER_LENGTH As Integer = INT32 + ' NT
                                       INT64 + ' ReadsPlus
                                       INT64 + ' ReadsMinus
                                       INT64 + ' Index
                                       INT32 + ' SharedPlus
                                       INT32  ' SharedMinus

    Private Function getCache() As Byte()
        Dim buffer As Byte() = New Byte(__BUFFER_LENGTH - 1) {}
        Dim temp As Byte()
        Dim p As i32 = 0

        temp = BitConverter.GetBytes(AscW(NT)) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p + (temp.Length), temp.Length)
        temp = BitConverter.GetBytes(ReadsPlus) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p + (temp.Length), temp.Length)
        temp = BitConverter.GetBytes(ReadsMinus) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p + (temp.Length), temp.Length)
        temp = BitConverter.GetBytes(Index) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p + (temp.Length), temp.Length)
        temp = BitConverter.GetBytes(SharedPlus) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p + (temp.Length), temp.Length)
        temp = BitConverter.GetBytes(SharedMinus) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p + (temp.Length), temp.Length)

        Return buffer
    End Function

    Public Overrides Sub Serialize(buffer As Stream)
        Dim data As Byte() = getCache()
        Call buffer.Write(data, Scan0, data.Length)
        Erase data
    End Sub

    Public Shared Function WriteDb(Db As Generic.IEnumerable(Of ReadsCount), saveDb As String) As Boolean
        Dim LQuery = (From x As ReadsCount In Db.AsParallel Select x.Serialize).ToArray.Unlist
        Return LQuery.FlushStream(saveDb)
    End Function

    Public Shared Function LoadDb(path As String) As ReadsCount()
        Dim buffer As Byte() = IO.File.ReadAllBytes(path)
        Dim chunks = buffer.Split(__BUFFER_LENGTH)
        Dim LQuery = (From block As Byte() In chunks.AsParallel Select New ReadsCount(block)).ToArray
        Return LQuery
    End Function
End Class
