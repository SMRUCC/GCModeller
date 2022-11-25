#Region "Microsoft.VisualBasic::cd1e16f35ad8c80bf9c4933bb47dc4f4, GCModeller\core\Bio.Assembly\Assembly\Expasy\NomenclatureDB\Enzyme.vb"

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

    '   Total Lines: 161
    '    Code Lines: 92
    ' Comment Lines: 52
    '   Blank Lines: 17
    '     File Size: 6.68 KB


    '     Class Enzyme
    ' 
    '         Properties: AlternateName, CatalyticActivity, Cofactor, Comments, Description
    '                     Identification, PROSITE, SwissProt
    ' 
    '         Function: __cLines, __createObjectFromText, CreateCatalystActivity, CreateCofactors, CreateProSiteReference
    '                   CreateSwissProtEntries, ToString, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.Expasy.Database

    ''' <summary>
    ''' 使用Uniprot编号为主的酶分类数据记录
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Enzyme : Implements INamedValue

        ''' <summary>
        ''' (ID)  EC编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("EC_ID", Namespace:="http://code.google.com/p/genome-in-code/mapping/expasy")>
        Public Property Identification As String Implements INamedValue.Key

        ''' <summary>
        ''' (DE)  (official name)         
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("FUNCTION_ANNOTATIONS")> Public Property Description As String
        ''' <summary>
        ''' (AN)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AlternateName As String()
        ''' <summary>
        ''' (CA)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CatalyticActivity As String()
        ''' <summary>
        ''' (CF)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Cofactor As String()
        ''' <summary>
        ''' (CC)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Comments As String
        ''' <summary>
        ''' (PR)  Cross-references to PROSITE
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PROSITE As String()
        ''' <summary>
        ''' (DR)  Cross-references to Swiss-Prot 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SwissProt As String()

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", Identification, Description)
        End Function

        Friend Shared Function __createObjectFromText(strData As String()) As Enzyme
            Dim data As KeyValuePair(Of String, String)() = TryParse(strData)
            Dim enzyme As New Enzyme With {
                .Comments = __cLines(data, "CC"),
                .Cofactor = CreateCofactors(data),
                .CatalyticActivity = CreateCatalystActivity(data),
                .Identification = data _
                    .Where(Function(x) String.Equals(x.Key, "ID")) _
                    .First _
                    .Value,
                .Description = __cLines(data, "DE"),
                .SwissProt = CreateSwissProtEntries(data),
                .PROSITE = CreateProSiteReference(data),
                .AlternateName =
                    LinqAPI.Exec(Of String) <=
 _
                    From x As KeyValuePair(Of String, String)
                    In data
                    Where String.Equals(x.Key, "AN")
                    Let strValue As String =
                        Mid(x.Value, 1, Len(x.Value) - 1)
                    Select strValue
            }

            Return enzyme
        End Function

        Private Shared Function CreateProSiteReference(DataChunk As KeyValuePair(Of String, String)()) As String()
            Dim strData As String = __cLines(DataChunk, "PR").Replace("PROSITE; ", "").Replace(" ", "")
            Dim ChunkBuffer As String() = strData.Split(CChar(";"))
            Return ChunkBuffer.Take(ChunkBuffer.Count - 1).ToArray
        End Function

        Private Shared Function CreateSwissProtEntries(DataChunk As KeyValuePair(Of String, String)()) As String()
            Dim ChunkTemp As String() = Strings.Split(__cLines(DataChunk, "DR"), ";")
            Dim LQuery = (From strItem As String In ChunkTemp Select strItem.Split(CChar(",")).First.Trim).ToArray
            Return LQuery
        End Function

        Private Shared Function CreateCatalystActivity(DataChunk As KeyValuePair(Of String, String)()) As String()
            Dim Equations = (From Item In DataChunk Where String.Equals(Item.Key, "CA") Select Regex.Replace(Mid(Item.Value, 1, Len(Item.Value) - 1), "\(\d+\)", "").Trim).ToArray
            Dim LQuery = (From strItem As String In Equations
                          Let strValue As String = Regex.Replace(strItem, "An? ", "", RegexOptions.IgnoreCase)
                          Select Regex.Replace(strValue, " an? ", "", RegexOptions.IgnoreCase)).ToArray
            Return LQuery
        End Function

        Private Shared Function CreateCofactors(DataChunk As KeyValuePair(Of String, String)()) As String()
            Dim strTemp As String = __cLines(DataChunk, "CF")
            If String.IsNullOrEmpty(strTemp) Then
                Return New String() {}
            End If
            strTemp = strTemp.Remove(strTemp.Length - 1, 1)

            Dim ArrayData As String() = Strings.Split(strTemp, " or ")
            Dim List As List(Of String) = New List(Of String)
            For Each strItem As String In ArrayData
                Call List.AddRange(Strings.Split(strItem, " and "))
            Next

            Return List.ToArray
        End Function

        Private Shared Function __cLines(data As KeyValuePair(Of String, String)(), keyword$) As String
            Dim LQuery = (From x In data Where String.Equals(x.Key, keyword) Select x.Value).ToArray
            Dim sb As New StringBuilder(1024)

            For Each strLine As String In LQuery
                Call sb.Append(strLine & " ")
            Next

            Return sb.ToString.Trim
        End Function

        Private Shared Function TryParse(strData As String()) As KeyValuePair(Of String, String)()
            Dim LQuery = (From strItem As String In strData
                          Let strHead As String = Mid(strItem, 1, 2)
                          Let strValue As String = Mid(strItem, 3).Trim
                          Select New KeyValuePair(Of String, String)(strHead, strValue)).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
