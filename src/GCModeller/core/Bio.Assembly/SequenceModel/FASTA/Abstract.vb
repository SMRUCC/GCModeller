Imports System.Text
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SequenceModel.FASTA

    Public Interface I_FastaProvider : Inherits I_PolymerSequenceModel
        ReadOnly Property Title As String
        ReadOnly Property Attributes As String()
    End Interface

    ''' <summary>
    ''' The fasta object is a sequence model object with a specific title to identify the sequence and a sequence data property to represents the specific molecule.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IAbstractFastaToken : Inherits I_PolymerSequenceModel
        ''' <summary>
        ''' The title value which contains some brief information about this sequence.(这条序列数据的标题摘要信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Title As String
        ''' <summary>
        ''' The attribute header of this FASTA file. The fasta header usually have some format which can be parsed by some 
        ''' specific loader and gets some well organized information about the sequence. The format of the header is 
        ''' usually different between each biological database.(这个FASTA文件的属性头，标题的格式通常在不同的数据库之间是具有很大差异的)
        ''' </summary>
        ''' <returns></returns>
        Property Attributes As String()
    End Interface

    Public Interface I_FastaObject
        Function GetSequenceData() As String
    End Interface
End Namespace