#Region "Microsoft.VisualBasic::b13c086491794906de5aa11ad23c7fd2, RDotNET.Extensions.GCModeller\KEGG\GSEABackground.vb"

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

    ' Module GSEABackground
    ' 
    '     Function: SaveBackgroundRda
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports SMRUCC.genomics.Assembly.KEGG
Imports VisualBasicNet = Microsoft.VisualBasic.Language.Runtime

Public Module GSEABackground

    ''' <summary>
    ''' 从标准参考图数据中建立通用的富集分析的背景模型
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function SaveBackgroundRda(maps As IEnumerable(Of DBGET.bGetObject.PathwayMap), rdafile$) As Boolean
        Dim backgroundSize%
        Dim terms As New List(Of String)
        Dim background As New var("GSEA.KEGGCompounds", "list()")
        Dim pathways As New var("list()")

        With New VisualBasicNet
            For Each map In maps
                pathways(map.EntryId) = base.list(
                    !id = map.EntryId,
                    !name = map.name,
                    !compounds = map.KEGGCompound _
                        .Select(Function(c) c.name) _
                        .DoCall(Function(list)
                                    Return base.c(list, stringVector:=True)
                                End Function),
                    !ko = map.KEGGOrthology _
                        .EntityList _
                        .DoCall(Function(list)
                                    Return base.c(list, stringVector:=True)
                                End Function)
                )
            Next
        End With

        With background
            !size = backgroundSize
            !pathways = pathways
        End With

        Return base.save({background}, file:=rdafile)
    End Function
End Module

