#Region "Microsoft.VisualBasic::3153ddaa5eb20f8b1170782e340636c8, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\TranscriptionUnits.vb"

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

    '   Total Lines: 38
    '    Code Lines: 19
    ' Comment Lines: 16
    '   Blank Lines: 3
    '     File Size: 2.16 KB


    '     Class TransUnits
    ' 
    '         Properties: AttributeList
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' Frames in this class encode transcription units, which are defined as a set of genes and
    ''' associated control regions that produce a single transcript. Thus, there is a one-to-one
    ''' correspondence between transcription start sites and transcription units. If a set of genes
    ''' is controlled by multiple transcription start sites, then a PGDB should define multiple
    ''' transcription-unit frames, one for each transcription start site.
    ''' (在本类型中所定义的对象编码一个转录单元，一个转录单元定义了一个基因及与其相关联的转录调控DNA片段
    ''' 的集合，故而，在本对象中有一个与转录单元相一一对应的转录起始位点。假若一个基因簇是由多个转录起始
    ''' 位点所控制的，那么将会在MetaCyc数据库之中分别定义与这些转录起始位点相对应的转录单元【即，每一个
    ''' 本类型的对象的属性之中，仅有一个转录起始位点属性】)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TransUnits : Inherits DataFile(Of Slots.TransUnit)

        ''' <summary>
        ''' 定义在转录单元类型之中的所有的属性的列表
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "CITATIONS", "COMMENT",
                    "COMMENT-INTERNAL", "COMPONENT-OF", "COMPONENTS", "CREDITS",
                    "DATA-SOURCE", "DBLINKS", "DOCUMENTATION", "EXTENT-UNKNOWN?",
                    "HIDE-SLOT?", "INSTANCE-NAME-TEMPLATE", "LEFT-END-POSITION",
                    "MEMBER-SORT-FN", "REGULATED-BY", "RIGHT-END-POSITION",
                    "SYNONYMS", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace
