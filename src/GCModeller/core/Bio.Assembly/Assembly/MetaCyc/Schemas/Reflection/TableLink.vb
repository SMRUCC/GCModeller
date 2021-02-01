#Region "Microsoft.VisualBasic::919bbd4316c18faf23e73e9c3e2b2d90, core\Bio.Assembly\Assembly\MetaCyc\Schemas\Reflection\TableLink.vb"

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

    '     Class ExternalKey
    ' 
    ' 
    '         Enum Directions
    ' 
    '             [In], [Out], Unknown
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: Direction, PropertyInfo, TableList
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Link, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.MetaCyc.Schema.Reflection

    ''' <summary>
    ''' 表示所标识的域为表与表之间的外键
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property, allowmultiple:=False, inherited:=True)>
    Public Class ExternalKey : Inherits Attribute

        Public Enum Directions
            ''' <summary>
            ''' 由其他对象指向本对象的关系枚举
            ''' </summary>
            ''' <remarks></remarks>
            [In]
            ''' <summary>
            ''' 由本对象指向其他对象的关系枚举
            ''' </summary>
            ''' <remarks></remarks>
            [Out]
            ''' <summary>
            ''' 未知的关系
            ''' </summary>
            ''' <remarks></remarks>
            Unknown
        End Enum

        Public Property TableList As MetaCyc.File.DataFiles.Slots.Object.Tables()
        Public Property Direction As MetaCyc.Schema.Reflection.ExternalKey.Directions
        Protected Friend Property PropertyInfo As System.Reflection.PropertyInfo

        Dim _Tables As String

        Protected Friend Shared Converts As Dictionary(Of String, MetaCyc.File.DataFiles.Slots.Object.Tables) =
            New Dictionary(Of String, File.DataFiles.Slots.Object.Tables) From
            {
                {"bindrxns", MetaCyc.File.DataFiles.Slots.Object.Tables.bindrxns},
                {"classes", MetaCyc.File.DataFiles.Slots.Object.Tables.classes},
                {"compounds", MetaCyc.File.DataFiles.Slots.Object.Tables.compounds},
                {"dnabindsites", MetaCyc.File.DataFiles.Slots.Object.Tables.dnabindsites},
                {"enzrxns", MetaCyc.File.DataFiles.Slots.Object.Tables.enzrxns},
                {"genes", MetaCyc.File.DataFiles.Slots.Object.Tables.genes},
                {"pathways", MetaCyc.File.DataFiles.Slots.Object.Tables.pathways},
                {"promoters", MetaCyc.File.DataFiles.Slots.Object.Tables.promoters},
                {"proteinfeatures", MetaCyc.File.DataFiles.Slots.Object.Tables.proteinfeatures},
                {"proteins", MetaCyc.File.DataFiles.Slots.Object.Tables.proteins},
                {"protligandcplxes", MetaCyc.File.DataFiles.Slots.Object.Tables.protligandcplxes},
                {"pubs", MetaCyc.File.DataFiles.Slots.Object.Tables.pubs},
                {"reactions", MetaCyc.File.DataFiles.Slots.Object.Tables.reactions},
                {"regulation", MetaCyc.File.DataFiles.Slots.Object.Tables.regulation},
                {"regulons", MetaCyc.File.DataFiles.Slots.Object.Tables.regulons},
                {"species", MetaCyc.File.DataFiles.Slots.Object.Tables.species},
                {"terminators", MetaCyc.File.DataFiles.Slots.Object.Tables.terminators},
                {"transunits", MetaCyc.File.DataFiles.Slots.Object.Tables.transunits},
                {"", MetaCyc.File.DataFiles.Slots.Object.Tables.classes}}

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="NameList">表名称列表，每一个表之间使用逗号分隔</param>
        ''' <param name="Direction"></param>
        ''' <remarks></remarks>
        Sub New(NameList As String, RelationDescription As String, Optional Direction As ExternalKey.Directions = Directions.In)
            _Direction = Direction
            _Tables = NameList
            Dim LQuery = From TableName As String In NameList.Split(CChar(",")) Let [enum] = Converts(TableName) Select [enum] '
            Me._TableList = LQuery.ToArray
        End Sub

        Protected Friend Function Link([Property] As System.Reflection.PropertyInfo) As ExternalKey
            Me.PropertyInfo = [Property]
            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}], direction: {1}", _Tables, _Direction.ToString)
        End Function
    End Class

    '<AttributeUsage(AttributeTargets.Class, allowmultiple:=False, inherited:=True)>
    'Public Class Table : Inherits Attribute

    '    Public Property Id As MetaCyc.File.DataFiles.Slots.Object.Tables

    '    Sub New(Id As MetaCyc.File.DataFiles.Slots.Object.Tables)
    '        Me.Id = Id
    '    End Sub

    '    Public Overrides Function ToString() As String
    '        Return Id.ToString
    '    End Function
    'End Class
End Namespace
