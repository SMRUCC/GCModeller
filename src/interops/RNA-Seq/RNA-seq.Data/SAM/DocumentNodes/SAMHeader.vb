Imports System.ComponentModel

Namespace SAM.DocumentElements

    ''' <summary>
    ''' 实际上就相当于一个字典来的
    ''' </summary>
    Public Class SAMHeader

        Public Property TAGValues As Dictionary(Of String, String)
        Public Property TAG As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="str">
        ''' Each header line begins with character `@' followed by a two-letter record type code. In the header,
        ''' each line Is TAB-delimited And except the @CO lines, each data field follows a format `TAG:VALUE'
        ''' where TAG Is a two-letter String that defines the content And the format Of VALUE. Each header line
        ''' should match :  /^@[A-Za-z][A-Za-z](\t[A-Za-z][A-Za-z0-9]:[ -~]+)+$/ Or /^@CO\t.*/. Tags
        ''' containing lowercase letters are reserved For End users.
        ''' (每一行都是从@符号开始，后面跟随者两个字母的数据类型码，使用TAB进行分割除了@CO行，每一个域都以键值对的形式出现:  TAG:Value)
        ''' </param>
        Sub New(str As String)
            Dim Tokens As String() = Strings.Split(str, vbTab)
            Me.TAG = Tokens(0)
            Me.TAGValues = (From s As String
                            In Tokens.Skip(1)
                            Let arr As String() = s.Split(":"c)
                            Select key = arr(0), value = arr(1)).ToArray.ToDictionary(Function(obj) obj.key, elementSelector:=Function(obj) obj.value)
        End Sub

        Public Function GenerateDocumentLine() As String
            Return TAG & vbTab & String.Join(vbTab, (From obj In Me.TAGValues Select $"{obj.Key}:{obj.Value}").ToArray)
        End Function

        Public Enum TAGS As Integer
            ''' <summary>
            ''' The header line. The first line if present.
            ''' </summary>
            ''' 
            <Description("The header line. The first line if present.")>
            HD = 0
            ''' <summary>
            ''' Reference sequence dictionary. The order of @SQ lines defines the alignment sorting order.
            ''' </summary>
            ''' 
            <Description("Reference sequence dictionary. The order of @SQ lines defines the alignment sorting order.")>
            SQ
            ''' <summary>
            ''' Read group. Unordered multiple @RG lines are allowed.
            ''' </summary>
            ''' 
            <Description("Read group. Unordered multiple @RG lines are allowed.")>
            RG
            ''' <summary>
            ''' Program.
            ''' </summary>
            ''' 
            <Description("Program.")>
            PG
            ''' <summary>
            ''' One-line text comment. Unordered multiple @CO lines are allowed.
            ''' </summary>
            ''' 
            <Description("One-line text comment. Unordered multiple @CO lines are allowed.")>
            CO
        End Enum

        Const TAGS_STRING = "@HD@SQ@RG@PG@CO"

        Public ReadOnly Property TAGValue As TAGS
            Get
                Dim index As Integer = InStr(TAGS_STRING, Me.TAG) / 3
                Return CType(index, TAGS)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.TAGValue.Description
        End Function
    End Class
End Namespace