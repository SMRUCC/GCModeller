#Region "Microsoft.VisualBasic::b81b97f8625643f934b2762295902820, Bio.Repository\NCBI\RepositoryExtensions.vb"

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

    '     Module RepositoryExtensions
    ' 
    '         Function: GetAssemblyPath, GetGenomeData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank

Namespace NCBI

    Public Module RepositoryExtensions

        ''' <summary>
        ''' 将非``plasmid``的基因组序列从指定的Genbank文件<paramref name="gb"/>之中拿出来
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <returns></returns>
        Public Function GetGenomeData(gb As String) As GBFF.File
            Return GBFF.File _
                .LoadDatabase(filePath:=gb) _
                .Where(Function(g)
                           Return InStr(g.Definition.Value, "plasmid", CompareMethod.Text) = 0
                       End Function) _
                .FirstOrDefault
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAssemblyPath(repo$, assembly$) As String
            Return $"{repo}/{assembly}/{assembly}_genomic.gbff"
        End Function
    End Module
End Namespace
