#Region "Microsoft.VisualBasic::5dd26f999aff82b2f04756cc3e03dc5c, GCModeller\core\Bio.Assembly\Assembly\NCBI\Taxonomy\Taxonomy.vb"

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

    '   Total Lines: 164
    '    Code Lines: 98
    ' Comment Lines: 43
    '   Blank Lines: 23
    '     File Size: 6.30 KB


    '     Class TaxiValue
    ' 
    '         Properties: Name, taxid, TaxonomyTree, Title, x
    ' 
    '         Function: BuildDictionary, ToString
    ' 
    '     Class TaxonValue
    ' 
    '         Properties: nodes, Rank, sp
    ' 
    '     Class BriefInfo
    ' 
    ' 
    ' 
    '     Module Taxonomy
    ' 
    '         Function: __gi2Taxid, __loadArchive, AcquireAuto, Archive, Hash_gi2Taxi
    '                   LoadArchive
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace Assembly.NCBI.Taxonomy

    Public Class TaxiValue

        Public Property Name As String
        ''' <summary>
        ''' Other tagged value
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
        ''' ``{gi, title}``
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function BuildDictionary(source As IEnumerable(Of TaxiValue)) As Dictionary(Of String, String)
            Dim groups = From p
                         In source
                         Select p
                         Group By gi = p.Name Into Group

            Return groups.ToDictionary(Function(x) x.gi,
                                       Function(x)
                                           Return x.Group _
                                               .Select(Function(o) o.Title) _
                                               .JoinBy("; ")
                                       End Function)
        End Function
    End Class

    Public Class TaxonValue

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

    Public Class BriefInfo

    End Class

    ''' <summary>
    ''' Build Taxonomy tree from NCBI genbank data.
    ''' </summary>
    Public Module Taxonomy

        ''' <summary>
        ''' {gi -> taxid}.(根绝文件的拓展名来识别)
        ''' </summary>
        ''' <param name="dmp"></param>
        ''' <returns></returns>
        Public Function AcquireAuto(dmp As String) As BucketDictionary(Of Integer, Integer)
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
        Public Function Hash_gi2Taxi(dmp As String) As BucketDictionary(Of Integer, Integer)
            Dim hash As BucketDictionary(Of Integer, Integer) =
                dmp.__gi2Taxid.CreateBuckets(Function(x) x(Scan0), Function(x) x(1))
            Return hash
        End Function

        <Extension>
        Private Iterator Function __gi2Taxid(dmp As String) As IEnumerable(Of Integer())
            For Each line As String In dmp.IterateAllLines
                Dim tokens As String() = line.Split(tab)
                Dim ns = tokens.Select(Function(s) CInt(s)).ToArray

                Yield ns
            Next
        End Function

        Const tab As Char = CChar(vbTab)

        ''' <summary>
        ''' 将文本数据库转换为二进制数据库已减少文件体积和加快文件的加载速度
        ''' </summary>
        ''' <param name="dmp"></param>
        ''' <param name="bin"></param>
        ''' <returns></returns>
        Public Function Archive(dmp As String, bin As String) As Boolean
            Using writer = bin.OpenWriter(Encodings.ASCII)
                For Each line As String In dmp.IterateAllLines
                    Dim tokens As String() = line.Split(tab)
                    Dim byts As Byte() = tokens _
                        .Select(Function(s) CInt(s)) _
                        .Select(AddressOf BitConverter.GetBytes) _
                        .ToVector
                    Call writer.BaseStream.Write(byts, Scan0, byts.Length)
                Next
            End Using

            Return True
        End Function

        Public Function LoadArchive(bin As String, Optional bufSize As Integer = 128 * 1024 * 1024 * 2 * RawStream.INT32) As BucketDictionary(Of Integer, Integer)
            Using reader As New BinaryReader(New FileStream(bin, FileMode.Open))
                Dim hash As BucketDictionary(Of Integer, Integer)

                Call "Start to load gi2taxi database into memory....".__DEBUG_ECHO
                hash = reader.__loadArchive.CreateBuckets(Function(x) x(Scan0), Function(x) x(1))
                Call "JOB DONE!".__DEBUG_ECHO
                Return hash
            End Using
        End Function

        <Extension>
        Private Iterator Function __loadArchive(reader As BinaryReader) As IEnumerable(Of Integer())
            Dim bs As Stream = reader.BaseStream
            Dim bl As Long = bs.Length

            Do While bs.Position < bl
                Dim k = reader.ReadBytes(RawStream.INT32)
                Dim v = reader.ReadBytes(RawStream.INT32)

                If k.Length <> 4 OrElse v.Length <> 4 Then
                Else
                    Yield {BitConverter.ToInt32(k, Scan0), BitConverter.ToInt32(v, Scan0)}
                End If
            Loop
        End Function
    End Module
End Namespace
