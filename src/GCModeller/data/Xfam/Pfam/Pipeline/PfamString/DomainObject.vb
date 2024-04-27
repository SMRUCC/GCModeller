#Region "Microsoft.VisualBasic::5911ddd7a3c18d7d7a9d2920fcf0b0bb, G:/GCModeller/src/GCModeller/data/Xfam/Pfam//Pipeline/PfamString/DomainObject.vb"

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

    '   Total Lines: 19
    '    Code Lines: 6
    ' Comment Lines: 11
    '   Blank Lines: 2
    '     File Size: 669 B


    '     Class DomainObject
    ' 
    '         Properties: Id_Handle, ProteinId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace PfamString

    ''' <summary>
    ''' 这个数据结构是对ChouFasman结构而言的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DomainObject : Inherits ProteinModel.DomainObject

        ''' <summary>
        ''' 在Pfam-String之中的位置，其格式为<see cref="SMRUCC.genomics.ProteinModel.DomainObject.Name"></see>
        ''' _Handle*<see cref="DomainObject.Name"></see>_Handle
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Id_Handle As String
        Public Property ProteinId As String
    End Class
End Namespace
