#Region "Microsoft.VisualBasic::831e339c745fac0f106c6a97cc00a76b, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\COG\Function.vb"

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

    '   Total Lines: 352
    '    Code Lines: 202
    ' Comment Lines: 110
    '   Blank Lines: 40
    '     File Size: 14.36 KB


    '     Class [Function]
    ' 
    '         Properties: Catalogs, NotAssigned
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: [Default], __getCategory, __trimCOGs, CanbeCategoryAs, Category
    '                   ClassCategory, GetCatalog, (+2 Overloads) GetCategories, GetCategory, IndexOf
    '                   Statistics
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Assembly.NCBI.COG

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks>
    ''' ###### INFORMATION STORAGE AND PROCESSING
    ''' + [J] Translation, ribosomal structure and biogenesis
    ''' + [A] RNA processing and modification
    ''' + [K] Transcription
    ''' + [L] Replication, recombination and repair
    ''' + [B] Chromatin structure and dynamics
    '''
    ''' ###### CELLULAR PROCESSES AND SIGNALING
    ''' + [D] Cell cycle control, cell division, chromosome partitioning
    ''' + [Y] Nuclear structure
    ''' + [V] Defense mechanisms
    ''' + [T] Signal transduction mechanisms
    ''' + [M] Cell wall/membrane/envelope biogenesis
    ''' + [N] Cell motility
    ''' + [Z] Cytoskeleton
    ''' + [W] Extracellular structures
    ''' + [U] Intracellular trafficking, secretion, and vesicular transport
    ''' + [O] Posttranslational modification, protein turnover, chaperones
    '''
    ''' ###### METABOLISM
    ''' + [C] Energy production and conversion
    ''' + [G] Carbohydrate transport and metabolism
    ''' + [E] Amino acid transport and metabolism
    ''' + [F] Nucleotide transport and metabolism
    ''' + [H] Coenzyme transport and metabolism
    ''' + [I] Lipid transport and metabolism
    ''' + [P] Inorganic ion transport and metabolism
    ''' + [Q] Secondary metabolites biosynthesis, transport and catabolism
    '''
    ''' ###### POORLY CHARACTERIZED
    ''' + [R] General function prediction only
    ''' + [S] Function unknown
    ''' </remarks>
    Public Class [Function]

        <XmlElement> Public Property Catalogs As Catalog()

        Public Function IndexOf(Id As Char) As Integer
            For i As Integer = 0 To Catalogs.Length - 1

                With Catalogs(i)

                    If .SubClasses.ContainsKey(Id) Then
                        Return i
                    End If
                End With
            Next

            Return -1
        End Function

        ''' <summary>
        ''' 当具有多个COG分类的时候，可以用这个来判断该基因是否可以被分类为指定的大分类
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="COG">必须是经过<see cref="BioAssemblyExtensions.GetCOGCategory(String)"/>修剪的字符串</param>
        Public Function CanbeCategoryAs(COG As String, category As COGCategories) As Boolean
            For Each c As Char In COG
                Dim cat As COGCategories = __getCategory(c)

                If cat > 0 Then
                    If category = CType(cat, COGCategories) Then
                        Return True
                    End If
                End If
            Next

            Return False
        End Function

        Public Function Category(COG As String) As Integer
            Dim n As Integer

            For Each cogChar In COG
                Dim m = __getCategory(cogChar)
                n += m  ' 分类编号是互斥的，可以直接相加
            Next

            Return n
        End Function

        ''' <summary>
        ''' 得到COG分类
        ''' </summary>
        ''' <param name="cogChar"></param>
        ''' <returns></returns>
        Private Function __getCategory(cogChar As Char) As COGCategories
            For Each [class] As COG.Catalog In Me._Catalogs  '' ??? 有BUG？？？
                If [class].GetDescription(cogChar, "") Then
                    Return [class].Class
                End If
            Next

            Return 0
        End Function

        Public Shared ReadOnly Property NotAssigned$ = COGCategories.NotAssigned.Description

        Public Function GetCatalog(COG As Char) As NamedValue(Of String)
            Dim func$ = Nothing

            For Each [class] As COG.Catalog In Me._Catalogs  '' ??? 有BUG？？？
                If [class].GetDescription(COG, func) Then
                    Return New NamedValue(Of String) With {
                        .Name = [class].Description,
                        .Value = func
                    }
                End If
            Next

            Return New NamedValue(Of String) With {
                .Name = [Function].NotAssigned,
                .Value = "-"
            }
        End Function

        ''' <summary>
        ''' 请注意，这个函数只会返回一个COG编号
        ''' </summary>
        ''' <param name="COG">已经自动处理好所有事情了</param>
        ''' <returns></returns>
        Public Function GetCategory(COG As String) As COGCategories
            COG = __trimCOGs(COG)

            If String.IsNullOrEmpty(COG) OrElse
                String.Equals(COG, "-") Then

                Return COGCategories.NotAssigned
            Else
                Dim COGChar As Char = COG.First
                Dim Category As Integer = __getCategory(COGChar)

                If Category = 0 Then
                    Return COGCategories.NotAssigned
                Else
                    Return CType(Category, COGCategories)
                End If
            End If
        End Function

        Public Function GetCategories(COG As String) As COGCategories()
            COG = __trimCOGs(COG)

            If String.IsNullOrEmpty(COG) OrElse
                String.Equals(COG, "-") Then

                Return {
                    COGCategories.NotAssigned
                }
            End If

            Return COG.Select(Function(x) __getCategory(x)).ToArray
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value">COGxxxZZZ</param>
        ''' <returns></returns>
        Friend Shared Function __trimCOGs(value As String) As String
            If String.IsNullOrEmpty(value) Then
                Return ""
            End If

            Dim COG As String = Regex.Replace(value, "cog\d+", "", RegexOptions.IgnoreCase)
            Return COG
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="category">相加得到的值，可以在这里使用这个函数进行分解</param>
        ''' <returns></returns>
        Public Function GetCategories(category As Integer) As COGCategories()
            If category = COGCategories.NotAssigned OrElse
                category = 0 Then
                Return New COGCategories() {}
            End If

            Dim list As New List(Of COGCategories)

            For Each cat As Catalog In Me.Catalogs
                If category Or cat.Class Then
                    Call list.Add(cat.Class)
                End If
            Next

            Return list.ToArray
        End Function

        ''' <summary>
        ''' COG profiling counting.
        ''' </summary>
        ''' <param name="lstId">List COG id</param>
        ''' <returns></returns>
        Public Function Statistics(lstId As String()) As Double()
            Dim Data As Double() = New Double(Me.Catalogs.Length - 1) {}
            For Each id As String In lstId
                If Not String.IsNullOrEmpty(id.Trim) Then
                    For Each ch In id
                        Dim idx = Me.IndexOf(ch)
                        If idx = -1 Then
                            Continue For
                        End If
                        Data(idx) += 1
                    Next
                End If
            Next

            Return Data
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <returns></returns>
        Public Overloads Function ClassCategory(Of T As IFeatureDigest)(source As IEnumerable(Of T)) As Dictionary(Of COGCategories, String())
            Dim table As New Dictionary(Of COGCategories, String()) From {  '主要是为了填满所有分类，因为source之中可能并不包含有所有的cog分类
                {COGCategories.Genetics, New String() {}},
                {COGCategories.Metabolism, New String() {}},
                {COGCategories.NotAssigned, New String() {}},
                {COGCategories.Signaling, New String() {}},
                {COGCategories.Unclassified, New String() {}}
            }
            Dim LQuery = (From x In (From gene As T In source.AsParallel
                                     Let categories As COGCategories() = GetCategories(gene.Feature)
                                     Select (From cat As COGCategories In categories
                                             Select geneId = gene.Key,
                                                 category = GetCategory(gene.Feature)).ToArray).Unlist
                          Select geneid = x.geneId,
                              category = x.category
                          Group By category Into Group).ToArray
            For Each [class] In LQuery
                table([class].category) = [class] _
                    .Group _
                    .Select(Function(obj) obj.geneid) _
                    .ToArray
            Next

            Return table
        End Function

        Protected Sub New()
        End Sub

        ''' <summary>
        ''' Default COGs function catalog data
        ''' </summary>
        ''' <remarks>
        ''' ###### INFORMATION STORAGE AND PROCESSING
        ''' + [J] Translation, ribosomal structure and biogenesis
        ''' + [A] RNA processing and modification
        ''' + [K] Transcription
        ''' + [L] Replication, recombination and repair
        ''' + [B] Chromatin structure and dynamics
        '''
        ''' ###### CELLULAR PROCESSES AND SIGNALING
        ''' + [D] Cell cycle control, cell division, chromosome partitioning
        ''' + [Y] Nuclear structure
        ''' + [V] Defense mechanisms
        ''' + [T] Signal transduction mechanisms
        ''' + [M] Cell wall/membrane/envelope biogenesis
        ''' + [N] Cell motility
        ''' + [Z] Cytoskeleton
        ''' + [W] Extracellular structures
        ''' + [U] Intracellular trafficking, secretion, and vesicular transport
        ''' + [O] Posttranslational modification, protein turnover, chaperones
        '''
        ''' ###### METABOLISM
        ''' + [C] Energy production and conversion
        ''' + [G] Carbohydrate transport and metabolism
        ''' + [E] Amino acid transport and metabolism
        ''' + [F] Nucleotide transport and metabolism
        ''' + [H] Coenzyme transport and metabolism
        ''' + [I] Lipid transport and metabolism
        ''' + [P] Inorganic ion transport and metabolism
        ''' + [Q] Secondary metabolites biosynthesis, transport and catabolism
        '''
        ''' ###### POORLY CHARACTERIZED
        ''' + [R] General function prediction only
        ''' + [S] Function unknown
        ''' </remarks>
        Public Shared Function [Default]() As [Function]
            Dim catalogs As Catalog() = {
                New Catalog With {
                    .Class = COGCategories.Genetics,
                    .Description = "INFORMATION STORAGE AND PROCESSING",
                    .SubClasses = New Dictionary(Of Char, String) From {
                        {"J"c, "Translation, ribosomal structure and biogenesis"},
                        {"A"c, "RNA processing and modification"},
                        {"K"c, "Transcription"},
                        {"L"c, "Replication, recombination and repair"},
                        {"B"c, "Chromatin structure and dynamics"}
                    }
                }, New Catalog With {
                    .Class = COGCategories.Signaling,
                    .Description = "CELLULAR PROCESSES AND SIGNALING",
                    .SubClasses = New Dictionary(Of Char, String) From {
                        {"D"c, "Cell cycle control, cell division, chromosome partitioning"},
                        {"Y"c, "Nuclear structure"},
                        {"V"c, "Defense mechanisms"},
                        {"T"c, "Signal transduction mechanisms"},
                        {"M"c, "Cell wall/membrane/envelope biogenesis"},
                        {"N"c, "Cell motility"},
                        {"Z"c, "Cytoskeleton"},
                        {"W"c, "Extracellular structures"},
                        {"U"c, "Intracellular trafficking, secretion, and vesicular transport"},
                        {"O"c, "Posttranslational modification, protein turnover, chaperones"}
                    }
                }, New Catalog With {
                    .Class = COGCategories.Metabolism,
                    .Description = "METABOLISM",
                    .SubClasses = New Dictionary(Of Char, String) From {
                        {"C"c, "Energy production and conversion"},
                        {"G"c, "Carbohydrate transport and metabolism"},
                        {"E"c, "Amino acid transport and metabolism"},
                        {"F"c, "Nucleotide transport and metabolism"},
                        {"H"c, "Coenzyme transport and metabolism"},
                        {"I"c, "Lipid transport and metabolism"},
                        {"P"c, "Inorganic ion transport and metabolism"},
                        {"Q"c, "Secondary metabolites biosynthesis, transport and catabolism"}
                    }
                }, New Catalog With {
                    .Class = COGCategories.Unclassified,
                    .Description = "POORLY CHARACTERIZED",
                    .SubClasses = New Dictionary(Of Char, String) From {
                        {"R"c, "General function prediction only"},
                        {"S"c, "Function unknown"}
                    }
                }
            }

            Return New [Function] With {
                .Catalogs = catalogs
            }
        End Function
    End Class
End Namespace
