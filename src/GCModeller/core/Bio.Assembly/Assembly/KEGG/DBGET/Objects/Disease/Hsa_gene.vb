#Region "Microsoft.VisualBasic::084b0e0e08ea74da7405c894d81c56be, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Disease\Hsa_gene.vb"

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

    '   Total Lines: 84
    '    Code Lines: 51
    ' Comment Lines: 22
    '   Blank Lines: 11
    '     File Size: 3.03 KB


    '     Class Hsa_gene
    ' 
    '         Properties: AA, Definition, Disease, DrugTarget, Entry
    '                     GeneName, Modules, MyNames, MyPositions, NT
    '                     OtherDBs, Pathway, Position
    ' 
    '         Function: Match, MatchAnyName, MatchAnyPosition, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' The data model of the genes in the human genome.(人类基因组之中的基因模型)    
    ''' </summary>
    <XmlRoot("HumanGene")> Public Class Hsa_gene : Implements INamedValue

        Public Property Entry As String Implements IKeyedEntity(Of String).Key
        Public Property GeneName As String
        Public Property Definition As KeyValuePair
        Public Property Pathway As NamedValue()
        Public Property Disease As NamedValue()
        Public Property DrugTarget As KeyValuePair()
        ' Public Property Motif As NamedCollection(Of String)
        ' Public Property [Structure] As NamedCollection(Of String)
        Public Property Position As String
        Public Property AA As String
        Public Property NT As String
        Public Property OtherDBs As DBLink()
        Public Property Modules As NamedValue()

        ''' <summary>
        ''' Split of the <see cref="GeneName"/> property value
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MyNames As String()
            Get
                Return GeneName.Replace("'", "").Trim.StringSplit(",\s+")
            End Get
        End Property

        Public ReadOnly Property MyPositions As String()
            Get
                Return Position.StringSplit("\s+and\s+")
            End Get
        End Property

        ''' <summary>
        ''' + 两个都匹配，则返回2
        ''' + 匹配任意一个，则返回1
        ''' + 匹配不上任何一个，则返回0
        ''' </summary>
        ''' <param name="pos$"></param>
        ''' <param name="symbol$"></param>
        ''' <returns></returns>
        Public Function Match(pos$, symbol$) As Integer
            Dim n%

            n += If(MatchAnyPosition(pos), 1, 0)
            n += If(MatchAnyName(symbol), 1, 0)

            Return n
        End Function

        Public Function MatchAnyPosition(pos$) As Boolean
            Return Not MyPositions _
                .Where(Function(l) l.TextEquals(pos)) _
                .FirstOrDefault _
                .StringEmpty
        End Function

        ''' <summary>
        ''' 目标输入的基因名称符号能够匹配上这个基因对象的任意一个名称
        ''' </summary>
        ''' <param name="symbol$"></param>
        ''' <returns></returns>
        Public Function MatchAnyName(symbol$) As Boolean
            Return Not MyNames _
                .Where(Function(s$) s.TextEquals(symbol)) _
                .FirstOrDefault _
                .StringEmpty
        End Function

        Public Overrides Function ToString() As String
            Return Definition.ToString
        End Function
    End Class
End Namespace
