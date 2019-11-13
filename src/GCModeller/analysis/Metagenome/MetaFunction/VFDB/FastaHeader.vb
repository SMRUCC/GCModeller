#Region "Microsoft.VisualBasic::80ddebde7daac4c00072c50acd6c9943, analysis\Metagenome\MetaFunction\VFDB\FastaHeader.vb"

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

    '     Class FastaHeader
    ' 
    '         Properties: fullName, geneName, organism, VFID, xref
    ' 
    '         Function: ParseHeader, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace VFDB

    ''' <summary>
    ''' http://www.mgc.ac.cn/VFs/download.htm
    ''' </summary>
    Public Class FastaHeader

        Public Property VFID As String
        Public Property xref As String
        Public Property geneName As String
        Public Property fullName As String
        Public Property organism As String

        Public Overrides Function ToString() As String
            Return fullName
        End Function

        Public Shared Function ParseHeader(fasta As FastaSeq) As FastaHeader
            Dim title = Strings.Trim(fasta.Title)
            Dim orgName = title.Matches("\[.+?\]").Last
            Dim xref = title.Split.First

            title = title.Remove(orgName, RegexOptions.None)
            title = title.Remove(xref, RegexOptions.None)

            Dim geneName = title.Matches("\(.+?\)").First
            Dim external = xref.Match("\(.+\)")
            Dim VFID = xref.Remove(external, RegexOptions.None)

            title = title.Remove(geneName, RegexOptions.None).Trim
            xref = external.GetStackValue("(", ")")
            orgName = orgName.GetStackValue("[", "]")
            geneName = geneName.GetStackValue("(", ")")

            Return New FastaHeader With {
                .VFID = VFID,
                .xref = xref,
                .geneName = geneName,
                .fullName = title,
                .organism = orgName
            }
        End Function
    End Class

End Namespace
