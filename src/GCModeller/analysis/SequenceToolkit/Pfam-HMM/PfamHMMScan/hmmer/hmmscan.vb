#Region "Microsoft.VisualBasic::7c70f35905c8804fba7f795dabc0ee55, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\hmmer\hmmscan.vb"

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

    '     Class hmmscan
    ' 
    '         Properties: HMM, query, Querys, version
    ' 
    '         Function: GetTable, ToString
    ' 
    '     Class Query
    ' 
    '         Properties: Alignments, Hits, length, name, uncertain
    ' 
    '         Function: __getTable, GetTable, ToString
    ' 
    '     Class ScanTable
    ' 
    '         Properties: BestBias, BestEvalue, BestScore, describ, exp
    '                     FullBias, FullEvalue, FullScore, len, model
    '                     name
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetPfamToken, ToString
    ' 
    '     Class Hit
    ' 
    '         Properties: Best, Description, exp, Full, Model
    '                     N
    ' 
    '         Function: ToString
    ' 
    '     Structure Score
    ' 
    '         Properties: bias, Evalue, score
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class Alignment
    ' 
    '         Properties: Aligns, describ, model
    ' 
    '         Function: ToString
    ' 
    '     Class Align
    ' 
    '         Properties: acc, alifrom, aliTo, bias, cEvalue
    '                     envfrom, envTo, hmmfrom, hmmTo, iEvalue
    '                     IsMatched, rank, score
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetPfamToken, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace hmmscan

    ''' <summary>
    ''' hmmscan :: search sequence(s) against a profile database
    ''' </summary>
    Public Class hmmscan

        ''' <summary>
        ''' HMMER 3.1b1 (May 2013); http://hmmer.org/
        ''' </summary>
        ''' <returns></returns>
        Public Property version As String
        ''' <summary>
        ''' query sequence file
        ''' </summary>
        ''' <returns></returns>
        Public Property query As String
        ''' <summary>
        ''' target HMM database
        ''' </summary>
        ''' <returns></returns>
        Public Property HMM As String
        Public Property Querys As Query()

        Public Overrides Function ToString() As String
            Return New With {version, query, HMM}.GetJson
        End Function

        Public Function GetTable() As ScanTable()
            Return LinqAPI.Exec(Of ScanTable) <= From query As Query
                                                 In Querys
                                                 Select result = query.GetTable
        End Function
    End Class

    ''' <summary>
    ''' Scores for complete sequence (score includes all domains)
    ''' </summary>
    Public Class Query

        Public Property name As String
        Public Property length As Integer
        Public Property Hits As Hit()
        ''' <summary>
        ''' ------ inclusion threshold ------
        ''' </summary>
        ''' <returns></returns>
        Public Property uncertain As Hit()
        Public Property Alignments As Alignment()
            Get
                Return __alignments
            End Get
            Set(value As Alignment())
                __alignments = value

                If __alignments.IsNullOrEmpty Then
                    __alignHash = New Dictionary(Of Alignment)
                Else
                    __alignHash = value.ToDictionary
                End If
            End Set
        End Property

        Dim __alignments As Alignment()
        Dim __alignHash As Dictionary(Of Alignment)

        Public Overrides Function ToString() As String
            Return $"{name}  [L={length}]"
        End Function

        Public Function GetTable() As ScanTable()
            Return (LinqAPI.MakeList(Of ScanTable) <=
                From x As Hit
                In uncertain.SafeQuery
                Select __getTable(x)) + Hits.Select(AddressOf __getTable)
        End Function

        Private Function __getTable(x As Hit) As IEnumerable(Of ScanTable)
            Return __alignHash(x.Model).Aligns.Select(Function(o) New ScanTable(Me, x, o))
        End Function
    End Class

    Public Class ScanTable : Inherits Align

        Public Property name As String
        Public Property len As Integer
        <Column(Name:="full.E-value")> Public Property FullEvalue As Double
        <Column(Name:="full.score")> Public Property FullScore As Double
        <Column(Name:="full.bias")> Public Property FullBias As Double
        <Column(Name:="best.E-value")> Public Property BestEvalue As Double
        <Column(Name:="best.score")> Public Property BestScore As Double
        <Column(Name:="best.bias")> Public Property BestBias As Double
        Public Property exp As Double
        Public Property model As String
        Public Property describ As String

        Sub New()
        End Sub

        Sub New(query As Query, hit As Hit, align As Align)
            name = query.name
            len = query.length
            FullEvalue = hit.Full.Evalue
            FullScore = hit.Full.score
            FullBias = hit.Full.bias
            BestBias = hit.Best.bias
            BestEvalue = hit.Best.Evalue
            BestScore = hit.Best.score
            exp = hit.exp
            model = hit.Model
            describ = hit.Description
            MyBase.rank = align.rank
            MyBase.acc = align.acc
            MyBase.alifrom = align.alifrom
            MyBase.aliTo = align.aliTo
            MyBase.bias = align.bias
            MyBase.cEvalue = align.cEvalue
            MyBase.envfrom = align.envfrom
            MyBase.envTo = align.envTo
            MyBase.hmmfrom = align.hmmfrom
            MyBase.hmmTo = align.hmmTo
            MyBase.iEvalue = align.iEvalue
            MyBase.score = align.score
        End Sub

        Public Overloads Function GetPfamToken() As String
            Return MyBase.GetPfamToken(model)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class Hit

        ''' <summary>
        ''' --- full sequence ---
        ''' </summary>
        ''' <returns></returns>
        Public Property Full As Score
        ''' <summary>
        ''' --- best 1 domain ---
        ''' </summary>
        ''' <returns></returns>
        Public Property Best As Score

