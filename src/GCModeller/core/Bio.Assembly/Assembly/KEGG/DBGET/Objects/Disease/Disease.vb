#Region "Microsoft.VisualBasic::0dbb4e0a3025a8ce04a0370388f75d30, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Disease\Disease.vb"

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

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class Disease : Implements INamedValue

        Public Property Category As String
        Public Property Comment As String
        Public Property Drug As KeyValuePair()
        Public Property Pathway As KeyValuePair()

        Public Property Entry As String Implements IKeyedEntity(Of String).Key
        Public Property Name As String

        Public Property Genes As TripleKeyValuesPair()
        Public Property Markers As String()
        Public Property OtherDBs As KeyValuePair()

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
