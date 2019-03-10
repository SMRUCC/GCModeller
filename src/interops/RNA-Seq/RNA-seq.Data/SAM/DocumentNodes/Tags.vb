#Region "Microsoft.VisualBasic::a8592bd4ef912c67a425f352512b82a8, RNA-Seq\RNA-seq.Data\SAM\DocumentNodes\Tags.vb"

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

    '     Enum Tags
    ' 
    '         CO, PG, RG, SQ
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace SAM

    Public Enum Tags As Byte
        ''' <summary>
        ''' The header line. The first line if present.
        ''' </summary>
        ''' 
        <Description("The header line. The first line if present.")>
        HD = 0
        ''' <summary>
        ''' Reference sequence dictionary. The order of @SQ lines defines the alignment sorting order.
        ''' </summary>
        ''' 
        <Description("Reference sequence dictionary. The order of @SQ lines defines the alignment sorting order.")>
        SQ
        ''' <summary>
        ''' Read group. Unordered multiple @RG lines are allowed.
        ''' </summary>
        ''' 
        <Description("Read group. Unordered multiple @RG lines are allowed.")>
        RG
        ''' <summary>
        ''' Program.
        ''' </summary>
        ''' 
        <Description("Program.")>
        PG
        ''' <summary>
        ''' One-line text comment. Unordered multiple @CO lines are allowed.
        ''' </summary>
        ''' 
        <Description("One-line text comment. Unordered multiple @CO lines are allowed.")>
        CO
    End Enum
End Namespace
