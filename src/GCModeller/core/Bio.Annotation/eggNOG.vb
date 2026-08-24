#Region "Microsoft.VisualBasic::676ce62eb2b110c3d9df747f21375a5b, core\Bio.Annotation\eggNOG.vb"

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

    '   Total Lines: 63
    '    Code Lines: 57 (90.48%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (9.52%)
    '     File Size: 2.41 KB


    ' Class eggNOG
    ' 
    '     Properties: BiGG_Reaction, BRITE, CAZy, COG_category, Description
    '                 EC, eggNOG_OGs, evalue, GOs, KEGG_ko
    '                 KEGG_Module, KEGG_Pathway, KEGG_rclass, KEGG_Reaction, KEGG_TC
    '                 max_annot_lvl, PFAMs, Preferred_name, query, score
    '                 seed_ortholog
    ' 
    '     Function: ParseTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class eggNOG : Implements INamedValue

    Public Property query As String Implements INamedValue.Key
    Public Property seed_ortholog As String
    Public Property evalue As Double
    Public Property score As Double
    Public Property eggNOG_OGs As String
    Public Property max_annot_lvl As String
    Public Property COG_category As String
    Public Property Description As String
    Public Property Preferred_name As String
    Public Property GOs As String
    Public Property EC As String
    Public Property KEGG_ko As String
    Public Property KEGG_Pathway As String
    Public Property KEGG_Module As String
    Public Property KEGG_Reaction As String
    Public Property KEGG_rclass As String
    Public Property BRITE As String
    Public Property KEGG_TC As String
    Public Property CAZy As String
    Public Property BiGG_Reaction As String
    Public Property PFAMs As String

    Public Shared Iterator Function ParseTable(file As Stream) As IEnumerable(Of eggNOG)
        For Each line As String In file.ReadAllLines
            If line.StartsWith("#"c) Then
                Continue For
            End If

            Dim s As New StringArrayPointer(line.Split(ControlChars.Tab))

            Yield New eggNOG With {
                .query = s.ReadString,
                .seed_ortholog = s.ReadString,
                .evalue = s.ReadDouble,
                .score = s.ReadDouble,
                .eggNOG_OGs = s.ReadString,
                .max_annot_lvl = s.ReadString,
                .COG_category = s.ReadString,
                .Description = s.ReadString,
                .Preferred_name = s.ReadString,
                .GOs = s.ReadString,
                .EC = s.ReadString,
                .KEGG_ko = s.ReadString,
                .KEGG_Pathway = s.ReadString,
                .KEGG_Module = s.ReadString,
                .KEGG_Reaction = s.ReadString,
                .KEGG_rclass = s.ReadString,
                .BRITE = s.ReadString,
                .KEGG_TC = s.ReadString,
                .CAZy = s.ReadString,
                .BiGG_Reaction = s.ReadString,
                .PFAMs = s.ReadString
            }
        Next
    End Function

End Class

