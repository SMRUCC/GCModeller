Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports API = SMRUCC.genomics.Assembly.DOOR.DOOR_API

Namespace Assembly.DOOR

    ''' <summary>
    ''' Parser and writer
    ''' </summary>
    Public Module DOOR_IO

        ''' <summary>
        ''' 解析已经读取的文本行为DOOR操纵子集合对象
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function [Imports](data As String(), Optional path$ = Nothing, Optional header As Boolean = True) As DOOR
            Dim LQuery As OperonGene() = LinqAPI.Exec(Of OperonGene) <=
 _
                From line As String
                In data.Skip(If(header, 1, 0))
                Where Not String.IsNullOrEmpty(line)
                Select OperonGene.TryParse(line)

            Dim DOOR As New DOOR With {
                .Genes = LQuery,
                .FilePath = path
            }
            Return DOOR
        End Function

        ''' <summary>
        ''' 文本的标题行
        ''' </summary>
        Const DOOR_title As String = "OperonID	GI	Synonym	Start	End	Strand	Length	COG_number	Product"

        <ExportAPI("Doc.Create")>
        <Extension>
        Public Function Text(data As IEnumerable(Of Operon)) As String
            Dim lines$() = data _
                .Select(AddressOf __lines) _
                .IteratesALL _
                .ToArray
            Dim value$ = {
                DOOR_title
            }.Join(lines) _
            .JoinBy(ASCII.LF)

            Return value
        End Function

        <Extension>
        Private Function __lines(operon As Operon) As String()
            Return LinqAPI.Exec(Of String) <=
 _
                From gene As OperonGene
                In operon.Value
                Let strand = If(gene.Location.Strand = Strands.Forward, "+", "-")
                Let rowData = {
                    operon.Key,
                    gene.GI,
                    gene.Synonym,
                    CStr(gene.Location.Left),
                    CStr(gene.Location.Right),
                    strand,
                    CStr(gene.Location.FragmentSize),
                    gene.COG_number,
                    gene.Product
                }
                Select String.Join(vbTab, rowData)
        End Function
    End Module
End Namespace