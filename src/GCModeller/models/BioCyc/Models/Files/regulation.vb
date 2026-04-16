#Region "Microsoft.VisualBasic::a3dcb6f0444dd766e3a18ca1764293ef, models\BioCyc\Models\Files\regulation.vb"

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

    '   Total Lines: 14
    '    Code Lines: 10 (71.43%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (28.57%)
    '     File Size: 372 B


    ' Class regulation
    ' 
    '     Properties: mode, regulatedEntity, regulator
    ' 
    ' /********************************************************************************/

#End Region


Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("regulation.dat")>
Public Class regulation : Inherits Model

    <AttributeField("MODE")>
    Public Property mode As String
    <AttributeField("REGULATED-ENTITY")>
    Public Property regulatedEntity As String
    <AttributeField("REGULATOR")>
    Public Property regulator As String

End Class

