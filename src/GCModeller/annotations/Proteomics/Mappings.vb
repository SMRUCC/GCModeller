#Region "Microsoft.VisualBasic::1db89429b5e94abd30a44a6ae87ef117, GCModeller\annotations\Proteomics\Mappings.vb"

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


    ' Code Statistics:

    '   Total Lines: 56
    '    Code Lines: 39
    ' Comment Lines: 11
    '   Blank Lines: 6
    '     File Size: 2.18 KB


    ' Module Mappings
    ' 
    '     Function: SplitID, UserCustomMaps
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.Web

''' <summary>
''' 编号处理模块
''' </summary>
Public Module Mappings

    ''' <summary>
    ''' 对于还没有参考基因组的物种蛋白组实验而言，实验数据的基因号可能会是用户自己定义的基因号，
    ''' 则需要使用这个函数将用户的基因号替换为所注释到的UniprotKB编号，方便进行后续的实验数据的分析
    ''' </summary>
    ''' <param name="DEGgenes"></param>
    ''' <param name="tsv$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function UserCustomMaps(DEGgenes As IEnumerable(Of EntityObject), tsv$) As EntityObject()
        Dim DEPgenes = DEGgenes.ToArray

        With tsv.MappingReader Or New Dictionary(Of String, String())().AsDefault
            If .Count > 0 Then

                ' 将用户基因号转换为uniprot编号
                For Each gene In DEPgenes
                    If .ContainsKey(gene.ID) Then
                        gene.ID = .ByRef(gene.ID).First
                    End If
                Next
            End If
        End With

        Return DEPgenes
    End Function

    <Extension>
    Public Function SplitID(DEGgenes As IEnumerable(Of EntityObject)) As EntityObject()
        Return DEGgenes _
            .Select(Function(gene)
                        Return gene.ID _
                            .Split(";"c) _
                            .Select(AddressOf Strings.Trim) _
                            .Select(Function(id)
                                        Return New EntityObject With {
                                            .ID = id,
                                            .Properties = gene.Properties.Clone
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function
End Module
