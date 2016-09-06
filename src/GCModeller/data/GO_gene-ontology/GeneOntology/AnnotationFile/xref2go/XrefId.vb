Imports System.Runtime.CompilerServices

Namespace xref2go

    Public MustInherit Class XrefId

        ''' <summary>
        ''' Parsing from raw string
        ''' </summary>
        ''' <param name="xrefId"></param>
        Sub New(xrefId As String)

        End Sub
    End Class

    Public Module XrefIdParser

        Public ReadOnly Property XrefIdParsers As IReadOnlyDictionary(Of XrefIdTypes, Func(Of String, XrefId)) =
            New Dictionary(Of XrefIdTypes, Func(Of String, XrefId)) From {
 _
            {XrefIdTypes.cog, Function(s) New cog(s)},
            {XrefIdTypes.ec, Function(s) New ec(s)},
            {XrefIdTypes.egad, Function(s) New egad(s)},
            {XrefIdTypes.genprotec, Function(s) New genprotec(s)},
            {XrefIdTypes.hamap, Function(s) New hamap(s)},
            {XrefIdTypes.interpro, Function(s) New interpro(s)},
            {XrefIdTypes.kegg, Function(s) New kegg(s)},
            {XrefIdTypes.metacyc, Function(s) New metacyc(s)},
            {XrefIdTypes.mips, Function(s) New mips(s)},
            {XrefIdTypes.multifun, Function(s) New multifun(s)},
            {XrefIdTypes.pfam, Function(s) New pfam(s)},
            {XrefIdTypes.pirsf, Function(s) New pirsf(s)},
            {XrefIdTypes.prints, Function(s) New prints(s)},
            {XrefIdTypes.prodom, Function(s) New prodom(s)},
            {XrefIdTypes.prosite, Function(s) New prosite(s)},
            {XrefIdTypes.reactome, Function(s) New reactome(s)},
            {XrefIdTypes.rfam, Function(s) New rfam(s)},
            {XrefIdTypes.rhea, Function(s) New rhea(s)},
            {XrefIdTypes.smart, Function(s) New smart(s)},
            {XrefIdTypes.tigr, Function(s) New tigr(s)},
            {XrefIdTypes.tigrfams, Function(s) New tigrfams(s)},
            {XrefIdTypes.um_bbd_enzymeid, Function(s) New um_bbd_enzymeid(s)},
            {XrefIdTypes.um_bbd_pathwayid, Function(s) New um_bbd_pathwayid(s)},
            {XrefIdTypes.um_bbd_reactionid, Function(s) New um_bbd_reactionid(s)},
            {XrefIdTypes.uniprotkb_kw, Function(s) New uniprotkb_kw(s)},
            {XrefIdTypes.uniprotkb_sl, Function(s) New uniprotkb_sl(s)}
        }

        <Extension>
        Public Function Parse(Of uid As XrefId)(raw As String, parser As XrefIdTypes) As uid
            If _XrefIdParsers.ContainsKey(parser) Then
                Return DirectCast(_XrefIdParsers(parser)(raw), uid)
            Else
                Return Nothing
            End If
        End Function
    End Module

    Public Class cog : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class ec : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class egad : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class genprotec : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class hamap : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class interpro : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class kegg : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class metacyc : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class mips : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class multifun : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class pfam : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class pirsf : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class prints : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class prodom : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class prosite : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class reactome : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class rfam : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class rhea : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class smart : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class tigr : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class tigrfams : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class um_bbd_enzymeid : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class um_bbd_pathwayid : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class um_bbd_reactionid : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class uniprotkb_kw : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class

    Public Class uniprotkb_sl : Inherits XrefId

        Public Sub New(xrefId As String)
            MyBase.New(xrefId)

        End Sub
    End Class
End Namespace