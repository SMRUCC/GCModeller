#Region "Microsoft.VisualBasic::1344635b015ae23f19edee630459bea1, ..\GCModeller\core\Bio.Assembly\Metagenomics\Taxonomy.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Namespace Metagenomics

    Public Structure Taxonomy

        Public Property scientificName As String
        ''' <summary>
        ''' 1. 域
        ''' </summary>
        Public Property domain As String
        ''' <summary>
        ''' 2. 界
        ''' </summary>
        Public Property kingdom As String
        ''' <summary>
        ''' 3. 门
        ''' </summary>
        Public Property phylum As String
        ''' <summary>
        ''' 4A. 纲
        ''' </summary>
        Public Property [class] As String
        ''' <summary>
        ''' 5B. 目
        ''' </summary>
        Public Property order As String
        ''' <summary>
        ''' 6C. 科
        ''' </summary>
        Public Property family As String
        ''' <summary>
        ''' 7D. 属
        ''' </summary>
        Public Property genus As String
        ''' <summary>
        ''' 8E. 种
        ''' </summary>
        Public Property species As String

        Public Overrides Function ToString() As String
            Return scientificName
        End Function
    End Structure
End Namespace
