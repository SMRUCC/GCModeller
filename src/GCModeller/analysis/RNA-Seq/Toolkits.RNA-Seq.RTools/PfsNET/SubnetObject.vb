#Region "Microsoft.VisualBasic::d3d9ebec55dabf684ff3d7cb48efb474, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\PfsNET\SubnetObject.vb"

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

    '     Class PfsNET
    ' 
    '         Properties: [Class], Flag, Identifier, n, SubNET
    '                     Vectors
    ' 
    '         Function: ToPFSNet, ToString, TryParse
    ' 
    '     Structure Vector
    ' 
    '         Properties: x
    ' 
    '         Function: ToString
    ' 
    '     Class NetDetails
    ' 
    '         Properties: Nodes, Pvalue, statistics, Vector, weight
    '                     weight2
    ' 
    '         Function: ToString, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.PFSNet

Namespace PfsNET

    ''' <summary>
    ''' 输出结果的XML文件
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PfsNET : Implements INamedValue

        <XmlAttribute> Public Property Identifier As String Implements INamedValue.Key

        <XmlAttribute> Public Property n As Integer
        <XmlAttribute> Public Property Flag As Boolean

        <XmlArray("PfsNET.Vectors")> Public Property Vectors As Vector()
        Public Property SubNET As NetDetails
        ''' <summary>
        ''' 1表示第一种类别，2表示第二种类型，只有这两种值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property [Class] As String

        Public Const LIST_ITEM As String = "[^,^(]+?=list\(\d+,(FALSE|TRUE),.+?,list\(\)\)\)"

        Public Shared Function TryParse(strData As String) As PfsNET
            Dim uId As String = Regex.Match(strData, ".+?=list\(\d+,(FALSE|TRUE),").Value
            strData = strData.Replace(uId, "")

            Dim strTemp As String = Regex.Match(uId, "list\(\d+,(FALSE|TRUE),", RegexOptions.IgnoreCase).Value
            uId = Regex.Split(uId, "=").First.Replace("`", "").Trim.Split.Last

            Dim n As Integer = Val(Regex.Match(strTemp, "\d+").Value)
            Dim f As Boolean = Regex.Match(strTemp, "TRUE|FALSE", RegexOptions.IgnoreCase).Value.ParseBoolean
            strData = strData.Replace(strTemp, "")
            strTemp = Regex.Match(strData, "c\(\d+(,\d+)*\)(,c\(\d+(,\d+)*\))*,").Value
            strData = strData.Replace(strTemp, "")

            Dim Vectors As Vector() =
                LinqAPI.Exec(Of Vector) <= From strLine As String
                                           In Regex.Matches(strTemp, "c\(\d+(,\d+)*\)", RegexOptions.IgnoreCase).ToArray
                                           Select New Vector With {
                                               .x = Regex.Matches(strLine, "-?\d+(\.\d+)?").ToArray(AddressOf Val)
                                           }
            Dim subnet As NetDetails = NetDetails.TryParse(strData)

            Return New PfsNET With {
                    .Identifier = uId,
                    .n = n,
                    .Flag = f,
                    .Vectors = Vectors,
                    .SubNET = subnet
                }
        End Function

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        Public Function ToPFSNet() As DataStructure.PFSNetGraph
            Dim PFSNet As New DataStructure.PFSNetGraph With {
                .Id = Me.Identifier,
                .masked = Me.Flag,
                .pvalue = Me.SubNET.Pvalue,
                .statistics = Me.SubNET.statistics
            }

            PFSNet.Nodes =
            LinqAPI.Exec(Of DataStructure.PFSNetGraphNode) <=
 _
                From i As Integer
                In SubNET.Nodes.Sequence
                Let node = New DataStructure.PFSNetGraphNode With {
                    .Name = Me.SubNET.Nodes(i),
                    .weight = Me.SubNET.weight.x(i),
                    .weight2 = Me.SubNET.weight2.x(i)
                }
                Select node

            Return PFSNet
        End Function
    End Class

    Public Structure Vector

        <XmlAttribute> Public Property x As Double()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    Public Class NetDetails

        Public Property Vector As Vector
        <XmlAttribute> Public Property statistics As Double
        <XmlAttribute("P.value")> Public Property Pvalue As Double
        <XmlArray("PfsNET.Nodes")> Public Property Nodes As String()
        <XmlElement("PfsNET.Weights")> Public Property weight As Vector
        <XmlElement("PfsNET.Weight2")> Public Property weight2 As Vector

        Public Shared Function TryParse(strData As String) As NetDetails
            Dim strTemp As String = Regex.Match(strData, "list\(c\(-?\d+(\.\d+)?(,-?\d+(\.\d+)?)*\),", RegexOptions.IgnoreCase).Value
            strData = strData.Replace(strTemp, "")
            Dim Subnet As NetDetails = New NetDetails
            Subnet.Vector = New Vector With {.x = (From n As Match In Regex.Matches(strTemp, "-?\d+(\.\d+)?") Select Val(n.Value)).ToArray}
            strTemp = Regex.Match(strData, "list\(statistics=-?\d+(\.\d+)?,p.value=-?\d+(\.\d+)?\),").Value
            Subnet.statistics = Val(Regex.Match(strTemp, "statistics=-?\d+(\.\d+)?").Value.Split(CChar("=")).Last.Trim)
            Subnet.Pvalue = Val(Regex.Match(strTemp, "p.value=-?\d+(\.\d+)?").Value.Split(CChar("=")).Last.Trim)
            strData = strData.Replace(strTemp, "")
            strTemp = Regex.Match(strData, "name=c\("".+?""(,"".+?"")*\),").Value
            Subnet.Nodes = (From m As Match In Regex.Matches(strTemp, """.+?""") Select m.Value.Replace("""", "")).ToArray
            strData = strData.Replace(strTemp, "")
            strTemp = Regex.Match(strData, "weight=c\(-?\d+(\.\d+)?(,-?\d+(\.\d+)?)*\),").Value
            strData = strData.Replace(strTemp, "")
            Subnet.weight = New Vector With {.x = (From m As Match In Regex.Matches(strTemp, "-?\d+(\.\d+)?") Select Val(m.Value)).ToArray}

            strTemp = Regex.Match(strData, "weight2=c\(-?\d+(\.\d+)?(,-?\d+(\.\d+)?)*\)").Value.Split(CChar("=")).Last
            Subnet.weight2 = New Vector With {.x = (From m As Match In Regex.Matches(strTemp, "-?\d+(\.\d+)?") Select Val(m.Value)).ToArray}

            Return Subnet
        End Function

        Public Overrides Function ToString() As String
            Return String.Join(", ", Nodes)
        End Function
    End Class
End Namespace
