#Region "Microsoft.VisualBasic::c1f01219692528e39ad702d382dce3cf, engine\IO\GCTabular\CsvTabularData\GeneObject.vb"

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

    '     Class GeneObject
    ' 
    '         Properties: COG, GeneName, Identifier, MotifSites, TranscriptProduct
    ' 
    '         Function: (+2 Overloads) CreateObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG

Namespace FileStream

    ''' <summary>
    ''' Gene Annotiation
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneObject : Implements INamedValue

        ''' <summary>
        ''' NCBI gene accession id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Identifier As String Implements INamedValue.Key
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
