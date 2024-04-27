#Region "Microsoft.VisualBasic::318c51362dfdb6a2e26374d7584a1e46, G:/GCModeller/src/GCModeller/data/Xfam/Pfam//Pipeline/PfamString/API.vb"

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

    '   Total Lines: 219
    '    Code Lines: 174
    ' Comment Lines: 19
    '   Blank Lines: 26
    '     File Size: 10.18 KB


    '     Module API
    ' 
    '         Function: Analysis, CLIParser, CreateDistruction, CreateDomainID, CreateObject
    '                   CreatePfamString, CreateProteinDescription, FromChouFasman, GenerateData, GetDomain
    '                   getDomainModel, getDomainTrace, ToPfamStringToken
    ' 
    '         Sub: ExportEvolgeniusView
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.ProteinModel

Namespace PfamString

    <Package("PfamString.API")>
    Public Module API

        <ExportAPI("FromChouFasman")>
        Public Function FromChouFasman(data As DomainObject) As PfamString
            Dim struct As String = data.Name
            struct = Mid(struct, 2, Len(struct) - 2)
            Dim [Structure] As PfamString =
                New PfamString With {
                    .ProteinId = data.Id_Handle,
                    .Description = data.Name,
                    .Length = data.Position.FragmentSize,
                    .PfamString = (From i As Integer In struct.Sequence Let c As Char = struct(i) Select String.Format("{0}({1}|{2})", c.ToString, i + 1, i + 2)).ToArray}
            [Structure].Domains = (From c As Char In struct.ToArray Select CStr(c) Distinct).ToArray

            Return [Structure]
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="strValue"></param>
        ''' <param name="normlz">一般是蛋白质的序列长度</param>
        ''' <returns></returns>
        Private Function getDomainModel(strValue As String, normlz As Integer) As ProteinModel.DomainObject
            Dim Loc As String = Regex.Match(strValue, "\(\d+\|\d+\)").Value
            Dim id As String = strValue.Replace(Loc, "")
            Dim lociX As Loci.Location = Loci.Location.CreateObject(Mid(Loc, 2, Len(Loc) - 2), "|")
            Dim DomainData As ProteinModel.DomainObject =
                New ProteinModel.DomainObject With {
                    .Name = id,
                    .Position = lociX,
                    .Location = New Loci.Position(lociX, normlz)
            }
            Return DomainData
        End Function

        Friend Function getDomainTrace(strValue As String, normlz As Integer) As ProteinModel.DomainObject
            Try
                Return getDomainModel(strValue, normlz)
            Catch ex As Exception
                ex = New Exception(strValue, ex)
                Throw ex
            End Try
        End Function

        ''' <summary>
        ''' <see cref="ProteinModel.DomainObject.Name"/>
        ''' </summary>
        ''' <param name="token"></param>
        ''' <returns></returns>
        '''
        <ExportAPI("GET.Domain")>
        Public Function GetDomain(token As String) As ProteinModel.DomainObject
            Return getDomainTrace(token, 1)
        End Function

        <ExportAPI("Pfam.Token"), Extension>
        Public Function ToPfamStringToken(dat As SMRUCC.genomics.ProteinModel.DomainObject) As String
            Return String.Format("{0}({1}|{2})", dat.Name, dat.Position.Left, dat.Position.Right)
        End Function

        ''' <summary>
        ''' &lt;ID>:&lt;Length>:&lt;pfam-string>
        ''' </summary>
        ''' <param name="view"></param>
        ''' <returns></returns>
        '''
        <ExportAPI("Parser.CLI")>
        Public Function CLIParser(view As String) As PfamString
            Dim Tokens As String() = view.Split(":"c)
            Dim ID As String = Tokens(Scan0)
            Dim Length As Integer = CInt(Val(Tokens(1)))
            Dim Pfam As String() = Tokens(2).Split("+"c)
            Return New PfamString With {
                .ProteinId = ID,
                .Length = Length,
                .PfamString = Pfam,
                .Domains = Pfam,
                .Description = "Auto Generated Data"
            }
        End Function

        <ExportAPI("EvolgeniusView")>
        Public Sub ExportEvolgeniusView(BLASTOutput As BlastPlus.v228, SavedFile As String)
            Dim LQuery = (From Query As BlastPlus.Query
                          In BLASTOutput.Queries
                          Let item = New KeyValuePair(Of Long, Protein)(Query.QueryLength, CreateProteinDescription(Query))
                          Select item
                          Order By item.Value.ID Ascending).ToArray

            Dim strData As String() = (From Protein In LQuery Select GenerateData(Protein)).ToArray
            Call IO.File.WriteAllLines(SavedFile, strData)
        End Sub

        Private Function GenerateData(Protein As KeyValuePair(Of Long, Protein)) As String
            Dim sBuilder As StringBuilder = New StringBuilder(2048)
            Call sBuilder.Append(String.Format("{0}    {1} ", Protein.Value.ID, Protein.Key))
            For Each Domain In Protein.Value.Domains
                Call sBuilder.Append(String.Format("{0},{1},{2},Pfam-A,,,{3},{4},{5}   ", Domain.Position.Left, Domain.Position.Right, Domain.CommonName, Domain.Name, Domain.EValue, Domain.BitScore))
            Next

            Return sBuilder.ToString
        End Function

        <ExportAPI("Analysis"), Extension>
        Public Function Analysis(BLASTOutput As BlastPlus.v228) As Protein()
            Dim LQuery = (From Query In BLASTOutput.Queries Select CreateProteinDescription(Query)).ToArray
            Return LQuery
        End Function

        <ExportAPI("ToPfamString"), Extension>
        Public Function CreatePfamString(BLASTOutput As BlastPlus.v228) As PfamString()
            Dim LQuery = (From Query In BLASTOutput.Queries.AsParallel
                          Let Protein = CreateProteinDescription(Query)
                          Let item = New KeyValuePair(Of Long, Protein)(Query.QueryLength, Protein)
                          Select item
                          Order By item.Value.ID Ascending).ToArray
            Dim ChunkBuffer = (From Protein In LQuery
                               Select New PfamString With {
                                   .ProteinId = Protein.Value.ID,
                                   .Length = Protein.Key,
                                   .Description = Protein.Value.Description,
                                   .PfamString = CreateDistruction(Protein.Value.Domains),
                                   .Domains = CreateDomainID(Protein.Value.Domains)}).ToArray
            Return ChunkBuffer
        End Function

        <ExportAPI("GET.Domain.Ids"), Extension>
        Public Function CreateDomainID(Domains As IEnumerable(Of ProteinModel.DomainObject)) As String()
            If Domains Is Nothing Then
                Return New String() {}
            End If

            Return (From Domain As ProteinModel.DomainObject
                    In Domains
                    Select String.Format("[{0}: {1}]({2}|{3})", Domain.CommonName, Domain.Title, Domain.Position.Left, Domain.Position.Right)).ToArray
        End Function

        <ExportAPI("GET.Distributes"), Extension>
        Public Function CreateDistruction(Domains As IEnumerable(Of ProteinModel.DomainObject)) As String()
            If Domains Is Nothing Then
                Return New String() {}
            End If

            Return (From Domain In Domains Select String.Format("{0}({1}|{2})", Domain.Name, Domain.Position.Left, Domain.Position.Right)).ToArray
        End Function

        <ExportAPI("Description")>
        Public Function CreateProteinDescription(QueryIteration As BlastPlus.Query) As Protein
            Dim UniqueId As String = QueryIteration.QueryName.Split.First
            Dim Description As String

            If String.Equals(UniqueId, QueryIteration.QueryName, StringComparison.OrdinalIgnoreCase) Then
                Description = ""
            Else
                Description = Mid(QueryIteration.QueryName, Len(UniqueId) + 1).Trim
            End If

            If QueryIteration.SubjectHits.IsNullOrEmpty Then
                Return New Protein With {
                    .ID = UniqueId,
                    .Domains = New SMRUCC.genomics.ProteinModel.DomainObject() {},
                    .SequenceData = "",
                    .Description = Description
                }
            Else
                Dim LQuery = From Hit As BlastPlus.SubjectHit
                             In QueryIteration.SubjectHits
                             Where Hit.Length / Val(Hit.LengthHit) > 0.85 AndAlso System.Math.Abs(Hit.LengthHit - Hit.LengthQuery) < 20
                             Let smp = CreateObject(Hit.Name.Replace("gnl|CDD|", ""))
                             Select New SMRUCC.genomics.ProteinModel.DomainObject(smp) With {
                                 .Position = New ComponentModel.Loci.Location() With {
                                 .Left = Val(Hit.Hsp.First.Query.Left),
                                 .Right = Val(Hit.Hsp.Last.Query.Right)},
                                 .BitScore = Hit.Score.RawScore,
                                 .EValue = Hit.Score.Expect} '

                Dim Domains = LQuery.ToArray
                Dim Protein As New Protein With {
                    .ID = UniqueId,
                    .Domains = Domains,
                    .SequenceData = "",
                    .Description = Description
                }
                Return Protein
            End If
        End Function

        Private Function CreateObject(strData As String) As SmpFile
            Dim Tokens As String() = Strings.Split(strData, "|")
            Dim SmpFile As CDD.SmpFile = New SmpFile With {.Id = Tokens.First}
            Dim p As i32 = 1

            SmpFile.Name = Tokens.GetItem(++p)
            SmpFile.CommonName = Tokens.GetItem(++p)
            'SmpFile.Title = Tokens.GetItem(p.MoveNext)
            p += 1
            SmpFile.Describes = Tokens.GetItem(++p)
            SmpFile.Title = SmpFile.Describes

            Return SmpFile
        End Function
    End Module
End Namespace
