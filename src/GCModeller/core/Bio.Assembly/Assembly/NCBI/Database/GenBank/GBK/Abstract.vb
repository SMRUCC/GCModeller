#Region "Microsoft.VisualBasic::3ec36872347058407e093000d884f18d, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Abstract.vb"

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

    '   Total Lines: 15
    '    Code Lines: 5
    ' Comment Lines: 8
    '   Blank Lines: 2
    '     File Size: 429 B


    '     Class IgbComponent
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' Genbank数据库文件的构件
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class IgbComponent

        ''' <summary>
        ''' Link to the genbank raw object.(这个构件对象所处在的``genbank``数据库对象.)
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend gb As File
    End Class
End Namespace
