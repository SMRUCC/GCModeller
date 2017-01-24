#Region "Microsoft.VisualBasic::1b1f208f20e58477ad8bbbf24c6bbac1, ..\GCModeller\data\STRING\WebAPI\APIModels.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text

Namespace StringDB.WebAPI

    Public MustInherit Class APIToken

        Protected Friend strData As String

        Public MustOverride Function GetToken() As String

        Public Overrides Function ToString() As String
            Return strData
        End Function
    End Class

    Public Class Database : Inherits APIToken

        ''' <summary>
        ''' JSON format either as a list of hashes/dictionaries, or as a plain list (if there is only one value to be returned per record)
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Json As Format = New Format With {
        .strData = "json",
        .Requests = New Request() {
            Request.resolve,
            Request.resolveList,
            Request.abstracts,
            Request.abstractsList
        }
    }

        ''' <summary>
        ''' Tab separated values, with a header line
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly tsv As Format = New Format With {
        .strData = "tsv",
        .Requests = New Request() {
            Request.resolve,
            Request.resolveList,
            Request.abstracts,
            Request.abstractsList,
            Request.actions,
            Request.actionsList,
            Request.interactors,
            Request.interactorsList
        }
    }

        ''' <summary>
        ''' Tab separated values, without header line
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly tsvNoHeader As Format = New Format With {
        .strData = "tsv-no-header",
        .Requests = New Request() {
            Request.resolve,
            Request.resolveList,
            Request.abstracts,
            Request.abstractsList,
            Request.actions,
            Request.actionsList,
            Request.interactors,
            Request.interactorsList
        }
    }
        ''' <summary>
        ''' The interaction network in PSI-MI 2.5 XML format
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly psiMi As Format = New Format With {
        .strData = "psi-mi",
        .Requests = New Request() {
            Request.interactors,
            Request.interactorsList,
            Request.interactions,
            Request.interactionsList
        }
    }
        ''' <summary>
        ''' Tab-delimited form of PSI-MI (similar to tsv, modeled after the IntAct specification. (Easier to parse, but contains less information than the XML format.)
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly psiMiTab As Format = New Format With {
        .strData = "psi-mi-tab",
        .Requests = New Request() {
            Request.interactors,
            Request.interactorsList,
            Request.interactions,
            Request.interactionsList
        }
    }
        ''' <summary>
        ''' The network image
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Image As Format = New Format With {
        .strData = "image",
        .Requests = New Request() {
            Request.network,
            Request.networkList
        }
    }

        Public Overrides Function GetToken() As String
            Return MyBase.strData
        End Function
    End Class

    Public Class Format : Inherits APIToken

        Public Property Requests As Request()

        Public Overrides Function GetToken() As String
            Return MyBase.strData
        End Function
    End Class

    Public Class Request : Inherits APIToken

        Private Sub New()
        End Sub

        ''' <summary>
        ''' List of items that match (in name or identifier) the query item
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly resolve As Request = New Request With {.strData = "resolve"}
        ''' <summary>
        ''' List of items that match (in name or identifier) the query items
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly resolveList As Request = New Request With {.strData = "resolveList"}
        ''' <summary>
        ''' List of abstracts that contain the query item
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly abstracts As Request = New Request With {.strData = "abstracts"}
        ''' <summary>
        ''' List of abstracts that contain any of the query items
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly abstractsList As Request = New Request With {.strData = "abstractsList"}
        ''' <summary>
        ''' List of interaction partners for the query item
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly interactors As Request = New Request With {.strData = "interactors"}
        ''' <summary>
        ''' List of interaction partners for any of the query items
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly interactorsList As Request = New Request With {.strData = "interactorsList"}
        ''' <summary>
        ''' Action partners for the query item
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly actions As Request = New Request With {.strData = "actions"}
        ''' <summary>
        ''' Action partners for any of the query items
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly actionsList As Request = New Request With {.strData = "actionsList"}
        ''' <summary>
        ''' Interaction network in PSI-MI 2.5 format or PSI-MI-TAB format (similar to tsv)
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly interactions As Request = New Request With {.strData = "interactions"}
        ''' <summary>
        ''' Interaction network as above, but for a list of identifiers
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly interactionsList As Request = New Request With {.strData = "interactionsList"}
        ''' <summary>
        ''' The network image for the query item
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly network As Request = New Request With {.strData = "network"}
        ''' <summary>
        ''' The network image for the query items
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly networkList As Request = New Request With {.strData = "networkList"}

        Public MustInherit Class Parameter : Inherits APIToken

            Protected _strValue As String

            Protected MustOverride ReadOnly Property Name As String

            ''' <summary>
            ''' required parameter for single item, e.g. DRD1_HUMAN
            ''' </summary>
            ''' <remarks></remarks>
            Public Class identifier : Inherits Parameter

                Sub New(Id As String)
                    MyBase._strValue = Id
                End Sub

                Protected Overrides ReadOnly Property Name As String
                    Get
                        Return "identifier"
                    End Get
                End Property
            End Class

            ''' <summary>
            ''' required parameter for multiple items, e.g.DRD1_HUMAN%0DDRD2_HUMAN
            ''' </summary>
            ''' <remarks></remarks>
            Public Class identifiers : Inherits Parameter

                Sub New(IdList As String())
                    Dim sBuilder As New StringBuilder(1024)
                    For Each item In IdList
                        Call sBuilder.Append(item & "%0D")
                    Next
                    Call sBuilder.Remove(sBuilder.Length - 3, 3)

                    MyBase._strValue = sBuilder.ToString
                End Sub

                Protected Overrides ReadOnly Property Name As String
                    Get
                        Return "identifiers"
                    End Get
                End Property
            End Class

            ''' <summary>
            ''' + For resolve requests: only-ids get the list of only the STRING identifiers (full by default) 
            ''' + For abstract requests: use colon pmids for alternative shapes of the pubmed identifier
            ''' </summary>
            ''' <remarks></remarks>
            Public Class format : Inherits Parameter

                Protected Overrides ReadOnly Property Name As String
                    Get
                        Return "format"
                    End Get
                End Property
            End Class

            ''' <summary>
            ''' Taxon identifiers (e.g. Human 9606, see: http://www.uniprot.org/taxonomy)
            ''' </summary>
            ''' <remarks></remarks>
            Public Class species : Inherits Parameter

                Protected Overrides ReadOnly Property Name As String
                    Get
                        Return "species"
                    End Get
                End Property
            End Class

            ''' <summary>
            ''' Maximum number of nodes to return, e.g 10.
            ''' </summary>
            ''' <remarks></remarks>
            Public Class limit : Inherits Parameter


                Protected Overrides ReadOnly Property Name As String
                    Get
                        Return "limit"
                    End Get
                End Property
            End Class

            ''' <summary>
            ''' Threshold of significance to include a interaction, a number between 0 and 1000
            ''' </summary>
            ''' <remarks></remarks>
            Public Class required_score : Inherits Parameter


                Protected Overrides ReadOnly Property Name As String
                    Get
                        Return "required_score"
                    End Get
                End Property
            End Class

            ''' <summary>
            ''' Number of additional nodes in network (ordered by score), e.g./ 10
            ''' </summary>
            ''' <remarks></remarks>
            Public Class additional_network_nodes : Inherits Parameter


                Protected Overrides ReadOnly Property Name As String
                    Get
                        Return "additional_network_nodes"
                    End Get
                End Property
            End Class

            ''' <summary>
            ''' The style of edges in the network. evidence for colored multilines. confidence for 
            ''' singled lines where hue correspond to confidence score. (actions for stitch only)
            ''' </summary>
            ''' <remarks></remarks>
            Public Class network_flavor : Inherits Parameter


                Protected Overrides ReadOnly Property Name As String
                    Get
                        Return "network_flavor"
                    End Get
                End Property
            End Class

            ''' <summary>
            ''' Your identifier for us.
            ''' </summary>
            ''' <remarks></remarks>
            Public Class caller_identity : Inherits Parameter

                Protected Overrides ReadOnly Property Name As String
                    Get
                        Return "caller_identity"
                    End Get
                End Property
            End Class

            Public Overrides Function GetToken() As String
                Return String.Format("{0}={1}", Me.Name, Me._strValue)
            End Function
        End Class

        Public Overrides Function GetToken() As String
            Return MyBase.strData
        End Function
    End Class
End Namespace
