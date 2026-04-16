#Region "Microsoft.VisualBasic::4081901616496600bd30aa74c49b7801, RNA-Seq\RNA-seq.Data\SAM\IndexStats.vb"

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

    '   Total Lines: 85
    '    Code Lines: 61 (71.76%)
    ' Comment Lines: 7 (8.24%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 17 (20.00%)
    '     File Size: 2.94 KB


    '     Class GeneData
    ' 
    '         Properties: GeneID, Length, RawCount, RPK, TPM
    ' 
    '     Class IndexStats
    ' 
    '         Properties: GeneID, Length, RawCount, UnmappedBases
    ' 
    '         Function: ConvertCountsToTPM, Parse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel

Namespace SAM

    ''' <summary>
    ''' gene abundance result
    ''' </summary>
    Public Class GeneData : Implements IExpressionValue

        Public Property GeneID As String Implements IExpressionValue.Identity
        Public Property Length As Double
        Public Property RawCount As Double
        Public Property RPK As Double
        Public Property TPM As Double Implements IExpressionValue.ExpressionValue

    End Class

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

        Public Shared Iterator Function ConvertCountsToTPM(stats As IEnumerable(Of IndexStats)) As IEnumerable(Of GeneData)
            Dim genes As New List(Of GeneData)()
            Dim totalRPK As Double = 0.0

            For Each hit As IndexStats In stats
                Dim gene As New GeneData() With {
                    .GeneID = hit.GeneID,
                    .Length = hit.Length,
                    .RawCount = hit.RawCount,
                    .RPK = (.RawCount * 1000.0) / .Length
                }

                genes.Add(gene)
                totalRPK += gene.RPK
            Next

            ' --- 第二步：根据 totalRPK 计算 TPM ---
            If totalRPK = 0 Then
                Call "Warning: Total RPK is 0. All TPM values will be 0.".warning
            End If

            For Each gene As GeneData In genes
                If totalRPK > 0 Then
                    gene.TPM = (gene.RPK / totalRPK) * 1000000.0
                Else
                    gene.TPM = 0.0
                End If

                Yield gene
            Next
        End Function

    End Class
End Namespace