#Region "-#dom-"

        Public Property exp As Double
        Public Property N As Integer
#End Region

        Public Property Model As String
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Structure Score

        <Column(Name:="E-value")> Public Property Evalue As Double
        Public Property score As Double
        Public Property bias As Double

        Sub New(buf As String())
            Call Me.New(buf(0), buf(1), buf(2))
        End Sub

        Sub New(e As String, s As String, b As String)
            Evalue = Val(e.Trim)
            score = Val(s.Trim)
            bias = Val(b.Trim)
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    Public Class Alignment : Implements INamedValue

        Public Property model As String Implements INamedValue.Key
        Public Property describ As String
        Public Property Aligns As Align()

        Public Overrides Function ToString() As String
            Return $"{model}  {describ}"
        End Function
    End Class

    ''' <summary>
    '''    #    score  bias  c-Evalue  i-Evalue hmmfrom  hmm to    alifrom  ali to    envfrom  env to     acc
    '''  ---   ------ ----- --------- --------- ------- -------    ------- -------    ------- -------    ----
    ''' </summary>
    Public Class Align : Implements IMatched

        <Column(Name:="#")> Public Property rank As String
        Public Property score As Double
        Public Property bias As Double
        <Column(Name:="c-Evalue")> Public Property cEvalue As Double
        <Column(Name:="i-Evalue")> Public Property iEvalue As Double
        Public Property hmmfrom As Integer
        <Column(Name:="hmm To")> Public Property hmmTo As Integer
        Public Property alifrom As Integer
        <Column(Name:="ali To")> Public Property aliTo As Integer
        Public Property envfrom As Integer
        <Column(Name:="env To")> Public Property envTo As Integer
        Public Property acc As Double

        Private ReadOnly Property IsMatched As Boolean Implements IMatched.IsMatched
            Get
                Return rank.Last <> "?"c
            End Get
        End Property

        Public Function GetPfamToken(name As String) As String
            Return $"{name}({alifrom}|{aliTo})"
        End Function

        Friend Sub New(buf As String())
            rank = (buf(1) & buf(2)).Trim
            score = Val(buf(3).Trim)
            bias = Val(buf(5).Trim)
            cEvalue = Val(buf(7).Trim)
            iEvalue = Val(buf(9).Trim)
            hmmfrom = CInt(Val(buf(11).Trim))
            hmmTo = CInt(Val(buf(13).Trim))
            alifrom = CInt(Val(buf(15).Trim))
            aliTo = CInt(Val(buf(17).Trim))
            envfrom = CInt(Val(buf(19).Trim))
            envTo = CInt(Val(buf(21).Trim))
            acc = Val(buf(23).Trim)
        End Sub

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
