Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Keywords

    Public Class Link : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "LINK"
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef links As Link, line As String) As Link
            If links Is Nothing Then
                links = New Link
            End If
            links.str.Append(line)
            Return links
        End Function

    End Class

    Public Class CISPEP : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "CISPEP"
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef CISPEP As CISPEP, line As String) As CISPEP
            If CISPEP Is Nothing Then
                CISPEP = New CISPEP
            End If
            CISPEP.str.Append(line)
            Return CISPEP
        End Function
    End Class

    Public Class HETSYN : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "HETSYN"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef hetatom As HETSYN, str As String) As HETSYN
            If hetatom Is Nothing Then
                hetatom = New HETSYN
            End If
            hetatom.str.Add(str)
            Return hetatom
        End Function

    End Class

    ''' <summary>
    ''' ### PDB文件中的CONECT字段用于明确指定原子间的特殊连接关系
    ''' 
    ''' CONECT记录提供了原子之间除标准共价键以外的特定连接信息，例如二硫键、配位键、氢键（在某些情况下）、盐桥，
    ''' 以及非标准残基（如辅因子、抑制剂）内部的键合关系或它们与蛋白质/核酸之间的连接。
    ''' </summary>
    Public Class CONECT : Inherits Keyword
        Implements Enumeration(Of NamedCollection(Of Integer))

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_CONECT
            End Get
        End Property

        ' CONECT 1198 355 1248 1256 1266 1267 1268
        '
        ' 列 1-6: CONECT(记录类型标识)
        ' 列 7-11: 参考原子的序列号 (例如 1198)
        ' 列 12-16, 17-21, 22-26, 27-31, ...: 与该参考原子相连的最多4个原子的序列号 (例如 355, 1248, 1256, 1266)。如果连接原子超过4个，会使用多个CONECT记录来描述同一个参考原子的所有连接。

        Dim links As New List(Of NamedCollection(Of Integer))

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder

            For Each link In Me.AsEnumerable
                Call sb.AppendLine(link.name & " => " & link.value.GetJson)
            Next

            Return sb.ToString
        End Function

        Friend Shared Function Append(ByRef conect As CONECT, str As String) As CONECT
            If conect Is Nothing Then
                conect = New CONECT
            End If

            Dim tokens As String() = str.StringSplit("\s+")
            Dim ref As String = tokens(0)
            Dim linksTo As Integer() = tokens.Skip(1).AsInteger

            Call conect.links.Add(New NamedCollection(Of Integer)(ref, linksTo))

            Return conect
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of NamedCollection(Of Integer)) Implements Enumeration(Of NamedCollection(Of Integer)).GenericEnumerator
            For Each link As NamedCollection(Of Integer) In links
                Yield link
            Next
        End Function
    End Class

    Public Class MODRES : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "MODRES"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef word As MODRES, str As String) As MODRES
            If word Is Nothing Then
                word = New MODRES
            End If
            word.str.Add(str)
            Return word
        End Function

    End Class

    Public Class SSBOND : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SSBOND"
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef bond As SSBOND, str As String) As SSBOND
            If bond Is Nothing Then
                bond = New SSBOND
            End If
            bond.str.Add(str)
            Return bond
        End Function

    End Class

    Public Class SPRSDE : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SPRSDE"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef sp As SPRSDE, str As String) As SPRSDE
            If sp Is Nothing Then
                sp = New SPRSDE
            End If
            sp.str.Add(str)
            Return sp
        End Function

    End Class

    Public Class CAVEAT : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "CAVEAT"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef cav As CAVEAT, str As String) As CAVEAT
            If cav Is Nothing Then
                cav = New CAVEAT
            End If
            cav.str.Add(str)
            Return cav
        End Function

    End Class

    Public Class MDLTYP : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "MDLTYP"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef type As MDLTYP, str As String) As MDLTYP
            If type Is Nothing Then
                type = New MDLTYP
            End If
            type.str.Add(str)
            Return type
        End Function

    End Class

    Public Class ANISOU : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "ANISOU"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef ani As ANISOU, str As String) As ANISOU
            If ani Is Nothing Then
                ani = New ANISOU
            End If
            ani.str.Add(str)
            Return ani
        End Function

    End Class

    Public Class SIGATM : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SIGATM"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef sig As SIGATM, str As String) As SIGATM
            If sig Is Nothing Then
                sig = New SIGATM
            End If
            sig.str.Add(str)
            Return sig
        End Function

    End Class

    Public Class SIGUIJ : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SIGUIJ"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef sig As SIGUIJ, str As String) As SIGUIJ
            If sig Is Nothing Then
                sig = New SIGUIJ
            End If
            sig.str.Add(str)
            Return sig
        End Function

    End Class

    Public Class SPLIT : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SPLIT"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef split As SPLIT, str As String) As SPLIT
            If split Is Nothing Then
                split = New SPLIT
            End If
            split.str.Add(str)
            Return split
        End Function

    End Class
End Namespace