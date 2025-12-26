#Region "Microsoft.VisualBasic::aa8d3231f8170c738601123851d926ee, annotations\GSEA\FisherCore\EnrichmentResult.vb"

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

'   Total Lines: 33
'    Code Lines: 16 (48.48%)
' Comment Lines: 11 (33.33%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 6 (18.18%)
'     File Size: 1.05 KB


' Class EnrichmentResult
' 
'     Properties: [class], category, cluster, description, enriched
'                 FDR, IDs, name, pvalue, score
'                 term
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.Statistics

''' <summary>
''' The GCModeller enrichment analysis output table
''' </summary>
Public Class EnrichmentResult : Implements IStatPvalue, IStatFDR

    Public Property term As String
    Public Property name As String

    Public Property [class] As String
    Public Property category As String
    Public Property description As String

    Public Property score As Double
    Public Property pvalue As Double Implements IStatPvalue.pValue
    Public Property FDR As Double Implements IStatFDR.adjPVal
    Public Property cluster As Integer
    ''' <summary>
    ''' 我们的差异基因列表中，属于目标代谢途径的基因的数量 / 在我们的差异基因列表中，不属于当前的代谢途径的基因的数量
    ''' </summary>
    ''' <returns></returns>
    Public Property enriched As Integer

    ''' <summary>
    ''' the enriched gene id set: input id set intersect with the background cluster id set.
    ''' </summary>
    ''' <returns></returns>
    Public Property IDs As String()

    Public Overrides Function ToString() As String
        Return term
    End Function

End Class
