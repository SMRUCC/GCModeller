#Region "Microsoft.VisualBasic::b212ca65bb6fcb10d4e5905e4899ddd6, CLI_tools\c2\Workflows\Merge.vb"

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

    ' Class Merge
    ' 
    '     Function: Invoke
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports LANS.SystemsBiology.DatabaseServices
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite

Public Class Merge

    ''' <summary>
    ''' 将多个数据合并在一起，并且去除了Pcc小于0.6的对象
    ''' </summary>
    ''' <param name="Dir"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Invoke(Dir As String) As MatchedResult()
        Dim TempChunk As List(Of String) = New List(Of String)
        For Each File As String In FileIO.FileSystem.GetFiles(Dir, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
            Call TempChunk.AddRange(IO.File.ReadAllLines(File))
        Next
        TempChunk = TempChunk.Distinct.ToList

        Dim Regulations = (From item In TempChunk.ToArray.LoadCsv(Of MatchedResult)(False) Where System.Math.Abs(item.TFPcc) >= 0.6 Select item).ToArray
        Dim OperonIdList As String() = (From item In Regulations Select item.DoorId Distinct Order By DoorId Ascending).ToArray
        Dim DataChunk As List(Of MatchedResult) = New List(Of MatchedResult)

        For Each Id As String In OperonIdList
            Dim LQuery = (From item In Regulations.AsParallel Where String.Equals(item.DoorId, Id) Select item).ToArray
            Dim TFList = (From item In LQuery Select item.TF Distinct Order By TF Ascending).ToArray
            For Each TFId As String In TFList
                Call DataChunk.AddRange((From item In LQuery.AsParallel Where String.Equals(item.TF, TFId) Select item Order By item.TFPcc Ascending).ToArray)
            Next
        Next

        Dim Path As String = FileIO.FileSystem.GetParentPath(Dir), Name As String = FileIO.FileSystem.GetName(Dir)
        Path = String.Format("{0}/{1}.csv", Path, Name)
        Call Regulations.SaveTo(Path, False)

        Return Regulations
    End Function

End Class
