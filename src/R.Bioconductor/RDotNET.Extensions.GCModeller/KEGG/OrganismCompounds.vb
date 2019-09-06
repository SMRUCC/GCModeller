#Region "Microsoft.VisualBasic::1102633c084d0f77d0cc42dc31be2111, RDotNET.Extensions.GCModeller\KEGG\OrganismCompounds.vb"

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

    ' Class OrganismCompounds
    ' 
    '     Properties: code, compounds, name
    ' 
    '     Function: LoadData, ToString, WriteRda
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports rda = RDotNET.Extensions.VisualBasic.Serialization.SaveRda

''' <summary>
''' 物种与代谢物之间的对应关系数据集
''' </summary>
Public Class OrganismCompounds

    Public Property code As String
    Public Property name As String
    Public Property compounds As NamedValue()

    Public Overrides Function ToString() As String
        Return name
    End Function

    Public Shared Function LoadData(repo As String) As OrganismCompounds
        Dim info = $"{repo}/kegg.json".LoadJSON(Of OrganismInfo)
        Dim maps = ls - l - r - "*.Xml" <= repo
        Dim compounds As NamedValue() = maps _
            .Select(Function(path) path.LoadXml(Of Pathway)) _
            .Select(Function(map)
                        Return map.compound.SafeQuery
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(c) c.name) _
            .Select(Function(cg) cg.First) _
            .ToArray

        Return New OrganismCompounds With {
            .code = info.code,
            .name = info.FullName,
            .compounds = compounds
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function WriteRda(dataset As OrganismCompounds, rdafile$) As Boolean
        Return rda.save(dataset, rdafile, name:="KEGG")
    End Function
End Class

