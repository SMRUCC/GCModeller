#Region "Microsoft.VisualBasic::5e18ce4308dc71b5ec58f59c387acd41, modules\ExperimentDesigner\DEGModel.vb"

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
    '    Code Lines: 14 (70.00%)
    ' Comment Lines: 4 (20.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (10.00%)
    '     File Size: 657 B


    ' Class DEGModel
    ' 
    '     Properties: [class], label, logFC, pvalue, VIP
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Public Class DEGModel : Implements IDeg

    Public Property label$ Implements IDeg.label
    Public Property logFC# Implements IDeg.log2FC
    Public Property pvalue# Implements IDeg.pvalue
    ''' <summary>
    ''' Variance importance
    ''' </summary>
    ''' <returns></returns>
    Public Property VIP As Double
    Public Property [class] As String

    Public Overrides Function ToString() As String
        If [class].StringEmpty Then
            Return $"[{label}] log2FC={logFC}, pvalue={pvalue}"
        Else
            Return $"[{label} | {[class]}] log2FC={logFC}, pvalue={pvalue}"
        End If
    End Function
End Class
