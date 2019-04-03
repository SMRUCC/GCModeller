Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' 因为物质编号可能会来自于不同的数据库，所以会需要使用这个对象将一些关键的物质映射为计算引擎所能够被识别的对象
''' </summary>
Public Class Definition

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property ATP As String
    Public Property Water As String
    Public Property ADP As String

    Public Property NucleicAcid As NucleicAcid
    Public Property AminoAcid As AminoAcid

End Class

Public Class NucleicAcid

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property A As String
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property U As String
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property G As String
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property C As String

    Default Public ReadOnly Property Base(compound As String) As String
        Get
            Select Case compound
                Case "A" : Return A
                Case "U", "T" : Return U
                Case "G" : Return G
                Case "C" : Return C
                Case Else
                    Throw New NotImplementedException(compound)
            End Select
        End Get
    End Property

End Class

Public Class AminoAcid

    ''' <summary>
    ''' L-Alanine
    ''' </summary>
    ''' <returns></returns>
    Public Property A As String
    ''' <summary>
    ''' L-Arginine
    ''' </summary>
    ''' <returns></returns>
    Public Property R As String
    ''' <summary>
    ''' L-Asparagine
    ''' </summary>
    ''' <returns></returns>
    Public Property N As String
    ''' <summary>
    ''' L-Aspartic acid
    ''' </summary>
    ''' <returns></returns>
    Public Property D As String
    ''' <summary>
    ''' L-Cysteine
    ''' </summary>
    ''' <returns></returns>
    Public Property C As String
    ''' <summary>
    ''' L-Glutamic acid
    ''' </summary>
    ''' <returns></returns>
    Public Property E As String
    ''' <summary>
    ''' L-Glutamine
    ''' </summary>
    ''' <returns></returns>
    Public Property Q As String
    ''' <summary>
    ''' Glycine
    ''' </summary>
    ''' <returns></returns>
    Public Property G As String
    ''' <summary>
    ''' L-Histidine
    ''' </summary>
    ''' <returns></returns>
    Public Property H As String
    ''' <summary>
    ''' L-Isoleucine
    ''' </summary>
    ''' <returns></returns>
    Public Property I As String
    ''' <summary>
    ''' L-Leucine
    ''' </summary>
    ''' <returns></returns>
    Public Property L As String
    ''' <summary>
    ''' L-Lysine
    ''' </summary>
    ''' <returns></returns>
    Public Property K As String
    ''' <summary>
    ''' L-Methionine
    ''' </summary>
    ''' <returns></returns>
    Public Property M As String
    ''' <summary>
    ''' L-Phenylalanine
    ''' </summary>
    ''' <returns></returns>
    Public Property F As String
    ''' <summary>
    ''' L-Proline
    ''' </summary>
    ''' <returns></returns>
    Public Property P As String
    ''' <summary>
    ''' L-Serine
    ''' </summary>
    ''' <returns></returns>
    Public Property S As String
    ''' <summary>
    ''' L-Threonine
    ''' </summary>
    ''' <returns></returns>
    Public Property T As String
    ''' <summary>
    ''' L-Tryptophan
    ''' </summary>
    ''' <returns></returns>
    Public Property W As String
    ''' <summary>
    ''' L-Tyrosine
    ''' </summary>
    ''' <returns></returns>
    Public Property Y As String
    ''' <summary>
    ''' L-Valine
    ''' </summary>
    ''' <returns></returns>
    Public Property V As String
    ''' <summary>
    ''' L-Selenocysteine
    ''' </summary>
    ''' <returns></returns>
    Public Property U As String
    ''' <summary>
    ''' L-Pyrrolysine
    ''' </summary>
    ''' <returns></returns>
    Public Property O As String

    Shared ReadOnly aa As Dictionary(Of String, PropertyInfo)

    Shared Sub New()
        aa = DataFramework.Schema(Of AminoAcid)(PropertyAccess.Readable, True, True) _
            .Values _
            .Where(Function(p) p.Name.Length = 1) _
            .ToDictionary(Function(a) a.Name)
    End Sub

    Default Public ReadOnly Property Residue(compound As String) As String
        Get
            Return aa(compound).GetValue(Me)
        End Get
    End Property

End Class