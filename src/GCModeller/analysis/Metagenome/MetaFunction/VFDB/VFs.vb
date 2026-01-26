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


' Code Statistics:

'   Total Lines: 48
'    Code Lines: 35 (72.92%)
' Comment Lines: 3 (6.25%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 10 (20.83%)
'     File Size: 1.59 KB


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
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace VFDB

    ''' <summary>
    ''' http://www.mgc.ac.cn/VFs/download.htm
    ''' </summary>
    Public Class VFs : Implements IPolymerSequenceModel, INamedValue

        Public Property VFID As String Implements INamedValue.Key
        Public Property xref As String
        Public Property geneName As String
        Public Property fullName As String
        Public Property organism As String

        Public Property sequence As String Implements SequenceModel.IPolymerSequenceModel.SequenceData

        Public Overrides Function ToString() As String
            Return fullName
        End Function

        Public Shared Iterator Function Parse(seqs As IEnumerable(Of FastaSeq)) As IEnumerable(Of VFs)
            For Each fa As FastaSeq In seqs
                Yield ParseHeader(fa)
            Next
        End Function

        Public Shared Function ParseHeader(fasta As FastaSeq) As VFs
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

            Return New VFs With {
                .VFID = VFID,
                .xref = xref,
                .geneName = geneName,
                .fullName = title,
                .organism = orgName,
                .sequence = fasta.SequenceData
            }
        End Function
    End Class

End Namespace
