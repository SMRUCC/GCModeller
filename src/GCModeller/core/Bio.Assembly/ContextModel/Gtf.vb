#Region "Microsoft.VisualBasic::70dc25eb0fdd3b86f0b62c96ae364fde, GCModeller\core\Bio.Assembly\ContextModel\Gtf.vb"

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
    '    Code Lines: 42
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.77 KB


    '     Module Gtf
    ' 
    '         Function: doParseOfGeneInfo, ParseFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    Public Module Gtf

        Public Function ParseFile(file As String) As GeneBrief()
            Dim geneLines As String() = file.SolveStream.LineTokens
            Dim genes As GeneBrief() = geneLines _
                .Select(Function(l) l.Split(ASCII.TAB).doParseOfGeneInfo) _
                .ToArray

            Return genes
        End Function

        <Extension>
        Private Function doParseOfGeneInfo(tokens As String()) As GeneBrief
            Dim chr As String = tokens(Scan0)
            Dim seqType As String = tokens(1)
            Dim type As String = tokens(2)
            Dim left As Integer = Integer.Parse(tokens(3))
            Dim right As Integer = Integer.Parse(tokens(4))
            Dim strand As Strands = tokens(6).GetStrand
            Dim info = tokens(8).StringSplit(";\s+") _
                .Select(AddressOf GetTagValue) _
                .ToDictionary(Function(t) t.Name,
                              Function(t)
                                  Return t.Value.Trim(""""c, " "c)
                              End Function)
            Dim geneId As String = info!gene_id

            Return New GeneBrief With {
                .Code = "",
                .COG = "",
                .Gene = geneId,
                .IsORF = True,
                .Length = right - left,
                .Location = New NucleotideLocation(left, right, strand),
                .PID = geneId,
                .Product = geneId,
                .Synonym = geneId
            }
        End Function
    End Module
End Namespace
