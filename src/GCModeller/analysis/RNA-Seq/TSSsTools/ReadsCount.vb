Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Net.Protocols

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
        Dim temp As Byte(), p As Integer

        ' NT
        temp = New Byte(INT32 - 1) {}
        Call Array.ConstrainedCopy(raw, p.Move(temp.Length), temp, Scan0, temp.Length)
        NT = ChrW(BitConverter.ToInt32(temp, Scan0))

        ' ReadsPlus
        temp = New Byte(INT64 - 1) {}
        Call Array.ConstrainedCopy(raw, p.Move(temp.Length), temp, Scan0, temp.Length)
        ReadsPlus = BitConverter.ToInt64(temp, Scan0)

        ' ReadsMinus
        Call Array.ConstrainedCopy(raw, p.Move(temp.Length), temp, Scan0, temp.Length)
        ReadsMinus = BitConverter.ToInt64(temp, Scan0)

        ' Index
        Call Array.ConstrainedCopy(raw, p.Move(temp.Length), temp, Scan0, temp.Length)
        Index = BitConverter.ToInt64(temp, Scan0)

        ' SharedPlus
        temp = New Byte(INT32 - 1) {}
        Call Array.ConstrainedCopy(raw, p.Move(temp.Length), temp, Scan0, temp.Length)
        SharedPlus = BitConverter.ToInt32(temp, Scan0)

        ' SharedMinus
        Call Array.ConstrainedCopy(raw, p.Move(temp.Length), temp, Scan0, temp.Length)
        SharedMinus = BitConverter.ToInt32(temp, Scan0)
    End Sub

    Const __BUFFER_LENGTH As Integer = INT32 + ' NT
                                       INT64 + ' ReadsPlus
                                       INT64 + ' ReadsMinus
                                       INT64 + ' Index
                                       INT32 + ' SharedPlus
                                       INT32  ' SharedMinus

    Public Overrides Function Serialize() As Byte()
        Dim buffer As Byte() = New Byte(__BUFFER_LENGTH - 1) {}
        Dim temp As Byte()
        Dim p As Integer

        temp = BitConverter.GetBytes(AscW(NT)) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p.Move(temp.Length), temp.Length)
        temp = BitConverter.GetBytes(ReadsPlus) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p.Move(temp.Length), temp.Length)
        temp = BitConverter.GetBytes(ReadsMinus) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p.Move(temp.Length), temp.Length)
        temp = BitConverter.GetBytes(Index) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p.Move(temp.Length), temp.Length)
        temp = BitConverter.GetBytes(SharedPlus) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p.Move(temp.Length), temp.Length)
        temp = BitConverter.GetBytes(SharedMinus) : Call Array.ConstrainedCopy(temp, Scan0, buffer, p.Move(temp.Length), temp.Length)

        Return buffer
    End Function

    Public Shared Function WriteDb(Db As Generic.IEnumerable(Of ReadsCount), saveDb As String) As Boolean
        Dim LQuery = (From x As ReadsCount In Db.AsParallel Select x.Serialize).ToArray.MatrixToList
        Return LQuery.FlushStream(saveDb)
    End Function

    Public Shared Function LoadDb(path As String) As ReadsCount()
        Dim buffer As Byte() = IO.File.ReadAllBytes(path)
        Dim chunks = buffer.Split(__BUFFER_LENGTH)
        Dim LQuery = (From block As Byte() In chunks.AsParallel Select New ReadsCount(block)).ToArray
        Return LQuery
    End Function
End Class
