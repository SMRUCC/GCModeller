
Imports Oracle.Java.IO

Namespace net

    ''' <summary>
    ''' Class URL represents a Uniform Resource Locator, a pointer to a "resource" on the World Wide Web. A resource can be something 
    ''' as simple as a file or a directory, or it can be a reference to a more complicated object, such as a query to a database or 
    ''' to a search engine. More information on the types of URLs and their formats can be found at:
    ''' 
    ''' http://www.socs.uts.edu.au/MosaicDocs-old/url-primer.html
    ''' 
    ''' In general, a URL can be broken into several parts. The previous example of a URL indicates that the protocol to use Is http 
    ''' (HyperText Transfer Protocol) And that the information resides on a host machine named www.socs.uts.edu.au. The information 
    ''' on that host machine Is named /MosaicDocs-old/url-primer.html. The exact meaning of this name on the host machine Is both 
    ''' protocol dependent And host dependent. The information normally resides in a file, but it could be generated on the fly. 
    ''' This component of the URL Is called the path component.
    ''' 
    ''' A URL can optionally specify a "port", which Is the port number To which the TCP connection Is made On the remote host machine. 
    ''' If the port Is Not specified, the Default port For the protocol Is used instead. For example, the Default port For http Is 80. 
    ''' An alternative port could be specified As
    ''' 
    ''' http://www.socs.uts.edu.au:80/MosaicDocs-old/url-primer.html
    ''' 
    ''' The syntax Of URL Is defined by RFC 2396: Uniform Resource Identifiers (URI): Generic Syntax, amended by RFC 2732: Format for 
    ''' Literal IPv6 Addresses in URLs. The Literal IPv6 address format also supports scope_ids. The syntax And usage of scope_ids 
    ''' Is described here.
    ''' 
    ''' A URL may have appended To it a "fragment", also known As a "ref" Or a "reference". The fragment Is indicated by the sharp sign 
    ''' character "#" followed by more characters. For example,
    ''' 
    ''' http://java.sun.com/index.html#chapter1
    ''' 
    ''' This fragment Is Not technically part Of the URL. Rather, it indicates that after the specified resource Is retrieved, the application 
    ''' Is specifically interested In that part Of the document that has the tag chapter1 attached To it. The meaning Of a tag Is resource 
    ''' specific.
    ''' 
    ''' An application can also specify a "relative URL", which contains only enough information To reach the resource relative To another URL. 
    ''' Relative URLs are frequently used within HTML pages. For example, If the contents Of the URL
    ''' 
    ''' http://java.sun.com/index.html
    ''' 
    ''' contained within it the relative URL:
    '''   FAQ.html
    ''' 
    ''' it would be a shorthand For
    ''' http://java.sun.com/FAQ.html
    ''' 
    ''' The relative URL need Not specify all the components Of a URL. If the protocol, host name, Or port number Is missing, the value Is 
    ''' inherited from the fully specified URL. The file component must be specified. The Optional fragment Is Not inherited.
    ''' 
    ''' The URL Class does Not itself encode Or decode any URL components according To the escaping mechanism defined In RFC2396. It Is the 
    ''' responsibility Of the caller To encode any fields, which need To be escaped prior To calling URL, And also To decode any escaped 
    ''' fields, that are returned from URL. Furthermore, because URL has no knowledge Of URL escaping, it does Not recognise equivalence 
    ''' between the encoded Or decoded form Of the same URL. For example, the two URLs
    ''' http://foo.com/hello world/ And http://foo.com/hello%20world
    ''' would be considered Not equal To Each other.
    ''' Note, the URI class does perform escaping of its component fields in certain circumstances. The recommended way to manage the encoding 
    ''' And decoding of URLs Is to use URI, And to convert between these two classes using toURI() And URI.toURL().
    ''' 
    ''' The URLEncoder And URLDecoder classes can also be used, but only For HTML form encoding, which Is Not the same As the encoding scheme defined In RFC2396.
    ''' </summary>  
    <Serializable> Public Class URL

        ''' <summary>
        ''' Creates a URL Object from the specified protocol, host, port number, And file.
        ''' host can be expressed As a host name Or a literal IP address. If IPv6 literal address Is used, it should be enclosed In square brackets ('[' and ']'), as specified by RFC 2732; However, the literal IPv6 address format defined in RFC 2373: IP Version 6 Addressing Architecture is also accepted.
        ''' 
        ''' Specifying a port number Of -1 indicates that the URL should use the Default port For the protocol.
        ''' 
        ''' If this Is the first URL Object being created With the specified protocol, a stream protocol handler Object, an instance Of Class URLStreamHandler, Is created For that protocol
        ''' 
        ''' If the application has previously Set up an instance Of URLStreamHandlerFactory As the stream handler factory, Then the createURLStreamHandler method Of that instance Is called With the protocol String As an argument To create the stream protocol handler.
        ''' If no URLStreamHandlerFactory has yet been Set up, Or If the factory's createURLStreamHandler method returns null, then the constructor finds the value of the system property:
        '''    java.protocol.handler.pkgs
        ''' 
        '''      If the value Of that system Property Is Not null, it Is interpreted As a list Of packages separated by a vertical slash character '|'. The constructor tries to load the class named:
        '''    &lt;package>.&lt;protocol>.Handler
        '''  
        ''' where &lt;package> Is replaced by the name of the package And &lt;protocol> Is replaced by the name of the protocol. If this class does Not exist, Or if the class exists but it Is Not a subclass of URLStreamHandler, then the next package in the list Is tried.
        ''' If the previous Step fails To find a protocol handler, Then the constructor tries To load from a system Default package.
        '''   &lt;System Default package>.&lt;protocol>.Handler
        '''  
        '''      If this Class does Not exist, Or If the Class exists but it Is Not a subclass Of URLStreamHandler, Then a MalformedURLException Is thrown.
        ''' Protocol handlers For the following protocols are guaranteed To exist On the search path :-
        ''' 
        '''   http, https, ftp, file, And jar
        ''' 
        ''' Protocol handlers For additional protocols may also be available.
        ''' No validation Of the inputs Is performed by this constructor.
        ''' </summary>  
        ''' <param name="protocol">the name of the protocol to use.</param>
        ''' <param name="host">the name of the host.</param>
        ''' <param name="port">the port number on the host.</param>
        ''' <param name="file">the file on the host</param>
        Sub New(protocol As String,
            host As String,
            port As Integer,
           file As String)

        End Sub

        ''' <summary>
        ''' Creates a URL from the specified protocol name, host name, And file name. The Default port For the specified protocol Is used.
        ''' This method Is equivalent To calling the four-argument constructor With the arguments being protocol, host, -1, And file. No validation Of the inputs Is performed by this constructor.
        ''' </summary>
        ''' <param name="protocol">the name of the protocol to use.</param>
        ''' <param name="host">the name of the host.</param>
        ''' <param name="file">the file on the host.</param>
        Sub New(protocol As String,
                  host As String,
                  file As String)

        End Sub

        ''' <summary>
        ''' Creates a URL Object from the specified protocol, host, port number, file, And handler. Specifying a port number Of -1 indicates that the URL should use the Default port For the protocol. Specifying a handler Of null indicates that the URL should use a Default stream handler For the protocol, As outlined For: java.net.URL#URL(java.lang.String, java.lang.String, int, java.lang.String)
        ''' If the handler Is Not null And there Is a security manager, the security manager's checkPermission method is called with a NetPermission("specifyStreamHandler") permission. This may result in a SecurityException. No validation of the inputs is performed by this constructor.
        ''' </summary>
        ''' <param name="protocol">the name of the protocol to use.</param>
        ''' <param name="host">the name of the host.</param>
        ''' <param name="port">the port number on the host.</param>
        ''' <param name="file">the file on the host</param>
        ''' <param name="handler">the stream handler for the URL.</param>
        Sub New(protocol As String,
             host As String,
           port As Integer,
            file As String,
           handler As URLStreamHandler)

        End Sub

        ''' <summary>
        ''' Creates a URL Object from the String representation.
        ''' This constructor Is equivalent To a Call To the two-argument constructor With a null first argument.
        ''' </summary>
        ''' <param name="spec">the String to parse as a URL.</param>
        Sub New(spec As String)

        End Sub

        ''' <summary>
        ''' Creates a URL by parsing the given spec within a specified context. The New URL Is created from the given context URL And the spec argument As described In RFC2396 "Uniform Resource Identifiers : Generic * Syntax" 
        ''' &lt;scheme> :  //&lt;authority>&lt;path>?&lt;query>#&lt;fragment>
        '''
        ''' The reference Is parsed into the scheme, authority, path, query And fragment parts. If the path component Is empty And the scheme, authority, And query components are undefined, Then the New URL Is a reference To the current document. Otherwise, the fragment And query parts present In the spec are used In the New URL.
        ''' If the scheme component Is defined In the given spec And does Not match the scheme Of the context, Then the New URL Is created As an absolute URL based On the spec alone. Otherwise the scheme component Is inherited from the context URL.
        '''
        ''' If the authority component Is present In the spec Then the spec Is treated As absolute And the spec authority And path will replace the context authority And path. If the authority component Is absent In the spec Then the authority Of the New URL will be inherited from the context.
        '''
        ''' If the spec's path component begins with a slash character "/" then the path is treated as absolute and the spec path replaces the context path.
        '''
        ''' Otherwise, the path Is treated as a relative path And Is appended to the context path, as described in RFC2396. Also, in this case, the path Is canonicalized through the removal of directory changes made by occurences of ".." And ".".
        '''
        ''' For a more detailed description of URL parsing, refer To RFC2396.
        ''' </summary>
        ''' <param name="context">the context in which to parse the specification.</param>
        ''' <param name="spec">the String to parse as a URL.</param>
        Sub New(context As URL,
            spec As String)

        End Sub

        ''' <summary>
        ''' Creates a URL by parsing the given spec With the specified handler within a specified context. If the handler Is null, the parsing occurs As With the two argument constructor.
        ''' </summary>
        ''' <param name="context">the context in which to parse the specification.</param>
        ''' <param name="spec">the String to parse as a URL.</param>
        ''' <param name="handler">the stream handler for the URL.</param>
        Sub New(context As URL,
           spec As String,
          handler As URLStreamHandler)

        End Sub

        ''' <summary>
        ''' Sets the fields of the URL. This is not a public method so that only URLStreamHandlers can modify URL fields. URLs are otherwise constant.
        ''' </summary>
        ''' <param name="protocol">the name of the protocol to use</param>
        ''' <param name="host">the name of the host</param>
        ''' <param name="port">the port number on the host</param>
        ''' <param name="file">the file on the host</param>
        ''' <param name="Ref">the internal reference in the URL</param>
        Protected Sub [set](protocol As [String], host As [String], port As Integer, file As [String], ref As [String])
        End Sub


        ''' <summary>
        ''' Sets the specified 8 fields of the URL. This is not a public method so that only URLStreamHandlers can modify URL fields. URLs are otherwise constant.
        ''' </summary>
        ''' <param name="protocol">the name of the protocol to use</param>
        ''' <param name="host">the name of the host</param>
        ''' <param name="port">the port number on the host</param>
        ''' <param name="authority">the authority part for the url</param>
        ''' <param name="userInfo">the username and password</param>
        ''' <param name="path">the file on the host</param>
        ''' <param name="query">the query part of this URL</param>
        ''' <param name="ref">the internal reference in the URL</param>
        Protected Sub [set](protocol As [String], host As [String], port As Integer, authority As [String], userInfo As [String], path As [String],
        query As [String], ref As [String])
        End Sub


        ''' <summary>
        ''' Gets the query part of this URL.
        ''' </summary>
        ''' <returns>the query part of this URL, or null if one does not exist</returns>
        Public Function getQuery() As [String]
        End Function

        ''' <summary>
        ''' Gets the path part of this URL.
        ''' </summary>
        ''' <returns>the path part of this URL, or an empty string if one does not exist</returns>
        Public Function getPath() As [String]
        End Function


        ''' <summary>
        ''' Gets the userInfo part of this URL.
        ''' </summary>
        ''' <returns>the userInfo part of this URL, or null if one does not exist</returns>
        Public Function getUserInfo() As [String]
        End Function


        ''' <summary>
        ''' Gets the authority part of this URL.
        ''' </summary>
        ''' <returns>the authority part of this URL</returns>
        Public Function getAuthority() As [String]
        End Function

        ''' <summary>
        ''' Gets the port number of this URL.
        ''' </summary>
        ''' <returns>the port number, or -1 if the port is not set</returns>
        Public Function getPort() As Integer
        End Function

        ''' <summary>
        ''' Gets the default port number of the protocol associated with this URL. If the URL scheme or the URLStreamHandler for the URL do not define a default port number, then -1 is returned.
        ''' </summary>
        ''' <returns>the port number</returns>
        Public Function getDefaultPort() As Integer
        End Function

        ''' <summary>
        ''' Gets the protocol name of this URL.
        ''' </summary>
        ''' <returns>the protocol of this URL.</returns>
        Public Function getProtocol() As [String]
        End Function

        ''' <summary>
        ''' Gets the host name of this URL, if applicable. The format of the host conforms to RFC 2732, i.e. for a literal IPv6 address, this method will return the IPv6 address enclosed in square brackets ('[' and ']').
        ''' </summary>
        ''' <returns>the host name of this URL.</returns>
        Public Function getHost() As [String]
        End Function

        ''' <summary>
        ''' Gets the file name of this URL. The returned file portion will be the same as getPath(), plus the concatenation of the value of getQuery(), if any. If there is no query portion, this method and getPath() will return identical results.
        ''' </summary>
        ''' <returns>the file name of this URL, or an empty string if one does not exist</returns>
        Public Function getFile() As [String]
        End Function

        ''' <summary>
        ''' Gets the anchor (also known as the "reference") of this URL.
        ''' </summary>
        ''' <returns>the anchor (also known as the "reference") of this URL, or null if one does not exist</returns>
        Public Function getRef() As [String]
        End Function

        ''' <summary>
        ''' Compares this URL for equality with another object.
        ''' If the given object is not a URL then this method immediately returns false.
        '''
        ''' Two URL objects are equal if they have the same protocol, reference equivalent hosts, have the same port number on the host, and the same file and fragment of the file.
        '''
        ''' Two hosts are considered equivalent if both host names can be resolved into the same IP addresses; else if either host name can't be resolved, the host names must be equal without regard to case; or both host names equal to null.
        '''
        ''' Since hosts comparison requires name resolution, this operation is a blocking operation.
        '''
        ''' Note: The defined behavior for equals is known to be inconsistent with virtual hosting in HTTP.
        ''' </summary>
        ''' <param name="obj">the URL to compare against.</param>
        ''' <returns>true if the objects are the same; false otherwise.</returns>
        Public Overrides Function equals(obj As [Object]) As Boolean
        End Function

        ''' <summary>
        ''' Creates an integer suitable for hash table indexing.
        ''' The hash code is based upon all the URL components relevant for URL comparison.As such, this operation is a blocking operation.
        ''' </summary>
        ''' <returns>a hash code for this URL.</returns>
        Public Function hashCode() As Integer
        End Function

        ''' <summary>
        ''' Compares two URLs, excluding the fragment component.
        ''' Returns true if this URL and the other argument are equal without taking the fragment component into consideration.
        ''' </summary>
        ''' <param name="other">the URL to compare against.</param>
        ''' <returns>true if they reference the same remote object; false otherwise.</returns>
        Public Function sameFile(other As URL) As Boolean
        End Function

        ''' <summary>
        ''' Constructs a string representation of this URL. The string is created by calling the toExternalForm method of the stream protocol handler for this object.
        ''' </summary>
        ''' <returns>a string representation of this object.</returns>
        Public Function toExternalForm() As [String]
        End Function

        ''' <summary>
        ''' Returns a URI equivalent to this URL. This method functions in the same way as new URI (this.toString()).
        ''' Note, any URL instance that complies with RFC 2396 can be converted to a URI.However, some URLs that are not strictly in compliance can not be converted to a URI.
        ''' </summary>
        ''' <returns>a URI instance equivalent to this URL.</returns>
        Public Function toURI() As Uri
        End Function

        ''' <summary>
        ''' Returns a URLConnection object that represents a connection to the remote object referred to by the URL.
        ''' A new connection is opened every time by calling the openConnection method of the protocol handler for this URL.
        '''
        ''' If for the URL's protocol (such as HTTP or JAR), there exists a public, specialized URLConnection subclass belonging to one of the following packages or one of their subpackages: java.lang, java.io, java.util, java.net, the connection returned will be of that subclass. For example, for HTTP an HttpURLConnection will be returned, and for JAR a JarURLConnection will be returned.
        ''' </summary>
        ''' <returns>a URLConnection to the URL.</returns>
        Public Function openConnection() As URLConnection
        End Function

        ''' <summary>
        ''' Same as openConnection(), except that the connection will be made through the specified proxy; Protocol handlers that do not support proxing will ignore the proxy parameter and make a normal connection. Calling this method preempts the system's default ProxySelector settings.
        ''' </summary>
        ''' <param name="proxy">the Proxy through which this connection will be made. If direct connection is desired, Proxy.NO_PROXY should be specified.</param>
        ''' <returns>a URLConnection to the URL.</returns>
        Public Function openConnection(proxy As Proxy) As URLConnection
        End Function

        ''' <summary>
        ''' Opens a connection to this URL and returns an InputStream for reading from that connection. This method is a shorthand for:
        ''' openConnection().getInputStream()
        ''' </summary>
        ''' <returns>an input stream for reading from the URL connection.</returns>
        Public Function openStream() As InputStream
        End Function

        ''' <summary>
        ''' Gets the contents of this URL. This method is a shorthand for:
        ''' openConnection().getContent()
        ''' </summary>   
        ''' <returns>the contents of this URL.</returns>
        Public Function getContent() As [Object]
        End Function

        ''' <summary>
        ''' Gets the contents of this URL. This method is a shorthand for:
        '''   openConnection().getContent(Class[])
        ''' </summary>
        ''' <param name="classes">an array of Java types</param>
        ''' <returns>the content object of this URL that is the first match of the types specified in the classes array. null if none of the requested types are supported.</returns>
        Public Function getContent(classes As [Class]()) As [Object]
        End Function

        ''' <summary>
        ''' Sets an application's URLStreamHandlerFactory. This method can be called at most once in a given Java Virtual Machine.
        ''' The URLStreamHandlerFactory instance is used to construct a stream protocol handler from a protocol name.
        '''
        ''' If there is a security manager, this method first calls the security manager's checkSetFactory method to ensure the operation is allowed. This could result in a SecurityException.
        ''' </summary>
        ''' <param name="fac">the desired factory.</param>
        Public Shared Sub setURLStreamHandlerFactory(fac As URLStreamHandlerFactory)
        End Sub

    End Class

    Public Class URLStreamHandlerFactory
    End Class

    Public Class Proxy
    End Class

    Public Class URLConnection
        Public contentLength As Long
    End Class

    ''' <summary>
    ''' The abstract class URLStreamHandler is the common superclass for all stream protocol handlers. A stream protocol handler knows how to make a connection for a particular protocol type, such as http, ftp, or gopher.
    ''' In most cases, an instance of a URLStreamHandler subclass is not created directly by an application.Rather, the first time a protocol name is encountered when constructing a URL, the appropriate stream protocol handler is automatically loaded.
    ''' </summary>
    Public MustInherit Class URLStreamHandler
        ''' <summary>
        ''' Opens a connection to the object referenced by the URL argument.This method should be overridden by a subclass.
        ''' If for the handler's protocol (such as HTTP or JAR), there exists a public, specialized URLConnection subclass belonging to one of the following packages or one of their subpackages: java.lang, java.io, java.util, java.net, the connection returned will be of that subclass. For example, for HTTP an HttpURLConnection will be returned, and for JAR a JarURLConnection will be returned.
        ''' </summary>
        ''' <param name="u">the URL that this connects to.</param>
        ''' <returns>a URLConnection object for the URL.</returns>
        Protected MustOverride Function openConnection(u As URL) As URLConnection

        ''' <summary>
        ''' Same as openConnection(URL), except that the connection will be made through the specified proxy; Protocol handlers that do not support proxying will ignore the proxy parameter and make a normal connection.Calling this method preempts the system's default ProxySelector settings.
        ''' </summary>
        ''' <param name="u">the URL that this connects to.</param>
        ''' <param name="p">the proxy through which the connection will be made.If direct connection is desired, Proxy.NO_PROXY should be specified.</param>
        ''' <returns>a URLConnection object for the URL.</returns>
        Protected Function openConnection(u As URL, p As Proxy) As URLConnection
        End Function

        ''' <summary>
        ''' Parses the string representation of a URL into a URL object.
        ''' If there is any inherited context, then it has already been copied into the URL argument.
        '''
        ''' The parseURL method of URLStreamHandler parses the string representation as if it were an http specification.Most URL protocol families have a similar parsing. A stream protocol handler for a protocol that has a different syntax must override this routine.
        ''' </summary>
        ''' <param name="u">the URL to receive the result of parsing the spec.</param>
        ''' <param name="spec">the String representing the URL that must be parsed.</param>
        ''' <param name="start">the character index at which to begin parsing. This is just past the ':' (if there is one) that specifies the determination of the protocol name.</param>
        ''' <param name="limit">the character position to stop parsing at. This is the end of the string or the position of the "#" character, if present.All information after the sharp sign indicates an anchor.</param>
        Protected Sub parseURL(u As URL, spec As [String], start As Integer, limit As Integer)
        End Sub

        ''' <summary>
        ''' Returns the default port for a URL parsed by this handler.This method is meant to be overidden by handlers with default port numbers.
        ''' </summary>
        ''' <returns>the default port for a URL parsed by this handler.</returns>
        Protected Function getDefaultPort() As Integer
        End Function

        ''' <summary>
        ''' Provides the default equals calculation.May be overidden by handlers for other protocols that have different requirements for equals(). This method requires that none of its arguments is null. This is guaranteed by the fact that it is only called by java.net.URL class.
        ''' </summary>
        ''' <param name="u1">a URL object</param>
        ''' <param name="u2">a URL object</param>
        ''' <returns>true if the two urls are considered equal, ie. they refer to the same fragment in the same file.</returns>
        Protected Overloads Function equals(u1 As URL, u2 As URL) As [Boolean]
        End Function

        ''' <summary>
        ''' Provides the default hash calculation.May be overidden by handlers for other protocols that have different requirements for hashCode calculation.
        ''' </summary>
        ''' <param name="u">a URL object</param>
        ''' <returns>an int suitable for hash table indexing</returns>
        Protected Function hashCode(u As URL) As Integer
        End Function

        ''' <summary>
        ''' Compare two urls to see whether they refer to the same file, i.e., having the same protocol, host, port, and path.This method requires that none of its arguments is null. This is guaranteed by the fact that it is only called indirectly by java.net.URL class.
        ''' </summary>
        ''' <param name="u1">a URL object</param>
        ''' <param name="u2">a URL object</param>
        ''' <returns>true if u1 and u2 refer to the same file</returns>
        Protected Function sameFile(u1 As URL, u2 As URL) As [Boolean]
        End Function

        ''' <summary>
        ''' Get the IP address of our host.An empty host field or a DNS failure will result in a null return.
        ''' </summary>
        ''' <param name="u">a URL object</param>
        ''' <returns>an InetAddress representing the host IP address.</returns>
        Protected Function getHostAddress(u As URL) As InetAddress
        End Function

        ''' <summary>
        ''' Compares the host components of two URLs.
        ''' </summary>
        ''' <param name="u1">the URL of the first host to compare</param>
        ''' <param name="u2">the URL of the second host to compare</param>
        ''' <returns>true if and only if they are equal, false otherwise.</returns>
        Protected Function hostsEqual(u1 As URL, u2 As URL) As [Boolean]
        End Function

        ''' <summary>
        ''' Converts a URL of a specific protocol to a String.
        ''' </summary>
        ''' <param name="u">the URL.</param>
        ''' <returns>a string representation of the URL argument.</returns>
        Protected Function toExternalForm(u As URL) As [String]
        End Function

        ''' <summary>
        ''' Sets the fields of the URL argument to the indicated values.Only classes derived from URLStreamHandler are supposed to be able to call the set method on a URL.
        ''' </summary>
        ''' <param name="u">the URL to modify.</param>
        ''' <param name="protocol">the protocol name.</param>
        ''' <param name="host">the remote host value for the URL.</param>
        ''' <param name="port">the port on the remote machine.</param>
        ''' <param name="authority">the authority part for the URL.</param>
        ''' <param name="userInfo">the userInfo part of the URL.</param>
        ''' <param name="path">the path component of the URL.</param>
        ''' <param name="query">the query part for the URL.</param>
        ''' <param name="ref">the reference.</param>
        Protected Sub setURL(u As URL, protocol As [String], host As [String], port As Integer, authority As [String], userInfo As [String],
        path As [String], query As [String], Ref As [String])
        End Sub

        ''' <summary>
        ''' Sets the fields of the URL argument to the indicated values.Only classes derived from URLStreamHandler are supposed to be able to call the set method on a URL.
        ''' </summary>
        ''' <param name="u">the URL to modify.</param>
        ''' <param name="protocol">the protocol name.This value is ignored since 1.2.</param>
        ''' <param name="host">the remote host value for the URL.</param>
        ''' <param name="port">the port on the remote machine.</param>
        ''' <param name="file">the file.</param>
        ''' <param name="ref">the reference.</param>
        Protected Sub setURL(u As URL, protocol As [String], host As [String], port As Integer, file As [String], Ref As [String])
        End Sub
    End Class

    Public Class InetAddress
    End Class
End Namespace