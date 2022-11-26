#Region "Microsoft.VisualBasic::9fa7da6fdb0e5528bca2c039bd5031a0, GCModeller\annotations\GSEA\FisherCore\EnrichmentResult.vb"

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

    '   Total Lines: 20
    '    Code Lines: 14
    ' Comment Lines: 3
    '   Blank Lines: 3
    '     File Size: 567 B


    ' Class EnrichmentResult
    ' 
    '     Properties: cluster, description, enriched, FDR, geneIDs
    '                 name, pvalue, score, term
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' The GCModeller enrichment analysis output table
''' </summary>
Public Class EnrichmentResult

    Public Property term As String
    Public Property name As String
    Public Property description As String
    Public Property geneIDs As String()
    Public Property score As Double
    Public Property pvalue As Double
    Public Property FDR As Double
    Public Property cluster As Integer
    Public Property enriched As String

    Public Overrides Function ToString() As String
        Return term
    End Function

End Class
