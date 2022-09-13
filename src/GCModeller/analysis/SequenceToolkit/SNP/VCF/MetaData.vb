#Region "Microsoft.VisualBasic::6f6775d35259b2962a4d5d841658095a, GCModeller\analysis\SequenceToolkit\SNP\VCF\MetaData.vb"

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

    '   Total Lines: 64
    '    Code Lines: 36
    ' Comment Lines: 14
    '   Blank Lines: 14
    '     File Size: 2.65 KB


    '     Class MetaData
    ' 
    '         Properties: contig, fileDate, fileformat, FILTER, FORMAT
    '                     INFO, reference, source
    ' 
    '         Function: ParseMeta, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace VCF

    Public Class MetaData

        '##fileformat=VCFv4.2
        Public Property fileformat As String

        '##fileDate=20151002
        Public Property fileDate As String

        '##source=callMomV0.2
        Public Property source As String

        '##reference=gi|251831106|ref|NC_012920.1| Homo sapiens mitochondrion, complete genome
        Public Property reference As String

        '##contig=<ID=MT,length=16569,assembly=b37>
        Public Property contig As String

        '##INFO=<ID=VT,Number=.,Type=String,Description="Alternate allele type. S=SNP, M=MNP, I=Indel">
        '##INFO=<ID=AC,Number=.,Type=Integer,Description="Alternate allele counts, comma delimited when multiple">
        Public Property INFO As String()

        '##FILTER=<ID=fa,Description="Genotypes called from fasta file">
        Public Property FILTER As String

        '##FORMAT=<ID=GT,Number=1,Type=String,Description="Genotype">
        Public Property FORMAT As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' 解析出vcf文件之中的元数据
        ''' </summary>
        ''' <param name="file$"></param>
        ''' <returns></returns>
        Public Shared Function ParseMeta(file$, Optional ByRef n% = Integer.MaxValue) As MetaData
            Dim table As Dictionary(Of String, String()) = GenericMeta.TryParseMetaDataRows(file)
            Dim defaultArray$() = {}
            Dim getValue = Function(key$)
                               Return table.TryGetValue(key, [default]:=defaultArray)
                           End Function

            n = table.Values.IteratesALL.Count

            Return New MetaData With {
                .contig = getValue(key:=NameOf(MetaData.contig)).FirstOrDefault,
                .fileDate = getValue(key:=NameOf(MetaData.fileDate)).FirstOrDefault,
                .fileformat = getValue(key:=NameOf(MetaData.fileformat)).FirstOrDefault,
                .FILTER = getValue(key:=NameOf(MetaData.FILTER)).FirstOrDefault,
                .FORMAT = getValue(key:=NameOf(MetaData.FORMAT)).FirstOrDefault,
                .INFO = getValue(key:=NameOf(MetaData.INFO)),
                .reference = getValue(key:=NameOf(MetaData.reference)).FirstOrDefault,
                .source = getValue(key:=NameOf(MetaData.source)).FirstOrDefault
            }
        End Function
    End Class
End Namespace
