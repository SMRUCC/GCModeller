#Region "Microsoft.VisualBasic::4f6fec9cbef12a8ce9b9223be8b5926c, GCModeller\models\BIOM\BIOM\v1.0\components.vb"

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

    '   Total Lines: 78
    '    Code Lines: 49
    ' Comment Lines: 15
    '   Blank Lines: 14
    '     File Size: 2.53 KB


    '     Class row
    ' 
    '         Properties: hasMetaInfo, id, metadata
    ' 
    '         Function: ToString
    ' 
    '     Class meta
    ' 
    '         Properties: KEGG_Pathways, lineage, taxonomy
    ' 
    '     Class column
    ' 
    '         Properties: id, metadata
    ' 
    '         Function: ToString
    ' 
    '     Class columnMeta
    ' 
    '         Properties: BarcodeSequence, BODY_SITE, Description, LinkerPrimerSequence
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics

Namespace v10.components

    ''' <summary>
    ''' 主要是存储OTU信息
    ''' </summary>
    Public Class row : Implements INamedValue

        Public Property id As String Implements IKeyedEntity(Of String).Key
        Public Property metadata As meta

        Public ReadOnly Property hasMetaInfo As Boolean
            Get
                Return Not metadata Is Nothing AndAlso
                    (Not metadata.KEGG_Pathways.IsNullOrEmpty OrElse
                     Not metadata.taxonomy.IsNullOrEmpty)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    Public Class meta

        ''' <summary>
        ''' 这个字符串数组就是一个taxonomy对象的数据
        ''' </summary>
        ''' <returns></returns>
        Public Property taxonomy As String()
        Public Property KEGG_Pathways As String()

        ''' <summary>
        ''' 将<see cref="taxonomy"/>属性值转换为标准的物种信息数据模型，
        ''' 如果目标字符串数组是空的，则这个属性返回空值
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property lineage As Taxonomy
            Get
                If taxonomy.IsNullOrEmpty Then
                    Return Nothing
                Else
                    Return New Taxonomy(BIOMTaxonomy.TaxonomyParser(taxonomy))
                End If
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 主要是存储sample信息
    ''' </summary>
    Public Class column : Implements INamedValue

        Public Property id As String Implements IKeyedEntity(Of String).Key
        Public Property metadata As columnMeta

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    Public Class columnMeta

        Public Property BarcodeSequence As String
        Public Property LinkerPrimerSequence As String
        Public Property BODY_SITE As String
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
