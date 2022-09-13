#Region "Microsoft.VisualBasic::0801728ddaf15f7cfb9af97c7c6ec2f9, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\SSDB\Models\Enzyme.vb"

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

    '   Total Lines: 28
    '    Code Lines: 16
    ' Comment Lines: 8
    '   Blank Lines: 4
    '     File Size: 886 B


    '     Class Enzyme
    ' 
    '         Properties: [class], comment, commonNames, entry, genes
    '                     IUBMB, orthology, product, reactions, substrate
    '                     sysname
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    Public Class Enzyme

        ''' <summary>
        ''' The ``EC`` number
        ''' </summary>
        ''' <returns></returns>
        Public Property entry As String
        Public Property commonNames As String()
        Public Property [class] As String()
        Public Property sysname As String
        Public Property IUBMB As String
        ''' <summary>
        ''' The kegg reaction id list
        ''' </summary>
        ''' <returns></returns>
        Public Property reactions As String()
        Public Property substrate As String()
        Public Property product As String()
        Public Property comment As String
        Public Property orthology As OrthologyTerms
        Public Property genes As NamedValue()

    End Class
End Namespace
