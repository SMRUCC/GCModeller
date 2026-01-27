#Region "Microsoft.VisualBasic::915af8b3743222ffdc269dc04f5f1273, analysis\Metagenome\MetaFunction\VFDB\Extensions.vb"

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

'   Total Lines: 22
'    Code Lines: 19 (86.36%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 3 (13.64%)
'     File Size: 846 B


'     Module Extensions
' 
'         Function: BuildVFDIndex
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Namespace VFDB

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function BuildVFDIndex(data As IEnumerable(Of VFs)) As Dictionary(Of String, Index(Of String))
            Return data _
                .GroupBy(Function(a) a.organism) _
                .ToDictionary(Function(org) org.Key,
                              Function(genes)
                                  Return genes _
                                     .Select(Function(g) g.geneName) _
                                     .Distinct _
                                     .Indexing
                              End Function)
        End Function

        <Extension>
        Public Function CreateModel(vfdb As IEnumerable(Of VFDBInfo)) As Background
            Dim clusters As IEnumerable(Of Cluster) = From vf As VFDBInfo
                                                      In vfdb
                                                      Group By vf.VFCID Into Group
                                                      Select VFCID.VFClass(Group)
            Dim model As New Background With {
                .clusters = clusters.ToArray,
                .id = "VFDB",
                .name = "VFDB: Virulence Factor Database",
                .size = .clusters.BackgroundSize,
                .comments = "a reference database for bacterial virulence factors"
            }

            Return model
        End Function

        <Extension>
        Private Function VFClass(id As String, vfs As IEnumerable(Of VFDBInfo)) As Cluster
            Dim members = vfs.ToArray
            Dim label As String = members(0).VFcategory
            Dim desc As String = members.Select(Function(vf) vf.Function).JoinBy("; ")

            desc = LLMs.LLMsTalk($"请基于下面对{label}分类下的病原体毒力因子的功能描述做出这类病原体毒力因子的功能总结：" & desc)

            Return New Cluster With {
                .category = members.First.VFcategory,
                .description = desc,
                .members = members.Select(Function(vf) vf.VFModel).ToArray,
                .ID = id,
                .names = label
            }
        End Function

        <Extension>
        Private Function VFModel(vf As VFDBInfo) As BackgroundGene
            Dim source As New NamedValue With {.name = "Bacteria", .text = vf.Bacteria}
            Dim fullname As New NamedValue With {.name = "FullName", .text = vf.VF_FullName}
            Dim struct As New NamedValue With {.name = "Structure", .text = vf.Structure}

            Return New BackgroundGene With {
                .accessionID = vf.VFID,
                .name = vf.VF_Name,
                .locus_tag = New NamedValue With {.name = vf.VFCID, .text = vf.Function},
                .[alias] = {vf.VF_Name},
                .term_id = (From term As NamedValue
                            In {source, fullname, struct}
                            Where Not term.text.StringEmpty).ToArray
            }
        End Function
    End Module
End Namespace
