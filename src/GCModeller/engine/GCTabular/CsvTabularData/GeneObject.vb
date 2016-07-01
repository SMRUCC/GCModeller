#Region "Microsoft.VisualBasic::322316de2b7b5cab2d0cd649521b7ab3, ..\GCModeller\engine\GCTabular\CsvTabularData\GeneObject.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace FileStream

    ''' <summary>
    ''' Gene Annotiation
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneObject : Implements sIdEnumerable

        ''' <summary>
        ''' NCBI gene accession id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Identifier As String Implements sIdEnumerable.Identifier
        Public Property GeneName As String
        Public Property COG As String

        ''' <summary>
        ''' ���������ת¼������<see cref="Transcript.UniqueId">RNA���Ӳ����UniqueId����ֵ</see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TranscriptProduct As String
        ''' <summary>
        ''' һ������֮��ͨ��������ɸ�motif����ͬ���صģ���ÿһ��motif�п��ܻᱻ�����������������
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MotifSites As String()

        Public Overrides Function ToString() As String
            Return GeneName
        End Function

        Public Shared Function CreateObject(GenesTable As Genes, MyvaCOG As MyvaCOG()) As GeneObject()
            Dim LQuery = (From Gene As Slots.Gene
                          In GenesTable.AsParallel
                          Let CogItem = MyvaCOG.GetItem(Gene.Accession1)
                          Let GeneObject = New GeneObject With {
                              .Identifier = Gene.Accession1,
                              .GeneName = Gene.CommonName,
                              .COG = If(CogItem Is Nothing, "", CogItem.Category)
                          }
                          Select GeneObject).ToArray
            Return LQuery
        End Function

        Public Shared Function CreateObject(GenesBrief As IEnumerable(Of GeneBrief), MyvaCOG As MyvaCOG()) As GeneObject()
            Dim LQuery = (From Gene As GeneBrief In GenesBrief.AsParallel
                          Let CogItem = MyvaCOG.GetItem(Gene.Synonym)
                          Let GeneObject = New GeneObject With {
                              .Identifier = Gene.Synonym,
                              .GeneName = Gene.Product,
                              .COG = If(CogItem Is Nothing, "", CogItem.Category)
                          }
                          Select GeneObject).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
