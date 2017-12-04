#Region "Microsoft.VisualBasic::bf79446dbab98557bc990a0f945c090d, ..\GCModeller\data\KEGG\Procedures\DumpUniprot.vb"

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

Imports System.Runtime.CompilerServices

Public Module DumpUniprot

    ''' <summary>
    ''' 因为KEGG没有对外的批量数据下载接口，所以在这里使用uniprot的下载数据导入为KEGG之中的基因的数据
    ''' </summary>
    ''' <param name="XML$"></param>
    ''' <returns></returns>
    <Extension> Public Function ImportsUniprotProteins(XML$)

    End Function
End Module

