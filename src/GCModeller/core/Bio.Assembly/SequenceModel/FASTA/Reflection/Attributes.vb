Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SequenceModel.FASTA.Reflection

    ''' <summary>
    ''' 自定義屬性用於指示哪一個屬性值為目標對象的序列數據
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Field, AllowMultiple:=False, Inherited:=True)>
    Public Class FastaSequenceEntry : Inherits Attribute
    End Class

    <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Field, AllowMultiple:=False, Inherited:=True)>
    Public Class FastaAttributeItem : Inherits Attribute

        Public Property Index As Integer
        Public Property Format As String
        ''' <summary>
        ''' 當Format屬性值不為空的時候，本參數失效
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Precursor As String

        Sub New(Index As Integer, Optional Format As String = "")
            Me.Index = Index
            Me.Format = Format
        End Sub

        Public Shared ReadOnly FsaAttributeItem As System.Type = GetType(FastaAttributeItem)
    End Class

    ''' <summary>
    ''' 用於類型定義上的FASTA序列對象的標題格式
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Class Or AttributeTargets.Struct, AllowMultiple:=False, Inherited:=True)>
    Public Class FastaObject : Inherits Attribute

        Public Property Format As String

        Sub New(TitleFormat As String)
            Format = TitleFormat
        End Sub

        Public Overrides Function ToString() As String
            Return Format
        End Function

        Public Shared ReadOnly FsaTitle As System.Type = GetType(FastaObject)
    End Class
End Namespace