#Region "Microsoft.VisualBasic::5e86124d936c612f50c4729157f5c5c0, ..\GCModeller\core\Bio.Assembly\SequenceModel\FASTA\IO\FastaToken.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Namespace SequenceModel.FASTA

    ''' <summary>
    ''' The FASTA format file of a bimolecular sequence.(Notice that this file is 
    ''' only contains on sequence.)
    ''' FASTA格式的生物分子序列文件。(但是请注意：文件中只包含一条序列的情况，假若需要自定义所生成的FASTA文件的标题的格式，请复写<see cref="FastaToken.ToString"></see>方法)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <PackageNamespace("GCModeller.IO.FastaToken", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Class FastaToken : Inherits ISequenceModel
        Implements I_PolymerSequenceModel
        Implements IAbstractFastaToken
        Implements ISaveHandle
        Implements I_FastaProvider

#Region "Object Properties"

        ''' <summary>
        ''' Does this sequence contains some gaps?(这条序列之中是否包含有空格？)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HaveGaps As Boolean
            Get
                Return InStr(SequenceData, "-") > 0 OrElse InStr(SequenceData, ".") > 0
            End Get
        End Property

        ''' <summary>
        ''' 方便通过<see cref="FASTA.FastaToken.AddAttribute">Add接口</see>向<see cref="FASTA.FastaToken.Attributes">Attribute列表</see>中添加数据
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InnerList As List(Of String)

        ''' <summary>
        ''' The attribute header of this FASTA file. The fasta header usually have some format which can be parsed by some 
        ''' specific loader and gets some well organized information about the sequence. The format of the header is 
        ''' usually different between each biological database.(这个FASTA文件的属性头，标题的格式通常在不同的数据库之间是具有很大差异的)
        ''' </summary>
        ''' <remarks></remarks>
        Public Overridable Property Attributes As String() Implements IAbstractFastaToken.Attributes, I_FastaProvider.Attributes
            Get
                Return _InnerList.ToArray
            End Get
            Set(value As String())
                _InnerList = value.ToList
            End Set
        End Property

        ''' <summary>
        ''' The sequence data that contains in this FASTA file.
        ''' (包含在这个FASTA文件之中的序列数据)
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Property SequenceData As String
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
        Public ReadOnly Property Title As String Implements IAbstractFastaToken.Title, I_FastaProvider.Title
            Get
                Return Me.ToString
            End Get
        End Property
#End Region

#Region "Header Attributes Operations"
        Public Sub AddAttribute(attrValue As String)
            Call _InnerList.Add(attrValue)
        End Sub

        Public Sub InsertAttribute(attrValue As String, Index As Integer)
            If _InnerList.Count - 1 > Index Then
                Call _InnerList.Insert(Index, attrValue)
            Else
                Call _InnerList.Add(attrValue)
            End If
        End Sub

        Public Sub RemoveAttribute(attrIndex As Integer)
            If _InnerList.Count - 1 > attrIndex Then
                Call _InnerList.RemoveAt(attrIndex)
            End If
        End Sub

        Public Sub SetAttribute(attrIndex As Integer, value As String)
            If _InnerList.Count - 1 > attrIndex Then
                _InnerList(attrIndex) = value
            Else
                Call _InnerList.Add(value)
            End If
        End Sub
#End Region

        Sub New()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">File path of a fasta sequence.</param>
        ''' <remarks></remarks>
        Sub New(path As String)
            Dim fa As FastaToken = FastaToken.Load(path)

            Attributes = fa.Attributes
            SequenceData = fa.SequenceData
        End Sub

        Sub New(LDM As FASTA.IAbstractFastaToken)
            Me.SequenceData = LDM.SequenceData
            Me.Attributes = LDM.Attributes
        End Sub

        Sub New(attrs As String(), seq As String)
            Me.SequenceData = seq
            Me.Attributes = attrs
        End Sub

        Sub New(LDM As I_FastaProvider)
            Me.SequenceData = LDM.SequenceData
            Me.Attributes = LDM.Attributes
        End Sub

        ''' <summary>
        ''' Get the title of this fasta object.(返回FASTA对象的标题，在所返回的值之中不包含有fasta标题之中的第一个字符>)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return String.Join("|", Me.Attributes)
        End Function

        ''' <summary>
        ''' You can using this function to convert the title from current format into another format.(使用这个方法将Fasta序列对象的标题从当前的格式转换为另外一种格式)
        ''' </summary>
        ''' <param name="MethodPointer"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GrepTitle(MethodPointer As TextGrepMethod) As FastaToken
            Dim strValue As String = MethodPointer(Me.ToString)
            Dim attributes As String() = New String() {strValue}

            Return New FastaToken With {
                .Attributes = attributes,
                .SequenceData = SequenceData
            }
        End Function

        Public Function ToUpper() As FastaToken
            Return New FastaToken(Attributes, SequenceData.ToUpper)
        End Function

        Public Function ToLower() As FastaToken
            Return New FastaToken(Attributes, SequenceData.ToLower)
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
        Public Shared Function LoadNucleotideData(path As String, Optional strict As Boolean = False) As FastaToken
            Dim nt As FastaToken = FastaToken.Load(path)

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

                    For Each protCh As Char In ISequenceModel.AA_CHARS_ALL
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
        ''' (这是一个安全的函数：从文件之中加载一条Fasta序列，当目标文件<paramref name="File"></paramref>不存在或者没有序列数据的时候，会返回空值)
        ''' </summary>
        ''' <param name="File">目标序列文件的文件路径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Load")>
        Public Shared Function Load(File As String, Optional deli As Char() = Nothing) As FastaToken
            Dim lines As String() = IO.File.ReadAllLines(File)

            If lines.IsNullOrEmpty Then
                Call $" {File.ToFileURL} is null or empty!".__DEBUG_ECHO
                Return Nothing
            End If

            Return ParseFromStream(lines, If(deli.IsNullOrEmpty, {"|"c}, deli))
        End Function

        ''' <summary>
        ''' Parsing a fasta sequence object from a collection of string value.(从字符串数据之中解析出Fasta序列数据)
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("FastaToken.From.Stream")>
        Public Shared Function ParseFromStream(stream As IEnumerable(Of String), deli As Char()) As FastaToken
            If stream.IsNullOrEmpty Then Return Nothing

            Dim lines As String() = stream.ToArray
            Dim fa As New FastaToken

            fa.Attributes = Mid(lines(Scan0), 2).Split(deli)
            fa.Attributes = fa.Attributes.ToArray(Function(s) s.Replace(StreamIterator.SOH, ""))
            fa.SequenceData = String.Join("", lines.Skip(1).ToArray)  ' Linux mono does not support <Extension> attribute!

            Return fa
        End Function

        ''' <summary>
        ''' Try parsing a fasta sequence object from a string chunk value.(尝试从一个字符串之中解析出一个fasta序列数据)
        ''' </summary>
        ''' <param name="strData">The string text value which is in the Fasta format.(FASTA格式的序列文本)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("FastaToken.Parser")>
        Public Shared Function TryParse(strData As String, Optional deli As Char = "|"c) As FastaToken
            Dim LQuery = (From strLine As String
                          In strData.Split(CChar(vbLf))
                          Select strLine.Replace(vbCr, "")).ToArray
            Return FastaToken.ParseFromStream(LQuery, {deli})
        End Function

        ''' <summary>
        ''' Generate a FASTA file data text string.(将这个FASTA对象转换为文件格式以方便进行存储)
        ''' </summary>
        ''' <param name="overrides">是否使用<see cref="ToString"></see>方法进行标题的复写，假若为假，则默认使用Attributes属性进行标题的生成，
        ''' 因为在继承类之中可能会复写ToString函数以生成不同的标题格式，则可以使用这个参数来决定是否使用复写的格式。</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <param name="LineBreak">大于0的数值会换行，小于或者等于0的数值不会换行</param>
        Public Function GenerateDocument(LineBreak As Integer, Optional [overrides] As Boolean = True) As String
            Dim sb As New StringBuilder(">", 10 * 1024)

            If [overrides] Then
                Call sb.Append(Me.ToString)
            Else
                Call sb.Append(String.Join("|", Attributes))
            End If

            Call sb.AppendLine()

            If LineBreak <= 0 Then
                Call sb.AppendLine(SequenceData)
            Else
                For i As Integer = 1 To Len(SequenceData) Step LineBreak
                    Dim Segment As String = Mid(SequenceData, i, LineBreak)
                    Call sb.AppendLine(Segment)
                Next
            End If

            Call sb.Replace(vbCr, "")

            Return sb.ToString
        End Function

        ''' <summary>
        ''' The Fasta sequence equals on both sequence data and title information.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is FastaToken Then
                Dim [Object] = DirectCast(obj, FastaToken)
                Return String.Equals([Object].Title, Me.Title) AndAlso
                    String.Equals([Object].SequenceData, Me.SequenceData)
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Copy data to a new FASTA object.(将本对象的数据拷贝至一个新的FASTA序列对象中)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Copy() As FastaToken
            Dim FastaObject As FastaToken = New FastaToken With {
                .Attributes = New String(Me.Attributes.Count - 1) {}
            }
            Call Me.Attributes.CopyTo(FastaObject.Attributes, index:=Scan0)
            FastaObject.SequenceData = Me.SequenceData
            Return FastaObject
        End Function

        ''' <summary>
        ''' Copy the value in current fasta object into another fasta object.(将当前的序列数据复制到目标序列数据对象之中)
        ''' </summary>
        ''' <typeparam name="TFasta">Fasta sequence object type.(目标序列数据类型)</typeparam>
        ''' <param name="FastaObject">The target fasta object will be copied to, if the value is null of this fasta 
        ''' object, then this function will generate a new fasta sequence object.(假若值为空，则会创建一个新的序列对象)</param>
        ''' <remarks></remarks>
        Public Overloads Sub CopyTo(Of TFasta As FastaToken)(ByRef FastaObject As TFasta)
            If FastaObject Is Nothing Then
                FastaObject = Activator.CreateInstance(Of TFasta)()
            End If

            FastaObject.Attributes = Me.Attributes
            FastaObject.SequenceData = Me.SequenceData
        End Sub

        ''' <summary>
        ''' Copy the value to a new specific type fasta object.
        ''' </summary>
        ''' <typeparam name="T">Type information of the target fasta object.</typeparam>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Copy(Of T As FastaToken)() As T
            Dim Model As T = Activator.CreateInstance(Of T)()
            Call CopyTo(Model)
            Return Model
        End Function

        ''' <summary>
        ''' Reverse the sequence data of the current fasta sequence object and then returns the new created fasta object.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Reverse() As FastaToken
            Dim attrs As List(Of String) = Me.Attributes.ToList.Join("Reversed_sequence")
            Dim revSeq As String = New String(SequenceData.Reverse.ToArray)
            Dim fa As FastaToken = New FastaToken With {
                .Attributes = attrs.ToArray,
                .SequenceData = revSeq
            }

            Return fa
        End Function

        ''' <summary>
        ''' Convert the specific feature data in Genbank database into a fasta sequence.
        ''' </summary>
        ''' <param name="Feature">只是从这个特性对象之中得到蛋白质序列</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Shadows Widening Operator CType(Feature As Feature) As FastaToken
            Dim GI = From s As String In Feature.QueryDuplicated("db_xref") Where InStr(s, "GI:") Select s.Split(CChar(":")).Last '
            Dim gb As String = String.Format("gb|{0}", Feature.Query("protein_id"))
            Dim fa As New FastaToken With {
                .Attributes = {$"gi|{GI.FirstOrDefault}", gb, Feature.Query("locus_tag"), Feature.Query("product")},
                .SequenceData = Feature.Query("translation")
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
        Public Shared Function Complement(FASTA As FastaToken) As FastaToken
            If FASTA.IsProtSource Then Throw New DataException(InvalidComplementSource)

            Dim cmplSeq As String = NucleotideModels.NucleicAcid.Complement(FASTA.SequenceData)
            Dim FastaObject = New SequenceModel.FASTA.FastaToken With {
                .Attributes = FASTA.Attributes,
                .SequenceData = cmplSeq
            }
            Return FastaObject
        End Function

        Const InvalidComplementSource As String = "Data type miss match: the sequence information is a protein sequence, can not get complement sequence."

        ''' <summary>
        ''' Generate the document text value for this fasta object.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Shadows Narrowing Operator CType(obj As FastaToken) As String
            Return obj.GenerateDocument(LineBreak:=60)
        End Operator

        <ExportAPI("ToDoc")>
        Public Shared Function GenerateDocumentText(FastaObject As IAbstractFastaToken) As String
            Return String.Format(">{0}{1}{2}", FastaObject.Title, vbCrLf, FastaObject.SequenceData).Replace(vbCr, "")
        End Function

        ''' <summary>
        ''' Save the current fasta sequence object into the file system.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveTo(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            Return SaveTo(LineBreak:=60, Path:=Path, encoding:=encoding)
        End Function

        ''' <summary>
        ''' Save the current fasta sequence object into the file system. <param name="LineBreak"></param> smaller than 1 will means no line break in the saved fasta sequence.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveTo(LineBreak As Integer, Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean
            Dim ParentDir As String = FileIO.FileSystem.GetParentPath(Path)

            If encoding Is Nothing Then
                encoding = System.Text.Encoding.Default
            End If

            Call FileIO.FileSystem.CreateDirectory(ParentDir)
            Call FileIO.FileSystem.WriteAllText(Path, Me.GenerateDocument(LineBreak), append:=False, encoding:=encoding)
            Return True
        End Function

        ''' <summary>
        ''' 过长的序列不每隔60个字符进行一次换行，而是直接使用一行数据进行存储
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveAsOneLine(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean
            Dim FastaBuilder As StringBuilder = New StringBuilder(">")
            Call FastaBuilder.AppendLine(ToString)
            Call FastaBuilder.AppendLine(SequenceData)

            Return FastaBuilder.ToString.SaveTo(Path, encoding)
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return SaveTo(Path, encoding.GetEncodings)
        End Function
    End Class
End Namespace
