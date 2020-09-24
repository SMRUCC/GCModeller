#Region "Microsoft.VisualBasic::eb9eae6714fdd0340f0a1409441d7558, analysis\Motifs\CRISPR\sgRNAcas\Result.vb"

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

    ' Class SingleStrandResult
    ' 
    '     Properties: [End], CRISPR_target_sequence, GC, Length, sgRID
    '                 Start
    ' 
    '     Function: LoadDocument, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

''' <summary>
''' A.Final_report/report_protospacer_single.txt
''' </summary>
''' <remarks></remarks>
Public Class SingleStrandResult
    Public Property sgRID As String
    Public Property Start As Integer
    Public Property [End] As Integer
    <Column("CRISPR_target_sequence(5'-3')")> Public Property CRISPR_target_sequence As String
    <Column("Length(nt)")> Public ReadOnly Property Length As Integer
        Get
            Return Len(CRISPR_target_sequence)
        End Get
    End Property
    <Column("GC%")> Public Property GC As Double

    Public Overrides Function ToString() As String
        Return String.Join(vbTab, {sgRID, Start, [End], CRISPR_target_sequence, Length, GC & " %"})
    End Function

    Public Shared Function LoadDocument(Path As String) As SingleStrandResult()
        Dim Csv = DataImports.Imports(Path, vbTab)
        Return Csv.AsDataSource(Of SingleStrandResult)(False).ToArray
    End Function
End Class
