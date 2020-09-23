#Region "Microsoft.VisualBasic::d363fd85796c70f450c7020265d8a892, CLI_tools\MEME\Cli\MotifSimilarity\MotifDraws.vb"

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

    ' Module MotifDraws
    ' 
    '     Properties: FamilyMotifs
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) CreateViews
    ' 
    ' Class MotifSWTViews
    ' 
    '     Properties: [Class], Category, Hit, Locus, Motif
    '                 MotifId, Name, Query, SW, Type
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery

Public Module MotifDraws

    ''' <summary>
    ''' 按照家族分类的Motif计算模型
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property FamilyMotifs As Dictionary(Of String, AnnotationModel())

    Sub New()
        FamilyMotifs = (From x In Motifs
                        Let Family As String = x.Key.Split("."c).First
                        Select x.Value,
                            Family
                        Group By Family Into Group) _
                            .ToDictionary(Function(x) x.Family,
                                          Function(x) x.Group.Select(Function(xx) xx.Value).ToArray)
    End Sub

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="Footprints">{Module, Families}</param>
    ''' <param name="models">{Module, Model}</param>
    ''' <returns></returns>
    Public Function CreateViews(Footprints As Dictionary(Of String, String()),
                                models As Dictionary(Of AnnotationModel),
                                pheno As ModuleClassAPI,
                                EXPORT As String) As Integer
        Dim LQuery = (From x In Footprints
                      Let model = models(x.Key)
                      Let subs = x.Value.Select(Function(name) FamilyMotifs(name))
                      Select model, subs, x).ToArray
        Dim Views = (From view In (From x In LQuery.AsParallel
                                   Select x.model.CreateViews(x.subs.ToVector, x.x.Key, pheno, EXPORT)).IteratesALL
                     Select view
                     Order By view.MotifId Ascending).ToArray

        Dim html As StringBuilder = New StringBuilder
        Call html.AppendLine("<table>")
        Call html.AppendLine("<tr>
<td>{MotifId}</td>
<td>{Locus}</td>
<td>{Motif}</td>
<td>{Hit}</td>
<td>{SW}/></td>
<td>{Query}/></td>
</tr>")
        For Each line In Views
            Call html.AppendLine(line.ToString)
        Next

        Call html.AppendLine("</table>")
        Call Views.SaveTo(EXPORT & "/Views.Csv")
        Return html.SaveTo(EXPORT & "/Views.html").CLICode
    End Function

    <Extension>
    Public Function CreateViews(query As AnnotationModel, subs As AnnotationModel(), trace As String, modCat As ModuleClassAPI, EXPORT As String) As MotifSWTViews()
        Dim swTOM_OUT As Output() = (From x In subs
                                     Let result = SWTom.Compare(query, x, bitsLevel:=2, minW:=6, cutoff:=0.3, tomThreshold:=0.3)
                                     Where Not result Is Nothing AndAlso Not result.HSP.IsNullOrEmpty
                                     Select result).ToArray
        Dim list As New List(Of MotifSWTViews)
        Dim queryMotif As String = EXPORT & query.Uid.NormalizePathString & "/query.png"
        Dim locus As String = query.Sites.Select(Function(x) x.Name).JoinBy(", ")
        Dim modX As String = Strings.Split(trace, "::").Last.Split("."c).First
        Dim A As String = Nothing, B As String = Nothing, C As String = Nothing
        Dim modObj = modCat.GetBriteInfo(modX, A, B, C)
        Dim name As String = modCat.GetName(modObj)

        For Each x As Output In swTOM_OUT
            Dim img As String = EXPORT & "/" & query.Uid.NormalizePathString & "/" & x.Subject.Uid.NormalizePathString
            For Each hsp In x.HSP
                Dim id As String = hsp.ToString.NormalizePathString
                Dim path As String = img & "." & id & ".png"

                Call hsp.Visual.SaveAs(path, ImageFormats.Png)
                Call path.__DEBUG_ECHO

                list += New MotifSWTViews With {
                    .MotifId = trace.Split(":"c).Last,
                    .Motif = hsp.Query.Motif,
                    .Locus = locus,
                    .Query = queryMotif,
                    .SW = path,
                    .Hit = x.Subject.Uid & $" ({hsp.Subject.Motif})",
                    .Type = A,
                    .Class = B,
                    .Category = C,
                    .Name = name
                }
            Next
        Next

        Call SequenceLogoAPI.DrawLogo(query).Save(queryMotif)

        Return list.ToArray
    End Function
End Module

Public Class MotifSWTViews
    Public Property MotifId As String
    Public Property Locus As String
    Public Property Name As String
    Public Property Type As String
    Public Property [Class] As String
    Public Property Category As String
    Public Property Motif As String
    Public Property Hit As String
    Public Property SW As String
    Public Property Query As String

    Public Overrides Function ToString() As String
        Return $"<tr>
<td>{MotifId}</td>
<td>{Locus}</td>
<td>{Type }</td>
<td>{[Class] }</td>
<td>{Category}</td>
<td>{Motif}</td>
<td>{Hit}</td>
<td><img src=""{SW}""/></td>
<td><img src=""{Query}""/></td>
</tr>"
    End Function
End Class
