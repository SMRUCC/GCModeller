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

        <Extension>
        Public Function giTaxiHash(dmp As String) As Dictionary(Of Integer, Integer)
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
                Dim buf As Byte()
                Dim bs As Stream = reader.BaseStream
                Dim bl As Long = bs.Length
                Dim ns As Integer()

                Call "Start to load gi2taxi database into memory....".__DEBUG_ECHO

                Do While bs.Position < bl
                    buf = reader.ReadBytes(bufSize)
                    ns = buf.SplitIterator(RawStream.INT32).ToArray(
                        Function(b) BitConverter.ToInt32(b, Scan0))
                    For Each p As Integer() In ns.SplitIterator(2)
                        hash(p(0)) = p(1)
                    Next

                    Call Console.Write("load next buffer")
                Loop

                Call "JOB DONE!".__DEBUG_ECHO

                Return hash
            End Using
        End Function
    End Module
End Namespace