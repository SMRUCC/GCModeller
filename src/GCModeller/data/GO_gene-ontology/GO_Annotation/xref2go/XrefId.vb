#Region "Microsoft.VisualBasic::fce96c29bbe1b06ec5443f58b70b314c, G:/GCModeller/src/GCModeller/data/GO_gene-ontology/GO_Annotation//xref2go/XrefId.vb"

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

    '   Total Lines: 266
    '    Code Lines: 177
    ' Comment Lines: 4
    '   Blank Lines: 85
    '     File Size: 6.64 KB


    '     Class XrefId
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Module XrefIdParser
    ' 
    '         Properties: XrefIdParsers
    ' 
    '         Function: Parse
    ' 
    '     Class cog
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class ec
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class egad
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class genprotec
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class hamap
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class interpro
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class kegg
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class metacyc
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class mips
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class multifun
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class pfam
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class pirsf
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class prints
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class prodom
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class prosite
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class reactome
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class rfam
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class rhea
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class smart
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class tigr
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class tigrfams
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class um_bbd_enzymeid
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class um_bbd_pathwayid
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class um_bbd_reactionid
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class uniprotkb_kw
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class uniprotkb_sl
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
