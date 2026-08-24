#Region "Microsoft.VisualBasic::5e4a6dd6c5dd8c23b37f4b3854dd72f8, RNA-Seq\RNA-seq.Data\SAM\featureCount\IndexStats.vb"

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

    '   Total Lines: 38
    '    Code Lines: 28 (73.68%)
    ' Comment Lines: 3 (7.89%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (18.42%)
    '     File Size: 1.35 KB


    '     Class IndexStats
    ' 
    '         Properties: GeneID, Length, RawCount, UnmappedBases
    ' 
    '         Function: Parse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Language

Namespace SAM.featureCount

    ''' <summary>
    ''' A row of the samtool indexstats output
    ''' </summary>
    Public Class IndexStats

        Public Property GeneID As String
        Public Property Length As Integer
        Public Property RawCount As Integer
        Public Property UnmappedBases As Integer

        Public Shared Iterator Function Parse(file As Stream) As IEnumerable(Of IndexStats)
            Using str As New StreamReader(file)
                Dim line As Value(Of String) = ""

                Do While (line = str.ReadLine) IsNot Nothing
                    If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("*") OrElse line.StartsWith("@") Then
                        Continue Do
                    End If

                    Dim fields As String() = line.Split(vbTab)
                    Dim gene_count As New IndexStats With {
                        .GeneID = fields(0),
                        .Length = CInt(fields(1)),
                        .RawCount = CInt(fields(2)),
                        .UnmappedBases = CInt(Val(fields.ElementAtOrNull(3)))
                    }

                    Yield gene_count
                Loop
            End Using
        End Function
    End Class
End Namespace
