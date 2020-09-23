#Region "Microsoft.VisualBasic::66cdaf87a8c1ab6c43e0936ee85acfe5, CLI_tools\S.M.A.R.T\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: (+2 Overloads) Append, AsName, ContainsAny, ContainsKeyword, GetIdList
    '               Install
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST
Imports SMRUCC.genomics.SequenceModel

Module Extensions

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns>返回安装成功的数据库的数目</returns>
    ''' <remarks></remarks>
    Public Function Install(CDDDatabase As NCBI.CDD.Database, Blast As LocalBLAST.InteropService.InitializeParameter) As Integer
        Dim LocalBLAST As LocalBLAST.InteropService.InteropService = InteropService.CreateInstance(Blast)
        Dim i As Integer
        For Each Db As KeyValuePair(Of System.Func(Of String), System.Func(Of FASTA.FastaFile)) In CDDDatabase.DbPaths.Values
            Dim DbPath As String = Db.Key()()
            If Not FileIO.FileSystem.FileExists(DbPath) Then
                printf("[ERR] Database ""%s"" file not found!", DbPath)
                Continue For
            End If
            Call LocalBLAST.FormatDb(DbPath, LocalBLAST.MolTypeProtein).Start(waitForExit:=True)

            i += 1
            printf("[INFO] Install database ""%s"" successfully!", DbPath)
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

    <Extension> Public Function Append(Of T)(List As List(Of T), Collection As Generic.IEnumerable(Of T)) As Integer
        Call List.AddRange(Collection)
        Return List.Count
    End Function

    <Extension> Public Function Append(Of T)(List As List(Of T), e As T) As Integer
        Call List.Add(e)
        Return List.Count
    End Function

    <Extension> Public Function GetIdList(source As IEnumerable(Of FASTA.FastaSeq)) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each fsa In source
            Call sBuilder.AppendFormat("{0},", fsa.Headers.First)
        Next
        Call sBuilder.Remove(sBuilder.Length - 1, 1)

        Return sBuilder.ToString
    End Function
End Module
