#Region "Microsoft.VisualBasic::b79ca46748d9608a69a7784f9882e272, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Gene.vb"

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

    '   Total Lines: 123
    '    Code Lines: 31
    ' Comment Lines: 75
    '   Blank Lines: 17
    '     File Size: 6.47 KB


    '     Class Gene
    ' 
    '         Properties: Accession1, Accession2, CentisomePosition, ComponentOf, InParalogousGeneGroup
    '                     Interrupted, LastUpdate, LeftEndPosition, Product, ProductString
    '                     RightEndPosition, Table, TranscriptionDirection
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    Public Class Gene : Inherits [Object]

        ''' <summary>
        ''' These slots encode the position of the left and right ends of the gene on the 
        ''' chromosome or plasmid on which the gene resides. "Left" means the end of the 
        ''' gene toward the coordinate-system origin (0). Therefore, the Left-End-Position 
        ''' is always less than the Right-End-Position.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property LeftEndPosition As String

        ''' <summary>
        ''' These slots encode the position of the left and right ends of the gene on the 
        ''' chromosome or plasmid on which the gene resides. "Left" means the end of the 
        ''' gene toward the coordinate-system origin (0). Therefore, the Left-End-Position 
        ''' is always less than the Right-End-Position.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property RightEndPosition As String

        ''' <summary>
        ''' This slot lists the map position of this gene on the chromosome in centisome units 
        ''' (percentage length of the chromosome). The centisome-position values are computed 
        ''' automatically by Pathway Tools from the Left-End-Position slot. The value is a number 
        ''' between 0 and 100, inclusive.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property CentisomePosition As String

        ''' <summary>
        ''' This slot specifies the direction along the chromosome in which this gene is transcribed; 
        ''' allowable values are "+" and "-".
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property TranscriptionDirection As String

        ''' <summary>
        ''' This slot holds the ID of a polypeptide or tRNA frame, which is the product of this gene. 
        ''' This slot may contain multiple values for two possible reasons: a given gene might be 
        ''' translated from more than one start codon, giving rise to products of different lengths; 
        ''' the product of the gene may undergo chemical modification. In the latter case, the gene 
        ''' lists all modified forms of the protein in its Product slot.(对于MetaCyc数据库而言，本属性
        ''' 值包含有所有类型的蛋白质对象的UniqueID，但是在编译后的计算机模型之中，仅包含有不同启动子而形成
        ''' 的所有不同长度的多肽链)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("proteins", "express", ExternalKey.Directions.Out)> <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Product As List(Of String)

        ''' <summary>
        ''' If True, indicates that the specified gene is interrupted, that is, has a premature stop codon.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="INTERRUPTED?")> Public Property Interrupted As String

        <ExternalKey("transunit", "has component", ExternalKey.Directions.In)> <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property ComponentOf As List(Of String)
        <MetaCycField()> Public Property InParalogousGeneGroup As String
        <MetaCycField()> Public Property LastUpdate As String
        <MetaCycField()> Public Property ProductString As String

        ''' <summary>
        ''' The unique identifier of this gene object in the NCBI genbak database.
        ''' (本基因对象在NCBI Genbak数据库之中的唯一标识符)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="ACCESSION-1", type:=MetaCycField.Types.String)> Public Property Accession1 As String
        <MetaCycField(name:="ACCESSION-2", type:=MetaCycField.Types.String)> Public Property Accession2 As String

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.genes
            End Get
        End Property

        Public Overrides Function ToString() As String
            If String.Equals(Accession1, CommonName) Then
                Return String.Format("{0} ({1})", Identifier, Accession1)
            Else
                Return String.Format("{0} ({1}, {2})", Identifier, Accession1, CommonName)
            End If
        End Function

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As Gene
        '    Dim NewObj As Gene = New Gene

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of MetaCyc.File.DataFiles.Slots.Gene) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Genes.AttributeList, e), NewObj)

        '    If NewObj.Object.ContainsKey("LEFT-END-POSITION") Then NewObj.LeftEndPosition = NewObj.Object("LEFT-END-POSITION")
        '    If NewObj.Object.ContainsKey("RIGHT-END-POSITION") Then NewObj.RightEndPosition = NewObj.Object("RIGHT-END-POSITION")
        '    If NewObj.Object.ContainsKey("TRANSCRIPTION-DIRECTION") Then NewObj.TranscriptionDirection = NewObj.Object("TRANSCRIPTION-DIRECTION")
        '    If NewObj.Object.ContainsKey("INTERRUPTED?") Then NewObj.Interrupted = NewObj.Object("INTERRUPTED?")
        '    If NewObj.Object.ContainsKey("ACCESSION-1") Then NewObj.Accession1 = NewObj.Object("ACCESSION-1")
        '    If NewObj.Object.ContainsKey("ACCESSION-2") Then NewObj.Accession2 = NewObj.Object("ACCESSION-2")
        '    If NewObj.Object.ContainsKey("LAST-UPDATE") Then NewObj.LastUpdate = NewObj.Object("LAST-UPDATE")

        '    NewObj.DBLinks = StringQuery(NewObj.Object, "DBLINKS( \d+)?")
        '    NewObj.ComponentOf = StringQuery(NewObj.Object, "COMPONENT-OF( \d+)?")
        '    NewObj.Product = StringQuery(NewObj.Object, "PRODUCT( \d+)?")

        '    Return NewObj
        'End Operator
    End Class
End Namespace
