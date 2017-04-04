#Region "Microsoft.VisualBasic::8f0b0d477e860ff579e8e103481b6024, ..\core\Bio.Assembly\Assembly\EBI.ChEBI\Database\DATA.vb"

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

Namespace Assembly.EBI.ChEBI

    Public Module DATA

        ''' <summary>
        ''' 读取从ChEBI的ftp服务器之上所下载的tsv数据表格文件然后通过链接构建出完整的分子数据模型<see cref="ChEBIEntity"/>.
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        Public Iterator Function BuildEntitysFromTsv(DIR$) As IEnumerable(Of ChEBIEntity)

        End Function
    End Module
End Namespace
