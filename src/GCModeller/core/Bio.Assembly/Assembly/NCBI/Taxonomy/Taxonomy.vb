Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.NCBI

    Public Class TaxiValue

        Public Property Name As String
        ''' <summary>
        ''' Other tag value
        ''' </summary>
        ''' <returns></returns>
        Public Property x As String
        Public Property Title As String
        Public Property taxid As String
        Public Property TaxonomyTree As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' {gi, title}
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function BuildHash(source As IEnumerable(Of TaxiValue)) As Dictionary(Of String, String)
            Dim Groups = From p
                         In source
                         Select p
                         Group By gi = p.Name Into Group
            Return Groups.ToDictionary(
                Function(x) x.gi,
                Function(x) x.Group.Select(
                Function(o) o.Title).JoinBy("; "))
        End Function
    End Class

    Public Class TaxonValue : Inherits ClassObject

        ''' <summary>
        ''' Class level
        ''' </summary>
        ''' <returns></returns>
        Public Property Rank As String
        Public Property nodes As TaxonValue()
        ''' <summary>
        ''' ``gi -> gbff.head``
        ''' </summary>
        ''' <returns></returns>
        Public Property sp As NamedValue(Of BriefInfo)
    End Class

    Public Class BriefInfo : Inherits ClassObject

    End Class

    ''' <summary>
    ''' Build Taxonomy tree from NCBI genbank data.
    ''' </summary>
    Public Module Taxonomy

        ''' <summary>
        ''' 根绝文件的拓展名来识别
        ''' </summary>
        ''' <param name="dmp"></param>
        ''' <returns></returns>
        Public Function AcquireAuto(dmp As String) As Dictionary(Of Integer, Integer)
            If String.Equals(dmp.Split("."c).Last, "bin", StringComparison.OrdinalIgnoreCase) Then
                Return Taxonomy.LoadArchive(bin:=dmp)
            Else
                Return Taxonomy.Hash_gi2Taxi(dmp)
            End If
        End Function

        ''' <summary>
        ''' Probably you should do the match in the bash first by using script"
        ''' 
        ''' ```bash
        ''' grep ">" nt-18S.fasta | cut -f2 -d'|' | sort | uniq >gi.txt
        ''' tabtk_subset /biostack/database/taxonomy/gi_taxid_nucl.dmp gi.txt 1 0 >gi_match.txt 
        ''' ```
        ''' 
        ''' Then using the generated ``gi_match.txt`` as the inputs for parameter <paramref name="dmp"/>, 
        ''' this operation will save your time, no needs to load the entire database.
        ''' </summary>
        ''' <param name="dmp"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Hash_gi2Taxi(dmp As String) As Dictionary(Of Integer, Integer)
            Dim hash As New Dictionary(Of Integer, Integer)

            For Each line As String In dmp.IterateAllLines
                Dim tokens As String() = line.Split(tab)
                Dim ns = tokens.ToArray(Function(s) CInt(s))
                Dim k As Integer = ns(0), v As Integer = ns(1)

                hash(k) = v
            Next

            Return hash
        End Function

        Const tab As Char = CChar(vbTab)

        ''' <summary>
        ''' 将文本数据库转换为二进制数据库已减少文件体积和加快文件的加载速度
        ''' </summary>
        ''' <param name="dmp"></param>
        ''' <param name="bin"></param>
        ''' <returns></returns>
        Public Function Archive(dmp As String, bin As String) As Boolean
            Using writer = bin.OpenWriter
                For Each line As String In dmp.IterateAllLines
                    Dim tokens As String() = line.Split(tab)
                    Dim byts As Byte() = tokens _
                        .Select(Function(s) CInt(s)) _
                        .ToArray(AddressOf BitConverter.GetBytes).MatrixToVector
                    Call writer.BaseStream.Write(byts, Scan0, byts.Length)
                Next
            End Using

            Return True
        End Function

        Public Function LoadArchive(bin As String, Optional bufSize As Integer = 128 * 1024 * 1024 * 2 * RawStream.INT32) As Dictionary(Of Integer, Integer)
            Using reader As New BinaryReader(New FileStream(bin, FileMode.Open))
                Dim hash As New Dictionary(Of Integer, Integer)
                Dim bs As Stream = reader.BaseStream
                Dim bl As Long = bs.Length

                Call "Start to load gi2taxi database into memory....".__DEBUG_ECHO

                Do While bs.Position < bl
                    Dim k = reader.ReadBytes(RawStream.INT32)
                    Dim v = reader.ReadBytes(RawStream.INT32)

                    If k.Length <> 4 OrElse v.Length <> 4 Then
                    Else
                        hash(BitConverter.ToInt32(k, Scan0)) = BitConverter.ToInt32(v, Scan0)
                    End If
                Loop

                Call "JOB DONE!".__DEBUG_ECHO

                Return hash
            End Using
        End Function
    End Module
End Namespace