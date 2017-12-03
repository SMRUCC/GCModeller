#Region "Microsoft.VisualBasic::f56e8baf2f025aa6e073c9d547af5ac4, ..\GCModeller\analysis\Metagenome\Metagenome\OTUTable.vb"

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

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Text

''' <summary>
''' 这个对象记录了当前的宏基因组实验之中的每一个OTU在样品之中的含量的多少
''' </summary>
Public Class OTUTable : Inherits DataSet
    
    ''' <summary>
    ''' 这个函数会自动兼容csv或者tsv格式的
    ''' </summary>
    ''' <param name="table$"></param>
    ''' <returns></returns>
    Public Shared Function LoadSample(table$, Optional ID$ = "OTU_ID") As OTUTable()
        Dim fieldMaps As New Dictionary(Of String, String) From {
            {ID, NameOf(OTUTable.ID)}
        }

        If table.ReadFirstLine.Count(ASCII.TAB) > 1 Then
            ' tsv文件
            Return table _
                .ReadAllLines _
                .ImportsTsv(Of OTUTable)(fieldMaps)
        Else
            Return table.LoadCsv(Of OTUTable)(maps:=fieldMaps)
        End If
    End Function
End Class

