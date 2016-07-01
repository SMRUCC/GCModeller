#Region "Microsoft.VisualBasic::bc0778af30a1fcf978018e06e8dba433, ..\GCModeller\analysis\Xfam\Pfam\Parser\PfamString\DomainObject.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ProteinModel

Namespace PfamString

    ''' <summary>
    ''' 这个数据结构是对ChouFasman结构而言的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DomainObject : Inherits SMRUCC.genomics.ProteinModel.DomainObject

        ''' <summary>
        ''' 在Pfam-String之中的位置，其格式为<see cref="SMRUCC.genomics.ProteinModel.DomainObject.Identifier"></see>
        ''' _Handle*<see cref="SMRUCC.genomics.ProteinModel.DomainObject.Identifier"></see>_Handle
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Id_Handle As String
        Public Property ProteinId As String
    End Class
End Namespace
