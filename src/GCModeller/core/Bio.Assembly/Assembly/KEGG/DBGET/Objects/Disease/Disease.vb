#Region "Microsoft.VisualBasic::232e1340f93e74aeadecefebedf6439b, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Disease\Disease.vb"

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

    '   Total Lines: 35
    '    Code Lines: 23
    ' Comment Lines: 5
    '   Blank Lines: 7
    '     File Size: 1.30 KB


    '     Class Disease
    ' 
    '         Properties: Carcinogen, Category, Comment, Description, Drug
    '                     Entry, Genes, Markers, Name, OtherDBs
    '                     Pathway, References
    ' 
    '         Function: HumanGeneID
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class Disease : Implements INamedValue

        Public Property Category As String
        Public Property Comment As String
        Public Property Drug As KeyValuePair()
        Public Property Pathway As NamedValue()

        Public Property Entry As String Implements IKeyedEntity(Of String).Key
        Public Property Name As String

        Public Property Genes As [Property]()
        Public Property Markers As String()
        Public Property OtherDBs As DBLink()

        Public Property References As Reference()
        Public Property Description As String
        Public Property Carcinogen As String

        ''' <summary>
        ''' 从标签文本之中解析出人基因组的基因的编号
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        Public Shared Function HumanGeneID(s$) As String
            Return s.GetStackValue("[", "]").Split(":"c).Last
        End Function
    End Class
End Namespace
