﻿#Region "Microsoft.VisualBasic::3f2f41c37fe977bdf2752fcc20ccfd8f, visualize\Circos\Circos\ConfFiles\Nodes\Base\Rule.vb"

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

    '     Class ConditionalRule
    ' 
    '         Properties: color, condition
    ' 
    '         Function: Build
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Configurations.Nodes.Plots

    Public Class ConditionalRule : Inherits CircosDocument
        Implements ICircosDocNode

        <Circos> Public Property condition As String = "var(value) > 0.6"
        <Circos> Public Property color As String = "red"

        Public Overrides Function Build(IndentLevel As Integer) As String
            Return Me.GenerateCircosDocumentElement("rule", IndentLevel, Nothing)
        End Function
    End Class
End Namespace
