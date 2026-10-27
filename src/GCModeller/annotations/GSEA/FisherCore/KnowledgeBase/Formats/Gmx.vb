Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Formats

    ''' <summary>
    ''' #### GMX: Gene Matrix file format (``*.gmx``)
    ''' 
    ''' The GMX format is the transposed variant of the <see cref="Gmt"/> format: 
    ''' in the GMX format each **column** represents a gene set, while in the GMT 
    ''' format each row represents a gene set.
    ''' 
    ''' The GMX format uses tabs to separate the fields, and the file layout is:
    ''' 
    ''' 1. (gene set name 1) (TAB) (gene set name 2) ... (TAB) (gene set name N)
    ''' 2. (description 1) (TAB) (description 2) ... (TAB) (description N)
    ''' 3. (gene 1 of each gene set) ...
    ''' 
    ''' The member genes are read row by row from the third line, an empty cell in a 
    ''' column means that the current gene set has no more member gene at this row. 
    ''' 
    ''' Note that the description line is a **required** part of the file format: if it 
    ''' is missing then the member genes would be read as the description data, and the 
    ''' parsed result will contains no gene set at all(``numberOfSets`` is zero).
    ''' </summary>
    ''' <remarks>
    ''' http://software.broadinstitute.org/cancer/software/gsea/wiki/index.php/Data_formats#Gene_Set_Data_Formats
    ''' </remarks>
    Public Class Gmx : Implements IEnumerable(Of Cluster)

        ''' <summary>
        ''' The gene sets that defined in the gmx file, each column in the source 
        ''' file is parsed as one <see cref="Cluster"/> object.
        ''' </summary>
        ''' <returns></returns>
        Public Property clusters As Cluster()

        ''' <summary>
        ''' The number of the gene sets that defined in this gmx file.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property numberOfSets As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If clusters Is Nothing Then
                    Return 0
                Else
                    Return clusters.Length
                End If
            End Get
        End Property

        ''' <summary>
        ''' The comment lines(``#`` prefix) that read from the gmx file, usually 
        ''' these data is the metadata of the gene set database.
        ''' </summary>
        ''' <returns></returns>
        Public Property comments As String()

        ''' <summary>
        ''' parse gmx file
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function LoadFile(path As String) As Gmx
            Return Parse(path.ReadAllLines)
        End Function

        ''' <summary>
        ''' parse gmx file from a collection of the text lines
        ''' </summary>
        ''' <param name="lines"></param>
        ''' <returns></returns>
        Public Shared Function Parse(lines As IEnumerable(Of String)) As Gmx
            Dim names As String() = Nothing
            Dim header As String() = Nothing
            Dim rows As New List(Of String())
            Dim members As List(Of String)() = Nothing
            Dim description As String() = Nothing
            Dim comments As New List(Of String)

            ' 第一行是基因集的名称，第二行是基因集的描述信息，余下的行则是成员基因
            For Each line As String In lines.SafeQuery
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If

                Dim text As String = line.TrimEnd

                ' 以#开头的行是注释行，例如记录了species或者database的元信息
                If text.TrimStart.StartsWith("#") Then
                    Call comments.Add(text.Trim)
                    Continue For
                End If

                Dim cols As String() = text.Split(ControlChars.Tab)

                If names Is Nothing Then
                    names = cols _
                        .Select(Function(s) s.Trim) _
                        .ToArray
                ElseIf header Is Nothing Then
                    header = cols
                Else
                    Call rows.Add(cols)
                End If
            Next

            If names Is Nothing Then
                Return New Gmx With {
                    .clusters = New Cluster() {},
                    .comments = comments.ToArray
                }
            End If

            members = newColumns(names.Length)
            description = paddingDescription(header, names.Length)

            For Each row As String() In rows
                Call appendRow(members, row, names.Length)
            Next

            Return New Gmx With {
                .clusters = buildClusters(names, description, members),
                .comments = comments.ToArray
            }
        End Function

        Private Shared Function newColumns(numberOfSets As Integer) As List(Of String)()
            Dim columns As List(Of String)() = New List(Of String)(numberOfSets - 1) {}

            For i As Integer = 0 To numberOfSets - 1
                columns(i) = New List(Of String)
            Next

            Return columns
        End Function

        Private Shared Function buildClusters(names As String(), description As String(),
                                             members As List(Of String)()) As Cluster()

            Dim sets As New List(Of Cluster)

            For i As Integer = 0 To names.Length - 1
                Dim geneSet As String = names(i)

                If String.IsNullOrWhiteSpace(geneSet) Then
                    Continue For
                End If

                ' 同一个基因集之中重复出现的基因只计一次，否则会破坏超几何检验的列联表
                Dim genes As String() = members(i) _
                    .Distinct(StringComparer.Ordinal) _
                    .ToArray

                If genes.Length = 0 Then
                    Continue For
                End If

                Dim desc As String = Nothing

                If Not description Is Nothing AndAlso i < description.Length Then
                    desc = description(i)
                End If
                If String.IsNullOrWhiteSpace(desc) Then
                    desc = geneSet
                End If

                Call sets.Add(New Cluster With {
                    .ID = geneSet,
                    .names = geneSet,
                    .description = desc,
                    .members = genes _
                        .Select(Function(gene) New BackgroundGene(gene)) _
                        .ToArray
                })
            Next

            Return sets.ToArray
        End Function

        ''' <summary>
        ''' append the member genes in a gmx data row into the gene set column list
        ''' </summary>
        Private Shared Sub appendRow(members As List(Of String)(), row As String(), numberOfSets As Integer)
            Dim n As Integer = If(row Is Nothing, 0, row.Length)

            If n > numberOfSets Then
                n = numberOfSets
            End If

            For i As Integer = 0 To n - 1
                Dim gene As String = row(i).Trim

                If Not String.IsNullOrWhiteSpace(gene) Then
                    Call members(i).Add(gene)
                End If
            Next
        End Sub

        Private Shared Function paddingDescription(header As String(), numberOfSets As Integer) As String()
            If header Is Nothing Then
                Return Nothing
            End If

            Dim desc As String() = New String(numberOfSets - 1) {}

            For i As Integer = 0 To numberOfSets - 1
                If i < header.Length Then
                    desc(i) = header(i).Trim
                Else
                    desc(i) = ""
                End If
            Next

            Return desc
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Cluster) Implements IEnumerable(Of Cluster).GetEnumerator
            For Each cluster As Cluster In clusters.SafeQuery
                Yield cluster
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public Overrides Function ToString() As String
            Return $"GMX {{{numberOfSets} gene set(s)}}"
        End Function
    End Class
End Namespace
