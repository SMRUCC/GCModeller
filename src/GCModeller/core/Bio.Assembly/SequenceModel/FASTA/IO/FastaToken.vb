#Region "Microsoft.VisualBasic::e2a896de3769d582172aab5fef928172, Bio.Assembly\SequenceModel\FASTA\IO\FastaToken.vb"

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

    '     Class FastaSeq
    ' 
    '         Properties: HaveGaps, Headers, Length, locus_tag, SequenceData
    '                     Title
    ' 
    '         Constructor: (+7 Overloads) Sub New
    ' 
    '         Function: Clone, Complement, Copy, Equals, GenerateDocument
    '                   GenerateDocumentText, GetSequenceData, GrepTitle, Load, LoadNucleotideData
    '                   objClone, ParseFromStream, Reverse, Save, SaveAsOneLine
    '                   (+2 Overloads) SaveTo, SequenceLineBreak, ToLower, ToString, ToUpper
    '                   TryParse
    ' 
    '         Sub: AddAttribute, CopyTo, InsertAttribute, RemoveAttribute, SequenceLineBreak
    '              SetAttribute
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace SequenceModel.FASTA

    ''' <summary>
    ''' The FASTA format file of a bimolecular sequence.(Notice that this file is 
    ''' only contains on sequence.)
    ''' FASTA格式的生物分子序列文件。(但是请注意：文件中只包含一条序列的情况，假若需要自定义所生成的FASTA文件的标题的格式，请复写<see cref="FastaSeq.ToString"></see>方法)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("GCModeller.IO.FastaToken", Publisher:="amethyst.asuka@gcmodeller.org")>
    <ActiveViews(FastaSeq.SampleView, type:="bash")>
    Public Class FastaSeq : Inherits ISequenceModel
        Implements IPolymerSequenceModel
        Implements IAbstractFastaToken
        Implements IFastaProvider
        Implements ICloneable
        Implements ICloneable(Of FastaSeq)

        Friend Const SampleView = ">LexA
AAGCGAACAAATGTTCTATA"

#Region "Object Properties"

        ''' <summary>
        ''' Does this sequence contains some gaps?(这条序列之中是否包含有空格？)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HaveGaps As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return InStr(SequenceData, "-") > 0 OrElse InStr(SequenceData, ".") > 0
            End Get
        End Property

        ''' <summary>
        ''' 方便通过<see cref="FASTA.FastaSeq.AddAttribute">Add接口</see>向<see cref="FASTA.FastaSeq.Headers">Attribute列表</see>中添加数据
        ''' </summary>
        ''' <remarks></remarks>
        Dim innerList As List(Of String)

        ''' <summary>
        ''' The attribute header of this FASTA file. The fasta header usually have some format which can be parsed by some 
        ''' specific loader and gets some well organized information about the sequence. The format of the header is 
        ''' usually different between each biological database.(这个FASTA文件的属性头，标题的格式通常在不同的数据库之间是具有很大差异的)
        ''' </summary>
        ''' <remarks></remarks>
        Public Overridable Property Headers As String() Implements IAbstractFastaToken.headers
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return innerList.ToArray
            End Get
            Set(value As String())
                innerList = value.AsList
            End Set
        End Property

        ''' <summary>
        ''' The sequence data that contains in this FASTA file.
        ''' (包含在这个FASTA文件之中的序列数据)
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Property SequenceData As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return MyBase.SequenceData
            End Get
            Set(value As String)
                MyBase.SequenceData = value
            End Set
        End Property

        ''' <summary>
        ''' Get the sequence length of this Fasta object.(获取序列的长度)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Length As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Len(Me.SequenceData)
            End Get
        End Property

        ''' <summary>
        ''' The first character ">" is not included in the title string data.(标题之中是不包含有FASTA数据的第一个>字符的)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Title As String Implements IAbstractFastaToken.title, IFastaProvider.title
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Me.ToString
            End Get
        End Property

        Public ReadOnly Property locus_tag As String
            Get
                Return Headers(Scan0).Trim.Split.First
            End Get
        End Property
#End Region

#Region "Header Attributes Operations"
        Public Sub AddAttribute(attrValue As String)
            Call innerList.Add(attrValue)
        End Sub

        Public Sub InsertAttribute(attrValue As String, Index As Integer)
            If innerList.Count - 1 > Index Then
                Call innerList.Insert(Index, attrValue)
            Else
                Call innerList.Add(attrValue)
            End If
        End Sub

        Public Sub RemoveAttribute(attrIndex As Integer)
            If innerList.Count - 1 > attrIndex Then
                Call innerList.RemoveAt(attrIndex)
            End If
        End Sub

        Public Sub SetAttribute(attrIndex As Integer, value As String)
            If innerList.Count - 1 > attrIndex Then
                innerList(attrIndex) = value
            Else
                Call innerList.Add(value)
            End If
        End Sub
#End Region

        ''' <summary>
        ''' NCBI style header delimiter
        ''' </summary>
        Public Const DefaultHeaderDelimiter$ = "|"

        Sub New()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">File path of a fasta sequence.</param>
        ''' <remarks></remarks>
        Sub New(path As String)
            Dim fa As FastaSeq = FastaSeq.Load(path)

            Headers = fa.Headers
            SequenceData = fa.SequenceData
        End Sub

        Sub New(seq As IAbstractFastaToken)
            Me.SequenceData = seq.SequenceData
            Me.Headers = seq.headers
        End Sub

        Sub New(attrs As IEnumerable(Of String), seq As String)
            Me.SequenceData = seq
            Me.Headers = attrs.ToArray
        End Sub

        Sub New(seq As IFastaProvider, Optional attributeParser As Func(Of String, String()) = Nothing)
            Me.SequenceData = seq.GetSequenceData
            Me.Headers = (attributeParser Or defaultTitleAttributes)(seq.title)
        End Sub

        Sub New(attrs$(), seq As IPolymerSequenceModel)
            Call Me.New(attrs, seq.SequenceData)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(sequence$, Optional title$ = "unnamed sequence")
            Call Me.New(title.Split("|"c), sequence)
        End Sub

        ''' <summary>
        ''' Get the title of this fasta object.(返回FASTA对象的标题，在所返回的值之中不包含有fasta标题之中的第一个字符>)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return String.Join(DefaultHeaderDelimiter, Me.Headers)
        End Function

        Public Shared ReadOnly defaultTitleAttributes As New [Default](Of Func(Of String, String()))(Function(title) title.Split("|"c))

        ''' <summary>
        ''' You can using this function to convert the title from current format into another format.(使用这个方法将Fasta序列对象的标题从当前的格式转换为另外一种格式)
        ''' </summary>
        ''' <param name="grep"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GrepTitle(grep As TextGrepMethod) As FastaSeq
            Dim strValue As String = grep(Me.ToString)
            Dim attributes As String() = New String() {strValue}

            Return New FastaSeq With {
                .Headers = attributes,
                .SequenceData = SequenceData
            }
        End Function

        ''' <summary>
        ''' Convert the <see cref="SequenceData"/> to upper case and then return the new created <see cref="FastaSeq"/>.
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToUpper() As FastaSeq
            Return New FastaSeq(Headers, SequenceData.ToUpper)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToLower() As FastaSeq
            Return New FastaSeq(Headers, SequenceData.ToLower)
        End Function

        ''' <summary>
        ''' Load the fasta sequence file as a nucleotide sequence from a specific <paramref name="path"></paramref>, the function will returns 
        ''' a null value if the sequence contains some non-nucleotide character.
        ''' (从一条序列的FASTA文件之中加载一条核酸序列，当含有非法字符的时候，会返回空文件)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="strict">当拥有空格数据的时候，假若参数为真，则会返回空文件，反之不会做任何处理</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Load.NT")>
        Public Shared Function LoadNucleotideData(path As String, Optional strict As Boolean = False) As FastaSeq
            Dim nt As FastaSeq = FastaSeq.Load(path)

            If nt Is Nothing Then
                Return Nothing
            Else
                nt.SequenceData = nt.SequenceData.ToUpper.Replace("N", "-") '处理空格数据
            End If

            If nt.IsProtSource Then   '判断是否为蛋白质序列
                Call $" ""{path.ToFileURL}"" is a protein sequence!".__DEBUG_ECHO

                If strict Then
                    Return Nothing
                Else
                    Call "Replace these invalid character as NT gaps.".Warning

                    Dim sb As New StringBuilder(nt.SequenceData)

                    For Each protCh As Char In TypeExtensions.AA_CHARS_ALL
                        Call sb.Replace(protCh, "-")
                    Next

                    nt.SequenceData = sb.ToString
                End If
            End If

            If nt.HaveGaps Then
                Call $" ""{path.ToFileURL}"" has gaps in the sequence data!".__DEBUG_ECHO

                If strict Then
                    Return Nothing
                End If
            End If

            Return nt
        End Function

        ''' <summary>
        ''' Load a single sequence fasta file object, if the target file path is not exists on the file system or 
        ''' the file format is not correct, then this function will returns a null object value. 
        ''' (这是一个安全的函数：从文件之中加载一条Fasta序列，当目标文件<paramref name="file"></paramref>不存在或者没有序列数据的时候，会返回空值)
        ''' </summary>
        ''' <param name="file">目标序列文件的文件路径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Load")>
        Public Shared Function Load(file$, Optional deli As Char() = Nothing) As FastaSeq
            Dim lines As String() = IO.File.ReadAllLines(file.FixPath)

            If lines.IsNullOrEmpty Then
                Call $" {file.ToFileURL} is null or empty!".__DEBUG_ECHO
                Return Nothing
            Else
                Return ParseFromStream(lines, If(deli.IsNullOrEmpty, {"|"c}, deli))
            End If
        End Function

        ''' <summary>
        ''' Parsing a fasta sequence object from a collection of string value.(从字符串数据之中解析出Fasta序列数据)
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("FastaToken.From.Stream")>
        Public Shared Function ParseFromStream(stream As IEnumerable(Of String), deli As Char()) As FastaSeq
            Dim lines$() = stream.SafeQuery.ToArray

            If lines.Length = 0 Then
                Return Nothing
            End If

            Dim attrs$() = Mid(lines(Scan0), 2).Trim.Split(deli)
            Dim removeInvalids = Function(s$) s.Replace(StreamIterator.SOH, "")

            attrs = attrs.Select(removeInvalids).ToArray

            Dim fa As New FastaSeq With {
                .Headers = attrs,
                .SequenceData = String.Join("", lines.Skip(1).ToArray).ToUpper
            }

            Return fa
        End Function

        ''' <summary>
        ''' Try parsing a fasta sequence object from a string chunk value.(尝试从一个字符串之中解析出一个fasta序列数据)
        ''' </summary>
        ''' <param name="s">The string text value which is in the Fasta format.(FASTA格式的序列文本)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("FastaToken.Parser")>
        Public Shared Function TryParse(s As String, Optional deli As Char = DefaultHeaderDelimiter) As FastaSeq
            Return FastaSeq.ParseFromStream(s.LineTokens, {deli})
        End Function

        ''' <summary>
        ''' Generate a FASTA file data text string.(将这个FASTA对象转换为文件格式以方便进行存储)
        ''' </summary>
        ''' <param name="overrides">是否使用<see cref="ToString"></see>方法进行标题的复写，假若为假，则默认使用Attributes属性进行标题的生成，
        ''' 因为在继承类之中可能会复写ToString函数以生成不同的标题格式，则可以使用这个参数来决定是否使用复写的格式。</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <param name="lineBreak">大于0的数值会换行，小于或者等于0的数值不会换行</param>
        Public Function GenerateDocument(lineBreak As Integer, Optional [overrides] As Boolean = True, Optional removeCR As Boolean = True) As String
            Dim sb As New StringBuilder(">", 10 * 1024)

            If [overrides] Then
                Call sb.Append(Me.ToString)
            Else
                Call sb.Append(String.Join(DefaultHeaderDelimiter, Headers))
            End If

            Call sb.AppendLine()
            Call SequenceLineBreak(sb, lineBreak, SequenceData)

            If removeCR Then
                Call sb.Replace(vbCr, "")
            End If

            Return sb.ToString
        End Function

        Public Shared Sub SequenceLineBreak(ByRef sb As StringBuilder, lineBreak%, sequence$)
            If lineBreak <= 0 Then
                Call sb.AppendLine(sequence)
            Else
                For Each sg As String In sequence.Chunks(lineBreak)
                    Call sb.AppendLine(sg)
                Next
            End If
        End Sub

        Public Shared Function SequenceLineBreak(lineBreak%, sequence$) As String
            With New StringBuilder
                FastaSeq.SequenceLineBreak(.ByRef, lineBreak, sequence)
                Return .ToString
            End With
        End Function

        ''' <summary>
        ''' The Fasta sequence equals on both sequence data and title information.(值比较来判断是否相等)
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is FastaSeq Then
                Dim fa As FastaSeq = DirectCast(obj, FastaSeq)
                Return String.Equals(fa.Title, Me.Title) AndAlso
                    String.Equals(fa.SequenceData, Me.SequenceData)
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Copy the value in current fasta object into another fasta object.(将当前的序列数据复制到目标序列数据对象之中)
        ''' </summary>
        ''' <typeparam name="TFasta">Fasta sequence object type.(目标序列数据类型)</typeparam>
        ''' <param name="FastaObject">The target fasta object will be copied to, if the value is null of this fasta 
        ''' object, then this function will generate a new fasta sequence object.(假若值为空，则会创建一个新的序列对象)</param>
        ''' <remarks></remarks>
        Public Overloads Sub CopyTo(Of TFasta As FastaSeq)(ByRef FastaObject As TFasta)
            If FastaObject Is Nothing Then
                FastaObject = Activator.CreateInstance(Of TFasta)()
            End If

            FastaObject.Headers = Me.Headers
            FastaObject.SequenceData = Me.SequenceData
        End Sub

        ''' <summary>
        ''' Copy the value to a new specific type fasta object.
        ''' </summary>
        ''' <typeparam name="T">Type information of the target fasta object.</typeparam>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Copy(Of T As FastaSeq)() As T
            Dim Model As T = Activator.CreateInstance(Of T)()
            Call CopyTo(Model)
            Return Model
        End Function

        ''' <summary>
        ''' Reverse the sequence data of the current fasta sequence object and then returns the new created fasta object.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Reverse() As FastaSeq
            Dim attrs As List(Of String) = Headers.AsList.Join("Reversed_sequence")
            Dim revSeq As String = New String(SequenceData.Reverse.ToArray)
            Dim fa As New FastaSeq With {
                .Headers = attrs.ToArray,
                .SequenceData = revSeq
            }

            Return fa
        End Function

        ''' <summary>
        ''' Convert the specific feature data in Genbank database into a fasta sequence.
        ''' </summary>
        ''' <param name="feature">只是从这个特性对象之中得到蛋白质序列</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Shadows Widening Operator CType(feature As Feature) As FastaSeq
            Dim GI$ = LinqAPI.DefaultFirst(Of String) <=
                From s As String
                In feature.QueryDuplicated("db_xref")
                Where InStr(s, "GI:")
                Select s.Split(CChar(":")).Last '
            Dim gb As String = String.Format("gb|{0}", feature.Query("protein_id"))
            Dim fa As New FastaSeq With {
                .Headers = {$"gi|{GI}", gb, feature.Query("locus_tag"), feature.Query("product")},
                .SequenceData = feature.Query("translation")
            }

            Return fa
        End Operator

        ''' <summary>
        ''' Gets the complement sequence data of a specific fasta sequence.(获取某一条核酸序列的互补序列)
        ''' </summary>
        ''' <param name="FASTA">The target fasta sequence object should be a nucleotide sequence, or this function will returns a null value.
        ''' (目标FASTA对象必须为核酸序列，否则返回空值)</param>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Complement")>
        Public Shared Function Complement(FASTA As FastaSeq) As FastaSeq
            If FASTA.IsProtSource Then
                Throw New DataException(InvalidComplementSource)
            End If

            Dim cmplSeq As String = NucleicAcid.Complement(FASTA.SequenceData)
            Dim FastaObject As New FastaSeq With {
                .Headers = FASTA.Headers,
                .SequenceData = cmplSeq
            }
            Return FastaObject
        End Function

        ''' <summary>
        ''' Data type miss match: the sequence information is a protein sequence, can not get complement sequence.
        ''' </summary>
        Const InvalidComplementSource As String = "Data type miss match: the sequence information is a protein sequence, can not get complement sequence."

        ''' <summary>
        ''' Generate the document text value for this fasta object.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Shadows Narrowing Operator CType(obj As FastaSeq) As String
            Return obj.GenerateDocument(lineBreak:=60)
        End Operator

        <ExportAPI("ToDoc")>
        Public Shared Function GenerateDocumentText(FastaObject As IAbstractFastaToken) As String
            Return String.Format(">{0}{1}{2}", FastaObject.title, vbCrLf, FastaObject.SequenceData).Replace(vbCr, "")
        End Function

        ''' <summary>
        ''' Save the current fasta sequence object into the file system.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveTo(Path As String, encoding As Encoding) As Boolean
            Return SaveTo(lineBreak:=60, Path:=Path, encoding:=encoding)
        End Function

        ''' <summary>
        ''' Save the current fasta sequence object into the file system. <param name="lineBreak"></param> smaller than 1 will means no line break in the saved fasta sequence.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveTo(lineBreak As Integer, Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean
            Dim doc$ = GenerateDocument(lineBreak)

            If encoding Is Nothing Then
                encoding = Encoding.ASCII
            End If

            Return doc.SaveTo(Path, encoding, False)
        End Function

        ''' <summary>
        ''' 过长的序列不每隔60个字符进行一次换行，而是直接使用一行数据进行存储
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveAsOneLine(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean
            Dim sb As New StringBuilder(">")

            Call sb.AppendLine(ToString)
            Call sb.AppendLine(SequenceData)

            If encoding Is Nothing Then
                encoding = Encodings.ASCII.CodePage
            End If

            Return sb.ToString.SaveTo(Path, encoding)
        End Function

        Public Function Save(Path$, Optional encoding As Encodings = Encodings.ASCII) As Boolean
            Return SaveTo(Path, encoding.CodePage)
        End Function

        Private Function objClone() As Object Implements ICloneable.Clone
            Return Clone()
        End Function

        ''' <summary>
        ''' Copy data to a new FASTA object.(将本对象的数据拷贝至一个新的FASTA序列对象中)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Clone() As FastaSeq Implements ICloneable(Of FastaSeq).Clone
            ' 在這裏完完全全的按值複製
            Return New FastaSeq With {
                .Headers = Me.Headers.ToArray,
                .SequenceData = New String(SequenceData)
            }
        End Function

        Private Function GetSequenceData() As String Implements ISequenceProvider.GetSequenceData
            Return SequenceData
        End Function
    End Class
End Namespace
