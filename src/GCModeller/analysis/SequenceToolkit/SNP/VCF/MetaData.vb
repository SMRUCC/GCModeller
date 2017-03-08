Imports Microsoft.VisualBasic.Data.csv.IO
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
        Public Shared Function ParseMeta(file$) As MetaData
            Dim table As Dictionary(Of String, String()) = GenericMeta.TryParseMetaDataRows(file)
            Dim defaultArray$() = {}
            Dim getValue = Function(key$)
                               Return table.TryGetValue(key, [default]:=defaultArray)
                           End Function

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
