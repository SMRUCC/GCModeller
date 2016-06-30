Imports LANS.SystemsBiology.Assembly.NCBI.COG
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace DESeq2

    ''' <summary>
    ''' 没有基因的表达数据，只有变化值
    ''' </summary>
    ''' <remarks>
    ''' 请注意在这里面的treated vs untreated就是~condition的对比，可以看作为NY vs MMX
    ''' </remarks>
    Public Class DESeq2Diff : Implements sIdEnumerable

        Public Overridable Property locus_tag As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' The base mean over all rows.
        ''' (表达量变化是identical的基因可以直接使用这个值来作为表达量)
        ''' </summary>
        ''' <returns></returns>
        Public Property baseMean As Double
        ''' <summary>
        ''' log2 fold change (MAP): condition treated vs untreated
        ''' </summary>
        ''' <returns></returns>
        Public Property log2FoldChange As Double
        ''' <summary>
        ''' standard error: condition treated vs untreated
        ''' </summary>
        ''' <returns></returns>
        Public Property lfcSE As Double
        ''' <summary>
        ''' Wald statistic: condition treated vs untreated
        ''' </summary>
        ''' <returns></returns>
        Public Property stat As Double
        ''' <summary>
        ''' Wald test p-value: condition treated vs untreated
        ''' </summary>
        ''' <returns></returns>
        Public Property pvalue As Double
        ''' <summary>
        ''' BH adjusted p-values
        ''' </summary>
        ''' <returns></returns>
        Public Property padj As Double

        Public Overrides Function ToString() As String
            Return locus_tag
        End Function
    End Class

    Public Class DESeqCOGs
        Public Property COG As String
        Public Property Category As COGCategories
        <ColumnAttribute("COG.Describe")> Public Property COGDescription As String
        <ColumnAttribute("Category.Describe")> Public Property CategoryDescription As String
        Public ReadOnly Property NumbersOfGenes As Integer
            Get
                Return (0 - NumbersOfDiffDown) + ' 为了绘图方便，这个是负数的
                        NumbersOfDiffUp +
                        NumbersOfIdenticalHigh +
                        NumbersOfIdenticalLow
            End Get
        End Property

        <CollectionAttribute("Diff.UP")> Public Property DiffUp As String()
        <CollectionAttribute("Diff.Down")> Public Property DiffDown As String()
        Public ReadOnly Property NumbersOfDiffUp As Integer
            Get
                If DiffUp Is Nothing Then
                    Return 0
                Else
                    Return DiffUp.Length
                End If
            End Get
        End Property

        ''' <summary>
        ''' 为了绘图方便，这个是负数的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NumbersOfDiffDown As Integer
            Get
                If DiffDown Is Nothing Then
                    Return 0
                Else
                    Return 0 - DiffDown.Length
                End If
            End Get
        End Property

        <CollectionAttribute("Identical.Low")> Public Property IdenticalLow As String()
        <CollectionAttribute("Identical.High")> Public Property IdenticalHigh As String()

        Public ReadOnly Property NumbersOfIdenticalLow As Integer
            Get
                If IdenticalLow Is Nothing Then
                    Return 0
                Else
                    Return IdenticalLow.Length
                End If
            End Get
        End Property
        Public ReadOnly Property NumbersOfIdenticalHigh As Integer
            Get
                If IdenticalHigh Is Nothing Then
                    Return 0
                Else
                    Return IdenticalHigh.Length
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{COGDescription} // {CategoryDescription}"
        End Function
    End Class
End Namespace