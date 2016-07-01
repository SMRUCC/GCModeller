Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

''' <summary>
''' The genome partitional data.(基因组分区数据)
''' </summary>
''' <remarks></remarks>
Public Class PartitioningData : Implements IAbstractFastaToken

    ''' <summary>
    ''' 当前的功能分区的标签信息
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Tag")> Public Property PartitioningTag As String
    ''' <summary>
    ''' 分区的起始位置
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Loci.Sp")> Public Property LociLeft As Integer
    ''' <summary>
    ''' 分区的结束位置
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Loci.St")> Public Property LociRight As Integer
    Public Property ORFList As String()
    Public Property GenomeID As String
    Public ReadOnly Property Length As Integer
        Get
            If LociRight > LociLeft Then
                Return LociRight - LociLeft
            Else
                'Join Locations
                Return Len(SequenceData)
            End If
        End Get
    End Property

    Public ReadOnly Property GC As Double
        Get
            Return NucleotideModels.GCContent(Me)
        End Get
    End Property

    Public ReadOnly Property Title As String Implements IAbstractFastaToken.Title
        Get
            Return String.Format("{0} ({1})", GenomeID, PartitioningTag)
        End Get
    End Property

    ''' <summary>
    ''' 当前的基因组分区之中的基因序列
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Ignored> Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

    Public Property Attributes As String() Implements IAbstractFastaToken.Attributes

    Public Overrides Function ToString() As String
        Return Title & ":  " & SequenceData
    End Function

    Public Function ToFasta() As FastaToken
        Return New FastaToken With {
            .SequenceData = SequenceData,
            .Attributes = New String() {Me.GenomeID, Me.PartitioningTag}
        }
    End Function
End Class
