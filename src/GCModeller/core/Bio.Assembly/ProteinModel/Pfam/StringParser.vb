Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ProteinModel

    Public Module StringParser

        <ExportAPI("FromChouFasman")>
        Public Function FromChouFasman(data As StructDomainObject) As PfamString
            Dim struct As String = data.Name
            struct = Mid(struct, 2, Len(struct) - 2)
            Dim [Structure] As New PfamString With {
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
            Dim lociX As Location = Location.CreateObject(Mid(Loc, 2, Len(Loc) - 2), "|")
            Dim DomainData As ProteinModel.DomainObject =
                New ProteinModel.DomainObject With {
                    .Name = id,
                    .Position = lociX,
                    .Location = New Position(lociX, normlz)
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
            Return String.Format("{0}({1}|{2})", dat.Name, dat.Position.left, dat.Position.right)
        End Function

        <ExportAPI("GET.Domain.Ids"), Extension>
        Public Function CreateDomainID(Domains As IEnumerable(Of ProteinModel.DomainObject)) As String()
            If Domains Is Nothing Then
                Return New String() {}
            End If

            Return (From Domain As ProteinModel.DomainObject
                    In Domains
                    Select String.Format("[{0}: {1}]({2}|{3})", Domain.CommonName, Domain.Title, Domain.Position.left, Domain.Position.right)).ToArray
        End Function

        <ExportAPI("GET.Distributes"), Extension>
        Public Function CreateDistruction(Domains As IEnumerable(Of ProteinModel.DomainObject)) As String()
            If Domains Is Nothing Then
                Return New String() {}
            End If

            Return (From Domain In Domains Select String.Format("{0}({1}|{2})", Domain.Name, Domain.Position.left, Domain.Position.right)).ToArray
        End Function
    End Module
End Namespace