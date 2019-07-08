#Region "Microsoft.VisualBasic::c9925084e34019b32beba12f6e7cbce8, WebCloud\SMRUCC.HTTPInternal\AppEngine\API\Methods.vb"

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

    '     Class [GET]
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMethodHelp
    ' 
    '     Class POST
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMethodHelp
    ' 
    '     Class HEAD
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMethodHelp
    ' 
    '     Class PUT
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMethodHelp
    ' 
    '     Class DELETE
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMethodHelp
    ' 
    '     Class TRACE
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMethodHelp
    ' 
    '     Class OPTIONS
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMethodHelp
    ' 
    '     Class CONNECT
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMethodHelp
    ' 
    '     Class PATCH
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMethodHelp
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.Text.Xml

Namespace AppEngine.APIMethods

    ''' <summary>
    ''' GET when you want to retrieve data (GET DATA).
    ''' </summary>
    Public Class [GET] : Inherits APIMethod

        ''' <summary>
        ''' type argument using for Build example json
        ''' </summary>
        ''' <param name="responseExample"></param>
        Sub New(Optional responseExample As Type = Nothing)
            Call MyBase.New(responseExample Or stringType)
        End Sub

        Public Overrides Function GetMethodHelp(EntryPoint As MethodInfo) As String
            Return $"Method: <strong>{GetType([GET]).Name}</strong><br />
{__getParameters(EntryPoint)}<br />
Returns:<br />
<pre>{GetExample()}
</pre>"
        End Function
    End Class

    ''' <summary>
    ''' POST when you want to send data (POST DATA). The POST method requests that the server accept the entity 
    ''' enclosed In the request As a New subordinate Of the web resource identified by the URI. 
    ''' The data POSTed might be, For example, an annotation For existing resources; a message For a bulletin board, 
    ''' newsgroup, mailing list, Or comment thread; a block Of data that Is the result Of submitting a web form 
    ''' To a data-handling process; Or an item To add To a database.[14]
    ''' </summary>
    Public Class POST : Inherits APIMethod

        ''' <summary>
        ''' By default is <see cref="String"/> type.
        ''' </summary>
        ''' <param name="responseExample"></param>
        Sub New(Optional responseExample As Type = Nothing)
            Call MyBase.New(responseExample Or stringType)
        End Sub

        Public Overrides Function GetMethodHelp(EntryPoint As MethodInfo) As String
            Return $"Method: <strong>{GetType(POST).Name}</strong><br />
{__getParameters(EntryPoint)}<br />
Returns:<br />
<pre>{GetExample()}
</pre>"
        End Function
    End Class

    ''' <summary>
    ''' The HEAD method asks For a response identical To that Of a Get request, but without the response body. 
    ''' This Is useful For retrieving meta-information written In response headers, without having To transport the entire content.
    ''' </summary>
    Public Class HEAD : Inherits APIMethod

        Sub New(Optional responseExample As Type = Nothing)
            Call MyBase.New(responseExample Or stringType)
        End Sub

        Public Overrides Function GetMethodHelp(EntryPoint As MethodInfo) As String
            Throw New NotImplementedException()
        End Function
    End Class

    ''' <summary>
    ''' The PUT method requests that the enclosed entity be stored under the supplied URI. If the URI refers 
    ''' To an already existing resource, it Is modified; If the URI does Not point To an existing resource, 
    ''' Then the server can create the resource With that URI.[15]
    ''' </summary>
    Public Class PUT : Inherits APIMethod

        Sub New(Optional responseExample As Type = Nothing)
            Call MyBase.New(responseExample Or stringType)
        End Sub

        Public Overrides Function GetMethodHelp(EntryPoint As MethodInfo) As String
            Return sprintf(<div>
                               Method: <strong><%= GetType(PUT).Name %></strong>
                               <br/>
                               %s<br/>
                               Returns:<br/>
                               <pre>
                                   %s
                               </pre>
                           </div>, __getParameters(EntryPoint), GetExample())
        End Function
    End Class

    ''' <summary>
    ''' The DELETE method deletes the specified resource.
    ''' </summary>
    Public Class DELETE : Inherits APIMethod

        Sub New(Optional responseExample As Type = Nothing)
            Call MyBase.New(responseExample Or stringType)
        End Sub

        Public Overrides Function GetMethodHelp(EntryPoint As MethodInfo) As String
            Throw New NotImplementedException()
        End Function
    End Class

    ''' <summary>
    ''' The TRACE method echoes the received request so that a client can see what (If any) changes Or additions have been made by intermediate servers.
    ''' </summary>
    Public Class TRACE : Inherits APIMethod

        Sub New(Optional responseExample As Type = Nothing)
            Call MyBase.New(responseExample Or stringType)
        End Sub

        Public Overrides Function GetMethodHelp(EntryPoint As MethodInfo) As String
            Throw New NotImplementedException()
        End Function
    End Class

    ''' <summary>
    ''' The OPTIONS method returns the HTTP methods that the server supports For the specified URL. 
    ''' This can be used To check the functionality Of a web server by requesting '*' instead of a specific resource.
    ''' </summary>
    Public Class OPTIONS : Inherits APIMethod

        Sub New(Optional responseExample As Type = Nothing)
            Call MyBase.New(responseExample Or stringType)
        End Sub

        Public Overrides Function GetMethodHelp(EntryPoint As MethodInfo) As String
            Throw New NotImplementedException()
        End Function
    End Class

    ''' <summary>
    ''' The CONNECT method converts the request connection to a transparent TCP/IP tunnel, usually to facilitate 
    ''' SSL-encrypted communication (HTTPS) through an unencrypted HTTP proxy.[17][18] See HTTP CONNECT tunneling.
    ''' </summary>
    Public Class CONNECT : Inherits APIMethod

        Sub New(Optional responseExample As Type = Nothing)
            Call MyBase.New(responseExample Or stringType)
        End Sub

        Public Overrides Function GetMethodHelp(EntryPoint As MethodInfo) As String
            Throw New NotImplementedException()
        End Function
    End Class

    ''' <summary>
    ''' The PATCH method applies Partial modifications To a resource.[19]
    ''' </summary>
    Public Class PATCH : Inherits APIMethod

        Sub New(Optional responseExample As Type = Nothing)
            Call MyBase.New(responseExample Or stringType)
        End Sub

        Public Overrides Function GetMethodHelp(EntryPoint As MethodInfo) As String
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
