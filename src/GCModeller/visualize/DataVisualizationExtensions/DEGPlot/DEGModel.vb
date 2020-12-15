#Region "Microsoft.VisualBasic::9b0ea6d9fca5c1e8ba1f1579151c184a, visualize\DataVisualizationExtensions\DEGPlot\DEGModel.vb"

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

' Structure DEGModel
' 
'     Properties: label, logFC, pvalue
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Public Structure DEGModel : Implements IDeg

    Public Property label$ Implements IDeg.label
    Public Property logFC# Implements IDeg.log2FC
    Public Property pvalue# Implements IDeg.pvalue
    Public Property [class] As String

    Public Overrides Function ToString() As String
        Return $"[{label}] log2FC={logFC}, pvalue={pvalue}"
    End Function
End Structure
