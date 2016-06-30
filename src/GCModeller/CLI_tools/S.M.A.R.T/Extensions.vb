Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Terminal.STDIO

Module Extensions

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns>返回安装成功的数据库的数目</returns>
    ''' <remarks></remarks>
    Public Function Install(CDDDatabase As Assembly.NCBI.CDD.Database, Blast As NCBI.Extensions.LocalBLAST.InteropService.InitializeParameter) As Integer
        Dim LocalBLAST As NCBI.Extensions.LocalBLAST.InteropService.InteropService = NCBI.Extensions.LocalBLAST.InteropService.CreateInstance(Blast)
        Dim i As Integer
        For Each Db As KeyValuePair(Of System.Func(Of String), System.Func(Of SequenceModel.FASTA.FastaFile)) In CDDDatabase.DbPaths.Values
            Dim DbPath As String = Db.Key()()
            If Not FileIO.FileSystem.FileExists(DbPath) Then
                Printf("[ERR] Database ""%s"" file not found!", DbPath)
                Continue For
            End If
            Call LocalBLAST.FormatDb(DbPath, LocalBLAST.MolTypeProtein).Start(WaitForExit:=True)

            i += 1
            Printf("[INFO] Install database ""%s"" successfully!", DbPath)
        Next

        Return i
    End Function

    <Extension> Public Function AsName(e As Date) As String
        Return String.Format("{0}-{1}-{2}", Now.Hour, Now.Minute, Now.Second)
    End Function

    ''' <summary>
    ''' 必须所有的元素都包含在内才返回真
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="Keywords"></param>
    ''' <param name="SenseCase"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function ContainsKeyword(Text As String, Keywords As String(), SenseCase As CompareMethod) As Boolean
        Dim Query = From word As String In Keywords Where InStr(Text, word, SenseCase) > 0 Select 1 '
        Return Query.ToArray.Count = Keywords.Count
    End Function

    ''' <summary>
    ''' 只要目标字符串之中包含列表中的任意一个元素就返回真
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="Keywords"></param>
    ''' <param name="SenseCase"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function ContainsAny(Text As String, Keywords As String(), SenseCase As CompareMethod) As Boolean
        Dim Query = From word As String In Keywords Where InStr(Text, word, SenseCase) > 0 Select 1 '
        Return Query.ToArray.Count > 0
    End Function

    ''' <summary>
    ''' 获取所有Hit对象的标识号
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetAllHits(BlastLog As NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput) As String()
        Dim List As List(Of String) = New List(Of String)
        Dim LQuery = From Query In BlastLog.Queries Where Not Query.Hits Is Nothing Select List.Append((From Hit In Query.Hits Select Hit.Name).ToArray) ' 
        LQuery = LQuery.ToArray

        Return List.ToArray
    End Function

    <Extension> Public Function Append(Of T)(List As List(Of T), Collection As Generic.IEnumerable(Of T)) As Integer
        Call List.AddRange(Collection)
        Return List.Count
    End Function

    <Extension> Public Function Append(Of T)(List As List(Of T), e As T) As Integer
        Call List.Add(e)
        Return List.Count
    End Function

    <Extension> Public Function GetIdList(Collection As Generic.IEnumerable(Of SequenceModel.FASTA.FastaToken)) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each fsa In Collection
            Call sBuilder.AppendFormat("{0},", fsa.Attributes.First)
        Next
        Call sBuilder.Remove(sBuilder.Length - 1, 1)

        Return sBuilder.ToString
    End Function
End Module
