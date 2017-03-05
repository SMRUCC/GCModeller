Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
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
            DOOR.DOOROperonView = API.CreateOperonView(DOOR)
            Return DOOR
        End Function

        ''' <summary>
        ''' 文本的标题行
        ''' </summary>
        Const docTitle As String = "OperonID	GI	Synonym	Start	End	Strand	Length	COG_number	Product"

        <ExportAPI("Doc.Create")>
        <Extension>
        Public Function GenerateDocument(data As IEnumerable(Of Operon)) As String
            Dim LQuery As String() = LinqAPI.Exec(Of String) <=
 _
                From Operon As Operon
                In data
                Select From gene As OperonGene
                       In Operon.Value
                       Let strand = If(gene.Location.Strand = Strands.Forward, "+", "-")
                       Let rowData = {
                           Operon.Key,
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

            Dim sb As New StringBuilder(1024)

            Call sb.AppendLine(docTitle)

            For Each Line In LQuery
                Call sb.AppendLine(Line)
            Next

            Dim value As String = sb.ToString
            Return value
        End Function
    End Module
End Namespace