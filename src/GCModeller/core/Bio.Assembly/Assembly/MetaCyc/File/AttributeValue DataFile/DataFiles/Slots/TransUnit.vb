#Region "Microsoft.VisualBasic::a41f75c549a1b212a69f30f5d2f0b677, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\TransUnit.vb"

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

    '   Total Lines: 68
    '    Code Lines: 17
    ' Comment Lines: 42
    '   Blank Lines: 9
    '     File Size: 3.57 KB


    '     Class TransUnit
    ' 
    '         Properties: Components, ExtentUnknown, Table
    ' 
    '         Function: GetGeneIds
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

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
    Public Class TransUnit : Inherits MetaCyc.File.DataFiles.Slots.Object
        ''' <summary>
        ''' The Components slot of a transcription unit lists the DNA segments within the transcription
        ''' unit, including transcription start sites (Promoters), Terminators, DNA binding sites,
        ''' and genes.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Components As List(Of String)

        ''' <summary>
        ''' The value of this slot should be True when it is not known to how many genes the transcription
        ''' unit extends; that is, it is not known which is the last gene in the transcription
        ''' unit.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="EXTENT-UNKNOWN?")> Public Property ExtentUnknown As String

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.transunits
            End Get
        End Property

        ''' <summary>
        ''' 从所有的基因标号的列表中查询出本转录单元对象中是基因对象的标识号的集合
        ''' </summary>
        ''' <param name="GeneList">All Gene List</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetGeneIds(GeneList As String()) As String()
            Dim Query = From Id As String In Components Where Array.IndexOf(GeneList, Id) > -1 Select Id '
            Return Query.ToArray
        End Function

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As TransUnit
        '    Dim NewObj As TransUnit = New TransUnit

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of MetaCyc.File.DataFiles.Slots.TransUnit) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Genes.AttributeList, e), NewObj)

        '    If NewObj.Object.ContainsKey("EXTENT-UNKNOWN?") Then NewObj.ExtentUnknown = NewObj.Object("EXTENT-UNKNOWN?") Else NewObj.ExtentUnknown = String.Empty
        '    NewObj.Components = StringQuery(NewObj.Object, "COMPONENTS( \d+)?")

        '    Return NewObj
        'End Operator
    End Class
End Namespace
